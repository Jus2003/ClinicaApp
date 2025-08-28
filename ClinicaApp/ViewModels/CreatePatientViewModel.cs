using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ClinicaApp.Models;
using ClinicaApp.Services;

namespace ClinicaApp.ViewModels
{
    public class CreatePatientViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private Patient _patient;
        private bool _isLoading;
        private string _message;
        private bool _isSuccess;
        private DateTime _fechaNacimiento;

        public CreatePatientViewModel()
        {
            _apiService = new ApiService();
            _patient = new Patient();
            _fechaNacimiento = DateTime.Now.AddYears(-30);

            CreateCommand = new Command(async () => await CreatePatientAsync(), () => !IsLoading);
            CancelCommand = new Command(async () => await CancelAsync());
        }

        public Patient Patient
        {
            get => _patient;
            set
            {
                _patient = value;
                OnPropertyChanged();
            }
        }

        public DateTime FechaNacimiento
        {
            get => _fechaNacimiento;
            set
            {
                _fechaNacimiento = value;
                Patient.FechaNacimiento = value.ToString("yyyy-MM-dd");
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

        public ObservableCollection<string> Generos { get; set; } = new ObservableCollection<string> { "M", "F" };

        public ICommand CreateCommand { get; }
        public ICommand CancelCommand { get; }

        private async Task CreatePatientAsync()
        {
            if (!ValidateForm())
                return;

            IsLoading = true;
            Message = "";

            try
            {
                var response = await _apiService.CreatePatientAsync(Patient);

                if (response.Success)
                {
                    Message = $"¡Paciente creado exitosamente!\n\nDatos de acceso:\nUsuario: {response.Data.Username}\nContraseña temporal: {response.Data.PasswordTemporal}\n\nSe ha enviado la información por email.";
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
            if (string.IsNullOrWhiteSpace(Patient.Nombre))
            {
                Message = "El nombre es requerido";
                IsSuccess = false;
                return false;
            }

            if (string.IsNullOrWhiteSpace(Patient.Apellido))
            {
                Message = "El apellido es requerido";
                IsSuccess = false;
                return false;
            }

            if (string.IsNullOrWhiteSpace(Patient.Cedula) || Patient.Cedula.Length != 10)
            {
                Message = "La cédula debe tener 10 dígitos";
                IsSuccess = false;
                return false;
            }

            if (string.IsNullOrWhiteSpace(Patient.Username))
            {
                Message = "El nombre de usuario es requerido";
                IsSuccess = false;
                return false;
            }

            if (string.IsNullOrWhiteSpace(Patient.Email) || !Patient.Email.Contains("@"))
            {
                Message = "Ingrese un email válido";
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
}