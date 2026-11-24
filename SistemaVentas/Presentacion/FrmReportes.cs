using System;
using System.Linq;
using System.Windows.Forms;
using Negocio;

namespace Presentacion
{
    /// <summary> Reportes de ventas, productos y clientes con exportación. </summary>
    public class FrmReportes : Form
    {
        private readonly ComboBox cboTipo = new() { Left = 0, Top = 60, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly DataGridView grid = new() { Left = 0, Top = 105, Size = new System.Drawing.Size(900, 400), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom };
        private readonly Label lblResumen = new() { Left = 530, Top = 66, AutoSize = true };

        public FrmReportes()
        {
            Estilo.Formulario(this); Estilo.Grid(grid);
            cboTipo.Items.AddRange(new object[] { "Ventas", "Productos", "Clientes", "Productos bajo stock" });
            cboTipo.SelectedIndexChanged += (_, _) => Cargar();
            var bExcel = Estilo.Boton("Exportar Excel", false); bExcel.SetBounds(215, 58, 140, 30);
            bExcel.Click += ExportarExcel;
            var bPdf = Estilo.Boton("Exportar PDF", false); bPdf.SetBounds(365, 58, 140, 30);
            bPdf.Click += ExportarPdf;
            Controls.AddRange(new Control[] { new Label { Text = "Reportes", Font = Estilo.Titulo, AutoSize = true }, cboTipo, bExcel, bPdf, lblResumen, grid });
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
