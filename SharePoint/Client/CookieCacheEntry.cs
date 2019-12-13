using System;

namespace AZFuncSPO.SharePoint.Client
{
    // Decompile from Microsoft.SharePoint.Client.Runtime.dll Version=16.1.0.0
    // For the purposes of running on multiplatform e.g. Azure Functions V2 on Linux
    internal class CookieCacheEntry
    {
        public string Cookie;
        public DateTime Expires;

        public bool IsValid
        {
            get
            {
                return DateTime.UtcNow < this.Expires;
            }
        }
    }
}
