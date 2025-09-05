using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Api.Data;
using Api.Services;
using FluentValidation;
using Api.Validators;
using Api.Dtos;
using Api.Handlers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configs
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Coloque: Bearer {token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            new string[] {}
        }
    });
});

var jwtCfg = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtCfg["Key"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtCfg["Issuer"],
            ValidAudience = jwtCfg["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EditorOrAdmin", p => p.RequireRole("Editor", "Admin"));
    options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
});

builder.Services.AddSingleton<TokenService>();
builder.Services.AddValidatorsFromAssemblyContaining<VeiculoValidator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Ok(new { mensagem = "Bem vindo a API de veículos - Minimal API", doc = "/swagger" }));

var auth = app.MapGroup("/auth").WithTags("Auth");
auth.MapPost("/login", AuthHandlers.Login).AllowAnonymous();

var veiculos = app.MapGroup("/veiculos").WithTags("Veículos");
veiculos.MapGet("/", VeiculoHandlers.ListVeiculos).RequireAuthorization();
veiculos.MapGet("/{id:int}", VeiculoHandlers.GetById).RequireAuthorization();
veiculos.MapPost("/", VeiculoHandlers.CreateVeiculo).RequireAuthorization("EditorOrAdmin");
veiculos.MapDelete("/{id:int}", VeiculoHandlers.DeleteVeiculo).RequireAuthorization("AdminOnly");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    if (!db.Administradores.Any())
    {
        db.Administradores.Add(new Api.Models.Administrador
        {
            Nome = "Admin",
            Login = "admin",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            Perfil = "Admin"
        });
        db.SaveChanges();
    }
}

app.Run();

public partial class Program { }
