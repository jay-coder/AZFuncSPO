using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.Collections.Generic;

namespace AZFuncSPO.Models
{
    public class TermModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ParentId { get; set; }
        public IEnumerable<TermModel> Children { get; set; }

        public bool IsRoot { get; set; }
        public string CustomSortOrder { get; set; }
        public TermModel() { }
        public TermModel(Term term)
        {
            Id = term.Id;
            Name = term.Name;
            if (!term.IsRoot)
            {
                ParentId = term.Parent.Id;
            }
            IsRoot = term.IsRoot;
            CustomSortOrder = term.CustomSortOrder;
        }
    }
}
