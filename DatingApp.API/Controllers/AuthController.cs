using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public IAuthRepository Repo { get; }
        public IConfiguration Config { get; }
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            this.Config = config;
            this.Repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto user)
        {
            user.UserName = user.UserName.ToLower();

            if (await Repo.UserExists(user.UserName))
                return BadRequest("User Name already is exists.");

            var userToCreate = new User
            {
                Username = user.UserName
            };

            var createdUser = await Repo.Register(userToCreate, user.Password);

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]UserForLoginDto userForLoginDto)
        {
            var userForRepo = await Repo.Login(userForLoginDto.UserName.ToLower(), userForLoginDto.Password);

            if (userForRepo == null)
            {
                return Unauthorized();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,userForRepo.Id.ToString()),
                new Claim(ClaimTypes.Name,userForRepo.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key , SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor{
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddDays(1),
                    SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new{token = tokenHandler.WriteToken(token)});

        }
    }
}