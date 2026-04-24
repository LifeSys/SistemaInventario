using System.Data;
using Microsoft.Data.SqlClient;

namespace SistemaInventario
{
    public partial class Form1 : Form
    {
        string conexion = "Server=localhost\\SQLEXPRESS;Database=InventarioDB;Trusted_Connection=True;TrustServerCertificate=True;";
        int idSeleccionado = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Listar();
        }

        // ================= VALIDAR =================
        bool Validar()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Ingrese nombre");
                return false;
            }

            if (!decimal.TryParse(txtPrecio.Text, out _))
            {
                MessageBox.Show("Precio inválido");
                return false;
            }

            if (!int.TryParse(txtStock.Text, out _))
            {
                MessageBox.Show("Stock inválido");
                return false;
            }

            return true;
        }

        // ================= LIMPIAR =================
        void Limpiar()
        {
            txtNombre.Clear();
            txtPrecio.Clear();
            txtStock.Clear();
            txtNombre.Focus();
            idSeleccionado = 0;
        }

        // ================= LISTAR =================
        void Listar()
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Productos", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvProductos.DataSource = dt;
            }
        }

        // ================= GUARDAR =================
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Validar()) return;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                con.Open();

                string query = "INSERT INTO Productos (Nombre, Precio, Stock) VALUES (@n, @p, @s)";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@n", txtNombre.Text);
                cmd.Parameters.AddWithValue("@p", decimal.Parse(txtPrecio.Text));
                cmd.Parameters.AddWithValue("@s", int.Parse(txtStock.Text));

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Producto guardado correctamente");
            Listar();
            Limpiar();
        }

        // ================= LISTAR BOTÓN =================
        private void btnListar_Click(object sender, EventArgs e)
        {
            Listar();
        }

        // ================= SELECCIONAR FILA =================
        private void dgvProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var fila = dgvProductos.Rows[e.RowIndex];

                idSeleccionado = Convert.ToInt32(fila.Cells["IdProducto"].Value);
                txtNombre.Text = fila.Cells["Nombre"].Value.ToString();
                txtPrecio.Text = fila.Cells["Precio"].Value.ToString();
                txtStock.Text = fila.Cells["Stock"].Value.ToString();
            }
        }

        // ================= ACTUALIZAR =================
        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un producto");
                return;
            }

            if (!Validar()) return;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                con.Open();

                string query = @"UPDATE Productos 
                                 SET Nombre=@n, Precio=@p, Stock=@s 
                                 WHERE IdProducto=@id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@n", txtNombre.Text);
                cmd.Parameters.AddWithValue("@p", decimal.Parse(txtPrecio.Text));
                cmd.Parameters.AddWithValue("@s", int.Parse(txtStock.Text));
                cmd.Parameters.AddWithValue("@id", idSeleccionado);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Producto actualizado");
            Listar();
            Limpiar();
        }

        // ================= ELIMINAR =================
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un producto");
                return;
            }

            var r = MessageBox.Show("¿Eliminar producto?", "Confirmar", MessageBoxButtons.YesNo);
            if (r != DialogResult.Yes) return;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                con.Open();

                string query = "DELETE FROM Productos WHERE IdProducto=@id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", idSeleccionado);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Producto eliminado");
            Listar();
            Limpiar();
        }

        // ================= NUEVO =================
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        // ================= BUSCAR =================
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = "SELECT * FROM Productos WHERE Nombre LIKE @buscar";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@buscar", "%" + txtBuscar.Text + "%");

                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvProductos.DataSource = dt;
            }
        }

        // ================= EVENTOS VACÍOS (EVITAR ERRORES) =================
        private void txtNombre_TextChanged(object sender, EventArgs e) { }
        private void txtPrecio_TextChanged(object sender, EventArgs e) { }
        private void txtStock_TextChanged(object sender, EventArgs e) { }
        private void dgvProductos_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
    }
}