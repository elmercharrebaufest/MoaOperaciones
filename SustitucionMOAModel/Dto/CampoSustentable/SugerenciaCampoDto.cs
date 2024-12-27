using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAModel.Dto.CampoSustentable
{
    public class SugerenciaCampoDto : CampoProveedorDto
    {
        public bool CampoYaPresentado { get; set; }

        public string NombreNuevoKmz { get; set; }
    }
}
