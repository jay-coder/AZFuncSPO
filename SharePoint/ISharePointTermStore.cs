using AZFuncSPO.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AZFuncSPO.SharePoint
{
    public interface ISharePointTermStore
    {
        Task Init(string collectionUrl, string username, string password);
        Task<IEnumerable<TermModel>> GetTerms(Guid termSetId);
        IEnumerable<TermModel> BuildTermTree(IEnumerable<TermModel> terms);
    }
}