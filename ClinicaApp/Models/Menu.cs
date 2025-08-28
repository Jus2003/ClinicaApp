using System.Text.Json.Serialization;

namespace ClinicaApp.Models
{
    public class Menu
    {
        [JsonPropertyName("id_menu")]
        public int IdMenu { get; set; }

        [JsonPropertyName("nombre_menu")]
        public string NombreMenu { get; set; }

        [JsonPropertyName("icono")]
        public string Icono { get; set; }

        [JsonPropertyName("orden")]
        public int Orden { get; set; }

        [JsonPropertyName("submenus")]
        public List<SubMenu> Submenus { get; set; } = new List<SubMenu>();
    }

    public class SubMenu
    {
        [JsonPropertyName("id_submenu")]
        public int IdSubmenu { get; set; }

        [JsonPropertyName("nombre_submenu")]
        public string NombreSubmenu { get; set; }

        [JsonPropertyName("uri_submenu")]
        public string UriSubmenu { get; set; }

        [JsonPropertyName("icono")]
        public string Icono { get; set; }
    }
}