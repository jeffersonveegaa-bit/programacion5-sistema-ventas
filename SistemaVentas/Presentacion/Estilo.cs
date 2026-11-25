using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Presentacion
{
    /// <summary> Paleta, estilos y utilidades de diseño compartidas por todos los formularios </summary>
    public static class Estilo
    {
        public static readonly Color Fondo = Color.FromArgb(246, 248, 250);
        public static readonly Color Primario = Color.FromArgb(36, 62, 94);
        public static readonly Color Borde = Color.FromArgb(222, 226, 232);
        public static readonly Color Texto = Color.FromArgb(40, 48, 60);
        public static readonly Color TextoSuave = Color.FromArgb(100, 110, 125);
        public static readonly Font Fuente = new("Segoe UI", 10F);
        public static readonly Font Titulo = new("Segoe UI Semibold", 16F);

        /// <summary> Textos amigables para los encabezados de las tablas (propiedad -> encabezado) </summary>
        private static readonly Dictionary<string, string> Encabezados = new()
        {
            ["Id"] = "N.º",
            ["Codigo"] = "Código",
            ["CodigoProducto"] = "Código",
            ["NombreProducto"] = "Producto",
            ["Nombre"] = "Nombre",
            ["Descripcion"] = "Descripción",
            ["Telefono"] = "Teléfono",
            ["Correo"] = "Correo electrónico",
        };

        /// <summary> Peso (ancho relativo) de cada columna de las tablas </summary>
        private static readonly Dictionary<string, float> Pesos = new()
        {
            ["Id"] = 45,
            ["Codigo"] = 70,
            ["CodigoProducto"] = 70,
            ["Nombre"] = 140,
            ["NombreProducto"] = 160,
            ["Descripcion"] = 170,
            ["Telefono"] = 90,
            ["Correo"] = 150,
            ["Precio"] = 90,
            ["Stock"] = 60,
            ["Cantidad"] = 70,
            ["Subtotal"] = 90,
            ["Total"] = 90,
            ["Fecha"] = 110,
            ["Cliente"] = 150,
        };

        /// <summary> Configura un formulario: colores, fuente y escalado de la pantalla </summary>
        public static void Formulario(Form f)
        {
            f.AutoScaleDimensions = new SizeF(96F, 96F);
            f.AutoScaleMode = AutoScaleMode.Dpi;
            f.BackColor = Fondo; f.Font = Fuente; f.ForeColor = Texto;
            f.StartPosition = FormStartPosition.CenterScreen;
        }

        /// <summary> Botón con el estilo de la aplicación; crece según su texto </summary>
        public static Button Boton(string texto, bool primario = true)
        {
            var b = new Button
            {
                Text = texto,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                MinimumSize = new Size(120, 34),
                Padding = new Padding(10, 2, 10, 2),
                Margin = new Padding(0, 0, 10, 10),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false,
                BackColor = primario ? Primario : Color.White,
                ForeColor = primario ? Color.White : Texto
            };
            b.FlatAppearance.BorderColor = primario ? Primario : Borde;
            return b;
        }

        /// <summary> Botón de acción destructiva (eliminar): texto y borde en rojo </summary>
        public static Button BotonPeligro(string texto)
        {
            var rojo = Color.FromArgb(185, 28, 28);
            var b = Boton(texto, false);
            b.ForeColor = rojo; b.FlatAppearance.BorderColor = rojo;
            return b;
        }

        /// <summary> Estilo de tabla: ocupa todo el espacio disponible y da formato a monedas y fechas </summary>
        public static void Grid(DataGridView g)
        {
            g.Dock = DockStyle.Fill; g.Margin = new Padding(0);
            g.BackgroundColor = Color.White; g.BorderStyle = BorderStyle.None;
            g.EnableHeadersVisualStyles = false; g.RowHeadersVisible = false;
            g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 242, 245);
            g.ColumnHeadersDefaultCellStyle.ForeColor = TextoSuave;
            g.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(240, 242, 245);
            g.ColumnHeadersDefaultCellStyle.SelectionForeColor = TextoSuave;
            g.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 0, 6, 0);
            g.DefaultCellStyle.Padding = new Padding(6, 0, 6, 0);
            g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 230, 242);
            g.DefaultCellStyle.SelectionForeColor = Texto;
            g.GridColor = Borde; g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect; g.ReadOnly = true;
            g.MultiSelect = false; g.AllowUserToResizeRows = false;
            g.AllowUserToAddRows = false; g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            g.DataBindingComplete += (_, _) => FormatearColumnas(g);
        }

        private static void FormatearColumnas(DataGridView g)
        {
            /// <summary> El nombre va justo después del código o del número </summary>
            if (g.Columns.Contains("Nombre") && (g.Columns.Contains("Codigo") || g.Columns.Contains("Id")))
                g.Columns["Nombre"]!.DisplayIndex = 1;

            foreach (DataGridViewColumn c in g.Columns)
            {
                if (Encabezados.TryGetValue(c.DataPropertyName, out var texto)) c.HeaderText = texto;
                if (Pesos.TryGetValue(c.DataPropertyName, out var peso)) c.FillWeight = peso;
                var alineacion = DataGridViewContentAlignment.MiddleLeft;
                if (c.ValueType != null)
                {
                    var tipo = Nullable.GetUnderlyingType(c.ValueType) ?? c.ValueType;
                    if (tipo == typeof(decimal)) { c.DefaultCellStyle.Format = "C2"; alineacion = DataGridViewContentAlignment.MiddleRight; }
                    else if (tipo == typeof(DateTime)) c.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                    else if (tipo == typeof(int)) alineacion = DataGridViewContentAlignment.MiddleRight;
                }
                // El encabezado se alinea igual que los datos de su columna
                c.DefaultCellStyle.Alignment = alineacion;
                c.HeaderCell.Style.Alignment = alineacion;
            }
        }

        /// <summary> Etiqueta suelta (se conserva por compatibilidad con código anterior) </summary>
        public static Label Etiqueta(string t, int x, int y) => new() { Text = t, Left = x, Top = y, AutoSize = true, ForeColor = TextoSuave };

        /// <summary> Campo de formulario: etiqueta arriba y control debajo, con ancho fijo opcional </summary>
        public static FlowLayoutPanel Campo(string etiqueta, Control control, int ancho = 0)
        {
            if (ancho > 0) control.Width = ancho;
            control.Margin = new Padding(0, 2, 0, 0);
            var panel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Margin = new Padding(0, 0, 14, 10)
            };
            panel.Controls.Add(new EtiquetaPlana { Text = etiqueta, AutoSize = true, ForeColor = TextoSuave, Margin = new Padding(0) });
            panel.Controls.Add(control);
            return panel;
        }

        /// <summary> Fila de controles que se acomodan en varias líneas si la ventana es angosta </summary>
        public static FlowLayoutPanel Fila(params Control[] controles)
        {
            var fila = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Margin = new Padding(0),
                Dock = DockStyle.Fill
            };
            fila.Controls.AddRange(controles);
            return fila;
        }

        /// <summary> Barra inferior: un control a la izquierda y los botones alineados a la derecha </summary>
        public static TableLayoutPanel BarraInferior(Control izquierda, params Control[] botones)
        {
            var barra = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 1,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 10, 0, 0)
            };
            barra.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            barra.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            izquierda.Anchor = AnchorStyles.Left; izquierda.Margin = new Padding(0);
            var flujo = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Margin = new Padding(0)
            };
            flujo.Controls.AddRange(botones);
            barra.Controls.Add(izquierda, 0, 0);
            barra.Controls.Add(flujo, 1, 0);
            return barra;
        }

        /// <summary>
        /// Estructura vertical de una pantalla: título, filas de altura automática y una fila y si la ventana es muy pequeña aparece scroll.
        /// </summary>
        public static TableLayoutPanel Raiz(Form f, string titulo, Control expandir, params Control[] filas)
        {
            f.AutoScroll = true;
            f.AutoScrollMinSize = new Size(760, 440);
            var tabla = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = filas.Length + 1,
                BackColor = Color.Transparent
            };
            tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tabla.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tabla.Controls.Add(new EtiquetaPlana { Text = titulo, Font = Titulo, AutoSize = true, Margin = new Padding(0, 0, 0, 12) }, 0, 0);
            for (int i = 0; i < filas.Length; i++)
            {
                tabla.RowStyles.Add(filas[i] == expandir ? new RowStyle(SizeType.Percent, 100F) : new RowStyle(SizeType.AutoSize));
                filas[i].Dock = DockStyle.Fill;
                tabla.Controls.Add(filas[i], 0, i + 1);
            }
            f.Controls.Add(tabla);
            return tabla;
        }
    }

    /// <summary>
    /// Etiqueta que dibuja su texto sin el espacio interno que WinForms agrega a la izquierda,
    /// para que quede alineada exactamente con el borde de los campos y botones.
    /// </summary>
    internal sealed class EtiquetaPlana : Label
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, ForeColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        }
    }
}