using System;
using System.Collections.Generic;
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
                    FechaDesde = new DateTime(2021, 09, 01),
                    FechaHasta = new DateTime(2021, 10, 01),
                    CreadoPorUsuarios = new List<string>(),
                };

                ObtenerSolpSAPResponse obtenerSolpSAPResponse = this._comprasService.ObtenerSolpsSAP(obtenerSolpRequest);

                var posicion = obtenerSolpSAPResponse.Posiciones.Single(x => x.NumeroPosicion == "0212105186");
                var servicioSuposicion = obtenerSolpSAPResponse.ServiciosSuposiciones.Single(x => x.NumeroPosicion == "0212105186");


            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}