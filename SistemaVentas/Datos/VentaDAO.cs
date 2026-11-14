using System;
using System.Collections.Generic;
using Entidades;
using MySql.Data.MySqlClient;

namespace Datos
{
    /// <summary> DAO de ventas: guarda cabecera, detalle y descuenta stock en una transacción </summary>
    public class VentaDAO
    {
        public int Registrar(Venta venta)
        {
            using var conexion = Conexion.Obtener(); conexion.Open();
            using var transaccion = conexion.BeginTransaction();
            try
            {
                var comandoVenta = new MySqlCommand("INSERT INTO ventas (fecha,cliente_id,total) VALUES (@fecha,@cliente_id,@total); SELECT LAST_INSERT_ID();", conexion, transaccion);
                comandoVenta.Parameters.AddWithValue("@fecha", venta.Fecha); comandoVenta.Parameters.AddWithValue("@cliente_id", venta.ClienteId); comandoVenta.Parameters.AddWithValue("@total", venta.Total);
                int ventaID = Convert.ToInt32(comandoVenta.ExecuteScalar());
                foreach (var detalle in venta.Detalles)
                {
                    var comandoDetalle = new MySqlCommand("INSERT INTO detalle_venta (venta_id,producto_codigo,cantidad,precio) VALUES (@venta_id,@producto_codigo,@cantidad,@precio)", conexion, transaccion);
                    comandoDetalle.Parameters.AddWithValue("@venta_id", ventaID); comandoDetalle.Parameters.AddWithValue("@producto_codigo", detalle.CodigoProducto);
                    comandoDetalle.Parameters.AddWithValue("@cantidad", detalle.Cantidad); comandoDetalle.Parameters.AddWithValue("@precio", detalle.Precio); comandoDetalle.ExecuteNonQuery();
                    var comandoStock = new MySqlCommand("UPDATE productos SET stock = stock - @cantidad WHERE codigo=@producto_codigo AND stock >= @cantidad", conexion, transaccion);
                    if (comandoStock.ExecuteNonQuery() == 0)
                        throw new InvalidOperationException("Stock insuficiente para el producto " + detalle.CodigoProducto);
                }
                transaccion.Commit();
                return ventaID;
            }
            catch { transaccion.Rollback(); throw; }
        }

        public List<VentaReporteDTO> Reporte()
        {
            var lista = new List<VentaReporteDTO>();
            using var conexion = Conexion.Obtener(); conexion.Open();
            using var comando = new MySqlCommand("SELECT v.id,v.fecha,c.nombre,v.total FROM ventas v JOIN clientes c ON c.id=v.cliente_id ORDER BY v.fecha DESC", conexion);
            using var lector = comando.ExecuteReader();
            while (lector.Read()) lista.Add(new VentaReporteDTO { Id = lector.GetInt32(0), Fecha = lector.GetDateTime(1), Cliente = lector.GetString(2), Total = lector.GetDecimal(3) });
            return lista;
        }
    }
}
