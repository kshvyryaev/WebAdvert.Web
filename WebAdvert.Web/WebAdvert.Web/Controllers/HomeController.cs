using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using WebAdvert.Web.Models;
using WebAdvert.Web.Models.Home;
using WebAdvert.Web.ServiceClients;

namespace WebAdvert.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAdvertsApiClient _advertsApiClient;
        private readonly ISearchApiClient _searchApiClient;
        private readonly IMapper _mapper;

        public HomeController(IAdvertsApiClient advertsApiClient, ISearchApiClient searchApiClient, IMapper mapper)
        {
            _advertsApiClient = advertsApiClient;
            _searchApiClient = searchApiClient;
            _mapper = mapper;
        }

        [Authorize]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> Index()
        {
            var allAds = await _advertsApiClient.GetAllAsync().ConfigureAwait(false);
            var allViewModels = allAds.Select(x => _mapper.Map<IndexViewModel>(x));

            return View(allViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> Search(string keyword)
        {
            var viewModels = new List<SearchViewModel>();

            var searchResult = await _searchApiClient.Search(keyword).ConfigureAwait(false);

            searchResult.ForEach(advertDoc =>
            {
                var viewModelItem = _mapper.Map<SearchViewModel>(advertDoc);
                viewModels.Add(viewModelItem);
            });

            return View("Search", viewModels);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
