using SistemaInventario.Entidades;

namespace SistemaInventario.Negocio;

public static class ProductoValidator
{
    public static ValidationResult Validar(Producto producto)
    {
        var resultado = new ValidationResult();

        if (string.IsNullOrWhiteSpace(producto.Nombre))
        {
            resultado.AgregarError("El nombre del producto es obligatorio.");
        }

        if (producto.Precio <= 0)
        {
            resultado.AgregarError("El precio debe ser mayor que 0.");
        }

        if (producto.Stock < 0)
        {
            resultado.AgregarError("El stock no puede ser negativo.");
        }

        return resultado;
    }
}
