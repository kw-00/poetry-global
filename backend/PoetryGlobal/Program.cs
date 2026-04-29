using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using PoetryGlobal.Config;
using PoetryGlobal.Exceptions;
using PoetryGlobal.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddLogging();


builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton(new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
});

builder.Services.AddSingleton(_ =>
{
    var connectionStringKey = "DB_CONNECTION_STRING";
    var connectionString = Environment.GetEnvironmentVariable(connectionStringKey) 
        ?? throw new EnvironmentVariableNotSetException(connectionStringKey);
    connectionString += ";Search Path=${user},public,app";
    return NpgsqlDataSource.Create(connectionString);
});

builder.Services.AddSingleton<IConfigWithValidation, ConfigWithParsing>();

// FEATURE: Poems
builder.Services.AddSingleton<ILanguageCache, LanguageCache>();
builder.Services.AddSingleton<IPoemSearchCache, PoemSearchCache>();

builder.Services.AddScoped<ILanguageRepository, LanguageRepository>();
builder.Services.AddScoped<IPoemMetadataRepository, PoemMetadataRepository>();
builder.Services.AddScoped<IPoemVersionRepository, PoemVersionRepository>();

builder.Services.AddScoped<IExternalPoetryAPIService, PoetryDbService>();
builder.Services.AddScoped<ITranslationService, LimitedMyMemoryTranslationService>();

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

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["SessionToken"];

                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }

                return Task.CompletedTask;
            }
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

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(exceptionHandlerApp =>
    {
        exceptionHandlerApp.Run(async context =>
        {
            context.Response.StatusCode = context.Features.Get<IExceptionHandlerFeature>()!.Error switch
            {
                UnauthorizedException => 401,
                NotFoundException e => 404,
                _ => 500
            };
            await context.Response.WriteAsync("Internal Server Error");
        });
    });
}

app.Run();


