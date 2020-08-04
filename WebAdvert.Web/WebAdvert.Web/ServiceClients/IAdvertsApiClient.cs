using System.Threading.Tasks;

namespace WebAdvert.Web.ServiceClients
{
    public interface IAdvertsApiClient
    {
        Task<AdvertResponse> CreateAsync(CreateAdvertModel model);

        Task<bool> ConfirmAsync(ConfirmAdvertRequest model);
    }
}
