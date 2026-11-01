using System.Drawing;
using System.Windows.Forms;

namespace Presentacion
{
    /// <summary>Paleta y estilos comunes para un diseño sobrio y fino.</summary>
    public static class Estilo
    {
        public static readonly Color Fondo = Color.FromArgb(246, 248, 250);
        public static readonly Color Primario = Color.FromArgb(36, 62, 94);
        public static readonly Color Borde = Color.FromArgb(222, 226, 232);
        public static readonly Color Texto = Color.FromArgb(40, 48, 60);
        public static readonly Font Fuente = new("Segoe UI", 10F);
        public static readonly Font Titulo = new("Segoe UI Semibold", 16F);

        public static void Formulario(Form f)
        {
            f.BackColor = Fondo; f.Font = Fuente; f.ForeColor = Texto;
            f.StartPosition = FormStartPosition.CenterScreen;
        }

        public static Button Boton(string texto, bool primario = true)
        {
            var b = new Button { Text = texto, Height = 34, Width = 120, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand,
                BackColor = primario ? Primario : Color.White, ForeColor = primario ? Color.White : Texto };
            b.FlatAppearance.BorderColor = primario ? Primario : Borde;
            return b;
        }

        public static void Grid(DataGridView g)
        {
            g.BackgroundColor = Color.White; g.BorderStyle = BorderStyle.None;
            g.EnableHeadersVisualStyles = false; g.RowHeadersVisible = false;
            g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 242, 245);
            g.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(100, 110, 125);
            g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 230, 242);
            g.DefaultCellStyle.SelectionForeColor = Texto;
            g.GridColor = Borde; g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect; g.ReadOnly = true;
            g.AllowUserToAddRows = false; g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        public static Label Etiqueta(string t, int x, int y) => new() { Text = t, Left = x, Top = y, AutoSize = true, ForeColor = Color.FromArgb(100, 110, 125) };
    }
}
