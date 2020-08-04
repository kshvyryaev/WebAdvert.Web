using AutoMapper;
using WebAdvert.Web.Models.Adverts;
using WebAdvert.Web.ServiceClients;

namespace WebAdvert.Web.Profiles
{
    public class WebsiteProfiles : Profile
    {
        public WebsiteProfiles()
        {
            CreateMap<CreateAdvertModel, CreateAdvertViewModel>().ReverseMap();
        }
    }
}
