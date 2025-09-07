using ClinicaApp.Helpers;
using ClinicaApp.ViewModels;

namespace ClinicaApp.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
        BindingContext = new LoginViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        System.Diagnostics.Debug.WriteLine("🟢 LoginPage: Apareciendo - NUKEAR TODO");

        // NUKEAR SESIÓN CADA VEZ QUE APARECE EL LOGIN
        SessionManager.ClearSession();

        // LIMPIAR CAMPOS
        if (BindingContext is LoginViewModel vm)
        {
            vm.Usuario = string.Empty;
            vm.Password = string.Empty;
            vm.ErrorMessage = string.Empty;
        }
    }
}