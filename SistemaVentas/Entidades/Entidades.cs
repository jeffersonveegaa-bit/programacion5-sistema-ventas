using System;
using System.Collections.Generic;

namespace Entidades
{
    //Clase base para entidades con nombre (herencia/polimorfismo)
    public abstract class EntidadBase
    {
        public string Nombre { get; set; } = "";

        // Descripción corta de la entidad.
        public abstract string Resumen();
    }

    // Producto del inventario.
    public class Producto : EntidadBase
    {
        public string Codigo { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public override string Resumen() => $"{Codigo} - {Nombre} ({Stock} disp.)";
        public override string ToString() => Resumen();
    }

    // Cliente del negocio.
    public class Cliente : EntidadBase
    {
        public int Id { get; set; }
        public string Telefono { get; set; } = "";
        public string Correo { get; set; } = "";
        public override string Resumen() => $"{Id} - {Nombre}";
        public override string ToString() => Nombre;
    }

    // Línea de detalle de una venta.
    public class DetalleVenta
    {
        public string CodigoProducto { get; set; } = "";
        public string NombreProducto { get; set; } = "";
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Subtotal => Cantidad * Precio;
    }

    // Cabecera de venta.
    public class Venta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public int ClienteId { get; set; }
        public List<DetalleVenta> Detalles { get; set; } = new();
        public decimal Total { get; set; }
    }

    // DTO usado para mostrar ventas en reportes.
    public class VentaReporteDTO
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; } = "";
        public decimal Total { get; set; }
    }
}
