using System;
using System.Collections.Generic;

namespace Entidades
{
    /// <summary>Clase base para entidades con nombre (herencia/polimorfismo).</summary>
    public abstract class EntidadBase
    {
        public string Nombre { get; set; } = "";
        /// <summary>Descripción corta de la entidad.</summary>
        public abstract string Resumen();
    }

    /// <summary>Producto del inventario.</summary>
    public class Producto : EntidadBase
    {
        public string Codigo { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public override string Resumen() => $"{Codigo} - {Nombre} ({Stock} disp.)";
        public override string ToString() => Resumen();
    }

    /// <summary>Cliente del negocio.</summary>
    public class Cliente : EntidadBase
    {
        public int Id { get; set; }
        public string Telefono { get; set; } = "";
        public string Correo { get; set; } = "";
        public override string Resumen() => $"{Id} - {Nombre}";
        public override string ToString() => Nombre;
    }

    /// <summary>Línea de detalle de una venta.</summary>
    public class DetalleVenta
    {
        public string CodigoProducto { get; set; } = "";
        public string NombreProducto { get; set; } = "";
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Subtotal => Cantidad * Precio;
    }

    /// <summary>Cabecera de venta.</summary>
    public class Venta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public int ClienteId { get; set; }
        public List<DetalleVenta> Detalles { get; set; } = new();
        public decimal Total { get; set; }
    }

    /// <summary>DTO usado para mostrar ventas en reportes.</summary>
    public class VentaReporteDTO
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; } = "";
        public decimal Total { get; set; }
    }
}
