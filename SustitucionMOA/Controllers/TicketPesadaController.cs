using iTextSharp.text;
using Newtonsoft.Json;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Models;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    public class TicketPesadaController : BaseController
    {
        readonly ITicketPesadaService ticketPesadaService;
        private readonly IScatoConsumer scatoConsumer;


        public TicketPesadaController(ITicketPesadaService ticketPesadaService, IScatoConsumer scatoConsumer)
        {
            this.ticketPesadaService = ticketPesadaService;
            this.scatoConsumer = scatoConsumer;
        }

        public JsonResult Obtener(string ticketPesadaJson)
        {
            var consultaTicketPesada = JsonConvert.DeserializeObject<ConsultaTicketPesada>(ticketPesadaJson);
            var listadoArchivos = ticketPesadaService.ObtenerTicket(consultaTicketPesada);
            return JsonCustom(new { data = listadoArchivos });
        }

        public JsonResult ObtenerNoGranos(string ticketPesadaNoGranosJson)
        {
            var consultaTicketPesadaNoGranos = JsonConvert.DeserializeObject<ConsultaTicketPesadaNoGranos>(ticketPesadaNoGranosJson);
            var listadoArchivos = ticketPesadaService.ObtenerTicketNoGranos(consultaTicketPesadaNoGranos);
            return JsonCustom(new { data = listadoArchivos });
        }

        [HttpGet]
        public JsonResult ListarDatosTicketPesada(DateTime fechaInicio, DateTime fechaEgreso, string cuitProveedor, string cuitTransportista, string ctg, string patente, string cuitIntermediarioFlete, bool esAdmin = false)
        {
            if(!esAdmin && string.IsNullOrEmpty(cuitProveedor))
            {
                cuitProveedor = this.FormatearCuit(SessionPersister.CUIT);
                cuitTransportista = this.FormatearCuit(SessionPersister.CUIT);
                cuitIntermediarioFlete = this.FormatearCuit(SessionPersister.CUIT);
            }
            var listadoOriginal = scatoConsumer.ObtenerDatosTicketPesada(fechaInicio, fechaEgreso, cuitProveedor, cuitTransportista, ctg, patente, cuitIntermediarioFlete, esAdmin);
            if (listadoOriginal == null)
            {
                return JsonCustom(new { data = new List<TicketPesadaDto>() });
            }
            var listadoMapeado = Array.ConvertAll(listadoOriginal, ticket => new TicketPesadaDto
            {
                CTG = ticket.CTG,
                ChoferNombreApellido = ticket.ChoferNombreApellido,
                FechaHoraEgreso = ticket.FechaHoraEgreso,
                FechaHoraIngreso = ticket.FechaHoraIngreso,
                IntermediarioFlete = ticket.IntermediarioFlete,
                IntermediarioFleteCUIT = ticket.IntermediarioFleteCUIT,
                Material = ticket.Material,
                Patente = ticket.Patente,
                BrutoOrigen = ticket.BrutoOrigen,
                NetoOrigen = ticket.NetoOrigen,
                TaraOrigen = ticket.TaraOrigen,
                BrutoPlanta = ticket.BrutoPlanta,
                NetoPlanta = ticket.NetoPlanta,
                TaraPlanta = ticket.TaraPlanta,
                TitularCPCUIT = ticket.TitularCPCUIT,
                Procedencia = ticket.Procedencia,
                Transportista = ticket.Transportista,
                TransportistaCUIT = ticket.TransportistaCUIT
            });

            return JsonCustom(new { data = listadoMapeado });
        }

        public string FormatearCuit(string cuit)
        {
            return $"{cuit.Substring(0, 2)}-{cuit.Substring(2, 8)}-{cuit.Substring(10, 1)}";
        }

    }
}