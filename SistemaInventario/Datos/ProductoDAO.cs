using Microsoft.Data.SqlClient;
using SistemaInventario.Entidades;

namespace SistemaInventario.Datos;

public class ProductoDAO(string connectionString) : IProductoRepository
{
    public async Task<IReadOnlyList<Producto>> ObtenerTodosAsync()
    {
        const string query = "SELECT IdProducto, Nombre, Precio, Stock FROM Productos ORDER BY IdProducto DESC";

        var productos = new List<Producto>();

        using var connection = new SqlConnection(connectionString);
        using var command = new SqlCommand(query, connection);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            productos.Add(MapearProducto(reader));
        }

        return productos;
    }

    public async Task<IReadOnlyList<Producto>> BuscarPorNombreAsync(string textoBusqueda)
    {
        const string query = @"
            SELECT IdProducto, Nombre, Precio, Stock
            FROM Productos
            WHERE Nombre LIKE @buscar
            ORDER BY Nombre";

        var productos = new List<Producto>();

        using var connection = new SqlConnection(connectionString);
        using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@buscar", $"%{textoBusqueda}%");

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            productos.Add(MapearProducto(reader));
        }

        return productos;
    }

    public async Task<int> InsertarAsync(Producto producto)
    {
        const string query = @"
            INSERT INTO Productos (Nombre, Precio, Stock)
            VALUES (@nombre, @precio, @stock);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        using var connection = new SqlConnection(connectionString);
        using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@nombre", producto.Nombre);
        command.Parameters.AddWithValue("@precio", producto.Precio);
        command.Parameters.AddWithValue("@stock", producto.Stock);

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();

        return result is int idGenerado ? idGenerado : 0;
    }

    public async Task<bool> ActualizarAsync(Producto producto)
    {
        const string query = @"
            UPDATE Productos
            SET Nombre = @nombre, Precio = @precio, Stock = @stock
            WHERE IdProducto = @id";

        using var connection = new SqlConnection(connectionString);
        using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@id", producto.IdProducto);
        command.Parameters.AddWithValue("@nombre", producto.Nombre);
        command.Parameters.AddWithValue("@precio", producto.Precio);
        command.Parameters.AddWithValue("@stock", producto.Stock);

        await connection.OpenAsync();
        var rows = await command.ExecuteNonQueryAsync();

        return rows > 0;
    }

    public async Task<bool> EliminarAsync(int idProducto)
    {
        const string query = "DELETE FROM Productos WHERE IdProducto = @id";

        using var connection = new SqlConnection(connectionString);
        using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@id", idProducto);

        await connection.OpenAsync();
        var rows = await command.ExecuteNonQueryAsync();

        return rows > 0;
    }

    private static Producto MapearProducto(SqlDataReader reader)
    {
        return new Producto
        {
            IdProducto = reader.GetInt32(0),
            Nombre = reader.GetString(1),
            Precio = reader.GetDecimal(2),
            Stock = reader.GetInt32(3)
        };
    }
}
