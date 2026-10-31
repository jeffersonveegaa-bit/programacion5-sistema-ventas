using System;
using System.Windows.Forms;
using Negocio;

namespace Presentacion
{
    /// <summary>Pantalla de inicio de sesión.</summary>
    public class FrmLogin : Form
    {
        private readonly TextBox txtUsuario = new() { Left = 30, Top = 95, Width = 260 };
        private readonly TextBox txtClave = new() { Left = 30, Top = 155, Width = 260, UseSystemPasswordChar = true };

        public FrmLogin()
        {
            Text = "Iniciar sesión"; ClientSize = new System.Drawing.Size(320, 260);
            FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false;
            Estilo.Formulario(this);
            var titulo = new Label { Text = "Sistema de Ventas", Font = Estilo.Titulo, Left = 28, Top = 20, AutoSize = true };
            var btn = Estilo.Boton("Ingresar"); btn.Left = 30; btn.Top = 200; btn.Width = 260;
            btn.Click += Ingresar; AcceptButton = btn;
            Controls.AddRange(new Control[] { titulo, Estilo.Etiqueta("Usuario", 30, 72), txtUsuario, Estilo.Etiqueta("Contraseña", 30, 132), txtClave, btn });
        }

        private void Ingresar(object? s, EventArgs e)
        {
            try
            {
                if (new AuthService().Login(txtUsuario.Text.Trim(), txtClave.Text)) { DialogResult = DialogResult.OK; Close(); }
                else MessageBox.Show("Usuario o contraseña incorrectos.", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex) { MessageBox.Show("No se pudo conectar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
    }
}
