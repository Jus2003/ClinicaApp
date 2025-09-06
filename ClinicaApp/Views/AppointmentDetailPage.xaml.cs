using ClinicaApp.ViewModels;

namespace ClinicaApp.Views;

public partial class AppointmentDetailPage : ContentPage
{
    public AppointmentDetailPage()
    {
        InitializeComponent();
        BindingContext = new AppointmentDetailViewModel();
    }
}