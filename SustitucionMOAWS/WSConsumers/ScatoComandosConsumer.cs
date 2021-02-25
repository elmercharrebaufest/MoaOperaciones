using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Models;
using SustitucionMOAWS.ScatoComandosWebService;

namespace SustitucionMOAWS.WSConsumers
{
    class ScatoComandosConsumer 
    {
        private readonly ServicioComandosClient servicioComandosClient = new ServicioComandosClient();

        private void ObtenerTicketPesada(ConsultaTicketPesada consultaTicketPesada)
        {
            ObtenerTickets comando = new ObtenerTickets
            {
                CP = "1000"
            };

            var resultado = servicioComandosClient.Ejecutar(comando);

            Console.Write("Test");

            return ;
        }
    }
}
