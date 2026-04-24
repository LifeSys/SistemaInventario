namespace SistemaInventario.Entidades;

public class Usuario
{
    public int Id { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Rol { get; set; } = Roles.Vendedor;
}

public static class Roles
{
    public const string Admin = "Admin";
    public const string Vendedor = "Vendedor";
}
