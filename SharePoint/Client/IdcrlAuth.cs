using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace AZFuncSPO.SharePoint.Client
{
    // Decompile from Microsoft.SharePoint.Client.Runtime.dll Version=16.1.0.0
    // For the purposes of running on multiplatform e.g. Azure Functions V2 on Linux
    internal class IdcrlAuth
    {
        private static Dictionary<string, int> s_partnerSoapErrorMap = new Dictionary<string, int>((IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase)
        {
          {
            "InvalidRequest",
            -2147186474
          },
          {
            "FailedAuthentication",
            -2147186446
          },
          {
            "RequestFailed",
            -2147186473
          },
          {
            "InvalidSecurityToken",
            -2147186472
          },
          {
            "AuthenticationBadElements",
            -2147186471
          },
          {
            "BadRequest",
            -2147186470
          },
          {
            "ExpiredData",
            -2147186469
          },
          {
            "InvalidTimeRange",
            -2147186468
          },
          {
            "InvalidScope",
            -2147186467
          },
          {
            "RenewNeeded",
            -2147186466
          },
          {
            "UnableToRenew",
            -2147186465
          }
        };
        private static IdcrlAuth.FederationProviderInfoCache s_FederationProviderInfoCache = new IdcrlAuth.FederationProviderInfoCache();
        private IdcrlEnvironment m_env;
        private string m_userRealmServiceUrl;
        private string m_securityTokenServiceUrl;
        private string m_federationTokenIssuer;
        private EventHandler<SharePointOnlineCredentialsWebRequestEventArgs> m_executingWebRequest;

        public IdcrlAuth(
          IdcrlEnvironment env,
          EventHandler<SharePointOnlineCredentialsWebRequestEventArgs> executingWebRequest
        )
        {
            this.m_env = env;
            //ClientULS.SendTraceTag(3454918U, ClientTraceCategory.Authentication, ClientTraceLevel.Verbose, "IDCRL Environment {0}", (object)env);
            if (this.m_env == IdcrlEnvironment.Production)
            {
                this.m_userRealmServiceUrl = "https://login.microsoftonline.com/GetUserRealm.srf";
                this.m_securityTokenServiceUrl = "https://login.microsoftonline.com/rst2.srf";
                this.m_federationTokenIssuer = "urn:federation:MicrosoftOnline";
            }
            else if (this.m_env == IdcrlEnvironment.Ppe)
            {
                this.m_userRealmServiceUrl = "https://login.windows-ppe.net/GetUserRealm.srf";
                this.m_securityTokenServiceUrl = "https://login.windows-ppe.net/rst2.srf";
                this.m_federationTokenIssuer = "urn:federation:MicrosoftOnline";
            }
            else
            {
                this.m_userRealmServiceUrl = "https://login.microsoftonline-int.com/GetUserRealm.srf";
                this.m_securityTokenServiceUrl = "https://login.microsoftonline-int.com/rst2.srf";
                this.m_federationTokenIssuer = "urn:federation:MicrosoftOnline-int";
            }
            this.m_executingWebRequest = executingWebRequest;
        }

        private string UserRealmServiceUrl
        {
            get
            {
                return this.m_userRealmServiceUrl;
            }
        }

        private string ServiceTokenUrl
        {
            get
            {
                return this.m_securityTokenServiceUrl;
            }
        }

        private string FederationTokenIssuer
        {
            get
            {
                return this.m_federationTokenIssuer;
            }
        }

        public string GetServiceToken(
          string username,
          string password,
          string serviceTarget,
          string servicePolicy)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException(nameof(username));
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrEmpty(serviceTarget))
                throw new ArgumentNullException(nameof(serviceTarget));
            this.InitFederationProviderInfoForUser(username);
            IdcrlAuth.UserRealmInfo userRealm = this.GetUserRealm(username);
            if (userRealm.IsFederated)
                return this.GetServiceToken(this.GetPartnerTicketFromAdfs(userRealm.STSAuthUrl, username, password), serviceTarget, servicePolicy);
            return this.GetServiceToken(this.BuildWsSecurityUsingUsernamePassword(username, password), serviceTarget, servicePolicy);
        }

        private IdcrlAuth.UserRealmInfo GetUserRealm(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentNullException(nameof(login));
            XDocument xdocument = this.DoPost(this.UserRealmServiceUrl, "application/x-www-form-urlencoded", string.Format((IFormatProvider)CultureInfo.InvariantCulture, "login={0}&xml=1", (object)Uri.EscapeDataString(login)), (Func<WebException, Exception>)null);
            XAttribute xattribute = xdocument.Root.Attribute((XName)"Success");
            if (xattribute == null || string.Compare(xattribute.Value, "true", StringComparison.OrdinalIgnoreCase) != 0)
            {
                //ClientULS.SendTraceTag(3454919U, ClientTraceCategory.Authentication, ClientTraceLevel.High, "Failed to get user's realm for user {0}", (object)login);
                throw IdcrlAuth.CreateIdcrlException(-2147186539);
            }
            XElement xelement1 = xdocument.Root.Element((XName)"NameSpaceType");
            if (xelement1 == null)
            {
                //ClientULS.SendTraceTag(3454920U, ClientTraceCategory.Authentication, ClientTraceLevel.High, "There is no NameSpaceType element in the response when get user realm for user {0}", (object)login);
                throw IdcrlAuth.CreateIdcrlException(-2147186539);
            }
            if (string.Compare(xelement1.Value, "Federated", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(xelement1.Value, "Managed", StringComparison.OrdinalIgnoreCase) != 0)
            {
                //ClientULS.SendTraceTag(3454921U, ClientTraceCategory.Authentication, ClientTraceLevel.High, "Unknown namespace type for user {0}", (object)login);
                throw IdcrlAuth.CreateIdcrlException(-2147186539);
            }
            IdcrlAuth.UserRealmInfo userRealmInfo = new IdcrlAuth.UserRealmInfo();
            userRealmInfo.IsFederated = 0 == string.Compare(xelement1.Value, "Federated", StringComparison.OrdinalIgnoreCase);
            XElement xelement2 = xdocument.Root.Element((XName)"STSAuthURL");
            if (xelement2 != null)
                userRealmInfo.STSAuthUrl = xelement2.Value;
            if (userRealmInfo.IsFederated && string.IsNullOrEmpty(userRealmInfo.STSAuthUrl))
            {
                //ClientULS.SendTraceTag(3454922U, ClientTraceCategory.Authentication, ClientTraceLevel.High, "User {0} is a federated account, but there is no STSAuthUrl for the user.", (object)login);
                throw IdcrlAuth.CreateIdcrlException(-2147186539);
            }
            //ClientULS.SendTraceTag(3454923U, ClientTraceCategory.Authentication, ClientTraceLevel.Verbose, "User={0}, IsFederated={1}, STSAuthUrl={2}", (object)login, (object)userRealmInfo.IsFederated, (object)userRealmInfo.STSAuthUrl);
            return userRealmInfo;
        }

        private string GetPartnerTicketFromAdfs(string adfsUrl, string username, string password)
        {
            string body = string.Format((IFormatProvider)CultureInfo.InvariantCulture, "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<s:Envelope xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:wsse=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\" xmlns:saml=\"urn:oasis:names:tc:SAML:1.0:assertion\" xmlns:wsp=\"http://schemas.xmlsoap.org/ws/2004/09/policy\" xmlns:wsu=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\" xmlns:wsa=\"http://www.w3.org/2005/08/addressing\" xmlns:wssc=\"http://schemas.xmlsoap.org/ws/2005/02/sc\" xmlns:wst=\"http://schemas.xmlsoap.org/ws/2005/02/trust\">\r\n    <s:Header>\r\n        <wsa:Action s:mustUnderstand=\"1\">http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue</wsa:Action>\r\n        <wsa:To s:mustUnderstand=\"1\">{0}</wsa:To>\r\n        <wsa:MessageID>{1}</wsa:MessageID>\r\n        <ps:AuthInfo xmlns:ps=\"http://schemas.microsoft.com/Passport/SoapServices/PPCRL\" Id=\"PPAuthInfo\">\r\n            <ps:HostingApp>Managed IDCRL</ps:HostingApp>\r\n            <ps:BinaryVersion>6</ps:BinaryVersion>\r\n            <ps:UIVersion>1</ps:UIVersion>\r\n            <ps:Cookies></ps:Cookies>\r\n            <ps:RequestParams>AQAAAAIAAABsYwQAAAAxMDMz</ps:RequestParams>\r\n        </ps:AuthInfo>\r\n        <wsse:Security>\r\n            <wsse:UsernameToken wsu:Id=\"user\">\r\n                <wsse:Username>{2}</wsse:Username>\r\n                <wsse:Password>{3}</wsse:Password>\r\n            </wsse:UsernameToken>\r\n            <wsu:Timestamp Id=\"Timestamp\">\r\n                <wsu:Created>{4}</wsu:Created>\r\n                <wsu:Expires>{5}</wsu:Expires>\r\n            </wsu:Timestamp>\r\n        </wsse:Security>\r\n    </s:Header>\r\n    <s:Body>\r\n        <wst:RequestSecurityToken Id=\"RST0\">\r\n            <wst:RequestType>http://schemas.xmlsoap.org/ws/2005/02/trust/Issue</wst:RequestType>\r\n            <wsp:AppliesTo>\r\n                <wsa:EndpointReference>\r\n                    <wsa:Address>{6}</wsa:Address>\r\n                </wsa:EndpointReference>\r\n            </wsp:AppliesTo>\r\n            <wst:KeyType>http://schemas.xmlsoap.org/ws/2005/05/identity/NoProofKey</wst:KeyType>\r\n        </wst:RequestSecurityToken>\r\n    </s:Body>\r\n</s:Envelope>", (object)IdcrlUtility.XmlValueEncode(adfsUrl), (object)Guid.NewGuid().ToString(), (object)IdcrlUtility.XmlValueEncode(username), (object)IdcrlUtility.XmlValueEncode(password), (object)DateTime.UtcNow.ToString("o", (IFormatProvider)CultureInfo.InvariantCulture), (object)DateTime.UtcNow.AddMinutes(10.0).ToString("o", (IFormatProvider)CultureInfo.InvariantCulture), (object)this.FederationTokenIssuer);
            XDocument xdoc = this.DoPost(adfsUrl, "application/soap+xml; charset=utf-8", body, new Func<WebException, Exception>(IdcrlAuth.HandleWebException));
            Exception soapException = IdcrlAuth.GetSoapException(xdoc);
            if (soapException != null)
            {
                //ClientULS.SendTraceTag(3454924U, ClientTraceCategory.Authentication, ClientTraceLevel.High, "SOAP error from {0}. Exception={1}", (object)adfsUrl, (object)soapException);
                throw soapException;
            }
            XElement elementAtPath = IdcrlUtility.GetElementAtPath(xdoc.Root, "{http://www.w3.org/2003/05/soap-envelope}Body", "{http://schemas.xmlsoap.org/ws/2005/02/trust}RequestSecurityTokenResponse", "{http://schemas.xmlsoap.org/ws/2005/02/trust}RequestedSecurityToken", "{urn:oasis:names:tc:SAML:1.0:assertion}Assertion");
            if (elementAtPath == null)
            {
                //ClientULS.SendTraceTag(3454925U, ClientTraceCategory.Authentication, ClientTraceLevel.High, "Cannot get security assertion for user {0} from {1}", (object)username, (object)adfsUrl);
                throw IdcrlAuth.CreateIdcrlException(-2147186451);
            }
            return elementAtPath.ToString(SaveOptions.DisableFormatting | SaveOptions.OmitDuplicateNamespaces);
        }

        private string GetServiceToken(string securityXml, string serviceTarget, string servicePolicy)
        {
            string serviceTokenUrl = this.ServiceTokenUrl;
            string str = string.Empty;
            if (!string.IsNullOrEmpty(servicePolicy))
                str = string.Format((IFormatProvider)CultureInfo.InvariantCulture, "<wsp:PolicyReference URI=\"{0}\"></wsp:PolicyReference>", (object)servicePolicy);
            string body = string.Format((IFormatProvider)CultureInfo.InvariantCulture, "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<S:Envelope xmlns:S=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:wsse=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\" xmlns:wsp=\"http://schemas.xmlsoap.org/ws/2004/09/policy\" xmlns:wsu=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\" xmlns:wsa=\"http://www.w3.org/2005/08/addressing\" xmlns:wst=\"http://schemas.xmlsoap.org/ws/2005/02/trust\">\r\n  <S:Header>\r\n    <wsa:Action S:mustUnderstand=\"1\">http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue</wsa:Action>\r\n    <wsa:To S:mustUnderstand=\"1\">{0}</wsa:To>\r\n    <ps:AuthInfo xmlns:ps=\"http://schemas.microsoft.com/LiveID/SoapServices/v1\" Id=\"PPAuthInfo\">\r\n      <ps:BinaryVersion>5</ps:BinaryVersion>\r\n      <ps:HostingApp>Managed IDCRL</ps:HostingApp>\r\n    </ps:AuthInfo>\r\n    <wsse:Security>{1}</wsse:Security>\r\n  </S:Header>\r\n  <S:Body>\r\n    <wst:RequestSecurityToken xmlns:wst=\"http://schemas.xmlsoap.org/ws/2005/02/trust\" Id=\"RST0\">\r\n      <wst:RequestType>http://schemas.xmlsoap.org/ws/2005/02/trust/Issue</wst:RequestType>\r\n      <wsp:AppliesTo>\r\n        <wsa:EndpointReference>\r\n          <wsa:Address>{2}</wsa:Address>\r\n        </wsa:EndpointReference>\r\n      </wsp:AppliesTo>\r\n      {3}\r\n    </wst:RequestSecurityToken>\r\n  </S:Body>\r\n</S:Envelope>\r\n", (object)IdcrlUtility.XmlValueEncode(serviceTokenUrl), (object)securityXml, (object)IdcrlUtility.XmlValueEncode(serviceTarget), (object)str);
            XDocument xdoc = this.DoPost(serviceTokenUrl, "application/soap+xml; charset=utf-8", body, new Func<WebException, Exception>(IdcrlAuth.HandleWebException));
            Exception soapException = IdcrlAuth.GetSoapException(xdoc);
            if (soapException != null)
            {
                //ClientULS.SendTraceTag(3454926U, ClientTraceCategory.Authentication, ClientTraceLevel.High, "Soap error from {0}. Exception={1}", (object)serviceTokenUrl, (object)soapException);
                throw soapException;
            }
            XElement elementAtPath = IdcrlUtility.GetElementAtPath(xdoc.Root, "{http://www.w3.org/2003/05/soap-envelope}Body", "{http://schemas.xmlsoap.org/ws/2005/02/trust}RequestSecurityTokenResponse", "{http://schemas.xmlsoap.org/ws/2005/02/trust}RequestedSecurityToken", "{http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd}BinarySecurityToken");
            if (elementAtPath == null)
            {
                //ClientULS.SendTraceTag(3454927U, ClientTraceCategory.Authentication, ClientTraceLevel.High, "Cannot get binary security token for from {0}", (object)serviceTokenUrl);
                throw IdcrlAuth.CreateIdcrlException(-2147186656);
            }
            return elementAtPath.Value;
        }

        private string BuildWsSecurityUsingUsernamePassword(string username, string password)
        {
            DateTime utcNow = DateTime.UtcNow;
            return string.Format((IFormatProvider)CultureInfo.InvariantCulture, "\r\n            <wsse:UsernameToken wsu:Id=\"user\">\r\n                <wsse:Username>{0}</wsse:Username>\r\n                <wsse:Password>{1}</wsse:Password>\r\n            </wsse:UsernameToken>\r\n            <wsu:Timestamp Id=\"Timestamp\">\r\n                <wsu:Created>{2}</wsu:Created>\r\n                <wsu:Expires>{3}</wsu:Expires>\r\n            </wsu:Timestamp>\r\n", (object)IdcrlUtility.XmlValueEncode(username), (object)IdcrlUtility.XmlValueEncode(password), (object)utcNow.ToString("o", (IFormatProvider)CultureInfo.InvariantCulture), (object)utcNow.AddDays(1.0).ToString("o", (IFormatProvider)CultureInfo.InvariantCulture));
        }

        private XDocument DoPost(
          string url,
          string contentType,
          string body,
          Func<WebException, Exception> webExceptionHandler)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = "POST";
            webRequest.ContentType = contentType;
            //ClientULS.SendTraceTag(3454928U, ClientTraceCategory.Authentication, ClientTraceLevel.Verbose, "Sending POST request to {0}", (object)url);
            if (this.m_executingWebRequest != null)
                this.m_executingWebRequest((object)this, new SharePointOnlineCredentialsWebRequestEventArgs(webRequest));
            using (Stream requestStream = webRequest.GetRequestStream())
            {
                if (body != null)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(body);
                    requestStream.Write(bytes, 0, bytes.Length);
                }
            }
            try
            {
                HttpWebResponse response = webRequest.GetResponse() as HttpWebResponse;
                if (response == null)
                {
                    //ClientULS.SendTraceTag(3454929U, ClientTraceCategory.Authentication, ClientTraceLevel.High, "Unexpected response for POST request to {0}", (object)url);
                    throw new InvalidOperationException();
                }
                using (response)
                {
                    using (TextReader textReader = (TextReader)new StreamReader(response.GetResponseStream()))
                    {
                        string end = textReader.ReadToEnd();
                        //ClientULS.SendTraceTag(3454930U, ClientTraceCategory.Authentication, ClientTraceLevel.Verbose, "URL={0}, StatusCode={1}, ResponseText={2}", (object)url, (object)(int)response.StatusCode, (object)end);
                        using (XmlReader reader = XmlReader.Create((TextReader)new StringReader(end)))
                            return XDocument.Load(reader);
                    }
                }
            }
            catch (WebException ex)
            {
                //ClientULS.SendTraceTag(3454931U, ClientTraceCategory.Authentication, ClientTraceLevel.High, "URL={0}, WebException={1}", (object)url, (object)ex);
                if (webExceptionHandler == null)
                {
                    throw;
                }
                else
                {
                    Exception exception = webExceptionHandler(ex);
                    if (exception != null)
                        throw exception;
                    throw;
                }
            }
        }

        private static Exception HandleWebException(WebException webException)
        {
            HttpWebResponse response = webException.Response as HttpWebResponse;
            if (response != null && response.ContentType != null)
            {
                if (response.ContentType.IndexOf("application/soap+xml", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    try
                    {
                        using (TextReader textReader = (TextReader)new StreamReader(response.GetResponseStream()))
                        {
                            string end = textReader.ReadToEnd();
                            //ClientULS.SendTraceTag(3454932U, ClientTraceCategory.Authentication, ClientTraceLevel.High, "StatusCode={0}, ResponseText={1}", (object)(int)response.StatusCode, (object)end);
                            using (XmlReader reader = XmlReader.Create((TextReader)new StringReader(end)))
                                return IdcrlAuth.GetSoapException(XDocument.Load(reader));
                        }
                    }
                    catch (XmlException ex)
                    {
                        //ClientULS.SendTraceTag(3454933U, ClientTraceCategory.Authentication, ClientTraceLevel.Medium, "Error when read error response. Exception={0}", (object)ex);
                    }
                    catch (IOException ex)
                    {
                        //ClientULS.SendTraceTag(3454934U, ClientTraceCategory.Authentication, ClientTraceLevel.Medium, "Error when read error response. Exception={0}", (object)ex);
                    }
                }
            }
            return (Exception)null;
        }

        private static Exception GetSoapException(XDocument xdoc)
        {
            if (IdcrlUtility.GetElementAtPath(xdoc.Root, "{http://www.w3.org/2003/05/soap-envelope}Body", "{http://www.w3.org/2003/05/soap-envelope}Fault") == null)
                return (Exception)null;
            XElement elementAtPath1 = IdcrlUtility.GetElementAtPath(xdoc.Root, "{http://www.w3.org/2003/05/soap-envelope}Body", "{http://www.w3.org/2003/05/soap-envelope}Fault", "{http://www.w3.org/2003/05/soap-envelope}Code", "{http://www.w3.org/2003/05/soap-envelope}Subcode", "{http://www.w3.org/2003/05/soap-envelope}Value");
            XElement elementAtPath2 = IdcrlUtility.GetElementAtPath(xdoc.Root, "{http://www.w3.org/2003/05/soap-envelope}Body", "{http://www.w3.org/2003/05/soap-envelope}Fault", "{http://www.w3.org/2003/05/soap-envelope}Detail", "{http://schemas.microsoft.com/Passport/SoapServices/SOAPFault}error", "{http://schemas.microsoft.com/Passport/SoapServices/SOAPFault}value");
            XElement elementAtPath3 = IdcrlUtility.GetElementAtPath(xdoc.Root, "{http://www.w3.org/2003/05/soap-envelope}Body", "{http://www.w3.org/2003/05/soap-envelope}Fault", "{http://www.w3.org/2003/05/soap-envelope}Detail", "{http://schemas.microsoft.com/Passport/SoapServices/SOAPFault}error", "{http://schemas.microsoft.com/Passport/SoapServices/SOAPFault}internalerror", "{http://schemas.microsoft.com/Passport/SoapServices/SOAPFault}text");
            string str1 = (string)null;
            if (elementAtPath1 != null)
            {
                str1 = elementAtPath1.Value;
                int num = str1.IndexOf(':');
                if (num >= 0)
                    str1 = str1.Substring(num + 1);
            }
            string s = (string)null;
            if (elementAtPath2 != null)
                s = elementAtPath2.Value;
            string str2 = (string)null;
            if (elementAtPath3 != null)
                str2 = elementAtPath3.Value;
            //ClientULS.SendTraceTag(3454935U, ClientTraceCategory.Authentication, ClientTraceLevel.High, "PassportErrorCode={0}, PassportDetailCode={1}, PassportErrorText={2}", (object)str1, (object)s, (object)str2);
            int hr;
            if (string.IsNullOrEmpty(s))
            {
                hr = IdcrlAuth.MapPartnerSoapFault(str1);
            }
            else
            {
                long result;
                if (s.StartsWith("0x", StringComparison.OrdinalIgnoreCase) && long.TryParse(s.Substring(2), NumberStyles.HexNumber, (IFormatProvider)CultureInfo.InvariantCulture, out result) || long.TryParse(s, NumberStyles.Integer, (IFormatProvider)CultureInfo.InvariantCulture, out result))
                {
                    hr = (int)result;
                    if (string.Compare(str1, "FailedAuthentication", StringComparison.OrdinalIgnoreCase) == 0)
                        hr = hr == -2147186639 ? hr : -2147186655;
                }
                else
                    hr = -2147186656;
            }
            return IdcrlAuth.CreateIdcrlException(hr);
        }

        private static int MapPartnerSoapFault(string code)
        {
            int num;
            if (IdcrlAuth.s_partnerSoapErrorMap.TryGetValue(code, out num))
                return num;
            return -2147186451;
        }

        private static Exception CreateIdcrlException(int hr)
        {
            string stringId;
            if (!IdcrlErrorCodes.TryGetErrorStringId(hr, out stringId))
                stringId = "PPCRL_REQUEST_E_UNKNOWN";
            return (Exception)new IdcrlException(Resources.GetString(stringId), hr);
        }

        private void InitFederationProviderInfoForUser(string username)
        {
            int num = username.IndexOf('@');
            if (num < 0 || num == username.Length - 1)
                throw ClientUtility.CreateArgumentException(nameof(username));
            IdcrlAuth.FederationProviderInfo federationProviderInfo = this.GetFederationProviderInfo(username.Substring(num + 1));
            if (federationProviderInfo != null)
            {
                this.m_userRealmServiceUrl = federationProviderInfo.UserRealmServiceUrl;
                this.m_securityTokenServiceUrl = federationProviderInfo.SecurityTokenServiceUrl;
                this.m_federationTokenIssuer = federationProviderInfo.FederationTokenIssuer;
            }
            //ClientULS.SendTraceTag(3454936U, ClientTraceCategory.Authentication, ClientTraceLevel.Verbose, "UserName={0}, UserRealmServiceUrl={1}, SecurityTokenServiceUrl={1}, FederationTokenIssuer={2}", (object)username, (object)this.m_userRealmServiceUrl, (object)this.m_securityTokenServiceUrl, (object)this.m_federationTokenIssuer);
        }

        private IdcrlAuth.FederationProviderInfo GetFederationProviderInfo(string domainname)
        {
            IdcrlAuth.FederationProviderInfo federationProviderInfo1;
            if (IdcrlAuth.s_FederationProviderInfoCache.TryGetValue(domainname, out federationProviderInfo1))
            {
                //ClientULS.SendTraceTag(3454937U, ClientTraceCategory.Authentication, ClientTraceLevel.Verbose, "Get federation provider information for {0} from cache. UserRealmServiceUrl={1}, SecurityTokenServiceUrl={2}, FederationTokenIssuer={3}", (object)domainname, federationProviderInfo1 == null ? (object)(string)null : (object)federationProviderInfo1.UserRealmServiceUrl, federationProviderInfo1 == null ? (object)(string)null : (object)federationProviderInfo1.SecurityTokenServiceUrl, federationProviderInfo1 == null ? (object)(string)null : (object)federationProviderInfo1.FederationTokenIssuer);
                return federationProviderInfo1;
            }
            IdcrlAuth.FederationProviderInfo federationProviderInfo2 = this.RequestFederationProviderInfo(domainname);
            IdcrlAuth.s_FederationProviderInfoCache.Put(domainname, federationProviderInfo2);
            //ClientULS.SendTraceTag(3454938U, ClientTraceCategory.Authentication, ClientTraceLevel.Medium, "Get federation provider information for {0} and put it in cache. UserRealmServcieUrl={1}, SecurityTokenServiceUrl={2}, FederationTokenIssuer={3}", (object)domainname, federationProviderInfo2 == null ? (object)(string)null : (object)federationProviderInfo2.UserRealmServiceUrl, federationProviderInfo2 == null ? (object)(string)null : (object)federationProviderInfo2.SecurityTokenServiceUrl, federationProviderInfo2 == null ? (object)(string)null : (object)federationProviderInfo2.FederationTokenIssuer);
            return federationProviderInfo2;
        }

        private IdcrlAuth.FederationProviderInfo RequestFederationProviderInfo(
          string domainname)
        {
            int num;
            for (; (num = domainname.IndexOf('.')) > 0; domainname = domainname.Substring(num + 1))
            {
                string url = string.Format((IFormatProvider)CultureInfo.InvariantCulture, IdcrlMessageConstants.FPUrlFullUrlFormat, (object)domainname);
                try
                {
                    string fpDomainName = IdcrlAuth.ParseFPDomainName(this.DoGet(url));
                    url = string.Format((IFormatProvider)CultureInfo.InvariantCulture, IdcrlMessageConstants.FPListFullUrlFormat, (object)domainname);
                    return IdcrlAuth.ParseFederationProviderInfo(this.DoGet(url), fpDomainName);
                }
                catch (WebException ex)
                {
                    //ClientULS.SendTraceTag(3454939U, ClientTraceCategory.Authentication, ClientTraceLevel.Medium, "Exception when request {0}. Exception={1}", (object)url, (object)ex);
                }
            }
            return (IdcrlAuth.FederationProviderInfo)null;
        }

        private static string ParseFPDomainName(XDocument xdoc)
        {
            XElement elementAtPath = IdcrlUtility.GetElementAtPath(xdoc.Root, "FPDOMAINNAME");
            if (elementAtPath == null)
            {
                //ClientULS.SendTraceTag(3454940U, ClientTraceCategory.Authentication, ClientTraceLevel.High, "Cannot find FPDOMAINNAME element");
                throw IdcrlAuth.CreateIdcrlException(-2147186646);
            }
            return elementAtPath.Value;
        }

        private static IdcrlAuth.FederationProviderInfo ParseFederationProviderInfo(
          XDocument xdoc,
          string fpDomainName)
        {
            foreach (XElement element in xdoc.Root.Elements((XName)"FP"))
            {
                if (element.Attribute((XName)"DomainName") != null && string.Equals(element.Attribute((XName)"DomainName").Value, fpDomainName, StringComparison.OrdinalIgnoreCase))
                {
                    XElement elementAtPath1 = IdcrlUtility.GetElementAtPath(element, "URL", "GETUSERREALM");
                    XElement elementAtPath2 = IdcrlUtility.GetElementAtPath(element, "URL", "RST2");
                    XElement elementAtPath3 = IdcrlUtility.GetElementAtPath(element, "URL", "ENTITYID");
                    if (elementAtPath1 == null || elementAtPath2 == null || elementAtPath3 == null)
                    {
                        //ClientULS.SendTraceTag(3454941U, ClientTraceCategory.Authentication, ClientTraceLevel.High, "Cannot get the user realm service url or security token service url for federation provider {0}", (object)fpDomainName);
                        throw IdcrlAuth.CreateIdcrlException(-2147186646);
                    }
                    //ClientULS.SendTraceTag(3454942U, ClientTraceCategory.Authentication, ClientTraceLevel.High, "Find federation provider information for federation provider domain name {0}. UserRealmServiceUrl={1}, SecurityTokenServiceUrl={2}, FederationTokenIssuer={3}", (object)fpDomainName, (object)elementAtPath1.Value, (object)elementAtPath2.Value, (object)elementAtPath3.Value);
                    return new IdcrlAuth.FederationProviderInfo()
                    {
                        UserRealmServiceUrl = elementAtPath1.Value,
                        SecurityTokenServiceUrl = elementAtPath2.Value,
                        FederationTokenIssuer = elementAtPath3.Value
                    };
                }
            }
            //ClientULS.SendTraceTag(3454943U, ClientTraceCategory.Authentication, ClientTraceLevel.High, "Cannot find federation provider information for federation domain {0}", (object)fpDomainName);
            throw IdcrlAuth.CreateIdcrlException(-2147186646);
        }

        private XDocument DoGet(string url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = "GET";
            //ClientULS.SendTraceTag(3454944U, ClientTraceCategory.Authentication, ClientTraceLevel.Verbose, "Sending GET request to {0}", (object)url);
            if (this.m_executingWebRequest != null)
                this.m_executingWebRequest((object)this, new SharePointOnlineCredentialsWebRequestEventArgs(webRequest));
            HttpWebResponse response = webRequest.GetResponse() as HttpWebResponse;
            if (response == null)
            {
                //ClientULS.SendTraceTag(3454945U, ClientTraceCategory.Authentication, ClientTraceLevel.High, "Unexpected response for GET request to URL {0}", (object)url);
                throw new InvalidOperationException();
            }
            using (response)
            {
                using (TextReader textReader = (TextReader)new StreamReader(response.GetResponseStream()))
                {
                    string end = textReader.ReadToEnd();
                    //ClientULS.SendTraceTag(3454946U, ClientTraceCategory.Authentication, ClientTraceLevel.Verbose, "StatusCode={0}, ResponseText={1}", (object)(int)response.StatusCode, (object)end);
                    using (XmlReader reader = XmlReader.Create((TextReader)new StringReader(end)))
                        return XDocument.Load(reader);
                }
            }
        }

        private class UserRealmInfo
        {
            public string STSAuthUrl { get; set; }

            public bool IsFederated { get; set; }
        }

        private class FederationProviderInfo
        {
            public string UserRealmServiceUrl { get; set; }

            public string SecurityTokenServiceUrl { get; set; }

            public string FederationTokenIssuer { get; set; }
        }

        private class FederationProviderInfoCacheEntry
        {
            public IdcrlAuth.FederationProviderInfo Value;
            public DateTime Expires;
        }

        private class FederationProviderInfoCache
        {
            private object m_lock = new object();
            private Dictionary<string, IdcrlAuth.FederationProviderInfoCacheEntry> m_cache = new Dictionary<string, IdcrlAuth.FederationProviderInfoCacheEntry>((IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase);
            private const int CacheLifetimeMinutes = 30;

            public bool TryGetValue(string domainname, out IdcrlAuth.FederationProviderInfo value)
            {
                lock (this.m_lock)
                {
                    IdcrlAuth.FederationProviderInfoCacheEntry providerInfoCacheEntry;
                    if (this.m_cache.TryGetValue(domainname, out providerInfoCacheEntry))
                    {
                        if (providerInfoCacheEntry.Expires > DateTime.UtcNow)
                        {
                            value = providerInfoCacheEntry.Value;
                            return true;
                        }
                    }
                }
                value = (IdcrlAuth.FederationProviderInfo)null;
                return false;
            }

            public void Put(string domainname, IdcrlAuth.FederationProviderInfo value)
            {
                lock (this.m_lock)
                    this.m_cache[domainname] = new IdcrlAuth.FederationProviderInfoCacheEntry()
                    {
                        Value = value,
                        Expires = DateTime.UtcNow.AddMinutes(30.0)
                    };
            }
        }
    }
}
