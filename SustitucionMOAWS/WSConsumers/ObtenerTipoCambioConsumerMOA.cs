using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ObtenerCecoSolpWebServiceMOA;
using SustitucionMOAWS.ObtenerTipoCambioWebServiceMOA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WSConsumers
{
    public class ObtenerTipoCambioConsumerMOA : IObtenerTipoCambioConsumerMOA
    {
        private readonly SI_MMRFC_OBTENER_TIPO_CAMBIOClient service;

        public ObtenerTipoCambioConsumerMOA()
        {
            service = new SI_MMRFC_OBTENER_TIPO_CAMBIOClient();
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
        }

        public ObtenerTipoCambioConsumerMOAResponse Request(string fecha, string monedaDestino, string monedaOrigen)
        {
            try
            {
                //string IM_FECHA = fecha;
                //string IM_MONEDA_DESTINO = monedaDestino;
                //string IM_MONEDA_ORIGEN = monedaOrigen;

                BAPIRET1 EX_RETURN = new BAPIRET1();
                BAPI1093_0 EX_TIPO_CAMBIO = new BAPI1093_0();


                var result = service.SI_MMRFC_OBTENER_TIPO_CAMBIO(fecha, monedaDestino, monedaOrigen, out EX_RETURN, out EX_TIPO_CAMBIO);

                return map(result, EX_RETURN, EX_TIPO_CAMBIO);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private ObtenerTipoCambioConsumerMOAResponse map(string result, BAPIRET1 EX_RETURN, BAPI1093_0 EX_TIPO_CAMBIO)
        {
            ObtenerTipoCambioConsumerMOAResponse resultado = new ObtenerTipoCambioConsumerMOAResponse();

            if (result != "200")
            {
                throw new WSCustomException(EX_RETURN.MESSAGE);
            }

            resultado.Fecha = EX_TIPO_CAMBIO.VALID_FROM;
            resultado.TipoCambio = EX_TIPO_CAMBIO.EXCH_RATE;
            resultado.MonedaOrigen = EX_TIPO_CAMBIO.FROM_CURR;
            resultado.MonedaDestino = EX_TIPO_CAMBIO.TO_CURRNCY;

            return resultado;
        }
                

    }
}



