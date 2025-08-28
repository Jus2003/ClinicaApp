using System.ComponentModel;
using System.Windows.Input;

namespace ClinicaApp.ViewModels
{
    public class UserManagementViewModel : INotifyPropertyChanged
    {
        public UserManagementViewModel()
        {
            CreateDoctorCommand = new Command(async () => await CreateDoctorAsync());
            CreatePatientCommand = new Command(async () => await CreatePatientAsync());
            BackCommand = new Command(async () => await BackAsync());
        }

        public ICommand CreateDoctorCommand { get; }
        public ICommand CreatePatientCommand { get; }
        public ICommand BackCommand { get; }

        private async Task CreateDoctorAsync()
        {
            await Shell.Current.GoToAsync("createdoctor");
        }

        private async Task CreatePatientAsync()
        {
            await Shell.Current.GoToAsync("createpatient");
        }

        private async Task BackAsync()
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