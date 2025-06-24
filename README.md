# TL;DR

Esta RFC define o padrão para testes unitários em projetos .NET, estabelecendo o uso de **xUnit**, **NSubstitute** e **AutoFixture** como stack principal para garantir qualidade, consistência e manutenibilidade do código.

## Problema

- Falta de padronização em ferramentas de teste
- Testes frágeis e difíceis de manter
- Setup manual repetitivo de objetos
- Inconsistência entre projetos da equipe

## Solução Proposta

### Stack de Ferramentas

| Ferramenta      | Propósito           | Justificativa                                                                               |
| --------------- | ------------------- | ------------------------------------------------------------------------------------------- |
| **xUnit**       | Framework de testes | Padrão da Microsoft<br>Injeção de dependência nativa<br>Melhor isolamento entre testes      |
| **NSubstitute** | Mocking             | Sintaxe limpa e intuitiva<br>Menos verboso que Moq<br>Melhor experiência de desenvolvimento |
| **AutoFixture** | Geração de dados    | Reduz boilerplate<br>Dados realistas automaticamente<br>Menos manutenção                    |

## Implementação

```csharp
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
}

[Theory]
[InlineData("john@example.com")]
[InlineData("alice@example.com")]
[InlineData("bob@example.com")]
public async Task GetByEmailAsync_WithInlineEmail_ReturnsExpectedUser(string email)
{
    // Arrange
    var cancellationToken = CancellationToken.None;
    var fakeConnection = Substitute.For<IDbConnection>();

    _databaseConnectionFactory
        .CreateConnectionAsync(cancellationToken)
        .Returns(Task.FromResult(fakeConnection));

    var expectedUser = _fixture
        .Build<User>()
        .With(e => e.Email, email)
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
                param.GetType().GetProperty("Email")!.GetValue(param)!.ToString() == email),
            cancellationToken)
        .Returns(Task.FromResult<User?>(expectedUser));

    // Act
    var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

    // Assert
    Assert.NotNull(user);
    Assert.Equal(email, user!.Email);
}

public static IEnumerable<object[]> EmailData =>
    new List<object[]>
    {
        new object[] { new User { Email = "test1@example.com", Name = "Test One" } },
        new object[] { new User { Email = "test2@example.com", Name = "Test Two" } },
        new object[] { new User { Email = "test3@example.com", Name = "Test Three" } },
    };

[Theory]
[MemberData(nameof(EmailData))]
public async Task GetByEmailAsync_WithUserObject_ReturnsExpectedUser(User input)
{
    // Arrange
    var cancellationToken = CancellationToken.None;
    var fakeConnection = Substitute.For<IDbConnection>();

    _databaseConnectionFactory
        .CreateConnectionAsync(cancellationToken)
        .Returns(Task.FromResult(fakeConnection));

    var expectedUser = _fixture
        .Build<User>()
        .With(u => u.Email, input.Email)
        .With(u => u.Name, input.Name)
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
                param.GetType().GetProperty("Email")!.GetValue(param)!.ToString() == input.Email),
            cancellationToken)
        .Returns(Task.FromResult<User?>(expectedUser));

    // Act
    var user = await _userRepository.GetByEmailAsync(input.Email, cancellationToken);

    // Assert
    Assert.NotNull(user);
    Assert.Equal(input.Email, user!.Email);
    Assert.Equal(input.Name, user.Name);
}
```

## Benefícios

### xUnit

- **Isolamento**: Cada teste roda em instância separada
- **Flexibilidade**: Suporte nativo a async/await
- **Performance**: Execução paralela por padrão
- **Extensibilidade**: Fácil criação de atributos customizados

### NSubstitute

- **Sintaxe Clara**: `substitute.Method().Returns(value)`
- **Verificação Simples**: `substitute.Received().Method()`
- **Menos Código**: Reduz verbosidade em 30-40%
- **IntelliSense**: Melhor suporte do IDE

### AutoFixture

- **Produtividade**: Elimina 70% do código de setup
- **Manutenibilidade**: Mudanças em modelos não quebram testes
- **Realismo**: Dados mais próximos da realidade
- **Customização**: `_fixture.Customize<User>(c => c.With(x => x.Email, "test@test.com"))`

## Testing Pyramid Strategy

```
    E2E Tests (5%)
   ─────────────────
  Integration Tests (15%)
 ─────────────────────────
	Unit Tests (80%)
```

---
**Status**: Proposta  
**Responsável**: [Leonardo Silva]  
**Data**: Junho 2025  
