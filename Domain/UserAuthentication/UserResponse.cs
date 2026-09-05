using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.UserAuthentication
{
    public class UserResponse
    {
        public int IdNo {  get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? ReTypePassword {  get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? Mobile {  get; set; } = string.Empty;
        public string? IsAdmin { get; set; } = string.Empty;
    } // class...

    public class AuthenticateUser
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    } // AuthenticateUser...

    public class UserClaimsClass
    {

    } // UserClaimsClass...
}
