using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Datos;
using Entidades;

namespace Negocio
{
    /// <summary>Reglas de negocio de productos.</summary>
    public class ProductoService
    {
        private readonly ProductoDAO _dao = RepositorioFactory.Productos();
        public List<Producto> Listar() => _dao.ObtenerTodos();
        public List<Producto> Buscar(string texto) =>
            Listar().Where(p => p.Nombre.Contains(texto, StringComparison.OrdinalIgnoreCase)).OrderBy(p => p.Nombre).ToList();
        public List<Producto> BajoStock(int minimo = 5) => Listar().Where(p => p.Stock < minimo).ToList();

        /// <summary>Valida un producto; lanza ArgumentException si es inválido.</summary>
        public static void Validar(Producto p)
        {
            if (string.IsNullOrWhiteSpace(p.Codigo)) throw new ArgumentException("El código es obligatorio.");
            if (string.IsNullOrWhiteSpace(p.Nombre)) throw new ArgumentException("El nombre es obligatorio.");
            if (p.Precio <= 0) throw new ArgumentException("El precio debe ser mayor a 0.");
            if (p.Stock < 0) throw new ArgumentException("El stock no puede ser negativo.");
        }
        public void Guardar(Producto p, bool nuevo) { Validar(p); if (nuevo) _dao.Insertar(p); else _dao.Actualizar(p); }
        public void Eliminar(string codigo) => _dao.Eliminar(codigo);

        /// <summary>Respaldo del inventario en JSON.</summary>
        public void ExportarJson(string ruta) =>
            File.WriteAllText(ruta, JsonSerializer.Serialize(Listar(), new JsonSerializerOptions { WriteIndented = true }));
    }

    /// <summary>Reglas de negocio de clientes.</summary>
    public class ClienteService
    {
        private readonly ClienteDAO _dao = RepositorioFactory.Clientes();
        public List<Cliente> Listar() => _dao.ObtenerTodos();
        public static void Validar(Cliente c)
        {
            if (string.IsNullOrWhiteSpace(c.Nombre)) throw new ArgumentException("El nombre es obligatorio.");
            if (!Regex.IsMatch(c.Correo ?? "", @"^\S+@\S+\.\S+$")) throw new ArgumentException("Correo electrónico inválido.");
        }
        public void Guardar(Cliente c) { Validar(c); if (c.Id == 0) _dao.Insertar(c); else _dao.Actualizar(c); }
        public void Eliminar(int id) => _dao.Eliminar(id);
    }

    /// <summary>Reglas de negocio de ventas.</summary>
    public class VentaService
    {
        private readonly VentaDAO _dao = RepositorioFactory.Ventas();

        /// <summary>Calcula el total de la venta usando LINQ.</summary>
        public static decimal CalcularTotal(IEnumerable<DetalleVenta> detalles) => detalles.Sum(d => d.Subtotal);

        /// <summary>Verifica stock disponible antes de agregar una línea.</summary>
        public static void ValidarStock(Producto p, int cantidadSolicitada)
        {
            if (cantidadSolicitada <= 0) throw new ArgumentException("La cantidad debe ser mayor a 0.");
            if (cantidadSolicitada > p.Stock) throw new InvalidOperationException($"Stock insuficiente. Disponible: {p.Stock}");
        }

        public int Registrar(Venta v)
        {
            if (v.Detalles.Count == 0) throw new InvalidOperationException("La venta no tiene productos.");
            v.Total = CalcularTotal(v.Detalles);
            return _dao.Registrar(v);
        }
        public List<VentaReporteDTO> Reporte() => _dao.Reporte();
    }

    /// <summary>Autenticación de usuarios.</summary>
    public class AuthService
    {
        public bool Login(string u, string c) => RepositorioFactory.Usuarios().Validar(u, c);
    }

    /// <summary>Exportación simple a CSV (abre en Excel).</summary>
    public static class Exportador
    {
        public static void ACsv<T>(IEnumerable<T> datos, string ruta)
        {
            var props = typeof(T).GetProperties();
            var sb = new StringBuilder();
            sb.AppendLine(string.Join(";", props.Select(p => p.Name)));
            foreach (var d in datos) sb.AppendLine(string.Join(";", props.Select(p => p.GetValue(d)?.ToString())));
            File.WriteAllText(ruta, sb.ToString(), Encoding.UTF8);
        }
    }
}
