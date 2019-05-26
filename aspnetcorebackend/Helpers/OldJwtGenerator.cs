using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace aspnetcorebackend.Helpers
{
    public static class OldJwtGenerator
    {
        public static object CreateJwt(SymmetricSecurityKey secretKey)
        {
//            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Secret)); // In Controller

            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var tokenOptions = new JwtSecurityToken(
                issuer: "http://localhost:5000", // name of the web server that issues the token
                audience: "http://localhost:5000", // representing valid recipients
                claims: new
                    List<Claim> // This is list of user roles, for example, the user can be an admin, manager or author 
                    {
                        new Claim("", ""), // TODO: Add claims
                    },
                expires: DateTime.Now
                    .AddDays(7), // date and time after which the token expires
                signingCredentials: signinCredentials
            );


            var payload = new Dictionary<string, object>
            {
                {"claim1", 0},
                {"claim2", "claim2-value"}
            };
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            var responseToken = new {Token = tokenString};

            return responseToken;
        }
    }
}