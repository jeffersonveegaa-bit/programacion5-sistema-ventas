using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Negocio
{
    // Exportación a CSV (abre en Excel).
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
