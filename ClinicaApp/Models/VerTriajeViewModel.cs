// ViewModels/VerTriajeViewModel.cs
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using ClinicaApp.Models;
using ClinicaApp.Services;
using static ClinicaApp.Models.CitaDetalle;

namespace ClinicaApp.ViewModels
{
    [QueryProperty(nameof(CitaId), "CitaId")]
    public class VerTriajeViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private int _citaId;
        private bool _isLoading;
        private string _message;

        public VerTriajeViewModel()
        {
            _apiService = new ApiService();
        }

        public int CitaId
        {
            get => _citaId;
            set
            {
                _citaId = value;
                OnPropertyChanged();
                LoadTriajeCommand?.Execute(null);
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

        public ObservableCollection<RespuestaTriajeCompleta> Respuestas { get; set; } = new();


        public ICommand LoadTriajeCommand => new Command(async () => await LoadTriajeAsync());

        private async Task LoadTriajeAsync()
        {
            if (CitaId <= 0) return;

            IsLoading = true;
            Message = "";

            try
            {
                var response = await _apiService.GetTriajePorCitaAsync(CitaId);

                if (response.Success && response.Data != null)
                {
                    Respuestas.Clear();
                    foreach (var respuesta in response.Data.Respuestas)
                    {
                        Respuestas.Add(respuesta);
                    }
                }
                else
                {
                    Message = response.Message ?? "Error al cargar el triaje";
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