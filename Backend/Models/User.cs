// Models/User.cs
namespace CoinTracker_Backend.Models
{
    public class User
    {
        public int UserId { get; set; }  // Соответствует userId в таблице AppUser
        public string Name { get; set; } // Соответствует Name в таблице AppUser
    }
}
