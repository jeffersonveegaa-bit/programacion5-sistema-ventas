namespace Entidades
{
    // Cliente del negocio.
    public class Cliente : EntidadBase
    {
        public int Id { get; set; }
        public string Telefono { get; set; } = "";
        public string Correo { get; set; } = "";
        public override string Resumen() => $"{Id} - {Nombre}";
        public override string ToString() => Nombre;
    }
}
