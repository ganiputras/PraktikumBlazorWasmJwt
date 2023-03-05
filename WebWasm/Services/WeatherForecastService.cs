using AutoMapper;
using Blazored.LocalStorage;
using WebWasm.Contracts;
using WebWasm.Services.Base;
using WebWasm.ViewModels;

namespace WebWasm.Services
{
   public class WeatherForecastService : BaseDataService, IWeatherForecastService
   {
      private readonly IMapper _mapper;
      public WeatherForecastService(IClient client, ILocalStorageService localStorage, IMapper mapper) : base(client, localStorage)
      {
         _mapper = mapper;
      }

      public async Task<List<WeatherForecastViewModel>> GetWeatherForecast()
      {
         await AddBearerToken();

         var result = await _client.GetWeatherForecastAsync();
         var mappedEvents = _mapper.Map<ICollection<WeatherForecastViewModel>>(result);
         return mappedEvents.ToList();
      }
   }
}
