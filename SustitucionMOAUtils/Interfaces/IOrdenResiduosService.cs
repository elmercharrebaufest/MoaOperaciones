using SustitucionMOAModel.Dto.OrdenResiduos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IOrdenResiduosService
    {
        ListarOrdenesResiduosResponse ObtenerListadoOrdenes();
    }
}
