using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary> DTO para el detalle de venta en el reporte. </summary>
namespace Entidades
{
    public class DetalleVentaReporteDTO
    {
        public string CodigoProducto { get; set; } = "";
        public string NombreProducto { get; set; } = "";
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Subtotal => Cantidad * Precio;
    }
}
