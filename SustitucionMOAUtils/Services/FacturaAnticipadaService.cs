using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.OrdenCarga;
using SustitucionMOAModel.Util;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAUtils.Services
{
    public class FacturaAnticipadaService : IFacturaAnticipadaService
    {
        private readonly IOrdenCargaConsumerMOA _consumerOrdenCarga;
        public FacturaAnticipadaService(IOrdenCargaConsumerMOA consumer)
        {
            _consumerOrdenCarga = consumer;
        }
        public List<string> ObtenerFacturasDeContrato(string numeroContrato)
        {
            var request = new OrdenCargaVisualizarClienteWSMOARequest
            {
                Contrato = numeroContrato,
                TipoContrato = TipoContratoFAS.ANTICIPADO,
                Fechas = ObtenerFechas()
            };
            var contratoSAP = _consumerOrdenCarga.OrdenCargaVisualizarClienteExecute(request).Resultados.First();

            if (contratoSAP == null)
                throw new InfoCustomException("No se encontró el contrato");


            return contratoSAP.Detalles
                .Where(det => !string.IsNullOrEmpty(det.FacturaLegal))
                .Select(det => det.FacturaLegal)
                .ToList();
        }

        private List<FechaWS> ObtenerFechas()
        {
            return new List<FechaWS>
                {
                    new FechaWS
                    {
                        fechaFin = DateTime.Now,
                        fechaInicio = DateTime.Parse(Constante.FECHA_BASICA)
                    }
                };
        }
    }
}
