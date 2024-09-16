using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using System.Data.Entity;
using SustitucionMOAModel.Dto;


namespace SustitucionMOAUtils.Services
{
    public class AprobacionesService : IAprobacionesService
    {
        private readonly Func<DbContext> _dbContextFactory;
        protected IRepositorio repositorio;
        protected IEmailCertificationService emailService;
        private readonly IComprasService comprasService;

        public AprobacionesService(IRepositorio repositorio, IEmailCertificationService emailService, Func<DbContext> context, IComprasService comprasService)
        {
            this.repositorio = repositorio;
            this.emailService = emailService;
            this._dbContextFactory = context;
            this.comprasService = comprasService;
        }

        public string Notificar()
        {
            using (var dbContext = _dbContextFactory())
            {

                dbContext.Database.CreateIfNotExists();

                var repositorio = new RepositorioEF(dbContext);
                var aprobaciones = repositorio.Listar<Aprobaciones>(a => a.Estado_certificacion == "Pendiente Aprobación").ToList();

                List<NotificacionEsPendientesDiariasDto> notificacionEsPendientesDiariasDtoList = new List<NotificacionEsPendientesDiariasDto>();

                foreach (var item in aprobaciones)
                {
                    var ordenCompra = comprasService.ObtenerOrdenDeCompra(item.NRO_OC);

                    NotificacionEsPendientesDiariasDto notificacionPendientes = new NotificacionEsPendientesDiariasDto();

                    notificacionPendientes.NRO_ES_LOCAL = item.NRO_ES_LOCAL;
                    notificacionPendientes.Proveedor = item.Proveedor;
                    notificacionPendientes.NRO_OC = item.NRO_OC;
                    notificacionPendientes.Texto_breve_servicio = item.Texto_breve_servicio;
                    notificacionPendientes.Cantidad_a_certificar = item.Cantidad_a_certificar;
                    notificacionPendientes.UM = item.UM;
                    notificacionPendientes.Porcentaje_a_certificar = item.Porcentaje_a_certificar;
                    notificacionPendientes.Monto_a_certificar = item.Monto_a_certificar;
                    notificacionPendientes.Aprobador_CDS = item.Aprobador_CDS;
                    notificacionPendientes.Moneda = ordenCompra.Cabecera.Moneda;

                    notificacionEsPendientesDiariasDtoList.Add(notificacionPendientes);
                }

                var aprobacionesPendientes = notificacionEsPendientesDiariasDtoList
                    .GroupBy(a => a.Aprobador_CDS)
                    .Select(grupo => (
                        Aprobador: grupo.Key,
                        Aprobaciones: grupo.ToList()
                    ))
                    .ToList();


                foreach (var aprobacionesPorAprobador in aprobacionesPendientes)
                {
                    emailService.SendDailyNotification(aprobacionesPorAprobador.Aprobador, aprobacionesPorAprobador.Aprobaciones);
                }
            }

            return "Ok";
            
        }
    }
}
