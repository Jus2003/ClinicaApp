// Views/CitaDetallePage.xaml.cs
using ClinicaApp.ViewModels;

namespace ClinicaApp.Views;

public partial class CitaDetallePage : ContentPage
{
    public CitaDetallePage()
    {
        InitializeComponent();
        BindingContext = new CitaDetalleViewModel();
    }
}