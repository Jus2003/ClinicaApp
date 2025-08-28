using ClinicaApp.ViewModels;

namespace ClinicaApp.Views;

public partial class CreateDoctorPage : ContentPage
{
    public CreateDoctorPage()
    {
        InitializeComponent();
        BindingContext = new CreateDoctorViewModel();
    }
}