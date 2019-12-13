using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AZFuncSPO.SharePoint
{
    public class SharePointList : ISharePointList
    {
        private HttpClient _httpClient;

        public SharePointList(IHttpClientFactory httpFactory)
        {
            _httpClient = httpFactory.CreateClient("SharePoint");
        }

        private async Task<HttpResponseMessage> GetHttpResponse(Uri url)
        {
            // Authenticated by HttpFactory
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return await _httpClient.GetAsync(url);
        }

        public async Task<string> GetString(Uri url)
        {
            var response = await GetHttpResponse(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return string.Empty;
        }
    }
}
