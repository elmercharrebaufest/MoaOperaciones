using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IGestionImpuestosService
    {
        IList<IngresosBrutosCoeficienteUnificadoDto> ListarCabeceras();
        
        IList<IngresosBrutosCoeficienteUnificadoDetalleDto> ListarDetalles(int idCabecera);

        string EditarDetalles(IngresosBrutosCoeficienteUnificadoDetalleDto coeficientes);

        string AutorizarCabecera(int idCabecera);
    }
}