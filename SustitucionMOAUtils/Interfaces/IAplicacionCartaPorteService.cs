using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.AplicacionCartaPorte;
using SustitucionMOAModel.Entities;
using System.Collections.Generic;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IAplicacionCartaPorteService
    {
        List<AplicacionCartaPorteDto> Listar(string mailUsuario, string fechaInicio, string fechaFin);
        AplicacionCartaPorteFiltrosDto ObtenerFiltros(List<AplicacionCartaPorteDto> aplicaciones);
        AplicacionCartaPorteDto Obtener(int aplicacionCCPPId,string mailUsuario);
        void EliminarAplicacion(int aplicacionId);
        ComboAplicacionesContratosCcppResponse ObtenerCombosDeContratoCCPP(string mailUsuario, string codigoProveedor, bool esCodigoCorredor);
        void GuardarAplicacion(CrearAplicacionCartaPorte aplicacionACrear, string mailUsuario);

        CargaMasivaResponse ProcesarCargaMasiva(HttpPostedFileBase archivo, string usuarioMail, string proveedorCodigo, bool esCodigoCorredor);
        void AprobarAplicacionPendiente(int idAplicacion);
        void RechazarAplicacionPendiente(int idAplicacion, string motivo);
    }
}
