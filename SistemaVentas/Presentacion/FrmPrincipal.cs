using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Presentacion
{
    /// <summary>Ventana principal con menú lateral.</summary>
    public class FrmPrincipal : Form
    {
        private readonly Panel contenido = new() { Dock = DockStyle.Fill, Padding = new Padding(24) };
        private readonly List<Button> _botones = new();

        public FrmPrincipal()
        {
            Text = "Sistema de Ventas"; WindowState = FormWindowState.Maximized; MinimumSize = new Size(820, 520);
            Estilo.Formulario(this);
            var menu = new FlowLayoutPanel
            {
                Dock = DockStyle.Left,
                Width = 220,
                BackColor = Color.White,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(12)
            };
            menu.Controls.Add(new Label { Text = "Sistema de Ventas", Font = new Font("Segoe UI Semibold", 12F), AutoSize = true, Margin = new Padding(4, 8, 4, 20) });
            Agregar(menu, "Productos", () => new FrmProductos());
            Agregar(menu, "Clientes", () => new FrmClientes());
            Agregar(menu, "Nueva venta", () => new FrmVentas());
            Agregar(menu, "Reportes", () => new FrmReportes());
            Controls.Add(contenido); Controls.Add(menu);
            Marcar(_botones[0]);
            Abrir(new FrmProductos());
        }

        private void Agregar(FlowLayoutPanel menu, string texto, Func<Form> crear)
        {
            var b = Estilo.Boton(texto, false);
            b.AutoSize = false; b.Size = new Size(190, 40); b.Margin = new Padding(0, 0, 0, 6);
            b.TextAlign = ContentAlignment.MiddleLeft; b.FlatAppearance.BorderSize = 0;
            b.Click += (_, _) => { Marcar(b); Abrir(crear()); };
            menu.Controls.Add(b); _botones.Add(b);
        }

        /// <summary>Resalta la opción del menú que está abierta.</summary>
        private void Marcar(Button activo)
        {
            foreach (var b in _botones) b.BackColor = b == activo ? Color.FromArgb(220, 230, 242) : Color.White;
        }

        /// <summary>Incrusta un formulario dentro del panel de contenido.</summary>
        private void Abrir(Form f)
        {
            var anteriores = new Control[contenido.Controls.Count];
            contenido.Controls.CopyTo(anteriores, 0);
            contenido.Controls.Clear();
            foreach (var anterior in anteriores) anterior.Dispose();
            f.TopLevel = false; f.FormBorderStyle = FormBorderStyle.None; f.Dock = DockStyle.Fill;
            contenido.Controls.Add(f); f.Show();
        }
    }
}