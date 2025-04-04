using Newtonsoft.Json;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Pesificacion;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Services;
using System;
using System.Web;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    [Authorize]
    public class PesificacionController : BaseController
    {
        private readonly IPesificacionService pesificacionService;
        private readonly ILogPesificacionService logPesificacionService;
        private readonly IUsuarioService usuarioService;


        public PesificacionController(IPesificacionService pesificacionService, ILogPesificacionService servicio, IUsuarioService usuarioService)
        {
            this.pesificacionService = pesificacionService;
            this.logPesificacionService = servicio;
            this.usuarioService = usuarioService;
        }

        public JsonResult GetFechaPesificacion()
        {
            return JsonCustom(pesificacionService.GetFechaPesificacion("yyyy-MM-dd"));
        }

        public JsonResult GetSoja200()
        {
            return JsonCustom(pesificacionService.GetSoja200());
        }
        public JsonResult GetDolarGirasol()
        {
            var data = pesificacionService.GetDolarGirasol();
            return JsonCustom(new { data });
        }
        public JsonResult GetDolarMaiz()
        {
            var data = pesificacionService.GetDolarMaiz();
            return JsonCustom(new { data });
        }
        [CustomPermisoAuthorizeAttribute(Roles = Permiso.PESIFICACION)]
        public ActionResult SetComprobante(string contrato)
        {
            Log.Info($"{System.Web.HttpContext.Current.Request.UserHostAddress}, {SessionPersister.Mail}, {this.GetType().Name}, {System.Reflection.MethodBase.GetCurrentMethod().Name}, SetComprobante(string contrato)  {contrato ?? "null"})");
            var contratoJson = JsonConvert.DeserializeObject<ContratoContenido>(contrato);

            //registro MOAOperaciones el alta de una pesificacion
            var nuevaPesificacion = new LogPesificacion
            {
                IdUsuario = ObtenerUsuarioActual().Id,
                Fecha = DateTime.Now,
                CodigoProveedor = SessionPersister.Proveedor,
                Contrato = this.ParseContrato(contratoJson.Contrato),
                Fijacion = this.ParseFijacion(contratoJson.Fijacion),
                CantidadKilos = contratoJson.Cantidad
            };

            var logPesificacion = logPesificacionService.GuardarPesificacion(nuevaPesificacion);

            var respuestaDeContrato = pesificacionService.SetContrato(SessionPersister.Proveedor, contratoJson.Contrato, contratoJson.Fijacion, contratoJson.Cantidad);

            //si todo el proceso fue exitoso actualizo en MOAOperaciones el exitoso en el log
            logPesificacionService.ActualizarEstadoLogPesificacion(new LogPesificacion { Id = logPesificacion.Id, EnvioExitoso = true });

            return JsonCustom(respuestaDeContrato);
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.PESIFICACION)]
        public ActionResult SetComprobantes(HttpPostedFileBase file)
        {
            var nuevaPesificacion = new LogPesificacion
            {
                IdUsuario = ObtenerUsuarioActual().Id,
                Fecha = DateTime.Now,
                CodigoProveedor = SessionPersister.Proveedor,
            };

            var logPesificacion = logPesificacionService.GuardarPesificacionAutomatica(nuevaPesificacion, file);
            var envioSap = pesificacionService.SetContratos(SessionPersister.Proveedor, file);

            logPesificacionService.ActualizarEstadoLogPesificacion(new LogPesificacion { Id = logPesificacion.Id, EnvioExitoso = true });
            return JsonCustom(envioSap);
        }

        public ActionResult GetContratos()
        {
            return JsonCustom(pesificacionService.GetContratos(SessionPersister.Proveedor));
        }

        [HttpGet]
        public JsonResult PesificacionesSAP()
        {
            return JsonCustom(new { data = pesificacionService.GetPesificacionesSAP(SessionPersister.Proveedor) });
        }

        private UsuarioDto ObtenerUsuarioActual()
        {
            string userMail = SessionPersister.Mail;
            return usuarioService.GetUsuario(userMail);
        }

        private int ParseContrato(string contrato)
        {
            int valor;
            if (int.TryParse(contrato, out valor))
            {
                return valor;
            }
            return 0;
        }

        private int? ParseFijacion(string fijacion)
        {
            if (string.IsNullOrEmpty(fijacion))
                return null;

            int valor;
            if (int.TryParse(fijacion, out valor))
            {
                return valor;
            }
            return 0;
        }
    }
}