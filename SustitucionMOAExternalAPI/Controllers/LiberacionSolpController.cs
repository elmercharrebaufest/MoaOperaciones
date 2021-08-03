using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SustitucionMOAExternalAPI.Controllers
{
    public class LiberacionSolpController : ApiController
    {
        [Authorize(Roles = "ABM SOLP")]
        public IHttpActionResult Post(string nrosolp, [FromBody]DateTime fechaLiberacion)
        {
            Log.ExternalAPIInfo(string.Format("Se informó la liberacion de la SOLP: {0} en la fecha {1}", nrosolp, fechaLiberacion));

            return Ok();
        }

    }
}
