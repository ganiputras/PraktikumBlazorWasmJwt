
using WebWasm.ViewModels;

namespace WebWasm.Contracts
{
   public interface IWeatherForecastService
   {
      Task<List<WeatherForecastViewModel>> GetWeatherForecast();
   }
}
