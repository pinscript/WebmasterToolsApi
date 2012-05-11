using System;
using System.Collections.Generic;
using System.Xml.Linq;
using WebmasterToolsApi.Extensions;
using WebmasterToolsApi.Http;

namespace WebmasterToolsApi {
    public class WebmasterTools {
        /// <summary>
        /// API version
        /// </summary>
        /// <remarks>
        /// Sent via the GData-Version header
        /// </remarks>
        public const int Version = 2;

        /// <summary>
        /// The current authorization token
        /// </summary>
        public readonly string Token;

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        /// <param name="token">A Google GData authorization token</param>
        public WebmasterTools(string token) {
            Token = token;
        }

        /// <summary>
        /// Http headers that every request needs to send
        /// </summary>
        public Dictionary<string, object> StandardHeaders {
            get {
                return new Dictionary<string, object> {
                    {"GData-Version", Version},
                    {"Authorization", "GoogleLogin auth=" + Token}
                };
            }
        }

        /// <summary>
        /// Retrieves a feed containing all sites associated with this account
        /// </summary>
        /// <remarks>https://developers.google.com/webmaster-tools/docs/2.0/reference#Feeds_sites</remarks>
        /// <returns>A dynamic object representing the XML feed</returns>
        public dynamic GetSites() {
            var response = HttpClient.Get(WebmasterToolsUrls.Sites, StandardHeaders);
            return ParseResponse(response);
        }

        /// <summary>
        /// Retrieves a feed containing all messages in a xml feed
        /// </summary>
        /// <remarks>https://developers.google.com/webmaster-tools/docs/2.0/reference#Elements_messages</remarks>
        /// <returns>A dynamic object representing the XML feed</returns>
        public dynamic GetMessages() {
            var response = HttpClient.Get(WebmasterToolsUrls.Messages, StandardHeaders);
            return ParseResponse(response);
        }

        /// <summary>
        /// Retrieves the site feed for a specific site
        /// </summary>
        /// <remarks>https://developers.google.com/webmaster-tools/docs/2.0/developers_guide_protocol#AD_Retrieving</remarks>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public dynamic GetSite(string siteId) {
            siteId = UrlEncode(siteId);
            var url = string.Format(WebmasterToolsUrls.Site, siteId);
            var response = HttpClient.Get(url, StandardHeaders);
            return ParseResponse(response);
        }

        /// <summary>
        /// Retrieves the sitemaps feed for a specific site
        /// </summary>
        /// <remarks>https://developers.google.com/webmaster-tools/docs/2.0/developers_guide_protocol#Sitemaps_About</remarks>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public dynamic GetSitemaps(string siteId) {
            siteId = UrlEncode(siteId);
            var url = string.Format(WebmasterToolsUrls.Sitemaps, siteId);
            var response = HttpClient.Get(url, StandardHeaders);
            return ParseResponse(response);
        }

        /// <summary>
        /// Retrieves the keywords feed for a specific site
        /// </summary>
        /// <remarks>https://developers.google.com/webmaster-tools/docs/2.0/developers_guide_protocol#Keywords_retrieving</remarks>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public dynamic GetKeywords(string siteId) {
            siteId = UrlEncode(siteId);
            var url = string.Format(WebmasterToolsUrls.Keywords, siteId);
            var response = HttpClient.Get(url, StandardHeaders);
            return ParseResponse(response);
        }

        /// <summary>
        /// Retrieves the crawl issues feed for a specific site
        /// </summary>
        /// <remarks>https://developers.google.com/webmaster-tools/docs/2.0/developers_guide_protocol#Crawl_Retrieving</remarks>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public const int CrawlIssuesPerPage = 100;
        public dynamic GetCrawlIssues(string siteId, int startIndex = 1) {
            siteId = UrlEncode(siteId);
            var url = string.Format(WebmasterToolsUrls.CrawlIssues, siteId, startIndex);
            var response = HttpClient.Get(url, StandardHeaders);
            return ParseResponse(response);
        }

        /// <summary>
        /// Parses a google feed into a ExpandoObject
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private static dynamic ParseResponse(string response) {
            var doc = XDocument.Parse(response);
            return doc.ToExpandoObject();
        }

        /// <summary>
        /// Works around the solution where .NET always
        /// sends the last slash un-urlencoded. This method
        /// urlencodes the string twice
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string UrlEncode(string value) {
            return Uri.EscapeDataString(Uri.EscapeDataString(value));
        }
    }
}