using System;
using System.Threading.Tasks;

namespace AZFuncSPO.SharePoint
{
    public interface ISharePointList
    {
        Task<string> GetString(Uri url);
    }
}
