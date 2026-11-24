namespace Entidades
{
    //Clase base para entidades con nombre (herencia/polimorfismo)
    public abstract class EntidadBase
    {
        public string Nombre { get; set; } = "";

        // Descripción corta de la entidad.
        public abstract string Resumen();
    }
}
