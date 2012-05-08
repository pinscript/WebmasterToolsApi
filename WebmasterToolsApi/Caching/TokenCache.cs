using System;
using System.IO;

namespace WebmasterToolsApi.Caching {
    public static class TokenCache {
        public const int TokenValidMinutes = 60*24*7; // 7 days

        /// <summary>
        /// Retrieves a token stored in a file
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <param name="miss">Function invoked on a cache miss </param>
        /// <returns>A valid token</returns>
        public static string GetToken(string path, Func<string> miss) {
            if (File.Exists(path)) {
                // Check token age
                var info = new FileInfo(path);
                if ((DateTime.Now - info.CreationTime).Minutes < TokenValidMinutes)
                    return File.ReadAllText(path);
            }

            var token = miss();
            WriteToken(path, token);
            return token;
        }

        /// <summary>
        /// Writes the token to cache
        /// </summary>
        /// <param name="path"></param>
        /// <param name="token"></param>
        public static void WriteToken(string path, string token) {
            if (File.Exists(path))
                File.Delete(path);

            File.WriteAllText(path, token);
        }
    }
}