using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
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
            try
            {
                List<FechaWS> fechas = CommonService.toDateList(fechaInicio, fechaFin);

                List<EcheqVisualizacionPendientePago> result = echeqVisualizarPendientePagoConsumerMOA.Request(proveedor, fechas);

                return result;
            }
            catch (ValidationCustomException e)
            {
                throw e;
            }
            catch (InfoCustomException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }
    }
}
