using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Reporte;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Services
{
    public class ReportesService: IReportesService
    {
        private readonly IRepositorio repositorio;

        public ReportesService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public void EnviarReporteLiquidacionesInformadas()
        {
            var fechaAReportar = DateTime.Today.AddDays(-1);
            //Vamos a reportar las liquidaciones informadas del día de ayer
            var liquidacionesAReportar = repositorio.Listar<LiquidacionInformada>(li => li.FechaInformada == fechaAReportar);

            if (liquidacionesAReportar.Any())
            {
                EmailSender.sendReporte(new ReporteLiquidacionesInformadas()
                {
                    Asunto = "Reporte de Liquidaciones Informadas - Resumen Diario",
                    Destinatario = ConfigurationManager.AppSettings["EmailToReporteLiquidacion"],
                    Liquidaciones = liquidacionesAReportar,
                    Template = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "ReporteLiquidacionesInformadas.html")
                });
            }
            else
            {
                throw new InfoCustomException("No se encontraron liquidaciones a reportar");
            }
        }
    }
}
