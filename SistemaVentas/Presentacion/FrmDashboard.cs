using System;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Negocio;

namespace Presentacion
{
    public class FrmDashboard : Form
    {
        private readonly Chart grafico = new() { Dock = DockStyle.Fill };

        public FrmDashboard()
        {
            Text = "Dashboard";
            Width = 900;
            Height = 550;
            Controls.Add(grafico);
            Load += (_, _) => CargarGrafico();
        }

        private void CargarGrafico()
        {
            grafico.ChartAreas.Clear();
            grafico.Series.Clear();
            grafico.Titles.Clear();
            grafico.Legends.Clear();

            var area = new ChartArea("Principal");
            area.AxisX.LabelStyle.Format = "dd/MM/yyyy";
            area.AxisY.Title = "Total vendido";
            grafico.ChartAreas.Add(area);
            grafico.Titles.Add("Total de ventas por día");

            var serie = new Series("Ventas") { ChartType = SeriesChartType.Column, XValueType = ChartValueType.Date };
            foreach (var (fecha, total) in new VentaService().TotalPorDia())
                serie.Points.AddXY(fecha, (double)total);
            grafico.Series.Add(serie);
        }
    }
}