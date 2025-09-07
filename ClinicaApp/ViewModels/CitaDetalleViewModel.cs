// ViewModels/CitaDetalleViewModel.cs
using System.ComponentModel;
using System.Windows.Input;
using ClinicaApp.Models;
using ClinicaApp.Services;

namespace ClinicaApp.ViewModels
{
    [QueryProperty(nameof(CitaId), "CitaId")]
    public class CitaDetalleViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private int _citaId;
        private CitaCompleta _cita;
        private EstadoTriaje _estadoTriaje;
        private bool _isLoading;
        private string _message;
        private bool _showTriajeButton;
        private string _triajeButtonText;

        public CitaDetalleViewModel()
        {
            _apiService = new ApiService();

            LoadCitaDetalleCommand = new Command(async () => await LoadCitaDetalleAsync());
            CompletarTriajeCommand = new Command(async () => await NavigateToTriajeAsync());
            BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        }

        public int CitaId
        {
            get => _citaId;
            set
            {
                _citaId = value;
                OnPropertyChanged();
                LoadCitaDetalleCommand.Execute(null);
            }
        }

        public CitaCompleta Cita
        {
            get => _cita;
            set
            {
                _cita = value;
                OnPropertyChanged();
            }
        }

        public EstadoTriaje EstadoTriaje
        {
            get => _estadoTriaje;
            set
            {
                _estadoTriaje = value;
                OnPropertyChanged();
                UpdateTriajeButton();
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

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public bool ShowTriajeButton
        {
            get => _showTriajeButton;
            set
            {
                _showTriajeButton = value;
                OnPropertyChanged();
            }
        }

        public string TriajeButtonText
        {
            get => _triajeButtonText;
            set
            {
                _triajeButtonText = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadCitaDetalleCommand { get; }
        public ICommand CompletarTriajeCommand { get; }
        public ICommand BackCommand { get; }

        private async Task LoadCitaDetalleAsync()
        {
            if (CitaId <= 0) return;

            IsLoading = true;
            Message = "";

            try
            {
                // Cargar detalle de la cita
                var citaResponse = await _apiService.GetCitaDetalladaAsync(CitaId);

                if (citaResponse.Success && citaResponse.Data?.Cita != null)
                {
                    Cita = citaResponse.Data.Cita;

                    // Verificar estado del triaje
                    var triajeResponse = await _apiService.VerificarEstadoTriajeAsync(CitaId);

                    if (triajeResponse.Success && triajeResponse.Data != null)
                    {
                        EstadoTriaje = triajeResponse.Data;
                    }
                    else
                    {
                        // Si no hay triaje, crear estado por defecto
                        EstadoTriaje = new EstadoTriaje
                        {
                            TriajeRealizado = false,
                            TriajeCompleto = false,
                            PuedeRealizarTriaje = true
                        };
                    }
                }
                else
                {
                    Message = citaResponse.Message ?? "Error al cargar el detalle de la cita";
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

        private void UpdateTriajeButton()
        {
            if (EstadoTriaje == null)
            {
                ShowTriajeButton = false;
                return;
            }

            ShowTriajeButton = EstadoTriaje.PuedeRealizarTriaje;

            if (EstadoTriaje.TriajeCompleto)
            {
                TriajeButtonText = "Ver Triaje Completado";
            }
            else if (EstadoTriaje.TriajeRealizado)
            {
                TriajeButtonText = "Continuar Triaje";
            }
            else
            {
                TriajeButtonText = "Completar Triaje";
            }
        }

        private async Task NavigateToTriajeAsync()
        {
            var parameters = new Dictionary<string, object>
            {
                { "CitaId", CitaId },
                { "TriajeCompleto", EstadoTriaje?.TriajeCompleto ?? false }
            };

            await Shell.Current.GoToAsync("triajeprogress", parameters);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}