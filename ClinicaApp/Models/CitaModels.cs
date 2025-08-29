using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicaApp.Models
{
    namespace ClinicaApp.Models
    {
        public class CitaRequest
        {
            public string CedulaPaciente { get; set; }
            public int IdMedico { get; set; }
            public int IdEspecialidad { get; set; }
            public int IdSucursal { get; set; }
            public string FechaCita { get; set; }
            public string HoraCita { get; set; }
            public string TipoCita { get; set; }
            public string MotivoConsulta { get; set; }
            public string? Observaciones { get; set; }
        }

        public class EspecialidadDisponible
        {
            public int IdEspecialidad { get; set; }
            public string NombreEspecialidad { get; set; }
            public string Descripcion { get; set; }
            public bool PermiteVirtual { get; set; }
            public bool PermitePresencial { get; set; }
            public int DuracionCitaMinutos { get; set; }
            public decimal PrecioConsulta { get; set; }
            public bool Activo { get; set; }
        }

        public class MedicoDisponible
        {
            public int IdMedico { get; set; }
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string NombreCompleto { get; set; }
            public string Email { get; set; }
            public string Telefono { get; set; }
            public string Sucursal { get; set; }
            public string Especialidad { get; set; }
            public string? NumeroLicencia { get; set; }
        }

        public class HorarioDisponible
        {
            public string Hora { get; set; }
            public string HoraFin { get; set; }
            public bool Disponible { get; set; }
            public int DuracionMinutos { get; set; }
        }

        public class ValidacionDisponibilidad
        {
            public bool Disponible { get; set; }
            public string FechaCita { get; set; }
            public string HoraCita { get; set; }
            public int IdMedico { get; set; }
            public string? Razon { get; set; }
        }

        public class CitaCreada
        {
            public string CitaCreada1 { get; set; }
            public string IdCita { get; set; }
            public string NumeroCita { get; set; }
            public PacienteCita Paciente { get; set; }
            public MedicoCita Medico { get; set; }
            public DetallesCita DetallesCita { get; set; }
            public InformacionVirtual? InformacionVirtual { get; set; }
            public Notificaciones Notificaciones { get; set; }
            public ProximosPasos ProximosPasos { get; set; }
            public string Timestamp { get; set; }
        }

        public class PacienteCita
        {
            public int Id { get; set; }
            public string NombreCompleto { get; set; }
            public string Cedula { get; set; }
            public string Email { get; set; }
        }

        public class MedicoCita
        {
            public int Id { get; set; }
            public string NombreCompleto { get; set; }
            public string Especialidad { get; set; }
            public string Email { get; set; }
        }

        public class DetallesCita
        {
            public string Fecha { get; set; }
            public string Hora { get; set; }
            public string Tipo { get; set; }
            public string Motivo { get; set; }
            public string? Observaciones { get; set; }
            public string Estado { get; set; }
            public string Sucursal { get; set; }
        }

        public class InformacionVirtual
        {
            public string EnlaceZoom { get; set; }
            public string IdReunion { get; set; }
            public string Password { get; set; }
            public string Instrucciones { get; set; }
        }

        public class Notificaciones
        {
            public string EmailPaciente { get; set; }
            public string EmailMedico { get; set; }
            public string Mensaje { get; set; }
        }

        public class ProximosPasos
        {
            public string ParaPaciente { get; set; }
            public string Recordatorio { get; set; }
        }
    }
}
