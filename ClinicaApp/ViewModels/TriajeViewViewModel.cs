using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ClinicaApp.Models;
using ClinicaApp.Services;
using static ClinicaApp.Models.CitaDetalle;

namespace ClinicaApp.ViewModels
{
    [QueryProperty(nameof(CitaId), "CitaId")]
    public class TriajeViewViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private int _citaId;
        private TriajeCompletadoResponse _triaje;
        private bool _isLoading;
        private string _message;

        public TriajeViewViewModel()
        {
            _apiService = new ApiService();
            BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
            Respuestas = new ObservableCollection<RespuestaTriajeCompleta>();
        }

        public int CitaId
        {
            get => _citaId;
            set
            {
                _citaId = value;
                OnPropertyChanged();
                LoadTriajeAsync();
            }
        }

        public TriajeCompletadoResponse Triaje
        {
            get => _triaje;
            set
            {
                _triaje = value;
                OnPropertyChanged();
                if (value?.Respuestas != null)
                {
                    Respuestas.Clear();
                    foreach (var respuesta in value.Respuestas)
                    {
                        Respuestas.Add(respuesta);
                    }
                }
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

        public ObservableCollection<RespuestaTriajeCompleta> Respuestas { get; }

        public ICommand BackCommand { get; }

        private async Task LoadTriajeAsync()
        {
            try
            {
                IsLoading = true;
                Message = "Cargando triaje del paciente...";

                var response = await _apiService.GetTriajePorCitaAsync(CitaId);

                if (response.Success && response.Data != null)
                {
                    Triaje = response.Data;
                    Message = "Triaje cargado exitosamente";
                }
                else
                {
                    Message = response.Message ?? "Error cargando el triaje";
                    await Shell.Current.DisplayAlert("Error", Message, "OK");
                }
            }
            catch (Exception ex)
            {
                Message = $"Error: {ex.Message}";
                await Shell.Current.DisplayAlert("Error", Message, "OK");
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