using PoC.UnitTests.Factories;
using PoC.UnitTests.Repositories;
using PoC.UnitTests.Wrapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//DI
var connectionString = builder.Configuration.GetConnectionString("Postgres");

if (string.IsNullOrEmpty(connectionString))
    throw new ArgumentException("Connection string is null or empty");

builder.Services.AddSingleton<IDatabaseConnectionFactory>(
    _ => new PostgresDatabaseConnection(connectionString));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDapperWrapper, DapperWrapper>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
