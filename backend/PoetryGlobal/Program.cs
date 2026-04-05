using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PoetryGlobal.Application.Exceptions;
using PoetryGlobal.Features.Auth;
using PoetryGlobal.Features.Languages;
using PoetryGlobal.Features.Poems;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// FEATURE: Auth
builder.Services.AddScoped<IAuthService, AuthService>();

// FEATURE: Languages
builder.Services.AddScoped<ILanguagesService, LanguagesService>();

// FEATURE: Poems
builder.Services.AddScoped<IPoemsService, PoemsService>();
builder.Services.AddScoped<IPoetryDbService, PoetryDbService>();
builder.Services.AddScoped<IDatabaseService, DatabaseService>();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.Run();


