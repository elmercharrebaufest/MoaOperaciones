using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class AsociarContratoDto
    {
        public string Codigo { get; set; }
        public List<FuenteAprovisionamientoDto> ContratosAsociados { get; set; }
        public string Centro { get; set; }
        public string ContratoMarco { get; set; }
        public int? Indice { get; set; }
        public string Tarea { get; set; }
        public string Proveedor { get; set; }
    }
}
