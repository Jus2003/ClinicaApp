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

        public TriajeDigitalViewModel()
        {
            _apiService = new ApiService();

            LoadCitasCommand = new Command(async () => await LoadCitasAsync());
            RefreshCommand = new Command(async () => await RefreshCitasAsync());
            CitaSelectedCommand = new Command<CitaDetalle>(async (cita) => await NavigateToCitaDetalleAsync(cita));

            LoadCitasCommand.Execute(null);
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

        // ViewModels/TriajeDigitalViewModel.cs - Corregir el método LoadCitasAsync

        // ViewModels/TriajeDigitalViewModel.cs - Método LoadCitasAsync completo

        private async Task LoadCitasAsync()
        {
            if (!SessionManager.IsLoggedIn)
            {
                Message = "Debe iniciar sesión para ver las citas";
                return;
            }

            IsLoading = true;
            Message = "";

            try
            {
                var pacienteId = GetPacienteId();

                System.Diagnostics.Debug.WriteLine($"ID del paciente: {pacienteId}");
                System.Diagnostics.Debug.WriteLine($"Usuario actual: {SessionManager.CurrentUser?.Nombre}");
                System.Diagnostics.Debug.WriteLine($"=== DEBUG TRIAJE ===");
                System.Diagnostics.Debug.WriteLine($"IsLoggedIn: {SessionManager.IsLoggedIn}");
                System.Diagnostics.Debug.WriteLine($"CurrentUser: {SessionManager.CurrentUser?.Nombre}");
                System.Diagnostics.Debug.WriteLine($"CurrentUser ID: {SessionManager.CurrentUser?.Id}");
                System.Diagnostics.Debug.WriteLine($"CurrentUser Rol: {SessionManager.CurrentUser?.Rol}");
                System.Diagnostics.Debug.WriteLine($"==================");

                if (pacienteId <= 0)
                {
                    Message = "No se pudo obtener el ID del paciente";
                    return;
                }

                var response = await _apiService.GetCitasPacienteAsync(pacienteId);

                System.Diagnostics.Debug.WriteLine($"=== RESPONSE DEBUG ===");
                System.Diagnostics.Debug.WriteLine($"Response Success: {response?.Success}");
                System.Diagnostics.Debug.WriteLine($"Response Message: {response?.Message}");
                System.Diagnostics.Debug.WriteLine($"Response Data is null: {response?.Data == null}");
                System.Diagnostics.Debug.WriteLine($"Response Data Citas is null: {response?.Data?.Citas == null}");
                System.Diagnostics.Debug.WriteLine($"Response Data Citas Count: {response?.Data?.Citas?.Count}");

                if (response.Success && response.Data?.Citas != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Agregando {response.Data.Citas.Count} citas...");

                    Citas.Clear();
                    foreach (var cita in response.Data.Citas)
                    {
                        System.Diagnostics.Debug.WriteLine($"Agregando cita ID: {cita.IdCita} - {cita.NombreEspecialidad}");
                        Citas.Add(cita);
                    }

                    HasCitas = Citas.Count > 0;

                    System.Diagnostics.Debug.WriteLine($"===== FINAL RESULT =====");
                    System.Diagnostics.Debug.WriteLine($"HasCitas: {HasCitas}");
                    System.Diagnostics.Debug.WriteLine($"Total en Collection: {Citas.Count}");
                    if (Citas.Count > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"Citas[0] NombreEspecialidad: {Citas[0]?.NombreEspecialidad}");
                        System.Diagnostics.Debug.WriteLine($"Citas[0] NombreMedico: {Citas[0]?.NombreMedico}");
                        System.Diagnostics.Debug.WriteLine($"Citas[0] FechaCita: {Citas[0]?.FechaCita}");
                    }
                    System.Diagnostics.Debug.WriteLine($"========================");

                    if (!HasCitas)
                    {
                        Message = "No tiene citas registradas";
                    }
                    else
                    {
                        Message = ""; // Limpiar mensaje de error
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Error en respuesta o datos nulos");
                    Message = response?.Message ?? "Error al cargar las citas";
                    HasCitas = false;
                }
            }
            catch (Exception ex)
            {
                Message = $"Error inesperado: {ex.Message}";
                HasCitas = false;
                System.Diagnostics.Debug.WriteLine($"Error LoadCitas: {ex}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        // ✅ CORREGIR estos métodos helper:
        private int GetPacienteId()
        {
            // Usar directamente el ID del usuario actual
            return SessionManager.CurrentUser?.Id ?? 0;
        }

        private async Task RefreshCitasAsync()
        {
            IsRefreshing = true;
            await LoadCitasAsync();
            IsRefreshing = false;
        }

        private async Task NavigateToCitaDetalleAsync(CitaDetalle cita)
        {
            if (cita == null)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: Cita es null");
                return;
            }

            System.Diagnostics.Debug.WriteLine($"=== NAVEGACIÓN ===");
            System.Diagnostics.Debug.WriteLine($"Navegando a cita ID: {cita.IdCita}");
            System.Diagnostics.Debug.WriteLine($"Especialidad: {cita.NombreEspecialidad}");
            System.Diagnostics.Debug.WriteLine($"Médico: {cita.NombreMedico}");

            try
            {
                var parameters = new Dictionary<string, object>
        {
            { "CitaId", cita.IdCita }
        };

                System.Diagnostics.Debug.WriteLine($"Parámetros: CitaId = {cita.IdCita}");

                await Shell.Current.GoToAsync("citadetalle", parameters);

                System.Diagnostics.Debug.WriteLine($"Navegación exitosa");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR en navegación: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}