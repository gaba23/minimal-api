using FluentValidation;
using Api.Dtos;

namespace Api.Validators;

public class VeiculoValidator : AbstractValidator<VeiculoCreateDto>
{
    public VeiculoValidator()
    {
        RuleFor(x => x.Marca).NotEmpty().MinimumLength(2);
        RuleFor(x => x.Modelo).NotEmpty().MinimumLength(1);
        RuleFor(x => x.Ano).InclusiveBetween(1950, DateTime.Now.Year + 1);
        RuleFor(x => x.Valor).GreaterThan(0);
        RuleFor(x => x.Placa).NotEmpty().Length(7); // ajuste conforme padrão
    }
}
