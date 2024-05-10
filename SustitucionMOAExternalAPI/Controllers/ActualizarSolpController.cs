using SustitucionMOAExternalAPI.Jobs;
using SustitucionMOAUtils.Logger;
using System;
using System.Web.Http;

namespace SustitucionMOAExternalAPI.Controllers
{
    public class ActualizarSolpController : ApiController
    {
        private readonly IJobService jobService;

        public ActualizarSolpController(IJobService jobService)
        {
            this.jobService = jobService;
        }

        [Authorize(Roles = "ABM SOLP")]
        public IHttpActionResult Post(string nrosolp)
        {
            try
            {
                Log.ExternalAPIInfo($"Inicio Se informaron cambios para la SOLP: {nrosolp}");
                jobService.ActualizarSolp(nrosolp);
                Log.ExternalAPIInfo($"Fin Se informaron cambios para la SOLP: {nrosolp}");

                return Ok();
            }
            catch (Exception ex)
            {
                Log.ExternalAPIError(ex);
                return InternalServerError(ex); 
            }
        }

        

    }
}
