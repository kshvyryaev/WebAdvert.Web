using System;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using Newtonsoft.Json;
using WebAdvert.Models;
using System.Collections.Generic;
using System.Linq;

namespace WebAdvert.Web.ServiceClients
{
    public class AdvertsApiClient : IAdvertsApiClient
    {
        private readonly string _baseAddress;
        private readonly HttpClient _client;
        private readonly IMapper _mapper;

        public AdvertsApiClient(IConfiguration configuration, HttpClient client, IMapper mapper)
        {
            _client = client;
            _mapper = mapper;
            _baseAddress = configuration.GetSection("AdvertApi").GetValue<string>("BaseUrl");
        }

        public async Task<AdvertResponse> CreateAsync(CreateAdvertModel model)
        {
            var advertApiModel = _mapper.Map<AdvertModel>(model);
            var jsonModel = JsonConvert.SerializeObject(advertApiModel);
            var response = await _client.PostAsync(
                new Uri($"{_baseAddress}/create"),
                new StringContent(jsonModel, Encoding.UTF8, "application/json")).ConfigureAwait(false);
            var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var createAdvertResponse = JsonConvert.DeserializeObject<CreateAdvertResponse>(responseString);
            var advertResponse = _mapper.Map<AdvertResponse>(createAdvertResponse);
            return advertResponse;
        }

        public async Task<bool> ConfirmAsync(ConfirmAdvertRequest model)
        {
            var advertModel = _mapper.Map<ConfirmAdvertModel>(model);
            var jsonModel = JsonConvert.SerializeObject(advertModel);
            var response = await _client.PutAsync(
                new Uri($"{_baseAddress}/confirm"),
                new StringContent(jsonModel, Encoding.UTF8, "application/json")).ConfigureAwait(false);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public async Task<List<Advertisement>> GetAllAsync()
        {
            var response = await _client.GetAsync(new Uri($"{_baseAddress}/all")).ConfigureAwait(false);
            var responseAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var models = JsonConvert.DeserializeObject<List<AdvertModel>>(responseAsString);
            return models.Select(x => _mapper.Map<Advertisement>(x)).ToList();
        }

        public async Task<Advertisement> GetAsync(string advertId)
        {
            var response = await _client.GetAsync(new Uri($"{_baseAddress}/{advertId}")).ConfigureAwait(false);
            var responseAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var model = JsonConvert.DeserializeObject<AdvertModel>(responseAsString);
            return _mapper.Map<Advertisement>(model);
        }
    }
}
