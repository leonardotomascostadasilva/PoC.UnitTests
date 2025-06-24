using AutoFixture;
using NSubstitute;
using PoC.UnitTests.Factories;
using PoC.UnitTests.Repositories;
using PoC.UnitTests.Wrapper;
using System.Data;

namespace UnitTests.Respositories
{
    public class UserRepositoryTests
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        private readonly IDapperWrapper _dapperWrapper;
        private readonly UserRepository _userRepository;
        private readonly Fixture _fixture = new();

        public UserRepositoryTests()
        {
            _databaseConnectionFactory = Substitute.For<IDatabaseConnectionFactory>();
            _dapperWrapper = Substitute.For<IDapperWrapper>();

            _userRepository = new UserRepository(_databaseConnectionFactory, _dapperWrapper);
        }

        [Fact]
        public async Task GetByEmailAsync_WhenUserExists_ReturnsUser()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var fakeConnection = Substitute.For<IDbConnection>();

            _databaseConnectionFactory
                .CreateConnectionAsync(cancellationToken)
                .Returns(Task.FromResult(fakeConnection));

            var expectedUser = _fixture
                .Build<User>()
                .With(e => e.Email, "test@example.com")
                .Create();

            const string sql = """
                SELECT id, 
                       name,
                       email,
                       created_at AS "CreatedAt" 
                FROM users
                WHERE email = @Email
            """;

            _dapperWrapper
                .QueryFirstOrDefaultAsync<User>(
                    fakeConnection,
                    sql,
                    Arg.Is<object>(param =>
                        param != null &&
                        param.GetType().GetProperty("Email")!.GetValue(param)!.ToString() == "test@example.com"),
                    cancellationToken)
                .Returns(Task.FromResult<User?>(expectedUser));

            // Act
            var user = await _userRepository.GetByEmailAsync("test@example.com", cancellationToken);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(expectedUser.Email, user!.Email);
            Assert.Equal(expectedUser.Name, user.Name);
        }


        [Fact]
        public void TestUserCreation() => Thread.Sleep(1000);

        [Fact]
        public void TestProductCreation() => Thread.Sleep(1000);

    }
}
