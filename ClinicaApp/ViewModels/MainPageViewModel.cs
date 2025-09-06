using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ClinicaApp.Models;
using ClinicaApp.Helpers;
using ClinicaApp.Views;

namespace ClinicaApp.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public MainPageViewModel()
        {
            LogoutCommand = new Command(async () => await LogoutAsync());
            NavigateCommand = new Command<SubMenu>(async (submenu) => await NavigateToSubmenuAsync(submenu));
            LoadUserData();
        }

        public string UserName => SessionManager.CurrentUser != null
            ? $"{SessionManager.CurrentUser.Nombre} {SessionManager.CurrentUser.Apellido}"
            : "";

        public string UserRole => SessionManager.CurrentUser?.Rol ?? "";
        public string UserBranch => SessionManager.CurrentUser?.Sucursal ?? "";

        public ObservableCollection<Menu> Menus { get; private set; } = new ObservableCollection<Menu>();

        public ICommand LogoutCommand { get; }
        public ICommand NavigateCommand { get; }

        private void LoadUserData()
        {
            if (SessionManager.UserMenus != null)
            {
                Menus.Clear();
                foreach (var menu in SessionManager.UserMenus)
                {
                    Menus.Add(menu);
                }
            }
            OnPropertyChanged(nameof(UserName));
            OnPropertyChanged(nameof(UserRole));
            OnPropertyChanged(nameof(UserBranch));
        }

        private async Task LogoutAsync()
        {
            SessionManager.ClearSession();
            await Shell.Current.GoToAsync("//login");
        }

        private async Task NavigateToSubmenuAsync(SubMenu submenu)
        {
            if (submenu != null)
            {
                // Verificar diferentes URIs de submenús
                switch (submenu.UriSubmenu)
                {
                    case "admin/usuarios":
                        await Shell.Current.GoToAsync("usermanagement");
                        break;
                    case "citas/disponibilidad":
                        await Shell.Current.GoToAsync("doctorschedule");
                        break;
                    case "citas/agendar":
                        await Shell.Current.GoToAsync("createappointment");
                        break;
                    case "pacientes/registrar":   // Submenú desde Pacientes → Registrar
                        await Shell.Current.GoToAsync("createpatient");
                        break;
                    case "citas/agenda":
                        await Shell.Current.GoToAsync("mi-agenda");
                        break;
                    case "consultas/triaje":
                        await Shell.Current.GoToAsync("ResponderTriajePage");
                        break;
                    case "consultas/atender":  // ✅ AGREGAR ESTA LÍNEA
                        await Shell.Current.GoToAsync("AttendPatientsPage");
                        break;
                    default:
                        await Shell.Current.DisplayAlert("Navegación",
                            $"Ir a: {submenu.NombreSubmenu}\nURI: {submenu.UriSubmenu}", "OK");
                        break;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}