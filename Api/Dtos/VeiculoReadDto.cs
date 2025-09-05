namespace Api.Dtos;

public class VeiculoReadDto
{
    public int Id { get; set; }
    public string Marca { get; set; } = null!;
    public string Modelo { get; set; } = null!;
    public int Ano { get; set; }
    public decimal Valor { get; set; }
    public string Placa { get; set; } = null!;
}
