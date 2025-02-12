using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;

namespace PriceNegotiationAPI.Services
{
    public interface IUserService
    {
        Task<SignInResult> LoginAsync(LoginRequest login, bool? useCookies, bool? useSessionCookies);
        Task LogOutAsync();
    }
}
