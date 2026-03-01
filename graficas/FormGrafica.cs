using System.Windows.Forms.DataVisualization.Charting;

namespace graficas
{
    public partial class FormGrafica : Form
    {
        private Dictionary<string, int> _datos;
        private SeriesChartType _tipoGrafica;
        private string _titulo;
        private string _nombreColumna;
        private int _totalRegistros;

        public FormGrafica(Dictionary<string, int> datos, SeriesChartType tipoGrafica, string titulo, string nombreColumna, int totalRegistros = 0)
        {
            InitializeComponent();
            _datos = datos;
            _tipoGrafica = tipoGrafica;
            _titulo = titulo;
            _nombreColumna = nombreColumna;
            _totalRegistros = totalRegistros;
            ConfigurarGrafica();
        }

        private void ConfigurarGrafica()
        {
            // Configurar el Chart
            chartGrafica.Titles.Clear();
            chartGrafica.Titles.Add(_titulo);
            chartGrafica.Titles[0].Font = new Font("Segoe UI", 14, FontStyle.Bold);

            // Agregar subtítulo con información
            if (_totalRegistros > 0)
            {
                int registrosMostrados = _datos.Values.Sum();
                string subtitulo = _datos.Count >= 50 
                    ? $"Top 50 de {_datos.Count:N0} valores únicos - {registrosMostrados:N0} de {_totalRegistros:N0} registros"
                    : $"{_datos.Count:N0} valores únicos - {_totalRegistros:N0} registros totales";
                
                chartGrafica.Titles.Add(subtitulo);
                chartGrafica.Titles[1].Font = new Font("Segoe UI", 9);
                chartGrafica.Titles[1].ForeColor = Color.Gray;
            }

            // Limpiar series existentes
            chartGrafica.Series.Clear();
            chartGrafica.ChartAreas.Clear();

            // Crear área de gráfico
            ChartArea area = new ChartArea("ChartArea1");
            area.BackColor = Color.White;
            chartGrafica.ChartAreas.Add(area);

            // Crear nueva serie
            Series serie = new Series("Datos");
            serie.ChartType = _tipoGrafica;

            // Ordenar datos por frecuencia (descendente)
            var datosOrdenados = _datos.OrderByDescending(x => x.Value).ToList();

            // Configuraciones específicas por tipo de gráfica
            if (_tipoGrafica == SeriesChartType.Doughnut)
            {
                ConfigurarGraficaDona(serie, datosOrdenados);
            }
            else if (_tipoGrafica == SeriesChartType.Column)
            {
                ConfigurarGraficaBarras(serie, datosOrdenados, area);
            }
            else if (_tipoGrafica == SeriesChartType.Line)
            {
                ConfigurarGraficaLineas(serie, datosOrdenados, area);
            }

            chartGrafica.Series.Add(serie);

            // Configurar leyenda
            chartGrafica.Legends.Clear();
            Legend leyenda = new Legend("Legend1");
            leyenda.Enabled = (_tipoGrafica == SeriesChartType.Doughnut);
            if (_tipoGrafica == SeriesChartType.Doughnut)
            {
                leyenda.Docking = Docking.Right;
                leyenda.Font = new Font("Segoe UI", 8);
            }
            chartGrafica.Legends.Add(leyenda);

            // Colores personalizados
            AsignarColores(serie);
        }

        private void ConfigurarGraficaDona(Series serie, List<KeyValuePair<string, int>> datosOrdenados)
        {
            foreach (var item in datosOrdenados)
            {
                DataPoint punto = new DataPoint();
                punto.YValues = new double[] { item.Value };
                
                double porcentaje = (item.Value * 100.0) / _datos.Values.Sum();
                punto.Label = $"{porcentaje:F1}%";
                punto.LegendText = $"{item.Key} ({item.Value:N0})";
                punto.ToolTip = $"{item.Key}\nFrecuencia: {item.Value:N0}\nPorcentaje: {porcentaje:F2}%";

                serie.Points.Add(punto);
            }

            serie["PieLabelStyle"] = "Outside";
            serie["DoughnutRadius"] = "40";
            serie.IsValueShownAsLabel = true;
            serie.Font = new Font("Segoe UI", 9);
        }

        private void ConfigurarGraficaBarras(Series serie, List<KeyValuePair<string, int>> datosOrdenados, ChartArea area)
        {
            // Configurar serie
            serie.IsValueShownAsLabel = true;
            serie.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            serie["PointWidth"] = "0.8";

            // Agregar puntos individuales
            for (int i = 0; i < datosOrdenados.Count; i++)
            {
                var item = datosOrdenados[i];
                DataPoint punto = new DataPoint();
                punto.SetValueXY(i, item.Value);
                punto.AxisLabel = item.Key.Length > 15 ? item.Key.Substring(0, 12) + "..." : item.Key;
                punto.Label = item.Value.ToString("N0");
                punto.ToolTip = $"{item.Key}\nFrecuencia: {item.Value:N0}";
                
                serie.Points.Add(punto);
            }

            // Configurar área del gráfico
            area.AxisX.Title = _nombreColumna;
            area.AxisX.TitleFont = new Font("Segoe UI", 10, FontStyle.Bold);
            area.AxisY.Title = "Frecuencia";
            area.AxisY.TitleFont = new Font("Segoe UI", 10, FontStyle.Bold);
            
            // Configurar eje X
            area.AxisX.Interval = 1;
            area.AxisX.IntervalType = DateTimeIntervalType.Number;
            area.AxisX.LabelStyle.Angle = -45;
            area.AxisX.LabelStyle.Font = new Font("Segoe UI", 8);
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisX.IsLabelAutoFit = true;
            
            // Configurar eje Y
            area.AxisY.LabelStyle.Format = "N0";
            area.AxisY.LabelStyle.Font = new Font("Segoe UI", 8);
            area.AxisY.MajorGrid.LineColor = Color.LightGray;
            area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
        }

        private void ConfigurarGraficaLineas(Series serie, List<KeyValuePair<string, int>> datosOrdenados, ChartArea area)
        {
            // Configurar serie
            serie.IsValueShownAsLabel = true;
            serie.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            serie.MarkerStyle = MarkerStyle.Circle;
            serie.MarkerSize = 10;
            serie.BorderWidth = 3;

            // Agregar puntos individuales
            for (int i = 0; i < datosOrdenados.Count; i++)
            {
                var item = datosOrdenados[i];
                DataPoint punto = new DataPoint();
                punto.SetValueXY(i, item.Value);
                punto.AxisLabel = item.Key.Length > 15 ? item.Key.Substring(0, 12) + "..." : item.Key;
                punto.Label = item.Value.ToString("N0");
                punto.ToolTip = $"{item.Key}\nFrecuencia: {item.Value:N0}";
                
                serie.Points.Add(punto);
            }

            // Configurar área del gráfico
            area.AxisX.Title = _nombreColumna;
            area.AxisX.TitleFont = new Font("Segoe UI", 10, FontStyle.Bold);
            area.AxisY.Title = "Frecuencia";
            area.AxisY.TitleFont = new Font("Segoe UI", 10, FontStyle.Bold);
            
            // Configurar eje X
            area.AxisX.Interval = 1;
            area.AxisX.IntervalType = DateTimeIntervalType.Number;
            area.AxisX.LabelStyle.Angle = -45;
            area.AxisX.LabelStyle.Font = new Font("Segoe UI", 8);
            area.AxisX.MajorGrid.LineColor = Color.LightGray;
            area.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            area.AxisX.IsLabelAutoFit = true;
            
            // Configurar eje Y
            area.AxisY.LabelStyle.Format = "N0";
            area.AxisY.LabelStyle.Font = new Font("Segoe UI", 8);
            area.AxisY.MajorGrid.LineColor = Color.LightGray;
            area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
        }

        private void AsignarColores(Series serie)
        {
            Color[] colores = new Color[]
            {
                Color.FromArgb(52, 152, 219),   // Azul
                Color.FromArgb(46, 204, 113),   // Verde
                Color.FromArgb(230, 126, 34),   // Naranja
                Color.FromArgb(231, 76, 60),    // Rojo
                Color.FromArgb(155, 89, 182),   // Púrpura
                Color.FromArgb(241, 196, 15),   // Amarillo
                Color.FromArgb(26, 188, 156),   // Turquesa
                Color.FromArgb(149, 165, 166),  // Gris
                Color.FromArgb(243, 156, 18),   // Naranja oscuro
                Color.FromArgb(192, 57, 43),    // Rojo oscuro
                Color.FromArgb(127, 140, 141),  // Gris azulado
                Color.FromArgb(211, 84, 0),     // Naranja quemado
                Color.FromArgb(41, 128, 185),   // Azul océano
                Color.FromArgb(39, 174, 96),    // Verde esmeralda
                Color.FromArgb(142, 68, 173)    // Púrpura oscuro
            };

            for (int i = 0; i < serie.Points.Count; i++)
            {
                serie.Points[i].Color = colores[i % colores.Length];
                
                // Para gráfica de líneas, usar el mismo color para el marcador
                if (_tipoGrafica == SeriesChartType.Line)
                {
                    serie.Points[i].MarkerColor = colores[i % colores.Length];
                    serie.Color = colores[0]; // Color de la línea principal
                }
            }
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg|BMP Image|*.bmp";
                    saveDialog.Title = "Guardar gráfica como imagen";
                    saveDialog.FileName = $"Grafica_{_nombreColumna}_{DateTime.Now:yyyyMMdd_HHmmss}";

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        chartGrafica.SaveImage(saveDialog.FileName, ChartImageFormat.Png);
                        MessageBox.Show("Gráfica exportada exitosamente.", 
                            "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar la gráfica: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}