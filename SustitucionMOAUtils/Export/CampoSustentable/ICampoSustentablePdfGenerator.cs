using SustitucionMOAModel.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Export.CampoSustentable
{
    public interface ICampoSustentablePdfGenerator
    {
        byte[] GenerarDeclaracionJurada(DeclaracionCampoSustentableDto datos);
        byte[] GenerarDeclaracionJuradaListaCampos(DeclaracionCampoSustentableDto declaracionCS);
    }
}
