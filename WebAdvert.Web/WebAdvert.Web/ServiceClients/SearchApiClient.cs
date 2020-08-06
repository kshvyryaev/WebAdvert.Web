using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace WebAdvert.Web.ServiceClients
{
    public class SearchApiClient : ISearchApiClient
    {
        private readonly HttpClient _client;
        private readonly string _url;

        public SearchApiClient(HttpClient client, IConfiguration configuration)
        {
            _client = client;
            _url = configuration.GetSection("SearchApi").GetValue<string>("BaseUrl");
        }

        public async Task<List<AdvertType>> Search(string keyword)
        {
            var result = new List<AdvertType>();
            var callUrl = $"{_url}/{keyword}";
            var httpResponse = await _client.GetAsync(new Uri(callUrl)).ConfigureAwait(false);

            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                var response = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                var adverts = JsonConvert.DeserializeObject<List<AdvertType>>(response);
                result.AddRange(adverts);
            }

            return result;
        }
    }
}
