using SistemaInventario.Entidades;

namespace SistemaInventario.Datos;

public interface IProductoRepository
{
    Task<IReadOnlyList<Producto>> ObtenerTodosAsync();
    Task<IReadOnlyList<Producto>> BuscarPorNombreAsync(string textoBusqueda);
    Task<int> InsertarAsync(Producto producto);
    Task<bool> ActualizarAsync(Producto producto);
    Task<bool> EliminarAsync(int idProducto);
}
