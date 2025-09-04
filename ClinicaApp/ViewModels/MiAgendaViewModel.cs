using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ClinicaApp.Models;
using ClinicaApp.Services;
using ClinicaApp.Helpers;

namespace ClinicaApp.ViewModels
{
    public class MiAgendaViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private bool _isLoading;
        private string _message;
        private bool _hasMessage;
        private Color _messageColor;
        private string _estadoIcon;

        public MiAgendaViewModel()
        {
            _apiService = new ApiService();

            ConfirmarCitaCommand = new Command<CitaAgenda>(async (cita) => await ConfirmarCitaAsync(cita));
            CancelarCitaCommand = new Command<CitaAgenda>(async (cita) => await CancelarCitaAsync(cita));
            ResponderTriajeCommand = new Command<CitaAgenda>(async (cita) => await ResponderTriajeAsync(cita));
            VerTriajeCommand = new Command<CitaAgenda>(async (cita) => await VerTriajeAsync(cita));
            RefreshCommand = new Command(async () => await LoadCitasAsync());

            LoadCitasAsync();
        }

        public ObservableCollection<CitaAgenda> Citas { get; set; } = new ObservableCollection<CitaAgenda>();

        public ICommand ConfirmarCitaCommand { get; }
        public ICommand CancelarCitaCommand { get; }
        public ICommand ResponderTriajeCommand { get; }
        public ICommand VerTriajeCommand { get; }
        public ICommand RefreshCommand { get; }

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
                HasMessage = !string.IsNullOrEmpty(value);
                OnPropertyChanged();
            }
        }

        public bool HasMessage
        {
            get => _hasMessage;
            set
            {
                _hasMessage = value;
                OnPropertyChanged();
            }
        }

        public Color MessageColor
        {
            get => _messageColor;
            set
            {
                _messageColor = value;
                OnPropertyChanged();
            }
        }

        public bool HasCitas => Citas?.Count > 0;
        public bool SinCitas => !HasCitas && !IsLoading;

        public string TituloAgenda
        {
            get
            {
                var usuario = SessionManager.CurrentUser;
                if (usuario?.IdRol == 3) // Médico
                    return "Mi Agenda - Médico";
                else if (usuario?.IdRol == 4) // Paciente
                    return "Mis Citas - Paciente";
                else
                    return "Mi Agenda";
            }
        }

        public string InfoUsuario
        {
            get
            {
                var usuario = SessionManager.CurrentUser;
                return usuario != null ? $"{usuario.Nombre} {usuario.Apellido} - {usuario.Rol}" : "";
            }
        }

        private async Task LoadCitasAsync()
        {
            try
            {
                IsLoading = true;
                Message = "";
                Citas.Clear();

                var usuario = SessionManager.CurrentUser;
                if (usuario == null)
                {
                    ShowError("No hay sesión activa");
                    return;
                }

                ApiResponse<dynamic> response = null;

                // Cargar citas según el rol del usuario
                if (usuario.IdRol == 3) // Médico
                {
                    response = await _apiService.GetCitasPorMedicoAsync(usuario.Id);
                }
                else if (usuario.IdRol == 4) // Paciente
                {
                    response = await _apiService.GetCitasPorPacienteAsync(usuario.Id);
                }
                else
                {
                    ShowError("Rol no autorizado para ver agenda");
                    return;
                }

                if (response?.Success == true && response.Data != null)
                {
                    var citasData = response.Data.GetProperty("citas");

                    foreach (var citaElement in citasData.EnumerateArray())
                    {
                        var cita = ParseCitaFromJson(citaElement, usuario.IdRol == 3);
                        Citas.Add(cita);
                    }

                    if (Citas.Count == 0)
                    {
                        ShowInfo("No tienes citas programadas");
                    }
                    else
                    {
                        ShowSuccess($"Se cargaron {Citas.Count} cita(s)");
                    }
                }
                else
                {
                    ShowError(response?.Message ?? "Error al cargar las citas");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error al cargar citas: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Error LoadCitas: {ex}");
            }
            finally
            {
                IsLoading = false;
                OnPropertyChanged(nameof(HasCitas));
                OnPropertyChanged(nameof(SinCitas));
            }
        }

        private CitaAgenda ParseCitaFromJson(System.Text.Json.JsonElement citaJson, bool esMedico)
        {
            return new CitaAgenda
            {
                IdCita = citaJson.GetProperty("id_cita").GetInt32(),
                FechaCita = citaJson.GetProperty("fecha_cita").GetString(),
                HoraCita = citaJson.GetProperty("hora_cita").GetString(),
                TipoCita = citaJson.GetProperty("tipo_cita").GetString(),
                EstadoCita = citaJson.GetProperty("estado_cita").GetString(),
                MotivoConsulta = citaJson.TryGetProperty("motivo_consulta", out var motivo) ? motivo.GetString() : "",
                Observaciones = citaJson.TryGetProperty("observaciones", out var obs) ? obs.GetString() : "",

                // Datos específicos según el rol
                NombrePaciente = citaJson.TryGetProperty("nombre_paciente", out var nomPac) ? nomPac.GetString() : "",
                CedulaPaciente = citaJson.TryGetProperty("cedula_paciente", out var cedPac) ? cedPac.GetString() : "",
                TelefonoPaciente = citaJson.TryGetProperty("telefono_paciente", out var telPac) ? telPac.GetString() : "",
                EmailPaciente = citaJson.TryGetProperty("email_paciente", out var emailPac) ? emailPac.GetString() : "",

                NombreMedico = citaJson.TryGetProperty("nombre_medico", out var nomMed) ? nomMed.GetString() : "",
                CedulaMedico = citaJson.TryGetProperty("cedula_medico", out var cedMed) ? cedMed.GetString() : "",
                TelefonoMedico = citaJson.TryGetProperty("telefono_medico", out var telMed) ? telMed.GetString() : "",
                EmailMedico = citaJson.TryGetProperty("email_medico", out var emailMed) ? emailMed.GetString() : "",

                NombreEspecialidad = citaJson.GetProperty("nombre_especialidad").GetString(),
                NombreSucursal = citaJson.GetProperty("nombre_sucursal").GetString(),

                EsMedico = esMedico
            };
        }

        private async Task ConfirmarCitaAsync(CitaAgenda cita)
        {
            if (cita == null) return;

            try
            {
                IsLoading = true;

                var response = await _apiService.CambiarEstadoCitaAsync(cita.IdCita, "confirmada");

                if (response?.Success == true)
                {
                    cita.EstadoCita = "confirmada";
                    ShowSuccess("Cita confirmada exitosamente");
                    await LoadCitasAsync(); // Recargar para actualizar botones
                }
                else
                {
                    ShowError(response?.Message ?? "Error al confirmar la cita");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error al confirmar cita: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task CancelarCitaAsync(CitaAgenda cita)
        {
            if (cita == null) return;

            try
            {
                // Solicitar motivo de cancelación
                string motivo = await Application.Current.MainPage.DisplayPromptAsync(
                    "Cancelar Cita",
                    "Ingrese el motivo de la cancelación:",
                    "Confirmar",
                    "Cancelar",
                    "Motivo de cancelación...",
                    maxLength: 200);

                if (string.IsNullOrWhiteSpace(motivo))
                {
                    ShowError("Debe ingresar un motivo para cancelar la cita");
                    return;
                }

                IsLoading = true;

                var response = await _apiService.CambiarEstadoCitaAsync(cita.IdCita, "cancelada", motivo);

                if (response?.Success == true)
                {
                    cita.EstadoCita = "cancelada";
                    ShowSuccess("Cita cancelada exitosamente");
                    await LoadCitasAsync(); // Recargar para actualizar botones
                }
                else
                {
                    ShowError(response?.Message ?? "Error al cancelar la cita");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error al cancelar cita: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ResponderTriajeAsync(CitaAgenda cita)
        {
            if (cita == null) return;

            try
            {
                // Navegar a la página de triaje del paciente
                await Shell.Current.GoToAsync($"responder-triaje?idCita={cita.IdCita}");
            }
            catch (Exception ex)
            {
                ShowError($"Error al navegar al triaje: {ex.Message}");
            }
        }

        private async Task VerTriajeAsync(CitaAgenda cita)
        {
            if (cita == null) return;

            try
            {
                // Navegar a la página para ver el triaje del médico
                await Shell.Current.GoToAsync($"ver-triaje?idCita={cita.IdCita}");
            }
            catch (Exception ex)
            {
                ShowError($"Error al navegar al triaje: {ex.Message}");
            }
        }

        private void ShowSuccess(string message)
        {
            Message = message;
            MessageColor = Color.FromArgb("#4CAF50");
        }

        private void ShowError(string message)
        {
            Message = message;
            MessageColor = Color.FromArgb("#F44336");
        }

        private void ShowInfo(string message)
        {
            Message = message;
            MessageColor = Color.FromArgb("#2196F3");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}