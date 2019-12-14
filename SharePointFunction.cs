using AZFuncSPO.SharePoint;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading.Tasks;

namespace AZFuncSPO
{
    public class SharePointFunction
    {
        private ISharePointList _spList;
        private ISharePointTermStore _spTermStore;
        private IConfiguration _config;
        public SharePointFunction(
            ISharePointList spList,
            ISharePointTermStore spTermStore,
            IConfiguration config)
        {
            _spList = spList;
            _spTermStore = spTermStore;
            _config = config;
        }
        [FunctionName("CallSharePointList")]
        public async Task<IActionResult> RunSPList(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // Format OData query
            var sbQuery = new StringBuilder();
            sbQuery.Append($"$select=*,Author/EMail,Editor/EMail&$expand=Author,Editor");

            // Get latest updates from SharePoint
            string strSharePointCollection = $"{_config["SharePoint_BaseUrl"]}/sites/{_config["SharePoint_Collection"]}";
            string strSharePointListUrl = $"{strSharePointCollection}/_api/web/lists/GetByTitle('{_config["SharePoint_List"]}')/items";
            var listJson = await _spList.GetString(new Uri($"{strSharePointListUrl}?{sbQuery.ToString()}"));

            return new OkObjectResult(listJson);
        }

        [FunctionName("CallSharePointTermStore")]
        public async Task<IActionResult> RunSPTermStore(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string strCollectionUrl = $"{_config["SharePoint_BaseUrl"]}/sites/{_config["SharePoint_Collection"]}";
            await _spTermStore.Init(strCollectionUrl, _config["SharePoint_Username"], _config["SharePoint_Password"]);
            var terms = await _spTermStore.GetTerms(new Guid(_config["SharePoint_TermId"]));
            var termList = _spTermStore.BuildTermTree(terms);

            return new OkObjectResult(termList);
        }

        [FunctionName("SharePointWebhook")]
        public async Task<IActionResult> SharePointWebhook(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // Validation from Webhook of SharePoint
            string validationToken = req.Query["validationtoken"];
            if (!string.IsNullOrEmpty(validationToken))
            {
                return new OkObjectResult(validationToken);
            }
            // Logic here
            return new OkObjectResult("Nothing");
        }
    }
}
