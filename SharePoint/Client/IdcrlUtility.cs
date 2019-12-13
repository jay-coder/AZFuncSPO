using System;
using System.Globalization;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace AZFuncSPO.SharePoint.Client
{
    // Decompile from Microsoft.SharePoint.Client.Runtime.dll Version=16.1.0.0
    // For the purposes of running on multiplatform e.g. Azure Functions V2 on Linux
    internal static class IdcrlUtility
    {
        private const string DummyElementName = "DummyElement";
        private const string DummyElementTag = "<DummyElement>";

        public static string XmlValueEncode(string value)
        {
            StringBuilder output = new StringBuilder();
            using (XmlWriter xmlWriter = XmlWriter.Create(output))
                xmlWriter.WriteElementString("DummyElement", value);
            string str = output.ToString();
            int startIndex = str.IndexOf("<DummyElement>", StringComparison.Ordinal) + "<DummyElement>".Length;
            int num = str.IndexOf('<', startIndex);
            return str.Substring(startIndex, num - startIndex);
        }

        public static XElement GetElementAtPath(XElement elem, params string[] paths)
        {
            foreach (string path in paths)
            {
                if (elem == null)
                    return (XElement)null;
                elem = elem.Element(XName.Get(path));
            }
            return elem;
        }

        public static string GetWebResponseHeader(WebResponse response)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (response != null && response.SupportsHeaders && response.Headers != null)
            {
                foreach (string allKey in response.Headers.AllKeys)
                {
                    if (stringBuilder.Length > 0)
                        stringBuilder.Append(", ");
                    stringBuilder.AppendFormat((IFormatProvider)CultureInfo.InvariantCulture, "{0}={1}", (object)allKey, (object)response.Headers[allKey]);
                }
            }
            return stringBuilder.ToString();
        }
    }
}
