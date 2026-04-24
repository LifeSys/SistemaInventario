using SistemaInventario.Entidades;

namespace SistemaInventario.Negocio;

public static class SessionManager
{
    public static Usuario? UsuarioActual { get; private set; }

    public static bool EstaAutenticado => UsuarioActual is not null;

    public static void IniciarSesion(Usuario usuario)
    {
        UsuarioActual = usuario;
    }

    public static void CerrarSesion()
    {
        UsuarioActual = null;
    }

    public static bool EsAdmin => UsuarioActual?.Rol.Equals(Roles.Admin, StringComparison.OrdinalIgnoreCase) == true;
}
