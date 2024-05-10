using Hangfire;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace SustitucionMOAExternalAPI.Controllers
{
    public class ActualizarSolpController : ApiController
    {
        private readonly IComprasService comprasService;

        public ActualizarSolpController(IComprasService comprasService)
        {
            this.comprasService = comprasService;
        }

        [Authorize(Roles = "ABM SOLP")]
        public IHttpActionResult Post(string nrosolp)
        {
            try
            {
                Log.ExternalAPIInfo($"Inicio Se informaron cambios para la SOLP: {nrosolp}");
                BackgroundJob.Enqueue(() => comprasService.ObtenerSolpesDesdeSAPJob(new SustitucionMOAWS.WSConsumers.ObtenerSolpRequest
                {
                    NumeroSolp = nrosolp.TrimStart('0').PadLeft(10, '0'),
                    FechaDesde = new DateTime(2010, 01, 01),
                    FechaHasta = DateTime.Now.Date.AddDays(1)
                }));
                Log.ExternalAPIInfo($"Fin Se informaron cambios para la SOLP: {nrosolp}");

                return Ok();
            }
            catch (Exception ex)
            {
                Log.ExternalAPIError(ex);
                return InternalServerError(ex); 
            }
        }

        public void ProcessSolicitud(string nrosolp)
        {
            try
            {
                // Procesar la solicitud aquí
                
            }
            catch (Exception ex)
            {
                Log.ExternalAPIError(ex);
            }
        }       

    }
}
