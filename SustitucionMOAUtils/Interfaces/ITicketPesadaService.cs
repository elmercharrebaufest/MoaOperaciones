using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.CartaPorte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Interfaces
{
    public interface ITicketPesadaService
    {
        byte[] ObtenerTicket(ConsultaTicketPesada consultaTicketPesada);
    }
}
