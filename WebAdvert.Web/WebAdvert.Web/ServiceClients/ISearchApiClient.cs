using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAdvert.Web.ServiceClients
{
    public interface ISearchApiClient
    {
        Task<List<AdvertType>> Search(string keyword);
    }
}
