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
    [Authorize]
    public class OrderController : BaseController
    {
        private readonly IOrderService orderService;

        public OrderController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        public ActionResult GetByProveedor(OrderParamsDto parametros)
        {
            if (parametros.vendedor == "undefined")
            {
                parametros.vendedor = string.Empty;
            }
            parametros.pagina = parametros.pagina == 0 ? 1 : parametros.pagina;
            parametros.elementosPorPagina = parametros.elementosPorPagina == 0 ? 10 : parametros.elementosPorPagina;

            string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");

            //descomentar
            ListaPaginada<DetalleOrdenDeCompraDto> result = orderService.ObtenerOrdenesCompraConDetalle(parametros, userMail);

            ////inicio datos de pruebas
            //List<DetalleOrdenDeCompraDto> items = new List<DetalleOrdenDeCompraDto>();
            //for (int i = 0; i < 100; i++)
            //{
            //    items.Add(new DetalleOrdenDeCompraDto { NumeroOrdenDeCompra = i.ToString() });
            //}
            //ListaPaginada<DetalleOrdenDeCompraDto> result = new ListaPaginada<DetalleOrdenDeCompraDto>(
            //     items.Skip(parametros.pagina * parametros.elementosPorPagina).Take(10).ToList(),
            //     parametros.pagina,
            //     parametros.elementosPorPagina,
            //     items.Count
            // );
            ////fin datos de pruebas

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