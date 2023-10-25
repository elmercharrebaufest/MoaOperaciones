using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using System.Collections.Generic;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IGestionImpuestosService
    {
        IList<IngresosBrutosCoeficienteUnificadoDto> ListarCabeceras();
        IList<IngresosBrutosCoeficienteUnificadoDetalleDto> ListarDetalles(int idCabecera);
        EditarIngresosBrutosCoeficienteUnificadoDetalleResponseDto EditarIngresosBrutosCoeficienteUnificadoDetalle(IngresosBrutosCoeficienteUnificadoDetalleDto ingresosBrutosCoeficienteUnificadoDetalleDto);
        IList<MovimientoIngresosBrutosCoeficienteUnificadoDto> InsertarMovimientoIngresosBrutosCoeficienteUnificado(MovimientoIngresosBrutosCoeficienteUnificadoCustomDto movimientoIngresosBrutosCoeficienteUnificadoCustomDto);
        EditarIngresosBrutosCoeficienteUnificadoResponseDto EditarIngresosBrutosCoeficienteUnificado(IngresosBrutosCoeficienteUnificadoDto ingresosBrutosCoeficienteUnificadoDto);
        string AutorizarCabecera(int idCabecera, string mailUsuario);
        string ObtenerRutaArchivoFormularioCM05(int idCabecera);
        IList<MovimientoIngresosBrutosCoeficienteUnificadoDto> ListarMovimientos(int idCabecera);
        List<EstadoIngresosBrutosCoeficienteUnificadoDto> ListarEstados();
        List<SecuenciaIngresosBrutosCoeficienteUnificadoDto> ListarSecuenciaIngresosBrutosCoeficientesUnificador();
        void ActualizarCM05(int consultaId, Usuario usuario);
    }
}