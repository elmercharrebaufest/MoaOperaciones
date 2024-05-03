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
        private readonly Queue<string> colaDeSolicitudes = new Queue<string>();
        private readonly object lockObj = new object(); 

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

                lock (lockObj)
                {
                    colaDeSolicitudes.Enqueue(nrosolp.TrimStart('0').PadLeft(10, '0'));
                }

                if (colaDeSolicitudes.Count == 1)
                {
                    ProcesarSiguienteSolicitud();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                Log.ExternalAPIError(ex);
                return InternalServerError(ex); 
            }
        }


        private void ProcesarSiguienteSolicitud()
        {
            string nrosolp;
            lock (lockObj)
            {
                if (colaDeSolicitudes.Count == 0)
                {
                    return;
                }
                nrosolp = colaDeSolicitudes.Peek();
            }

            try
            {
                comprasService.ObtenerSolpesDesdeSAPJob(new SustitucionMOAWS.WSConsumers.ObtenerSolpRequest
                {
                    NumeroSolp = nrosolp,
                    FechaDesde = new DateTime(2010, 01, 01),
                    FechaHasta = DateTime.Now.Date.AddDays(1)
                });
            }
            catch (Exception ex)
            {
                Log.ExternalAPIError(ex);
            }
            finally
            {
                lock (lockObj)
                {
                    colaDeSolicitudes.Dequeue();
                }

                ProcesarSiguienteSolicitud();
            }
        }

    }
}
