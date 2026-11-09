using System;
using System.Collections.Generic;
using Entidades;
using MySql.Data.MySqlClient;

namespace Datos
{
    /// <sumary> Conexión con la base de datos </sumary>
    public static class Conexion
    {
        public static MySqlConnection Obtener()
        {
            var cadenaConexion = Environment.GetEnvironmentVariable("VENTAS_CONEXION"); 
            if (string.IsNullOrWhiteSpace(cadenaConexion))
                throw new InvalidOperationException(
                    "Falta la variable de entorno VENTAS_CONEXION");
            return new MySqlConnection(cadenaConexion);
        }
    }

    /// <sumary> Contrato genérico de repositorio </sumary>
    public interface IRepositorio<T, TKey>
    {
        List<T> ObtenerTodos();
        void Insertar(T item);
        void Actualizar(T item);
        void Eliminar(TKey id);
    }

    /// <summary> DAO de productos (consultas parametrizadas contra SQL Injection) </summary>
    public class ProductoDAO : IRepositorio<Producto, string>
    {
        public List<Producto> ObtenerTodos()
        {
            var lista = new List<Producto>();
            using var conexion = Conexion.Obtener(); conexion.Open();
            using var comando = new MySqlCommand("SELECT codigo,nombre,descripcion,precio,stock FROM productos", conexion);
            using var lector = comando.ExecuteReader();
            while (lector.Read())
                lista.Add(new Producto { Codigo = lector.GetString(0), Nombre = lector.GetString(1), Descripcion = lector.IsDBNull(2) ? "" : lector.GetString(2), Precio = lector.GetDecimal(3), Stock = lector.GetInt32(4) });
            return lista;
        }

        public void Insertar(Producto producto) => Ejecutar("INSERT INTO productos VALUES (@c,@n,@d,@p,@s)", producto);
        public void Actualizar(Producto producto) => Ejecutar("UPDATE productos SET nombre=@n,descripcion=@d,precio=@p,stock=@s WHERE codigo=@c", producto);
        public void Eliminar(string codigo)
        {
            using var conexion = Conexion.Obtener(); conexion.Open();
            using var comando = new MySqlCommand("DELETE FROM productos WHERE codigo=@c", conexion);
            comando.Parameters.AddWithValue("@c", codigo); comando.ExecuteNonQuery();
        }
        private static void Ejecutar(string sql, Producto p)
        {
            using var conexion = Conexion.Obtener(); conexion.Open();
            using var comando = new MySqlCommand(sql, conexion);
            comando.Parameters.AddWithValue("@c", p.Codigo); comando.Parameters.AddWithValue("@n", p.Nombre);
            comando.Parameters.AddWithValue("@d", p.Descripcion); comando.Parameters.AddWithValue("@p", p.Precio);
            comando.Parameters.AddWithValue("@s", p.Stock); comando.ExecuteNonQuery();
        }
    }

    /// <summary> DAO de clientes </summary>
    public class ClienteDAO : IRepositorio<Cliente, int>
    {
        public List<Cliente> ObtenerTodos()
        {
            var lista = new List<Cliente>();
            using var conexion = Conexion.Obtener(); conexion.Open();
            using var comando = new MySqlCommand("SELECT id,nombre,telefono,correo FROM clientes", conexion);
            using var lector = comando.ExecuteReader();
            while (lector.Read())
                lista.Add(new Cliente { Id = lector.GetInt32(0), Nombre = lector.GetString(1), Telefono = lector.GetString(2), Correo = lector.GetString(3) });
            return lista;
        }
        public void Insertar(Cliente cliente) => Ejecutar("INSERT INTO clientes (nombre,telefono,correo) VALUES (@n,@t,@e)", cliente);
        public void Actualizar(Cliente cliente) => Ejecutar("UPDATE clientes SET nombre=@n,telefono=@t,correo=@e WHERE id=@id", cliente);
        public void Eliminar(int id)
        {
            using var conexion = Conexion.Obtener(); conexion.Open();
            using var comando = new MySqlCommand("DELETE FROM clientes WHERE id=@id", conexion);
            comando.Parameters.AddWithValue("@id", id); comando.ExecuteNonQuery();
        }
        private static void Ejecutar(string sql, Cliente cliente)
        {
            using var conexion = Conexion.Obtener(); conexion.Open();
            using var comando = new MySqlCommand(sql, conexion);
            comando.Parameters.AddWithValue("@id", cliente.Id); comando.Parameters.AddWithValue("@n", cliente.Nombre);
            comando.Parameters.AddWithValue("@t", cliente.Telefono); comando.Parameters.AddWithValue("@e", cliente.Correo);
            comando.ExecuteNonQuery();
        }
    }

    /// <summary> DAO de ventas: guarda cabecera, detalle y descuenta stock en una transacción </summary>
    public class VentaDAO
    {
        public int Registrar(Venta venta)
        {
            using var conexion = Conexion.Obtener(); conexion.Open();
            using var transaccion = conexion.BeginTransaction();
            try
            {
                var comandoVenta = new MySqlCommand("INSERT INTO ventas (fecha,cliente_id,total) VALUES (@f,@c,@t); SELECT LAST_INSERT_ID();", conexion, transaccion);
                comandoVenta.Parameters.AddWithValue("@f", venta.Fecha); comandoVenta.Parameters.AddWithValue("@c", venta.ClienteId); comandoVenta.Parameters.AddWithValue("@t", venta.Total);
                int ventaID = Convert.ToInt32(comandoVenta.ExecuteScalar());
                foreach (var detalle in venta.Detalles)
                {
                    var comandoDetalle = new MySqlCommand("INSERT INTO detalle_venta (venta_id,producto_codigo,cantidad,precio) VALUES (@v,@p,@c,@pr)", conexion, transaccion);
                    comandoDetalle.Parameters.AddWithValue("@v", ventaID); comandoDetalle.Parameters.AddWithValue("@p", detalle.CodigoProducto);
                    comandoDetalle.Parameters.AddWithValue("@c", detalle.Cantidad); comandoDetalle.Parameters.AddWithValue("@pr", detalle.Precio); comandoDetalle.ExecuteNonQuery();
                    var comandoStock = new MySqlCommand("UPDATE productos SET stock = stock - @c WHERE codigo=@p", conexion, transaccion);
                    comandoStock.Parameters.AddWithValue("@c", detalle.Cantidad); comandoStock.Parameters.AddWithValue("@p", detalle.CodigoProducto); comandoStock.ExecuteNonQuery();
                }
                transaccion.Commit();
                return ventaID;
            }
            catch { transaccion.Rollback(); throw; }
        }

        public List<VentaReporteDTO> Reporte()
        {
            var lista = new List<VentaReporteDTO>();
            using var conexion = Conexion.Obtener(); conexion.Open();
            using var comando = new MySqlCommand("SELECT v.id,v.fecha,c.nombre,v.total FROM ventas v JOIN clientes c ON c.id=v.cliente_id ORDER BY v.fecha DESC", conexion);
            using var lector = comando.ExecuteReader();
            while (lector.Read()) lista.Add(new VentaReporteDTO { Id = lector.GetInt32(0), Fecha = lector.GetDateTime(1), Cliente = lector.GetString(2), Total = lector.GetDecimal(3) });
            return lista;
        }
    }

    /// <summary> Valida credenciales de usuario </summary>
    public class UsuarioDAO
    {
        public bool Validar(string usuario, string clave)
        {
            using var conexion = Conexion.Obtener(); conexion.Open();
            using var comando = new MySqlCommand("SELECT COUNT(*) FROM usuarios WHERE usuario=@u AND clave=@c", conexion);
            comando.Parameters.AddWithValue("@u", usuario); comando.Parameters.AddWithValue("@c", clave);
            return Convert.ToInt32(comando.ExecuteScalar()) > 0;
        }
    }

    /// <summary> Factory crea los repositorios </summary>
    public static class RepositorioFactory
    {
        public static ProductoDAO Productos() => new();
        public static ClienteDAO Clientes() => new();
        public static VentaDAO Ventas() => new();
        public static UsuarioDAO Usuarios() => new();
    }
}
