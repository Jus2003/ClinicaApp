using System.ComponentModel;
using System.Windows.Input;
using ClinicaApp.Models;
using ClinicaApp.Services;

namespace ClinicaApp.ViewModels
{
    [QueryProperty(nameof(AppointmentId), "AppointmentId")]
    public class AppointmentDetailViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private int _appointmentId;
        private AppointmentDetail _appointment;
        private bool _isLoading;
        private string _message;
        private bool _isSuccess;
        private bool _canComplete;
        private bool _addObservations;
        private bool _addPrescription;
        private string _observations;
        private string _medicamento;
        private string _dosis;
        private string _frecuencia;
        private string _duracion;
        private string _cantidad;

        public bool ShowObservationsForm
        {
            get => _addObservations;
            set
            {
                _addObservations = value;
                OnPropertyChanged();
            }
        }

        public AppointmentDetailViewModel()
        {
            _apiService = new ApiService();
            CompleteAppointmentCommand = new Command(async () => await CompleteAppointmentAsync(), () => CanComplete);
            BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        }

        public int AppointmentId
        {
            get => _appointmentId;
            set
            {
                _appointmentId = value;
                OnPropertyChanged();
                LoadAppointmentDetailAsync();
            }
        }

        public AppointmentDetail Appointment
        {
            get => _appointment;
            set
            {
                _appointment = value;
                OnPropertyChanged();
                UpdateCanComplete();
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

        public bool CanComplete
        {
            get => _canComplete;
            set
            {
                _canComplete = value;
                OnPropertyChanged();
                ((Command)CompleteAppointmentCommand).ChangeCanExecute();
            }
        }

        public bool AddObservations
        {
            get => _addObservations;
            set
            {
                _addObservations = value;
                OnPropertyChanged();
            }
        }

        public bool AddPrescription
        {
            get => _addPrescription;
            set
            {
                _addPrescription = value;
                OnPropertyChanged();
            }
        }

        public string Observations
        {
            get => _observations;
            set
            {
                _observations = value;
                OnPropertyChanged();
            }
        }

        // Propiedades para receta
        public string Medicamento
        {
            get => _medicamento;
            set
            {
                _medicamento = value;
                OnPropertyChanged();
            }
        }

        public string Dosis
        {
            get => _dosis;
            set
            {
                _dosis = value;
                OnPropertyChanged();
            }
        }

        public string Frecuencia
        {
            get => _frecuencia;
            set
            {
                _frecuencia = value;
                OnPropertyChanged();
            }
        }

        public string Duracion
        {
            get => _duracion;
            set
            {
                _duracion = value;
                OnPropertyChanged();
            }
        }

        public string Cantidad
        {
            get => _cantidad;
            set
            {
                _cantidad = value;
                OnPropertyChanged();
            }
        }

        public ICommand CompleteAppointmentCommand { get; }
        public ICommand BackCommand { get; }

        private async Task LoadAppointmentDetailAsync()
        {
            try
            {
                IsLoading = true;
                var response = await _apiService.GetAppointmentDetailAsync(AppointmentId);

                if (response.Success && response.Data?.Cita != null)
                {
                    Appointment = response.Data.Cita;
                    IsSuccess = true;
                }
                else
                {
                    Message = response.Message ?? "No se pudieron cargar los detalles";
                    IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                Message = $"Error: {ex.Message}";
                IsSuccess = false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void UpdateCanComplete()
        {
            CanComplete = Appointment != null &&
                         (Appointment.EstadoCita == "confirmada" || Appointment.EstadoCita == "en_curso") &&
                         !IsLoading;
        }

        private async Task CompleteAppointmentAsync()
        {
            try
            {
                IsLoading = true;

                // Cambiar estado de la cita
                var changeStatusRequest = new ChangeAppointmentStatusRequest
                {
                    NuevoEstado = "completada",
                    Observaciones = AddObservations ? Observations : null
                };

                var statusResponse = await _apiService.ChangeAppointmentStatusAsync(AppointmentId, changeStatusRequest);

                if (!statusResponse.Success)
                {
                    Message = $"Error al completar cita: {statusResponse.Message}";
                    return;
                }

                // Crear receta si se marcó la opción
                if (AddPrescription && !string.IsNullOrEmpty(Medicamento))
                {
                    var prescriptionRequest = new CreatePrescriptionRequest
                    {
                        IdCita = AppointmentId,
                        Medicamento = Medicamento,
                        Dosis = Dosis,
                        Frecuencia = Frecuencia,
                        Duracion = Duracion,
                        Cantidad = Cantidad
                    };

                    await _apiService.CreatePrescriptionAsync(prescriptionRequest);
                }

                await Shell.Current.DisplayAlert("Éxito", "¡Cita completada exitosamente!", "OK");
                await Shell.Current.GoToAsync("..");

            }
            catch (Exception ex)
            {
                Message = $"Error: {ex.Message}";
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