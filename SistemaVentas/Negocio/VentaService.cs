using System;
using System.Collections.Generic;
using System.Linq;
using Datos;
using Entidades;

namespace Negocio
{
    // Reglas de negocio de ventas.
    public class VentaService
    {
        private readonly VentaDAO _dao = RepositorioFactory.Ventas();

        // Calcula el total de la venta usando LINQ.
        public static decimal CalcularTotal(IEnumerable<DetalleVenta> detalles) => detalles.Sum(d => d.Subtotal);

        // Verifica stock disponible antes de agregar una línea.
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
    }
}
