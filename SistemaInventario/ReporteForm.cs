using System.Text;
using SistemaInventario.Entidades;
using SistemaInventario.Negocio;

namespace SistemaInventario.Presentacion;

public class ReporteForm : Form
{
    private readonly VentaService _ventaService;

    private readonly Label lblTotalDia = new() { AutoSize = true, Font = new Font("Segoe UI", 12, FontStyle.Bold) };
    private readonly DataGridView dgvTop = new() { ReadOnly = true, AutoGenerateColumns = true };
    private readonly DataGridView dgvStockBajo = new() { ReadOnly = true, AutoGenerateColumns = true };
    private readonly Button btnRefrescar = new() { Text = "Refrescar" };
    private readonly Button btnExportar = new() { Text = "Exportar CSV" };

    public ReporteForm(VentaService ventaService)
    {
        _ventaService = ventaService;

        Text = "Reportes";
        StartPosition = FormStartPosition.CenterParent;
        Width = 920;
        Height = 580;

        lblTotalDia.SetBounds(20, 20, 400, 32);
        btnRefrescar.SetBounds(680, 20, 100, 32);
        btnExportar.SetBounds(790, 20, 100, 32);

        var lblTop = new Label { Text = "Productos más vendidos", AutoSize = true };
        lblTop.SetBounds(20, 70, 200, 24);
        dgvTop.SetBounds(20, 95, 870, 180);

        var lblStock = new Label { Text = "Stock bajo", AutoSize = true };
        lblStock.SetBounds(20, 290, 200, 24);
        dgvStockBajo.SetBounds(20, 315, 870, 210);

        btnRefrescar.Click += async (_, _) => await CargarDatosAsync();
        btnExportar.Click += async (_, _) => await ExportarCsvAsync();

        Controls.AddRange([lblTotalDia, btnRefrescar, btnExportar, lblTop, dgvTop, lblStock, dgvStockBajo]);

        Load += async (_, _) => await CargarDatosAsync();
    }

    private async Task CargarDatosAsync()
    {
        var reporte = await _ventaService.ObtenerReporteAsync();

        lblTotalDia.Text = $"Total de ventas de hoy: {reporte.TotalVentasDia:C2}";
        dgvTop.DataSource = reporte.ProductosMasVendidos;
        dgvStockBajo.DataSource = reporte.ProductosStockBajo;
    }

    private async Task ExportarCsvAsync()
    {
        var reporte = await _ventaService.ObtenerReporteAsync();

        using var dialog = new SaveFileDialog
        {
            Filter = "CSV|*.csv",
            FileName = $"Reporte_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
        };

        if (dialog.ShowDialog() != DialogResult.OK) return;

        var sb = new StringBuilder();
        sb.AppendLine("Total ventas del día;" + reporte.TotalVentasDia);
        sb.AppendLine();
        sb.AppendLine("Productos más vendidos");
        sb.AppendLine("Producto;Cantidad;Total Facturado");
        foreach (var item in reporte.ProductosMasVendidos)
        {
            sb.AppendLine($"{item.Producto};{item.CantidadVendida};{item.TotalFacturado}");
        }

        sb.AppendLine();
        sb.AppendLine("Stock bajo");
        sb.AppendLine("IdProducto;Nombre;StockActual");
        foreach (var item in reporte.ProductosStockBajo)
        {
            sb.AppendLine($"{item.IdProducto};{item.Nombre};{item.StockActual}");
        }

        await File.WriteAllTextAsync(dialog.FileName, sb.ToString(), Encoding.UTF8);
        MessageBox.Show("Reporte exportado correctamente.", "Reportes", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
