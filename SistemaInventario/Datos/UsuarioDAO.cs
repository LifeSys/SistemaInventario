using Microsoft.Data.SqlClient;
using SistemaInventario.Entidades;

namespace SistemaInventario.Datos;

public class UsuarioDAO(string connectionString)
{
    public async Task<Usuario?> ObtenerPorCredencialesAsync(string usuario, string passwordHash)
    {
        const string query = @"
            SELECT Id, Usuario, Password, Rol
            FROM Usuarios
            WHERE Usuario = @usuario AND Password = @password";

        using var connection = new SqlConnection(connectionString);
        using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@usuario", usuario);
        command.Parameters.AddWithValue("@password", passwordHash);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return MapearUsuario(reader);
    }

    public async Task<IReadOnlyList<Usuario>> ObtenerTodosAsync(string filtro = "")
    {
        const string baseQuery = @"
            SELECT Id, Usuario, Password, Rol
            FROM Usuarios
            WHERE @filtro = '' OR Usuario LIKE @texto
            ORDER BY Usuario";

        var usuarios = new List<Usuario>();

        using var connection = new SqlConnection(connectionString);
        using var command = new SqlCommand(baseQuery, connection);

        var texto = filtro.Trim();
        command.Parameters.AddWithValue("@filtro", texto);
        command.Parameters.AddWithValue("@texto", $"%{texto}%");

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            usuarios.Add(MapearUsuario(reader));
        }

        return usuarios;
    }

    public async Task<int> InsertarAsync(Usuario usuario)
    {
        const string query = @"
            INSERT INTO Usuarios (Usuario, Password, Rol)
            VALUES (@usuario, @password, @rol);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        using var connection = new SqlConnection(connectionString);
        using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@usuario", usuario.NombreUsuario);
        command.Parameters.AddWithValue("@password", usuario.PasswordHash);
        command.Parameters.AddWithValue("@rol", usuario.Rol);

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();

        return result is int id ? id : 0;
    }

    public async Task<bool> ActualizarAsync(Usuario usuario)
    {
        const string query = @"
            UPDATE Usuarios
            SET Usuario = @usuario,
                Password = @password,
                Rol = @rol
            WHERE Id = @id";

        using var connection = new SqlConnection(connectionString);
        using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@id", usuario.Id);
        command.Parameters.AddWithValue("@usuario", usuario.NombreUsuario);
        command.Parameters.AddWithValue("@password", usuario.PasswordHash);
        command.Parameters.AddWithValue("@rol", usuario.Rol);

        await connection.OpenAsync();
        var rows = await command.ExecuteNonQueryAsync();

        return rows > 0;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        const string query = "DELETE FROM Usuarios WHERE Id = @id";

        using var connection = new SqlConnection(connectionString);
        using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@id", id);

        await connection.OpenAsync();
        var rows = await command.ExecuteNonQueryAsync();

        return rows > 0;
    }

    public async Task<bool> ExisteUsuarioAsync(string usuario, int idExcluir = 0)
    {
        const string query = @"
            SELECT COUNT(1)
            FROM Usuarios
            WHERE Usuario = @usuario AND (@idExcluir = 0 OR Id <> @idExcluir)";

        using var connection = new SqlConnection(connectionString);
        using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@usuario", usuario);
        command.Parameters.AddWithValue("@idExcluir", idExcluir);

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();

        return result is int count && count > 0;
    }

    private static Usuario MapearUsuario(SqlDataReader reader)
    {
        return new Usuario
        {
            Id = reader.GetInt32(0),
            NombreUsuario = reader.GetString(1),
            PasswordHash = reader.GetString(2),
            Rol = reader.GetString(3)
        };
    }
}
