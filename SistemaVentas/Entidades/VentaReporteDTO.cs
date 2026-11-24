using System;

namespace Entidades
{
    /// <summary> DTO usado para mostrar ventas en reportes. </summary>
    public class VentaReporteDTO
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; } = "";
        public decimal Total { get; set; }
    }
}
