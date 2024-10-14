using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
namespace GloboClimaSPA.Interfaces
{
    public interface IAuthenticationStateService
    {
        Task<string> GetAccessTokenAsync();
        Task<AuthenticationState> GetAuthenticationStateAsync();
        IEnumerable<Claim> ParseClaimsFromJwt(string jwt);
    }
}