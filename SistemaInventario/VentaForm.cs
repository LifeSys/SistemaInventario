using SistemaInventario.Entidades;
using SistemaInventario.Negocio;

namespace SistemaInventario.Presentacion;

public class VentaForm : Form
{
    private readonly VentaService _ventaService;

    private readonly ComboBox cboProductos = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox txtPrecio = new() { ReadOnly = true };
    private readonly NumericUpDown nudCantidad = new() { Minimum = 1, Maximum = 100000, Value = 1 };
    private readonly TextBox txtTotal = new() { ReadOnly = true };
    private readonly TextBox txtBuscar = new() { PlaceholderText = "Buscar venta por producto/usuario" };
    private readonly DataGridView dgvVentas = new() { ReadOnly = true, AutoGenerateColumns = true };
    private readonly Button btnVender = new() { Text = "Registrar venta" };

    public VentaForm(VentaService ventaService)
    {
        _ventaService = ventaService;

        Text = "Gestión de Ventas";
        StartPosition = FormStartPosition.CenterParent;
        Width = 900;
        Height = 520;

        var lblProducto = new Label { Text = "Producto", AutoSize = true };
        var lblPrecio = new Label { Text = "Precio", AutoSize = true };
        var lblCantidad = new Label { Text = "Cantidad", AutoSize = true };
        var lblTotal = new Label { Text = "Total", AutoSize = true };

        lblProducto.SetBounds(20, 22, 70, 25);
        cboProductos.SetBounds(95, 20, 240, 28);

        lblPrecio.SetBounds(360, 22, 50, 25);
        txtPrecio.SetBounds(415, 20, 120, 28);

        lblCantidad.SetBounds(550, 22, 60, 25);
        nudCantidad.SetBounds(615, 20, 70, 28);

        lblTotal.SetBounds(700, 22, 40, 25);
        txtTotal.SetBounds(745, 20, 120, 28);

        btnVender.SetBounds(20, 60, 180, 32);
        txtBuscar.SetBounds(220, 62, 400, 28);
        dgvVentas.SetBounds(20, 105, 845, 355);

        cboProductos.SelectedIndexChanged += async (_, _) => await RefrescarPrecioYTotalAsync();
        nudCantidad.ValueChanged += (_, _) => CalcularTotal();
        txtBuscar.TextChanged += async (_, _) => await CargarDatosAsync(txtBuscar.Text);
        btnVender.Click += BtnVender_Click;

        Controls.AddRange([lblProducto, cboProductos, lblPrecio, txtPrecio, lblCantidad, nudCantidad, lblTotal, txtTotal, btnVender, txtBuscar, dgvVentas]);

        Load += async (_, _) =>
        {
            await CargarProductosAsync();
            await CargarDatosAsync();
            LimpiarCampos();
        };
    }

    private async Task CargarProductosAsync()
    {
        var productos = await _ventaService.ObtenerProductosAsync();
        cboProductos.DataSource = productos.ToList();
        cboProductos.DisplayMember = nameof(Producto.Nombre);
        cboProductos.ValueMember = nameof(Producto.IdProducto);
    }

    private async Task CargarDatosAsync(string filtro = "")
    {
        var ventas = await _ventaService.ObtenerVentasAsync(filtro);
        dgvVentas.DataSource = ventas;
    }

    private async Task RefrescarPrecioYTotalAsync()
    {
        if (cboProductos.SelectedValue is not int idProducto)
        {
            txtPrecio.Text = "0.00";
            txtTotal.Text = "0.00";
            return;
        }

        var precio = await _ventaService.ObtenerPrecioProductoAsync(idProducto);
        txtPrecio.Text = precio.ToString("N2");
        CalcularTotal();
    }

    private void CalcularTotal()
    {
        if (!decimal.TryParse(txtPrecio.Text, out var precio))
        {
            txtTotal.Text = "0.00";
            return;
        }

        txtTotal.Text = (precio * nudCantidad.Value).ToString("N2");
    }

    private async void BtnVender_Click(object? sender, EventArgs e)
    {
        if (cboProductos.SelectedValue is not int idProducto)
        {
            MessageBox.Show("Seleccione un producto.", "Ventas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var resultado = await _ventaService.RegistrarVentaAsync(idProducto, (int)nudCantidad.Value);
        MessageBox.Show(resultado.Mensaje, "Ventas", MessageBoxButtons.OK,
            resultado.Exito ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

        if (!resultado.Exito) return;

        await CargarDatosAsync(txtBuscar.Text);
        await CargarProductosAsync();
        LimpiarCampos();
    }

    private void LimpiarCampos()
    {
        nudCantidad.Value = 1;
        if (cboProductos.Items.Count > 0)
        {
            cboProductos.SelectedIndex = 0;
        }
    }
}
