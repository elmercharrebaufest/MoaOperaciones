using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenResiduos;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    public class OrdenResiduosController : BaseController
    {
        private readonly IOrdenResiduosService ordenResiduosService;

        public OrdenResiduosController(IOrdenResiduosService ordenResiduosService)
        {
            this.ordenResiduosService = ordenResiduosService;
        }

        [HttpGet]
        public ActionResult ObtenerListadoOrdenes(string fechaInicio, string fechaFin)
        {
            var response = new SustitucionMOAApiResponse<ListarOrdenesResiduosResponse>();
            try
            {
                response.Data = ordenResiduosService.ObtenerListadoOrdenes();
            }
            catch (InfoCustomException ice)
            {
                response.Info = ice.Message;
            }
            catch (ValidationCustomException vce)
            {
                response.Error = vce.Message;
            }
            catch (Exception ex)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }
    }
}