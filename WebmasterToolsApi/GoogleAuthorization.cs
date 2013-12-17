using System;
using System.Collections.Generic;
using System.Reflection;
using WebmasterToolsApi.Http;

namespace WebmasterToolsApi
{
    /// <summary>
    /// Provides a simple facade for fetching a authorization token
    /// </summary>
    public class GoogleAuthorization
    {
        public const string AuthorizationUrl = "https://www.google.com/accounts/ClientLogin";

        /// <summary>
        /// Fetch a authorization token
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string GetAuthorizationToken(string username, string password)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"Email", username},
                    {"Passwd", password},
                    {"accountType", "GOOGLE"},
                    {"service", "sitemaps"},
                    {"source", Assembly.GetExecutingAssembly().GetName().Name}
                };

            var response = HttpClient.Post(AuthorizationUrl, parameters);
            if (string.IsNullOrEmpty(response))
                throw new Exception("Authorization exception");

            var token = response.Remove(0, response.IndexOf("Auth=", StringComparison.Ordinal) + "Auth=".Length);
            return token;
        }
    }
}