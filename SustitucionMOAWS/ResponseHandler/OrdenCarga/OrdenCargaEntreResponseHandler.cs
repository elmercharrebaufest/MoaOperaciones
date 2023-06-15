using System;
using SustitucionMOAModel.Enums.MoaWS.OrdenCargaWS;

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

        public CrearEntregaResEnum GetResultado()
        {
            switch (respuestaSap)
            {
                case "OE-00": return CrearEntregaResEnum.OK;
                case "OE-01": return CrearEntregaResEnum.NoExisteTransportista;
                case "OE-02": return CrearEntregaResEnum.EntregaCreadaErrorAlInsertarOE02;
                case "OE-03": return CrearEntregaResEnum.EntregaCreadaErrorAlInsertarOE03;
                case "OE-04": return CrearEntregaResEnum.FaltaCargarKmEnContrato;
                default: return CrearEntregaResEnum.ErrorRespuestaInesperadaDeSap;
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
