using Newtonsoft.Json;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.LogPesificacion;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Pesificacion;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
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
            if (file == null || file.ContentLength == 0)
                throw new ArgumentException("No se recibió ningún archivo.");

            var extension = System.IO.Path.GetExtension(file.FileName);
            if (!string.Equals(extension, ".csv", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("El archivo debe ser de tipo .csv");

            var contratosCSV = pesificacionService.LeerContratosCSV(file);
            if (!contratosCSV.Any())
                throw new ValidationCustomException("El archivo no contiene contratos válidos.");

            // 1. Validar filas con datos faltantes
            if (contratosCSV.Any(c => string.IsNullOrEmpty(c.Contrato) || string.IsNullOrEmpty(c.Correo) || c.Cantidad <= 0))
            {
                throw new ValidationCustomException("El archivo contiene filas con información faltante (Contrato, Correo o Kilos). Por favor, corrija el archivo.");
            }

            // 2. Crear logs para todos los contratos del CSV
            var usuarioActual = ObtenerUsuarioActual();
            var logsParaGuardar = contratosCSV.Select(cto => new LogPesificacion
            {
                IdUsuario = usuarioActual.Id,
                Fecha = DateTime.Now,
                CodigoProveedor = SessionPersister.Proveedor,
                Contrato = this.ParseContrato(cto.Contrato),
                Fijacion = this.ParseFijacion(cto.Fijacion),
                CantidadKilos = cto.Cantidad,
                EsCargaMasiva = true
            }).ToList();

            var logsGuardados = logPesificacionService.GuardarPesificaciones(logsParaGuardar);

            // 3. Procesar contratos y obtener respuesta
            var respuestaDeContrato = pesificacionService.SetContratos(SessionPersister.Proveedor, contratosCSV);

            // 4. Usar ContratosOk para identificar los logs exitosos
            var exitososKeys = new HashSet<string>(respuestaDeContrato.ContratosOk ?? new List<string>());

            var idsLogsExitosos = logsGuardados
                .Where(log =>
                {
                    var logKey = $"{log.Contrato?.ToString() ?? ""}-{log.Fijacion?.ToString() ?? ""}";
                    return exitososKeys.Contains(logKey);
                })
                .Select(log => log.Id)
                .ToList();

            // 5. Actualizar estado y notificar si hubo éxitos
            if (idsLogsExitosos.Any())
            {
                // Actualizar estado en BD
                logPesificacionService.ActualizarEstadoLogPesificacion(idsLogsExitosos);

                // Enviar correo de notificación
                var correo = contratosCSV.FirstOrDefault()?.Correo;
                if (!string.IsNullOrEmpty(correo))
                {
                    var contratosExitososParaEmail = exitososKeys
                        .Select(key =>
                        {
                            var parts = key.Split('-');
                            var contrato = parts[0];
                            var fijacion = parts.Length > 1 && !string.IsNullOrEmpty(parts[1]) ? parts[1] : null;
                            return string.IsNullOrEmpty(fijacion)
                                   ? $"Contrato: {contrato}"
                                   : $"Contrato: {contrato}, Fijación: {fijacion}";
                        })
                        .ToList();

                    var emailService = new EmailService();
                    emailService.EnviarMail(new SustitucionMOAUtils.Email.EmailSenderData
                    {
                        Mails = new List<string> { correo },
                        Asunto = "Resultado de Pesificación Masiva de Contratos",
                        Cuerpo = $"Se procesaron los siguientes contratos:<br/><br/>{string.Join("<br/>", contratosExitososParaEmail)}"
                    });
                }
            }

            return JsonCustom(respuestaDeContrato);
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