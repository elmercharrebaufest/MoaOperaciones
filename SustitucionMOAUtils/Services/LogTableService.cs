using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.CartaPorte.Formulario;
using SustitucionMOAModel.Models.WSMapMOA.Contrato.Detalle;
using SustitucionMOAModel.Models.WSMapMOA.Echeq;
using SustitucionMOARepositorio;
using SustitucionMOARepositorio.ConsultasEF;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.WSConsumers;

namespace SustitucionMOAUtils.Services
{
    public class LogTableService : ILogTableService
    {

        private readonly IRepositorio repositorio;

        public LogTableService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;

        }

        public List<LogTableCountErrors> ObtenerLogs(DateTime? desde, bool soloErrores)
        {
            //los frontLogger los excluyo por que no nos suman nada por ahora
            var resultado = repositorio.Listar<LogTable, LogTableDto>(
                l => new LogTableDto(l),
                x => (!soloErrores || x.Level == "error") && x.Logger != "frontLogger" && (desde == null || x.Date >= desde));

            var agrupado = resultado
                .GroupBy(g =>
                    new { g.Logger, g.Level }
                )
                .Select(logGroup => new LogTableCountErrors
                {
                    Logger = logGroup.Key.Logger,
                    Level = logGroup.Key.Level,
                    Count = logGroup.Count(),
                    //Errors = logGroup.ToList()
                })
                .ToList();
            return agrupado;
        }

        public List<LogTableCountErrors> ObtenerLogs(LogRequest request)
        {
            List<LogTableCountErrors> agrupado = repositorio.ListarConsulta(new ObtenerLogsConsulta(request));
            return agrupado;
        }
    }

}
