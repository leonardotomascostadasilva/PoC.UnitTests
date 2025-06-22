using Dapper;
using System.Data;

namespace PoC.UnitTests.Wrapper
{
    public interface IDapperWrapper
    {
        Task<T?> QueryFirstOrDefaultAsync<T>(IDbConnection connection, string sql, object? param = null, CancellationToken cancellationToken = default);
    }

    public class DapperWrapper : IDapperWrapper
    {
        public Task<T?> QueryFirstOrDefaultAsync<T>(IDbConnection connection, string sql, object? param = null, CancellationToken cancellationToken = default)
        {
            return connection.QueryFirstOrDefaultAsync<T>(sql, param);
        }
    }
}
