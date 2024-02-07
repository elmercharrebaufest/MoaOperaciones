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
        //[HttpGet]
        public ActionResult GetByProveedor(OrderParamsDto parametros)
        {
            try
            {
                // Filtro necesario por el tipo de dato que envía el front desde que se amplió la búsqueda de proveedores.
                if (parametros.vendedor == "undefined")
                {
                    parametros.vendedor = string.Empty;
                }

                ListaPaginada<DetalleOrdenDeCompraDto> result = orderService.ObtenerOrdenesCompraConDetalle(parametros);

                if (result.Items.Count > 0)
                {
                    result.Items.FirstOrDefault().ItemsTotales = result.ItemsTotales;
                    result.Items.FirstOrDefault().Pagina = result.Pagina;
                    result.Items.FirstOrDefault().ItemPorPagina = result.ItemsPorPagina;

                }

                return ContentCustom(new { data = result });
            }
            catch (Exception ex)
            {
                return JsonCustom(new { error = ex.Message });
            }
        }

    }
}