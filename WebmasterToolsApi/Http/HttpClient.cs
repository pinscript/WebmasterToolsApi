using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace WebmasterToolsApi.Http
{
    public class HttpClient
    {
        /// <summary>
        /// Turn debug on/off
        /// </summary>
        public static bool Debug = false;

        /// <summary>
        /// Issues a HTTP GET
        /// </summary>
        /// <param name="url">The url</param>
        /// <param name="headers">Optional headers</param>
        /// <returns>The response HTML</returns>
        public static string Get(string url, Dictionary<string, object> headers = null)
        {
            var req = (HttpWebRequest) WebRequest.Create(url);

            if (headers != null && headers.Any())
            {
                foreach (var header in headers)
                {
                    req.Headers.Add(header.Key, header.Value.ToString());
                }
            }

            if (Debug)
                Console.WriteLine(req.RequestUri.ToString());

            using (var response = (HttpWebResponse) req.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var sr = new StreamReader(responseStream))
            {
                return sr.ReadToEnd().Trim();
            }
        }

        /// <summary>
        /// Issues a HTTP POST
        /// </summary>
        /// <param name="url">The url</param>
        /// <param name="parameters">Parameters to send as post data</param>
        /// <returns>The response HTML</returns>
        public static string Post(string url, Dictionary<string, object> parameters)
        {
            if (url == null) throw new ArgumentNullException("url");
            if (parameters == null) throw new ArgumentNullException("parameters");

            var req = WebRequest.Create(url);
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";

            var data = GetPostData(parameters);
            var bytes = Encoding.ASCII.GetBytes(data);

            Stream stream = null;

            if (Debug)
                Console.WriteLine(req.RequestUri.ToString());

            try
            {
                req.ContentLength = bytes.Length;
                stream = req.GetRequestStream();
                stream.Write(bytes, 0, bytes.Length);
            }
            catch (WebException ex)
            {
                throw new WebException("Error", ex);
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
            }

            try
            {
                using (var webResponse = req.GetResponse())
                using (var streamReader = new StreamReader(webResponse.GetResponseStream()))
                {
                    return streamReader.ReadToEnd().Trim();
                }
            }
            catch (WebException ex)
            {
                throw new WebException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Creates a post data string
        /// </summary>
        /// <param name="values">The values to convert</param>
        /// <returns>A string represeting values as post data</returns>
        protected static string GetPostData(Dictionary<string, object> values)
        {
            if (values == null || !values.Any())
                return string.Empty;

            return string.Join("&", values.Select(x => string.Format("{0}={1}", x.Key, x.Value)));
        }
    }
}