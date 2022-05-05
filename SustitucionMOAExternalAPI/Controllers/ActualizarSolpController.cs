using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
                Log.ExternalAPIInfo(string.Format("Se informaron cambios para la SOLP: {0}", nrosolp));
                comprasService.ObtenerSolpesDesdeSAPJob(new SustitucionMOAWS.WSConsumers.ObtenerSolpRequest
                {
                    NumeroSolp = nrosolp.TrimStart('0').PadLeft(10, '0'),
                    FechaDesde = new DateTime(2010, 01, 01),
                    FechaHasta = DateTime.Now.Date.AddDays(1)
                });
            }
            catch(Exception ex)
            {
                Log.ExternalAPIError(ex);
            }

            return Ok();
        }

    }
}
