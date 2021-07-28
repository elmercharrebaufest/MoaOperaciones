using SustitucionMOAModel.Dto;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IGestionImpuestosService
    {
        IList<IngresosBrutosCoeficienteUnificadoDto> ListarCabeceras();
        
        IList<IngresosBrutosCoeficienteUnificadoDetalleDto> ListarDetalles(int idCabecera);

        void AutorizarCabecera(int idCabecera);
    }
}