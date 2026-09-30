// language: C#, file: RobloxClientLauncher.cs
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace RobloxDirect
{
    public sealed class RobloxClient
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string ExecutablePath { get; set; }

        public RobloxClient(string id, string displayName, string executablePath)
        {
            Id = id;
            DisplayName = displayName;
            ExecutablePath = executablePath;
        }
    }

    public static class RobloxClientLauncher
    {
        private static readonly ClientDefinition[] KnownDefinitions = new[]
        {
            new ClientDefinition("voidstrap", "Voidstrap",
                new[] { "Voidstrap", "Voidtrap" },
                new[] { "Voidstrap.exe", "Voidtrap.exe" }),
            new ClientDefinition("fishstrap", "Fishstrap",
                new[] { "Fishstrap", "Fishtrap" },
                new[] { "Fishstrap.exe", "Fishtrap.exe" }),
            new ClientDefinition("bloxstrap", "Bloxstrap",
                new[] { "Bloxstrap", "Bloxtrap" },
                new[] { "Bloxstrap.exe", "Bloxtrap.exe" })
        };

        public static IReadOnlyList<RobloxClient> DetectClients()
        {
            var clients = new List<RobloxClient> { new RobloxClient("default", "Roblox thường", null) };
            var discoveredPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (ClientDefinition definition in KnownDefinitions)
            {
                string executablePath = FindExecutable(definition);
                if (executablePath != null)
                {
                    clients.Add(new RobloxClient(definition.Id, definition.DisplayName, executablePath));
                    discoveredPaths.Add(Path.GetFullPath(executablePath));
                }
            }

            foreach (InstalledClient installedClient in GetRegisteredClients())
            {
                foreach (string executablePath in GetInstalledExecutables(installedClient.InstallLocation))
                {
                    if (!IsRobloxClientExecutable(executablePath, installedClient.DisplayName))
                        continue;

                    string fullPath = Path.GetFullPath(executablePath);
                    if (!discoveredPaths.Add(fullPath))
                        continue;

                    string displayName = GetClientDisplayName(executablePath, installedClient.DisplayName);
                    clients.Add(new RobloxClient("installed:" + fullPath, displayName, fullPath));
                }
            }

            return clients;
        }

        public static RobloxClient FindById(string id)
        {
            return DetectClients().FirstOrDefault(client =>
                string.Equals(client.Id, id, StringComparison.OrdinalIgnoreCase));
        }

        public static void Launch(RobloxClient client, string robloxUri)
        {
            ProcessStartInfo startInfo = client.ExecutablePath == null
                ? new ProcessStartInfo(robloxUri) { UseShellExecute = true }
                : new ProcessStartInfo(client.ExecutablePath, QuoteArgument(robloxUri)) { UseShellExecute = true };

            Process.Start(startInfo);
        }

        private static string FindExecutable(ClientDefinition definition)
        {
            foreach (string path in GetCandidatePaths(definition))
            {
                if (File.Exists(path)) return path;
            }
            return null;
        }

        private static List<InstalledClient> GetRegisteredClients()
        {
            var installedClients = new List<InstalledClient>();
            RegistryHive[] hives = { RegistryHive.CurrentUser, RegistryHive.LocalMachine };
            RegistryView[] views = { RegistryView.Registry64, RegistryView.Registry32 };

            foreach (RegistryHive hive in hives)
            {
                foreach (RegistryView view in views)
                {
                    try
                    {
                        using (RegistryKey baseKey = RegistryKey.OpenBaseKey(hive, view))
                        using (RegistryKey uninstallKey = baseKey.OpenSubKey(
                            "Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall"))
                        {
                            if (uninstallKey == null) continue;

                            foreach (string subKeyName in uninstallKey.GetSubKeyNames())
                            {
                                using (RegistryKey appKey = uninstallKey.OpenSubKey(subKeyName))
                                {
                                    if (appKey == null) continue;

                                    string displayName = appKey.GetValue("DisplayName") as string ?? string.Empty;
                                    string installLocation = appKey.GetValue("InstallLocation") as string;
                                    if (string.IsNullOrWhiteSpace(installLocation) ||
                                        !Directory.Exists(installLocation))
                                        continue;

                                    installedClients.Add(new InstalledClient(displayName, installLocation));
                                }
                            }
                        }
                    }
                    catch (UnauthorizedAccessException) { }
                    catch (IOException) { }
                    catch (System.Security.SecurityException) { }
                }
            }

            return installedClients;
        }

        private static IEnumerable<string> GetInstalledExecutables(string installLocation)
        {
            var executablePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                foreach (string path in Directory.GetFiles(installLocation, "*.exe", SearchOption.TopDirectoryOnly))
                    executablePaths.Add(path);

                foreach (string subdirectory in Directory.GetDirectories(installLocation))
                {
                    foreach (string path in Directory.GetFiles(subdirectory, "*.exe", SearchOption.TopDirectoryOnly))
                        executablePaths.Add(path);
                }
            }
            catch (UnauthorizedAccessException) { }
            catch (IOException) { }

            return executablePaths;
        }

        private static bool IsRobloxClientExecutable(string executablePath, string registeredName)
        {
            string executableName = Path.GetFileNameWithoutExtension(executablePath);
            if (ContainsAny(executableName, "uninstall", "unins", "setup", "installer", "updater", "studio"))
                return false;

            bool relatedName = ContainsAny(registeredName, "roblox", "strap");
            bool launcherName = ContainsAny(executableName, "roblox", "player", "launcher", "bootstrap", "client");
            if (relatedName && launcherName)
                return true;

            try
            {
                FileVersionInfo version = FileVersionInfo.GetVersionInfo(executablePath);
                string metadata = (version.ProductName ?? string.Empty) + " " +
                    (version.FileDescription ?? string.Empty) + " " +
                    (version.InternalName ?? string.Empty);
                return metadata.IndexOf("roblox", StringComparison.OrdinalIgnoreCase) >= 0 &&
                    metadata.IndexOf("studio", StringComparison.OrdinalIgnoreCase) < 0;
            }
            catch (IOException)
            {
                return false;
            }
        }

        private static string GetClientDisplayName(string executablePath, string registeredName)
        {
            try
            {
                FileVersionInfo version = FileVersionInfo.GetVersionInfo(executablePath);
                if (!string.IsNullOrWhiteSpace(version.ProductName) &&
                    version.ProductName.IndexOf("roblox", StringComparison.OrdinalIgnoreCase) < 0)
                    return version.ProductName;
            }
            catch (IOException) { }

            if (!string.IsNullOrWhiteSpace(registeredName))
                return registeredName;

            return Path.GetFileNameWithoutExtension(executablePath);
        }

        private static bool ContainsAny(string value, params string[] terms)
        {
            return terms.Any(term =>
                value.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private static IEnumerable<string> GetCandidatePaths(ClientDefinition definition)
        {
            string[] roots = new string[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
            };

            foreach (string root in roots.Where(path => !string.IsNullOrWhiteSpace(path)))
            {
                foreach (string directoryName in definition.DirectoryNames)
                {
                    foreach (string executableName in definition.ExecutableNames)
                    {
                        yield return Path.Combine(root, directoryName, executableName);
                        yield return Path.Combine(root, "Programs", directoryName, executableName);
                        yield return Path.Combine(root, directoryName, "Versions", executableName);
                    }
                }
            }

            foreach (RegistryHive hive in new[] { RegistryHive.CurrentUser, RegistryHive.LocalMachine })
            {
                foreach (RegistryView view in new[] { RegistryView.Registry64, RegistryView.Registry32 })
                {
                    using (RegistryKey uninstallKey = RegistryKey.OpenBaseKey(hive, view)
                        .OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall"))
                    {
                        if (uninstallKey == null) continue;

                        foreach (string subKeyName in uninstallKey.GetSubKeyNames())
                        {
                            using (RegistryKey appKey = uninstallKey.OpenSubKey(subKeyName))
                            {
                                string displayName = appKey != null && appKey.GetValue("DisplayName") != null
                                    ? appKey.GetValue("DisplayName") as string ?? string.Empty
                                    : string.Empty;

                                if (!definition.DirectoryNames.Any(name =>
                                    displayName.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0))
                                    continue;

                                string installLocation = appKey != null
                                    ? appKey.GetValue("InstallLocation") as string
                                    : null;
                                if (string.IsNullOrWhiteSpace(installLocation)) continue;

                                foreach (string executableName in definition.ExecutableNames)
                                    yield return Path.Combine(installLocation, executableName);
                            }
                        }
                    }
                }
            }
        }

        private static string QuoteArgument(string value)
        {
            return "\"" + value.Replace("\"", "\\\"") + "\"";
        }

        private sealed class ClientDefinition
        {
            public string Id { get; set; }
            public string DisplayName { get; set; }
            public string[] DirectoryNames { get; set; }
            public string[] ExecutableNames { get; set; }

            public ClientDefinition(string id, string displayName, string[] dirs, string[] exes)
            {
                Id = id;
                DisplayName = displayName;
                DirectoryNames = dirs;
                ExecutableNames = exes;
            }
        }

        private sealed class InstalledClient
        {
            public string DisplayName { get; }
            public string InstallLocation { get; }

            public InstalledClient(string displayName, string installLocation)
            {
                DisplayName = displayName;
                InstallLocation = installLocation;
            }
        }
    }
}