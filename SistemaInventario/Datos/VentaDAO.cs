using Microsoft.Data.SqlClient;
using SistemaInventario.Entidades;

namespace SistemaInventario.Datos;

public class VentaDAO(string connectionString)
{
    public async Task<IReadOnlyList<Venta>> ObtenerVentasAsync(string filtro = "")
    {
        const string query = @"
            SELECT v.IdVenta, v.IdProducto, p.Nombre, v.Cantidad, v.PrecioUnitario, v.Total, v.Fecha, v.IdUsuario, u.Usuario
            FROM Ventas v
            INNER JOIN Productos p ON p.IdProducto = v.IdProducto
            INNER JOIN Usuarios u ON u.Id = v.IdUsuario
            WHERE @filtro = '' OR p.Nombre LIKE @texto OR u.Usuario LIKE @texto
            ORDER BY v.Fecha DESC";

        var ventas = new List<Venta>();

        using var connection = new SqlConnection(connectionString);
        using var command = new SqlCommand(query, connection);

        var texto = filtro.Trim();
        command.Parameters.AddWithValue("@filtro", texto);
        command.Parameters.AddWithValue("@texto", $"%{texto}%");

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            ventas.Add(MapearVenta(reader));
        }

        return ventas;
    }

    public async Task<(bool Exito, string Mensaje)> RegistrarVentaAsync(int idProducto, int cantidad, int idUsuario)
    {
        const string queryStock = "SELECT Stock, Precio FROM Productos WHERE IdProducto = @idProducto";
        const string queryInsertVenta = @"
            INSERT INTO Ventas (IdProducto, Cantidad, PrecioUnitario, Total, Fecha, IdUsuario)
            VALUES (@idProducto, @cantidad, @precioUnitario, @total, GETDATE(), @idUsuario);";
        const string queryUpdateStock = "UPDATE Productos SET Stock = Stock - @cantidad WHERE IdProducto = @idProducto";

        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        using var transaction = await connection.BeginTransactionAsync();

        try
        {
            using var cmdStock = new SqlCommand(queryStock, connection, (SqlTransaction)transaction);
            cmdStock.Parameters.AddWithValue("@idProducto", idProducto);

            using var reader = await cmdStock.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                await transaction.RollbackAsync();
                return (false, "Producto no encontrado.");
            }

            var stockActual = reader.GetInt32(0);
            var precioUnitario = reader.GetDecimal(1);
            await reader.CloseAsync();

            if (stockActual < cantidad)
            {
                await transaction.RollbackAsync();
                return (false, "Stock insuficiente para completar la venta.");
            }

            var total = precioUnitario * cantidad;

            using var cmdInsert = new SqlCommand(queryInsertVenta, connection, (SqlTransaction)transaction);
            cmdInsert.Parameters.AddWithValue("@idProducto", idProducto);
            cmdInsert.Parameters.AddWithValue("@cantidad", cantidad);
            cmdInsert.Parameters.AddWithValue("@precioUnitario", precioUnitario);
            cmdInsert.Parameters.AddWithValue("@total", total);
            cmdInsert.Parameters.AddWithValue("@idUsuario", idUsuario);
            await cmdInsert.ExecuteNonQueryAsync();

            using var cmdUpdate = new SqlCommand(queryUpdateStock, connection, (SqlTransaction)transaction);
            cmdUpdate.Parameters.AddWithValue("@idProducto", idProducto);
            cmdUpdate.Parameters.AddWithValue("@cantidad", cantidad);
            await cmdUpdate.ExecuteNonQueryAsync();

            await transaction.CommitAsync();
            return (true, "Venta registrada correctamente.");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<ReporteResumen> ObtenerReporteAsync(int topProductos = 5, int stockMinimo = 5)
    {
        var reporte = new ReporteResumen();

        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        const string queryTotalDia = @"
            SELECT ISNULL(SUM(Total), 0)
            FROM Ventas
            WHERE CAST(Fecha AS DATE) = CAST(GETDATE() AS DATE)";

        using (var totalCmd = new SqlCommand(queryTotalDia, connection))
        {
            var total = await totalCmd.ExecuteScalarAsync();
            reporte.TotalVentasDia = total is decimal value ? value : 0;
        }

        const string queryTop = @"
            SELECT TOP (@top) p.Nombre, SUM(v.Cantidad) AS CantidadVendida, SUM(v.Total) AS TotalFacturado
            FROM Ventas v
            INNER JOIN Productos p ON p.IdProducto = v.IdProducto
            GROUP BY p.Nombre
            ORDER BY CantidadVendida DESC";

        var topVendidos = new List<ProductoMasVendido>();
        using (var topCmd = new SqlCommand(queryTop, connection))
        {
            topCmd.Parameters.AddWithValue("@top", topProductos);
            using var reader = await topCmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                topVendidos.Add(new ProductoMasVendido
                {
                    Producto = reader.GetString(0),
                    CantidadVendida = reader.GetInt32(1),
                    TotalFacturado = reader.GetDecimal(2)
                });
            }
        }

        const string queryStockBajo = @"
            SELECT IdProducto, Nombre, Stock
            FROM Productos
            WHERE Stock <= @minimo
            ORDER BY Stock ASC";

        var stockBajo = new List<ProductoStockBajo>();
        using (var stockCmd = new SqlCommand(queryStockBajo, connection))
        {
            stockCmd.Parameters.AddWithValue("@minimo", stockMinimo);
            using var reader = await stockCmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                stockBajo.Add(new ProductoStockBajo
                {
                    IdProducto = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    StockActual = reader.GetInt32(2)
                });
            }
        }

        reporte.ProductosMasVendidos = topVendidos;
        reporte.ProductosStockBajo = stockBajo;
        return reporte;
    }

    private static Venta MapearVenta(SqlDataReader reader)
    {
        return new Venta
        {
            IdVenta = reader.GetInt32(0),
            IdProducto = reader.GetInt32(1),
            NombreProducto = reader.GetString(2),
            Cantidad = reader.GetInt32(3),
            PrecioUnitario = reader.GetDecimal(4),
            Total = reader.GetDecimal(5),
            Fecha = reader.GetDateTime(6),
            IdUsuario = reader.GetInt32(7),
            UsuarioNombre = reader.GetString(8)
        };
    }
}
