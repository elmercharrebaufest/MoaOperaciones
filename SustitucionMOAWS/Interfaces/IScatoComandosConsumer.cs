using SustitucionMOAModel.Models;
using SustitucionMOAWS.ScatoComandosWebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.Interfaces
{
    public interface IScatoComandosConsumer
    {
        ResultadoTickets ObtenerTicketPesada(string numeroCartaPorte);
        ResultadoConsultarTicketsNoGranos ObtenerTicketPesadaNoGranos(string patenteCamion, DateTime fechaDesde, DateTime fechaHasta);
        Resultado CrearCpsOtrosPuertos(List<CartaPorteOtrosPuertosDto> cps, string usuario);
    }
}
