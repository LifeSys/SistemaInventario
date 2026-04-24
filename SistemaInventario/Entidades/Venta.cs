namespace SistemaInventario.Entidades;

public class Venta
{
    public int IdVenta { get; set; }
    public int IdProducto { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Total { get; set; }
    public DateTime Fecha { get; set; }
    public int IdUsuario { get; set; }
    public string UsuarioNombre { get; set; } = string.Empty;
}

public class ReporteResumen
{
    public decimal TotalVentasDia { get; set; }
    public IReadOnlyList<ProductoMasVendido> ProductosMasVendidos { get; set; } = [];
    public IReadOnlyList<ProductoStockBajo> ProductosStockBajo { get; set; } = [];
}

public class ProductoMasVendido
{
    public string Producto { get; set; } = string.Empty;
    public int CantidadVendida { get; set; }
    public decimal TotalFacturado { get; set; }
}

public class ProductoStockBajo
{
    public int IdProducto { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int StockActual { get; set; }
}
