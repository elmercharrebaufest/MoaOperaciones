using SustitucionMOAModel.Dto;
using SustitucionMOAUtils.Logger;
using System;
using System.Web.Http;

namespace SustitucionMOAExternalAPI.Controllers
{
    public class LogQrController : ApiController
    {

        public LogQrController()
        {
        }

        [Authorize(Roles = "ABM SOLP")]
        public IHttpActionResult Post([FromBody] LogQrRequestDto request)
        {
            try
            {
                Log.QrError(new FrontLoggerRequestDto
                {
                    Message = request.Context,
                    Additional = new System.Collections.Generic.List<object> { request.Error },
                    Timestamp = DateTime.Now
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