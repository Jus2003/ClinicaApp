using ClinicaApp.ViewModels;

namespace ClinicaApp.Views;

public partial class DoctorSchedulePage : ContentPage
{
    public DoctorSchedulePage()
    {
        InitializeComponent();
        BindingContext = new DoctorScheduleViewModel();
    }
}