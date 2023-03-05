using AutoMapper;
using WebWasm.Pages;
using WebWasm.Services.Base;
using WebWasm.ViewModels;

namespace WebWasm.Profiles
{
   public class MappingProfile : Profile
   {
      public MappingProfile()
      {
         //Vms are coming in from the API, ViewModel are the local entities in Blazor
         CreateMap<WeatherForecast, WeatherForecastViewModel>().ReverseMap();
      }
   }
}
