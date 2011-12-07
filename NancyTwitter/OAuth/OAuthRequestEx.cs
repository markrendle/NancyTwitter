namespace NancyTwitter.OAuth
{
    using System;
    using System.Net;

    public static class OAuthRequestEx
    {
        public static string UploadString(this WebClient webClient, Uri uri, string body, OAuthParameterSet parameterSet)
        {
            webClient.Headers.Set("Authorization", parameterSet.GetOAuthHeaderString(uri, "POST"));
            return webClient.UploadString(uri, body);
        }

        public static string DownloadString(this WebClient webClient, Uri uri, OAuthParameterSet parameterSet)
        {
            webClient.Headers.Set("Authorization", parameterSet.GetOAuthHeaderString(uri, "GET"));
            return webClient.DownloadString(uri);
        }

// ReSharper disable InconsistentNaming
        public static void DownloadStringAsync(this WebClient webClient, Uri uri, OAuthParameterSet parameterSet)
// ReSharper restore InconsistentNaming
        {
            webClient.Headers.Set("Authorization", parameterSet.GetOAuthHeaderString(uri, "GET"));
            webClient.DownloadStringAsync(uri);
        }

// ReSharper disable InconsistentNaming
        public static void UploadStringAsync(this WebClient webClient, Uri uri, string body, OAuthParameterSet parameterSet)
// ReSharper restore InconsistentNaming
        {
            webClient.Headers.Set("Authorization", parameterSet.GetOAuthHeaderString(uri, "POST"));
            webClient.UploadStringAsync(uri, body);
        }

        public static string UrlEncode(this string source)
        {
            return Uri.EscapeDataString(source);
        }

        static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long ToUnixTime(this DateTime target)
        {
            return (long)(target - UnixEpoch).TotalSeconds;
        }
    }
}