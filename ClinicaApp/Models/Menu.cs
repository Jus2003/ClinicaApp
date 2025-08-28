using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicaApp.Models
{
    public class Menu
    {
        public int IdMenu { get; set; }
        public string NombreMenu { get; set; }
        public string Icono { get; set; }
        public int Orden { get; set; }
        public List<SubMenu> Submenus { get; set; } = new List<SubMenu>();
    }

    public class SubMenu
    {
        public int IdSubmenu { get; set; }
        public string NombreSubmenu { get; set; }
        public string UriSubmenu { get; set; }
        public string Icono { get; set; }
    }
}