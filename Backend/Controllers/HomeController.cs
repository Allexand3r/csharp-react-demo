using Microsoft.AspNetCore.Mvc;
using CoinTracker_Backend.Models;
using CoinTracker_Backend.ViewMapper;
using CoinTracker_Backend.ViewModels;
using System.Threading.Tasks;

namespace CoinTracker_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
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
                        return Ok(new { message = "Login successful" });
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
    }
}
