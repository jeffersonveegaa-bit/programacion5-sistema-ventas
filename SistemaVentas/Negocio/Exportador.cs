using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Negocio
{
    /// <summary> Exportación a CSV (abre en Excel) </summary>
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

        /// <summary> Exportación a PDF </summary>
        public static void APdf<T>(IEnumerable<T> datos, string ruta, string titulo)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var props = typeof(T).GetProperties();
            var filas = datos.ToList();

            Document.Create(documento =>
            {
                documento.Page(pagina =>
                {
                    pagina.Size(PageSizes.A4.Landscape());
                    pagina.Margin(30);
                    pagina.DefaultTextStyle(x => x.FontSize(10));

                    pagina.Header().Text(titulo).SemiBold().FontSize(18);

                    pagina.Content().PaddingVertical(10).Table(tabla =>
                    {
                        tabla.ColumnsDefinition(columnas =>
                        {
                            foreach (var _ in props) columnas.RelativeColumn();
                        });

                        tabla.Header(encabezado =>
                        {
                            foreach (var p in props)
                                encabezado.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text(p.Name).SemiBold();
                        });

                        foreach (var fila in filas)
                            foreach (var p in props)
                                tabla.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(p.GetValue(fila)?.ToString() ?? "");
                    });

                    pagina.Footer().AlignCenter().Text(texto =>
                    {
                        texto.Span("Página ");
                        texto.CurrentPageNumber();
                    });
                });
            }).GeneratePdf(ruta);
        }
    }
}
