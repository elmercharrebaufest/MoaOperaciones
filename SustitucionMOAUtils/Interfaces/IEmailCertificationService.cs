using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.Compras;
using SustitucionMOAModel.Dto.OrdenesCompra;
using SustitucionMOAModel.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IEmailCertificationService
    {
        Task SendNotifyRejectionEmail(EmailDetailCertificateDto emailDetailCertificateDto);

        Task EnviarMailAprobacion(List<Aprobaciones> apList, Proveedor prov, int userId, string destinatario, List<ReporteDto> reporte, IEnumerable<AdjuntosEntradasDeServicio> adjuntosMetadata);

        Task SendAprobalProviderEmail(EmailDetailCertificateDto emailDetailCertificateDto, string reference);

        void SendDailyNotification(string to, List<NotificacionEsPendientesDiariasDto> aprobaciones);
    }
}
