using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.CartaPorte;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Services
{
    public class TicketPesadaService : ITicketPesadaService
    {
        readonly IScatoConsumer scatoConsumer;

        public TicketPesadaService(IScatoConsumer scatoConsumer)
        {
            this.scatoConsumer = scatoConsumer;
        }

        public List<CartaPorteFoto> ObtenerTicket(ConsultaTicketPesada consultaTicketPesada)
        {
            var fotosCP = this.scatoConsumer.ObtenerFotoCartaPorte(consultaTicketPesada.NumeroCartaPorte.ToString());

            return fotosCP;
        }
    }
}
