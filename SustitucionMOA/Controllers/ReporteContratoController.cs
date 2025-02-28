using Newtonsoft.Json;
using SustitucionMOAModel.Models.WSMapMOA.ReporteContrato;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    [Authorize]
    public class ReporteContratoController : BaseController
    {
        private readonly IOrdenDeCargaService ordenDeCargaService;
        protected readonly IReporteContratoService reporteContratoService;

        public ReporteContratoController(IOrdenDeCargaService ordenDeCargaService, IReporteContratoService reporteContratoService)
        {
            this.ordenDeCargaService = ordenDeCargaService;
            this.reporteContratoService = reporteContratoService;
        }
        // GET: ReporteContrato


        public JsonResult GetContratos(string fechaInicio, string fechaFin, bool mostrarPendientes, string contrato)
        {
            var proveedor = SessionPersister.Proveedor;
            var mailUsuario = SessionPersister.getUsername();
            if (string.IsNullOrEmpty(proveedor))
            {
                return JsonCustom(new { logout = true });
            }

            var contratos = reporteContratoService.GetContratosReporte(mailUsuario, proveedor, fechaInicio, fechaFin, mostrarPendientes, null);
            return JsonCustom(contratos);
        }

        public ActionResult ObtenerContratosFiltro(string fechaInicio, string fechaFin, string cliente, string producto, string tipoContrato, bool mostrarPendientes, string dataContrato)
        {
            var proveedor = SessionPersister.Proveedor;
            var mailUsuario = SessionPersister.getUsername();
            if (string.IsNullOrEmpty(proveedor))
            {
                return JsonCustom(new { logout = true });
            }
            var dataFiltro = JsonConvert.DeserializeObject<ReporteContratoWSMOAResponse>(dataContrato);
            var contratos = reporteContratoService.GetContratosReporte(mailUsuario, proveedor, fechaInicio, fechaFin, mostrarPendientes, dataFiltro);
            return JsonCustom(contratos);
        }

        public ActionResult ObtenerDetalleContrato(string contrato, string fechaInicio, string fechaFin)
        {
            var mailUsuario = SessionPersister.getUsername();
            var detalleContrato = reporteContratoService.GetContratosDetalle(contrato, SessionPersister.Proveedor, fechaInicio, fechaFin);

            Result result = new Result();
            foreach (var det in detalleContrato.data.Resultados)
            {
                result.Detalles = det.Detalles;

                foreach (var item in result.Detalles)
                {
                    if (!item.Entrega.Equals(string.Empty))
                    {
                        try
                        {
                            var orden = ordenDeCargaService.ObtenerPorNroEntrega(mailUsuario, item.Entrega);
                            item.OrdenCargaId = orden.Id.ToString();
                        }
                        catch (Exception)
                        {
                            item.OrdenCargaId = string.Empty;
                        }
                    }
                }
            }

            return JsonCustom(new { data = result.Detalles });
        }
        public ActionResult ObtenerOrdenDeCarga(string nroEntrega)
        {
            var mailUsuario = SessionPersister.getUsername();

            return JsonCustom(new { data = ordenDeCargaService.ObtenerPorNroEntrega(mailUsuario, nroEntrega) });
        }
    }
}