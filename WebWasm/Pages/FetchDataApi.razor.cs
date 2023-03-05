using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using WebWasm.Auth;
using WebWasm.Contracts;
using WebWasm.ViewModels;

namespace WebWasm.Pages
{
   public partial class FetchDataApi
   {
  
      [Inject]
      public IWeatherForecastService WeatherForecastService { get; set; }

      public ICollection<WeatherForecastViewModel> weatherForecast { get; set; }

      protected override async Task OnInitializedAsync()
      {
        
        weatherForecast = await WeatherForecastService.GetWeatherForecast();
      }
    
   }
}
