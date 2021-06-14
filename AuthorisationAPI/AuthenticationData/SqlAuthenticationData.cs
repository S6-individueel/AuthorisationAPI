﻿using AuthorisationAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorisationAPI.AuthenticationData
{
    public class SqlAuthenticationData : IAuthenticationData
    {
        private UsersContext _usersContext;

        public SqlAuthenticationData(UsersContext usersContext)
        {
            _usersContext = usersContext;
        }
        public User AuthenticateUser(User user)
        {
            foreach (var existingUser in _usersContext.Users.Where(r => r.Email == user.Email))
            {
                if(existingUser.Password == user.Password)
                {
                    user.Id = existingUser.Id;
                    return user;
                }             
            }
            return null;
        }
    }
}
