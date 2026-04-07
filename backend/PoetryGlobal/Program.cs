using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using PoetryGlobal.Exceptions;
using PoetryGlobal.Features.Auth;
using PoetryGlobal.Features.Poems;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHttpClient();
builder.Services.AddSingleton(_ =>
{
    var connectionStringKey = "DB_CONNECTION_STRING";
    var connectionString = Environment.GetEnvironmentVariable(connectionStringKey) 
        ?? throw new EnvironmentVariableNotSetException(connectionStringKey);
    connectionString += ";Search Path=${user},public,app";
    return NpgsqlDataSource.Create(connectionString);
});

// FEATURE: Auth
builder.Services.AddScoped<IAuthService, AuthService>();

// FEATURE: Poems
builder.Services.AddScoped<ILanguageRepository, LanguageRepository>();
builder.Services.AddScoped<IPoemRepository, PoemRepository>();
builder.Services.AddScoped<IPoetryDbService, PoetryDbService>();
builder.Services.AddScoped<ITranslationService, TranslationService>();
builder.Services.AddScoped<IPoemOrchestration, PoemOrchestration>();

builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var secret = Environment.GetEnvironmentVariable("JWT_SECRET")
            ?? throw new EnvironmentVariableNotSetException("JWT_SECRET");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret))
        };
    });




var app = builder.Build();
app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();


app.Run();


