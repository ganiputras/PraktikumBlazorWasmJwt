using Application.Models.Authentication;

namespace Application.Contracts.Identity;

public interface IAppAuthenticationService
{
   Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request);
   Task<RegistrationResponse> RegisterAsync(RegistrationRequest request);
}