using Microsoft.AspNetCore.Mvc;
using CoinTracker_Backend.Models;
using CoinTracker_Backend.ViewMapper;
using CoinTracker_Backend.ViewModels;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace CoinTracker_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public HomeController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _context.GetUser(model.Email);
                if (existingUser?.Email != null)
                {
                    return Ok(new { message = "Already registered", status = "warning" });
                }

                var userModel = AuthMapper.MapRegisterViewModelToUserModel(model);
                userModel.Salt = GenerateSalt();
                userModel.Password = HashPassword(model.Password, userModel.Salt);
                userModel.Status = 1;

                var result = await _context.CreateUser(userModel);
                if (result > 0)
                {
                    return Ok(new { message = "User registered successfully" });
                }
                else
                {
                    return StatusCode(500, new { message = "Error creating user" });
                }
            }

            return BadRequest(new { message = "Invalid model" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.GetUser(model.Email);
                if (user?.Email != null)
                {
                    var hashedPassword = HashPassword(model.Password, user.Salt);
                    if (user.Password == hashedPassword)
                    {
                        var token = GenerateJwtToken(user);
                        return Ok(new
                        {
                            message = "Login successful",
                            token = token
                        });
                    }
                }

                return BadRequest(new { message = "Invalid email or password" });
            }

            return BadRequest(new { message = "Invalid model" });
        }

        private string GenerateSalt()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        private string HashPassword(string password, string salt)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var saltedPassword = $"{password}{salt}";
                var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(saltedPassword));
                return Convert.ToBase64String(bytes);
            }
        }

        private string GenerateJwtToken(UserModel user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings.GetValue<string>("Secret");
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
