using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IGestionImpuestosService
    {
        IList<IngresosBrutosCoeficienteUnificadoDto> ListarCabeceras();
        
        IList<IngresosBrutosCoeficienteUnificadoDetalleDto> ListarDetalles(int idCabecera);

        EditarIngresosBrutosCoeficienteUnificadoDetalleResponseDto EditarIngresosBrutosCoeficienteUnificadoDetalle(IngresosBrutosCoeficienteUnificadoDetalleDto ingresosBrutosCoeficienteUnificadoDetalleDto);

        string AutorizarCabecera(int idCabecera);

        string ObtenerRutaArchivoFormularioCM05(int idCabecera);
    }
}