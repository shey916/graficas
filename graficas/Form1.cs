using Microsoft.VisualBasic.FileIO;
using System.Windows.Forms.DataVisualization.Charting;
using System.Collections.Concurrent;

namespace graficas
{
    public partial class Form1 : Form
    {
        private const int INTERVALO_ACTUALIZACION_PROGRESO = 10000;
        private const int LIMITE_VALORES_GRAFICA = 50;
        private const int ANCHO_COLUMNA_PREDETERMINADO = 120;

        private readonly List<string[]> _registrosCompletos = new();
        private string[] _nombresColumnas = Array.Empty<string>();
        private int _indiceColumnaSeleccionada = -1;

        public Form1()
        {
            InitializeComponent();
            ConfigurarDataGridView();
        }

        private void ConfigurarDataGridView()
        {
            dgvDatos.VirtualMode = true;
            dgvDatos.CellValueNeeded += AlSolicitarValorCelda;
            dgvDatos.SelectionChanged += AlCambiarSeleccion;
            dgvDatos.AllowUserToAddRows = false;
            dgvDatos.AllowUserToDeleteRows = false;
            dgvDatos.ReadOnly = true;
            dgvDatos.RowHeadersVisible = false;
            dgvDatos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        }

        private void AlSolicitarValorCelda(object? sender, DataGridViewCellValueEventArgs e)
        {
            if (EsIndiceValido(e.RowIndex, e.ColumnIndex))
            {
                e.Value = _registrosCompletos[e.RowIndex][e.ColumnIndex];
            }
        }

        private bool EsIndiceValido(int fila, int columna)
        {
            return fila >= 0 && fila < _registrosCompletos.Count &&
                   columna >= 0 && columna < _nombresColumnas.Length;
        }

        private void AlCambiarSeleccion(object? sender, EventArgs e)
        {
            if (dgvDatos.CurrentCell != null)
            {
                _indiceColumnaSeleccionada = dgvDatos.CurrentCell.ColumnIndex;
                ResaltarColumnaSeleccionada();
            }
        }

        private void ResaltarColumnaSeleccionada()
        {
            RestablecerEstiloColumnas();
            AplicarEstiloColumnaActiva();
        }

        private void RestablecerEstiloColumnas()
        {
            foreach (DataGridViewColumn columna in dgvDatos.Columns)
            {
                columna.DefaultCellStyle.BackColor = Color.White;
                columna.HeaderCell.Style.BackColor = Color.FromArgb(240, 240, 240);
                columna.HeaderCell.Style.ForeColor = Color.Black;
            }
        }

        private void AplicarEstiloColumnaActiva()
        {
            if (HayColumnaSeleccionadaValida())
            {
                DataGridViewColumn columnaActiva = dgvDatos.Columns[_indiceColumnaSeleccionada];
                columnaActiva.DefaultCellStyle.BackColor = Color.FromArgb(230, 244, 255);
                columnaActiva.HeaderCell.Style.BackColor = Color.FromArgb(0, 122, 204);
                columnaActiva.HeaderCell.Style.ForeColor = Color.White;
            }
        }

        private bool HayColumnaSeleccionadaValida()
        {
            return _indiceColumnaSeleccionada >= 0 && _indiceColumnaSeleccionada < dgvDatos.Columns.Count;
        }

        private async void btnLoadFile_Click(object sender, EventArgs e)
        {
            try
            {
                await CargarArchivoCSVAsync();
            }
            catch (Exception ex)
            {
                MostrarErrorCarga(ex);
                HabilitarControles(true);
            }
        }

        private async Task CargarArchivoCSVAsync()
        {
            using (OpenFileDialog dialogoAbrir = CrearDialogoAbrirCSV())
            {
                if (UsuarioSeleccionoArchivo(dialogoAbrir))
                {
                    await ProcesarArchivoSeleccionado(dialogoAbrir.FileName);
                }
            }
        }

        private OpenFileDialog CrearDialogoAbrirCSV()
        {
            return new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                Title = "Seleccionar archivo CSV"
            };
        }

        private bool UsuarioSeleccionoArchivo(OpenFileDialog dialogo)
        {
            return dialogo.ShowDialog() == DialogResult.OK;
        }

        private async Task ProcesarArchivoSeleccionado(string rutaArchivo)
        {
            HabilitarControles(false);
            await CargarDatosDesdeArchivoAsync(rutaArchivo);
            HabilitarControles(true);
        }

        private async Task CargarDatosDesdeArchivoAsync(string rutaArchivo)
        {
            using (FormProgreso dialogoProgreso = new FormProgreso())
            {
                dialogoProgreso.Show();
                
                await LeerArchivoCSVEnHiloSecundario(rutaArchivo, dialogoProgreso);
                ConfigurarTablaConDatosCargados(dialogoProgreso);
                
                dialogoProgreso.Close();
                MostrarEstadisticasCarga();
            }
        }

        private async Task LeerArchivoCSVEnHiloSecundario(string rutaArchivo, FormProgreso dialogoProgreso)
        {
            await Task.Run(() => LeerArchivoCSV(rutaArchivo, dialogoProgreso));
        }

        private void LeerArchivoCSV(string rutaArchivo, FormProgreso dialogoProgreso)
        {
            LimpiarDatosAnteriores();
            dialogoProgreso.ActualizarMensaje("Cargando archivo CSV...");

            using (TextFieldParser parser = CrearParserCSV(rutaArchivo))
            {
                LeerEncabezados(parser);
                LeerRegistros(parser, dialogoProgreso);
            }
        }

        private void LimpiarDatosAnteriores()
        {
            _registrosCompletos.Clear();
            dgvDatos.Columns.Clear();
        }

        private TextFieldParser CrearParserCSV(string rutaArchivo)
        {
            return new TextFieldParser(rutaArchivo)
            {
                TextFieldType = FieldType.Delimited,
                Delimiters = new[] { "," },
                HasFieldsEnclosedInQuotes = true
            };
        }

        private void LeerEncabezados(TextFieldParser parser)
        {
            if (!parser.EndOfData)
            {
                _nombresColumnas = parser.ReadFields() ?? Array.Empty<string>();
            }
        }

        private void LeerRegistros(TextFieldParser parser, FormProgreso dialogoProgreso)
        {
            int contadorRegistros = 0;

            while (!parser.EndOfData)
            {
                string[] campos = parser.ReadFields() ?? Array.Empty<string>();
                _registrosCompletos.Add(campos);

                contadorRegistros++;
                ActualizarProgresoSiCorresponde(contadorRegistros, dialogoProgreso);
            }
        }

        private void ActualizarProgresoSiCorresponde(int contadorRegistros, FormProgreso dialogoProgreso)
        {
            if (EsMomentoActualizarProgreso(contadorRegistros))
            {
                ActualizarMensajeProgreso(contadorRegistros, dialogoProgreso);
            }
        }

        private bool EsMomentoActualizarProgreso(int contador)
        {
            return contador % INTERVALO_ACTUALIZACION_PROGRESO == 0;
        }

        private void ActualizarMensajeProgreso(int contadorRegistros, FormProgreso dialogoProgreso)
        {
            this.Invoke((MethodInvoker)delegate
            {
                dialogoProgreso.ActualizarMensaje($"Cargando... {contadorRegistros:N0} registros");
            });
        }

        private void ConfigurarTablaConDatosCargados(FormProgreso dialogoProgreso)
        {
            dialogoProgreso.ActualizarMensaje("Configurando tabla...");
            
            CrearColumnasEnTabla();
            dgvDatos.RowCount = _registrosCompletos.Count;
        }

        private void CrearColumnasEnTabla()
        {
            dgvDatos.Columns.Clear();
            
            foreach (string nombreColumna in _nombresColumnas)
            {
                AgregarColumnaATabla(nombreColumna);
            }
        }

        private void AgregarColumnaATabla(string nombreColumna)
        {
            dgvDatos.Columns.Add(nombreColumna, nombreColumna);
            dgvDatos.Columns[dgvDatos.Columns.Count - 1].Width = ANCHO_COLUMNA_PREDETERMINADO;
        }

        private void MostrarEstadisticasCarga()
        {
            string mensaje = GenerarMensajeEstadisticas();
            
            MessageBox.Show(
                mensaje,
                "Éxito",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private string GenerarMensajeEstadisticas()
        {
            return $"Archivo cargado exitosamente.\n\n" +
                   $"📊 Registros: {_registrosCompletos.Count:N0}\n" +
                   $"📋 Columnas: {_nombresColumnas.Length}";
        }

        private void MostrarErrorCarga(Exception ex)
        {
            MessageBox.Show(
                $"Error al cargar el archivo: {ex.Message}",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private void btnGraficaDona_Click(object sender, EventArgs e)
        {
            MostrarGrafica(SeriesChartType.Doughnut, "Gráfica de Dona");
        }

        private void btnGraficaBarras_Click(object sender, EventArgs e)
        {
            MostrarGrafica(SeriesChartType.Column, "Gráfica de Barras");
        }

        private void btnGraficaLineas_Click(object sender, EventArgs e)
        {
            MostrarGrafica(SeriesChartType.Line, "Gráfica de Líneas");
        }

        private async void MostrarGrafica(SeriesChartType tipoGrafica, string tituloGrafica)
        {
            try
            {
                if (!ValidarDatosParaGrafica())
                {
                    return;
                }

                string nombreColumna = ObtenerNombreColumnaSeleccionada();
                Dictionary<string, int> frecuencias = await CalcularFrecuenciasAsync(nombreColumna);

                if (frecuencias.Count == 0)
                {
                    MostrarAdvertenciaDatosVacios();
                    return;
                }

                Dictionary<string, int> top50 = ObtenerTopValores(frecuencias);
                AbrirFormularioGrafica(tipoGrafica, tituloGrafica, nombreColumna, top50);
            }
            catch (Exception ex)
            {
                MostrarErrorGrafica(ex);
            }
        }

        private bool ValidarDatosParaGrafica()
        {
            if (!HayDatosCargados())
            {
                MostrarAdvertenciaSinDatos();
                return false;
            }

            if (!HayColumnaSeleccionada())
            {
                MostrarAdvertenciaSeleccionarColumna();
                return false;
            }

            return true;
        }

        private bool HayDatosCargados()
        {
            return _registrosCompletos.Count > 0 && _nombresColumnas.Length > 0;
        }

        private bool HayColumnaSeleccionada()
        {
            return _indiceColumnaSeleccionada >= 0 && _indiceColumnaSeleccionada < _nombresColumnas.Length;
        }

        private void MostrarAdvertenciaSinDatos()
        {
            MessageBox.Show(
                "No hay datos cargados en la tabla.",
                "Advertencia",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        private void MostrarAdvertenciaSeleccionarColumna()
        {
            MessageBox.Show(
                "Por favor, seleccione una celda en la columna que desea graficar.",
                "Advertencia",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        private void MostrarAdvertenciaDatosVacios()
        {
            MessageBox.Show(
                "No hay datos válidos en la columna seleccionada.",
                "Advertencia",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        private string ObtenerNombreColumnaSeleccionada()
        {
            return _nombresColumnas[_indiceColumnaSeleccionada];
        }

        private async Task<Dictionary<string, int>> CalcularFrecuenciasAsync(string nombreColumna)
        {
            using (FormProgreso dialogoProgreso = new FormProgreso())
            {
                dialogoProgreso.Show();
                dialogoProgreso.ActualizarMensaje($"Procesando columna '{nombreColumna}'...");

                var frecuencias = await ProcesarDatosEnParalelo();
                
                dialogoProgreso.Close();
                return frecuencias;
            }
        }

        private async Task<Dictionary<string, int>> ProcesarDatosEnParalelo()
        {
            return await Task.Run(() =>
            {
                ConcurrentDictionary<string, int> frecuenciasConcurrentes = new();

                Parallel.ForEach(_registrosCompletos, registro =>
                {
                    AgregarFrecuenciaSiEsValida(registro, frecuenciasConcurrentes);
                });

                return new Dictionary<string, int>(frecuenciasConcurrentes);
            });
        }

        private void AgregarFrecuenciaSiEsValida(string[] registro, ConcurrentDictionary<string, int> frecuencias)
        {
            if (_indiceColumnaSeleccionada < registro.Length)
            {
                string valor = ExtraerValorLimpio(registro[_indiceColumnaSeleccionada]);

                if (!string.IsNullOrWhiteSpace(valor))
                {
                    frecuencias.AddOrUpdate(valor, 1, (clave, valorAnterior) => valorAnterior + 1);
                }
            }
        }

        private string ExtraerValorLimpio(string? valorCelda)
        {
            return valorCelda?.Trim() ?? "Sin datos";
        }

        private Dictionary<string, int> ObtenerTopValores(Dictionary<string, int> frecuencias)
        {
            return frecuencias
                .OrderByDescending(par => par.Value)
                .Take(LIMITE_VALORES_GRAFICA)
                .ToDictionary(par => par.Key, par => par.Value);
        }

        private void AbrirFormularioGrafica(
            SeriesChartType tipoGrafica,
            string tituloBase,
            string nombreColumna,
            Dictionary<string, int> datos)
        {
            string tituloCompleto = $"{tituloBase} - {nombreColumna}";
            
            FormGrafica formulario = new FormGrafica(
                datos,
                tipoGrafica,
                tituloCompleto,
                nombreColumna,
                _registrosCompletos.Count);
                
            formulario.ShowDialog();
        }

        private void MostrarErrorGrafica(Exception ex)
        {
            MessageBox.Show(
                $"Error al generar la gráfica: {ex.Message}",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private void HabilitarControles(bool habilitar)
        {
            CambiarEstadoBotones(habilitar);
            CambiarCursor(habilitar);
        }

        private void CambiarEstadoBotones(bool habilitar)
        {
            btnLoadFile.Enabled = habilitar;
            btnGraficaDona.Enabled = habilitar;
            btnGraficaBarras.Enabled = habilitar;
            btnGraficaLineas.Enabled = habilitar;
            dgvDatos.Enabled = habilitar;
        }

        private void CambiarCursor(bool modoNormal)
        {
            this.Cursor = modoNormal ? Cursors.Default : Cursors.WaitCursor;
        }
    }
}

