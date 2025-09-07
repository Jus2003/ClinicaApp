// ViewModels/TriajeDigitalViewModel.cs
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ClinicaApp.Models;
using ClinicaApp.Services;
using ClinicaApp.Helpers;

namespace ClinicaApp.ViewModels
{
    public class TriajeDigitalViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private bool _isLoading;
        private bool _isRefreshing;
        private string _message;
        private bool _hasCitas;
        public ICommand SelectOptionCommand { get; }


        public TriajeDigitalViewModel()
        {
            _apiService = new ApiService();

            LoadCitasCommand = new Command(async () => await LoadCitasAsync());
            RefreshCommand = new Command(async () => await RefreshCitasAsync());
            CitaSelectedCommand = new Command<CitaDetalle>(async (cita) => await NavigateToCitaDetalleAsync(cita));
            SelectOptionCommand = new Command<string>((option) => SelectOption(option));


            LoadCitasCommand.Execute(null);
        }

        private void SelectOption(string option)
        {
            if (CurrentQuestion != null && !IsReadOnly)
            {
                CurrentQuestion.Respuesta = option;
                OnPropertyChanged(nameof(CurrentQuestion));
                UpdateNavigationButtons();
            }
        }

        public ObservableCollection<CitaDetalle> Citas { get; set; } = new ObservableCollection<CitaDetalle>();

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
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

        public bool HasCitas
        {
            get => _hasCitas;
            set
            {
                _hasCitas = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadCitasCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand CitaSelectedCommand { get; }

        private async Task LoadCitasAsync()
        {
            if (!UserSession.IsPaciente())
            {
                Message = "Debe iniciar sesión como paciente para ver las citas";
                return;
            }

            IsLoading = true;
            Message = "";

            try
            {
                var pacienteId = UserSession.GetPacienteId();
                var response = await _apiService.GetCitasPacienteAsync(pacienteId);

                if (response.Success && response.Data?.Citas != null)
                {
                    Citas.Clear();
                    foreach (var cita in response.Data.Citas)
                    {
                        Citas.Add(cita);
                    }

                    HasCitas = Citas.Count > 0;

                    if (!HasCitas)
                    {
                        Message = "No tiene citas registradas";
                    }
                }
                else
                {
                    Message = response.Message ?? "Error al cargar las citas";
                    HasCitas = false;
                }
            }
            catch (Exception ex)
            {
                Message = $"Error inesperado: {ex.Message}";
                HasCitas = false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task RefreshCitasAsync()
        {
            IsRefreshing = true;
            await LoadCitasAsync();
            IsRefreshing = false;
        }

        private async Task NavigateToCitaDetalleAsync(CitaDetalle cita)
        {
            if (cita == null) return;

            var parameters = new Dictionary<string, object>
            {
                { "CitaId", cita.IdCita }
            };

            await Shell.Current.GoToAsync("citadetalle", parameters);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}