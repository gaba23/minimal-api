using Api.Data;
using Api.Dtos;
using Api.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Api.Handlers;

public static class VeiculoHandlers
{
    public static async Task<IResult> CreateVeiculo(
        VeiculoCreateDto dto,
        AppDbContext db,
        IValidator<VeiculoCreateDto> validator)
    {
        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return Results.BadRequest(validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));

        var entity = new Veiculo
        {
            Marca = dto.Marca,
            Modelo = dto.Modelo,
            Ano = dto.Ano,
            Valor = dto.Valor,
            Placa = dto.Placa
        };

        db.Veiculos.Add(entity);
        await db.SaveChangesAsync();

        var read = new Api.Dtos.VeiculoReadDto
        {
            Id = entity.Id,
            Marca = entity.Marca,
            Modelo = entity.Modelo,
            Ano = entity.Ano,
            Valor = entity.Valor,
            Placa = entity.Placa
        };

        return Results.Created($"/veiculos/{entity.Id}", read);
    }

    public static async Task<IResult> ListVeiculos(AppDbContext db)
    {
        var list = await db.Veiculos
            .Select(v => new Api.Dtos.VeiculoReadDto {
                Id = v.Id, Marca = v.Marca, Modelo = v.Modelo, Ano = v.Ano, Valor = v.Valor, Placa = v.Placa
            })
            .ToListAsync();
        return Results.Ok(list);
    }

    public static async Task<IResult> GetById(int id, AppDbContext db)
    {
        var v = await db.Veiculos.FindAsync(id);
        if (v == null) return Results.NotFound();
        return Results.Ok(new Api.Dtos.VeiculoReadDto {
            Id = v.Id, Marca = v.Marca, Modelo = v.Modelo, Ano = v.Ano, Valor = v.Valor, Placa = v.Placa
        });
    }

    public static async Task<IResult> DeleteVeiculo(int id, AppDbContext db)
    {
        var v = await db.Veiculos.FindAsync(id);
        if (v == null) return Results.NotFound();
        db.Veiculos.Remove(v);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

}
