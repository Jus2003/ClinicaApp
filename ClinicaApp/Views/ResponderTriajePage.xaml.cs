using ClinicaApp.ViewModels;

namespace ClinicaApp.Views;

[QueryProperty(nameof(IdCita), "idCita")]
public partial class ResponderTriajePage : ContentPage
{
    public string IdCita { get; set; }

    public ResponderTriajePage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (!string.IsNullOrEmpty(IdCita) && int.TryParse(IdCita, out int citaId))
        {
            BindingContext = new ResponderTriajeViewModel(citaId);
        }
        else
        {
            DisplayAlert("Error", "ID de cita inválido", "OK");
            Shell.Current.GoToAsync("..");
        }
    }
}