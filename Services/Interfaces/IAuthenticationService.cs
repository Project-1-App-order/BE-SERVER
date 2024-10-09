using api.DTOs.Authentication;
using api.Responses;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace api.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<ServiceResponse.GeneralResponse> ResgisterAsync(RegisterDTO registerDTO);
        Task<ServiceResponse.LoginResponse> LoginAsync(LoginDTO registerDTO);
        JwtSecurityToken GetToken(List<Claim> authClaims);

    }
}
