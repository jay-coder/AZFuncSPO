using Microsoft.SharePoint.Client;
using System;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security;

namespace AZFuncSPO.SharePoint.Client
{
    // Decompile from Microsoft.SharePoint.Client.Runtime.dll Version=16.1.0.0
    // For the purposes of running on multiplatform e.g. Azure Functions V2 on Linux
    internal class SharePointOnlineAuthenticationProvider
    {
        private static string s_idcrlEnvironment;

        public string GetAuthenticationCookie(
            Uri url,
            string username,
            string password,
            bool alwaysThrowOnFailure,
            EventHandler<SharePointOnlineCredentialsWebRequestEventArgs> executingWebRequest
        )
        {
            if (url == (Uri)null)
                throw new ArgumentNullException(nameof(url));
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException(nameof(username));
            if (password == null)
                throw new ArgumentNullException(nameof(password));
            SharePointOnlineAuthenticationProvider.IdcrlHeader idcrlHeader = this.GetIdcrlHeader(url, alwaysThrowOnFailure, executingWebRequest);
            if (idcrlHeader == null)
            {
                //ClientULS.SendTraceTag(3991707U, ClientTraceCategory.Authentication, ClientTraceLevel.Medium, "Cannot get IDCRL header for {0}", (object)url);
                if (alwaysThrowOnFailure)
                    throw new ClientRequestException(Resources.GetString("CannotContactSite", (object)url));
                return (string)null;
            }
            IdcrlAuth idcrlAuth = new IdcrlAuth(string.Compare(SharePointOnlineAuthenticationProvider.IdcrlServiceEnvironment, "INT-MSO", StringComparison.OrdinalIgnoreCase) != 0 ? (!string.Equals(SharePointOnlineAuthenticationProvider.IdcrlServiceEnvironment, "PPE-MSO", StringComparison.OrdinalIgnoreCase) ? IdcrlEnvironment.Production : IdcrlEnvironment.Ppe) : IdcrlEnvironment.Int, executingWebRequest);

            var securePassword = new SecureString();
            password.ToList().ForEach(c => securePassword.AppendChar(c));
            string password1 = SharePointOnlineAuthenticationProvider.FromSecureString(securePassword);

            string serviceToken = idcrlAuth.GetServiceToken(username, password1, idcrlHeader.ServiceTarget, idcrlHeader.ServicePolicy);
            if (!string.IsNullOrEmpty(serviceToken))
                return this.GetCookie(url, idcrlHeader.Endpoint, serviceToken, alwaysThrowOnFailure, executingWebRequest);
            //ClientULS.SendTraceTag(3991708U, ClientTraceCategory.Authentication, ClientTraceLevel.Medium, "Cannot get IDCRL ticket for username {0}", (object)username);
            if (alwaysThrowOnFailure)
                throw new IdcrlException(Resources.GetString("PPCRL_REQUEST_E_UNKNOWN", (object)-2147186615));
            return (string)null;
        }

        private string GetCookie(
          Uri url,
          string endpoint,
          string ticket,
          bool throwIfFail,
          EventHandler<SharePointOnlineCredentialsWebRequestEventArgs> executingWebRequest)
        {
            Uri uri = new Uri(url, endpoint);
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);
            CookieContainer cookieContainer = new CookieContainer();
            webRequest.CookieContainer = cookieContainer;
            webRequest.Headers[HttpRequestHeader.Authorization] = "BPOSIDCRL " + ticket;
            webRequest.Headers["X-IDCRL_ACCEPTED"] = "t";
            webRequest.Pipelined = false;
            if (executingWebRequest != null)
                executingWebRequest((object)this, new SharePointOnlineCredentialsWebRequestEventArgs(webRequest));
            WebResponse response = webRequest.GetResponse();
            string cookieHeader = cookieContainer.GetCookieHeader(uri);
            if (string.IsNullOrWhiteSpace(cookieHeader))
            {
                UriBuilder uriBuilder = new UriBuilder(uri);
                uriBuilder.Host = webRequest.Host;
                //ClientULS.SendTraceTag(5825556U, ClientTraceCategory.Authentication, ClientTraceLevel.Verbose, "Try get cookie using {0}", (object)uriBuilder.ToString());
                cookieHeader = cookieContainer.GetCookieHeader(uriBuilder.Uri);
                //ClientULS.SendTraceTag(5825557U, ClientTraceCategory.Authentication, ClientTraceLevel.Medium, "Get cookie using {0} and cookie value is {0}", (object)uriBuilder.ToString(), (object)cookieHeader);
            }
            response?.Close();
            if (string.IsNullOrWhiteSpace(cookieHeader))
            {
                //ClientULS.SendTraceTag(3991709U, ClientTraceCategory.Authentication, ClientTraceLevel.Medium, "Cannot get cookie for {0}", (object)url);
                if (throwIfFail)
                    throw new ClientRequestException(Resources.GetString("CannotGetCookie", (object)url));
            }
            return cookieHeader;
        }

        private SharePointOnlineAuthenticationProvider.IdcrlHeader GetIdcrlHeader(
          Uri url,
          bool alwaysThrowOnFailure,
          EventHandler<SharePointOnlineCredentialsWebRequestEventArgs> executingWebRequest)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers["X-IDCRL_ACCEPTED"] = "t";
            webRequest.AuthenticationLevel = AuthenticationLevel.None;
            webRequest.Pipelined = false;
            if (executingWebRequest != null)
                executingWebRequest((object)this, new SharePointOnlineCredentialsWebRequestEventArgs(webRequest));
            HttpWebResponse response;
            try
            {
                response = webRequest.GetResponse() as HttpWebResponse;
            }
            catch (WebException ex)
            {
                //ClientULS.SendTraceTag(3991710U, ClientTraceCategory.Authentication, ClientTraceLevel.Medium, "Exception in request. Url={0}, WebException={1}", (object)url, (object)ex);
                response = ex.Response as HttpWebResponse;
                if (alwaysThrowOnFailure)
                {
                    if (response != null)
                    {
                        if (response.StatusCode != HttpStatusCode.Forbidden)
                        {
                            if (response.StatusCode == HttpStatusCode.Unauthorized)
                                goto label_9;
                        }
                        else
                            goto label_9;
                    }
                    throw;
                }
            }
            label_9:
            if (response == null)
            {
                //ClientULS.SendTraceTag(3991711U, ClientTraceCategory.Authentication, ClientTraceLevel.High, "Cannot get response for request to {0}", (object)url);
                if (alwaysThrowOnFailure)
                    throw new ClientRequestException(Resources.GetString("CannotContactSite", (object)url));
                return (SharePointOnlineAuthenticationProvider.IdcrlHeader)null;
            }
            string webResponseHeader = IdcrlUtility.GetWebResponseHeader((WebResponse)response);
            HttpStatusCode statusCode = response.StatusCode;
            //ClientULS.SendTraceTag(4839637U, ClientTraceCategory.Authentication, ClientTraceLevel.Medium, "Response.StatusCode={0}, Headers={1}", (object)statusCode, (object)webResponseHeader);
            string header = response.Headers["X-IDCRL_AUTH_PARAMS_V1"];
            if (string.IsNullOrEmpty(header))
                header = response.Headers[HttpResponseHeader.WwwAuthenticate];
            response.Close();
            //ClientULS.SendTraceTag(3991712U, ClientTraceCategory.Authentication, ClientTraceLevel.Medium, "IdcrlHeader={0}", (object)header);
            return this.ParseIdcrlHeader(header, url, statusCode, webResponseHeader, alwaysThrowOnFailure);
        }

        private SharePointOnlineAuthenticationProvider.IdcrlHeader ParseIdcrlHeader(
          string headerValue,
          Uri url,
          HttpStatusCode statusCode,
          string allResponseHeaders,
          bool alwaysThrowOnFailure)
        {
            if (string.IsNullOrWhiteSpace(headerValue))
            {
                //ClientULS.SendTraceTag(3991713U, ClientTraceCategory.Authentication, ClientTraceLevel.Medium, "IDCRL header value is empty");
                if (alwaysThrowOnFailure)
                    throw new NotSupportedException(Resources.GetString("SharePointClientCredentialsNotSupported", (object)url.OriginalString, (object)statusCode, (object)allResponseHeaders));
                return (SharePointOnlineAuthenticationProvider.IdcrlHeader)null;
            }
            SharePointOnlineAuthenticationProvider.IdcrlHeader idcrlHeader = new SharePointOnlineAuthenticationProvider.IdcrlHeader();
            string str1 = headerValue;
            char[] chArray = new char[1] { ',' };
            foreach (string str2 in str1.Split(chArray))
            {
                string[] strArray = str2.Trim().Split('=');
                if (strArray.Length == 2)
                {
                    strArray[0] = strArray[0].Trim().ToUpperInvariant();
                    strArray[1] = strArray[1].Trim(' ', '"');
                    switch (strArray[0])
                    {
                        case "IDCRL TYPE":
                            idcrlHeader.IdcrlType = strArray[1];
                            continue;
                        case "ENDPOINT":
                            idcrlHeader.Endpoint = strArray[1];
                            continue;
                        case "ROOTDOMAIN":
                            idcrlHeader.ServiceTarget = strArray[1];
                            continue;
                        case "POLICY":
                            idcrlHeader.ServicePolicy = strArray[1];
                            continue;
                        default:
                            continue;
                    }
                }
            }
            if (idcrlHeader.IdcrlType != "BPOSIDCRL" || string.IsNullOrEmpty(idcrlHeader.ServicePolicy) || (string.IsNullOrEmpty(idcrlHeader.ServiceTarget) || string.IsNullOrEmpty(idcrlHeader.Endpoint)))
            {
                //ClientULS.SendTraceTag(3991714U, ClientTraceCategory.Authentication, ClientTraceLevel.Medium, "Cannot extract required information from IDCRL header. Header={0}, IdcrlType={1}, ServicePolicy={2}, ServiceTarget={3}, Endpoint={4}", (object)headerValue, (object)idcrlHeader.IdcrlType, (object)idcrlHeader.ServicePolicy, (object)idcrlHeader.ServiceTarget, (object)idcrlHeader.Endpoint);
                if (alwaysThrowOnFailure)
                    throw new ClientRequestException(Resources.GetString("InvalidIdcrlHeader", (object)url.OriginalString, (object)headerValue, (object)statusCode, (object)allResponseHeaders));
                idcrlHeader = (SharePointOnlineAuthenticationProvider.IdcrlHeader)null;
            }
            return idcrlHeader;
        }

        private static string FromSecureString(SecureString value)
        {
            IntPtr bstr = Marshal.SecureStringToBSTR(value);
            if (bstr == IntPtr.Zero)
                return string.Empty;
            try
            {
                return Marshal.PtrToStringBSTR(bstr);
            }
            finally
            {
                Marshal.FreeBSTR(bstr);
            }
        }

        private static string IdcrlServiceEnvironment
        {
            get
            {
                string str1 = SharePointOnlineAuthenticationProvider.s_idcrlEnvironment;
                if (str1 == null)
                {
                    str1 = "production";
                    //RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\MSOIdentityCRL");
                    //if (registryKey != null)
                    //{
                    //    string str2 = (string)registryKey.GetValue("ServiceEnvironment", (object)null);
                    //    if (string.Compare(str2, "INT-MSO", StringComparison.OrdinalIgnoreCase) == 0)
                    //        str1 = "INT-MSO";
                    //    else if (string.Equals(str2, "PPE-MSO", StringComparison.OrdinalIgnoreCase))
                    //        str1 = "PPE-MSO";
                    //    registryKey.Close();
                    //}
                    //ClientULS.SendTraceTag(3991715U, ClientTraceCategory.Authentication, ClientTraceLevel.Medium, "IdcrlServiceEnvironment={0}", (object)str1);
                    SharePointOnlineAuthenticationProvider.s_idcrlEnvironment = str1;
                }
                return str1;
            }
        }

        internal bool DoesSupportIdcrl(Uri uri)
        {
            if (uri == (Uri)null)
                throw new ArgumentNullException(nameof(uri));
            return this.GetIdcrlHeader(uri, true, (EventHandler<SharePointOnlineCredentialsWebRequestEventArgs>)null) != null;
        }

        private class IdcrlHeader
        {
            public string IdcrlType;
            public string ServiceTarget;
            public string ServicePolicy;
            public string Endpoint;
        }
    }
}
