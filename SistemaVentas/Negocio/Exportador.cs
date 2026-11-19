using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp.Fonts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Negocio
{
    public static class Exportador
    {

        /// <summary> Constructor para fuentes de PDFSharp </summary>
        static Exportador()
        {
            GlobalFontSettings.UseWindowsFontsUnderWindows = true;
        }

        /// <summary> Exportación a CSV (abre en Excel) </summary>
        public static void ACsv<T>(IEnumerable<T> datos, string ruta)
        {
            var propiedades = typeof(T).GetProperties();
            string separador = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            var contenido = new StringBuilder();

            var encabezados = propiedades.Select(propiedad => EscaparCelda(Regex.Replace(propiedad.Name, "(?<!^)([A-Z])", " $1"), separador));
            contenido.AppendLine(string.Join(separador, encabezados));

            foreach (var registro in datos)
            {
                var valores = propiedades.Select(propiedad => EscaparCelda(FormatearValor(propiedad.GetValue(registro)), separador));
                contenido.AppendLine(string.Join(separador, valores));
            }

            File.WriteAllText(ruta, contenido.ToString(), Encoding.UTF8);
        }

        /// <summary> Formato de valores para la exportación a CSV </summary>
        private static string FormatearValor(object? valor) => valor switch
        {
            null => "",
            DateTime fecha => fecha.ToString("dd/MM/yyyy HH:mm"), decimal monto => monto.ToString("F2"), _ => valor.ToString() ?? ""
        };

        /// <summary> Formato para los separadores en la exportación a CSV </summary>
        private static string EscaparCelda(string texto, string separador) =>
            texto.Contains(separador) || texto.Contains('"') || texto.Contains('\n') ? "\"" + texto.Replace("\"", "\"\"") + "\"" : texto;

        /// <summary> Exportación a PDF </summary>
        public static void APdf<T>(IEnumerable<T> datos, string ruta, string titulo)
        {
            var props = typeof(T).GetProperties();
            var filas = datos.ToList();

            var documento = new Document();
            documento.Styles[StyleNames.Normal]!.Font.Name = "Arial";
            documento.Styles[StyleNames.Normal]!.Font.Size = 9;

            var seccion = documento.AddSection();
            seccion.PageSetup.PageFormat = PageFormat.A4;
            seccion.PageSetup.Orientation = Orientation.Landscape;
            seccion.PageSetup.LeftMargin = Unit.FromCentimeter(1.5);
            seccion.PageSetup.RightMargin = Unit.FromCentimeter(1.5);

            var parrafoTitulo = seccion.AddParagraph(titulo);
            parrafoTitulo.Format.Font.Size = 16;
            parrafoTitulo.Format.Font.Bold = true;
            parrafoTitulo.Format.SpaceAfter = Unit.FromCentimeter(0.5);

            var tabla = seccion.AddTable();
            tabla.Borders.Width = 0.5;
            double anchoColumna = (29.7 - 3.0) / props.Length;
            foreach (var _ in props) tabla.AddColumn(Unit.FromCentimeter(anchoColumna));

            var encabezado = tabla.AddRow();
            encabezado.HeadingFormat = true;
            encabezado.Format.Font.Bold = true;
            encabezado.Shading.Color = Colors.LightGray;
            for (int i = 0; i < props.Length; i++)
                encabezado.Cells[i].AddParagraph(props[i].Name);

            foreach (var fila in filas)
            {
                var filaTabla = tabla.AddRow();
                for (int i = 0; i < props.Length; i++)
                    filaTabla.Cells[i].AddParagraph(props[i].GetValue(fila)?.ToString() ?? "");
            }

            var renderizador = new PdfDocumentRenderer { Document = documento };
            renderizador.RenderDocument();
            renderizador.PdfDocument.Save(ruta);
        }
    }
}
