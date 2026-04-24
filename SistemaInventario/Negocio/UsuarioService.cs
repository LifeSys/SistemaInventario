using SistemaInventario.Datos;
using SistemaInventario.Entidades;

namespace SistemaInventario.Negocio;

public class UsuarioService(UsuarioDAO usuarioDao)
{
    public async Task<(bool Exito, string Mensaje)> LoginAsync(string usuario, string password)
    {
        if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(password))
        {
            return (false, "Ingrese usuario y contraseña.");
        }

        var user = await usuarioDao.ObtenerPorCredencialesAsync(usuario.Trim(), PasswordHasher.Hash(password));
        if (user is null)
        {
            return (false, "Credenciales inválidas.");
        }

        SessionManager.IniciarSesion(user);
        return (true, "Bienvenido al sistema.");
    }

    public Task<IReadOnlyList<Usuario>> ObtenerUsuariosAsync(string filtro = "") => usuarioDao.ObtenerTodosAsync(filtro);

    public async Task<(bool Exito, string Mensaje)> CrearUsuarioAsync(Usuario usuario, string passwordPlano)
    {
        var validacion = await ValidarUsuarioAsync(usuario, passwordPlano, false);
        if (!validacion.Exito)
        {
            return validacion;
        }

        usuario.PasswordHash = PasswordHasher.Hash(passwordPlano);
        var id = await usuarioDao.InsertarAsync(usuario);
        return id > 0 ? (true, "Usuario registrado correctamente.") : (false, "No se pudo registrar el usuario.");
    }

    public async Task<(bool Exito, string Mensaje)> ActualizarUsuarioAsync(Usuario usuario, string? nuevaPassword)
    {
        if (usuario.Id <= 0)
        {
            return (false, "Usuario inválido para actualización.");
        }

        var validacion = await ValidarUsuarioAsync(usuario, nuevaPassword, true);
        if (!validacion.Exito)
        {
            return validacion;
        }

        if (!string.IsNullOrWhiteSpace(nuevaPassword))
        {
            usuario.PasswordHash = PasswordHasher.Hash(nuevaPassword);
        }

        var actualizado = await usuarioDao.ActualizarAsync(usuario);
        return actualizado ? (true, "Usuario actualizado correctamente.") : (false, "No se encontró el usuario.");
    }

    public async Task<(bool Exito, string Mensaje)> EliminarUsuarioAsync(int id)
    {
        if (id <= 0)
        {
            return (false, "Seleccione un usuario válido.");
        }

        var eliminado = await usuarioDao.EliminarAsync(id);
        return eliminado ? (true, "Usuario eliminado correctamente.") : (false, "No se encontró el usuario.");
    }

    private async Task<(bool Exito, string Mensaje)> ValidarUsuarioAsync(Usuario usuario, string? passwordPlano, bool esEdicion)
    {
        if (string.IsNullOrWhiteSpace(usuario.NombreUsuario))
        {
            return (false, "El nombre de usuario es obligatorio.");
        }

        if (!usuario.Rol.Equals(Roles.Admin, StringComparison.OrdinalIgnoreCase)
            && !usuario.Rol.Equals(Roles.Vendedor, StringComparison.OrdinalIgnoreCase))
        {
            return (false, "El rol seleccionado no es válido.");
        }

        if (!esEdicion && string.IsNullOrWhiteSpace(passwordPlano))
        {
            return (false, "La contraseña es obligatoria.");
        }

        if (!string.IsNullOrWhiteSpace(passwordPlano) && passwordPlano.Trim().Length < 4)
        {
            return (false, "La contraseña debe tener al menos 4 caracteres.");
        }

        var existe = await usuarioDao.ExisteUsuarioAsync(usuario.NombreUsuario.Trim(), esEdicion ? usuario.Id : 0);
        if (existe)
        {
            return (false, "El usuario ya existe.");
        }

        return (true, string.Empty);
    }
}
