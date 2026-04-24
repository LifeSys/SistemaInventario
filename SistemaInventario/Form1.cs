using SistemaInventario.Entidades;
using SistemaInventario.Negocio;

namespace SistemaInventario.Presentacion;

public partial class Form1 : Form
{
    private readonly ProductoService _productoService;
    private int _idSeleccionado;
    private FormMode _modo = FormMode.Nuevo;

    private enum FormMode
    {
        Nuevo,
        Editar
    }

    public Form1(ProductoService productoService)
    {
        _productoService = productoService;
        InitializeComponent();
    }

    private async void Form1_Load(object sender, EventArgs e)
    {
        await CargarDatosAsync();
        LimpiarCampos();
    }

    private async Task CargarDatosAsync()
    {
        try
        {
            var productos = await _productoService.ObtenerProductosAsync();
            dgvProductos.DataSource = productos;
        }
        catch (Exception)
        {
            MessageBox.Show("Ocurrió un error al cargar los productos.", "Inventario", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LimpiarCampos()
    {
        txtNombre.Clear();
        txtPrecio.Clear();
        txtStock.Clear();
        txtNombre.Focus();

        _idSeleccionado = 0;
        CambiarModo(FormMode.Nuevo);
    }

    private void CargarProductoSeleccionado(DataGridViewRow fila)
    {
        _idSeleccionado = Convert.ToInt32(fila.Cells[nameof(Producto.IdProducto)].Value);
        txtNombre.Text = fila.Cells[nameof(Producto.Nombre)].Value?.ToString();
        txtPrecio.Text = fila.Cells[nameof(Producto.Precio)].Value?.ToString();
        txtStock.Text = fila.Cells[nameof(Producto.Stock)].Value?.ToString();
        CambiarModo(FormMode.Editar);
    }

    private Producto? ObtenerProductoDesdeFormulario()
    {
        if (!decimal.TryParse(txtPrecio.Text.Trim(), out var precio))
        {
            MessageBox.Show("Ingrese un precio válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtPrecio.Focus();
            return null;
        }

        if (!int.TryParse(txtStock.Text.Trim(), out var stock))
        {
            MessageBox.Show("Ingrese un stock válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtStock.Focus();
            return null;
        }

        return new Producto
        {
            IdProducto = _idSeleccionado,
            Nombre = txtNombre.Text.Trim(),
            Precio = precio,
            Stock = stock
        };
    }

    private void CambiarModo(FormMode modo)
    {
        _modo = modo;
        lblEstado.Text = modo == FormMode.Nuevo ? "Estado: Nuevo" : "Estado: Edición";
        btnGuardar.Enabled = modo == FormMode.Nuevo;
        btnActualizar.Enabled = modo == FormMode.Editar;
        btnEliminar.Enabled = modo == FormMode.Editar;
    }

    private async void btnGuardar_Click(object sender, EventArgs e)
    {
        var producto = ObtenerProductoDesdeFormulario();
        if (producto is null) return;

        try
        {
            var resultado = await _productoService.CrearProductoAsync(producto);
            MessageBox.Show(resultado.Mensaje, "Inventario", MessageBoxButtons.OK,
                resultado.Exito ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (!resultado.Exito) return;

            await CargarDatosAsync();
            LimpiarCampos();
        }
        catch (Exception)
        {
            MessageBox.Show("No fue posible guardar el producto.", "Inventario", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void btnActualizar_Click(object sender, EventArgs e)
    {
        if (_modo != FormMode.Editar)
        {
            MessageBox.Show("Seleccione un producto para editar.", "Inventario", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var producto = ObtenerProductoDesdeFormulario();
        if (producto is null) return;

        try
        {
            var resultado = await _productoService.ActualizarProductoAsync(producto);
            MessageBox.Show(resultado.Mensaje, "Inventario", MessageBoxButtons.OK,
                resultado.Exito ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (!resultado.Exito) return;

            await CargarDatosAsync();
            LimpiarCampos();
        }
        catch (Exception)
        {
            MessageBox.Show("No fue posible actualizar el producto.", "Inventario", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void btnEliminar_Click(object sender, EventArgs e)
    {
        if (_modo != FormMode.Editar)
        {
            MessageBox.Show("Seleccione un producto para eliminar.", "Inventario", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var confirmacion = MessageBox.Show(
            "¿Desea eliminar el producto seleccionado?",
            "Confirmar eliminación",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirmacion != DialogResult.Yes) return;

        try
        {
            var resultado = await _productoService.EliminarProductoAsync(_idSeleccionado);
            MessageBox.Show(resultado.Mensaje, "Inventario", MessageBoxButtons.OK,
                resultado.Exito ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (!resultado.Exito) return;

            await CargarDatosAsync();
            LimpiarCampos();
        }
        catch (Exception)
        {
            MessageBox.Show("No fue posible eliminar el producto.", "Inventario", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnNuevo_Click(object sender, EventArgs e)
    {
        LimpiarCampos();
    }

    private async void btnListar_Click(object sender, EventArgs e)
    {
        await CargarDatosAsync();
    }

    private async void btnBuscar_Click(object sender, EventArgs e)
    {
        await BuscarAsync();
    }

    private async void txtBuscar_TextChanged(object sender, EventArgs e)
    {
        await BuscarAsync();
    }

    private async Task BuscarAsync()
    {
        try
        {
            var productos = await _productoService.BuscarProductosAsync(txtBuscar.Text);
            dgvProductos.DataSource = productos;
        }
        catch (Exception)
        {
            MessageBox.Show("Ocurrió un error durante la búsqueda.", "Inventario", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void dgvProductos_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;

        var fila = dgvProductos.Rows[e.RowIndex];
        CargarProductoSeleccionado(fila);
    }
}
