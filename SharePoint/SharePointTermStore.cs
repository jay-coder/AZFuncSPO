using AZFuncSPO.Models;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AZFuncSPO.SharePoint
{
    public class SharePointTermStore : ISharePointTermStore
    {
        private TermStore _termStore;

        public async Task Init(string collectionUrl, string username, string password)
        {
            if (_termStore == null)
            {
                // Initialise Term Store may take a bit long at the first time
                using (var ctx = new ClientContext(collectionUrl))
                {
                    ctx.Credentials = new SharePointOnlineCredentials(username, password);
                    var taxonomySession = TaxonomySession.GetTaxonomySession(ctx);
                    var termStore = taxonomySession.GetDefaultSiteCollectionTermStore();
                    ctx.Load(termStore,
                        s => s.Groups.Include(
                            g => g.TermSets.Include(
                                ts => ts.Name
                            )
                        )
                    );
                    await ctx.ExecuteQueryAsync();
                    _termStore = termStore;
                }
            }
        }

        public async Task<IEnumerable<TermModel>> GetTerms(Guid termSetId)
        {
            if (_termStore == null)
            {
                throw new NullReferenceException("Term Store has not been initialized yet");
            }
            // Get Terms underneath "NSW Trains"
            var ctx = _termStore.Context;
            var oimTermSet = _termStore.GetTermSet(termSetId);
            var terms = oimTermSet.GetAllTerms();
            ctx.Load(terms, includes =>
                includes.Include(
                    i => i.Id,
                    i => i.Name,
                    i => i.Parent,
                    i => i.Parent.Id,
                    i => i.Parent.Name,
                    i => i.IsRoot,
                    i => i.CustomSortOrder
                )
            );
            await ctx.ExecuteQueryAsync();
            if (terms != null && terms.Any())
            {
                return terms.Select(s => new TermModel(s));
            }
            return null;
        }

        public IEnumerable<TermModel> BuildTermTree(IEnumerable<TermModel> terms)
        {
            if (terms != null && terms.Any())
            {
                var termTree = new List<TermModel>();
                var termList = new List<TermModel>();
                var treeRoot = terms.FirstOrDefault(f => f.IsRoot);
                if (treeRoot != null)
                {
                    IterateTree(terms, treeRoot, termTree, termList);
                }
                return termList;
            }
            return null;
        }

        private void IterateTree(IEnumerable<TermModel> terms, TermModel termNode, IList<TermModel> termTree, IList<TermModel> termList)
        {
            var childList = terms.Where(w => !w.IsRoot && w.ParentId == termNode.Id).ToList();
            // Sort by Custom Order
            if (!string.IsNullOrEmpty(termNode.CustomSortOrder))
            {
                var idOrderList = termNode.CustomSortOrder.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                childList = idOrderList.Select(s => terms.FirstOrDefault(f => f.Id == new Guid(s))).Where(w => w != null).ToList();
            }
            IList<TermModel> childTermList = null;
            if (childList != null && childList.Any())
            {
                childTermList = new List<TermModel>();
                foreach (var child in childList)
                {
                    IterateTree(terms, child, childTermList, termList);
                }
            }
            termNode.Children = childTermList;
            termTree.Add(termNode);
            termList.Add(termNode);
        }
    }
}
