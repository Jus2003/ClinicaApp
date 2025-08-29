using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ClinicaApp.Models;
using ClinicaApp.Services;

namespace ClinicaApp.ViewModels
{
    public class CreateAppointmentViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private string _tipoCitaSeleccionada = "presencial";
        private Especialidad _especialidadSeleccionada;
        private Sucursal _sucursalSeleccionada;
        private MedicoPorEspecialidad _medicoSeleccionado;
        private DateTime _fechaSeleccionada = DateTime.Today.AddDays(1);
        private HorarioDisponible _horarioSeleccionado;
        private string _cedulaPaciente;
        private PatientResponse _pacienteEncontrado;
        private string _motivoConsulta;
        private string _observaciones;
        private bool _isLoading;
        private bool _showMedicos;
        private bool _showHorarios;
        private bool _showPacienteInfo;
        private bool _showCreatePatientButton;
        private string _message;
        private bool _isSuccess;

        public CreateAppointmentViewModel()
        {
            _apiService = new ApiService();

            LoadEspecialidadesCommand = new Command(async () => await LoadEspecialidadesAsync());
            LoadMedicosCommand = new Command(async () => await LoadMedicosAsync(), () => EspecialidadSeleccionada != null && SucursalSeleccionada != null);
            LoadHorariosCommand = new Command(async () => await LoadHorariosAsync(), () => MedicoSeleccionado != null);
            SearchPatientCommand = new Command(async () => await SearchPatientAsync(), () => !string.IsNullOrWhiteSpace(CedulaPaciente));
            CreateAppointmentCommand = new Command(async () => await CreateAppointmentAsync(), () => CanCreateAppointment());
            CreatePatientCommand = new Command(async () => await NavigateToCreatePatientAsync());
            SelectHorarioCommand = new Command<HorarioDisponible>(SelectHorario); // NUEVO COMANDO
            SelectMedicoCommand = new Command<MedicoPorEspecialidad>(SelectMedico);

            InitializeData();
        }

        public ICommand SelectMedicoCommand { get; }


        private void SelectMedico(MedicoPorEspecialidad medico)
        {
            MedicoSeleccionado = medico;
            System.Diagnostics.Debug.WriteLine($"Médico seleccionado: {medico?.NombreCompleto}");
            ResetHorariosSelection();
            ((Command)LoadHorariosCommand).ChangeCanExecute();
        }

        // Agregar estas propiedades
        public ICommand SelectHorarioCommand { get; }

        // Agregar este método
        private void SelectHorario(HorarioDisponible horario)
        {
            HorarioSeleccionado = horario;
            System.Diagnostics.Debug.WriteLine($"Horario seleccionado: {horario?.Hora}");
            ((Command)CreateAppointmentCommand).ChangeCanExecute();
        }

        public ObservableCollection<string> TiposCita { get; } = new ObservableCollection<string> { "presencial", "virtual" };
        public ObservableCollection<Especialidad> Especialidades { get; set; } = new ObservableCollection<Especialidad>();
        public ObservableCollection<Sucursal> Sucursales { get; set; } = new ObservableCollection<Sucursal>();
        public ObservableCollection<MedicoPorEspecialidad> Medicos { get; set; } = new ObservableCollection<MedicoPorEspecialidad>();
        public ObservableCollection<HorarioDisponible> HorariosDisponibles { get; set; } = new ObservableCollection<HorarioDisponible>();

        public string TipoCitaSeleccionada
        {
            get => _tipoCitaSeleccionada;
            set
            {
                _tipoCitaSeleccionada = value;
                OnPropertyChanged();
                LoadEspecialidadesCommand.Execute(null);
                ResetSelection();
            }
        }

        public Especialidad EspecialidadSeleccionada
        {
            get => _especialidadSeleccionada;
            set
            {
                _especialidadSeleccionada = value;
                OnPropertyChanged();
                ((Command)LoadMedicosCommand).ChangeCanExecute();
                ResetMedicosSelection();
            }
        }

        public Sucursal SucursalSeleccionada
        {
            get => _sucursalSeleccionada;
            set
            {
                _sucursalSeleccionada = value;
                OnPropertyChanged();
                ((Command)LoadMedicosCommand).ChangeCanExecute();
                ResetMedicosSelection();
            }
        }

        public MedicoPorEspecialidad MedicoSeleccionado
        {
            get => _medicoSeleccionado;
            set
            {
                _medicoSeleccionado = value;
                OnPropertyChanged();
                ((Command)LoadHorariosCommand).ChangeCanExecute();
                ResetHorariosSelection();
            }
        }

        public DateTime FechaSeleccionada
        {
            get => _fechaSeleccionada;
            set
            {
                _fechaSeleccionada = value;
                OnPropertyChanged();
                if (MedicoSeleccionado != null)
                {
                    LoadHorariosCommand.Execute(null);
                }
            }
        }

        public HorarioDisponible HorarioSeleccionado
        {
            get => _horarioSeleccionado;
            set
            {
                _horarioSeleccionado = value;
                OnPropertyChanged();
                ((Command)CreateAppointmentCommand).ChangeCanExecute();
            }
        }

        public string CedulaPaciente
        {
            get => _cedulaPaciente;
            set
            {
                _cedulaPaciente = value;
                OnPropertyChanged();
                ((Command)SearchPatientCommand).ChangeCanExecute();
                ResetPacienteInfo();
            }
        }

        public PatientResponse PacienteEncontrado
        {
            get => _pacienteEncontrado;
            set
            {
                _pacienteEncontrado = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PacienteInfo));
                ShowPacienteInfo = value != null;
                ShowCreatePatientButton = value == null && !string.IsNullOrWhiteSpace(CedulaPaciente);
            }
        }

        public string PacienteInfo => PacienteEncontrado != null
            ? $"{PacienteEncontrado.NombreCompleto}\nEmail: {PacienteEncontrado.Email}"
            : "";

        public string MotivoConsulta
        {
            get => _motivoConsulta;
            set
            {
                _motivoConsulta = value;
                OnPropertyChanged();
                ((Command)CreateAppointmentCommand).ChangeCanExecute();
            }
        }

        public string Observaciones
        {
            get => _observaciones;
            set
            {
                _observaciones = value;
                OnPropertyChanged();
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

        public bool ShowMedicos
        {
            get => _showMedicos;
            set
            {
                _showMedicos = value;
                OnPropertyChanged();
            }
        }

        public bool ShowHorarios
        {
            get => _showHorarios;
            set
            {
                _showHorarios = value;
                OnPropertyChanged();
            }
        }

        public bool ShowPacienteInfo
        {
            get => _showPacienteInfo;
            set
            {
                _showPacienteInfo = value;
                OnPropertyChanged();
            }
        }

        public bool ShowCreatePatientButton
        {
            get => _showCreatePatientButton;
            set
            {
                _showCreatePatientButton = value;
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

        public ICommand LoadEspecialidadesCommand { get; }
        public ICommand LoadMedicosCommand { get; }
        public ICommand LoadHorariosCommand { get; }
        public ICommand SearchPatientCommand { get; }
        public ICommand CreateAppointmentCommand { get; }
        public ICommand CreatePatientCommand { get; }

        private async void InitializeData()
        {
            try
            {
                IsLoading = true;

                // Cargar sucursales
                var sucursalesResponse = await _apiService.GetSucursalesAsync();
                if (sucursalesResponse.Success && sucursalesResponse.Data != null)
                {
                    Sucursales.Clear();
                    foreach (var sucursal in sucursalesResponse.Data)
                    {
                        Sucursales.Add(sucursal);
                    }
                    SucursalSeleccionada = Sucursales.FirstOrDefault();
                }

                // Cargar especialidades iniciales
                await LoadEspecialidadesAsync();
            }
            catch (Exception ex)
            {
                Message = $"Error inicializando: {ex.Message}";
                IsSuccess = false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadEspecialidadesAsync()
        {
            try
            {
                var response = await _apiService.GetEspecialidadesPorTipoAsync(TipoCitaSeleccionada);
                if (response.Success && response.Data != null)
                {
                    Especialidades.Clear();
                    foreach (var especialidad in response.Data)
                    {
                        Especialidades.Add(especialidad);
                    }
                }
            }
            catch (Exception ex)
            {
                Message = $"Error cargando especialidades: {ex.Message}";
                IsSuccess = false;
            }
        }

        private async Task LoadMedicosAsync()
        {
            if (EspecialidadSeleccionada == null || SucursalSeleccionada == null) return;

            try
            {
                IsLoading = true;
                ShowMedicos = false;

                var response = await _apiService.GetMedicosPorEspecialidadAsync(
                    EspecialidadSeleccionada.IdEspecialidad,
                    TipoCitaSeleccionada);

                if (response.Success && response.Data?.Medicos != null)
                {
                    Medicos.Clear();
                    foreach (var medico in response.Data.Medicos)
                    {
                        Medicos.Add(medico);
                    }
                    ShowMedicos = Medicos.Count > 0;
                    Message = $"Se encontraron {Medicos.Count} médicos disponibles";
                    IsSuccess = true;
                }
                else
                {
                    Message = "No se encontraron médicos para esta especialidad";
                    IsSuccess = false;
                    ShowMedicos = false;
                }
            }
            catch (Exception ex)
            {
                Message = $"Error cargando médicos: {ex.Message}";
                IsSuccess = false;
                ShowMedicos = false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadHorariosAsync()
        {
            if (MedicoSeleccionado == null) return;

            try
            {
                IsLoading = true;
                ShowHorarios = false;

                var fechaStr = FechaSeleccionada.ToString("yyyy-MM-dd");
                var response = await _apiService.GetHorariosDisponiblesAsync(MedicoSeleccionado.IdMedico, fechaStr);

                if (response.Success && response.Data?.HorariosDisponibles != null)
                {
                    HorariosDisponibles.Clear();
                    foreach (var horario in response.Data.HorariosDisponibles.Where(h => h.Disponible))
                    {
                        HorariosDisponibles.Add(horario);
                    }
                    ShowHorarios = HorariosDisponibles.Count > 0;
                    Message = $"Se encontraron {HorariosDisponibles.Count} horarios disponibles";
                    IsSuccess = true;
                }
                else
                {
                    Message = "No hay horarios disponibles para esta fecha";
                    IsSuccess = false;
                    ShowHorarios = false;
                }
            }
            catch (Exception ex)
            {
                Message = $"Error cargando horarios: {ex.Message}";
                IsSuccess = false;
                ShowHorarios = false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task SearchPatientAsync()
        {
            if (string.IsNullOrWhiteSpace(CedulaPaciente)) return;

            try
            {
                IsLoading = true;

                var response = await _apiService.BuscarPacientePorCedulaAsync(CedulaPaciente);
                if (response.Success && response.Data != null)
                {
                    PacienteEncontrado = response.Data;
                    Message = "Paciente encontrado";
                    IsSuccess = true;
                }
                else
                {
                    PacienteEncontrado = null;
                    Message = "Paciente no encontrado en el sistema";
                    IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                Message = $"Error buscando paciente: {ex.Message}";
                IsSuccess = false;
                PacienteEncontrado = null;
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task CreateAppointmentAsync()
        {
            if (!CanCreateAppointment()) return;

            try
            {
                IsLoading = true;

                // Validar disponibilidad antes de crear
                var validacion = await _apiService.ValidarDisponibilidadAsync(
                    MedicoSeleccionado.IdMedico,
                    FechaSeleccionada.ToString("yyyy-MM-dd"),
                    HorarioSeleccionado.Hora);

                if (!validacion.Success || !validacion.Data.Disponible)
                {
                    Message = "El horario seleccionado ya no está disponible";
                    IsSuccess = false;
                    return;
                }

                // Crear la cita
                var citaRequest = new CitaRequest
                {
                    CedulaPaciente = CedulaPaciente,
                    IdMedico = MedicoSeleccionado.IdMedico,
                    IdEspecialidad = EspecialidadSeleccionada.IdEspecialidad,
                    IdSucursal = SucursalSeleccionada.IdSucursal,
                    FechaCita = FechaSeleccionada.ToString("yyyy-MM-dd"),
                    HoraCita = HorarioSeleccionado.Hora,
                    TipoCita = TipoCitaSeleccionada,
                    MotivoConsulta = MotivoConsulta,
                    Observaciones = Observaciones ?? ""
                };

                var response = await _apiService.CrearCitaAsync(citaRequest);

                if (response.Success && response.Data != null)
                {
                    var cita = response.Data;
                    var mensajeExito = $"¡Cita creada exitosamente!\n\n" +
                                     $"Número: {cita.NumeroCita}\n" +
                                     $"Paciente: {cita.Paciente.NombreCompleto}\n" +
                                     $"Médico: {cita.Medico.NombreCompleto}\n" +
                                     $"Fecha: {cita.DetallesCita.Fecha} a las {cita.DetallesCita.Hora}\n" +
                                     $"Tipo: {cita.DetallesCita.Tipo}\n\n";

                    if (cita.InformacionVirtual != null)
                    {
                        mensajeExito += $"Información Virtual:\n" +
                                      $"Enlace: {cita.InformacionVirtual.EnlaceZoom}\n" +
                                      $"Password: {cita.InformacionVirtual.Password}\n\n";
                    }

                    mensajeExito += $"Notificaciones: {cita.Notificaciones.Mensaje}";

                    await Shell.Current.DisplayAlert("Cita Creada", mensajeExito, "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    Message = response.Message ?? "Error al crear la cita";
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

        private async Task NavigateToCreatePatientAsync()
        {
            // Pasar la cédula como parámetro para pre-llenar el formulario
            await Shell.Current.GoToAsync($"createpatient?cedula={CedulaPaciente}");
        }

        private bool CanCreateAppointment()
        {
            var canCreate = EspecialidadSeleccionada != null &&
                            SucursalSeleccionada != null &&
                            MedicoSeleccionado != null &&
                            HorarioSeleccionado != null &&
                            PacienteEncontrado != null &&
                            !string.IsNullOrWhiteSpace(MotivoConsulta);

            System.Diagnostics.Debug.WriteLine($"CanCreateAppointment: {canCreate}");
            System.Diagnostics.Debug.WriteLine($"- Especialidad: {EspecialidadSeleccionada?.NombreEspecialidad}");
            System.Diagnostics.Debug.WriteLine($"- Medico: {MedicoSeleccionado?.NombreCompleto}");
            System.Diagnostics.Debug.WriteLine($"- Horario: {HorarioSeleccionado?.Hora}");
            System.Diagnostics.Debug.WriteLine($"- Paciente: {PacienteEncontrado?.NombreCompleto}");
            System.Diagnostics.Debug.WriteLine($"- Motivo: {MotivoConsulta}");

            return canCreate;
        }

        private void ResetSelection()
        {
            EspecialidadSeleccionada = null;
            ResetMedicosSelection();
        }

        private void ResetMedicosSelection()
        {
            MedicoSeleccionado = null;
            ShowMedicos = false;
            Medicos.Clear();
            ResetHorariosSelection();
        }

        private void ResetHorariosSelection()
        {
            HorarioSeleccionado = null;
            ShowHorarios = false;
            HorariosDisponibles.Clear();
        }

        private void ResetPacienteInfo()
        {
            PacienteEncontrado = null;
            ShowPacienteInfo = false;
            ShowCreatePatientButton = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}