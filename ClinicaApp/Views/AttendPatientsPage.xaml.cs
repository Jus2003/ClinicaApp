using ClinicaApp.ViewModels;

namespace ClinicaApp.Views;

public partial class AttendPatientsPage : ContentPage
{
    public AttendPatientsPage()
    {
        InitializeComponent();
        BindingContext = new AttendPatientsViewModel();
    }
}