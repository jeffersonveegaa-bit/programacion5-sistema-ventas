using System.Drawing;
using System.Windows.Forms;

namespace Presentacion
{
    /// <summary>Ventana principal con menú lateral.</summary>
    public class FrmPrincipal : Form
    {
        private readonly Panel contenido = new() { Dock = DockStyle.Fill, Padding = new Padding(24) };

        public FrmPrincipal()
        {
            Text = "Sistema de Ventas"; WindowState = FormWindowState.Maximized; MinimumSize = new Size(1000, 650);
            Estilo.Formulario(this);
            var menu = new FlowLayoutPanel { Dock = DockStyle.Left, Width = 200, BackColor = Color.White, FlowDirection = FlowDirection.TopDown, Padding = new Padding(12) };
            menu.Controls.Add(new Label { Text = "Sistema de Ventas", Font = new Font("Segoe UI Semibold", 12F), AutoSize = true, Margin = new Padding(4, 8, 4, 20) });
            Agregar(menu, "Productos", () => new FrmProductos());
            Agregar(menu, "Clientes", () => new FrmClientes());
            Agregar(menu, "Nueva venta", () => new FrmVentas());
            Agregar(menu, "Reportes", () => new FrmReportes());
            Controls.Add(contenido); Controls.Add(menu);
            Abrir(new FrmProductos());
        }

        private void Agregar(FlowLayoutPanel menu, string texto, System.Func<Form> crear)
        {
            var b = Estilo.Boton(texto, false); b.Width = 172; b.TextAlign = ContentAlignment.MiddleLeft;
            b.FlatAppearance.BorderSize = 0; b.Click += (_, _) => Abrir(crear());
            menu.Controls.Add(b);
        }

        /// <summary>Incrusta un formulario hijo dentro del panel de contenido.</summary>
        private void Abrir(Form f)
        {
            contenido.Controls.Clear();
            f.TopLevel = false; f.FormBorderStyle = FormBorderStyle.None; f.Dock = DockStyle.Fill;
            contenido.Controls.Add(f); f.Show();
        }
    }
}
