using Microsoft.VisualBasic.FileIO;
using System.Windows.Forms.DataVisualization.Charting;
using System.Collections.Concurrent;

namespace graficas
{
    public partial class Form1 : Form
    {
        private List<string[]> _datosCompletos = new List<string[]>();
        private string[] _encabezados = Array.Empty<string>();
        private int _columnIndexSeleccionada = -1;

        public Form1()
        {
            InitializeComponent();
            ConfigurarDataGridView();
        }

        private void ConfigurarDataGridView()
        {
            // Configuración optimizada para grandes volúmenes de datos
            dgvDatos.VirtualMode = true;
            dgvDatos.CellValueNeeded += DgvDatos_CellValueNeeded;
            dgvDatos.SelectionChanged += DgvDatos_SelectionChanged;
            dgvDatos.AllowUserToAddRows = false;
            dgvDatos.AllowUserToDeleteRows = false;
            dgvDatos.ReadOnly = true;
            dgvDatos.RowHeadersVisible = false;
            dgvDatos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        }

        private void DgvDatos_CellValueNeeded(object? sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < _datosCompletos.Count && 
                e.ColumnIndex >= 0 && e.ColumnIndex < _encabezados.Length)
            {
                e.Value = _datosCompletos[e.RowIndex][e.ColumnIndex];
            }
        }

        private void DgvDatos_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvDatos.CurrentCell != null)
            {
                _columnIndexSeleccionada = dgvDatos.CurrentCell.ColumnIndex;
                
                // Resaltar visualmente la columna seleccionada
                foreach (DataGridViewColumn col in dgvDatos.Columns)
                {
                    col.DefaultCellStyle.BackColor = Color.White;
                    col.HeaderCell.Style.BackColor = Color.FromArgb(240, 240, 240);
                }
                
                if (_columnIndexSeleccionada >= 0 && _columnIndexSeleccionada < dgvDatos.Columns.Count)
                {
                    dgvDatos.Columns[_columnIndexSeleccionada].DefaultCellStyle.BackColor = Color.FromArgb(230, 244, 255);
                    dgvDatos.Columns[_columnIndexSeleccionada].HeaderCell.Style.BackColor = Color.FromArgb(0, 122, 204);
                    dgvDatos.Columns[_columnIndexSeleccionada].HeaderCell.Style.ForeColor = Color.White;
                }
            }
        }

        private async void btnLoadFile_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                    openFileDialog.Title = "Seleccionar archivo CSV";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Deshabilitar controles durante la carga
                        HabilitarControles(false);
                        
                        await LoadCsvFileAsync(openFileDialog.FileName);
                        
                        HabilitarControles(true);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el archivo: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                HabilitarControles(true);
            }
        }

        private async Task LoadCsvFileAsync(string filePath)
        {
            // Mostrar formulario de progreso
            using (FormProgreso formProgreso = new FormProgreso())
            {
                formProgreso.Show();
                formProgreso.ActualizarMensaje("Cargando archivo CSV...");

                await Task.Run(() =>
                {
                    _datosCompletos.Clear();
                    dgvDatos.Columns.Clear();

                    using (TextFieldParser parser = new TextFieldParser(filePath))
                    {
                        parser.TextFieldType = FieldType.Delimited;
                        parser.SetDelimiters(",");
                        parser.HasFieldsEnclosedInQuotes = true;

                        // Leer encabezados
                        if (!parser.EndOfData)
                        {
                            _encabezados = parser.ReadFields() ?? Array.Empty<string>();
                        }

                        int contador = 0;
                        // Leer datos con buffer
                        while (!parser.EndOfData)
                        {
                            string[] fields = parser.ReadFields() ?? Array.Empty<string>();
                            _datosCompletos.Add(fields);

                            contador++;
                            if (contador % 10000 == 0)
                            {
                                // Actualizar progreso cada 10,000 registros
                                this.Invoke((MethodInvoker)delegate
                                {
                                    formProgreso.ActualizarMensaje($"Cargando... {contador:N0} registros");
                                });
                            }
                        }
                    }
                });

                // Configurar DataGridView en el hilo de UI
                formProgreso.ActualizarMensaje("Configurando tabla...");
                
                dgvDatos.Columns.Clear();
                foreach (string header in _encabezados)
                {
                    dgvDatos.Columns.Add(header, header);
                    dgvDatos.Columns[dgvDatos.Columns.Count - 1].Width = 120;
                }

                dgvDatos.RowCount = _datosCompletos.Count;

                formProgreso.Close();

                MessageBox.Show($"Archivo cargado exitosamente.\n\n" +
                               $"📊 Registros: {_datosCompletos.Count:N0}\n" +
                               $"📋 Columnas: {_encabezados.Length}", 
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
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

        private async void MostrarGrafica(SeriesChartType tipoGrafica, string titulo)
        {
            try
            {
                // Validar que haya datos
                if (_datosCompletos.Count == 0 || _encabezados.Length == 0)
                {
                    MessageBox.Show("No hay datos cargados en la tabla.", 
                        "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validar columna seleccionada
                if (_columnIndexSeleccionada < 0 || _columnIndexSeleccionada >= _encabezados.Length)
                {
                    MessageBox.Show("Por favor, seleccione una celda en la columna que desea graficar.", 
                        "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string columnName = _encabezados[_columnIndexSeleccionada];

                // Mostrar progreso
                using (FormProgreso formProgreso = new FormProgreso())
                {
                    formProgreso.Show();
                    formProgreso.ActualizarMensaje($"Procesando columna '{columnName}'...");

                    // Procesar datos en paralelo
                    Dictionary<string, int> datosAgrupados = await Task.Run(() =>
                    {
                        ConcurrentDictionary<string, int> resultadoConcurrente = new ConcurrentDictionary<string, int>();

                        Parallel.ForEach(_datosCompletos, row =>
                        {
                            if (_columnIndexSeleccionada < row.Length)
                            {
                                string valor = row[_columnIndexSeleccionada]?.Trim() ?? "Sin datos";
                                
                                if (!string.IsNullOrWhiteSpace(valor))
                                {
                                    resultadoConcurrente.AddOrUpdate(valor, 1, (key, oldValue) => oldValue + 1);
                                }
                            }
                        });

                        return new Dictionary<string, int>(resultadoConcurrente);
                    });

                    formProgreso.Close();

                    if (datosAgrupados.Count == 0)
                    {
                        MessageBox.Show("No hay datos válidos en la columna seleccionada.", 
                            "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Limitar a los top 50 para gráficas más legibles
                    var datosTop = datosAgrupados
                        .OrderByDescending(x => x.Value)
                        .Take(50)
                        .ToDictionary(x => x.Key, x => x.Value);

                    // Abrir formulario de gráfica
                    FormGrafica formGrafica = new FormGrafica(
                        datosTop, 
                        tipoGrafica, 
                        $"{titulo} - {columnName}", 
                        columnName,
                        _datosCompletos.Count);
                    formGrafica.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar la gráfica: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HabilitarControles(bool habilitar)
        {
            btnLoadFile.Enabled = habilitar;
            btnGraficaDona.Enabled = habilitar;
            btnGraficaBarras.Enabled = habilitar;
            btnGraficaLineas.Enabled = habilitar;
            dgvDatos.Enabled = habilitar;
            
            if (!habilitar)
            {
                this.Cursor = Cursors.WaitCursor;
            }
            else
            {
                this.Cursor = Cursors.Default;
            }
        }
    }
}

