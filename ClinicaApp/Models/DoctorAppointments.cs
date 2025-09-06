using System.Text.Json.Serialization;

namespace ClinicaApp.Models
{
    // Modelo para obtener citas del médico
    public class DoctorAppointmentsRequest
    {
        [JsonPropertyName("id_medico")]
        public int IdMedico { get; set; }
    }

    public class DoctorAppointmentsResponse
    {
        [JsonPropertyName("id_medico_consultado")]
        public int IdMedicoConsultado { get; set; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }

        [JsonPropertyName("medico")]
        public DoctorInfo Medico { get; set; }

        [JsonPropertyName("citas")]
        public List<AppointmentSummary> Citas { get; set; }

        [JsonPropertyName("estadisticas")]
        public AppointmentStatistics Estadisticas { get; set; }
    }

    public class DoctorInfo
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nombre_completo")]
        public string NombreCompleto { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("especialidades")]
        public string Especialidades { get; set; }
    }

    public class AppointmentSummary
    {
        [JsonPropertyName("id_cita")]
        public int IdCita { get; set; }

        [JsonPropertyName("fecha_cita")]
        public string FechaCita { get; set; }

        [JsonPropertyName("hora_cita")]
        public string HoraCita { get; set; }

        [JsonPropertyName("tipo_cita")]
        public string TipoCita { get; set; }

        [JsonPropertyName("estado_cita")]
        public string EstadoCita { get; set; }

        [JsonPropertyName("motivo_consulta")]
        public string MotivoConsulta { get; set; }

        [JsonPropertyName("nombre_paciente")]
        public string NombrePaciente { get; set; }

        [JsonPropertyName("nombre_especialidad")]
        public string NombreEspecialidad { get; set; }

        // Propiedades calculadas para UI
        public string EstadoFormateado => EstadoCita switch
        {
            "agendada" => "📅 Agendada",
            "confirmada" => "✅ Confirmada",
            "en_curso" => "🏥 En Curso",
            "completada" => "✅ Completada",
            "cancelada" => "❌ Cancelada",
            "no_asistio" => "⚠️ No Asistió",
            _ => EstadoCita
        };

        public Color EstadoColor => EstadoCita switch
        {
            "agendada" => Colors.Orange,
            "confirmada" => Colors.Green,
            "en_curso" => Colors.Blue,
            "completada" => Colors.DarkGreen,
            "cancelada" => Colors.Red,
            "no_asistio" => Colors.Gray,
            _ => Colors.Black
        };
    }

    public class AppointmentStatistics
    {
        [JsonPropertyName("total_citas")]
        public int TotalCitas { get; set; }

        [JsonPropertyName("citas_hoy")]
        public int CitasHoy { get; set; }
    }

    // Modelo para detalles de cita
    public class AppointmentDetailRequest
    {
        [JsonPropertyName("id_cita")]
        public int IdCita { get; set; }
    }

    public class AppointmentDetailResponse
    {
        [JsonPropertyName("cita")]
        public AppointmentDetail Cita { get; set; }
    }

    public class AppointmentDetail
    {
        [JsonPropertyName("id_cita")]
        public int IdCita { get; set; }

        [JsonPropertyName("fecha_cita")]
        public string FechaCita { get; set; }

        [JsonPropertyName("hora_cita")]
        public string HoraCita { get; set; }

        [JsonPropertyName("tipo_cita")]
        public string TipoCita { get; set; }

        [JsonPropertyName("estado_cita")]
        public string EstadoCita { get; set; }

        [JsonPropertyName("motivo_consulta")]
        public string MotivoConsulta { get; set; }

        [JsonPropertyName("observaciones")]
        public string Observaciones { get; set; }

        [JsonPropertyName("nombre_paciente")]
        public string NombrePaciente { get; set; }

        [JsonPropertyName("cedula_paciente")]
        public string CedulaPaciente { get; set; }

        [JsonPropertyName("telefono_paciente")]
        public string TelefonoPaciente { get; set; }

        [JsonPropertyName("email_paciente")]
        public string EmailPaciente { get; set; }

        [JsonPropertyName("nombre_especialidad")]
        public string NombreEspecialidad { get; set; }

        [JsonPropertyName("nombre_sucursal")]
        public string NombreSucursal { get; set; }
    }

    // Modelos para cambiar estado y crear receta
    public class ChangeAppointmentStatusRequest
    {
        [JsonPropertyName("nuevo_estado")]
        public string NuevoEstado { get; set; }

        [JsonPropertyName("observaciones")]
        public string Observaciones { get; set; }
    }

    public class CreatePrescriptionRequest
    {
        [JsonPropertyName("id_cita")]
        public int IdCita { get; set; }

        [JsonPropertyName("medicamento")]
        public string Medicamento { get; set; }

        [JsonPropertyName("concentracion")]
        public string Concentracion { get; set; }

        [JsonPropertyName("forma_farmaceutica")]
        public string FormaFarmaceutica { get; set; }

        [JsonPropertyName("dosis")]
        public string Dosis { get; set; }

        [JsonPropertyName("frecuencia")]
        public string Frecuencia { get; set; }

        [JsonPropertyName("duracion")]
        public string Duracion { get; set; }

        [JsonPropertyName("cantidad")]
        public string Cantidad { get; set; }

        [JsonPropertyName("indicaciones_especiales")]
        public string IndicacionesEspeciales { get; set; }
    }
}