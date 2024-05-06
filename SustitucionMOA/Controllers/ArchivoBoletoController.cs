using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebHttp = System.Web.Http;
using System.Web.Mvc;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.ArchivoBoleto;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;

namespace SustitucionMOA.Controllers
{
    [System.Web.Mvc.SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class ArchivoBoletoController : BaseController
    {
        private readonly IArchivoBoletoService archivoBoletoService;

        public ArchivoBoletoController(IArchivoBoletoService archivoBoletoService)
        {
            this.archivoBoletoService = archivoBoletoService;
        }


        [HttpPost]
        public async Task<ActionResult> CrearArchivoBoleto([WebHttp.FromBody]CrearReqArchivoBoletoDto data)
        {

            var apiResponse = new SustitucionMOAApiResponse<ArchivoBoletoDto>();
            try
            {
                data.ProveedorId = SessionPersister.ProveedorId;
                data.EmailUsuario = SessionPersister.getUsername();
                apiResponse.Data = await archivoBoletoService.CrearArchivoBoleto(data);
            }
            catch (ValidationCustomException e)
            {
                apiResponse.Error = ErrorMsg.Error;
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                apiResponse.Error = ErrorMsg.Error;
            }
            return ContentCustom(apiResponse);
        }
        [HttpGet]
        public ActionResult ListarArchivosBoleto([WebHttp.FromUri] ListarReqArchivoBoletoDto request)
        {
            var apiResponse = new SustitucionMOAApiResponse<List<ArchivoBoletoDto>>();
            try
            {
                request.ProveedorId = SessionPersister.ProveedorId;
                apiResponse.Data = archivoBoletoService.ListarArchivosBoleto(request);
            }
            catch(InfoCustomException e)
            {
                apiResponse.Info = e.Message;
            }
            catch (ValidationCustomException e)
            {
                apiResponse.Error = ErrorMsg.Error;
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                apiResponse.Error = ErrorMsg.Error;
            }
            return ContentCustom(apiResponse);
        }

    }
}