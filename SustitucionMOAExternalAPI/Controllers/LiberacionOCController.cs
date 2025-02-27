using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Web.Http;

namespace SustitucionMOAExternalAPI.Controllers
{
    public class LiberacionOCController : ApiController
    {
        private readonly IComprasService comprasSvc;
        private readonly IEntradaServicioService entradaServicioService;

        public LiberacionOCController(IComprasService comprasSvc, IEntradaServicioService entradaServicioService)
        {
            this.comprasSvc = comprasSvc;
            this.entradaServicioService = entradaServicioService;
        }

        [Authorize(Roles = "ABM SOLP")]
        public IHttpActionResult Post(string nroOc, DateTime fechaLiberacion)
        {
            try
            {
                Log.ExternalAPIInfo(string.Format("Se informó la liberacion de la OC: {0} en la fecha {1}", nroOc, fechaLiberacion));
                comprasSvc.ActualizarFechaLiberacionOC(nroOc, fechaLiberacion);
                Log.ExternalAPIInfo(string.Format("Se envio el mail de la liberacion de la OC: {0} en la fecha {1}", nroOc, fechaLiberacion));
                entradaServicioService.GenerarCertificacionAutomaticaPorLiberacionOC(nroOc);
            }
            catch (Exception ex)
            {
                Log.ExternalAPIError(ex);
            }

            return Ok();
        }

    }
}
