using System.ComponentModel;
using System.Windows.Input;
using ClinicaApp.Services;
using ClinicaApp.Helpers;

namespace ClinicaApp.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private string _usuario;
        private string _password;
        private bool _isLoading;
        private string _errorMessage;

        public LoginViewModel()
        {
            _apiService = new ApiService();
            LoginCommand = new Command(async () => await LoginAsync(), () => !IsLoading);
            ForgotPasswordCommand = new Command(async () => await GoToForgotPasswordAsync());
        }

        public string Usuario
        {
            get => _usuario;
            set
            {
                _usuario = value;
                OnPropertyChanged();
                ((Command)LoginCommand).ChangeCanExecute();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
                ((Command)LoginCommand).ChangeCanExecute();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
                ((Command)LoginCommand).ChangeCanExecute();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand ForgotPasswordCommand { get; }

        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Usuario) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Por favor ingrese usuario y contraseña";
                return;
            }

            IsLoading = true;
            ErrorMessage = "";

            try
            {
                var response = await _apiService.LoginAsync(Usuario, Password);

                if (response.Success)
                {
                    SessionManager.SetUserSession(response.Data);
                    await Shell.Current.GoToAsync("//main");
                }
                else
                {
                    ErrorMessage = response.Message;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task GoToForgotPasswordAsync()
        {
            await Shell.Current.GoToAsync("forgotpassword");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}