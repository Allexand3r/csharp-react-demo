using Microsoft.EntityFrameworkCore;
using Npgsql;
using Dapper;
using System.Threading.Tasks;

namespace CoinTracker_Backend.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public async Task<UserModel> GetUser(string email)
        {
            using (var connection = new NpgsqlConnection(DbHelper.ConnString))
            {
                connection.Open();
                return await connection.QueryFirstOrDefaultAsync<UserModel>(@"
                    SELECT UserId, Email, Password, Salt, Status
                    FROM AppUser
                    WHERE Email = @Email", new { Email = email }) ?? new UserModel();
            }
        }

        public async Task<UserModel> GetUser(int id)
        {
            using (var connection = new NpgsqlConnection(DbHelper.ConnString))
            {
                connection.Open();
                return await connection.QueryFirstOrDefaultAsync<UserModel>(@"
                    SELECT UserId, Email, Password, Salt, Status
                    FROM AppUser
                    WHERE UserId = @Id", new { Id = id }) ?? new UserModel();
            }
        }

        public async Task<int> CreateUser(UserModel model)
        {
            using (var connection = new NpgsqlConnection(DbHelper.ConnString))
            {
                connection.Open();
                string sql = @"
                    INSERT INTO AppUser (Email, Password, Salt, Status)
                    VALUES (@Email, @Password, @Salt, @Status)";
                return await connection.ExecuteAsync(sql, model);
            }
        }
    }
}
