using api.DTOs.Authentication;
using api.Responses;

namespace api.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<ServiceResponse.GeneralResponse> ResgisterAsync(RegisterDTO registerDTO);
    }
}
