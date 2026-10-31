using System;
using System.Collections.Generic;
using Entidades;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Negocio;

namespace Pruebas
{
    [TestClass]
    public class PruebasNegocio
    {
        [TestMethod]
        public void CalcularTotal_SumaSubtotales()
        {
            var d = new List<DetalleVenta> { new() { Cantidad = 2, Precio = 100 }, new() { Cantidad = 1, Precio = 50 } };
            Assert.AreEqual(250m, VentaService.CalcularTotal(d));
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ValidarStock_Insuficiente_Lanza() => VentaService.ValidarStock(new Producto { Stock = 3 }, 5);

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void Producto_PrecioCero_Lanza() => ProductoService.Validar(new Producto { Codigo = "X", Nombre = "Y", Precio = 0 });

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void Cliente_CorreoInvalido_Lanza() => ClienteService.Validar(new Cliente { Nombre = "Ana", Correo = "malo" });
    }
}
