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
    }
}
