using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAWS.WSConsumers;
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
        RespuestaGuardarSOLP GuardarSolp(SolpDto solp, HttpFileCollectionBase adjuntos);
        string ObtenerRutaArchivo(int archivoId);

        List<TablaSapDto> ObtenerTablaSap(string tabla);
        List<TablaGeneralDto> ObtenerTablaGeneral(string tabla);
        List<CentroDireccionDto> ObtenerCentrosDireccion();
        List<SolpDto> ListarSolp(UsuarioDto usuarioActual);
        string BorrarSolp(int idSolp);
        SolpDto TraerSolpId(int idSolp);
        List<TablaEstadoDto> ObtenerTablaEstado(string tabla);
        byte[] GenerarSolpPdf(int idSolp);
        string GenerarZipPliego(int idSolp, string pathBase);
        List<TablaSapDto> ObtenerServiciosSap();
        List<TablaSapDto> AutocompleteTablaSap(string tabla, string valor);
        List<TablaSapDto> ObtenerCecoSap();
        List<TablaSapDto> ObtenerCuentasSap();
        List<TablaSapDto> ObtenerOrdenesSap();
        List<TablaSapDto> ObtenerDatosPorCodigosSap(List<TablaSapDto> codigos);
        void ActualizarFechaLiberacion(string nrosolp, DateTime fechaLiberacion);
    }
}
