using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.ArchivoBoleto;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebHttp = System.Web.Http;

namespace SustitucionMOA.Controllers
{
    [System.Web.Mvc.SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [Authorize]
    public class ArchivoBoletoController : BaseController
    {
        private readonly IArchivoBoletoService archivoBoletoService;

        public ArchivoBoletoController(IArchivoBoletoService archivoBoletoService)
        {
            this.archivoBoletoService = archivoBoletoService;
        }

        [HttpPost]
        public async Task<ActionResult> CrearArchivoBoleto([WebHttp.FromBody] CrearReqArchivoBoletoDto data)
        {
            var apiResponse = new SustitucionMOAApiResponse<ArchivoBoletoDto>();
            data.ProveedorId = SessionPersister.ProveedorId;
            data.EmailUsuario = SessionPersister.getUsername();
            apiResponse.Data = await archivoBoletoService.CrearArchivoBoleto(data);
            return ContentCustom(apiResponse);
        }

        [HttpGet]
        public ActionResult ListarArchivosBoleto([WebHttp.FromUri] ListarReqArchivoBoletoDto request)
        {
            var apiResponse = new SustitucionMOAApiResponse<List<ArchivoBoletoDto>>();
            request.ProveedorId = SessionPersister.ProveedorId;
            apiResponse.Data = archivoBoletoService.ListarArchivosBoleto(request);
            return ContentCustom(apiResponse);
        }
    }
}
