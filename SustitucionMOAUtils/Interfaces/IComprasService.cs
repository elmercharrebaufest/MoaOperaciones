using SustitucionMOAModel.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IComprasService
    {
        SolpDto GuardarSolp(SolpDto solp, HttpFileCollectionBase adjuntos);

    }
}
