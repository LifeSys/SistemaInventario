using SistemaInventario.Datos;
using SistemaInventario.Entidades;

namespace SistemaInventario.Negocio;

public class VentaService(VentaDAO ventaDao, IProductoRepository productoRepository)
{
    public Task<IReadOnlyList<Venta>> ObtenerVentasAsync(string filtro = "") => ventaDao.ObtenerVentasAsync(filtro);

    public async Task<(bool Exito, string Mensaje)> RegistrarVentaAsync(int idProducto, int cantidad)
    {
        if (SessionManager.UsuarioActual is null)
        {
            return (false, "No hay una sesión activa.");
        }

        if (idProducto <= 0)
        {
            return (false, "Seleccione un producto válido.");
        }

        if (cantidad <= 0)
        {
            return (false, "La cantidad debe ser mayor a 0.");
        }

        return await ventaDao.RegistrarVentaAsync(idProducto, cantidad, SessionManager.UsuarioActual.Id);
    }

    public async Task<decimal> ObtenerPrecioProductoAsync(int idProducto)
    {
        var productos = await productoRepository.ObtenerTodosAsync();
        return productos.FirstOrDefault(p => p.IdProducto == idProducto)?.Precio ?? 0;
    }

    public Task<IReadOnlyList<Producto>> ObtenerProductosAsync() => productoRepository.ObtenerTodosAsync();

    public Task<ReporteResumen> ObtenerReporteAsync() => ventaDao.ObtenerReporteAsync();
}
