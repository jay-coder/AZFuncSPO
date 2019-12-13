using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Http;
using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Net.Http;
using AZFuncSPO.SharePoint.Client;

namespace AZFuncSPO.SharePoint
{
    public class SharePointAuthenticationBuilder
    {
        // Cache Cookies
        private object _lock = new object();
        private Hashtable _cachedCookies = new Hashtable();
        public event EventHandler<SharePointOnlineCredentialsWebRequestEventArgs> ExecutingWebRequest;

        private string BaseUrl { get; set; }
        private string Username { get; set; }
        private string Password { get; set; }

        public SharePointAuthenticationBuilder(IConfiguration config)
        {
            BaseUrl = config[$"SharePoint_BaseUrl"];
            Username = config[$"SharePoint_Username"];
            Password = config[$"SharePoint_Password"];
        }

        public void BuildHttpMessageHandler(HttpMessageHandlerBuilder builder)
        {
            Uri uri = new Uri(BaseUrl);
            var credentials = new SharePointOnlineCredentials(Username, Password);
            var handler = new HttpClientHandler()
            {
                Credentials = credentials
            };
            handler.CookieContainer.SetCookies(uri, GetAuthenticationCookie(uri, true, false));

            builder.PrimaryHandler = handler;
            builder.Build();
        }

        public string GetAuthenticationCookie(Uri url, bool refresh, bool alwaysThrowOnFailure)
        {

            if (url == (Uri)null)
            { 
                throw ClientUtility.CreateArgumentNullException(nameof(url));
            }
            if (!url.IsAbsoluteUri)
            {
                throw ClientUtility.CreateArgumentException(nameof(url));
            }
            url = new Uri(url, "/");
            string str = (string)null;
            CookieCacheEntry cachedCookie = (CookieCacheEntry)this._cachedCookies[(object)url];
            if (!refresh && cachedCookie != null && cachedCookie.IsValid)
            {
                //ClientULS.SendTraceTag(3454916U, ClientTraceCategory.Authentication, ClientTraceLevel.Verbose, "Get cookie from cache for URL {0}", (object)url1);
                return cachedCookie.Cookie;
            }
            if (refresh)
            {
                str = new SharePointOnlineAuthenticationProvider().GetAuthenticationCookie(url, Username, Password, alwaysThrowOnFailure, this.ExecutingWebRequest);
                if (!string.IsNullOrEmpty(str))
                {
                    //ClientULS.SendTraceTag(3454917U, ClientTraceCategory.Authentication, ClientTraceLevel.Medium, "Put cookie in cache for URL {0}", (object)url1);
                    lock (this._lock)
                        this._cachedCookies[(object)url] = (object)new CookieCacheEntry()
                        {
                            Cookie = str,
                            Expires = DateTime.UtcNow.AddHours(1.0)
                        };
                }
            }
            return str;
        }
    }
}
