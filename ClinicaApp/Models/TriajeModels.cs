using System.ComponentModel;
using System.Collections.ObjectModel;

namespace ClinicaApp.Models
{
    public class PreguntaTriaje : INotifyPropertyChanged
    {
        public int IdPregunta { get; set; }
        public string Pregunta { get; set; }
        public string TipoPregunta { get; set; }
        public List<string> Opciones { get; set; } = new List<string>();
        public bool Obligatoria { get; set; }
        public int Orden { get; set; }

        private string _respuesta;
        public string Respuesta
        {
            get => _respuesta;
            set
            {
                _respuesta = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TieneRespuesta));
            }
        }

        private double? _valorNumerico;
        public double? ValorNumerico
        {
            get => _valorNumerico;
            set
            {
                _valorNumerico = value;
                OnPropertyChanged();
            }
        }

        public bool TieneRespuesta => !string.IsNullOrEmpty(Respuesta);

        // Propiedades para UI
        public bool EsTexto => TipoPregunta == "texto";
        public bool EsNumero => TipoPregunta == "numero";
        public bool EsEscala => TipoPregunta == "escala";
        public bool EsMultiple => TipoPregunta == "multiple";
        public bool EsSiNo => TipoPregunta == "sino";

        public string TituloConAsterisco => Obligatoria ? $"{Pregunta} *" : Pregunta;
        public Color TituloColor => Obligatoria ? Color.FromArgb("#D32F2F") : Color.FromArgb("#333333");

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public bool EsSiNoYTieneRespuesta => EsSiNo && !string.IsNullOrEmpty(Respuesta);
        // Agregar dentro de la clase PreguntaTriaje

    }

    public class RespuestaTriaje
    {
        public int IdPregunta { get; set; }
        public string Respuesta { get; set; }
        public double? ValorNumerico { get; set; }
    }

    public class TriajeCompleto
    {
        public int IdCita { get; set; }
        public bool TriajeRealizado { get; set; }
        public bool EsCompleto { get; set; } // 👈 CAMBIAR DE TriajeCompleto a EsCompleto
        public List<RespuestaTriajeDetallada> Respuestas { get; set; } = new List<RespuestaTriajeDetallada>();
        public EstadisticasTriaje Estadisticas { get; set; }
        public InfoCitaTriaje InfoCita { get; set; }
    }

    public class RespuestaTriajeDetallada
    {
        public int IdRespuesta { get; set; }
        public int IdCita { get; set; }
        public int IdPregunta { get; set; }
        public string Respuesta { get; set; }
        public double? ValorNumerico { get; set; }
        public string FechaRespuesta { get; set; }
        public string TipoTriaje { get; set; }
        public string UsuarioRegistro { get; set; }
        public string Pregunta { get; set; }
        public string TipoPregunta { get; set; }
        public List<string> Opciones { get; set; } = new List<string>();

        // Para mostrar en UI
        public string PreguntaFormateada => $"• {Pregunta}";
        public string RespuestaFormateada => ValorNumerico.HasValue ?
            $"{Respuesta} ({ValorNumerico})" : Respuesta;
        public Color TipoColor => TipoPregunta switch
        {
            "texto" => Color.FromArgb("#2196F3"),
            "numero" => Color.FromArgb("#FF9800"),
            "escala" => Color.FromArgb("#4CAF50"),
            "multiple" => Color.FromArgb("#9C27B0"),
            "sino" => Color.FromArgb("#607D8B"),
            _ => Color.FromArgb("#666666")
        };
    }

    public class EstadisticasTriaje
    {
        public int TotalPreguntasRespondidas { get; set; }
        public int ObligatoriasRespondidas { get; set; }
        public string TipoTriaje { get; set; }
        public string FechaInicio { get; set; }
        public string FechaFin { get; set; }
    }

    public class InfoCitaTriaje
    {
        public string FechaCita { get; set; }
        public string HoraCita { get; set; }
        public string EstadoCita { get; set; }
        public string Paciente { get; set; }
        public string Medico { get; set; }
        public string Especialidad { get; set; }
    }


}