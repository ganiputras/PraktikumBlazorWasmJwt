using Application.Contracts.Identity;
using Application.Models.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
   //[Microsoft.AspNetCore.Components.Route("api/[controller]")]
   [Route("api/[controller]")]
   [ApiController]

   public class AccountController : ControllerBase
   {
      private readonly IAppAuthenticationService _authenticationService;
      public AccountController(IAppAuthenticationService authenticationService)
      {
         _authenticationService = authenticationService;
      }

      [HttpPost("authenticate")]
      public async Task<ActionResult<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request)
      {
         return Ok(await _authenticationService.AuthenticateAsync(request));
      }

      [HttpPost("register")]
      public async Task<ActionResult<RegistrationResponse>> RegisterAsync(RegistrationRequest request)
      {
         return Ok(await _authenticationService.RegisterAsync(request));
      }
   }
}
