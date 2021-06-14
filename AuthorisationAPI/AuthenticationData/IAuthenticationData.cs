using AuthorisationAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorisationAPI.AuthenticationData
{
    public interface IAuthenticationData
    {
        User AuthenticateUser(User user);
    }
}
