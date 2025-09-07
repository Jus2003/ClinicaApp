// Views/TriajeProgressPage.xaml.cs
using ClinicaApp.ViewModels;

namespace ClinicaApp.Views;

public partial class TriajeProgressPage : ContentPage
{
    public TriajeProgressPage()
    {
        InitializeComponent();
        BindingContext = new TriajeProgressViewModel();
    }

    private void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
    {
        if (BindingContext is TriajeProgressViewModel viewModel && viewModel.CurrentQuestion != null)
        {
            viewModel.CurrentQuestion.Respuesta = ((int)e.NewValue).ToString();
        }
    }
}