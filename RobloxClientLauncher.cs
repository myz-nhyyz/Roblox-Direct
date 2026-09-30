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
        private static readonly ClientDefinition[] Definitions = new[]
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

            foreach (ClientDefinition definition in Definitions)
            {
                string executablePath = FindExecutable(definition);
                if (executablePath != null)
                    clients.Add(new RobloxClient(definition.Id, definition.DisplayName, executablePath));
            }
            return clients;
        }

        public static RobloxClient FindById(string id)
        {
            ClientDefinition definition = Definitions.FirstOrDefault(item =>
                string.Equals(item.Id, id, StringComparison.OrdinalIgnoreCase));
            if (definition == null)
            {
                return string.Equals(id, "default", StringComparison.OrdinalIgnoreCase)
                    ? new RobloxClient("default", "Roblox thường", null)
                    : null;
            }

            string executablePath = FindExecutable(definition);
            return executablePath == null
                ? null
                : new RobloxClient(definition.Id, definition.DisplayName, executablePath);
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
    }
}