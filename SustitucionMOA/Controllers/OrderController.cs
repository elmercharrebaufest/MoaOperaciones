using SustitucionMOA.Utils;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenesCompra;
using SustitucionMOAUtils.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            // Filtro necesario por el tipo de dato que envía el front desde que se amplió la búsqueda de proveedores.
            if (parametros.vendedor == "undefined")
            {
                parametros.vendedor = string.Empty;
            }

            string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");

            ListaPaginada<DetalleOrdenDeCompraDto> result = orderService.ObtenerOrdenesCompraConDetalle(parametros, userMail);

            if (result.Items.Count > 0)
            {
                result.Items.FirstOrDefault().ItemsTotales = result.ItemsTotales;
                result.Items.FirstOrDefault().Pagina = result.Pagina;
                result.Items.FirstOrDefault().ItemPorPagina = result.ItemsPorPagina;

            }

            return ContentCustom(new { data = result });
        }


        public async Task<ActionResult> GetSolicitantesByNroSolped(List<string> solpList)
        {
            if (solpList.Count == 0)
                throw new ValidationCustomException("La lista de solped esta vacia");

            var result = await orderService.GetSolicitantes(solpList);

            return ContentCustom(new { data = result });
        }

    }
}