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

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; }

        [JsonPropertyName("apellido")]
        public string Apellido { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("cedula")]
        public string Cedula { get; set; }

        [JsonPropertyName("telefono")]
        public string Telefono { get; set; }

        [JsonPropertyName("rol")]
        public string Rol { get; set; }

        [JsonPropertyName("sucursal")]
        public string Sucursal { get; set; }

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

        // Propiedades calculadas para UI
        public string FechaHoraFormateada => $"{FechaCita} {HoraCita}";
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

        [JsonPropertyName("proximas_citas")]
        public int ProximasCitas { get; set; }

        [JsonPropertyName("completadas")]
        public int Completadas { get; set; }
    }

    // Modelo para obtener detalles de cita específica
    public class AppointmentDetailRequest
    {
        [JsonPropertyName("id_cita")]
        public int IdCita { get; set; }
    }

    public class AppointmentDetailResponse
    {
        [JsonPropertyName("id_cita_consultada")]
        public int IdCitaConsultada { get; set; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }

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

        [JsonPropertyName("nombre_medico")]
        public string NombreMedico { get; set; }

        [JsonPropertyName("telefono_medico")]
        public string TelefonoMedico { get; set; }

        [JsonPropertyName("nombre_especialidad")]
        public string NombreEspecialidad { get; set; }

        [JsonPropertyName("nombre_sucursal")]
        public string NombreSucursal { get; set; }

        [JsonPropertyName("direccion_sucursal")]
        public string DireccionSucursal { get; set; }
    }

    // Modelo para cambiar estado de cita
    public class ChangeAppointmentStatusRequest
    {
        [JsonPropertyName("nuevo_estado")]
        public string NuevoEstado { get; set; }

        [JsonPropertyName("observaciones")]
        public string Observaciones { get; set; }

        [JsonPropertyName("motivo_cambio")]
        public string MotivoCambio { get; set; }
    }

    public class ChangeAppointmentStatusResponse
    {
        [JsonPropertyName("id_cita")]
        public int IdCita { get; set; }

        [JsonPropertyName("estado_anterior")]
        public string EstadoAnterior { get; set; }

        [JsonPropertyName("estado_nuevo")]
        public string EstadoNuevo { get; set; }

        [JsonPropertyName("observaciones_nuevas")]
        public string ObservacionesNuevas { get; set; }

        [JsonPropertyName("emails_enviados")]
        public EmailStatus EmailsEnviados { get; set; }

        [JsonPropertyName("cambios_realizados")]
        public ChangeDetails CambiosRealizados { get; set; }
    }

    public class EmailStatus
    {
        [JsonPropertyName("paciente")]
        public bool Paciente { get; set; }

        [JsonPropertyName("medico")]
        public bool Medico { get; set; }
    }

    public class ChangeDetails
    {
        [JsonPropertyName("estado_actualizado")]
        public bool EstadoActualizado { get; set; }

        [JsonPropertyName("observaciones_guardadas")]
        public bool ObservacionesGuardadas { get; set; }

        [JsonPropertyName("emails_paciente")]
        public string EmailsPaciente { get; set; }

        [JsonPropertyName("emails_medico")]
        public string EmailsMedico { get; set; }

        [JsonPropertyName("fecha_cambio")]
        public string FechaCambio { get; set; }
    }

    // Modelo para crear receta médica
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

    public class CreatePrescriptionResponse
    {
        [JsonPropertyName("id_receta_cita")]
        public int IdRecetaCita { get; set; }

        [JsonPropertyName("codigo_receta")]
        public string CodigoReceta { get; set; }

        [JsonPropertyName("medicamento")]
        public string Medicamento { get; set; }

        [JsonPropertyName("paciente")]
        public string Paciente { get; set; }

        [JsonPropertyName("medico")]
        public string Medico { get; set; }

        [JsonPropertyName("fecha_emision")]
        public string FechaEmision { get; set; }

        [JsonPropertyName("fecha_vencimiento")]
        public string FechaVencimiento { get; set; }

        [JsonPropertyName("email_enviado")]
        public bool EmailEnviado { get; set; }

        [JsonPropertyName("notificacion_creada")]
        public bool NotificacionCreada { get; set; }

        [JsonPropertyName("cita_asociada")]
        public AssociatedAppointment CitaAsociada { get; set; }
    }

    public class AssociatedAppointment
    {
        [JsonPropertyName("id_cita")]
        public int IdCita { get; set; }

        [JsonPropertyName("fecha_cita")]
        public string FechaCita { get; set; }

        [JsonPropertyName("especialidad")]
        public string Especialidad { get; set; }

        [JsonPropertyName("sucursal")]
        public string Sucursal { get; set; }
    }
}