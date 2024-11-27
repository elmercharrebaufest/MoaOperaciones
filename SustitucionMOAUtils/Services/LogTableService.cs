using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOARepositorio.ConsultasEF;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

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
                l => new LogTableDto
                {
                    Id = l.Id,
                    Level = l.Level,
                    Logger = l.Logger,
                    Date = l.Date,
                    Exception = l.Exception,
                    Message = l.Message

                },
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
                    Errors = logGroup.ToList()
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
