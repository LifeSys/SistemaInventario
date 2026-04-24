using SistemaInventario.Datos;
using SistemaInventario.Entidades;

namespace SistemaInventario.Negocio;

public class ProductoService(IProductoRepository productoRepository)
{
    public Task<IReadOnlyList<Producto>> ObtenerProductosAsync()
    {
        return productoRepository.ObtenerTodosAsync();
    }

    public Task<IReadOnlyList<Producto>> BuscarProductosAsync(string textoBusqueda)
    {
        if (string.IsNullOrWhiteSpace(textoBusqueda))
        {
            return productoRepository.ObtenerTodosAsync();
        }

        return productoRepository.BuscarPorNombreAsync(textoBusqueda.Trim());
    }

    public async Task<(bool Exito, string Mensaje)> CrearProductoAsync(Producto producto)
    {
        var validacion = ProductoValidator.Validar(producto);
        if (!validacion.EsValido)
        {
            return (false, string.Join(Environment.NewLine, validacion.Errores));
        }

        var id = await productoRepository.InsertarAsync(producto);
        return id > 0
            ? (true, "Producto registrado correctamente.")
            : (false, "No se pudo registrar el producto.");
    }

    public async Task<(bool Exito, string Mensaje)> ActualizarProductoAsync(Producto producto)
    {
        if (producto.IdProducto <= 0)
        {
            return (false, "Debe seleccionar un producto válido para actualizar.");
        }

        var validacion = ProductoValidator.Validar(producto);
        if (!validacion.EsValido)
        {
            return (false, string.Join(Environment.NewLine, validacion.Errores));
        }

        var actualizado = await productoRepository.ActualizarAsync(producto);
        return actualizado
            ? (true, "Producto actualizado correctamente.")
            : (false, "No se encontró el producto a actualizar.");
    }

    public async Task<(bool Exito, string Mensaje)> EliminarProductoAsync(int idProducto)
    {
        if (idProducto <= 0)
        {
            return (false, "Debe seleccionar un producto válido para eliminar.");
        }

        var eliminado = await productoRepository.EliminarAsync(idProducto);
        return eliminado
            ? (true, "Producto eliminado correctamente.")
            : (false, "No se encontró el producto a eliminar.");
    }
}
