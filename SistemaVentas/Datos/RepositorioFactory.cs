namespace Datos
{
    /// <summary> Factory crea los repositorios </summary>
    public static class RepositorioFactory
    {
        public static ProductoDAO Productos() => new();
        public static ClienteDAO Clientes() => new();
        public static VentaDAO Ventas() => new();
        public static UsuarioDAO Usuarios() => new();
    }
}
