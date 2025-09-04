using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ClinicaApp.Models;
using ClinicaApp.Services;
using ClinicaApp.Helpers;

namespace ClinicaApp.ViewModels
{
    public class VerTriajeViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private readonly int _idCita;
        private bool _isLoading;
        private string _message;
        private bool _hasMessage;
        private Color _messageColor;
        private TriajeCompleto _triajeData;
        private bool _sinTriaje;

        public VerTriajeViewModel(int idCita)
        {
            _apiService = new ApiService();
            _idCita = idCita;

            RefreshCommand = new Command(async () => await LoadTriajeAsync());
            GenerarReporteCommand = new Command(async () => await GenerarReporteAsync(), () => TieneRespuestas);
            EnviarEmailCommand = new Command(async () => await EnviarEmailAsync(), () => TieneRespuestas);
            VolverCommand = new Command(async () => await Shell.Current.GoToAsync(".."));

            LoadTriajeAsync();
        }

        public ObservableCollection<RespuestaTriajeDetallada> Respuestas { get; set; } = new ObservableCollection<RespuestaTriajeDetallada>();

        public ICommand RefreshCommand { get; }
        public ICommand GenerarReporteCommand { get; }
        public ICommand EnviarEmailCommand { get; }
        public ICommand VolverCommand { get; }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                HasMessage = !string.IsNullOrEmpty(value);
                OnPropertyChanged();
            }
        }

        public bool HasMessage
        {
            get => _hasMessage;
            set
            {
                _hasMessage = value;
                OnPropertyChanged();
            }
        }

        public Color MessageColor
        {
            get => _messageColor;
            set
            {
                _messageColor = value;
                OnPropertyChanged();
            }
        }

        public bool SinTriaje
        {
            get => _sinTriaje;
            set
            {
                _sinTriaje = value;
                OnPropertyChanged();
            }
        }

        public TriajeCompleto TriajeData
        {
            get => _triajeData;
            set
            {
                _triajeData = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasInfoCita));
                OnPropertyChanged(nameof(HasEstadisticas));
                OnPropertyChanged(nameof(TieneRespuestas));
                OnPropertyChanged(nameof(InfoCitaTexto));
                OnPropertyChanged(nameof(EstadoTriaje));
                OnPropertyChanged(nameof(EstadoColor));
                OnPropertyChanged(nameof(TipoTriaje));
                OnPropertyChanged(nameof(FechaTriaje));

                ((Command)GenerarReporteCommand).ChangeCanExecute();
                ((Command)EnviarEmailCommand).ChangeCanExecute();
            }
        }

        public bool HasInfoCita => TriajeData?.InfoCita != null;
        public bool HasEstadisticas => TriajeData != null && TriajeData.TriajeRealizado;
        public bool TieneRespuestas => Respuestas?.Count > 0;

        public string InfoCitaTexto
        {
            get
            {
                if (TriajeData?.InfoCita == null) return "";

                var infoCita = TriajeData.InfoCita;
                var fecha = DateTime.Parse(infoCita.FechaCita).ToString("dd/MM/yyyy");
                var hora = TimeSpan.Parse(infoCita.HoraCita).ToString(@"hh\:mm");

                return $"Cita: {fecha} - {hora}\n" +
                       $"Paciente: {infoCita.Paciente}\n" +
                       $"{infoCita.Especialidad}";
            }
        }

        public string EstadoTriaje
        {
            get
            {
                if (TriajeData == null) return "Sin información";

                if (!TriajeData.TriajeRealizado)
                    return "PENDIENTE";

                return TriajeData.TriajeCompleto ? "COMPLETADO" : "PARCIAL";
            }
        }

        public Color EstadoColor
        {
            get
            {
                return EstadoTriaje switch
                {
                    "COMPLETADO" => Color.FromArgb("#4CAF50"),
                    "PARCIAL" => Color.FromArgb("#FF9800"),
                    "PENDIENTE" => Color.FromArgb("#F44336"),
                    _ => Color.FromArgb("#666666")
                };
            }
        }

        public string TipoTriaje => TriajeData?.Estadisticas?.TipoTriaje?.ToUpper() ?? "DIGITAL";

        public string FechaTriaje
        {
            get
            {
                if (TriajeData?.Estadisticas?.FechaInicio == null) return "No disponible";

                try
                {
                    var fecha = DateTime.Parse(TriajeData.Estadisticas.FechaInicio);
                    return fecha.ToString("dd/MM/yyyy HH:mm");
                }
                catch
                {
                    return TriajeData.Estadisticas.FechaInicio;
                }
            }
        }

        private async Task LoadTriajeAsync()
        {
            try
            {
                IsLoading = true;
                Message = "Cargando triaje...";
                Respuestas.Clear();
                SinTriaje = false;

                var response = await _apiService.GetTriajePorCitaAsync(_idCita);

                if (response?.Success == true && response.Data != null)
                {
                    // Triaje encontrado
                    TriajeData = response.Data;

                    if (TriajeData.Respuestas?.Count > 0)
                    {
                        foreach (var respuesta in TriajeData.Respuestas.OrderBy(r => r.IdPregunta))
                        {
                            Respuestas.Add(respuesta);
                        }

                        var completado = TriajeData.TriajeCompleto ? "completo" : "parcial";
                        ShowSuccess($"Triaje cargado exitosamente ({completado})");
                    }
                    else
                    {
                        ShowInfo("El triaje no tiene respuestas registradas");
                    }
                }
                else if (response?.Data != null)
                {
                    // Triaje no realizado, pero tenemos info de la cita
                    TriajeData = response.Data;
                    SinTriaje = true;
                    ShowInfo("El paciente aún no ha completado el triaje");
                }
                else
                {
                    ShowError(response?.Message ?? "Error al cargar el triaje");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error al cargar triaje: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Error LoadTriaje: {ex}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task GenerarReporteAsync()
        {
            try
            {
                if (!TieneRespuestas) return;

                IsLoading = true;
                Message = "Generando reporte...";

                // Generar reporte en texto
                var reporte = GenerarReporteTexto();

                // Guardar en archivo temporal (simulación)
                var nombreArchivo = $"Triaje_Cita_{_idCita}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

                // En una implementación real, aquí guardarías el archivo o lo mostrarías
                await Application.Current.MainPage.DisplayAlert(
                    "📄 Reporte Generado",
                    $"Reporte del triaje:\n\n{reporte.Substring(0, Math.Min(200, reporte.Length))}...\n\n" +
                    $"Archivo: {nombreArchivo}",
                    "OK");

                ShowSuccess("Reporte generado exitosamente");
            }
            catch (Exception ex)
            {
                ShowError($"Error al generar reporte: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task EnviarEmailAsync()
        {
            try
            {
                if (!TieneRespuestas) return;

                IsLoading = true;
                Message = "Preparando email...";

                var usuario = SessionManager.CurrentUser;
                var reporte = GenerarReporteTexto();

                // Simulación de envío de email
                await Task.Delay(2000);

                await Application.Current.MainPage.DisplayAlert(
                    "📧 Email Preparado",
                    $"Se ha preparado un email con el reporte del triaje.\n\n" +
                    $"Para: {usuario?.Email ?? "doctor@clinica.com"}\n" +
                    $"Asunto: Triaje - Cita #{_idCita}\n" +
                    $"Adjunto: Reporte completo del triaje\n\n" +
                    $"En una implementación real, se enviaría automáticamente.",
                    "OK");

                ShowSuccess("Email preparado exitosamente");
            }
            catch (Exception ex)
            {
                ShowError($"Error al preparar email: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private string GenerarReporteTexto()
        {
            if (TriajeData?.InfoCita == null || !TieneRespuestas)
                return "No hay datos disponibles para el reporte.";

            var sb = new System.Text.StringBuilder();

            // Header
            sb.AppendLine("=================================");
            sb.AppendLine("     REPORTE DE TRIAJE MÉDICO    ");
            sb.AppendLine("=================================");
            sb.AppendLine();

            // Info de la cita
            var infoCita = TriajeData.InfoCita;
            sb.AppendLine("INFORMACIÓN DE LA CITA:");
            sb.AppendLine($"• ID Cita: {_idCita}");
            sb.AppendLine($"• Fecha: {DateTime.Parse(infoCita.FechaCita):dd/MM/yyyy}");
            sb.AppendLine($"• Hora: {TimeSpan.Parse(infoCita.HoraCita):hh\\:mm}");
            sb.AppendLine($"• Paciente: {infoCita.Paciente}");
            sb.AppendLine($"• Médico: {infoCita.Medico}");
            sb.AppendLine($"• Especialidad: {infoCita.Especialidad}");
            sb.AppendLine();

            // Estado del triaje
            sb.AppendLine("ESTADO DEL TRIAJE:");
            sb.AppendLine($"• Estado: {EstadoTriaje}");
            sb.AppendLine($"• Tipo: {TipoTriaje}");
            sb.AppendLine($"• Fecha de realización: {FechaTriaje}");
            sb.AppendLine($"• Total de respuestas: {Respuestas.Count}");
            sb.AppendLine();

            // Respuestas
            sb.AppendLine("RESPUESTAS DEL PACIENTE:");
            sb.AppendLine(new string('-', 50));

            int contador = 1;
            foreach (var respuesta in Respuestas)
            {
                sb.AppendLine($"{contador}. {respuesta.Pregunta}");
                sb.AppendLine($"   Tipo: {respuesta.TipoPregunta}");
                sb.AppendLine($"   Respuesta: {respuesta.RespuestaFormateada}");
                sb.AppendLine($"   Fecha: {DateTime.Parse(respuesta.FechaRespuesta):dd/MM/yyyy HH:mm}");
                sb.AppendLine();
                contador++;
            }

            // Footer
            sb.AppendLine(new string('=', 50));
            sb.AppendLine($"Reporte generado el: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            sb.AppendLine("Sistema de Clínica Médica");

            return sb.ToString();
        }

        private void ShowSuccess(string message)
        {
            Message = message;
            MessageColor = Color.FromArgb("#4CAF50");
        }

        private void ShowError(string message)
        {
            Message = message;
            MessageColor = Color.FromArgb("#F44336");
        }

        private void ShowInfo(string message)
        {
            Message = message;
            MessageColor = Color.FromArgb("#2196F3");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}