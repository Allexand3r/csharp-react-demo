// Database/DbHelper.cs
using Npgsql;
using System.Threading.Tasks;

namespace CoinTracker_Backend.Database
{
    public class DbHelper
    {
        public static string ConnString = "User ID=postgres;Password=password;Host=localhost;Port=5432;Database=test123";

        public static async Task InsertUserAsync(string name)
        {
            await using var conn = new NpgsqlConnection(ConnString);
            await conn.OpenAsync();

            var commandText = "INSERT INTO AppUser (Name) VALUES (@name)";
            await using var cmd = new NpgsqlCommand(commandText, conn);
            cmd.Parameters.AddWithValue("name", name);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}
