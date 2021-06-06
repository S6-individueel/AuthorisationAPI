using AuthorisationAPI.AuthenticationData;
using AuthorisationAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthorisationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorisationController : Controller
    {
        private IAuthenticationData _authenticationData;

        protected IConfiguration _config;

        public AuthorisationController(IAuthenticationData authenticationData, IConfiguration config)
        {
            _authenticationData = authenticationData;
            _config = config;
        }

        [HttpGet]
        public IActionResult Authenticate(User user)
        {
            var audienceConfig = _config.GetSection("Audience");
            var authenticated = _authenticationData.AuthenticateUser(user);
            if(authenticated != false)
            {
                var now = DateTime.UtcNow;

                var claims = new Claim[]
                {
        /*    new Claim(JwtRegisteredClaimNames.Sub, name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),*/
            new Claim(JwtRegisteredClaimNames.Iat, now.ToUniversalTime().ToString(), ClaimValueTypes.Integer64)
                };

                var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(audienceConfig["Secret"]));
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateIssuer = true,
                    ValidIssuer = audienceConfig["Iss"],
                    ValidateAudience = true,
                    ValidAudience = audienceConfig["Aud"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    RequireExpirationTime = true,

                };

                var jwt = new JwtSecurityToken(
                    issuer: audienceConfig["Iss"],
                    audience: audienceConfig["Aud"],
                    claims: claims,
                    notBefore: now,
                    expires: now.Add(TimeSpan.FromHours(24)),
                    signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                );
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
                var responseJson = new
                {
                    access_token = encodedJwt,
                    expires_in = (int)TimeSpan.FromHours(24).TotalHours
                };

                return Ok(responseJson);
            }
                return NotFound($"User was not found, check email and password");          
        }
    }
}
