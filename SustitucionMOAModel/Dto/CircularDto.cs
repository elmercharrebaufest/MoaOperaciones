using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Entities;

namespace SustitucionMOAModel.Dto
{
    public class CircularDto
    {      
        public int PeticionDeOferta { get; set; }
        public List<ArchivoDto> Adjuntos { get; set; }
        public int UsuarioId { get; set; }
        public string Observacion { get; set; }
        public List<int> UsuarioIds { get; set; }
        public bool? RequiereCambioDeFecha { get; set; }
        public DateTime? PlazoDeOferta { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public string Estado { get; set; }
        public string EstadoColor { get; set; }
        public int Estado_Id { get; set; }
    }
}
