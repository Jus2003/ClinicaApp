using System.ComponentModel;
using Microsoft.Maui;

namespace ClinicaApp.Models
{
    public class CitaAgenda : INotifyPropertyChanged
    {
        public int IdCita { get; set; }
        public string FechaCita { get; set; }
        public string HoraCita { get; set; }
        public string TipoCita { get; set; }
        public string EstadoCita { get; set; }
        public string MotivoConsulta { get; set; }
        public string Observaciones { get; set; }
        public string NombrePaciente { get; set; }
        public string CedulaPaciente { get; set; }
        public string TelefonoPaciente { get; set; }
        public string EmailPaciente { get; set; }
        public string NombreMedico { get; set; }
        public string CedulaMedico { get; set; }
        public string TelefonoMedico { get; set; }
        public string EmailMedico { get; set; }
        public string NombreEspecialidad { get; set; }
        public string NombreSucursal { get; set; }

        // Propiedades calculadas para la UI
        public string FechaHora => $"📅 {DateTime.Parse(FechaCita):dd/MM/yyyy} - ⏰ {TimeSpan.Parse(HoraCita):hh\\:mm}";

        public string EstadoTexto => EstadoCita switch
        {
            "agendada" => "AGENDADA",
            "confirmada" => "CONFIRMADA",
            "en_curso" => "EN CURSO",
            "completada" => "COMPLETADA",
            "cancelada" => "CANCELADA",
            "no_asistio" => "NO ASISTIÓ",
            _ => "DESCONOCIDO"
        };

        public string EstadoIcon => EstadoCita switch
        {
            "agendada" => "🕐",
            "confirmada" => "✅",
            "en_curso" => "🏥",
            "completada" => "✔️",
            "cancelada" => "❌",
            "no_asistio" => "❌",
            _ => "❓"
        };

        public Color EstadoColor => EstadoCita switch
        {
            "agendada" => Color.FromArgb("#FF9800"),
            "confirmada" => Color.FromArgb("#4CAF50"),
            "en_curso" => Color.FromArgb("#2196F3"),
            "completada" => Color.FromArgb("#8BC34A"),
            "cancelada" => Color.FromArgb("#F44336"),
            "no_asistio" => Color.FromArgb("#9E9E9E"),
            _ => Color.FromArgb("#666666")
        };

        // Para determinar qué mostrar según el rol del usuario
        public bool EsMedico { get; set; }
        public bool EsPaciente => !EsMedico;

        public string PacienteOMedico => EsMedico ? NombrePaciente : NombreMedico;
        public string TituloCita => EsMedico ? $"Paciente: {NombrePaciente}" : $"Dr(a). {NombreMedico}";
        public string Especialidad => $"🏥 {NombreEspecialidad}";
        public string Sucursal => $"📍 {NombreSucursal}";
        public bool HasMotivo => !string.IsNullOrEmpty(MotivoConsulta);

        // Permisos según estado y rol
        public bool PuedeConfirmar => EsMedico && EstadoCita == "agendada";
        public bool PuedeCancelar => EsMedico && (EstadoCita == "agendada" || EstadoCita == "confirmada");
        public bool EsPacienteYPuedeCancelar => EsPaciente && (EstadoCita == "agendada" || EstadoCita == "confirmada");

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }

    }
}