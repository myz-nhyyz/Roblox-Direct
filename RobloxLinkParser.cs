// language: C#, file: RobloxLinkParser.cs
using System;
using System.Text.RegularExpressions;

namespace RobloxDirect
{
    public sealed class RobloxLinkException : Exception
    {
        public RobloxLinkException(string message) : base(message) { }
    }

    public static class RobloxLinkParser
    {
        private static readonly Regex GameIdRegex = new Regex(
            @"/games/(\d+)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static string ToRobloxUri(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new RobloxLinkException("Hãy dán một link Roblox trước.");

            Uri uri;
            if (!Uri.TryCreate(input.Trim(), UriKind.Absolute, out uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                throw new RobloxLinkException("Link không hợp lệ. Hãy dùng link bắt đầu bằng https://roblox.com.");

            string host = uri.Host.ToLowerInvariant();
            if (host != "roblox.com" && !host.EndsWith(".roblox.com", StringComparison.Ordinal))
                throw new RobloxLinkException("Đây không phải là link Roblox được hỗ trợ.");

            Match gameMatch = GameIdRegex.Match(uri.AbsolutePath);
            if (gameMatch.Success)
                return "roblox://experiences/start?placeId=" + gameMatch.Groups[1].Value;

            string code = GetQueryValue(uri, "code");
            if (!string.IsNullOrWhiteSpace(code))
            {
                string type = GetQueryValue(uri, "type") ?? "Server";
                return "roblox://navigation/share_links?code=" + Uri.EscapeDataString(code) +
                       "&type=" + Uri.EscapeDataString(type);
            }

            throw new RobloxLinkException("Không tìm thấy place ID hoặc invite code trong link này.");
        }

        private static string GetQueryValue(Uri uri, string key)
        {
            foreach (string pair in uri.Query.TrimStart('?').Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] parts = pair.Split(new[] { '=' }, 2);
                if (parts.Length == 2 &&
                    string.Equals(Uri.UnescapeDataString(parts[0]), key, StringComparison.OrdinalIgnoreCase))
                {
                    return Uri.UnescapeDataString(parts[1].Replace('+', ' '));
                }
            }
            return null;
        }
    }
}