using Newtonsoft.Json;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.AplicacionCartaPorte;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    [Authorize]
    public class AplicacionCartaPorteController : BaseController
    {
        readonly IAplicacionCartaPorteService aplicacionCCPPService;

        public AplicacionCartaPorteController(IAplicacionCartaPorteService aplicacionCCPPService)
        {
            this.aplicacionCCPPService = aplicacionCCPPService;
        }

        [HttpGet]
        public ActionResult GetListado(string fechaInicio, string fechaFin)
        {
            var mailUsuario = SessionPersister.Mail;
            var data = aplicacionCCPPService.Listar(mailUsuario, fechaInicio, fechaFin);
            if (!(data.Count > 0))
                throw new InfoCustomException("No se han encontrado aplicaciones cargadas");
            var filtros = aplicacionCCPPService.ObtenerFiltros(data);
            return JsonCustom(new { data, filtros });

        }

        [HttpGet]
        public ActionResult Get(int aplicacionCCPPId)
        {

            var mailUsuario = SessionPersister.Mail;

            return JsonCustom(new { data = aplicacionCCPPService.Obtener(aplicacionCCPPId, mailUsuario) });

        }

        [HttpGet]
        public ContentResult EliminarAplicacion(int aplicacionId)
        {
            var response = new SustitucionMOAApiResponse<bool>();

            aplicacionCCPPService.EliminarAplicacion(aplicacionId);
            response.Data = true;
            return ContentCustom(response);
        }

        public ContentResult ObtenerComboContratosCcpp()
        {
            var response = new SustitucionMOAApiResponse<ComboAplicacionesContratosCcppResponse>();

            var mailUsuario = SessionPersister.Mail;
            var codigoProveedor = SessionPersister.Proveedor;
            var esCodigoCorredor = SessionPersister.EsCodigoDeCorredor;
            response.Data = aplicacionCCPPService.ObtenerCombosDeContratoCCPP(mailUsuario, codigoProveedor, esCodigoCorredor);

            return ContentCustom(response);
        }
        [HttpPost]
        public ContentResult GuardarAplicacion(string aplicacionCCPPJSON)
        {
            var response = new SustitucionMOAApiResponse<bool>();

            var aplicacionACrear = JsonConvert.DeserializeObject<CrearAplicacionCartaPorte>(aplicacionCCPPJSON);
            var mailUsuario = SessionPersister.Mail;
            var codigoProveedor = SessionPersister.Proveedor;
            var esCodigoCorredor = SessionPersister.EsCodigoDeCorredor;
            aplicacionCCPPService.GuardarAplicacion(aplicacionACrear, mailUsuario, codigoProveedor, esCodigoCorredor);
            response.Data = true;

            return ContentCustom(response);
        }

        [HttpPost]
        public ContentResult CargarMasiva(HttpPostedFileBase archivo)
        {
            var response = new SustitucionMOAApiResponse<CargaMasivaResponse>();
            var mailUsuario = SessionPersister.Mail;
            var codigoProveedor = SessionPersister.Proveedor;
            var esCodigoCorredor = SessionPersister.EsCodigoDeCorredor;
            response.Data = aplicacionCCPPService.ProcesarCargaMasiva(archivo, mailUsuario, codigoProveedor, esCodigoCorredor);

            return ContentCustom(response);
        }

        [HttpPost]
        public ContentResult AprobarAplicacionesPendientes(List<int> idsAplicaciones)
        {
            var response = new SustitucionMOAApiResponse<bool> { Data = true };
            aplicacionCCPPService.AprobarAplicacionesPendientes(idsAplicaciones);

            return ContentCustom(response);
        }

        [HttpPost]
        public ContentResult RechazarAplicacionesPendientes(List<int> idsAplicaciones, string motivo)
        {
            var response = new SustitucionMOAApiResponse<bool> { Data = true };

            aplicacionCCPPService.RechazarAplicacionesPendientes(idsAplicaciones, motivo);

            return ContentCustom(response);
        }
    }
}