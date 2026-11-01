using System;
using System.Windows.Forms;
using Entidades;
using Negocio;

namespace Presentacion
{
    /// <summary>CRUD de productos.</summary>
    public class FrmProductos : Form
    {
        private readonly ProductoService _srv = new();
        private readonly TextBox txtCodigo = new() { Left = 0, Top = 75, Width = 110 };
        private readonly TextBox txtNombre = new() { Left = 125, Top = 75, Width = 200 };
        private readonly TextBox txtDesc = new() { Left = 340, Top = 75, Width = 220 };
        private readonly NumericUpDown numPrecio = new() { Left = 575, Top = 75, Width = 110, DecimalPlaces = 2, Maximum = 10000000 };
        private readonly NumericUpDown numStock = new() { Left = 700, Top = 75, Width = 80, Maximum = 100000 };
        private readonly TextBox txtBuscar = new() { Left = 0, Top = 170, Width = 260, PlaceholderText = "Buscar por nombre…" };
        private readonly DataGridView grid = new() { Left = 0, Top = 205, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom };
        private readonly ErrorProvider error = new();
        private bool _nuevo = true;

        public FrmProductos()
        {
            Estilo.Formulario(this); Estilo.Grid(grid);
            grid.Size = new System.Drawing.Size(900, 400);
            var titulo = new Label { Text = "Productos", Font = Estilo.Titulo, AutoSize = true };
            var bGuardar = Estilo.Boton("Guardar"); bGuardar.SetBounds(0, 115, 110, 34);
            var bEliminar = Estilo.Boton("Eliminar", false); bEliminar.SetBounds(120, 115, 110, 34);
            var bLimpiar = Estilo.Boton("Limpiar", false); bLimpiar.SetBounds(240, 115, 110, 34);
            bGuardar.Click += Guardar; bEliminar.Click += Eliminar; bLimpiar.Click += (_, _) => Limpiar();
            txtBuscar.TextChanged += (_, _) => grid.DataSource = _srv.Buscar(txtBuscar.Text);
            grid.CellClick += Seleccionar;
            Controls.AddRange(new Control[] { titulo,
                Estilo.Etiqueta("Código", 0, 55), txtCodigo, Estilo.Etiqueta("Nombre", 125, 55), txtNombre,
                Estilo.Etiqueta("Descripción", 340, 55), txtDesc, Estilo.Etiqueta("Precio", 575, 55), numPrecio,
                Estilo.Etiqueta("Stock", 700, 55), numStock, bGuardar, bEliminar, bLimpiar, txtBuscar, grid });
            Load += (_, _) => Cargar();
        }

        private void Cargar() { try { grid.DataSource = _srv.Listar(); } catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); } }

        private void Guardar(object? s, EventArgs e)
        {
            error.Clear();
            var p = new Producto { Codigo = txtCodigo.Text.Trim(), Nombre = txtNombre.Text.Trim(), Descripcion = txtDesc.Text.Trim(), Precio = numPrecio.Value, Stock = (int)numStock.Value };
            try { _srv.Guardar(p, _nuevo); Limpiar(); Cargar(); }
            catch (ArgumentException ex) { error.SetError(txtNombre, ex.Message); MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void Eliminar(object? s, EventArgs e)
        {
            if (_nuevo) return;
            if (MessageBox.Show("¿Eliminar este producto?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
            { try { _srv.Eliminar(txtCodigo.Text); Limpiar(); Cargar(); } catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); } }
        }

        private void Seleccionar(object? s, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || grid.Rows[e.RowIndex].DataBoundItem is not Producto p) return;
            txtCodigo.Text = p.Codigo; txtCodigo.Enabled = false; txtNombre.Text = p.Nombre; txtDesc.Text = p.Descripcion;
            numPrecio.Value = p.Precio; numStock.Value = p.Stock; _nuevo = false;
        }

        private void Limpiar()
        {
            txtCodigo.Clear(); txtCodigo.Enabled = true; txtNombre.Clear(); txtDesc.Clear();
            numPrecio.Value = 0; numStock.Value = 0; _nuevo = true; error.Clear();
        }
    }
}
