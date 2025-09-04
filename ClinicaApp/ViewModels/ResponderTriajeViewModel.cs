using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using System.Windows.Input;
using ClinicaApp.Helpers;
using ClinicaApp.Models;
using ClinicaApp.Services;

namespace ClinicaApp.ViewModels
{
    public class ResponderTriajeViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private readonly int _idCita;
        private bool _isLoading;
        private string _message;
        private bool _hasMessage;
        private Color _messageColor;
        private bool _triajeCompletado;
        private InfoCitaTriaje _infoCita;

        public ResponderTriajeViewModel(int idCita)
        {
            _apiService = new ApiService();
            _idCita = idCita;

            EnviarTriajeCommand = new Command(async () => await EnviarTriajeAsync(), () => PuedeEnviarTriaje);
            SeleccionarSiNoCommand = new Command<string>(SeleccionarSiNo);
            VolverCommand = new Command(async () => await Shell.Current.GoToAsync(".."));

            LoadPreguntasAsync();
        }

        public ObservableCollection<PreguntaTriaje> Preguntas { get; set; } = new ObservableCollection<PreguntaTriaje>();

        public ICommand EnviarTriajeCommand { get; }
        public ICommand SeleccionarSiNoCommand { get; }
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

        public bool TriajeCompletado
        {
            get => _triajeCompletado;
            set
            {
                _triajeCompletado = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PuedeEnviar));
            }
        }

        public InfoCitaTriaje InfoCita
        {
            get => _infoCita;
            set
            {
                _infoCita = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasInfoCita));
                OnPropertyChanged(nameof(InfoCitaTexto));
            }
        }

        public bool HasInfoCita => InfoCita != null;
        public bool HasPreguntas => Preguntas?.Count > 0 && !TriajeCompletado;
        public bool PuedeEnviar => HasPreguntas && !TriajeCompletado && !IsLoading;

        public string InfoCitaTexto
        {
            get
            {
                if (InfoCita == null) return "";

                var fecha = DateTime.Parse(InfoCita.FechaCita).ToString("dd/MM/yyyy");
                var hora = TimeSpan.Parse(InfoCita.HoraCita).ToString(@"hh\:mm");

                return $"Cita: {fecha} - {hora}\n" +
                       $"Dr(a). {InfoCita.Medico}\n" +
                       $"{InfoCita.Especialidad}";
            }
        }

        public string ProgresoTexto
        {
            get
            {
                if (!HasPreguntas) return "";

                var respondidas = Preguntas.Count(p => p.TieneRespuesta);
                var obligatorias = Preguntas.Count(p => p.Obligatoria);
                var obligatoriasRespondidas = Preguntas.Count(p => p.Obligatoria && p.TieneRespuesta);

                return $"Respondidas: {respondidas}/{Preguntas.Count} | Obligatorias: {obligatoriasRespondidas}/{obligatorias}";
            }
        }

        public double ProgresoDecimal
        {
            get
            {
                if (!HasPreguntas) return 0;

                var respondidas = Preguntas.Count(p => p.TieneRespuesta);
                return (double)respondidas / Preguntas.Count;
            }
        }

        private bool PuedeEnviarTriaje
        {
            get
            {
                if (!HasPreguntas || IsLoading || TriajeCompletado) return false;

                // Verificar que todas las preguntas obligatorias estén respondidas
                var obligatoriasRespondidas = Preguntas.Where(p => p.Obligatoria).All(p => p.TieneRespuesta);
                return obligatoriasRespondidas;
            }
        }

        private async Task LoadPreguntasAsync()
        {
            try
            {
                IsLoading = true;
                Message = "Cargando preguntas...";

                // Primero verificar si ya tiene triaje
                var estadoResponse = await _apiService.VerificarEstadoTriajeAsync(_idCita);
                if (estadoResponse?.Success == true && estadoResponse.Data != null)
                {
                    var data = estadoResponse.Data.Value;
                    var yaRealizado = data.GetProperty("triaje_realizado").GetBoolean();

                    if (yaRealizado)
                    {
                        TriajeCompletado = true;
                        ShowSuccess("El triaje para esta cita ya fue completado anteriormente");
                        return;
                    }
                    // Cargar info de la cita
                    if (data.TryGetProperty("info_cita", out JsonElement citaInfo)) // 👈 CAMBIAR var por JsonElement
                    {
                        InfoCita = new InfoCitaTriaje
                        {
                            FechaCita = citaInfo.GetProperty("fecha_cita").GetString(),
                            HoraCita = citaInfo.GetProperty("hora_cita").GetString(),
                            EstadoCita = citaInfo.GetProperty("estado_cita").GetString(),
                            Paciente = citaInfo.TryGetProperty("paciente", out var pac) ? pac.GetString() : "",
                            Medico = citaInfo.TryGetProperty("medico", out var med) ? med.GetString() : "",
                            Especialidad = citaInfo.TryGetProperty("especialidad", out var esp) ? esp.GetString() : ""
                        };
                    }
                }

                // Cargar preguntas
                var preguntasResponse = await _apiService.GetPreguntasTriajeAsync();
                if (preguntasResponse?.Success == true && preguntasResponse.Data != null)
                {
                    Preguntas.Clear();

                    foreach (var pregunta in preguntasResponse.Data.OrderBy(p => p.Orden))
                    {
                        // Suscribirse a cambios en las respuestas para actualizar el progreso
                        pregunta.PropertyChanged += Pregunta_PropertyChanged;
                        Preguntas.Add(pregunta);
                    }

                    Message = $"Se cargaron {Preguntas.Count} preguntas del triaje";
                    ShowInfo(Message);
                }
                else
                {
                    ShowError(preguntasResponse?.Message ?? "Error al cargar las preguntas del triaje");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error al cargar preguntas: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Error LoadPreguntas: {ex}");
            }
            finally
            {
                IsLoading = false;
                OnPropertyChanged(nameof(HasPreguntas));
                OnPropertyChanged(nameof(PuedeEnviar));
                ((Command)EnviarTriajeCommand).ChangeCanExecute();
            }
        }

        private void Pregunta_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PreguntaTriaje.Respuesta) ||
                e.PropertyName == nameof(PreguntaTriaje.TieneRespuesta))
            {
                OnPropertyChanged(nameof(ProgresoTexto));
                OnPropertyChanged(nameof(ProgresoDecimal));
                ((Command)EnviarTriajeCommand).ChangeCanExecute();
            }
        }

        private void SeleccionarSiNo(string opcion)
        {
            // Encontrar la pregunta activa de tipo sino
            var preguntaSiNo = Preguntas.FirstOrDefault(p => p.EsSiNo &&
                Application.Current.MainPage.FindByName("SliderEscala") != null);

            // Para simplificar, usamos el parámetro para todas las preguntas sino
            // En una implementación real, necesitarías identificar qué pregunta específica

            // Por ahora, buscaremos la primera pregunta SiNo sin respuesta
            var pregunta = Preguntas.FirstOrDefault(p => p.EsSiNo && string.IsNullOrEmpty(p.Respuesta));
            if (pregunta != null)
            {
                pregunta.Respuesta = opcion;
            }
        }

        private async Task EnviarTriajeAsync()
        {
            try
            {
                IsLoading = true;
                Message = "Enviando triaje...";

                // Validar respuestas obligatorias
                var obligatoriasSinResponder = Preguntas.Where(p => p.Obligatoria && !p.TieneRespuesta).ToList();
                if (obligatoriasSinResponder.Any())
                {
                    var preguntasFaltantes = string.Join(", ", obligatoriasSinResponder.Select(p => p.Pregunta));
                    ShowError($"Faltan respuestas obligatorias: {preguntasFaltantes}");
                    return;
                }

                // Preparar respuestas
                var respuestas = new List<RespuestaTriaje>();
                foreach (var pregunta in Preguntas.Where(p => p.TieneRespuesta))
                {
                    var respuesta = new RespuestaTriaje
                    {
                        IdPregunta = pregunta.IdPregunta,
                        Respuesta = pregunta.Respuesta
                    };

                    // Agregar valor numérico si aplica
                    if (pregunta.EsEscala && pregunta.ValorNumerico.HasValue)
                    {
                        respuesta.ValorNumerico = pregunta.ValorNumerico.Value;
                        respuesta.Respuesta = pregunta.ValorNumerico.Value.ToString("F0");
                    }
                    else if (pregunta.EsNumero && double.TryParse(pregunta.Respuesta, out double valor))
                    {
                        respuesta.ValorNumerico = valor;
                    }

                    respuestas.Add(respuesta);
                }

                // Enviar triaje
                var usuario = SessionManager.CurrentUser;
                var response = await _apiService.EnviarRespuestasTriajeAsync(_idCita, respuestas, usuario?.Id ?? 1);

                if (response?.Success == true)
                {
                    TriajeCompletado = true;
                    ShowSuccess("¡Triaje enviado exitosamente!");

                    // Mostrar resumen
                    await Task.Delay(2000);
                    await Application.Current.MainPage.DisplayAlert(
                        "✅ Triaje Completado",
                        $"Su triaje ha sido enviado correctamente.\n\n" +
                        $"Respuestas enviadas: {respuestas.Count}\n" +
                        $"El médico podrá revisar sus respuestas antes de la cita.",
                        "Entendido");
                }
                else
                {
                    ShowError(response?.Message ?? "Error al enviar el triaje");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error al enviar triaje: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Error EnviarTriaje: {ex}");
            }
            finally
            {
                IsLoading = false;
            }
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