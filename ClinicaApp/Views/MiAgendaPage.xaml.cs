using ClinicaApp.ViewModels;

namespace ClinicaApp.Views;

public partial class MiAgendaPage : ContentPage
{
    public MiAgendaPage()
    {
        InitializeComponent();
        BindingContext = new MiAgendaViewModel();
    }
}