using ClinicaApp.ViewModels;

namespace ClinicaApp.Views;

public partial class CreatePatientPage : ContentPage
{
    public CreatePatientPage()
    {
        InitializeComponent();
        BindingContext = new CreatePatientViewModel();
    }
}