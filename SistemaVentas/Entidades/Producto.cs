namespace Entidades
{
    /// <summary> Producto del inventario que hereda de EntidadBase. </summary>
    public class Producto : EntidadBase
    {
        public string Codigo { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public override string Resumen() => $"{Codigo} - {Nombre} ({Stock} disp.)";
        public override string ToString() => Resumen();
    }
}
