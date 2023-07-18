using SustitucionMOAModel.Enums.MoaWS.OrdenCargaWS;
using SustitucionMOAWS.OrdenCargaControlSAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.ResponseHandler.OrdenCarga
{
    public class ControlCargaResponseHandler
    {
        private readonly string logResponse = string.Empty;
        private readonly List<ControlCargaResEnum> respuestasSap = new List<ControlCargaResEnum>();
        //private List<string> contratosSap = new List<string>();

        //public bool TieneMultiplesContratos { get; private set; } = false;

        public ControlCargaResponseHandler(ZMPES7060[] mensajesSap)
        {
            foreach (var item in mensajesSap)
            {
                var resp = item.MENSAJE;
                logResponse += resp + ". ";
                //if (resp.Contains("|"))
                //{
                //    TieneMultiplesContratos = true;
                //    contratosSap.Add(resp);
                //}
                //else
                //{
                //    respuestasSap.Add(ResponseConverter.GetOrdenCargaControlCargaResponse(resp));
                //}
                respuestasSap.Add(ResponseConverter.GetOrdenCargaControlCargaResponse(resp));
            }
        }

        /// <summary>
        /// Si el cliente tiene varios contratos, devuelve los números. Sino, dispara excepción
        /// </summary>
        /// <returns>Lista de números de contratos</returns>
        //public List<string> ObtenerNumerosContratos()
        //{
        //    if (contratosSap.Count <= 1)
        //    {
        //        throw new Exception("La respuesta de OrdenCarga no tiene múltiples contratos - " + logResponse);
        //    }
        //    return contratosSap.Select(c => c.Split('|')[0]).ToList();
        //}

        /// <summary>
        /// Obtener la respuesta recibida de SAP, si existe exactamente una. Caso contrario, devuelve excepción.
        /// Usar cuando no se contempla recibir más de una respuesta. Sino, usar método TieneRespuesta
        /// </summary>
        public ControlCargaResEnum ObtenerRespuestaUnica()
        {
            if (respuestasSap.Count == 0)
            {
                throw new Exception("No se encontró respuesta para ControlCarga - " + logResponse);
            }
            if (respuestasSap.Count > 1)
            {
                throw new Exception("No hay respuesta única para ControlCarga - " + logResponse);
            }
            return respuestasSap.First();
        }

        public bool TieneRespuesta(ControlCargaResEnum respuestaSap)
        {
            return respuestasSap.Contains(respuestaSap);
        }

        public string GetCodigoDeRespuesta(ControlCargaResEnum valor)
        {
            return ResponseConverter.GetCodigoControlCarga(valor);
        }
    }
}
