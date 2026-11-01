using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Entidades;
using Negocio;

namespace Presentacion
{
    /// <summary>Registro de ventas (cabecera-detalle).</summary>
    public class FrmVentas : Form
    {
        private readonly ComboBox cboCliente = new() { Left = 0, Top = 75, Width = 240, DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly ComboBox cboProducto = new() { Left = 255, Top = 75, Width = 260, DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly NumericUpDown numCant = new() { Left = 530, Top = 75, Width = 80, Minimum = 1, Maximum = 1000 };
        private readonly DataGridView grid = new() { Left = 0, Top = 120, Size = new System.Drawing.Size(760, 340) };
        private readonly Label lblTotal = new() { Left = 0, Top = 475, AutoSize = true, Font = Estilo.Titulo };
        private readonly BindingList<DetalleVenta> _detalle = new();

        public FrmVentas()
        {
            Estilo.Formulario(this); Estilo.Grid(grid); grid.DataSource = _detalle;
            var bAgregar = Estilo.Boton("Agregar", false); bAgregar.SetBounds(625, 73, 110, 30);
            var bQuitar = Estilo.Boton("Quitar línea", false); bQuitar.SetBounds(520, 520, 110, 34);
            var bRegistrar = Estilo.Boton("Registrar venta"); bRegistrar.SetBounds(640, 520, 140, 34);
            bAgregar.Click += Agregar; bRegistrar.Click += Registrar;
            bQuitar.Click += (_, _) => { if (grid.CurrentRow?.DataBoundItem is DetalleVenta d) { _detalle.Remove(d); ActualizarTotal(); } };
            Controls.AddRange(new Control[] { new Label { Text = "Nueva venta", Font = Estilo.Titulo, AutoSize = true },
                Estilo.Etiqueta("Cliente", 0, 55), cboCliente, Estilo.Etiqueta("Producto", 255, 55), cboProducto,
                Estilo.Etiqueta("Cantidad", 530, 55), numCant, bAgregar, grid, lblTotal, bQuitar, bRegistrar });
            Load += (_, _) => Cargar();
        }

        private void Cargar()
        {
            try
            {
                cboCliente.DataSource = new ClienteService().Listar();
                cboProducto.DataSource = new ProductoService().Listar();
                _detalle.Clear(); ActualizarTotal();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); }
        }

        private void Agregar(object? s, EventArgs e)
        {
            if (cboProducto.SelectedItem is not Producto p) return;
            int cant = (int)numCant.Value;
            var existente = _detalle.FirstOrDefault(d => d.CodigoProducto == p.Codigo);
            try
            {
                VentaService.ValidarStock(p, cant + (existente?.Cantidad ?? 0));
                if (existente != null) { existente.Cantidad += cant; grid.Refresh(); }
                else _detalle.Add(new DetalleVenta { CodigoProducto = p.Codigo, NombreProducto = p.Nombre, Cantidad = cant, Precio = p.Precio });
                ActualizarTotal();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void ActualizarTotal() => lblTotal.Text = "Total: " + VentaService.CalcularTotal(_detalle).ToString("C2");

        private void Registrar(object? s, EventArgs e)
        {
            if (cboCliente.SelectedItem is not Cliente c) return;
            try
            {
                int id = new VentaService().Registrar(new Venta { ClienteId = c.Id, Detalles = _detalle.ToList() });
                MessageBox.Show($"Venta #{id} registrada correctamente.", "Ventas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Cargar();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
    }
}
