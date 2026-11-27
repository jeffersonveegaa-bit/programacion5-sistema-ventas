using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Entidades;
using Negocio;

namespace Presentacion
{
    /// <summary>CRUD de clientes.</summary>
    public class FrmClientes : Form
    {
        private readonly ClienteService _srv = new();
        private readonly TextBox txtNombre = new();
        private readonly TextBox txtTelefono = new();
        private readonly TextBox txtCorreo = new();
        private readonly DataGridView grid = new();
        private readonly Label lblConteo = new() { AutoSize = true, ForeColor = Estilo.TextoSuave };
        private readonly Button bEliminar = Estilo.BotonPeligro("Eliminar");
        private int _id;

        public FrmClientes()
        {
            Estilo.Formulario(this); Estilo.Grid(grid);
            var bGuardar = Estilo.Boton("Guardar");
            var bLimpiar = Estilo.Boton("Limpiar", false);
            bEliminar.Enabled = false;
            bGuardar.Click += Guardar; bEliminar.Click += Eliminar; bLimpiar.Click += (_, _) => Limpiar();
            grid.CellClick += (_, e) =>
            {
                if (e.RowIndex < 0 || grid.Rows[e.RowIndex].DataBoundItem is not Cliente c) return;
                _id = c.Id; txtNombre.Text = c.Nombre; txtTelefono.Text = c.Telefono; txtCorreo.Text = c.Correo;
                bEliminar.Enabled = true;
            };

            var campos = Estilo.Fila(
                Estilo.Campo("Nombre", txtNombre, 260), Estilo.Campo("Teléfono", txtTelefono, 150),
                Estilo.Campo("Correo electrónico", txtCorreo, 260));
            var acciones = Estilo.Fila(bGuardar, bLimpiar);
            var barra = Estilo.BarraInferior(lblConteo, bEliminar);   // Eliminar queda abajo, a la derecha
            Estilo.Raiz(this, "Clientes", grid, campos, acciones, grid, barra);
            Load += (_, _) => Cargar();
        }

        /// <summary>Muestra una lista en la tabla y actualiza el contador.</summary>
        private void Mostrar(List<Cliente> lista)
        {
            grid.DataSource = lista;
            lblConteo.Text = lista.Count == 1 ? "1 cliente" : $"{lista.Count} clientes";
        }

        private void Cargar() { try { Mostrar(_srv.Listar()); } catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); } }

        private void Guardar(object? s, EventArgs e)
        {
            try { _srv.Guardar(new Cliente { Id = _id, Nombre = txtNombre.Text.Trim(), Telefono = txtTelefono.Text.Trim(), Correo = txtCorreo.Text.Trim() }); Limpiar(); Cargar(); }
            catch (ArgumentException ex) { MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void Eliminar(object? s, EventArgs e)
        {
            if (_id <= 0) return;
            if (MessageBox.Show("¿Eliminar cliente?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            try { _srv.Eliminar(_id); Limpiar(); Cargar(); }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Operación no permitida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void Limpiar() { _id = 0; txtNombre.Clear(); txtTelefono.Clear(); txtCorreo.Clear(); bEliminar.Enabled = false; }
    }
}