using GloboClimaSPA.Models;
namespace GloboClimaSPA.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterUserAsync(RegisterUserModel registerUserModel);
        Task<bool> ConfirmUserAsync(string email, string verificationCode);
        Task<bool> LoginUserAsync(string email, string password);
        Task<UserModel?> GetCurrentUserAsync(string accessToken);
        Task<string?> GetAccessTokenAsync();
        Task LogoutUserAsync();
    }
}