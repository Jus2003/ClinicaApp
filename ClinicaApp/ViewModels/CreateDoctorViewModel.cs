using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ClinicaApp.Models;
using ClinicaApp.Services;

namespace ClinicaApp.ViewModels
{
    public class CreateDoctorViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private Doctor _doctor;
        private bool _isLoading;
        private string _message;
        private bool _isSuccess;
        private DateTime _fechaNacimiento;
        private Sucursal _selectedSucursal;

        public CreateDoctorViewModel()
        {
            _apiService = new ApiService();
            _doctor = new Doctor();
            _fechaNacimiento = DateTime.Now.AddYears(-25);

            CreateCommand = new Command(async () => await CreateDoctorAsync(), () => !IsLoading);
            CancelCommand = new Command(async () => await CancelAsync());

            InitializeData();
        }

        public Doctor Doctor
        {
            get => _doctor;
            set
            {
                _doctor = value;
                OnPropertyChanged();
            }
        }

        public DateTime FechaNacimiento
        {
            get => _fechaNacimiento;
            set
            {
                _fechaNacimiento = value;
                Doctor.FechaNacimiento = value.ToString("yyyy-MM-dd");
                OnPropertyChanged();
            }
        }

        public Sucursal SelectedSucursal
        {
            get => _selectedSucursal;
            set
            {
                _selectedSucursal = value;
                Doctor.IdSucursal = value?.IdSucursal ?? 0;
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
                ((Command)CreateCommand).ChangeCanExecute();
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

        public ObservableCollection<EspecialidadSelectable> Especialidades { get; set; } = new ObservableCollection<EspecialidadSelectable>();
        public ObservableCollection<Sucursal> Sucursales { get; set; } = new ObservableCollection<Sucursal>();
        public ObservableCollection<string> Generos { get; set; } = new ObservableCollection<string> { "M", "F" };

        public ICommand CreateCommand { get; }
        public ICommand CancelCommand { get; }

        private async void InitializeData()
        {
            try
            {
                IsLoading = true;

                // Cargar especialidades
                var especialidadesResponse = await _apiService.GetEspecialidadesAsync();
                if (especialidadesResponse.Success && especialidadesResponse.Data != null)
                {
                    Especialidades.Clear();
                    foreach (var esp in especialidadesResponse.Data)
                    {
                        Especialidades.Add(new EspecialidadSelectable(esp));
                    }
                }

                // Cargar sucursales
                var sucursalesResponse = await _apiService.GetSucursalesAsync();
                if (sucursalesResponse.Success && sucursalesResponse.Data != null)
                {
                    Sucursales.Clear();
                    foreach (var suc in sucursalesResponse.Data)
                    {
                        Sucursales.Add(suc);
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

        private async Task CreateDoctorAsync()
        {
            if (!ValidateForm())
                return;

            IsLoading = true;
            Message = "";

            try
            {
                // Obtener especialidades seleccionadas
                Doctor.Especialidades = Especialidades
                    .Where(e => e.IsSelected)
                    .Select(e => e.Especialidad.IdEspecialidad)
                    .ToList();

                var response = await _apiService.CreateDoctorAsync(Doctor);

                if (response.Success)
                {
                    Message = $"¡Médico creado exitosamente!\n\nDatos de acceso:\nUsuario: {response.Data.Username}\nContraseña temporal: {response.Data.PasswordTemporal}\n\nSe ha enviado la información por email.";
                    IsSuccess = true;

                    // Limpiar formulario después de 3 segundos
                    await Task.Delay(3000);
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    Message = response.Message;
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

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(Doctor.Nombre))
            {
                Message = "El nombre es requerido";
                IsSuccess = false;
                return false;
            }

            if (string.IsNullOrWhiteSpace(Doctor.Apellido))
            {
                Message = "El apellido es requerido";
                IsSuccess = false;
                return false;
            }

            if (string.IsNullOrWhiteSpace(Doctor.Cedula) || Doctor.Cedula.Length != 10)
            {
                Message = "La cédula debe tener 10 dígitos";
                IsSuccess = false;
                return false;
            }

            if (string.IsNullOrWhiteSpace(Doctor.Username))
            {
                Message = "El nombre de usuario es requerido";
                IsSuccess = false;
                return false;
            }

            if (string.IsNullOrWhiteSpace(Doctor.Email) || !Doctor.Email.Contains("@"))
            {
                Message = "Ingrese un email válido";
                IsSuccess = false;
                return false;
            }

            if (SelectedSucursal == null)
            {
                Message = "Seleccione una sucursal";
                IsSuccess = false;
                return false;
            }

            if (!Especialidades.Any(e => e.IsSelected))
            {
                Message = "Seleccione al menos una especialidad";
                IsSuccess = false;
                return false;
            }

            return true;
        }

        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class EspecialidadSelectable : INotifyPropertyChanged
    {
        private bool _isSelected;

        public EspecialidadSelectable(Especialidad especialidad)
        {
            Especialidad = especialidad;
        }

        public Especialidad Especialidad { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public string NombreEspecialidad => Especialidad.NombreEspecialidad;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}