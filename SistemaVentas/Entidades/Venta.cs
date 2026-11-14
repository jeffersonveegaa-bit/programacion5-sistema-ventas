using System;
using System.Collections.Generic;

namespace Entidades
{
    // Cabecera de venta.
    public class Venta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public int ClienteId { get; set; }
        public List<DetalleVenta> Detalles { get; set; } = new();
        public decimal Total { get; set; }
    }
}
