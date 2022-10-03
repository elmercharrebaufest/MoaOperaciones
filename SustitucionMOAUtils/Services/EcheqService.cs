using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.Echeq;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.WSConsumers;

namespace SustitucionMOAUtils.Services
{
    public class EcheqService : IEcheqService
    {
        private readonly IEcheqVisualizarPendientePagoConsumerMOA echeqVisualizarPendientePagoConsumerMOA;

		public EcheqService(IEcheqVisualizarPendientePagoConsumerMOA echeqVisualizarPendientePagoConsumerMOA)
		{
			this.echeqVisualizarPendientePagoConsumerMOA = echeqVisualizarPendientePagoConsumerMOA;
		}

		public List<EcheqVisualizacionPendientePago> ObtenerPendientePago(string proveedor, string fechaInicio, string fechaFin)
        {
            List<FechaWS> fechas = CommonService.toDateList(fechaInicio, fechaFin);

            List<EcheqVisualizacionPendientePago> result =  echeqVisualizarPendientePagoConsumerMOA.Request(proveedor, fechas);
         
            return result;
        }
    }
}
