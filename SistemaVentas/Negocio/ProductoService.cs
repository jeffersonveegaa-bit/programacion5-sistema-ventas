using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Datos;
using Entidades;
using MySql.Data.MySqlClient;

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
        //public void Eliminar(string codigo) => _dao.Eliminar(codigo);
        public void Eliminar(string codigo)
        {
            try
            {
                _dao.Eliminar(codigo);
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1451)
                {
                    throw new InvalidOperationException(
                        "No se puede eliminar el producto porque está asociado a una o más ventas.");
                }

                throw;
            }
        }

        /// <summary> Respaldo del inventario en JSON. </summary>
        public void ExportarJson(string ruta) =>
            File.WriteAllText(ruta, JsonSerializer.Serialize(Listar(), new JsonSerializerOptions { WriteIndented = true }));

        /// <summary> Para convertir el texo en productos,  </summary>
        public int ImportarJson(string ruta)
        {
            List<Producto>? productos;
            try { productos = JsonSerializer.Deserialize<List<Producto>>(File.ReadAllText(ruta)); }
            catch (JsonException) { throw new ArgumentException("El archivo no es un respaldo de productos válido."); }

            if (productos == null) throw new ArgumentException("El archivo no contiene productos.");

            foreach (var producto in productos) Validar(producto);

            var codigosExistentes = Listar().Select(existente => existente.Codigo).ToHashSet();
            foreach (var producto in productos)
            {
                if (codigosExistentes.Contains(producto.Codigo)) _dao.Actualizar(producto);
                else { _dao.Insertar(producto); codigosExistentes.Add(producto.Codigo); }
            }
            return productos.Count;
        }
    }
}
