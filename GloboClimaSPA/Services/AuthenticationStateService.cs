using System.Security.Claims;
using System.Text.Json;
using GloboClimaSPA.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;

public class AuthenticationStateService(IJSRuntime jsRuntime, IAuthService authService) : AuthenticationStateProvider, IAuthenticationStateService
{
    private readonly IJSRuntime _jsRuntime = jsRuntime;
    private readonly IAuthService _authService = authService;
    private AuthenticationState _currentState = new(new ClaimsPrincipal());

    public async Task<string> GetAccessTokenAsync()
    {
        var token = await _authService.GetAccessTokenAsync() ?? throw new InvalidOperationException("Access token is null.");
        return token;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await GetAccessTokenAsync();
        if (string.IsNullOrEmpty(token))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
        var user = new ClaimsPrincipal(identity);

        _currentState = new AuthenticationState(user);
        NotifyAuthenticationStateChanged(Task.FromResult(_currentState));

        return _currentState;
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = WebEncoders.Base64UrlDecode(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        var claims = keyValuePairs?.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString() ?? ""));
        if (claims is not null && claims.Any())
        {
            return claims;
        }
        return [];

    }

    IEnumerable<Claim> IAuthenticationStateService.ParseClaimsFromJwt(string jwt)
    {
        throw new NotImplementedException();
    }
}
