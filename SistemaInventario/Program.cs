using SistemaInventario.Datos;
using SistemaInventario.Negocio;
using SistemaInventario.Presentacion;

namespace SistemaInventario;

internal static class Program
{
    private const string ConnectionString = "Server=localhost\\SQLEXPRESS;Database=InventarioDB;Trusted_Connection=True;TrustServerCertificate=True;";

    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        var productoRepository = new ProductoDAO(ConnectionString);
        var productoService = new ProductoService(productoRepository);

        Application.Run(new Form1(productoService));
    }
}
