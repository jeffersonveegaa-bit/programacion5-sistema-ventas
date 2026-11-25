using System;
using System.Drawing;
using System.Windows.Forms;
using Negocio;

namespace Presentacion
{
    /// <summary>Pantalla de inicio de sesión.</summary>
    public class FrmLogin : Form
    {
        private readonly TextBox txtUsuario = new();
        private readonly TextBox txtClave = new() { UseSystemPasswordChar = true };

        public FrmLogin()
        {
            Text = "Iniciar sesión";
            FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; MinimizeBox = false;
            Estilo.Formulario(this);
            AutoSize = true; AutoSizeMode = AutoSizeMode.GrowAndShrink; Padding = new Padding(32, 28, 32, 24);

            var btn = Estilo.Boton("Ingresar");
            btn.AutoSize = false; btn.Size = new Size(280, 40); btn.Margin = new Padding(0, 6, 0, 0);
            btn.Click += Ingresar; AcceptButton = btn;

            /// <summary>Dock = Fill hace que el Padding del formulario sea el margen real a los cuatro lados</summary>
            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };
            /// <summary>Título y subtítulo centrados sobre el ancho de los campos</summary>
            panel.Controls.Add(new Label
            {
                Text = "Sistema de Ventas",
                Font = Estilo.Titulo,
                AutoSize = false,
                Size = new Size(280, 40),
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(0)
            });
            panel.Controls.Add(new Label
            {
                Text = "Inicie sesión para continuar",
                ForeColor = Estilo.TextoSuave,
                AutoSize = false,
                Size = new Size(280, 26),
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(0, 0, 0, 18)
            });
            panel.Controls.Add(Estilo.Campo("Usuario", txtUsuario, 280));
            panel.Controls.Add(Estilo.Campo("Contraseña", txtClave, 280));
            panel.Controls.Add(btn);
            Controls.Add(panel);
            Shown += (_, _) => txtUsuario.Focus();
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