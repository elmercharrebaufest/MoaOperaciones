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
                ObtenerSolpRequest obtenerSolpRequest = new ObtenerSolpRequest
                {
                    FechaDesde = new DateTime(2022, 01, 11), // Convert.ToDateTime(ConfigurationManager.AppSettings["FechaInicioConsultaSolp"].ToString()),
                    FechaHasta = DateTime.Now,
                    CreadoPorUsuarios = new List<string>(),
                };
                _comprasService.ObtenerYGuardarSolpSap(obtenerSolpRequest);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}