using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ClinicaApp.Models;
using ClinicaApp.Services;
using ClinicaApp.Helpers;

namespace ClinicaApp.ViewModels
{
    public class AttendPatientsViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private bool _isLoading;
        private string _message;
        private bool _isSuccess;

        public AttendPatientsViewModel()
        {
            _apiService = new ApiService();
            Appointments = new ObservableCollection<AppointmentSummary>();

            RefreshCommand = new Command(async () => await LoadAppointmentsAsync());
            ViewDetailsCommand = new Command<AppointmentSummary>(async (appointment) => await ViewAppointmentDetailsAsync(appointment));

            LoadAppointmentsAsync();
        }

        public ObservableCollection<AppointmentSummary> Appointments { get; set; }

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

        public bool IsSuccess
        {
            get => _isSuccess;
            set
            {
                _isSuccess = value;
                OnPropertyChanged();
            }
        }

        public bool HasAppointments => Appointments?.Count > 0;

        public ICommand RefreshCommand { get; }
        public ICommand ViewDetailsCommand { get; }

        private async Task LoadAppointmentsAsync()
        {
            try
            {
                IsLoading = true;
                Message = "";

                var currentUser = SessionManager.CurrentUser;
                if (currentUser == null || currentUser.Rol != "Médico")
                {
                    Message = "Error: Usuario no autorizado";
                    IsSuccess = false;
                    return;
                }

                var response = await _apiService.GetDoctorAppointmentsAsync(currentUser.Id);

                if (response.Success && response.Data?.Citas != null)
                {
                    Appointments.Clear();
                    foreach (var appointment in response.Data.Citas)
                    {
                        Appointments.Add(appointment);
                    }

                    Message = $"Se cargaron {Appointments.Count} citas";
                    IsSuccess = true;
                }
                else
                {
                    Message = response.Message ?? "No se pudieron cargar las citas";
                    IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                Message = $"Error cargando citas: {ex.Message}";
                IsSuccess = false;
            }
            finally
            {
                IsLoading = false;
                OnPropertyChanged(nameof(HasAppointments));
            }
        }

        private async Task ViewAppointmentDetailsAsync(AppointmentSummary appointment)
        {
            try
            {
                if (appointment == null) return;
                await Shell.Current.GoToAsync($"AppointmentDetailPage?AppointmentId={appointment.IdCita}");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al abrir detalles: {ex.Message}", "OK");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}