using System;
using System.Windows.Forms;
using Entidades;
using Negocio;

namespace Presentacion
{
    /// <summary>CRUD de clientes.</summary>
    public class FrmClientes : Form
    {
        private readonly ClienteService _srv = new();
        private readonly TextBox txtNombre = new() { Left = 0, Top = 75, Width = 240 };
        private readonly TextBox txtTelefono = new() { Left = 255, Top = 75, Width = 140 };
        private readonly TextBox txtCorreo = new() { Left = 410, Top = 75, Width = 240 };
        private readonly DataGridView grid = new() { Left = 0, Top = 165, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom };
        private int _id;

        public FrmClientes()
        {
            Estilo.Formulario(this); Estilo.Grid(grid); grid.Size = new System.Drawing.Size(900, 420);
            var bGuardar = Estilo.Boton("Guardar"); bGuardar.SetBounds(0, 115, 110, 34);
            var bEliminar = Estilo.Boton("Eliminar", false); bEliminar.SetBounds(120, 115, 110, 34);
            var bLimpiar = Estilo.Boton("Limpiar", false); bLimpiar.SetBounds(240, 115, 110, 34);
            bGuardar.Click += Guardar; bLimpiar.Click += (_, _) => Limpiar();
            bEliminar.Click += (_, _) => { if (_id > 0 && MessageBox.Show("¿Eliminar cliente?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes) { _srv.Eliminar(_id); Limpiar(); Cargar(); } };
            grid.CellClick += (_, e) =>
            {
                if (e.RowIndex < 0 || grid.Rows[e.RowIndex].DataBoundItem is not Cliente c) return;
                _id = c.Id; txtNombre.Text = c.Nombre; txtTelefono.Text = c.Telefono; txtCorreo.Text = c.Correo;
            };
            Controls.AddRange(new Control[] { new Label { Text = "Clientes", Font = Estilo.Titulo, AutoSize = true },
                Estilo.Etiqueta("Nombre", 0, 55), txtNombre, Estilo.Etiqueta("Teléfono", 255, 55), txtTelefono,
                Estilo.Etiqueta("Correo", 410, 55), txtCorreo, bGuardar, bEliminar, bLimpiar, grid });
            Load += (_, _) => Cargar();
        }

        private void Cargar() { try { grid.DataSource = _srv.Listar(); } catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); } }

        private void Guardar(object? s, EventArgs e)
        {
            try { _srv.Guardar(new Cliente { Id = _id, Nombre = txtNombre.Text.Trim(), Telefono = txtTelefono.Text.Trim(), Correo = txtCorreo.Text.Trim() }); Limpiar(); Cargar(); }
            catch (ArgumentException ex) { MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void Limpiar() { _id = 0; txtNombre.Clear(); txtTelefono.Clear(); txtCorreo.Clear(); }
    }
}
