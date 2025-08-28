using ClinicaApp.ViewModels;

namespace ClinicaApp.Views;

public partial class UserManagementPage : ContentPage
{
    public UserManagementPage()
    {
        InitializeComponent();
        BindingContext = new UserManagementViewModel();
    }
}