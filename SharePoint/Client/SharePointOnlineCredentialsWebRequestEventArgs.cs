using System;
using System.Net;

namespace AZFuncSPO.SharePoint.Client
{
    // Decompile from Microsoft.SharePoint.Client.Runtime.dll Version=16.1.0.0
    // For the purposes of running on multiplatform e.g. Azure Functions V2 on Linux
    public sealed class SharePointOnlineCredentialsWebRequestEventArgs : EventArgs
    {
        private HttpWebRequest m_webRequest;

        internal SharePointOnlineCredentialsWebRequestEventArgs(HttpWebRequest webRequest)
        {
            this.m_webRequest = webRequest;
        }

        public HttpWebRequest WebRequest
        {
            get
            {
                return this.m_webRequest;
            }
        }
    }
}
