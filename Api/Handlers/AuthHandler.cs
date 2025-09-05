using Api.Data;
using Api.Dtos;
using Api.Services;
using Microsoft.EntityFrameworkCore;
using Api.Models;

namespace Api.Handlers;

public static class AuthHandlers
{
    public static async Task<IResult> Login(LoginRequest req, AppDbContext db, TokenService tokenService)
    {
        var admin = await db.Administradores.FirstOrDefaultAsync(a => a.Login == req.Login);
        if (admin == null) return Results.Unauthorized();
        if (!BCrypt.Net.BCrypt.Verify(req.Senha, admin.SenhaHash)) return Results.Unauthorized();

        var token = tokenService.GenerateToken(admin);
        return Results.Ok(new { token });
    }
}
