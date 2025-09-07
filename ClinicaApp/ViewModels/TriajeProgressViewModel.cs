// ViewModels/TriajeProgressViewModel.cs
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Input;
using ClinicaApp.Models;
using ClinicaApp.Services;

namespace ClinicaApp.ViewModels
{
    [QueryProperty(nameof(CitaId), "CitaId")]
    [QueryProperty(nameof(TriajeCompleto), "TriajeCompleto")]
    public class TriajeProgressViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private int _citaId;
        private bool _triajeCompleto;
        private int _currentQuestionIndex;
        private PreguntaTriaje _currentQuestion;
        private bool _isLoading;
        private bool _canGoNext;
        private bool _canGoPrevious;
        private bool _isLastQuestion;
        private string _progressText;
        private double _progressValue;
        private string _message;
        private bool _isReadOnly;
        public ICommand SelectOptionCommand { get; }

        public TriajeProgressViewModel()
        {
            _apiService = new ApiService();

            LoadTriajeCommand = new Command(async () => await LoadTriajeAsync());
            NextQuestionCommand = new Command(async () => await NextQuestionAsync(), () => CanGoNext);
            PreviousQuestionCommand = new Command(async () => await PreviousQuestionAsync(), () => CanGoPrevious);
            FinishTriajeCommand = new Command(async () => await FinishTriajeAsync());
            BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
            SelectOptionCommand = new Command<string>((option) => SelectOption(option));
        }

        public ObservableCollection<PreguntaTriaje> Preguntas { get; set; } = new ObservableCollection<PreguntaTriaje>();

        private void SelectOption(string option)
        {
            // ✅ VALIDAR SI ES READ-ONLY
            if (CurrentQuestion != null && !IsReadOnly)
            {
                System.Diagnostics.Debug.WriteLine($"Opción seleccionada: {option} para pregunta: {CurrentQuestion.Pregunta}");

                CurrentQuestion.Respuesta = option;
                OnPropertyChanged(nameof(CurrentQuestion));
                UpdateNavigationButtons();

                System.Diagnostics.Debug.WriteLine($"Respuesta actualizada: {CurrentQuestion.Respuesta}");
            }
            else if (IsReadOnly)
            {
                System.Diagnostics.Debug.WriteLine($"Modo solo lectura - tap ignorado");
            }
        }


        public int CitaId
        {
            get => _citaId;
            set
            {
                _citaId = value;
                OnPropertyChanged();
            }
        }

        public bool TriajeCompleto
        {
            get => _triajeCompleto;
            set
            {
                _triajeCompleto = value;
                OnPropertyChanged();
                LoadTriajeCommand.Execute(null);
            }
        }

        public int CurrentQuestionIndex
        {
            get => _currentQuestionIndex;
            set
            {
                _currentQuestionIndex = value;
                OnPropertyChanged();
                UpdateCurrentQuestion();
                UpdateProgress();
                UpdateNavigationButtons();
            }
        }

        public PreguntaTriaje CurrentQuestion
        {
            get => _currentQuestion;
            set
            {
                _currentQuestion = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public bool CanGoNext
        {
            get => _canGoNext;
            set
            {
                _canGoNext = value;
                OnPropertyChanged();
                ((Command)NextQuestionCommand).ChangeCanExecute();
            }
        }

        public bool CanGoPrevious
        {
            get => _canGoPrevious;
            set
            {
                _canGoPrevious = value;
                OnPropertyChanged();
                ((Command)PreviousQuestionCommand).ChangeCanExecute();
            }
        }

        public bool IsLastQuestion
        {
            get => _isLastQuestion;
            set
            {
                _isLastQuestion = value;
                OnPropertyChanged();
            }
        }

        public string ProgressText
        {
            get => _progressText;
            set
            {
                _progressText = value;
                OnPropertyChanged();
            }
        }

        public double ProgressValue
        {
            get => _progressValue;
            set
            {
                _progressValue = value;
                OnPropertyChanged();
            }
        }

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public bool IsReadOnly
        {
            get => _isReadOnly;
            set
            {
                _isReadOnly = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadTriajeCommand { get; }
        public ICommand NextQuestionCommand { get; }
        public ICommand PreviousQuestionCommand { get; }
        public ICommand FinishTriajeCommand { get; }
        public ICommand BackCommand { get; }

        private async Task LoadTriajeAsync()
        {
            IsLoading = true;
            Message = "";

            try
            {
                System.Diagnostics.Debug.WriteLine($"LoadTriajeAsync - TriajeCompleto: {TriajeCompleto}");

                if (TriajeCompleto)
                {
                    System.Diagnostics.Debug.WriteLine("Cargando triaje completado...");
                    await LoadTriajeCompletadoAsync();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Cargando preguntas nuevas...");
                    await LoadPreguntasTriajeAsync();
                }
            }
            catch (Exception ex)
            {
                Message = $"Error inesperado: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Error en LoadTriajeAsync: {ex}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        // En TriajeProgressViewModel.cs - Corregir LoadTriajeCompletadoAsync:

        // En TriajeProgressViewModel.cs, simplificar LoadTriajeCompletadoAsync:

        private async Task LoadTriajeCompletadoAsync()
        {
            System.Diagnostics.Debug.WriteLine($"=== CARGANDO TRIAJE COMPLETADO ===");

            try
            {
                var response = await _apiService.ObtenerTriajeCompletadoAsync(CitaId);

                if (response.Success && response.Data != null)
                {
                    var jsonElement = (JsonElement)response.Data;

                    if (jsonElement.TryGetProperty("respuestas", out var respuestasProperty))
                    {
                        System.Diagnostics.Debug.WriteLine($"Deserializando {respuestasProperty.GetArrayLength()} respuestas...");

                        // ✅ USAR TRY-CATCH ESPECÍFICO PARA LA DESERIALIZACIÓN
                        try
                        {
                            var respuestas = JsonSerializer.Deserialize<List<RespuestaTriajeCompleta>>(
                                respuestasProperty.GetRawText(),
                                new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true,
                                    NumberHandling = JsonNumberHandling.AllowReadingFromString // ✅ PERMITIR STRINGS COMO NÚMEROS
                                });

                            System.Diagnostics.Debug.WriteLine($"Respuestas deserializadas exitosamente: {respuestas?.Count}");

                            // Convertir a preguntas
                            Preguntas.Clear();
                            foreach (var respuesta in respuestas.OrderBy(r => r.IdPregunta))
                            {
                                var pregunta = new PreguntaTriaje
                                {
                                    IdPregunta = respuesta.IdPregunta,
                                    Pregunta = respuesta.Pregunta,
                                    TipoPregunta = respuesta.TipoPregunta,
                                    Respuesta = respuesta.Respuesta, // ✅ Verificar que esto se asigne correctamente
                                    Obligatoria = 1,
                                    Orden = respuesta.IdPregunta
                                };

                                // ✅ AGREGAR DEBUG ESPECÍFICO
                                System.Diagnostics.Debug.WriteLine($"=== PREGUNTA {respuesta.IdPregunta} ===");
                                System.Diagnostics.Debug.WriteLine($"Tipo: {respuesta.TipoPregunta}");
                                System.Diagnostics.Debug.WriteLine($"Pregunta: {respuesta.Pregunta}");
                                System.Diagnostics.Debug.WriteLine($"Respuesta original: '{respuesta.Respuesta}'");
                                System.Diagnostics.Debug.WriteLine($"Respuesta asignada: '{pregunta.Respuesta}'");
                                System.Diagnostics.Debug.WriteLine($"========================");

                                if (respuesta.Opciones != null)
                                {
                                    ProcessQuestionOptions(pregunta, respuesta.Opciones);

                                    // ✅ DEBUG PARA OPCIONES MÚLTIPLES
                                    if (pregunta.TipoPregunta == "multiple")
                                    {
                                        System.Diagnostics.Debug.WriteLine($"Opciones múltiples cargadas: {pregunta.OpcionesLista?.Count}");
                                        if (pregunta.OpcionesLista != null)
                                        {
                                            foreach (var opcion in pregunta.OpcionesLista)
                                            {
                                                System.Diagnostics.Debug.WriteLine($"  - {opcion}");
                                            }
                                        }
                                    }
                                }

                                Preguntas.Add(pregunta);
                            }

                            IsReadOnly = true;
                            CurrentQuestionIndex = 0;
                            Message = $"Triaje completado - {Preguntas.Count} respuestas cargadas";

                            System.Diagnostics.Debug.WriteLine($"✅ Triaje completado cargado: {Preguntas.Count} preguntas");
                        }
                        catch (JsonException jsonEx)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error de deserialización JSON: {jsonEx.Message}");
                            Message = "Error al procesar las respuestas del triaje";
                        }
                    }
                    else
                    {
                        Message = "No se encontraron respuestas del triaje";
                    }
                }
                else
                {
                    Message = response?.Message ?? "Error al cargar el triaje completado";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                Message = $"Error inesperado: {ex.Message}";
            }
        }

        private async Task LoadPreguntasTriajeAsync()
        {
            var response = await _apiService.GetPreguntasTriajeAsync();

            if (response.Success && response.Data != null)
            {
                Preguntas.Clear();
                foreach (var pregunta in response.Data.OrderBy(p => p.Orden))
                {
                    ProcessQuestionOptions(pregunta, pregunta.Opciones);
                    Preguntas.Add(pregunta);
                }

                IsReadOnly = false;
                CurrentQuestionIndex = 0;
            }
            else
            {
                Message = response.Message ?? "Error al cargar las preguntas del triaje";
            }
        }

        private void ProcessQuestionOptions(PreguntaTriaje pregunta, object opciones)
        {
            if (opciones == null) return;

            try
            {
                if (pregunta.TipoPregunta == "multiple" && opciones is JsonElement element && element.ValueKind == JsonValueKind.Array)
                {
                    pregunta.OpcionesLista = JsonSerializer.Deserialize<List<string>>(element.GetRawText());
                }
                else if (pregunta.TipoPregunta == "escala")
                {
                    if (opciones is JsonElement escalaElement)
                    {
                        pregunta.OpcionesEscalaObj = JsonSerializer.Deserialize<OpcionesEscala>(
                            escalaElement.GetRawText(),
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                    else
                    {
                        pregunta.OpcionesEscalaObj = JsonSerializer.Deserialize<OpcionesEscala>(
                            JsonSerializer.Serialize(opciones),
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error procesando opciones: {ex.Message}");
            }
        }


        private void UpdateCurrentQuestion()
        {
            if (Preguntas.Count > 0 && CurrentQuestionIndex >= 0 && CurrentQuestionIndex < Preguntas.Count)
            {
                CurrentQuestion = Preguntas[CurrentQuestionIndex];

                // ✅ AGREGAR DEBUG
                System.Diagnostics.Debug.WriteLine($"=== CURRENT QUESTION UPDATE ===");
                System.Diagnostics.Debug.WriteLine($"Index: {CurrentQuestionIndex}");
                System.Diagnostics.Debug.WriteLine($"Pregunta: {CurrentQuestion?.Pregunta}");
                System.Diagnostics.Debug.WriteLine($"Respuesta: '{CurrentQuestion?.Respuesta}'");
                System.Diagnostics.Debug.WriteLine($"Tipo: {CurrentQuestion?.TipoPregunta}");
                System.Diagnostics.Debug.WriteLine($"==============================");

                OnPropertyChanged(); // Notificar que CurrentQuestion cambió
            }
        }

        private void UpdateProgress()
        {
            if (Preguntas.Count > 0)
            {
                ProgressValue = (double)(CurrentQuestionIndex + 1) / Preguntas.Count;
                ProgressText = $"Pregunta {CurrentQuestionIndex + 1} de {Preguntas.Count}";
            }
        }

        private void UpdateNavigationButtons()
        {
            CanGoPrevious = CurrentQuestionIndex > 0;
            IsLastQuestion = CurrentQuestionIndex == Preguntas.Count - 1;

            if (!IsReadOnly)
            {
                CanGoNext = IsQuestionAnswered() && CurrentQuestionIndex < Preguntas.Count - 1;
            }
            else
            {
                CanGoNext = CurrentQuestionIndex < Preguntas.Count - 1;
            }
        }

        private bool IsQuestionAnswered()
        {
            if (CurrentQuestion == null) return false;

            if (CurrentQuestion.EsObligatoria)
            {
                return !string.IsNullOrWhiteSpace(CurrentQuestion.Respuesta);
            }

            return true; // Las preguntas no obligatorias siempre permiten continuar
        }

        private async Task NextQuestionAsync()
        {
            if (CurrentQuestionIndex < Preguntas.Count - 1)
            {
                CurrentQuestionIndex++;
            }
        }

        private async Task PreviousQuestionAsync()
        {
            if (CurrentQuestionIndex > 0)
            {
                CurrentQuestionIndex--;
            }
        }

        private async Task FinishTriajeAsync()
        {
            if (IsReadOnly)
            {
                await Shell.Current.GoToAsync("..");
                return;
            }

            // Validar que todas las preguntas obligatorias estén respondidas
            var preguntasObligatoriasNoRespondidas = Preguntas
                .Where(p => p.EsObligatoria && string.IsNullOrWhiteSpace(p.Respuesta))
                .ToList();

            if (preguntasObligatoriasNoRespondidas.Any())
            {
                var preguntasPendientes = string.Join(", ", preguntasObligatoriasNoRespondidas.Select(p => p.Orden));
                Message = $"Debe responder las preguntas obligatorias: {preguntasPendientes}";
                return;
            }

            // Preparar respuestas para enviar
            var respuestas = new List<RespuestaTriaje>();

            foreach (var pregunta in Preguntas.Where(p => !string.IsNullOrWhiteSpace(p.Respuesta)))
            {
                var respuesta = new RespuestaTriaje
                {
                    IdPregunta = pregunta.IdPregunta,
                    Respuesta = pregunta.Respuesta
                };

                // Si es una pregunta de escala, agregar valor numérico
                if (pregunta.TipoPregunta == "escala" && decimal.TryParse(pregunta.Respuesta, out var valorNumerico))
                {
                    respuesta.ValorNumerico = valorNumerico;
                }

                respuestas.Add(respuesta);
            }

            var request = new TriajeRequest
            {
                IdCita = CitaId,
                TipoTriaje = "digital",
                Respuestas = respuestas
            };

            IsLoading = true;

            try
            {
                var response = await _apiService.ResponderTriajeAsync(request);

                if (response.Success)
                {
                    await Shell.Current.DisplayAlert("Éxito", "Triaje completado exitosamente", "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    Message = response.Message ?? "Error al guardar el triaje";
                }
            }
            catch (Exception ex)
            {
                Message = $"Error inesperado: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}