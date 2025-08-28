using System.Text.Json.Serialization;
using ClinicaApp.Helpers;

namespace ClinicaApp.Models
{
    public class MedicoBasico
    {
        [JsonPropertyName("id_medico")]
        [JsonConverter(typeof(StringToIntConverter))]
        public int IdMedico { get; set; }

        [JsonPropertyName("cedula")]
        public string Cedula { get; set; }

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

        [JsonPropertyName("activo")]
        public int Activo { get; set; }

        [JsonPropertyName("sucursal")]
        public string Sucursal { get; set; }

        [JsonPropertyName("especialidades")]
        public string Especialidades { get; set; }
    }

    public class Horario
    {
        [JsonPropertyName("id_horario")]
        [JsonConverter(typeof(StringToIntConverter))]
        public int IdHorario { get; set; }

        [JsonPropertyName("id_medico")]
        [JsonConverter(typeof(StringToIntConverter))]
        public int IdMedico { get; set; }

        [JsonPropertyName("id_sucursal")]
        [JsonConverter(typeof(StringToIntConverter))]
        public int IdSucursal { get; set; }

        [JsonPropertyName("nombre_sucursal")]
        public string NombreSucursal { get; set; }

        [JsonPropertyName("dia_semana")]
        [JsonConverter(typeof(StringToIntConverter))]
        public int DiaSemana { get; set; }

        [JsonPropertyName("nombre_dia")]
        public string NombreDia { get; set; }

        [JsonPropertyName("hora_inicio")]
        public string HoraInicio { get; set; }

        [JsonPropertyName("hora_fin")]
        public string HoraFin { get; set; }

        [JsonPropertyName("activo")]
        [JsonConverter(typeof(StringToIntConverter))]
        public int Activo { get; set; }

        [JsonPropertyName("fecha_creacion")]
        public string FechaCreacion { get; set; }
    }

    public class HorarioRequest
    {
        [JsonPropertyName("dia_semana")]
        public int DiaSemana { get; set; }

        [JsonPropertyName("hora_inicio")]
        public string HoraInicio { get; set; }

        [JsonPropertyName("hora_fin")]
        public string HoraFin { get; set; }

        [JsonPropertyName("id_sucursal")]
        public int IdSucursal { get; set; }
    }

    public class AsignarHorariosRequest
    {
        [JsonPropertyName("horarios")]
        public List<HorarioRequest> Horarios { get; set; } = new List<HorarioRequest>();
    }

    public class HorariosResponse
    {
        [JsonPropertyName("id_medico")]
        [JsonConverter(typeof(StringToIntConverter))]
        public int IdMedico { get; set; }

        [JsonPropertyName("horarios")]
        public List<Horario> Horarios { get; set; } = new List<Horario>();
    }
}