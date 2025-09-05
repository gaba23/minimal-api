namespace Api.Models;

public class Administrador
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Login { get; set; } = null!;
    public string SenhaHash { get; set; } = null!;
    public string Perfil { get; set; } = "Admin";
}
