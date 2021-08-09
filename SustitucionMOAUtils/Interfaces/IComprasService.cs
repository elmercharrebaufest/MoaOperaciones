using SustitucionMOAModel.Dto;
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
        SolpDto GuardarSolp(SolpDto solp, HttpFileCollectionBase adjuntos);
        string ObtenerRutaArchivo(int archivoId);

        List<TablaSapDto> ObtenerTablaSap(string tabla);
        List<TablaGeneralDto> ObtenerTablaGeneral(string tabla);
        List<CentroDireccionDto> ObtenerCentrosDireccion();
        List<SolpDto> ListarSolp();
        string BorrarSolp(int idSolp);
        SolpDto TraerSolpId(int idSolp);
        List<TablaEstadoDto> ObtenerTablaEstado(string tabla);
        byte[] GenerarSolpPdf(int idSolp);

        string GenerarZipPliego(int idSolp, string pathBase);

        ObtenerServiciosSolpConsumerMOA test();

    }
}
