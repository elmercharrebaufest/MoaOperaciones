using SustitucionMOAModel.Entities;
using SustitucionMOAUtils.Services.Email.Dto;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IEmailComprasService
    {
        void EnviarMailCotizacionCreada(Cotizacion cotizacion);
        void EnviarMailSolpLiberada(Solp solp);
        void EnviarMailFinalizacionPliegoMultiple(Pliego pliego,List<Solp> solps);
        void EnviarMailPeticionDeOferta(MailPeticionDeOfertaRequest req);
        void EnviarMailReporteTrabajoYaHecho(byte[] reporteExcel, string nombreArchivo);
    }
}
