using ClinicaApp.ViewModels;

namespace ClinicaApp.Views;

[QueryProperty(nameof(IdCita), "idCita")]
public partial class VerTriajePage : ContentPage
{
    public string IdCita { get; set; }

    public VerTriajePage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (!string.IsNullOrEmpty(IdCita) && int.TryParse(IdCita, out int citaId))
        {
            var viewModel = new VerTriajeViewModel();
            viewModel.CitaId = citaId;
        }
        else
        {
            DisplayAlert("Error", "ID de cita inválido", "OK");
            Shell.Current.GoToAsync("..");
        }
    }
}