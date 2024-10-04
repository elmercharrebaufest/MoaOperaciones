using log4net.Core;
using Molinos.Scato.Repositorio;
using Ninject.Activation;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAModel.Models.WSMapMOA.Pago.NoGranos;
using SustitucionMOARepositorio.Extensiones;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Linq;

namespace SustitucionMOARepositorio.ConsultasEF
{
    public class ObtenerLogsConsulta : IConsulta<LogTableCountErrors>
    {
        private readonly LogRequest request;

        public ObtenerLogsConsulta(LogRequest request)
        {
            this.request = request;
        }
        public List<LogTableCountErrors> Ejecutar(DbContext contexto)
        {
            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            
            var sinLevels = !request.Levels.Any();
            var sinLoggers = !request.Loggers.Any();
            var resultado = from x in contexto.Set<LogTable>()
                            where x.Logger != "frontLogger" && 
                            //!x.Logger.Contains("hangfire") &&
                            (request.Desde == null || x.Date >= request.Desde) &&
                            (request.Hasta == null || x.Date <= request.Hasta) &&
                            (sinLoggers || request.Loggers.Contains(x.Logger)) &&
                            (sinLevels || request.Levels.Contains(x.Level)) 
                            group x by new { x.Logger, x.Level } into logGroup
                            select new LogTableCountErrors
                            {
                                Logger = logGroup.Key.Logger.Replace("Logger",""),
                                Level = logGroup.Key.Level,
                                LoggerLevel = logGroup.Key.Logger.Replace("Logger", "") + " - " + logGroup.Key.Level,
                                Count = logGroup.Count(),
                                Errors = logGroup.Select(lg => new LogTableDto
                                {
                                    Id = lg.Id,
                                    Logger = lg.Logger.Replace("Logger", ""),
                                    Level = lg.Level,
                                    Message = lg.Message,
                                    Date = lg.Date,
                                    Exception = lg.Exception,
                                })
                            };

            return resultado.ToList();

        }
    }
}