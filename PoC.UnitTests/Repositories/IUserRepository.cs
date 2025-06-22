using PoC.UnitTests.Factories;
using PoC.UnitTests.Wrapper;

namespace PoC.UnitTests.Repositories
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }
    public interface IUserRepository
    {
       Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    }

    public sealed class UserRepository(IDatabaseConnectionFactory databaseConnectionFactory, IDapperWrapper dapperWrapper) : IUserRepository
    {
        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            const string sql = """
                SELECT id, 
                       name,
                       email,
                       created_at AS "CreatedAt" 
                FROM users
                WHERE email = @Email
            """;

            using var connection = await databaseConnectionFactory.CreateConnectionAsync(cancellationToken);

            return await dapperWrapper.QueryFirstOrDefaultAsync<User>(connection, sql, new { Email = email }, cancellationToken);
        }
    }
}
