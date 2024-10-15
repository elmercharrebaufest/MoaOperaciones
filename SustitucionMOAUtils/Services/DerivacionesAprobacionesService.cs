using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Services
{
    public class DerivacionesAprobacionesService : IDerivacionesAprobacionesService
    {
        protected readonly IRepositorio repositorio;
        protected readonly Lazy<IEntradaServicioService> entradaServicioService;


        public DerivacionesAprobacionesService(IRepositorio repositorio, Lazy<IEntradaServicioService> entradaServicioService)
        {
            this.repositorio = repositorio;
            this.entradaServicioService = entradaServicioService;
        }

        public void ReturnAprobaciones(string mailFiscal, string mailSuplente)
        {
            var aprobacionesPendientes = repositorio.Listar<Aprobaciones>(x => x.Fiscal_SOLPED == mailFiscal
                && x.Suplente == mailFiscal && x.Aprobador_CDS == mailSuplente
                && x.Estado_certificacion == "Pendiente Aprobación");

            List<string> eSLocalesInicio = new List<string>();

            foreach (var item in aprobacionesPendientes)
            {
                item.Aprobador_CDS = item.Fiscal_SOLPED;
                item.Suplente = mailSuplente;

                if (!string.IsNullOrEmpty(item.NRO_ES_LOCAL) && !eSLocalesInicio.Contains(item.NRO_ES_LOCAL))
                {
                    eSLocalesInicio.Add(item.NRO_ES_LOCAL);
                }
            }

            if (eSLocalesInicio.Count > 0)
            {
                repositorio.GuardarCambios();

                try
                {
                    var entradaServicio = entradaServicioService.Value;
                    Task.Run(() => entradaServicio.NotificarReasignaciones(eSLocalesInicio, (RepositorioEF)repositorio)).Wait();
                    Logger.Log.Info("Notificaciones de reasignacion a suplente enviadas");

                }
                catch (Exception ex)
                {
                    Logger.Log.Info("Error en la notificación de reasignaciones del proceso de reasignación: " + ex.Message);
                }
            }
        }
    }
}
