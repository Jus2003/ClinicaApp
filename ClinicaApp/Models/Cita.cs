using System.Text.Json.Serialization;
using ClinicaApp.Helpers;

namespace ClinicaApp.Models
{
    public class CitaRequest
    {
        [JsonPropertyName("cedula_paciente")]
        public string CedulaPaciente { get; set; }

        [JsonPropertyName("id_medico")]
        public int IdMedico { get; set; }

        [JsonPropertyName("id_especialidad")]
        public int IdEspecialidad { get; set; }

        [JsonPropertyName("id_sucursal")]
        public int IdSucursal { get; set; }

        [JsonPropertyName("fecha_cita")]
        public string FechaCita { get; set; }

        [JsonPropertyName("hora_cita")]
        public string HoraCita { get; set; }

        [JsonPropertyName("tipo_cita")]
        public string TipoCita { get; set; }

        [JsonPropertyName("motivo_consulta")]
        public string MotivoConsulta { get; set; }

        [JsonPropertyName("observaciones")]
        public string Observaciones { get; set; }
    }

    public class MedicoPorEspecialidad
    {
        [JsonPropertyName("id_medico")]
        [JsonConverter(typeof(StringToIntConverter))]
        public int IdMedico { get; set; }

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; }

        [JsonPropertyName("apellido")]
        public string Apellido { get; set; }

        [JsonPropertyName("nombre_completo")]
        public string NombreCompleto { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("telefono")]
        public string Telefono { get; set; }

        [JsonPropertyName("sucursal")]
        public string Sucursal { get; set; }

        [JsonPropertyName("especialidad")]
        public string Especialidad { get; set; }

        [JsonPropertyName("numero_licencia")]
        public string NumeroLicencia { get; set; }
    }

    public class HorarioDisponible
    {
        [JsonPropertyName("hora")]
        public string Hora { get; set; }

        [JsonPropertyName("hora_fin")]
        public string HoraFin { get; set; }

        [JsonPropertyName("disponible")]
        public bool Disponible { get; set; }

        [JsonPropertyName("duracion_minutos")]
        public int DuracionMinutos { get; set; }
    }

    public class HorariosDisponiblesRequest
    {
        [JsonPropertyName("id_medico")]
        public int IdMedico { get; set; }

        [JsonPropertyName("fecha")]
        public string Fecha { get; set; }

        [JsonPropertyName("duracion_minutos")]
        public int DuracionMinutos { get; set; } = 45;
    }

    public class ValidarDisponibilidadRequest
    {
        [JsonPropertyName("id_medico")]
        public int IdMedico { get; set; }

        [JsonPropertyName("fecha_cita")]
        public string FechaCita { get; set; }

        [JsonPropertyName("hora_cita")]
        public string HoraCita { get; set; }
    }

    public class CitaResponse
    {
        [JsonPropertyName("cita_creada")]
        public string CitaCreada { get; set; }

        [JsonPropertyName("id_cita")]
        [JsonConverter(typeof(StringToIntConverter))]
        public int IdCita { get; set; }

        [JsonPropertyName("numero_cita")]
        public string NumeroCita { get; set; }

        [JsonPropertyName("paciente")]
        public PacienteCita Paciente { get; set; }

        [JsonPropertyName("medico")]
        public MedicoCita Medico { get; set; }

        [JsonPropertyName("detalles_cita")]
        public DetallesCita DetallesCita { get; set; }

        [JsonPropertyName("informacion_virtual")]
        public InformacionVirtual InformacionVirtual { get; set; }

        [JsonPropertyName("notificaciones")]
        public Notificaciones Notificaciones { get; set; }

        [JsonPropertyName("proximos_pasos")]
        public ProximosPasos ProximosPasos { get; set; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }
    }

    public class PacienteCita
    {
        [JsonPropertyName("id")]
        [JsonConverter(typeof(StringToIntConverter))]
        public int Id { get; set; }

        [JsonPropertyName("nombre_completo")]
        public string NombreCompleto { get; set; }

        [JsonPropertyName("cedula")]
        public string Cedula { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }
    }

    public class MedicoCita
    {
        [JsonPropertyName("id")]
        [JsonConverter(typeof(StringToIntConverter))]
        public int Id { get; set; }

        [JsonPropertyName("nombre_completo")]
        public string NombreCompleto { get; set; }

        [JsonPropertyName("especialidad")]
        public string Especialidad { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }
    }

    public class DetallesCita
    {
        [JsonPropertyName("fecha")]
        public string Fecha { get; set; }

        [JsonPropertyName("hora")]
        public string Hora { get; set; }

        [JsonPropertyName("tipo")]
        public string Tipo { get; set; }

        [JsonPropertyName("motivo")]
        public string Motivo { get; set; }

        [JsonPropertyName("observaciones")]
        public string Observaciones { get; set; }

        [JsonPropertyName("estado")]
        public string Estado { get; set; }

        [JsonPropertyName("sucursal")]
        public string Sucursal { get; set; }
    }

    public class InformacionVirtual
    {
        [JsonPropertyName("🎥 Enlace_Zoom")]
        public string EnlaceZoom { get; set; }

        [JsonPropertyName("🔢 ID_Reunion")]
        public string IdReunion { get; set; }

        [JsonPropertyName("🔐 Password")]
        public int Password { get; set; }

        [JsonPropertyName("📝 Instrucciones")]
        public string Instrucciones { get; set; }
    }

    public class Notificaciones
    {
        [JsonPropertyName("email_paciente")]
        public string EmailPaciente { get; set; }

        [JsonPropertyName("email_medico")]
        public string EmailMedico { get; set; }

        [JsonPropertyName("mensaje")]
        public string Mensaje { get; set; }
    }

    public class ProximosPasos
    {
        [JsonPropertyName("para_paciente")]
        public string ParaPaciente { get; set; }

        [JsonPropertyName("recordatorio")]
        public string Recordatorio { get; set; }
    }

    public class MedicosEspecialidadResponse
    {
        [JsonPropertyName("id_especialidad")]
        [JsonConverter(typeof(StringToIntConverter))]
        public int IdEspecialidad { get; set; }

        [JsonPropertyName("tipo_cita")]
        public string TipoCita { get; set; }

        [JsonPropertyName("medicos")]
        public List<MedicoPorEspecialidad> Medicos { get; set; } = new List<MedicoPorEspecialidad>();
    }

    public class HorariosDisponiblesResponse
    {
        [JsonPropertyName("id_medico")]
        [JsonConverter(typeof(StringToIntConverter))]
        public int IdMedico { get; set; }

        [JsonPropertyName("fecha")]
        public string Fecha { get; set; }

        [JsonPropertyName("duracion_minutos")]
        public int DuracionMinutos { get; set; }

        [JsonPropertyName("horarios_disponibles")]
        public List<HorarioDisponible> HorariosDisponibles { get; set; } = new List<HorarioDisponible>();
    }

    public class ValidarDisponibilidadResponse
    {
        [JsonPropertyName("disponible")]
        public bool Disponible { get; set; }

        [JsonPropertyName("fecha_cita")]
        public string FechaCita { get; set; }

        [JsonPropertyName("hora_cita")]
        public string HoraCita { get; set; }

        [JsonPropertyName("id_medico")]
        [JsonConverter(typeof(StringToIntConverter))]
        public int IdMedico { get; set; }

        [JsonPropertyName("razon")]
        public string Razon { get; set; }
    }
}