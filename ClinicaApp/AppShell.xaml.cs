using ClinicaApp.Views;

namespace ClinicaApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("forgotpassword", typeof(ForgotPasswordPage));
    }
}