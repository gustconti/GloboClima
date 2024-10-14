using Blazored.LocalStorage;  // Ensure you have this using directive
using Microsoft.AspNetCore.Components;
using Amazon.CognitoIdentityProvider;
using GloboClimaSPA.Interfaces;
using GloboClimaSPA.Models;
using Amazon.CognitoIdentityProvider.Model;
using System.Net;

namespace GloboClimaSPA.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAmazonCognitoIdentityProvider _cognitoProvider;
        private readonly NavigationManager _navigationManager;
        private readonly IConfiguration _configuration;
        private readonly string? _cognitoClientId;
        ILocalStorageService _localStorage;


        public AuthService(
            IAmazonCognitoIdentityProvider cognitoProvider,
            NavigationManager navigationManager,
            IConfiguration configuration,
            ILocalStorageService localStorage)
        {
            _cognitoProvider = cognitoProvider;
            _cognitoClientId = configuration["Cognito:ClientId"];
            _configuration = configuration;
            _localStorage = localStorage;
        }

        public async Task<bool> RegisterUserAsync(RegisterUserModel registerUserModel)
        {
            var signUpRequest = new SignUpRequest
            {
                ClientId = _cognitoClientId,
                Username = registerUserModel.Email,
                Password = registerUserModel.Password,
                UserAttributes =
                [
                    new AttributeType
                    {
                        Name = "email",
                        Value = registerUserModel.Email
                    }
                ]
            };

            try
            {
                var signUpResponse = await _cognitoProvider.SignUpAsync(signUpRequest);
                return signUpResponse.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception)
            {
                // Log error and handle exceptions (e.g., user already exists, password strength issues, etc.)
                return false;
            }
        }

        public async Task<bool> ConfirmUserAsync(string email, string verificationCode)
        {
            var confirmRequest = new ConfirmSignUpRequest
            {
                ClientId = _cognitoClientId,
                Username = email,
                ConfirmationCode = verificationCode
            };

            try
            {
                var confirmResponse = await _cognitoProvider.ConfirmSignUpAsync(confirmRequest);
                return confirmResponse.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception)
            {
                // Handle confirmation errors (e.g., invalid code, expired code)
                return false;
            }
        }

        public async Task<bool> LoginUserAsync(string email, string password)
        {
            var authRequest = new InitiateAuthRequest
            {
                ClientId = _cognitoClientId,
                AuthFlow = AuthFlowType.USER_PASSWORD_AUTH,
                AuthParameters = new Dictionary<string, string>
            {
                { "USERNAME", email },
                { "PASSWORD", password }
            }
            };

            try
            {
                var authResponse = await _cognitoProvider.InitiateAuthAsync(authRequest);
                // Store tokens or handle successful login
                return authResponse.AuthenticationResult != null;
            }
            catch (Exception)
            {
                // Handle login failure (e.g., wrong password, user not found)
                return false;
            }
        }

        public async Task<string?> GetAccessTokenAsync()
        {
            try
            {
                var authRequest = new AdminInitiateAuthRequest
                {
                    AuthFlow = AuthFlowType.USER_PASSWORD_AUTH,
                    ClientId = _cognitoClientId,
                    AuthParameters = new Dictionary<string, string>
                {
                    { "USERNAME", "user-email" },  // Replace with the user's email/username
                    { "PASSWORD", "user-password" }  // Replace with the user's password
                }
                };

                var authResponse = await _cognitoProvider.AdminInitiateAuthAsync(authRequest);
                return authResponse.AuthenticationResult?.IdToken; // Or AccessToken depending on your use case
            }
            catch (Exception)
            {
                // Log or handle the error as appropriate
                return string.Empty;
            }
        }
        public async Task LogoutUserAsync()
        {
            try
            {
                // Call Cognito's global logout endpoint to revoke the tokens
                var globalSignOutRequest = new GlobalSignOutRequest
                {
                    AccessToken = await GetAccessTokenAsync()  // You need the access token to log the user out
                };

                var response = await _cognitoProvider.GlobalSignOutAsync(globalSignOutRequest);

                if (response.HttpStatusCode == HttpStatusCode.OK)
                {
                    // Clear the tokens stored on the client side (if applicable)
                    ClearTokens();

                    // Optionally redirect the user to a different page
                    _navigationManager.NavigateTo("/login", true);  // Assuming _navigationManager is injected
                }
                else
                {
                    // Handle logout failure, log an error, or notify the user
                    throw new Exception("Error logging out from Cognito.");
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                Console.WriteLine($"Logout failed: {ex.Message}");
                throw;
            }
        }

        private void ClearTokens()
        {
            // Assuming tokens are stored in local storage or session storage
            // Example: Clearing local storage
            _localStorage.RemoveItemAsync("access_token");
            _localStorage.RemoveItemAsync("id_token");
            _localStorage.RemoveItemAsync("refresh_token");
        }


        public async Task<UserModel?> GetCurrentUserAsync(string accessToken)
        {
            try
            {
                // Assuming the user is already authenticated and the token is stored in a secure place
                var currentUser = await _cognitoProvider.GetUserAsync(new GetUserRequest()
                {
                    AccessToken = accessToken
                });

                var userAttributes = currentUser.UserAttributes.ToDictionary(attr => attr.Name, attr => attr.Value);

                var userModel = new UserModel
                {
                    Email = userAttributes["email"],
                    Username = userAttributes["preferred_username"],  // Or whatever attributes you have
                };

                return userModel;
            }
            catch (Exception)
            {
                // Handle errors like invalid token or expired session
                return null;
            }
        }

        // Other methods like RegisterUserAsync, ConfirmUserAsync, etc.
    }

}