using Microsoft.Owin.Security;
using SustitucionMOAAssets;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
using System;
using System.Web;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    public class AprobacionExternaController : BaseController
    {
        readonly IEntradaServicioService entradaServicioService;
        readonly IUsuarioService usuarioService;
        private readonly OrderService orderService;

        public AprobacionExternaController(IEntradaServicioService entradaServicioService, OrderService orderService, IUsuarioService usuarioService)
        {
            this.entradaServicioService = entradaServicioService;
            this.orderService = orderService;
            this.usuarioService = usuarioService;
        }

        public ActionResult Index(string nroESLocal)
        {
            string nulled = "Anulada";
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
            var currency = entradaServicioService.GetCurrencyType(aprobacionesListdb[0].NRO_OC);

            Proveedor prov = new Proveedor();

            if (aprobacionesListdb.Count > 0)
            {
                OrderParamsDto orderParams = new OrderParamsDto();
                orderParams.OrdenCompraId = aprobacionesListdb[0].NRO_OC;
                prov = orderService.BuscarProveedor(orderParams);

                if (!string.IsNullOrEmpty(User))
                {
                    var Usuario = usuarioService.GetUsuarioPorId(int.Parse(User));
                    if (aprobacionesListdb[0].Aprobador_CDS == Usuario.Mail && aprobacionesListdb[0].Estado_certificacion != nulled)
                    {
                        isApprover = true;
                    }
                }
            }

            string proveedorName = !string.IsNullOrEmpty(prov.RazonSocial) ? prov.RazonSocial : "";
            var result = new { aprobacionesList = aprobacionesListdb, proveedor = proveedorName, aprobador = isApprover, versionAnt = oldES, nroOc = aprobacionesListdb[0].NRO_OC, moneda = currency };
            return JsonCustom(new { data = result });
        }
    }
}
