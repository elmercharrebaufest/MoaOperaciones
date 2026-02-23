using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Models;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface ITicketPesadaService
    {
        List<ArchivoDescargaDto> ObtenerTicket(ConsultaTicketPesada consultaTicketPesada);
		List<ArchivoDescargaDto> ObtenerTicket(TrackingRequestDto trackingRequest);
		List<ArchivoDescargaDto> ObtenerTicketNoGranos(ConsultaTicketPesadaNoGranos consultaTicketPesadaNoGranos);
    }
}
