using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SustitucionMOA.Jobs
{
    public interface IVencimientoOrdenesDeCargaSapJob : IHangfireJob { };

    public class VencimientoOrdenesDeCargaSapJob : IVencimientoOrdenesDeCargaSapJob
    {
        private readonly IOrdenDeCargaService ordebDeCargaService;

        public VencimientoOrdenesDeCargaSapJob(IOrdenDeCargaService ordebDeCargaService)
        {
            this.ordebDeCargaService = ordebDeCargaService;
        }

        public void Execute()
        {
            try
            {
                var response = ordebDeCargaService.verificarVencimientoOrdenDeCarga();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}