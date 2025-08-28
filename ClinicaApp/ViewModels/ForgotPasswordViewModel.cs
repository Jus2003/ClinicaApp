using System.ComponentModel;
using System.Windows.Input;
using ClinicaApp.Services;

namespace ClinicaApp.ViewModels
{
    public class ForgotPasswordViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private string _email;
        private bool _isLoading;
        private string _message;
        private bool _showSuccess;

        public ForgotPasswordViewModel()
        {
            _apiService = new ApiService();
            SendCommand = new Command(async () => await SendRecoveryEmailAsync(), () => !IsLoading);
            BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
                ((Command)SendCommand).ChangeCanExecute();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
                ((Command)SendCommand).ChangeCanExecute();
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

        public bool ShowSuccess
        {
            get => _showSuccess;
            set
            {
                _showSuccess = value;
                OnPropertyChanged();
            }
        }

        public ICommand SendCommand { get; }
        public ICommand BackCommand { get; }

        private async Task SendRecoveryEmailAsync()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                Message = "Por favor ingrese su correo electrónico";
                ShowSuccess = false;
                return;
            }

            IsLoading = true;
            Message = "";

            try
            {
                var response = await _apiService.ForgotPasswordAsync(Email);

                if (response.Success)
                {
                    if (response.Data != null)
                    {
                        // Mostrar información de la contraseña temporal
                        Message = $"{response.Message}\n\nUsuario: {response.Data.Usuario}\nContraseña temporal: {response.Data.PasswordTemporal}\n\n{response.Data.Nota}";
                    }
                    else
                    {
                        Message = response.Message;
                    }
                    ShowSuccess = true;
                }
                else
                {
                    Message = response.Message;
                    ShowSuccess = false;
                }
            }
            catch (Exception ex)
            {
                Message = $"Error: {ex.Message}";
                ShowSuccess = false;
                System.Diagnostics.Debug.WriteLine($"Error en ForgotPassword: {ex}");
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