using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ClinicaApp.Models;
using ClinicaApp.Services;
using ClinicaApp.Helpers;
using ClinicaApp.Views;

namespace ClinicaApp.ViewModels
{
    public class AttendPatientsViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private bool _isLoading;
        private string _message;
        private bool _isSuccess;
        private AppointmentSummary _selectedAppointment;
        private ObservableCollection<AppointmentSummary> _filteredAppointments;
        private string _filterStatus = "Todas";

        public AttendPatientsViewModel()
        {
            _apiService = new ApiService();
            Appointments = new ObservableCollection<AppointmentSummary>();
            FilteredAppointments = new ObservableCollection<AppointmentSummary>();

            RefreshCommand = new Command(async () => await LoadAppointmentsAsync());
            ViewDetailsCommand = new Command<AppointmentSummary>(async (appointment) => await ViewAppointmentDetailsAsync(appointment));
            FilterCommand = new Command<string>(FilterAppointments);

            LoadAppointmentsAsync();
        }

        public ObservableCollection<AppointmentSummary> Appointments { get; set; }

        public ObservableCollection<AppointmentSummary> FilteredAppointments
        {
            get => _filteredAppointments;
            set
            {
                _filteredAppointments = value;
                OnPropertyChanged();
            }
        }

        public List<string> StatusFilters { get; } = new List<string>
        {
            "Todas", "Confirmada", "En Curso", "Agendada", "Completada"
        };

        public string FilterStatus
        {
            get => _filterStatus;
            set
            {
                _filterStatus = value;
                OnPropertyChanged();
                FilterAppointments(value);
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

        public bool IsSuccess
        {
            get => _isSuccess;
            set
            {
                _isSuccess = value;
                OnPropertyChanged();
            }
        }

        public bool HasAppointments => FilteredAppointments?.Count > 0;

        public ICommand RefreshCommand { get; }
        public ICommand ViewDetailsCommand { get; }
        public ICommand FilterCommand { get; }

        private async Task LoadAppointmentsAsync()
        {
            try
            {
                IsLoading = true;
                Message = "";

                // Obtener ID del médico logueado
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

                    FilterAppointments(FilterStatus);
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

        private void FilterAppointments(string status)
        {
            if (Appointments == null) return;

            var filtered = status switch
            {
                "Confirmada" => Appointments.Where(a => a.EstadoCita == "confirmada"),
                "En Curso" => Appointments.Where(a => a.EstadoCita == "en_curso"),
                "Agendada" => Appointments.Where(a => a.EstadoCita == "agendada"),
                "Completada" => Appointments.Where(a => a.EstadoCita == "completada"),
                _ => Appointments
            };

            FilteredAppointments.Clear();
            foreach (var appointment in filtered.OrderBy(a => a.FechaCita).ThenBy(a => a.HoraCita))
            {
                FilteredAppointments.Add(appointment);
            }

            OnPropertyChanged(nameof(HasAppointments));
        }

        private async Task ViewAppointmentDetailsAsync(AppointmentSummary appointment)
        {
            try
            {
                if (appointment == null) return;

                // Navegar a la página de detalles
                await Shell.Current.GoToAsync($"{nameof(AppointmentDetailPage)}?AppointmentId={appointment.IdCita}");
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