using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class LegajoDto
    {        
        public int SolpId { get; set; }
        public int PeticionDeOfertaId { get; set; }        
        public DateTime Fecha { get; set; }
        public string Observacion { get; set; }
        public int? UsuarioProveedorId { get; set; }
        public UsuarioDto Proveedor { get; set; }
        public int? ArchivoId { get; set; }
        public string FechaFormateado { get; set; }
        public bool Leido { get; set; } = true;

    }
}
