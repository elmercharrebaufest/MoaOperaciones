using Newtonsoft.Json;
using SustitucionMOAModel.Models;
using SustitucionMOAUtils.Interfaces;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    public class TicketPesadaController : BaseController
    {
        readonly ITicketPesadaService ticketPesadaService;

        public TicketPesadaController(ITicketPesadaService ticketPesadaService)
        {
            this.ticketPesadaService = ticketPesadaService;
        }

        public JsonResult Obtener(string ticketPesadaJson)
        {
            var consultaTicketPesada = JsonConvert.DeserializeObject<ConsultaTicketPesada>(ticketPesadaJson);
            var listadoArchivos = ticketPesadaService.ObtenerTicket(consultaTicketPesada);
            return JsonCustom(new { data = listadoArchivos });
        }
    }
}