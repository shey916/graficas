namespace graficas
{
    public partial class FormProgreso : Form
    {
        public FormProgreso()
        {
            InitializeComponent();
        }

        public void ActualizarMensaje(string mensaje)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(ActualizarMensaje), mensaje);
                return;
            }
            
            lblMensaje.Text = mensaje;
            Application.DoEvents();
        }
    }
}