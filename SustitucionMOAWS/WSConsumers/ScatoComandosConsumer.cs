using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Models;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ScatoComandosWebService;

namespace SustitucionMOAWS.WSConsumers
{
    public class ScatoComandosConsumer: IScatoComandosConsumer
    {
        private readonly ServicioComandosClient servicioComandosClient = new ServicioComandosClient();

        public ResultadoTickets ObtenerTicketPesada(string numeroCartaPorte)
        {
            return (ResultadoTickets)servicioComandosClient.Ejecutar(new ObtenerTickets
            {
                CP = numeroCartaPorte
            });
        }

        public Resultado CrearCpsOtrosPuertos(List<CartaPorteOtrosPuertosDto> cps, string usuario)
        {
            return servicioComandosClient.Ejecutar(new CrearCpOtrosPuertos
            {
                Dto = cps.ToArray(),
                Usuario = usuario
            });
        }
    }
}
