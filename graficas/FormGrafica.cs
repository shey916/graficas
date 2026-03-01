using System.Windows.Forms.DataVisualization.Charting;

namespace graficas
{
    public partial class FormGrafica : Form
    {
        private const int LIMITE_CARACTERES_ETIQUETA = 15;
        private const int CARACTERES_TRUNCADOS = 12;
        private const string SUFIJO_TRUNCADO = "...";
        private const int RADIO_DONA = 40;
        private const double ANCHO_PUNTO_BARRA = 0.8;
        private const int TAMANO_MARCADOR = 10;
        private const int ANCHO_LINEA = 3;

        private readonly Dictionary<string, int> _frecuenciaPorCategoria;
        private readonly SeriesChartType _tipoGrafica;
        private readonly string _tituloGrafica;
        private readonly string _nombreColumna;
        private readonly int _totalRegistros;

        public FormGrafica(
            Dictionary<string, int> frecuenciaPorCategoria,
            SeriesChartType tipoGrafica,
            string tituloGrafica,
            string nombreColumna,
            int totalRegistros = 0)
        {
            InitializeComponent();
            
            _frecuenciaPorCategoria = frecuenciaPorCategoria ?? throw new ArgumentNullException(nameof(frecuenciaPorCategoria));
            _tipoGrafica = tipoGrafica;
            _tituloGrafica = tituloGrafica ?? throw new ArgumentNullException(nameof(tituloGrafica));
            _nombreColumna = nombreColumna ?? throw new ArgumentNullException(nameof(nombreColumna));
            _totalRegistros = totalRegistros;
            
            ConfigurarGrafica();
        }

        private void ConfigurarGrafica()
        {
            LimpiarGrafica();
            ConfigurarTitulos();
            
            ChartArea area = CrearAreaGrafico();
            Series serie = CrearSerie();
            
            var datosOrdenados = OrdenarDatosPorFrecuencia();
            ConfigurarSerieSegunTipo(serie, datosOrdenados, area);
            
            chartGrafica.Series.Add(serie);
            ConfigurarLeyenda();
            AplicarPaletaColores(serie);
        }

        private void LimpiarGrafica()
        {
            chartGrafica.Titles.Clear();
            chartGrafica.Series.Clear();
            chartGrafica.ChartAreas.Clear();
            chartGrafica.Legends.Clear();
        }

        private void ConfigurarTitulos()
        {
            AgregarTituloPrincipal();
            
            if (HayRegistrosParaMostrar())
            {
                AgregarSubtituloEstadisticas();
            }
        }

        private void AgregarTituloPrincipal()
        {
            Title titulo = new Title(_tituloGrafica)
            {
                Font = new Font("Segoe UI", 14, FontStyle.Bold)
            };
            chartGrafica.Titles.Add(titulo);
        }

        private bool HayRegistrosParaMostrar()
        {
            return _totalRegistros > 0;
        }

        private void AgregarSubtituloEstadisticas()
        {
            int registrosMostrados = CalcularTotalRegistrosMostrados();
            string textoSubtitulo = GenerarTextoEstadisticas(registrosMostrados);
            
            Title subtitulo = new Title(textoSubtitulo)
            {
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray
            };
            
            chartGrafica.Titles.Add(subtitulo);
        }

        private int CalcularTotalRegistrosMostrados()
        {
            return _frecuenciaPorCategoria.Values.Sum();
        }

        private string GenerarTextoEstadisticas(int registrosMostrados)
        {
            int valoresUnicos = _frecuenciaPorCategoria.Count;
            
            return valoresUnicos >= 50
                ? $"Top 50 de {valoresUnicos:N0} valores únicos - {registrosMostrados:N0} de {_totalRegistros:N0} registros"
                : $"{valoresUnicos:N0} valores únicos - {_totalRegistros:N0} registros totales";
        }

        private ChartArea CrearAreaGrafico()
        {
            ChartArea area = new ChartArea("AreaPrincipal")
            {
                BackColor = Color.White
            };
            chartGrafica.ChartAreas.Add(area);
            return area;
        }

        private Series CrearSerie()
        {
            return new Series("Datos")
            {
                ChartType = _tipoGrafica
            };
        }

        private List<KeyValuePair<string, int>> OrdenarDatosPorFrecuencia()
        {
            return _frecuenciaPorCategoria
                .OrderByDescending(par => par.Value)
                .ToList();
        }

        private void ConfigurarSerieSegunTipo(Series serie, List<KeyValuePair<string, int>> datosOrdenados, ChartArea area)
        {
            switch (_tipoGrafica)
            {
                case SeriesChartType.Doughnut:
                    ConfigurarGraficaDona(serie, datosOrdenados);
                    break;
                    
                case SeriesChartType.Column:
                    ConfigurarGraficaBarras(serie, datosOrdenados, area);
                    break;
                    
                case SeriesChartType.Line:
                    ConfigurarGraficaLineas(serie, datosOrdenados, area);
                    break;
            }
        }

        private void ConfigurarGraficaDona(Series serie, List<KeyValuePair<string, int>> datosOrdenados)
        {
            int totalFrecuencias = CalcularTotalRegistrosMostrados();
            
            foreach (var categoria in datosOrdenados)
            {
                DataPoint punto = CrearPuntoGraficaDona(categoria, totalFrecuencias);
                serie.Points.Add(punto);
            }

            AplicarEstiloGraficaDona(serie);
        }

        private DataPoint CrearPuntoGraficaDona(KeyValuePair<string, int> categoria, int totalFrecuencias)
        {
            double porcentaje = CalcularPorcentaje(categoria.Value, totalFrecuencias);
            
            return new DataPoint
            {
                YValues = new double[] { categoria.Value },
                Label = $"{porcentaje:F1}%",
                LegendText = $"{categoria.Key} ({categoria.Value:N0})",
                ToolTip = GenerarTooltipDona(categoria.Key, categoria.Value, porcentaje)
            };
        }

        private double CalcularPorcentaje(int valor, int total)
        {
            return (valor * 100.0) / total;
        }

        private string GenerarTooltipDona(string categoria, int frecuencia, double porcentaje)
        {
            return $"{categoria}\nFrecuencia: {frecuencia:N0}\nPorcentaje: {porcentaje:F2}%";
        }

        private void AplicarEstiloGraficaDona(Series serie)
        {
            serie["PieLabelStyle"] = "Outside";
            serie["DoughnutRadius"] = RADIO_DONA.ToString();
            serie.IsValueShownAsLabel = true;
            serie.Font = new Font("Segoe UI", 9);
        }

        private void ConfigurarGraficaBarras(Series serie, List<KeyValuePair<string, int>> datosOrdenados, ChartArea area)
        {
            AplicarEstiloSerieBarras(serie);
            AgregarPuntosBarras(serie, datosOrdenados);
            ConfigurarEjes(area, esModoLinea: false);
        }

        private void AplicarEstiloSerieBarras(Series serie)
        {
            serie.IsValueShownAsLabel = true;
            serie.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            serie["PointWidth"] = ANCHO_PUNTO_BARRA.ToString();
        }

        private void AgregarPuntosBarras(Series serie, List<KeyValuePair<string, int>> datosOrdenados)
        {
            for (int indice = 0; indice < datosOrdenados.Count; indice++)
            {
                var categoria = datosOrdenados[indice];
                DataPoint punto = CrearPuntoGraficaBarrasOLineas(categoria, indice);
                serie.Points.Add(punto);
            }
        }

        private void ConfigurarGraficaLineas(Series serie, List<KeyValuePair<string, int>> datosOrdenados, ChartArea area)
        {
            AplicarEstiloSerieLineas(serie);
            AgregarPuntosLineas(serie, datosOrdenados);
            ConfigurarEjes(area, esModoLinea: true);
        }

        private void AplicarEstiloSerieLineas(Series serie)
        {
            serie.IsValueShownAsLabel = true;
            serie.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            serie.MarkerStyle = MarkerStyle.Circle;
            serie.MarkerSize = TAMANO_MARCADOR;
            serie.BorderWidth = ANCHO_LINEA;
        }

        private void AgregarPuntosLineas(Series serie, List<KeyValuePair<string, int>> datosOrdenados)
        {
            for (int indice = 0; indice < datosOrdenados.Count; indice++)
            {
                var categoria = datosOrdenados[indice];
                DataPoint punto = CrearPuntoGraficaBarrasOLineas(categoria, indice);
                serie.Points.Add(punto);
            }
        }

        private DataPoint CrearPuntoGraficaBarrasOLineas(KeyValuePair<string, int> categoria, int indice)
        {
            return new DataPoint
            {
                XValue = indice,
                YValues = new double[] { categoria.Value },
                AxisLabel = TruncarTextoSiEsNecesario(categoria.Key),
                Label = categoria.Value.ToString("N0"),
                ToolTip = $"{categoria.Key}\nFrecuencia: {categoria.Value:N0}"
            };
        }

        private string TruncarTextoSiEsNecesario(string texto)
        {
            return texto.Length > LIMITE_CARACTERES_ETIQUETA
                ? texto.Substring(0, CARACTERES_TRUNCADOS) + SUFIJO_TRUNCADO
                : texto;
        }

        private void ConfigurarEjes(ChartArea area, bool esModoLinea)
        {
            ConfigurarEjeX(area, esModoLinea);
            ConfigurarEjeY(area);
        }

        private void ConfigurarEjeX(ChartArea area, bool esModoLinea)
        {
            area.AxisX.Title = _nombreColumna;
            area.AxisX.TitleFont = new Font("Segoe UI", 10, FontStyle.Bold);
            area.AxisX.Interval = 1;
            area.AxisX.IntervalType = DateTimeIntervalType.Number;
            area.AxisX.LabelStyle.Angle = -45;
            area.AxisX.LabelStyle.Font = new Font("Segoe UI", 8);
            area.AxisX.IsLabelAutoFit = true;

            if (esModoLinea)
            {
                area.AxisX.MajorGrid.LineColor = Color.LightGray;
                area.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            }
            else
            {
                area.AxisX.MajorGrid.Enabled = false;
            }
        }

        private void ConfigurarEjeY(ChartArea area)
        {
            area.AxisY.Title = "Frecuencia";
            area.AxisY.TitleFont = new Font("Segoe UI", 10, FontStyle.Bold);
            area.AxisY.LabelStyle.Format = "N0";
            area.AxisY.LabelStyle.Font = new Font("Segoe UI", 8);
            area.AxisY.MajorGrid.LineColor = Color.LightGray;
            area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
        }

        private void ConfigurarLeyenda()
        {
            Legend leyenda = new Legend("LeyendaPrincipal")
            {
                Enabled = EsGraficaDona()
            };

            if (EsGraficaDona())
            {
                leyenda.Docking = Docking.Right;
                leyenda.Font = new Font("Segoe UI", 8);
            }

            chartGrafica.Legends.Add(leyenda);
        }

        private bool EsGraficaDona()
        {
            return _tipoGrafica == SeriesChartType.Doughnut;
        }

        private void AplicarPaletaColores(Series serie)
        {
            Color[] paletaColores = ObtenerPaletaColores();

            for (int i = 0; i < serie.Points.Count; i++)
            {
                Color color = paletaColores[i % paletaColores.Length];
                serie.Points[i].Color = color;

                if (_tipoGrafica == SeriesChartType.Line)
                {
                    serie.Points[i].MarkerColor = color;
                    serie.Color = paletaColores[0];
                }
            }
        }

        private Color[] ObtenerPaletaColores()
        {
            return new[]
            {
                Color.FromArgb(52, 152, 219),   // Azul cielo
                Color.FromArgb(46, 204, 113),   // Verde esmeralda
                Color.FromArgb(230, 126, 34),   // Naranja calabaza
                Color.FromArgb(231, 76, 60),    // Rojo alizarina
                Color.FromArgb(155, 89, 182),   // Amatista
                Color.FromArgb(241, 196, 15),   // Amarillo sol
                Color.FromArgb(26, 188, 156),   // Turquesa
                Color.FromArgb(149, 165, 166),  // Gris concreto
                Color.FromArgb(243, 156, 18),   // Naranja
                Color.FromArgb(192, 57, 43),    // Granada
                Color.FromArgb(127, 140, 141),  // Asbesto
                Color.FromArgb(211, 84, 0),     // Calabaza oscura
                Color.FromArgb(41, 128, 185),   // Azul peter river
                Color.FromArgb(39, 174, 96),    // Nefritis
                Color.FromArgb(142, 68, 173)    // Wisteria
            };
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            try
            {
                ExportarGraficaComoImagen();
            }
            catch (Exception ex)
            {
                MostrarErrorExportacion(ex);
            }
        }

        private void ExportarGraficaComoImagen()
        {
            using (SaveFileDialog dialogoGuardar = CrearDialogoGuardarImagen())
            {
                if (UsuarioConfirmoGuardado(dialogoGuardar))
                {
                    GuardarImagenYNotificar(dialogoGuardar.FileName);
                }
            }
        }

        private SaveFileDialog CrearDialogoGuardarImagen()
        {
            return new SaveFileDialog
            {
                Filter = "PNG Image|*.png|JPEG Image|*.jpg|BMP Image|*.bmp",
                Title = "Guardar gráfica como imagen",
                FileName = GenerarNombreArchivoExportacion()
            };
        }

        private string GenerarNombreArchivoExportacion()
        {
            string nombreColumnaLimpio = LimpiarNombreArchivo(_nombreColumna);
            string marcaTemporal = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            return $"Grafica_{nombreColumnaLimpio}_{marcaTemporal}";
        }

        private string LimpiarNombreArchivo(string nombre)
        {
            char[] caracteresInvalidos = Path.GetInvalidFileNameChars();
            return new string(nombre.Where(c => !caracteresInvalidos.Contains(c)).ToArray());
        }

        private bool UsuarioConfirmoGuardado(SaveFileDialog dialogo)
        {
            return dialogo.ShowDialog() == DialogResult.OK;
        }

        private void GuardarImagenYNotificar(string rutaArchivo)
        {
            chartGrafica.SaveImage(rutaArchivo, ChartImageFormat.Png);
            MostrarExitoExportacion();
        }

        private void MostrarExitoExportacion()
        {
            MessageBox.Show(
                "Gráfica exportada exitosamente.",
                "Éxito",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void MostrarErrorExportacion(Exception ex)
        {
            MessageBox.Show(
                $"Error al exportar la gráfica: {ex.Message}",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}