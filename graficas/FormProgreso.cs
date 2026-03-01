namespace graficas
{
    public partial class FormProgreso : Form
    {
        public FormProgreso()
        {
            InitializeComponent();
        }

        public void ActualizarMensaje(string mensajeNuevo)
        {
            if (RequiereInvocacion())
            {
                InvocarActualizacionSegura(mensajeNuevo);
                return;
            }

            AplicarMensaje(mensajeNuevo);
        }

        private bool RequiereInvocacion()
        {
            return InvokeRequired;
        }

        private void InvocarActualizacionSegura(string mensaje)
        {
            Invoke(new Action<string>(ActualizarMensaje), mensaje);
        }

        private void AplicarMensaje(string mensaje)
        {
            lblMensaje.Text = mensaje;
            Application.DoEvents();
        }
    }
}