using System.Text.Json.Serialization;
using ClinicaApp.Helpers;

namespace ClinicaApp.Models
{
    public class Doctor
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("cedula")]
        public string Cedula { get; set; }

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; }

        [JsonPropertyName("apellido")]
        public string Apellido { get; set; }

        [JsonPropertyName("fecha_nacimiento")]
        public string FechaNacimiento { get; set; }

        [JsonPropertyName("genero")]
        public string Genero { get; set; }

        [JsonPropertyName("telefono")]
        public string Telefono { get; set; }

        [JsonPropertyName("direccion")]
        public string Direccion { get; set; }

        [JsonPropertyName("id_sucursal")]
        public int IdSucursal { get; set; }

        [JsonPropertyName("especialidades")]
        public List<int> Especialidades { get; set; } = new List<int>();
    }

    public class Patient
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("cedula")]
        public string Cedula { get; set; }

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; }

        [JsonPropertyName("apellido")]
        public string Apellido { get; set; }

        [JsonPropertyName("fecha_nacimiento")]
        public string FechaNacimiento { get; set; }

        [JsonPropertyName("genero")]
        public string Genero { get; set; }

        [JsonPropertyName("telefono")]
        public string Telefono { get; set; }

        [JsonPropertyName("direccion")]
        public string Direccion { get; set; }
    }

    public class Especialidad
    {
        [JsonPropertyName("id_especialidad")]
        public int IdEspecialidad { get; set; }

        [JsonPropertyName("nombre_especialidad")]  // ← Corregido
        public string NombreEspecialidad { get; set; }

        [JsonPropertyName("descripcion")]
        public string Descripcion { get; set; }

        [JsonPropertyName("activo")]
        public bool Activo { get; set; }
    }

    public class Sucursal
    {
        [JsonPropertyName("id_sucursal")]
        public int IdSucursal { get; set; }

        [JsonPropertyName("nombre_sucursal")]
        public string NombreSucursal { get; set; }

        [JsonPropertyName("activo")]
        public bool Activo { get; set; }
    }

    public class DoctorResponse
    {
        [JsonPropertyName("id_medico")]
        [JsonConverter(typeof(StringToIntConverter))]  // Agregar este converter
        public int IdMedico { get; set; }

        [JsonPropertyName("nombre_completo")]
        public string NombreCompleto { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("password_temporal")]
        public string PasswordTemporal { get; set; }

        [JsonPropertyName("email_enviado")]
        public bool EmailEnviado { get; set; }

        [JsonPropertyName("especialidades_asignadas")]
        public int EspecialidadesAsignadas { get; set; }
    }

    public class PatientResponse
    {
        [JsonPropertyName("id_paciente")]
        [JsonConverter(typeof(StringToIntConverter))]  // Agregar el mismo converter
        public int IdPaciente { get; set; }

        [JsonPropertyName("nombre_completo")]
        public string NombreCompleto { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("password_temporal")]
        public string PasswordTemporal { get; set; }

        [JsonPropertyName("email_enviado")]
        public bool EmailEnviado { get; set; }
    }
}