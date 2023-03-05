
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using WebWasm.Auth;
using WebWasm.Contracts;

namespace WebWasm.Pages
{
   public partial class Index
   {
      [Inject]
      private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

      [Inject]
      public NavigationManager NavigationManager { get; set; }

      [Inject]
      public IAuthenticationService AuthenticationService { get; set; }


      protected override async Task OnInitializedAsync()
      {
         await ((CustomAuthenticationStateProvider)AuthenticationStateProvider).GetAuthenticationStateAsync();
      }

      protected void NavigateToLogin()
      {
         NavigationManager.NavigateTo("login");
      }

      protected void NavigateToRegister()
      {
         NavigationManager.NavigateTo("register");
      }

      protected async void Logout()
      {
        await AuthenticationService.Logout();
      }
   }
}