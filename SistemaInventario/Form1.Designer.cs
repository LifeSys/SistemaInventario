namespace SistemaInventario
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblNombre = new Label();
            lblPrecio = new Label();
            lblStock = new Label();
            txtStock = new TextBox();
            txtNombre = new TextBox();
            txtPrecio = new TextBox();
            btnGuardar = new Button();
            btnListar = new Button();
            dgvProductos = new DataGridView();
            btnActualizar = new Button();
            btnEliminar = new Button();
            btnNuevo = new Button();
            btnBuscar = new Button();
            txtBuscar = new TextBox();
            ((System.ComponentModel.ISupportInitialize)dgvProductos).BeginInit();
            SuspendLayout();
            // 
            // lblNombre
            // 
            lblNombre.AutoSize = true;
            lblNombre.Location = new Point(70, 76);
            lblNombre.Name = "lblNombre";
            lblNombre.Size = new Size(51, 15);
            lblNombre.TabIndex = 0;
            lblNombre.Text = "Nombre";
            // 
            // lblPrecio
            // 
            lblPrecio.AutoSize = true;
            lblPrecio.Location = new Point(70, 126);
            lblPrecio.Name = "lblPrecio";
            lblPrecio.Size = new Size(40, 15);
            lblPrecio.TabIndex = 1;
            lblPrecio.Text = "Precio";
            // 
            // lblStock
            // 
            lblStock.AutoSize = true;
            lblStock.Location = new Point(70, 170);
            lblStock.Name = "lblStock";
            lblStock.Size = new Size(36, 15);
            lblStock.TabIndex = 2;
            lblStock.Text = "Stock";
            // 
            // txtStock
            // 
            txtStock.Location = new Point(127, 162);
            txtStock.Name = "txtStock";
            txtStock.Size = new Size(100, 23);
            txtStock.TabIndex = 4;
            // 
            // txtNombre
            // 
            txtNombre.Location = new Point(127, 68);
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(100, 23);
            txtNombre.TabIndex = 5;
            // 
            // txtPrecio
            // 
            txtPrecio.Location = new Point(127, 118);
            txtPrecio.Name = "txtPrecio";
            txtPrecio.Size = new Size(100, 23);
            txtPrecio.TabIndex = 6;
            // 
            // btnGuardar
            // 
            btnGuardar.Location = new Point(127, 267);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(100, 23);
            btnGuardar.TabIndex = 7;
            btnGuardar.Text = "GUARDAR";
            btnGuardar.UseVisualStyleBackColor = true;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // btnListar
            // 
            btnListar.Location = new Point(127, 296);
            btnListar.Name = "btnListar";
            btnListar.Size = new Size(100, 23);
            btnListar.TabIndex = 8;
            btnListar.Text = "LISTAR";
            btnListar.UseVisualStyleBackColor = true;
            btnListar.Click += btnListar_Click;
            // 
            // dgvProductos
            // 
            dgvProductos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvProductos.Location = new Point(289, 32);
            dgvProductos.Name = "dgvProductos";
            dgvProductos.Size = new Size(352, 481);
            dgvProductos.TabIndex = 9;
            dgvProductos.CellClick += dgvProductos_CellClick;
            // 
            // btnActualizar
            // 
            btnActualizar.Location = new Point(127, 325);
            btnActualizar.Name = "btnActualizar";
            btnActualizar.Size = new Size(100, 23);
            btnActualizar.TabIndex = 10;
            btnActualizar.Text = "ACTUALIZAR";
            btnActualizar.UseVisualStyleBackColor = true;
            btnActualizar.Click += btnActualizar_Click;
            // 
            // btnEliminar
            // 
            btnEliminar.Location = new Point(127, 354);
            btnEliminar.Name = "btnEliminar";
            btnEliminar.Size = new Size(100, 23);
            btnEliminar.TabIndex = 11;
            btnEliminar.Text = "ELIMINAR";
            btnEliminar.UseVisualStyleBackColor = true;
            btnEliminar.Click += btnEliminar_Click;
            // 
            // btnNuevo
            // 
            btnNuevo.Location = new Point(127, 383);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new Size(100, 23);
            btnNuevo.TabIndex = 12;
            btnNuevo.Text = "NUEVO";
            btnNuevo.UseVisualStyleBackColor = true;
            btnNuevo.Click += btnNuevo_Click;
            // 
            // btnBuscar
            // 
            btnBuscar.Location = new Point(127, 238);
            btnBuscar.Name = "btnBuscar";
            btnBuscar.Size = new Size(100, 23);
            btnBuscar.TabIndex = 13;
            btnBuscar.Text = "BUSCAR";
            btnBuscar.UseVisualStyleBackColor = true;
            btnBuscar.Click += btnBuscar_Click;
            // 
            // txtBuscar
            // 
            txtBuscar.Location = new Point(127, 209);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Size = new Size(100, 23);
            txtBuscar.TabIndex = 14;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(660, 547);
            Controls.Add(txtBuscar);
            Controls.Add(btnBuscar);
            Controls.Add(btnNuevo);
            Controls.Add(btnEliminar);
            Controls.Add(btnActualizar);
            Controls.Add(dgvProductos);
            Controls.Add(btnListar);
            Controls.Add(btnGuardar);
            Controls.Add(txtPrecio);
            Controls.Add(txtNombre);
            Controls.Add(txtStock);
            Controls.Add(lblStock);
            Controls.Add(lblPrecio);
            Controls.Add(lblNombre);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dgvProductos).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblNombre;
        private Label lblPrecio;
        private Label lblStock;
        private TextBox txtStock;
        private TextBox txtNombre;
        private TextBox txtPrecio;
        private Button btnGuardar;
        private Button btnListar;
        private DataGridView dgvProductos;
        private Button btnActualizar;
        private Button btnEliminar;
        private Button btnNuevo;
        private Button btnBuscar;
        private TextBox txtBuscar;
    }
}
