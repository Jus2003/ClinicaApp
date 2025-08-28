using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ClinicaApp.Models;
using ClinicaApp.Services;

namespace ClinicaApp.ViewModels
{
    public class DoctorScheduleViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private MedicoBasico _selectedMedico;
        private bool _showHorarios;
        private bool _showEditForm;
        private bool _isLoading;
        private string _message;
        private bool _isSuccess;

        public DoctorScheduleViewModel()
        {
            _apiService = new ApiService();

            LoadHorariosCommand = new Command(async () => await LoadHorariosAsync(), () => HasSelectedMedico && !IsLoading);
            EditHorariosCommand = new Command(() => ShowEditForm = true);
            AddHorarioCommand = new Command(AddNuevoHorario);
            RemoveHorarioCommand = new Command<NuevoHorarioViewModel>(RemoveHorario);
            SaveHorariosCommand = new Command(async () => await SaveHorariosAsync(), () => !IsLoading);
            CancelEditCommand = new Command(CancelEdit);

            InitializeData();
        }

        public ObservableCollection<MedicoBasico> Medicos { get; set; } = new ObservableCollection<MedicoBasico>();
        public ObservableCollection<Horario> HorariosActuales { get; set; } = new ObservableCollection<Horario>();
        public ObservableCollection<NuevoHorarioViewModel> NuevosHorarios { get; set; } = new ObservableCollection<NuevoHorarioViewModel>();
        public ObservableCollection<Sucursal> Sucursales { get; set; } = new ObservableCollection<Sucursal>();

        public List<string> DiasSemana { get; } = new List<string>
        {
            "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo"
        };

        public MedicoBasico SelectedMedico
        {
            get => _selectedMedico;
            set
            {
                _selectedMedico = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasSelectedMedico));
                OnPropertyChanged(nameof(MedicoInfo));
                ((Command)LoadHorariosCommand).ChangeCanExecute();

                // Limpiar horarios cuando cambie el médico
                ShowHorarios = false;
                ShowEditForm = false;
                HorariosActuales.Clear();
            }
        }

        public bool HasSelectedMedico => SelectedMedico != null;

        public string MedicoInfo => SelectedMedico != null
            ? $"Horarios de: {SelectedMedico.NombreCompleto} - {SelectedMedico.Especialidades}"
            : "";

        public bool ShowHorarios
        {
            get => _showHorarios;
            set
            {
                _showHorarios = value;
                OnPropertyChanged();
            }
        }

        public bool ShowEditForm
        {
            get => _showEditForm;
            set
            {
                _showEditForm = value;
                OnPropertyChanged();
                if (value)
                {
                    PrepareEditForm();
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
                ((Command)LoadHorariosCommand).ChangeCanExecute();
                ((Command)SaveHorariosCommand).ChangeCanExecute();
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

        public ICommand LoadHorariosCommand { get; }
        public ICommand EditHorariosCommand { get; }
        public ICommand AddHorarioCommand { get; }
        public ICommand RemoveHorarioCommand { get; }
        public ICommand SaveHorariosCommand { get; }
        public ICommand CancelEditCommand { get; }

        private async void InitializeData()
        {
            try
            {
                IsLoading = true;

                // Cargar médicos
                var medicosResponse = await _apiService.GetMedicosAsync();
                if (medicosResponse.Success && medicosResponse.Data != null)
                {
                    Medicos.Clear();
                    foreach (var medico in medicosResponse.Data)
                    {
                        Medicos.Add(medico);
                    }
                }

                // Cargar sucursales (usando datos hardcodeados como antes)
                var sucursalesResponse = await _apiService.GetSucursalesAsync();
                if (sucursalesResponse.Success && sucursalesResponse.Data != null)
                {
                    Sucursales.Clear();
                    foreach (var sucursal in sucursalesResponse.Data)
                    {
                        Sucursales.Add(sucursal);
                    }
                }
            }
            catch (Exception ex)
            {
                Message = $"Error cargando datos: {ex.Message}";
                IsSuccess = false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadHorariosAsync()
        {
            if (SelectedMedico == null) return;

            try
            {
                IsLoading = true;
                Message = "";

                var response = await _apiService.GetHorariosMedicoAsync(SelectedMedico.IdMedico);

                if (response.Success && response.Data != null)
                {
                    HorariosActuales.Clear();
                    foreach (var horario in response.Data.Horarios)
                    {
                        HorariosActuales.Add(horario);
                    }
                    ShowHorarios = true;
                    Message = $"Se encontraron {HorariosActuales.Count} horarios";
                    IsSuccess = true;
                }
                else
                {
                    Message = response.Message ?? "No se pudieron cargar los horarios";
                    IsSuccess = false;
                    ShowHorarios = true; // Mostrar aunque no tenga horarios
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

        private void PrepareEditForm()
        {
            // Limpiar formulario
            NuevosHorarios.Clear();

            // Si tiene horarios actuales, copiarlos al formulario de edición
            foreach (var horario in HorariosActuales)
            {
                var sucursal = Sucursales.FirstOrDefault(s => s.IdSucursal == horario.IdSucursal);
                var nuevoHorario = new NuevoHorarioViewModel(Sucursales.ToList())
                {
                    DiaSemanaIndex = horario.DiaSemana - 1, // Convertir a índice (0-based)
                    HoraInicioTime = TimeSpan.Parse(horario.HoraInicio),
                    HoraFinTime = TimeSpan.Parse(horario.HoraFin),
                    SelectedSucursal = sucursal
                };
                NuevosHorarios.Add(nuevoHorario);
            }

            // Si no tiene horarios, agregar uno vacío
            if (NuevosHorarios.Count == 0)
            {
                AddNuevoHorario();
            }
        }

        private void AddNuevoHorario()
        {
            NuevosHorarios.Add(new NuevoHorarioViewModel(Sucursales.ToList()));
        }

        private void RemoveHorario(NuevoHorarioViewModel horario)
        {
            NuevosHorarios.Remove(horario);
        }

        private async Task SaveHorariosAsync()
        {
            if (SelectedMedico == null) return;

            try
            {
                IsLoading = true;
                Message = "";

                // Validar horarios
                var horariosValidos = new List<HorarioRequest>();
                foreach (var horario in NuevosHorarios)
                {
                    if (horario.IsValid())
                    {
                        horariosValidos.Add(new HorarioRequest
                        {
                            DiaSemana = horario.DiaSemanaIndex + 1, // Convertir a 1-based
                            HoraInicio = horario.HoraInicioTime.ToString(@"hh\:mm"),
                            HoraFin = horario.HoraFinTime.ToString(@"hh\:mm"),
                            IdSucursal = horario.SelectedSucursal?.IdSucursal ?? 1
                        });
                    }
                }

                if (horariosValidos.Count == 0)
                {
                    Message = "Debe agregar al menos un horario válido";
                    IsSuccess = false;
                    return;
                }

                var response = await _apiService.AsignarHorariosAsync(SelectedMedico.IdMedico, horariosValidos);

                if (response.Success)
                {
                    Message = "Horarios guardados exitosamente";
                    IsSuccess = true;
                    ShowEditForm = false;

                    // Recargar horarios
                    await LoadHorariosAsync();
                }
                else
                {
                    Message = response.Message ?? "Error al guardar horarios";
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

        private void CancelEdit()
        {
            ShowEditForm = false;
            Message = "";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class NuevoHorarioViewModel : INotifyPropertyChanged
    {
        private int _diaSemanaIndex;
        private TimeSpan _horaInicioTime = new TimeSpan(8, 0, 0);
        private TimeSpan _horaFinTime = new TimeSpan(17, 0, 0);
        private Sucursal _selectedSucursal;

        public NuevoHorarioViewModel(List<Sucursal> sucursales)
        {
            Sucursales = sucursales;
            SelectedSucursal = sucursales.FirstOrDefault();
        }

        public List<Sucursal> Sucursales { get; }

        public int DiaSemanaIndex
        {
            get => _diaSemanaIndex;
            set
            {
                _diaSemanaIndex = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan HoraInicioTime
        {
            get => _horaInicioTime;
            set
            {
                _horaInicioTime = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan HoraFinTime
        {
            get => _horaFinTime;
            set
            {
                _horaFinTime = value;
                OnPropertyChanged();
            }
        }

        public Sucursal SelectedSucursal
        {
            get => _selectedSucursal;
            set
            {
                _selectedSucursal = value;
                OnPropertyChanged();
            }
        }

        public bool IsValid()
        {
            return SelectedSucursal != null &&
                   HoraInicioTime < HoraFinTime;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}