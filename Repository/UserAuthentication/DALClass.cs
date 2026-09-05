using Azure;
using Domain.UserAuthentication;
using Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Repository.UserAuthentication
{
    namespace NUserAuthentication
    {
        internal sealed class DALClass : IUserResponse
        {
            private readonly BulkyContext context;
            private readonly IConfiguration configuration;
            private readonly IHttpContextAccessor httpContext;

            public DALClass(BulkyContext context, IConfiguration configuration, IHttpContextAccessor httpContext)
            {
                this.context = context;
                this.configuration = configuration;
                this.httpContext = httpContext;
            } // constructor...

            public async Task<string> CreateNewUser(UserResponse user)
            {
                string message = string.Empty;

                var trans = await context.Database.BeginTransactionAsync();
                try
                {
                    string isAdmin = context.Users.Count() > 0 ? "No" : "Yes";

                    User u = new User();
                    u.UserName = user.UserName;
                    u.Password = user.Password;
                    u.Email = user.Email;
                    u.Mobile = user.Mobile;
                    u.IsAdmin = isAdmin;
                    await context.AddAsync(u);
                    await context.SaveChangesAsync();

                    foreach(var m in context.Modules)
                    {
                        ModulePolicyMapping mpcView = new ModulePolicyMapping();
                        mpcView.ModuleName = m.ModuleName;
                        mpcView.UserIdNo = u.IdNo;
                        mpcView.PolicyName = m.ModuleName + "-" + "VIEW_POLICY";
                        mpcView.PermissionType = isAdmin == "Yes" ? "View" : "None";
                        mpcView.IsAdmin = isAdmin;
                        await context.AddAsync(mpcView);
                        await context.SaveChangesAsync();

                        ModulePolicyMapping mpcAll = new ModulePolicyMapping();
                        mpcAll.ModuleName = m.ModuleName;
                        mpcAll.UserIdNo = u.IdNo;
                        mpcAll.PolicyName = m.ModuleName + "-" + "ALL_POLICY";
                        mpcAll.PermissionType = isAdmin == "Yes" ? "Edit" : "None";
                        mpcAll.IsAdmin = isAdmin;
                        await context.AddAsync(mpcAll);
                        await context.SaveChangesAsync();
                    } // end of foreach loop...

                    await trans.CommitAsync();
                    message = "Success";
                }
                catch (Exception ex)
                {
                    await trans.RollbackAsync();
                    message = ex.ToString();
                }
                finally
                {
                    trans.Dispose();
                }

                return message;
            } // CreateNewUser...

            public async Task<bool> AuthenticateUser(AuthenticateUser user)
            {
                return await Task.Run(() =>
                {
                    return context.Users.Any(m => m.UserName == user.UserName && m.Password == user.Password);
                });                
            } // AuthenticateUser...

            public async Task<UserResponse> GetApplicationUser(string username)
            {
                return (await
                (from user in context.Users
                 where user.UserName == username
                 select new UserResponse
                 {
                     IdNo = user.IdNo,
                     UserName = user.UserName,
                     Password = user.Password,
                     Email = user.Email,
                     Mobile = user.Mobile,
                     IsAdmin = user.IsAdmin
                 }).FirstOrDefaultAsync() ?? new UserResponse());
            } // GetApplicationUser...

            public async Task<string> GenerateJWT(string username, DateTime expiresAt)
            {
                string securityKey = configuration.GetValue<string>("SecurityKey") ?? string.Empty;
                SigningCredentials credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey)),
                            SecurityAlgorithms.HmacSha256Signature);
                
                var userResponse = await GetApplicationUser(username);
                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim("UserName", username));
                foreach(var data in context.ModulePolicyMappings.Where(m => m.UserIdNo == userResponse.IdNo))
                {
                    claims.Add(new Claim(data.PolicyName, data.PermissionType));
                }
                ClaimsIdentity identity = new ClaimsIdentity(claims);

                SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
                {
                    SigningCredentials = credentials,
                    Subject = identity,
                    NotBefore = DateTime.UtcNow
                };

                JsonWebTokenHandler handler = new JsonWebTokenHandler();
                string token = handler.CreateToken(descriptor);

                return token;
            } // GenerateJWT...

            public async Task<RefreshTokenClass> PopulateRefreshToken(string username)
            {
                var existingUser = context.Users.Where(m => m.UserName ==  username).FirstOrDefault();
                string token = Guid.NewGuid().ToString();
                existingUser!.RefreshToken = token;
                context.Update(existingUser);
                await context.SaveChangesAsync();

                RefreshTokenClass rtc = new RefreshTokenClass();
                rtc.UserName = username;
                rtc.Token = token;

                return rtc;
            } // PopulateRefreshToken...

            public async Task<bool> ValidateRTOnJWTExpiryAndGenerateJWT(DateTime expiresAt)
            {
                string rtStr = httpContext.HttpContext!.Request.Cookies["RT"]!.ToString() ?? string.Empty;
                RefreshTokenClass rtc = JsonConvert.DeserializeObject<RefreshTokenClass>(rtStr) ?? new RefreshTokenClass();
                string tokenUserName = rtc.UserName ?? string.Empty;
                string token = rtc.Token ?? string.Empty;
                string databaseToken = context.Users.Where(m => m.UserName == tokenUserName).FirstOrDefault()!.RefreshToken ?? string.Empty;

                if(token == databaseToken)
                {
                    string rtNew = Guid.NewGuid().ToString();
                    var existingUser = context.Users.Where(m => m.UserName == tokenUserName).FirstOrDefault();
                    existingUser!.RefreshToken = rtNew;
                    context.Update(existingUser);
                    await context.SaveChangesAsync();

                    string newJWT = await GenerateJWT(tokenUserName, expiresAt);
                    RefreshTokenClass rtcNew = new RefreshTokenClass();
                    rtcNew.UserName = tokenUserName;
                    rtcNew.Token = rtNew;
                    
                    httpContext.HttpContext!.Response.Cookies.Append("JWT", newJWT, new CookieOptions()
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                    });

                    httpContext.HttpContext.Response.Cookies.Append("RT", JsonConvert.SerializeObject(rtcNew), new CookieOptions()
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None
                    });

                    return true;
                } // end if...

                return false;
            } // ValidateRTOnJWTExpiryAndGenerateJWT...

            public async Task<bool> IsLoggedIn()
            {
                string rtStr = httpContext.HttpContext!.Request.Cookies["RT"] ?? string.Empty;

                if(string.IsNullOrEmpty(rtStr)) return false;

                RefreshTokenClass? rtc = JsonConvert.DeserializeObject<RefreshTokenClass>(rtStr);
                if(rtc == null) return false;

                string username = rtc.UserName;
                if(username == null) return false;

                if (context.Users.Any(m => m.UserName == username)) return await Task.Run(() => true); else return await Task.Run(() => false);
            } // IsLoggedIn...

            public async Task<string> GetLoggedInUserName()
            {
                string strRT = httpContext.HttpContext!.Request.Cookies["RT"] ?? string.Empty;
                
                if(!string.IsNullOrWhiteSpace(strRT))
                {
                    RefreshTokenClass rtc = JsonConvert.DeserializeObject<RefreshTokenClass>(strRT) ?? new RefreshTokenClass();
                    string username = rtc.UserName ?? string.Empty;
                    return await Task.Run(() => username);
                }

                return string.Empty;
            } // GetLoggedInUserName...

            public async Task<dynamic> GetUserClaims()
            {
                if (httpContext.HttpContext!.Request.Cookies["RT"] == null)
                {
                    return null!;
                }

                string strRT = httpContext.HttpContext.Request.Cookies["RT"] ?? string.Empty;
                RefreshTokenClass rtc = JsonConvert.DeserializeObject<RefreshTokenClass>(strRT) ?? new RefreshTokenClass();
                string username = rtc.UserName;
                int? userIdNo = await context.Users.Where(m =>  m.UserName == username).Select(s => s.IdNo).FirstOrDefaultAsync();

                var claims = await context.ModulePolicyMappings.Where(m => m.UserIdNo == userIdNo).Select(s => new
                {
                    Type = s.PolicyName,
                    Value = s.PermissionType
                }).ToListAsync();
                return claims;
            } // GetUserClaims...

            public async Task LogoutUser()
            {
                if(httpContext.HttpContext!.Request.Cookies["RT"] != null)
                {
                    string rtStr = httpContext.HttpContext.Request.Cookies["RT"] ?? string.Empty;
                    RefreshTokenClass rtc = JsonConvert.DeserializeObject<RefreshTokenClass>(rtStr)!;
                    string username = rtc.UserName;

                    var existingUser = await context.Users.FirstOrDefaultAsync(x => x.UserName == username);
                    existingUser!.RefreshToken = string.Empty;
                    context.Update(existingUser);
                    await context.SaveChangesAsync();
                } // end if...

                httpContext.HttpContext!.Response.Cookies.Delete("JWT", new CookieOptions()
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None
                });

                httpContext.HttpContext!.Response.Cookies.Delete("RT", new CookieOptions()
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None
                });
            } // LogoutUser...
        } // DALClass...
    } // NUserAuthentication...
}
