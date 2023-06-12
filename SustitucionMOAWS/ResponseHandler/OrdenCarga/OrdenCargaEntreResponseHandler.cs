using System;
using SustitucionMOAWS.Enum.OrdenCargaConsumer;

namespace SustitucionMOAWS.ResponseHandler.OrdenCarga
{
    public class OrdenCargaEntreResponseHandler
    {
        private readonly string respuestaSap;
        private readonly string numeroEntrega;

        public OrdenCargaEntreResponseHandler(string respuestaSap, string numeroEntrega)
        {
            this.respuestaSap = respuestaSap;
            this.numeroEntrega = numeroEntrega;
        }

        public OrdenCargaCrearEntrega GetResultado()
        {
            switch (respuestaSap)
            {
                case "OE-00": return OrdenCargaCrearEntrega.OK;
                case "OE-01": return OrdenCargaCrearEntrega.NoExisteTransportista;
                case "OE-02": return OrdenCargaCrearEntrega.EntregaCreadaErrorAlInsertarOE02;
                case "OE-03": return OrdenCargaCrearEntrega.EntregaCreadaErrorAlInsertarOE03;
                case "OE-04": return OrdenCargaCrearEntrega.FaltaCargarKmEnContrato;
                default: return OrdenCargaCrearEntrega.ErrorRespuestaInesperadaDeSap;
            }
        }

        public string GetNumeroEntrega()
        {
            if (string.IsNullOrEmpty(numeroEntrega))
            {
                throw new Exception("No hay número de entrega. Respuesta SAP: " + respuestaSap);
            }
            return numeroEntrega;
        }

        public string GetLogRespuestaSap()
        {
            return respuestaSap;
        }
    }
}
