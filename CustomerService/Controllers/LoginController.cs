using System;
using System.Collections.Generic;
using System.Linq;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using CustomerService.Models;

namespace CustomerService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    
    public class LoginController 
        : ControllerBase
    {
        private readonly IConfiguration configReader;
       // private const int TimeLimitInMin = 10;                                      //Additional

        private readonly IList<Credential> _appUsers = new List<Credential>         //Athenticated Users
        {
           new Credential{FullName = "Admin User", UserName = "admin", Password = "1234", UserRole = "Admin"},
           new Credential{FullName = "Test User", UserName = "user", Password = "1234", UserRole = "User"},

        };

        public LoginController(IConfiguration config)
        {
            configReader = config;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login([FromBody] [FromHeader] Credential credential)
        {
            IActionResult unauthorizedresponse = Unauthorized();                            //Default response

            var user = this.GetMatchingUserFromRepo(credential);                            //Authenticaticating User

            if (user != null)                                                   //Authentication successful
            {
                var tokenString = GenerateJWTToken(user);                       //Generate JWT

                unauthorizedresponse = base.Ok(new {
                    token = tokenString, 
                    user.UserRole,
                    user.FullName
                });       //Changes & 200 OK response
            }
            return unauthorizedresponse;
        }

       private Credential GetMatchingUserFromRepo(Credential loginCredentials)
        {
            Credential matchinguserfound = _appUsers.SingleOrDefault(x => x.UserName == loginCredentials.UserName && x.Password == loginCredentials.Password);
            return matchinguserfound;
        }

        private string GenerateJWTToken(Credential credential)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configReader["Jwt:SecretKey"]));
            var SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var Claims = new[]                                                      //set the claims and role
            {
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, credential.UserName),
                new Claim("fullName", credential.FullName),
                new Claim("role", credential.UserRole),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: configReader["Jwt:Issuer"],
                audience: configReader["Jwt:Audience"],
                claims: Claims,
                expires: DateTime.Now.AddMinutes(30),                  //Changes
                signingCredentials: SigningCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}