using System; //trae las herramientas basicas de c#
using System.ComponentModel;// lista que avisa a la tabla cuabdo ingresa un dato
using System.Linq; // permite sumar  y buscar dentro de las listas
using System.Windows.Forms; // es la que trae las herramientas visuales (ventanas, botones...)
using Entidades; //es la que trae las clases de modelos (producto , cliente, etc)
using Negocio; // trae las servicios que tiene reglas de negocios

namespace Presentacion
{
    /// <summary>Registro de ventas (cabecera-detalle).</summary>
    public class FrmVentas : Form
    {
        // listas desplegables para elegir al cliente.      .dropDownlist (solo puede elegir, no escribir)
        private readonly ComboBox cboCliente = new() { DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly ComboBox cboProducto = new() { DropDownStyle = ComboBoxStyle.DropDownList };
        //cuadro numerico para la cantidad con un maximo de mil unidades
        private readonly NumericUpDown numCant = new() { Minimum = 1, Maximum = 1000 };
        //tabla donde se muestrael total de la ventas
        private readonly DataGridView grid = new();
        //Muestra el total de la venta con letra grande
        private readonly Label lblTotal = new() { AutoSize = true, Font = Estilo.Titulo };
        // lista de lineas con producto, cantidad, precio, la tabla se llena con con ellas
        private readonly BindingList<DetalleVenta> _detalle = new();

        //constructor, ejecuta y abre la patalla para usarla
        public FrmVentas()
        {
            Estilo.Formulario(this); Estilo.Grid(grid); grid.DataSource = _detalle;
            var bAgregar = Estilo.Boton("Agregar", false);
            var bQuitar = Estilo.BotonPeligro("Quitar línea");
            var bRegistrar = Estilo.Boton("Registrar venta");
            bAgregar.Click += Agregar; bRegistrar.Click += Registrar;
            bQuitar.Click += (_, _) => { if (grid.CurrentRow?.DataBoundItem is DetalleVenta d) { _detalle.Remove(d); ActualizarTotal(); } };

            var seleccion = Estilo.Fila(
                Estilo.Campo("Cliente", cboCliente, 260), Estilo.Campo("Producto", cboProducto, 300),
                Estilo.Campo("Cantidad", numCant, 90), Estilo.Campo(" ", bAgregar));
            var barra = Estilo.BarraInferior(lblTotal, bQuitar, bRegistrar);
            Estilo.Raiz(this, "Nueva venta", grid, seleccion, grid, barra);
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
            if (cboProducto.SelectedItem is not Producto p)
            {
                MessageBox.Show("Seleccione un producto.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
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
            if (cboCliente.SelectedItem is not Cliente c)
            {
                cboProducto.DataSource = new ProductoService().Listar(); /// <summary>Refresca la lista de products</summary>
                MessageBox.Show("Seleccione un cliente antes de registrar la venta.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
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