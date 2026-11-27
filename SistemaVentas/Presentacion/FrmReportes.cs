using Entidades;
using Microsoft.VisualBasic;
using Negocio;
using System;
using System.Linq;
using System.Windows.Forms;

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
            grid.CellClick += MostrarDetalleVenta;
            cboTipo.Items.AddRange(new object[] { "Ventas", "Productos", "Clientes", "Productos bajo stock", "Detalle de venta" });
            cboTipo.SelectedIndexChanged += (_, _) => Cargar();
            var bExcel = Estilo.Boton("Exportar Excel", false);
            bExcel.Click += ExportarExcel;
            var bPdf = Estilo.Boton("Exportar PDF", false);
            bPdf.Click += ExportarPdf;
            var bGrafico = Estilo.Boton("Gráfico de ventas", false);
            var bVolver = Estilo.Boton("Volver a ventas", false);
            bVolver.Click += (_, _) =>
            {
                cboTipo.SelectedIndex = 0;
                Cargar();
            };
            bGrafico.Click += (_, _) => { using var dashboard = new FrmDashboard(); dashboard.ShowDialog(); };
            var barra = Estilo.Fila(
                Estilo.Campo("Tipo de reporte", cboTipo, 240),
                Estilo.Campo(" ", bExcel),
                Estilo.Campo(" ", bPdf),
                Estilo.Campo(" ", bGrafico),
                Estilo.Campo(" ", bVolver),
                Estilo.Campo("Resumen", lblResumen));
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
                    case 4:

                        string texto = Interaction.InputBox(
                            "Digite el ID de la venta",
                            "Detalle de venta",
                            "1");

                        if (int.TryParse(texto, out int ventaId))
                        {
                            var detalle = new VentaService().ObtenerDetalle(ventaId);

                            grid.DataSource = detalle;

                            lblResumen.Text =
                                $"{detalle.Count} líneas de detalle";
                        }

                        break;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); }
        }


        /// metodo para cargar detalles en el clic de ventas
        private void MostrarDetalleVenta(object? sender, DataGridViewCellEventArgs e)
        {
            if (cboTipo.SelectedIndex != 0)
                return;

            if (e.RowIndex < 0)
                return;

            if (grid.Rows[e.RowIndex].DataBoundItem is not VentaReporteDTO venta)
                return;

            var detalle = new VentaService().ObtenerDetalle(venta.Id);

            grid.DataSource = detalle;

            lblResumen.Text = $"Detalle de la venta #{venta.Id}";
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