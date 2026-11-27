namespace Entidades
{
    /// <summary> Clase base para entidades con nombre (herencia/polimorfismo). </summary>
    public abstract class EntidadBase
    {
        public string Nombre { get; set; } = "";

        /// <summary> Método para obtener una descripción corta de la entidad. </summary>
        public abstract string Resumen();
    }
}
