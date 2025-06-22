using Npgsql;
using System.Data;

namespace PoC.UnitTests.Factories
{
    public interface IDatabaseConnectionFactory
    {
        public Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken);
    }

    public sealed class PostgresDatabaseConnection(string connectionString) : IDatabaseConnectionFactory
    {
        public async Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken)
        {
            var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);
            return connection;
        }
    }
}
