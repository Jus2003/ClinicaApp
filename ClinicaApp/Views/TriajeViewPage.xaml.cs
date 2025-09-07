using ClinicaApp.ViewModels;

namespace ClinicaApp.Views;

public partial class TriajeViewPage : ContentPage
{
    public TriajeViewPage()
    {
        InitializeComponent();
        BindingContext = new TriajeViewViewModel();
    }
}