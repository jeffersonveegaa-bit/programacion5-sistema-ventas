using System.Collections.Generic;

namespace Datos
{
    /// <sumary> Contrato genérico de repositorio </sumary>
    public interface IRepositorio<T, TKey>
    {
        List<T> ObtenerTodos();
        void Insertar(T item);
        void Actualizar(T item);
        void Eliminar(TKey id);
    }
}
