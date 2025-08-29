using ClinicaApp.ViewModels;

namespace ClinicaApp.Views;

public partial class CreateAppointmentPage : ContentPage
{
    public CreateAppointmentPage()
    {
        InitializeComponent();
        BindingContext = new CreateAppointmentViewModel();
    }
}