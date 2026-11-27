// Trae las herramientas básicas de C# (tipos de uso común)
using System;
// Trae las herramientas para crear ventanas (Windows Forms)
using System.Windows.Forms;

// Agrupa todo el código de la parte visual del sistema.
namespace Presentacion
{
   // Punto de entrada: aquí empieza a ejecutarse el programa.
    internal static class Program
    {   // Indica que la ventana trabaja en un solo hilo, requisito de Windows Forms.
        [STAThread]
        // Main es el método que se ejecuta primero al abrir el programa
        static void Main()
        {    // Prepara la configuración básica de las ventanas como la fuente, estilos...
            ApplicationConfiguration.Initialize();
            // Crea la ventana de inicio de sesión; "using" la libera de la memoria al terminar.
            using var login = new FrmLogin();

            // login.ShowDialog(): muestra la ventana de login 
            //DialogResult.OK: es la respuesta que devuelve el login al cerrarse.
            // Application.Run(new FrmPrincipal()): abre la ventana principal del sistema
            //   y mantiene el programa funcionando hasta que el usuario la cierre.
            if (login.ShowDialog() == DialogResult.OK) Application.Run(new FrmPrincipal());
        }
    }
}
