using ClinicaApp.Views;

namespace ClinicaApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Registrar rutas
        Routing.RegisterRoute("forgotpassword", typeof(ForgotPasswordPage));
        Routing.RegisterRoute("usermanagement", typeof(UserManagementPage));
        Routing.RegisterRoute("createdoctor", typeof(CreateDoctorPage));
        Routing.RegisterRoute("createpatient", typeof(CreatePatientPage));
        Routing.RegisterRoute("doctorschedule", typeof(DoctorSchedulePage));

    }
}