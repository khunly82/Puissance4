using Puissance4.API.Hubs;
using Puissance4.Business.Services;
using Puissance4.Infrastructure.Security;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(p => p.AddDefaultPolicy(b => b
    .WithOrigins("http://localhost:4200")
    .AllowCredentials()
    .AllowAnyHeader()
    .AllowAnyMethod()));

builder.Services.AddSignalR();

builder.Services.AddScoped<JwtSecurityTokenHandler>();
builder.Services.AddScoped<TokenManager>();
TokenManager.Config tokenConfig = builder.Configuration.GetSection("Jwt").Get<TokenManager.Config>() ?? throw new Exception("Jwt Config is missing");
builder.Services.AddSingleton(tokenConfig);

builder.Services.AddScoped<P4Service>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.MapHub<GameHub>("ws/game");

app.Run();
