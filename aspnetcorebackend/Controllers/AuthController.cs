using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using aspnetcorebackend.Contracts;
using aspnetcorebackend.Helpers;
using aspnetcorebackend.Identity;
using aspnetcorebackend.Models;
using aspnetcorebackend.Models.Entities;
using aspnetcorebackend.Repositories;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Serializers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace aspnetcorebackend.Controllers
{
    [Route("authentication")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserRepository _repo;
        private readonly AppSettings _appSettings;

        public AuthController(ApplicationDbContext context, IUserRepository repo, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _repo = repo;
            _appSettings = appSettings.Value;
        }

        // GET api/values
        [HttpPost]
        public IActionResult Login([FromBody] LoginModel model)
        {
            if (model == null)
                return BadRequest("Invalid client request");

            User user = _repo.Authenticate(model);
            if (user == null)
                return Unauthorized();
            

            var token = new JwtBuilder()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(_appSettings.Secret)
                .Issuer("http://localhost:5000")
                .Audience("http://localhost:5000")
                .IssuedAt(DateTime.UtcNow)
                .AddClaim("exp", DateTimeOffset.UtcNow.AddDays(7).ToUnixTimeSeconds())
                .AddClaim("Role", "admin")
                .Build();
            
            return Ok(new { Token = token });
        }
    }
}