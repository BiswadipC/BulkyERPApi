using Domain.UserAuthentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.UserAuthentication
{
    public interface IUserResponse
    {
        Task<string> CreateNewUser(UserResponse user);
        Task<bool> AuthenticateUser(AuthenticateUser user);
        Task<UserResponse> GetApplicationUser(string username);
        Task<string> GenerateJWT(string username, DateTime expiresAt);
        Task<RefreshTokenClass> PopulateRefreshToken(string username);
        Task<bool> ValidateRTOnJWTExpiryAndGenerateJWT(DateTime expiresAt);
        Task<bool> IsLoggedIn();
        Task<dynamic> GetUserClaims();
        Task<string> GetLoggedInUserName();
        Task LogoutUser();
    } // IUserResponse...
}
