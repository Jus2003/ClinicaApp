using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicaApp.Helpers
{
    using System.Globalization;

    namespace ClinicaApp.Helpers
    {
        public class DiaSemanaConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is int diaSemana)
                {
                    return diaSemana switch
                    {
                        1 => "Lunes",
                        2 => "Martes",
                        3 => "Miércoles",
                        4 => "Jueves",
                        5 => "Viernes",
                        6 => "Sábado",
                        7 => "Domingo",
                        _ => "Desconocido"
                    };
                }
                return "Desconocido";
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }

}