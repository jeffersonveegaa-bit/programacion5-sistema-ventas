using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp.Fonts;
using System;
using System.Collections.Generic;
using System.Linq;
using ClosedXML.Excel;

namespace Negocio
{
    public static class Exportador
    {

        /// <summary> Constructor para fuentes de PDFSharp </summary>
        static Exportador()
        {
            GlobalFontSettings.UseWindowsFontsUnderWindows = true;
        }

        /// <summary> Exportación a Excel </summary>
        public static void AExcel<T>(IEnumerable<T> datos, string ruta)
        {
            var propiedades = typeof(T).GetProperties();
            var registros = datos.ToList();

            using var libro = new XLWorkbook();
            var hoja = libro.Worksheets.Add("Reporte");

            var tabla = hoja.Cell(1, 1).InsertTable(registros, "Reporte", true);
            tabla.Theme = XLTableTheme.TableStyleMedium2;

            for (int columna = 0; columna < propiedades.Length; columna++)
            {
                var tipo = Nullable.GetUnderlyingType(propiedades[columna].PropertyType) ?? propiedades[columna].PropertyType;
                var columnaExcel = hoja.Column(columna + 1);

                if (tipo == typeof(DateTime)) columnaExcel.Style.DateFormat.Format = "dd/MM/yyyy HH:mm";
                else if (tipo == typeof(decimal)) columnaExcel.Style.NumberFormat.Format = "#,##0.00";
            }

            hoja.Columns().AdjustToContents();
            libro.SaveAs(ruta);
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
