using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class CampoProveedorListadoDto
    {
        public int Id { get; set; }

        public string NombreCampo { get; set; }

        public string NombreCosecha { get; set; }

        public double HectareasTotales { get; set; }

        public double HectareasSoja { get; set; }

        public double ToneladasAprobadas { get; set; }

        public double ToneladasPendientes { get; set; }

        public int CampoCosechaId { get; set; }

        public int ProveedorId { get; set; }
    }
}
