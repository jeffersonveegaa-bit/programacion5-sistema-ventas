using System;
using System.Collections.Generic;
using Entidades;
using MySql.Data.MySqlClient;

namespace Datos
{
    /// <summary>Provee las  conexiones a MySQL.</summary>
    public static class Conexion
    {
        private const string Cadena = "Server=localhost;Database=ventasdb;Uid=root;Pwd=root;";
        public static MySqlConnection Obtener() => new MySqlConnection(Cadena);
    }

    /// <summary>Contrato genérico de repositorio.</summary>
    public interface IRepositorio<T, TKey>
    {
        List<T> ObtenerTodos();
        void Insertar(T item);
        void Actualizar(T item);
        void Eliminar(TKey id);
    }

    /// <summary>DAO de productos (consultas parametrizadas contra SQL Injection).</summary>
    public class ProductoDAO : IRepositorio<Producto, string>
    {
        public List<Producto> ObtenerTodos()
        {
            var lista = new List<Producto>();
            using var cn = Conexion.Obtener(); cn.Open();
            using var cmd = new MySqlCommand("SELECT codigo,nombre,descripcion,precio,stock FROM productos", cn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add(new Producto { Codigo = r.GetString(0), Nombre = r.GetString(1), Descripcion = r.IsDBNull(2) ? "" : r.GetString(2), Precio = r.GetDecimal(3), Stock = r.GetInt32(4) });
            return lista;
        }
        public void Insertar(Producto p) => Ejecutar("INSERT INTO productos VALUES (@c,@n,@d,@p,@s)", p);
        public void Actualizar(Producto p) => Ejecutar("UPDATE productos SET nombre=@n,descripcion=@d,precio=@p,stock=@s WHERE codigo=@c", p);
        public void Eliminar(string codigo)
        {
            using var cn = Conexion.Obtener(); cn.Open();
            using var cmd = new MySqlCommand("DELETE FROM productos WHERE codigo=@c", cn);
            cmd.Parameters.AddWithValue("@c", codigo); cmd.ExecuteNonQuery();
        }
        private static void Ejecutar(string sql, Producto p)
        {
            using var cn = Conexion.Obtener(); cn.Open();
            using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@c", p.Codigo); cmd.Parameters.AddWithValue("@n", p.Nombre);
            cmd.Parameters.AddWithValue("@d", p.Descripcion); cmd.Parameters.AddWithValue("@p", p.Precio);
            cmd.Parameters.AddWithValue("@s", p.Stock); cmd.ExecuteNonQuery();
        }
    }

    /// <summary>DAO de clientes.</summary>
    public class ClienteDAO : IRepositorio<Cliente, int>
    {
        public List<Cliente> ObtenerTodos()
        {
            var lista = new List<Cliente>();
            using var cn = Conexion.Obtener(); cn.Open();
            using var cmd = new MySqlCommand("SELECT id,nombre,telefono,correo FROM clientes", cn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add(new Cliente { Id = r.GetInt32(0), Nombre = r.GetString(1), Telefono = r.GetString(2), Correo = r.GetString(3) });
            return lista;
        }
        public void Insertar(Cliente c) => Ejecutar("INSERT INTO clientes (nombre,telefono,correo) VALUES (@n,@t,@e)", c);
        public void Actualizar(Cliente c) => Ejecutar("UPDATE clientes SET nombre=@n,telefono=@t,correo=@e WHERE id=@id", c);
        public void Eliminar(int id)
        {
            using var cn = Conexion.Obtener(); cn.Open();
            using var cmd = new MySqlCommand("DELETE FROM clientes WHERE id=@id", cn);
            cmd.Parameters.AddWithValue("@id", id); cmd.ExecuteNonQuery();
        }
        private static void Ejecutar(string sql, Cliente c)
        {
            using var cn = Conexion.Obtener(); cn.Open();
            using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@id", c.Id); cmd.Parameters.AddWithValue("@n", c.Nombre);
            cmd.Parameters.AddWithValue("@t", c.Telefono); cmd.Parameters.AddWithValue("@e", c.Correo);
            cmd.ExecuteNonQuery();
        }
    }

    /// <summary>DAO de ventas: guarda cabecera, detalle y descuenta stock en una transacción.</summary>
    public class VentaDAO
    {
        public int Registrar(Venta v)
        {
            using var cn = Conexion.Obtener(); cn.Open();
            using var tx = cn.BeginTransaction();
            try
            {
                var cab = new MySqlCommand("INSERT INTO ventas (fecha,cliente_id,total) VALUES (@f,@c,@t); SELECT LAST_INSERT_ID();", cn, tx);
                cab.Parameters.AddWithValue("@f", v.Fecha); cab.Parameters.AddWithValue("@c", v.ClienteId); cab.Parameters.AddWithValue("@t", v.Total);
                int id = Convert.ToInt32(cab.ExecuteScalar());
                foreach (var d in v.Detalles)
                {
                    var det = new MySqlCommand("INSERT INTO detalle_venta (venta_id,producto_codigo,cantidad,precio) VALUES (@v,@p,@c,@pr)", cn, tx);
                    det.Parameters.AddWithValue("@v", id); det.Parameters.AddWithValue("@p", d.CodigoProducto);
                    det.Parameters.AddWithValue("@c", d.Cantidad); det.Parameters.AddWithValue("@pr", d.Precio); det.ExecuteNonQuery();
                    var st = new MySqlCommand("UPDATE productos SET stock = stock - @c WHERE codigo=@p", cn, tx);
                    st.Parameters.AddWithValue("@c", d.Cantidad); st.Parameters.AddWithValue("@p", d.CodigoProducto); st.ExecuteNonQuery();
                }
                tx.Commit();
                return id;
            }
            catch { tx.Rollback(); throw; }
        }

        public List<VentaReporteDTO> Reporte()
        {
            var lista = new List<VentaReporteDTO>();
            using var cn = Conexion.Obtener(); cn.Open();
            using var cmd = new MySqlCommand("SELECT v.id,v.fecha,c.nombre,v.total FROM ventas v JOIN clientes c ON c.id=v.cliente_id ORDER BY v.fecha DESC", cn);
            using var r = cmd.ExecuteReader();
            while (r.Read()) lista.Add(new VentaReporteDTO { Id = r.GetInt32(0), Fecha = r.GetDateTime(1), Cliente = r.GetString(2), Total = r.GetDecimal(3) });
            return lista;
        }
    }

    /// <summary>Valida credenciales de usuario.</summary>
    public class UsuarioDAO
    {
        public bool Validar(string usuario, string clave)
        {
            using var cn = Conexion.Obtener(); cn.Open();
            using var cmd = new MySqlCommand("SELECT COUNT(*) FROM usuarios WHERE usuario=@u AND clave=@c", cn);
            cmd.Parameters.AddWithValue("@u", usuario); cmd.Parameters.AddWithValue("@c", clave);
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }
    }

    /// <summary>Factory que crea los repositorios.</summary>
    public static class RepositorioFactory
    {
        public static ProductoDAO Productos() => new();
        public static ClienteDAO Clientes() => new();
        public static VentaDAO Ventas() => new();
        public static UsuarioDAO Usuarios() => new();
    }
}
