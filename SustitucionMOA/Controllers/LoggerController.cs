using SustitucionMOAModel.Dto;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    public class LoggerController : BaseController
    {
        private readonly ILogTableService logTableService;
        public LoggerController(ILogTableService logTableService)
        {
            this.logTableService = logTableService;
        }

        [HttpPost]
        public ActionResult Front(FrontLoggerRequestDto data)
        {
            Log.FrontError(data);
            return JsonCustom(true);
        }

        [HttpGet]
        public ActionResult ObtenerLogs(LogRequest request)
        {
            request.Levels = new List<string> { "Error" };
            //request.Desde = DateTime.Today;
            var data = logTableService.ObtenerLogs(request);
            return JsonCustom(data);
        }
    }
}