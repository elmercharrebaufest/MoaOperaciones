using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    public class AprobacionExternaController : BaseController
    {
        readonly IEntradaServicioService entradaServicioService;
        private readonly OrderService orderService;
        

        public AprobacionExternaController(IEntradaServicioService entradaServicioService, OrderService orderService)
        {
            this.entradaServicioService = entradaServicioService;
            this.orderService = orderService;
        }


        public ActionResult Index(string nroESLocal)
        {
            try
            {
                string ES = "";
                string User = "";               
                bool oldES = false;
                bool isApprover = false;
                if (nroESLocal.Contains("&"))
                {
                    ES = nroESLocal.Split('&')[0];
                    User = nroESLocal.Split('&')[1];
                }
                else
                {
                    ES = nroESLocal;
                    oldES = true;
                }

                var aprobacionesListdb = entradaServicioService.GetESTemporaria(ES);
                Proveedor prov = new Proveedor();

                if (aprobacionesListdb.Count > 0)
                {
                    OrderParamsDto orderParams = new OrderParamsDto();
                    orderParams.OrdenCompraId = aprobacionesListdb[0].NRO_OC;
                    prov = orderService.BuscarProveedor(orderParams);

                    if (!string.IsNullOrEmpty(User))
                    {
                        if (aprobacionesListdb[0].Aprobador_CDS == User)
                        {
                            isApprover = true;
                        }
                    }
                }



                string proveedorName = !string.IsNullOrEmpty(prov.RazonSocial) ? prov.RazonSocial : "";
                var result = new { aprobacionesList = aprobacionesListdb, proveedor = proveedorName, aprobador = isApprover, versionAnt = oldES };
                return JsonCustom(new { data = result });
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}