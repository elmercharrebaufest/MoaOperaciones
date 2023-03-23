using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Entities;

namespace SustitucionMOAModel.Dto
{
    public class GuardarPeticionDeOfertaDto
    {        
        public int SolpId { get; set; }
        public List<int> PosIds { get; set; }    
        public List<ArchivoDto> Adjuntos { get; set; }
        public string Observacion { get; set; }
        public List<int> UsuarioIds { get; set; }
        public UsuarioDto UsuarioActual { get; set; }

        public int Id { get; set; }
    }
}
