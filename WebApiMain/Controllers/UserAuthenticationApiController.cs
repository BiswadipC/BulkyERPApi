using Domain.UserAuthentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Repository.UserAuthentication;
using WebApiMain.Filters.Users;

namespace WebApiMain.Controllers
{
    [ApiController]

    public class UserAuthenticationApiController : ControllerBase
    {
        private readonly IUserResponse iuser;
        private readonly IConfiguration configuration;

        public UserAuthenticationApiController(IUserResponse iuser, IConfiguration configuration)
        {
            this.iuser = iuser;
            this.configuration = configuration;
        } // constructor...

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate(AuthenticateUser user)
        {
            if(await iuser.AuthenticateUser(user))
            {
                int jwtExpiryTime = configuration.GetValue<int>("JWTExpiryTime");
                DateTime expiresAt = DateTime.UtcNow.AddMinutes(jwtExpiryTime);
                string token = await iuser.GenerateJWT(user.UserName, expiresAt);
                RefreshTokenClass rtc = await iuser.PopulateRefreshToken(user.UserName);

                Response.Cookies.Append("JWT", token, new CookieOptions()
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None                    
                });

                Response.Cookies.Append("RT", JsonConvert.SerializeObject(rtc), new CookieOptions()
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None
                });

                return Ok(new
                {
                    Token = token,
                    ExpiresAt = expiresAt
                });
            } // end if...

            ModelState.AddModelError("AuthenticationFailed", "Invalid Username/Password. Access denied.");
            var problemDetails = new ValidationProblemDetails(ModelState)
            {
                Status = StatusCodes.Status400BadRequest
            };
            return new BadRequestObjectResult(problemDetails);
        } // Authenticate...

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await iuser.LogoutUser();
            return Ok();
        } // Logout...

        [HttpPost("create-new-user")]
        [NewUserCreationActionFilter]
        public async Task<IActionResult> CreateNewUser(UserResponse user)
        {
            string str = await iuser.CreateNewUser(user);
            if(str == "Success")
            {
                return Ok(new { message = str });
            }

            ModelState.AddModelError("BadRequest", str);
            var problemDetails = new ValidationProblemDetails(ModelState)
            {
                Status = StatusCodes.Status400BadRequest
            };
            return new BadRequestObjectResult(problemDetails);
        } // CreateNewUser...

        [HttpGet("IsLoggedIn")]
        public async Task<IActionResult> IsLoggedIn()
        {
            bool b = await iuser.IsLoggedIn();
            return b ? Ok(true) : BadRequest(false);
        } // IsLoggedIn...

        [HttpGet("GetUserClaims")]
        public async Task<IActionResult> GetUserClaims()
        {
            var claims = await iuser.GetUserClaims();
            return Ok(claims);
        } // GetUserClaims...

        [HttpGet("GetLoggedInUserName")]
        public async Task<IActionResult> GetLoggedInUserName()
        {
            string username = await iuser.GetLoggedInUserName();
            return string.IsNullOrWhiteSpace(username) ? BadRequest(string.Empty) : Ok(new { UserName = username });
        } // GetLoggedInUserName...
    } // class...
}