// Models/TriajeModels.cs
using System.ComponentModel;
using System.Text.Json.Serialization;
using static ClinicaApp.Models.CitaDetalle;
using ClinicaApp.Helpers;

namespace ClinicaApp.Models
{
    // Modelo para las preguntas del triaje
    public class PreguntaTriaje : INotifyPropertyChanged
    {
        private string _respuesta = "";

        [JsonPropertyName("id_pregunta")]
        public int IdPregunta { get; set; }

        [JsonPropertyName("pregunta")]
        public string Pregunta { get; set; }

        [JsonPropertyName("tipo_pregunta")]
        public string TipoPregunta { get; set; }

        [JsonPropertyName("obligatoria")]
        public int Obligatoria { get; set; }

        [JsonPropertyName("orden")]
        public int Orden { get; set; }

        [JsonPropertyName("activo")]
        public int Activo { get; set; }

        [JsonPropertyName("opciones")]
        public object Opciones { get; set; }

        // ✅ Hacer que Respuesta notifique cambios
        public string Respuesta
        {
            get => _respuesta;
            set
            {
                _respuesta = value;
                OnPropertyChanged();
            }
        }

        public bool EsObligatoria => Obligatoria == 1;
        public List<string> OpcionesLista { get; set; } = new();
        public OpcionesEscala OpcionesEscalaObj { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class OpcionesEscala
    {
        [JsonPropertyName("max")]
        public int Max { get; set; }

        [JsonPropertyName("min")]
        public int Min { get; set; }

        [JsonPropertyName("unidad")]
        public string Unidad { get; set; }

        [JsonPropertyName("etiquetas")]
        public Dictionary<string, string> Etiquetas { get; set; } = new();
    }

    // Modelo para respuestas del triaje
    public class RespuestaTriaje
    {
        [JsonPropertyName("id_pregunta")]
        public int IdPregunta { get; set; }

        [JsonPropertyName("respuesta")]
        public string Respuesta { get; set; }

        [JsonPropertyName("valor_numerico")]
        public decimal? ValorNumerico { get; set; }
    }

    // Modelo para solicitud de triaje
    public class TriajeRequest
    {
        [JsonPropertyName("id_cita")]
        public int IdCita { get; set; }

        [JsonPropertyName("tipo_triaje")]
        public string TipoTriaje { get; set; } = "digital";

        [JsonPropertyName("respuestas")]
        public List<RespuestaTriaje> Respuestas { get; set; } = new();
    }

    // Modelo para la respuesta completa del triaje
    // En Models/TriajeModels.cs, actualizar RespuestaTriajeCompleta:

    public class RespuestaTriajeCompleta
    {
        [JsonPropertyName("id_respuesta")]
        public int IdRespuesta { get; set; }

        [JsonPropertyName("id_cita")]
        public int IdCita { get; set; }

        [JsonPropertyName("id_pregunta")]
        public int IdPregunta { get; set; }

        [JsonPropertyName("respuesta")]
        public string Respuesta { get; set; }

        [JsonPropertyName("valor_numerico")]
        [JsonConverter(typeof(StringToNullableDecimalConverter))] // ✅ AGREGAR CONVERTER
        public decimal? ValorNumerico { get; set; }

        [JsonPropertyName("fecha_respuesta")]
        public string FechaRespuesta { get; set; }

        [JsonPropertyName("tipo_triaje")]
        public string TipoTriaje { get; set; }

        [JsonPropertyName("pregunta")]
        public string Pregunta { get; set; }

        [JsonPropertyName("tipo_pregunta")]
        public string TipoPregunta { get; set; }

        [JsonPropertyName("opciones")]
        public object Opciones { get; set; }

        [JsonPropertyName("usuario_registro")]
        public string UsuarioRegistro { get; set; }
    }

    // Modelo para el estado del triaje
    public class EstadoTriaje
    {
        [JsonPropertyName("id_cita")]
        public string IdCita { get; set; }

        [JsonPropertyName("estado_cita")]
        public string EstadoCita { get; set; }

        [JsonPropertyName("triaje_realizado")]
        public bool TriajeRealizado { get; set; }

        [JsonPropertyName("triaje_completo")]
        public bool TriajeCompleto { get; set; }

        [JsonPropertyName("puede_realizar_triaje")]
        public bool PuedeRealizarTriaje { get; set; }

        [JsonPropertyName("estadisticas")]
        public EstadisticasTriaje Estadisticas { get; set; }

        [JsonPropertyName("info_cita")]
        public InfoCitaTriaje InfoCita { get; set; }
    }

    public class EstadisticasTriaje
    {
        [JsonPropertyName("total_preguntas_respondidas")]
        public int TotalPreguntasRespondidas { get; set; }

        [JsonPropertyName("obligatorias_respondidas")]
        public int ObligatoriasRespondidas { get; set; }

        [JsonPropertyName("tipo_triaje")]
        public string TipoTriaje { get; set; }

        [JsonPropertyName("fecha_inicio")]
        public string FechaInicio { get; set; }

        [JsonPropertyName("fecha_fin")]
        public string FechaFin { get; set; }
    }

    public class InfoCitaTriaje
    {
        [JsonPropertyName("fecha_cita")]
        public string FechaCita { get; set; }

        [JsonPropertyName("hora_cita")]
        public string HoraCita { get; set; }

        [JsonPropertyName("paciente")]
        public string Paciente { get; set; }

        [JsonPropertyName("medico")]
        public string Medico { get; set; }

        [JsonPropertyName("especialidad")]
        public string Especialidad { get; set; }
    }

    // Modelos para citas del paciente
    public class CitasPacienteRequest
    {
        [JsonPropertyName("id_paciente")]
        public int IdPaciente { get; set; }
    }

    public class CitaDetalle
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

        [JsonPropertyName("nombre_medico")]
        public string NombreMedico { get; set; }

        [JsonPropertyName("nombre_especialidad")]
        public string NombreEspecialidad { get; set; }

        [JsonPropertyName("nombre_sucursal")]
        public string NombreSucursal { get; set; }

        // Propiedades auxiliares para la UI
        public string FechaFormateada => DateTime.TryParse(FechaCita, out var fecha)
            ? fecha.ToString("dd/MM/yyyy") : FechaCita;

        public string HoraFormateada => TimeSpan.TryParse(HoraCita, out var hora)
            ? hora.ToString(@"hh\:mm") : HoraCita;

        public Color EstadoColor => EstadoCita?.ToLower() switch
        {
            "agendada" => Colors.Orange,
            "completada" => Colors.Green,
            "cancelada" => Colors.Red,
            _ => Colors.Gray
        };

        // Agregar estos modelos al archivo TriajeModels.cs

        public class CitasPacienteResponse
        {
            [JsonPropertyName("id_paciente_consultado")]
            public int IdPacienteConsultado { get; set; }

            [JsonPropertyName("timestamp")]
            public string Timestamp { get; set; }

            [JsonPropertyName("citas")]
            public List<CitaDetalle> Citas { get; set; } = new();
        }

        public class CitaDetalladaResponse
        {
            [JsonPropertyName("id_cita_consultada")]
            public int IdCitaConsultada { get; set; }

            [JsonPropertyName("timestamp")]
            public string Timestamp { get; set; }

            [JsonPropertyName("cita")]
            public CitaCompleta Cita { get; set; }
        }

        public class CitaCompleta
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

            [JsonPropertyName("nombre_medico")]
            public string NombreMedico { get; set; }

            [JsonPropertyName("nombre_especialidad")]
            public string NombreEspecialidad { get; set; }

            [JsonPropertyName("nombre_sucursal")]
            public string NombreSucursal { get; set; }

            [JsonPropertyName("direccion_sucursal")]
            public string DireccionSucursal { get; set; }

            // Propiedades auxiliares para la UI
            public string FechaFormateada => DateTime.TryParse(FechaCita, out var fecha)
                ? fecha.ToString("dd 'de' MMMM 'de' yyyy") : FechaCita;

            public string HoraFormateada => TimeSpan.TryParse(HoraCita, out var hora)
                ? hora.ToString(@"hh\:mm") : HoraCita;
        }

        public class TriajeCompletadoResponse
        {
            [JsonPropertyName("id_cita")]
            public string IdCita { get; set; }

            [JsonPropertyName("triaje_realizado")]
            public bool TriajeRealizado { get; set; }

            [JsonPropertyName("triaje_completo")]
            public bool TriajeCompleto { get; set; }

            [JsonPropertyName("respuestas")]
            public List<RespuestaTriajeCompleta> Respuestas { get; set; } = new(); // ✅ Usar RespuestaTriajeCompleta

            [JsonPropertyName("estadisticas")]
            public EstadisticasTriaje Estadisticas { get; set; }

            [JsonPropertyName("info_cita")]
            public InfoCitaTriaje InfoCita { get; set; }
        }

        public class RespuestaTriajeDetallada
        {
            [JsonPropertyName("id_respuesta")]
            public int IdRespuesta { get; set; }

            [JsonPropertyName("id_cita")]
            public int IdCita { get; set; }

            [JsonPropertyName("id_pregunta")]
            public int IdPregunta { get; set; }

            [JsonPropertyName("respuesta")]
            public string Respuesta { get; set; }

            [JsonPropertyName("valor_numerico")]
            public decimal? ValorNumerico { get; set; }

            [JsonPropertyName("fecha_respuesta")]
            public string FechaRespuesta { get; set; }

            [JsonPropertyName("tipo_triaje")]
            public string TipoTriaje { get; set; }

            [JsonPropertyName("pregunta")]
            public string Pregunta { get; set; }

            [JsonPropertyName("tipo_pregunta")]
            public string TipoPregunta { get; set; }

            [JsonPropertyName("opciones")]
            public object Opciones { get; set; }
        }
    }
}