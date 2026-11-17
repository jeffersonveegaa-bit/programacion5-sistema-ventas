using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp.Fonts;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Negocio
{
    /// <summary> Exportación a CSV (abre en Excel) </summary>
    public static class Exportador
    {

        /// <summary> Constructor para fuentes de PDFSharp </summary>
        static Exportador()
        {
            GlobalFontSettings.UseWindowsFontsUnderWindows = true;
        }
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
