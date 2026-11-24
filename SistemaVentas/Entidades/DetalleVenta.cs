namespace Entidades
{
    /// <summary> Línea de detalle de una venta. </summary>
    public class DetalleVenta
    {
        public string CodigoProducto { get; set; } = "";
        public string NombreProducto { get; set; } = "";
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Subtotal => Cantidad * Precio;
    }
}
