using SistemaInventario.Negocio;

namespace SistemaInventario.Presentacion;

public class MainForm : Form
{
    private readonly Func<Form1> _productoFormFactory;
    private readonly Func<VentaForm> _ventaFormFactory;
    private readonly Func<UsuarioForm> _usuarioFormFactory;
    private readonly Func<ReporteForm> _reporteFormFactory;

    private readonly Label lblUsuario = new() { AutoSize = true };
    private readonly Button btnProductos = new() { Text = "Productos", Width = 180, Height = 45 };
    private readonly Button btnVentas = new() { Text = "Ventas", Width = 180, Height = 45 };
    private readonly Button btnUsuarios = new() { Text = "Usuarios", Width = 180, Height = 45 };
    private readonly Button btnReportes = new() { Text = "Reportes", Width = 180, Height = 45 };

    public MainForm(
        Func<Form1> productoFormFactory,
        Func<VentaForm> ventaFormFactory,
        Func<UsuarioForm> usuarioFormFactory,
        Func<ReporteForm> reporteFormFactory)
    {
        _productoFormFactory = productoFormFactory;
        _ventaFormFactory = ventaFormFactory;
        _usuarioFormFactory = usuarioFormFactory;
        _reporteFormFactory = reporteFormFactory;

        Text = "Dashboard - Sistema Inventario";
        StartPosition = FormStartPosition.CenterScreen;
        Width = 460;
        Height = 320;

        lblUsuario.Text = $"Usuario: {SessionManager.UsuarioActual?.NombreUsuario} ({SessionManager.UsuarioActual?.Rol})";
        lblUsuario.SetBounds(20, 20, 400, 30);

        btnProductos.SetBounds(20, 70, 180, 45);
        btnVentas.SetBounds(220, 70, 180, 45);
        btnUsuarios.SetBounds(20, 130, 180, 45);
        btnReportes.SetBounds(220, 130, 180, 45);

        btnProductos.Click += (_, _) => AbrirModal(_productoFormFactory());
        btnVentas.Click += (_, _) => AbrirModal(_ventaFormFactory());
        btnUsuarios.Click += (_, _) => AbrirModal(_usuarioFormFactory());
        btnReportes.Click += (_, _) => AbrirModal(_reporteFormFactory());

        Controls.AddRange([lblUsuario, btnProductos, btnVentas, btnUsuarios, btnReportes]);

        AplicarPermisosPorRol();
    }

    private void AbrirModal(Form form)
    {
        using (form)
        {
            form.ShowDialog();
        }
    }

    private void AplicarPermisosPorRol()
    {
        if (SessionManager.EsAdmin)
        {
            return;
        }

        btnProductos.Enabled = false;
        btnUsuarios.Enabled = false;
        btnReportes.Enabled = false;
        btnVentas.Enabled = true;
    }
}
