using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.ObtenerCecoSolpWebServiceMOA;
using SustitucionMOAWS.ObtenerTipoCambioWebServiceMOA;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WSConsumers
{
    public class ObtenerTipoCambioConsumerMOA : IObtenerTipoCambioConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserS4"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassS4"];

        public ObtenerTipoCambioConsumerMOA()
        {

        }

        public ObtenerTipoCambioConsumerMOAResponse Request(string fecha, string monedaDestino, string monedaOrigen)
        {
            try
            {

                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_COMPRAS_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var request = new Z_MMRFC_OBTENER_TIPO_CAMBIO(){ 
                        IM_FECHA = fecha,
                        IM_MONEDA_DESTINO = monedaDestino,
                        IM_MONEDA_ORIGEN = monedaOrigen,
                    };
                    Log.Info($"SAP sin PI Z_MMRFC_OBTENER_TIPO_CAMBIO request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MMRFC_OBTENER_TIPO_CAMBIO(request);
                    Log.Info($"SAP sin PI Z_MMRFC_OBTENER_TIPO_CAMBIO response");
                    Log.Info(response.ToXml());
                    return MapSinPI(response);
                }
                else
                {
                    SI_MMRFC_OBTENER_TIPO_CAMBIOClient service = new SI_MMRFC_OBTENER_TIPO_CAMBIOClient();
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    ObtenerTipoCambioWebServiceMOA.BAPIRET1 EX_RETURN = new ObtenerTipoCambioWebServiceMOA.BAPIRET1();
                    ObtenerTipoCambioWebServiceMOA.BAPI1093_0 EX_TIPO_CAMBIO = new ObtenerTipoCambioWebServiceMOA.BAPI1093_0();
                    var result = service.SI_MMRFC_OBTENER_TIPO_CAMBIO(fecha, monedaDestino, monedaOrigen, out EX_RETURN, out EX_TIPO_CAMBIO);
                    return Map(result, EX_RETURN, EX_TIPO_CAMBIO);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private ObtenerTipoCambioConsumerMOAResponse MapSinPI(Z_MMRFC_OBTENER_TIPO_CAMBIOResponse response)
        {
            ObtenerTipoCambioConsumerMOAResponse resultado = new ObtenerTipoCambioConsumerMOAResponse();

            if (response.EX_EXITO != "200")
            {
                throw new WSCustomException(response.EX_RETURN.MESSAGE);
            }

            resultado.Fecha = response.EX_TIPO_CAMBIO.VALID_FROM;
            resultado.TipoCambio = response.EX_TIPO_CAMBIO.EXCH_RATE;
            resultado.MonedaOrigen = response.EX_TIPO_CAMBIO.FROM_CURR;
            resultado.MonedaDestino = response.EX_TIPO_CAMBIO.TO_CURRNCY;

            return resultado;
        }

        private ObtenerTipoCambioConsumerMOAResponse Map(string result, ObtenerTipoCambioWebServiceMOA.BAPIRET1 EX_RETURN, ObtenerTipoCambioWebServiceMOA.BAPI1093_0 EX_TIPO_CAMBIO)
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



