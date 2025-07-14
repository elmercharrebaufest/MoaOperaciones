using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.Compras;
using SustitucionMOAModel.Dto.OrdenesCompra;
using SustitucionMOAModel.Entities;
using SustitucionMOAUtils.Services.Email.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IEmailCertificationService
    {
        Task SendNotifyRejectionEmail(EmailDetailCertificateDto emailDetailCertificateDto);

        Task SendAprobalProviderEmail(EmailDetailCertificateDto emailDetailCertificateDto, string reference);

        void SendDailyNotification(string to, List<NotificacionEsPendientesDiariasDto> aprobaciones);
        void EnviarMailCertificacionAutomatica(string nroOC, string nroSolp, string mensaje, IEnumerable<string> destinatarios);
        void EnviarMailAprobacion(MailAprobacionESRequest request);
    }
}
