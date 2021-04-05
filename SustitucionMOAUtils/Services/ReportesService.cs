using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Reporte;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Export;
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

        public void EnviarReporteCamposSustentablesTSA()
        {
            var camposAReportar = repositorio.Listar<CampoProveedor>(cp => cp.FechaCreacion.HasValue && cp.FechaCreacion.Value == DateTime.Today)
                .Select(cp => new {
                    cp.CampoCosecha.Campo.IdScato,
                    cp.CampoCosecha.Campo.Id,
                    cp.Proveedor.RazonSocial,
                    cp.Proveedor.CUIT,
                    cp.CampoCosecha.Campo.Nombre,
                    Provincia = cp.CampoCosecha.Campo.Localidad.Provincia.Nombre,
                    Departamento = cp.CampoCosecha.Campo.Localidad.Partido.Descripcion,
                    Localidad = cp.CampoCosecha.Campo.Localidad.Nombre,
                    cp.Latitud,
                    cp.Longitud,
                    cp.HectareasSoja
                });

            var excelFile = ExcelExport.ToExcel(camposAReportar, new string[] { "ID", "Codigo Operaciones", "Titular CCPP", "CUIT", "Nombre del Establecimiento", "Provincia", "Departamento", "Localidad", "Latitud", "Longitud", "Has de soja declaradas" }, string.Empty);

            File.WriteAllText($"{ConfigurationManager.AppSettings["RutaArchivosProveedores"]}/{new Guid()}.xls", excelFile);

            //TODO: Cambiar el writealltext a la llamada que realmente envía el mail. Por ahora el reporte solo tiene el excel pero deberíamos poder manejar una lista de adjuntos
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
