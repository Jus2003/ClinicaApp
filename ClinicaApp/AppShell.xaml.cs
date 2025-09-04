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
        Routing.RegisterRoute("createappointment", typeof(CreateAppointmentPage));

        Routing.RegisterRoute("mi-agenda", typeof(MiAgendaPage));
        Routing.RegisterRoute("responder-triaje", typeof(ResponderTriajePage));
        Routing.RegisterRoute("ver-triaje", typeof(VerTriajePage));


    }
}