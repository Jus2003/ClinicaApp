using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ClinicaApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Cedula { get; set; }
        public string Telefono { get; set; }
        public string Rol { get; set; }
        public string Sucursal { get; set; }

        [JsonPropertyName("id_rol")]
        public int IdRol { get; set; }
    }
}