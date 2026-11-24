using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Datos;
using Entidades;

namespace Negocio
{
    /// <summary> Negocio de productos. </summary>
    public class ProductoService
    {
        private readonly ProductoDAO _dao = RepositorioFactory.Productos();
        public List<Producto> Listar() => _dao.ObtenerTodos();
        public List<Producto> Buscar(string texto) =>
            Listar().Where(p => p.Nombre.Contains(texto, StringComparison.OrdinalIgnoreCase)).OrderBy(p => p.Nombre).ToList();
        public List<Producto> BajoStock(int minimo = 5) => Listar().Where(p => p.Stock < minimo).ToList();

        /// <summary> Valida un producto; lanza ArgumentException si es inválido. </summary>
        public static void Validar(Producto p)
        {
            if (string.IsNullOrWhiteSpace(p.Codigo)) throw new ArgumentException("El código es obligatorio.");
            if (string.IsNullOrWhiteSpace(p.Nombre)) throw new ArgumentException("El nombre es obligatorio.");
            if (p.Precio <= 0) throw new ArgumentException("El precio debe ser mayor a 0.");
            if (p.Stock < 0) throw new ArgumentException("El stock no puede ser negativo.");
        }
        public void Guardar(Producto p, bool nuevo) { Validar(p); if (nuevo) _dao.Insertar(p); else _dao.Actualizar(p); }
        public void Eliminar(string codigo) => _dao.Eliminar(codigo);

        /// <summary> Respaldo del inventario en JSON. </summary>
        public void ExportarJson(string ruta) =>
            File.WriteAllText(ruta, JsonSerializer.Serialize(Listar(), new JsonSerializerOptions { WriteIndented = true }));
    }
}
