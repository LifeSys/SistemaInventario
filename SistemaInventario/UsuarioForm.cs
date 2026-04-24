using SistemaInventario.Entidades;
using SistemaInventario.Negocio;

namespace SistemaInventario.Presentacion;

public class UsuarioForm : Form
{
    private readonly UsuarioService _usuarioService;
    private int _idSeleccionado;

    private readonly TextBox txtUsuario = new();
    private readonly TextBox txtPassword = new() { UseSystemPasswordChar = true };
    private readonly ComboBox cboRol = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox txtBuscar = new() { PlaceholderText = "Buscar usuario" };
    private readonly DataGridView dgvUsuarios = new() { ReadOnly = true, AutoGenerateColumns = true };
    private readonly Button btnGuardar = new() { Text = "Guardar" };
    private readonly Button btnActualizar = new() { Text = "Actualizar", Enabled = false };
    private readonly Button btnEliminar = new() { Text = "Eliminar", Enabled = false };
    private readonly Button btnNuevo = new() { Text = "Nuevo" };

    public UsuarioForm(UsuarioService usuarioService)
    {
        _usuarioService = usuarioService;

        Text = "Administración de Usuarios";
        StartPosition = FormStartPosition.CenterParent;
        Width = 900;
        Height = 520;

        var lblUsuario = new Label { Text = "Usuario", AutoSize = true };
        var lblPassword = new Label { Text = "Contraseña", AutoSize = true };
        var lblRol = new Label { Text = "Rol", AutoSize = true };

        lblUsuario.SetBounds(20, 22, 70, 25);
        txtUsuario.SetBounds(100, 20, 180, 28);

        lblPassword.SetBounds(300, 22, 70, 25);
        txtPassword.SetBounds(380, 20, 180, 28);

        lblRol.SetBounds(580, 22, 40, 25);
        cboRol.SetBounds(630, 20, 120, 28);

        btnGuardar.SetBounds(20, 60, 120, 32);
        btnActualizar.SetBounds(150, 60, 120, 32);
        btnEliminar.SetBounds(280, 60, 120, 32);
        btnNuevo.SetBounds(410, 60, 120, 32);

        txtBuscar.SetBounds(550, 62, 300, 28);
        dgvUsuarios.SetBounds(20, 105, 830, 355);

        cboRol.Items.AddRange([Roles.Admin, Roles.Vendedor]);
        cboRol.SelectedIndex = 1;

        txtBuscar.TextChanged += async (_, _) => await CargarDatosAsync(txtBuscar.Text);
        btnGuardar.Click += BtnGuardar_Click;
        btnActualizar.Click += BtnActualizar_Click;
        btnEliminar.Click += BtnEliminar_Click;
        btnNuevo.Click += (_, _) => LimpiarCampos();
        dgvUsuarios.CellClick += DgvUsuarios_CellClick;

        Controls.AddRange([lblUsuario, txtUsuario, lblPassword, txtPassword, lblRol, cboRol, btnGuardar, btnActualizar, btnEliminar, btnNuevo, txtBuscar, dgvUsuarios]);

        Load += async (_, _) =>
        {
            await CargarDatosAsync();
            LimpiarCampos();
        };
    }

    private async Task CargarDatosAsync(string filtro = "")
    {
        dgvUsuarios.DataSource = await _usuarioService.ObtenerUsuariosAsync(filtro);
    }

    private void LimpiarCampos()
    {
        _idSeleccionado = 0;
        txtUsuario.Clear();
        txtPassword.Clear();
        cboRol.SelectedIndex = 1;
        CambiarModo(false);
    }

    private void CambiarModo(bool edicion)
    {
        btnGuardar.Enabled = !edicion;
        btnActualizar.Enabled = edicion;
        btnEliminar.Enabled = edicion;
    }

    private Usuario ObtenerEntidad() => new()
    {
        Id = _idSeleccionado,
        NombreUsuario = txtUsuario.Text.Trim(),
        Rol = cboRol.SelectedItem?.ToString() ?? Roles.Vendedor
    };

    private async void BtnGuardar_Click(object? sender, EventArgs e)
    {
        var resultado = await _usuarioService.CrearUsuarioAsync(ObtenerEntidad(), txtPassword.Text);
        MessageBox.Show(resultado.Mensaje, "Usuarios", MessageBoxButtons.OK,
            resultado.Exito ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

        if (!resultado.Exito) return;

        await CargarDatosAsync(txtBuscar.Text);
        LimpiarCampos();
    }

    private async void BtnActualizar_Click(object? sender, EventArgs e)
    {
        var usuario = ObtenerEntidad();
        var seleccionado = dgvUsuarios.CurrentRow?.DataBoundItem as Usuario;
        usuario.PasswordHash = seleccionado?.PasswordHash ?? string.Empty;

        var resultado = await _usuarioService.ActualizarUsuarioAsync(usuario, string.IsNullOrWhiteSpace(txtPassword.Text) ? null : txtPassword.Text);
        MessageBox.Show(resultado.Mensaje, "Usuarios", MessageBoxButtons.OK,
            resultado.Exito ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

        if (!resultado.Exito) return;

        await CargarDatosAsync(txtBuscar.Text);
        LimpiarCampos();
    }

    private async void BtnEliminar_Click(object? sender, EventArgs e)
    {
        if (_idSeleccionado <= 0) return;

        if (MessageBox.Show("¿Eliminar usuario seleccionado?", "Usuarios", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
        {
            return;
        }

        var resultado = await _usuarioService.EliminarUsuarioAsync(_idSeleccionado);
        MessageBox.Show(resultado.Mensaje, "Usuarios", MessageBoxButtons.OK,
            resultado.Exito ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

        if (!resultado.Exito) return;

        await CargarDatosAsync(txtBuscar.Text);
        LimpiarCampos();
    }

    private void DgvUsuarios_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;

        if (dgvUsuarios.Rows[e.RowIndex].DataBoundItem is not Usuario usuario) return;

        _idSeleccionado = usuario.Id;
        txtUsuario.Text = usuario.NombreUsuario;
        txtPassword.Clear();
        cboRol.SelectedItem = usuario.Rol;
        CambiarModo(true);
    }
}
