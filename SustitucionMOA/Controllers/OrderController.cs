using Newtonsoft.Json;
using SustitucionMOAAssets;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenesCompra;
using SustitucionMOAModel.Enums;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    public class OrderController : BaseController
    {
        private readonly IOrderService orderService;

        public OrderController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        //[ValidateInput(false)]
        //[CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_SOLP)]
        public ActionResult GetByProveedor(OrderParamsDto parametros)
        {
            try
            {
                // Este debe combinarse con permisos de usuario.
                //if (parametros.vendedor == "" || parametros.vendedor == null)
                //{
                //    parametros.vendedor = SessionPersister.Proveedor;
                //}

                //parametros.OrdenCompraId = "4123001971"; //"4123001549";

                List<DetalleOrdenDeCompraDto> result = orderService.ObtenerOrdenesCompraConDetalle(parametros);

                return JsonCustom(new { data = result });
            }
            catch (Exception ex)
            {
                // Manejo de errores
                return JsonCustom(new { error = ex.Message });
            }
        }

    }
}