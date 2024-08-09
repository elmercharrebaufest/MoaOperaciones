using Hangfire;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
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
                //Log.ExternalAPIInfo(string.Format("Inicio Se informaron cambios para la SOLP: {0}", nrosolp));
                //var solp = new SustitucionMOAWS.WSConsumers.ObtenerSolpRequest
                //{
                //    NumeroSolp = nrosolp.TrimStart('0').PadLeft(10, '0'),
                //    FechaDesde = new DateTime(2010, 01, 01),
                //    FechaHasta = DateTime.Now.Date.AddDays(1)
                //};

                //BackgroundJob.Enqueue(() =>
                //    comprasService.ObtenerSolpesDesdeSAPJob(solp)
                //);
                Task.Run(() =>
                {
                    comprasService.ExecuteObtenerSolpesDesdeSAPJob(new SustitucionMOAWS.WSConsumers.ObtenerSolpRequest
                    {
                        NumeroSolp = nrosolp.TrimStart('0').PadLeft(10, '0'),
                        FechaDesde = new DateTime(2010, 01, 01),
                        FechaHasta = DateTime.Now.Date.AddDays(1)
                    });
                });

            }
            catch (Exception ex)
            {
                Log.ExternalAPIError(ex);
            }

            return Ok();
        }

    }
}
