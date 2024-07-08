using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.Compras
{
    public class AdjuntosSolpDto
    {
        public string Pliego { get; set; }
        public string PDF { get; set; }
        public List<ArchivoDto> ArchivosEspecificacionesTecnicas { get; set; } = new List<ArchivoDto>();
        public List<ArchivoDto> ArchivosCotizacion { get; set; } = new List<ArchivoDto>();
        public List<ArchivoDto> ArchivosCondicionesEspeciales { get; set; } = new List<ArchivoDto>();
        public string ObservacionCondicionesEspeciales { get; set; }
    }
}
