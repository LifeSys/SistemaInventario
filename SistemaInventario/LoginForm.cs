using SistemaInventario.Negocio;

namespace SistemaInventario.Presentacion;

public class LoginForm : Form
{
    private readonly UsuarioService _usuarioService;
    private readonly Func<MainForm> _mainFactory;

    private readonly TextBox txtUsuario = new() { PlaceholderText = "Usuario" };
    private readonly TextBox txtPassword = new() { PlaceholderText = "Contraseña", UseSystemPasswordChar = true };
    private readonly Button btnLogin = new() { Text = "Iniciar sesión" };

    public LoginForm(UsuarioService usuarioService, Func<MainForm> mainFactory)
    {
        _usuarioService = usuarioService;
        _mainFactory = mainFactory;
        Text = "Login - Sistema Inventario";
        StartPosition = FormStartPosition.CenterScreen;
        Width = 380;
        Height = 220;

        txtUsuario.SetBounds(40, 30, 280, 30);
        txtPassword.SetBounds(40, 70, 280, 30);
        btnLogin.SetBounds(40, 115, 280, 35);

        btnLogin.Click += BtnLogin_Click;

        Controls.AddRange([txtUsuario, txtPassword, btnLogin]);
    }

    private async void BtnLogin_Click(object? sender, EventArgs e)
    {
        btnLogin.Enabled = false;
        try
        {
            var resultado = await _usuarioService.LoginAsync(txtUsuario.Text, txtPassword.Text);
            if (!resultado.Exito)
            {
                MessageBox.Show(resultado.Mensaje, "Autenticación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Hide();
            using var main = _mainFactory();
            main.ShowDialog();
            Close();
        }
        catch (Exception)
        {
            MessageBox.Show("No fue posible iniciar sesión.", "Autenticación", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnLogin.Enabled = true;
        }
    }
}
