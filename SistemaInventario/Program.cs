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

        var usuarioDao = new UsuarioDAO(ConnectionString);
        var usuarioService = new UsuarioService(usuarioDao);

        var ventaDao = new VentaDAO(ConnectionString);
        var ventaService = new VentaService(ventaDao, productoRepository);

        MainForm MainFactory() => new MainForm(
            () => new Form1(productoService),
            () => new VentaForm(ventaService),
            () => new UsuarioForm(usuarioService),
            () => new ReporteForm(ventaService));

        Application.Run(new LoginForm(usuarioService, MainFactory));
    }
}
