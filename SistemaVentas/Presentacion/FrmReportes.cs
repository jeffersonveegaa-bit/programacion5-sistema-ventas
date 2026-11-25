using System;
using System.Linq;
using System.Windows.Forms;
using Negocio;

namespace Presentacion
{
    /// <summary> Reportes de ventas, productos y clientes con exportación. </summary>
    public class FrmReportes : Form
    {
        private readonly ComboBox cboTipo = new() { DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly DataGridView grid = new();
        private readonly Label lblResumen = new() { AutoSize = true, Margin = new Padding(0, 6, 0, 0) };

        public FrmReportes()
        {
            Estilo.Formulario(this); Estilo.Grid(grid);
            cboTipo.Items.AddRange(new object[] { "Ventas", "Productos", "Clientes", "Productos bajo stock" });
            cboTipo.SelectedIndexChanged += (_, _) => Cargar();
            var bExcel = Estilo.Boton("Exportar Excel", false);
            bExcel.Click += ExportarExcel;
            var bPdf = Estilo.Boton("Exportar PDF", false);
            bPdf.Click += ExportarPdf;
            var bGrafico = Estilo.Boton("Gráfico de ventas", false);
            bGrafico.Click += (_, _) => { using var dashboard = new FrmDashboard(); dashboard.ShowDialog(); };
            var barra = Estilo.Fila(
                Estilo.Campo("Tipo de reporte", cboTipo, 240), Estilo.Campo(" ", bExcel),
                Estilo.Campo(" ", bPdf), Estilo.Campo(" ", bGrafico), Estilo.Campo("Resumen", lblResumen));
            Estilo.Raiz(this, "Reportes", grid, barra, grid);
            Load += (_, _) => cboTipo.SelectedIndex = 0;
        }

        private void Cargar()
        {
            try
            {
                switch (cboTipo.SelectedIndex)
                {
                    case 0:
                        var v = new VentaService().Reporte(); grid.DataSource = v;
                        lblResumen.Text = $"{v.Count} ventas · Total {v.Sum(x => x.Total):C2}"; break;
                    case 1: grid.DataSource = new ProductoService().Listar(); lblResumen.Text = ""; break;
                    case 2: grid.DataSource = new ClienteService().Listar(); lblResumen.Text = ""; break;
                    case 3: grid.DataSource = new ProductoService().BajoStock(); lblResumen.Text = ""; break;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); }
        }

        /// <summary> Método para exportar el reporte en Excel </summary>
        private void ExportarExcel(object? s, EventArgs e)
        {
            using var dlg = new SaveFileDialog { Filter = "Excel|*.xlsx", FileName = $"reporte_{cboTipo.Text}.xlsx" };
            if (dlg.ShowDialog() != DialogResult.OK) return;
            try
            {
                switch (cboTipo.SelectedIndex)
                {
                    case 0: Exportador.AExcel(new VentaService().Reporte(), dlg.FileName); break;
                    case 1: Exportador.AExcel(new ProductoService().Listar(), dlg.FileName); break;
                    case 2: Exportador.AExcel(new ClienteService().Listar(), dlg.FileName); break;
                    case 3: Exportador.AExcel(new ProductoService().BajoStock(), dlg.FileName); break;
                }
                MessageBox.Show("Reporte exportado.", "Reportes");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        /// <summary> Método para exportar reporte en PDF </summary>
        private void ExportarPdf(object? s, EventArgs e)
        {
            using var dlg = new SaveFileDialog { Filter = "PDF|*.pdf", FileName = $"reporte_{cboTipo.Text}.pdf" };
            if (dlg.ShowDialog() != DialogResult.OK) return;
            try
            {
                switch (cboTipo.SelectedIndex)
                {
                    case 0: Exportador.APdf(new VentaService().Reporte(), dlg.FileName, "Reporte de ventas"); break;
                    case 1: Exportador.APdf(new ProductoService().Listar(), dlg.FileName, "Reporte de productos"); break;
                    case 2: Exportador.APdf(new ClienteService().Listar(), dlg.FileName, "Reporte de clientes"); break;
                    case 3: Exportador.APdf(new ProductoService().BajoStock(), dlg.FileName, "Productos bajo stock"); break;
                }
                MessageBox.Show("Reporte exportado.", "Reportes");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
    }
}