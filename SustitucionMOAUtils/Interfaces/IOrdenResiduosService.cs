using SustitucionMOAModel.Dto.OrdenResiduos;
using SustitucionMOAModel.Models.DataAgro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IOrdenResiduosService
    {
        MaterialDto[] ObtenerMateriales();

        ListarOrdenesResiduosResponse ObtenerListadoOrdenes(string fechaInicio, string fechaFin);
        void VerificarVencimientoOrdenesResiduos();
    }
}
