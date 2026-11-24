namespace Entidades
{
    /// <summary> Cliente del negocio. </summary>
    public class Cliente : EntidadBase
    {
        public int Id { get; set; }
        public string Telefono { get; set; } = "";
        public string Correo { get; set; } = "";
        public override string Resumen() => $"{Id} - {Nombre}";
        public override string ToString() => Nombre;
    }
}
