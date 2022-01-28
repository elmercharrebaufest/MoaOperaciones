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
        public void Execute()
        {
            try
            {
                if(ConfigurationManager.AppSettings["ObtenerSolpsDesdeSAPJob_Habilitado"] == "1")
                {
                    ObtenerSolpRequest obtenerSolpRequest = new ObtenerSolpRequest
                    {
                        FechaDesde = Convert.ToDateTime(ConfigurationManager.AppSettings["FechaInicioObtenerSolpsDesdeSAPJob"].ToString()),
                        FechaHasta = Convert.ToDateTime(ConfigurationManager.AppSettings["FechaFinObtenerSolpsDesdeSAPJob"].ToString()),
                        CreadoPorUsuarios = new List<string>()
                    };

                    _comprasService.ObtenerSolpesDesdeSAPJob(obtenerSolpRequest);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
