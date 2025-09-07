// Views/TriajeDigitalPage.xaml.cs
using ClinicaApp.ViewModels;

namespace ClinicaApp.Views;

public partial class TriajeDigitalPage : ContentPage
{
    public TriajeDigitalPage()
    {
        InitializeComponent();
        BindingContext = new TriajeDigitalViewModel();
    }
}