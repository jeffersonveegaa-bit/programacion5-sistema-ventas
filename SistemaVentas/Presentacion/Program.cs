using System;
using System.Windows.Forms;

namespace Presentacion
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            using var login = new FrmLogin();
            if (login.ShowDialog() == DialogResult.OK) Application.Run(new FrmPrincipal());
        }
    }
}
