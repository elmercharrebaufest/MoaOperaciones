using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.WSConsumers;

namespace SustitucionMOA.Jobs
{
    public interface IObtenerSolpsDesdeSAPJob : IHangfireJob { }
    public class ObtenerSolpsDesdeSAPJob : IObtenerSolpsDesdeSAPJob
    {
        private readonly IComprasService _comprasService;
        private readonly IRepositorio repositorio;
        public ObtenerSolpsDesdeSAPJob(IComprasService comprasService, IRepositorio repositorio)
        {
            _comprasService = comprasService;
            this.repositorio = repositorio;
        }
        static readonly object _lock = new object();

        public void Execute()
        {
            lock (_lock)
            {
                try
                {
                    if (repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "ObtenerSolpsDesdeSAPJob").Habilitado == false)
                        return; 

                    if (ConfigurationManager.AppSettings["ObtenerSolpsDesdeSAPJob_Habilitado"] == "1")
                    {
                        var desde = new DateTime(2018, 01, 01);
                        var hasta = new DateTime(2022, 12, 01);
                        while (desde < hasta)
                        {
                            try
                            {
                                Log.Info($"ObtenerSolpesDesdeSAPJob desde {desde} hasta {desde.AddMonths(3)}");
                                ObtenerSolpRequest obtenerSolpRequest = new ObtenerSolpRequest
                                {
                                    FechaDesde = desde,
                                    FechaHasta = desde.AddMonths(3),
                                    CreadoPorUsuarios = new List<string>()
                                };
                                _comprasService.ObtenerSolpesDesdeSAPJob(obtenerSolpRequest);
                                desde = desde.AddMonths(3);
                            }
                            catch (Exception e )
                            {
                                Log.Info($"ObtenerSolpesDesdeSAPJob error");
                                Log.Error(e);
                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
    }
}
