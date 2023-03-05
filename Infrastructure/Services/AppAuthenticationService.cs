using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Contracts.Identity;
using Application.Models.Authentication;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public class AppAuthenticationService : IAppAuthenticationService
{
   private readonly JwtSettings _jwtSettings;
   private readonly SignInManager<ApplicationUser> _signInManager;
   private readonly UserManager<ApplicationUser> _userManager;

   public AppAuthenticationService(UserManager<ApplicationUser> userManager,
      IOptions<JwtSettings> jwtSettings,
      SignInManager<ApplicationUser> signInManager)
   {
      _userManager = userManager;
      _jwtSettings = jwtSettings.Value;
      _signInManager = signInManager;
   }

   public async Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request)
   {
      var user = await _userManager.FindByEmailAsync(request.Email);

      if (user == null) throw new Exception($"User with {request.Email} not found.");

      var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, false);

      if (!result.Succeeded) throw new Exception($"Credentials for '{request.Email} aren't valid'.");

      var jwtSecurityToken = await GenerateToken(user);

      var response = new AuthenticationResponse
      {
         Id = user.Id,
         Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
         Email = user.Email,
         UserName = user.UserName
      };

      return response;
   }

   public async Task<RegistrationResponse> RegisterAsync(RegistrationRequest request)
   {
      var existingUser = await _userManager.FindByNameAsync(request.UserName);

      if (existingUser != null) throw new Exception($"Username '{request.UserName}' already exists.");

      var user = new ApplicationUser
      {
         Email = request.Email,
         FirstName = request.FirstName,
         LastName = request.LastName,
         UserName = request.UserName,
         EmailConfirmed = true
      };

      var existingEmail = await _userManager.FindByEmailAsync(request.Email);

      if (existingEmail == null)
      {
         var result = await _userManager.CreateAsync(user, request.Password);

         if (result.Succeeded)
            return new RegistrationResponse { UserId = user.Id };
         throw new Exception($"{result.Errors}");
      }

      throw new Exception($"Email {request.Email} already exists.");
   }

   private async Task<JwtSecurityToken> GenerateToken(ApplicationUser user)
   {
      var userClaims = await _userManager.GetClaimsAsync(user);
      var roles = await _userManager.GetRolesAsync(user);

      var roleClaims = new List<Claim>();

      foreach (var t in roles)
         roleClaims.Add(new Claim("roles", t));

      var claims = new[]
         {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("uid", user.Id)
         }
         .Union(userClaims)
         .Union(roleClaims);

      var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
      var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

      var jwtSecurityToken = new JwtSecurityToken(
         _jwtSettings.Issuer,
         _jwtSettings.Audience,
         claims,
         expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
         signingCredentials: signingCredentials);
      return jwtSecurityToken;
   }
}