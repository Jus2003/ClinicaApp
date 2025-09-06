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
        private bool _showObservationsForm;
        private bool _showPrescriptionForm;
        private string _medicamento;
        private string _concentracion;
        private string _formaFarmaceutica;
        private string _dosis;
        private string _frecuencia;
        private string _duracion;
        private string _cantidad;
        private string _indicacionesEspeciales;

        public AppointmentDetailViewModel()
        {
            _apiService = new ApiService();
            CompleteAppointmentCommand = new Command(async () => await CompleteAppointmentAsync(), () => CanComplete);
            BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));

            FormasFarmaceuticas = new List<string>
            {
                "Tabletas", "Cápsulas", "Jarabe", "Gotas", "Crema", "Ungüento",
                "Inyección", "Suspensión", "Granulado", "Aerosol"
            };
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
                ((Command)CompleteAppointmentCommand).ChangeCanExecute();
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
                ShowObservationsForm = value;
                OnPropertyChanged();
                ValidateForm();
            }
        }

        public bool AddPrescription
        {
            get => _addPrescription;
            set
            {
                _addPrescription = value;
                ShowPrescriptionForm = value;
                OnPropertyChanged();
                ValidateForm();
            }
        }

        public string Observations
        {
            get => _observations;
            set
            {
                _observations = value;
                OnPropertyChanged();
                ValidateForm();
            }
        }

        public bool ShowObservationsForm
        {
            get => _showObservationsForm;
            set
            {
                _showObservationsForm = value;
                OnPropertyChanged();
            }
        }

        public bool ShowPrescriptionForm
        {
            get => _showPrescriptionForm;
            set
            {
                _showPrescriptionForm = value;
                OnPropertyChanged();
            }
        }

        public string Medicamento
        {
            get => _medicamento;
            set
            {
                _medicamento = value;
                OnPropertyChanged();
                ValidateForm();
            }
        }

        public string Concentracion
        {
            get => _concentracion;
            set
            {
                _concentracion = value;
                OnPropertyChanged();
            }
        }

        public string FormaFarmaceutica
        {
            get => _formaFarmaceutica;
            set
            {
                _formaFarmaceutica = value;
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
                ValidateForm();
            }
        }

        public string Frecuencia
        {
            get => _frecuencia;
            set
            {
                _frecuencia = value;
                OnPropertyChanged();
                ValidateForm();
            }
        }

        public string Duracion
        {
            get => _duracion;
            set
            {
                _duracion = value;
                OnPropertyChanged();
                ValidateForm();
            }
        }

        public string Cantidad
        {
            get => _cantidad;
            set
            {
                _cantidad = value;
                OnPropertyChanged();
                ValidateForm();
            }
        }

        public string IndicacionesEspeciales
        {
            get => _indicacionesEspeciales;
            set
            {
                _indicacionesEspeciales = value;
                OnPropertyChanged();
            }
        }

        public List<string> FormasFarmaceuticas { get; }

        public ICommand CompleteAppointmentCommand { get; }
        public ICommand BackCommand { get; }

        private async Task LoadAppointmentDetailAsync()
        {
            try
            {
                IsLoading = true;
                Message = "";

                var response = await _apiService.GetAppointmentDetailAsync(AppointmentId);

                if (response.Success && response.Data?.Cita != null)
                {
                    Appointment = response.Data.Cita;
                    Message = "Detalles cargados correctamente";
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
                Message = $"Error cargando detalles: {ex.Message}";
                IsSuccess = false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void UpdateCanComplete()
        {
            // Sin restricciones - cualquier cita se puede procesar
            CanComplete = Appointment != null && !IsLoading;
        }

        private void ValidateForm()
        {
            if (AddPrescription)
            {
                CanComplete = !string.IsNullOrWhiteSpace(Medicamento) &&
                             !string.IsNullOrWhiteSpace(Dosis) &&
                             !string.IsNullOrWhiteSpace(Frecuencia) &&
                             !string.IsNullOrWhiteSpace(Duracion) &&
                             !string.IsNullOrWhiteSpace(Cantidad) &&
                             !IsLoading;
            }
            else
            {
                CanComplete = Appointment != null && !IsLoading;
            }
        }

        private async Task CompleteAppointmentAsync()
        {
            try
            {
                IsLoading = true;
                Message = "";

                // Variables para tracking
                bool observacionesGuardadas = false;
                bool recetaCreada = false;

                // 1. Cambiar estado de la cita
                var changeStatusRequest = new ChangeAppointmentStatusRequest
                {
                    NuevoEstado = "completada",
                    Observaciones = AddObservations ? Observations : null
                };

                var statusResponse = await _apiService.ChangeAppointmentStatusAsync(AppointmentId, changeStatusRequest);

                if (!statusResponse.Success)
                {
                    Message = $"Error al completar cita: {statusResponse.Message}";
                    IsSuccess = false;
                    return;
                }

                // Marcar observaciones como guardadas
                if (AddObservations && !string.IsNullOrWhiteSpace(Observations))
                {
                    observacionesGuardadas = true;
                }

                // 2. AQUÍ ESTÁ EL PROBLEMA - Verificar si se debe crear receta
                if (AddPrescription)
                {
                    // ✅ AGREGAR ESTE DEBUG
                    Message = "Creando receta médica...";

                    // ✅ VERIFICAR TODOS LOS CAMPOS REQUERIDOS
                    if (string.IsNullOrWhiteSpace(Medicamento))
                    {
                        Message = "ERROR: Medicamento es requerido para la receta";
                        IsSuccess = false;
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(Dosis))
                    {
                        Message = "ERROR: Dosis es requerida para la receta";
                        IsSuccess = false;
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(Frecuencia))
                    {
                        Message = "ERROR: Frecuencia es requerida para la receta";
                        IsSuccess = false;
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(Duracion))
                    {
                        Message = "ERROR: Duración es requerida para la receta";
                        IsSuccess = false;
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(Cantidad))
                    {
                        Message = "ERROR: Cantidad es requerida para la receta";
                        IsSuccess = false;
                        return;
                    }

                    // ✅ MOSTRAR LOS DATOS QUE SE VAN A ENVIAR
                    var datosReceta = $"Datos de receta:\n" +
                                    $"- ID Cita: {AppointmentId}\n" +
                                    $"- Medicamento: {Medicamento}\n" +
                                    $"- Concentración: {Concentracion}\n" +
                                    $"- Forma: {FormaFarmaceutica}\n" +
                                    $"- Dosis: {Dosis}\n" +
                                    $"- Frecuencia: {Frecuencia}\n" +
                                    $"- Duración: {Duracion}\n" +
                                    $"- Cantidad: {Cantidad}\n" +
                                    $"- Indicaciones: {IndicacionesEspeciales}";

                    // Mostrar en debug o como mensaje temporal
                    await Shell.Current.DisplayAlert("DEBUG - Datos de Receta", datosReceta, "OK");

                    var prescriptionRequest = new CreatePrescriptionRequest
                    {
                        IdCita = AppointmentId,
                        Medicamento = Medicamento,
                        Concentracion = Concentracion ?? "",
                        FormaFarmaceutica = FormaFarmaceutica ?? "",
                        Dosis = Dosis,
                        Frecuencia = Frecuencia,
                        Duracion = Duracion,
                        Cantidad = Cantidad,
                        IndicacionesEspeciales = IndicacionesEspeciales ?? ""
                    };

                    var prescriptionResponse = await _apiService.CreatePrescriptionAsync(prescriptionRequest);

                    if (prescriptionResponse.Success)
                    {
                        recetaCreada = true;
                        Message = "✅ Receta creada exitosamente";
                    }
                    else
                    {
                        // ✅ MOSTRAR EL ERROR ESPECÍFICO
                        var errorDetallado = $"Error creando receta:\n" +
                                           $"Status: {prescriptionResponse.Status}\n" +
                                           $"Message: {prescriptionResponse.Message}";

                        await Shell.Current.DisplayAlert("ERROR - Receta", errorDetallado, "OK");

                        Message = $"Cita completada, pero error al crear receta: {prescriptionResponse.Message}";
                        IsSuccess = false;
                        return;
                    }
                }

                // 3. Mostrar mensaje final
                await Task.Delay(2000);
                await Shell.Current.GoToAsync("..");

            }
            catch (Exception ex)
            {
                // ✅ MOSTRAR ERROR DETALLADO
                var errorCompleto = $"Error en CompleteAppointmentAsync:\n" +
                                   $"Message: {ex.Message}\n" +
                                   $"StackTrace: {ex.StackTrace}";

                await Shell.Current.DisplayAlert("ERROR COMPLETO", errorCompleto, "OK");

                Message = $"Error completando cita: {ex.Message}";
                IsSuccess = false;
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