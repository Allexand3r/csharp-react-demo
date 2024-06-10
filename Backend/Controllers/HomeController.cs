using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CoinTracker_Backend.Database;
using CoinTracker_Backend.Models;

namespace CoinTracker_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserDto userDto)
        {
            if (!string.IsNullOrEmpty(userDto.Name))
            {
                await DbHelper.InsertUserAsync(userDto.Name);
                return Ok(new { message = "Name saved!" });
            }
            return BadRequest(new { message = "Name is required" });
        }
    }

    public class UserDto
    {
        public string Name { get; set; }
    }
}
