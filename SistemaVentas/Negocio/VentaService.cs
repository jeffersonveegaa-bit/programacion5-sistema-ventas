using System;
using System.Collections.Generic;
using System.Linq;
using Datos;
using Entidades;

namespace Negocio
{
    /// <summary> Reglas de negocio de ventas. </summary>
    public class VentaService
    {
        private readonly VentaDAO _dao = RepositorioFactory.Ventas();

        /// <summary> Calcula el total de la venta usando LINQ. </summary>
        public static decimal CalcularTotal(IEnumerable<DetalleVenta> detalles) => detalles.Sum(d => d.Subtotal);

        /// <summary> Verifica stock disponible antes de agregar una línea. </summary>
        public static void ValidarStock(Producto p, int cantidadSolicitada)
        {
            if (cantidadSolicitada <= 0) throw new ArgumentException("La cantidad debe ser mayor a 0.");
            if (cantidadSolicitada > p.Stock) throw new InvalidOperationException($"Stock insuficiente. Disponible: {p.Stock}");
        }

        public int Registrar(Venta v)
        {
            if (v.Detalles.Count == 0) throw new InvalidOperationException("La venta no tiene productos.");
            v.Total = CalcularTotal(v.Detalles);
            return _dao.Registrar(v);
        }
        public List<VentaReporteDTO> Reporte() => _dao.Reporte();

        /// <summary> Total de ventas agrupado por día, para el gráfico. </summary>
        public List<(DateTime Fecha, decimal Total)> TotalPorDia() => Reporte()
            .GroupBy(venta => venta.Fecha.Date)
            .OrderBy(grupo => grupo.Key)
            .Select(grupo => (grupo.Key, grupo.Sum(venta => venta.Total)))
            .ToList();
        public List<DetalleVentaReporteDTO> ObtenerDetalle(int ventaId) => _dao.ObtenerDetalle(ventaId);
    }
}
