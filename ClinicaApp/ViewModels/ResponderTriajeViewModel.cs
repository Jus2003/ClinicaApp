// ViewModels/ResponderTriajeViewModel.cs
using System.Collections.ObjectModel;
using System.ComponentModel;
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

                var fecha = DateTime.TryParse(InfoCita.FechaCita, out var fechaParsed)
                    ? fechaParsed.ToString("dd/MM/yyyy")
                    : InfoCita.FechaCita;

                var hora = TimeSpan.TryParse(InfoCita.HoraCita, out var horaParsed)
                    ? horaParsed.ToString(@"hh\:mm")
                    : InfoCita.HoraCita;

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

                var respondidas = Preguntas.Count(p => !string.IsNullOrWhiteSpace(p.Respuesta));
                var obligatorias = Preguntas.Count(p => p.EsObligatoria);
                var obligatoriasRespondidas = Preguntas.Count(p => p.EsObligatoria && !string.IsNullOrWhiteSpace(p.Respuesta));

                return $"Respondidas: {respondidas}/{Preguntas.Count} | Obligatorias: {obligatoriasRespondidas}/{obligatorias}";
            }
        }

        public double ProgresoDecimal
        {
            get
            {
                if (!HasPreguntas) return 0;

                var respondidas = Preguntas.Count(p => !string.IsNullOrWhiteSpace(p.Respuesta));
                return (double)respondidas / Preguntas.Count;
            }
        }

        private bool PuedeEnviarTriaje
        {
            get
            {
                if (!HasPreguntas || IsLoading || TriajeCompletado) return false;

                // Verificar que todas las preguntas obligatorias estén respondidas
                var obligatoriasRespondidas = Preguntas.Where(p => p.EsObligatoria).All(p => !string.IsNullOrWhiteSpace(p.Respuesta));
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
                    var estadoTriaje = estadoResponse.Data;

                    if (estadoTriaje.TriajeRealizado)
                    {
                        TriajeCompletado = true;
                        ShowSuccess("El triaje para esta cita ya fue completado anteriormente");
                        return;
                    }

                    // Cargar info de la cita
                    if (estadoTriaje.InfoCita != null)
                    {
                        InfoCita = estadoTriaje.InfoCita;
                    }
                }

                // Cargar preguntas
                var preguntasResponse = await _apiService.GetPreguntasTriajeAsync();
                if (preguntasResponse?.Success == true && preguntasResponse.Data != null)
                {
                    Preguntas.Clear();

                    foreach (var pregunta in preguntasResponse.Data.OrderBy(p => p.Orden))
                    {
                        // Procesar opciones según el tipo
                        ProcessarOpcionesPregunta(pregunta);

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

        private void ProcessarOpcionesPregunta(PreguntaTriaje pregunta)
        {
            if (pregunta.Opciones == null) return;

            try
            {
                if (pregunta.TipoPregunta == "multiple" && pregunta.Opciones is JsonElement element && element.ValueKind == JsonValueKind.Array)
                {
                    pregunta.OpcionesLista = JsonSerializer.Deserialize<List<string>>(element.GetRawText());
                }
                else if (pregunta.TipoPregunta == "escala")
                {
                    if (pregunta.Opciones is JsonElement escalaElement)
                    {
                        pregunta.OpcionesEscalaObj = JsonSerializer.Deserialize<OpcionesEscala>(
                            escalaElement.GetRawText(),
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error procesando opciones: {ex.Message}");
            }
        }

        private void Pregunta_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PreguntaTriaje.Respuesta))
            {
                OnPropertyChanged(nameof(ProgresoTexto));
                OnPropertyChanged(nameof(ProgresoDecimal));
                ((Command)EnviarTriajeCommand).ChangeCanExecute();
            }
        }

        private void SeleccionarSiNo(string opcion)
        {
            // Buscar la primera pregunta SiNo sin respuesta
            var pregunta = Preguntas.FirstOrDefault(p => p.TipoPregunta == "sino" && string.IsNullOrEmpty(p.Respuesta));
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
                var obligatoriasSinResponder = Preguntas.Where(p => p.EsObligatoria && string.IsNullOrWhiteSpace(p.Respuesta)).ToList();
                if (obligatoriasSinResponder.Any())
                {
                    var preguntasFaltantes = string.Join(", ", obligatoriasSinResponder.Select(p => p.Pregunta));
                    ShowError($"Faltan respuestas obligatorias: {preguntasFaltantes}");
                    return;
                }

                // Preparar respuestas
                var respuestas = new List<RespuestaTriaje>();
                foreach (var pregunta in Preguntas.Where(p => !string.IsNullOrWhiteSpace(p.Respuesta)))
                {
                    var respuesta = new RespuestaTriaje
                    {
                        IdPregunta = pregunta.IdPregunta,
                        Respuesta = pregunta.Respuesta
                    };

                    // Agregar valor numérico si aplica
                    if (pregunta.TipoPregunta == "escala" && decimal.TryParse(pregunta.Respuesta, out decimal valorEscala))
                    {
                        respuesta.ValorNumerico = valorEscala;
                    }
                    else if (pregunta.TipoPregunta == "numero" && decimal.TryParse(pregunta.Respuesta, out decimal valorNumero))
                    {
                        respuesta.ValorNumerico = valorNumero;
                    }

                    respuestas.Add(respuesta);
                }

                // Crear request
                var triajeRequest = new TriajeRequest
                {
                    IdCita = _idCita,
                    TipoTriaje = "digital",
                    Respuestas = respuestas
                };

                // Enviar triaje
                var response = await _apiService.ResponderTriajeAsync(triajeRequest);

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