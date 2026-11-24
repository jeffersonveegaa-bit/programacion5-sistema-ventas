using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Entidades;
using Negocio;

namespace Presentacion
{
    /// <summary>CRUD de productos.</summary>
    public class FrmProductos : Form
    {
        private readonly ProductoService _srv = new();
        private readonly TextBox txtCodigo = new();
        private readonly TextBox txtNombre = new();
        private readonly TextBox txtDesc = new();
        private readonly NumericUpDown numPrecio = new() { DecimalPlaces = 2, Maximum = 10000000, ThousandsSeparator = true };
        private readonly NumericUpDown numStock = new() { Maximum = 100000 };
        private readonly TextBox txtBuscar = new() { PlaceholderText = "Buscar por nombre…", Width = 280, Margin = new Padding(0, 4, 0, 8) };
        private readonly DataGridView grid = new();
        private readonly Label lblConteo = new() { AutoSize = true, ForeColor = Estilo.TextoSuave };
        private readonly Button bEliminar = Estilo.BotonPeligro("Eliminar");
        private readonly ErrorProvider error = new();
        private bool _nuevo = true;

        public FrmProductos()
        {
            Estilo.Formulario(this); Estilo.Grid(grid);
            var bGuardar = Estilo.Boton("Guardar");
            var bLimpiar = Estilo.Boton("Limpiar", false);
            bEliminar.Enabled = false;
            bGuardar.Click += Guardar; bEliminar.Click += Eliminar; bLimpiar.Click += (_, _) => Limpiar();
            txtBuscar.TextChanged += (_, _) => Mostrar(_srv.Buscar(txtBuscar.Text));
            grid.CellClick += Seleccionar;

            var campos = Estilo.Fila(
                Estilo.Campo("Código", txtCodigo, 110), Estilo.Campo("Nombre", txtNombre, 220),
                Estilo.Campo("Descripción", txtDesc, 260), Estilo.Campo("Precio", numPrecio, 130),
                Estilo.Campo("Stock", numStock, 90));
            var acciones = Estilo.Fila(bGuardar, bLimpiar);
            var busqueda = Estilo.Fila(txtBuscar);
            var barra = Estilo.BarraInferior(lblConteo, bEliminar);   // Eliminar queda abajo, a la derecha
            Estilo.Raiz(this, "Productos", grid, campos, acciones, busqueda, grid, barra);
            Load += (_, _) => Cargar();
        }

        /// <summary>Muestra una lista en la tabla y actualiza el contador.</summary>
        private void Mostrar(List<Producto> lista)
        {
            grid.DataSource = lista;
            lblConteo.Text = lista.Count == 1 ? "1 producto" : $"{lista.Count} productos";
        }

        private void Cargar() { try { Mostrar(_srv.Listar()); } catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); } }

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
            numPrecio.Value = p.Precio; numStock.Value = p.Stock; _nuevo = false; bEliminar.Enabled = true;
        }

        private void Limpiar()
        {
            txtCodigo.Clear(); txtCodigo.Enabled = true; txtNombre.Clear(); txtDesc.Clear();
            numPrecio.Value = 0; numStock.Value = 0; _nuevo = true; bEliminar.Enabled = false; error.Clear();
        }
    }
}