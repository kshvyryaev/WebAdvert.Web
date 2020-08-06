using AutoMapper;
using WebAdvert.Web.Models.Adverts;
using WebAdvert.Web.Models.Home;
using WebAdvert.Web.ServiceClients;

namespace WebAdvert.Web.Profiles
{
    public class WebsiteProfiles : Profile
    {
        public WebsiteProfiles()
        {
            CreateMap<CreateAdvertModel, CreateAdvertViewModel>().ReverseMap();

            CreateMap<AdvertType, SearchViewModel>()
                .ForMember(dest => dest.Id, src => src.MapFrom(field => field.Id))
                .ForMember(dest => dest.Title, src => src.MapFrom(field => field.Title));
        }
    }
}
