using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Web.Http;

namespace SustitucionMOAExternalAPI.Controllers
{
    public class LiberacionOCController : ApiController
    {
        private readonly IComprasService comprasSvc;

        public LiberacionOCController(IComprasService comprasSvc)
        {
            this.comprasSvc = comprasSvc;
        }

        [Authorize(Roles = "ABM SOLP")]
        public IHttpActionResult Post(string nroOc, DateTime fechaLiberacion)
        {
            try
            {
                Log.ExternalAPIInfo(string.Format("Se informó la liberacion de la OC: {0} en la fecha {1}", nroOc, fechaLiberacion));
                comprasSvc.ActualizarFechaLiberacionOC(nroOc, fechaLiberacion);
            }
            catch (Exception ex)
            {
                Log.ExternalAPIError(ex);
            }

            return Ok();
        }

    }
}
