using System;
using System.Configuration;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.EcheqAnularAperturaChequeWebServiceMOA;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MOAOP;

namespace SustitucionMOAWS.WSConsumers
{
    public class EcheqAnularAperturaChequeConsumerMOA : IEcheqAnularAperturaChequeConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserS4"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassS4"];

        public EcheqAnularAperturaChequeConsumerMOA()
        {

        }

        public ResultadoGenerico Request(string IM_CHEQUE, string IM_DOCUMENTO, string IM_EJERCICIO, string IM_FECHA_ANULACION, string IM_HORA_ANULACION, string IM_SOCIEDAD, string IM_USUARIO)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;
                    IM_USUARIO = string.IsNullOrEmpty(IM_USUARIO) ? "moaoperacion" : IM_USUARIO;
                    IM_USUARIO = IM_USUARIO.Length > 12 ? IM_USUARIO.Substring(0, 12) : IM_USUARIO;

                    var request = new Z_MPRFC_ANULAR_APERTURA_CHEQUE(){
                        IM_CHEQUE = IM_CHEQUE,
                        IM_DOCUMENTO = IM_DOCUMENTO,
                        IM_EJERCICIO = IM_EJERCICIO,
                        IM_FECHA_ANULACION = IM_FECHA_ANULACION,
                        IM_HORA_ANULACION = IM_HORA_ANULACION,
                        IM_SOCIEDAD = IM_SOCIEDAD,
                        IM_USUARIO_ANULACION = IM_USUARIO
                    };
                    Log.Info($"SAP sin PI Z_MPRFC_ANULAR_APERTURA_CHEQUE request");
                    Log.Info(request.ToXml());

                    var response = agent.Z_MPRFC_ANULAR_APERTURA_CHEQUE(request);

                    Log.Info($"SAP sin PI Z_MPRFC_ANULAR_APERTURA_CHEQUE response");
                    Log.Info(response.ToXml());

                    ResultadoGenerico resultado = new ResultadoGenerico();

                    if (response.EX_MENSAJE != "Datos actualizados correctamente")
                    {
                        resultado.Error("", response.EX_MENSAJE);
                    }

                    return resultado;


                }
                else
                {
                    SI_MPRFC_ANULAR_APERTURA_CHEQUEClient service = new SI_MPRFC_ANULAR_APERTURA_CHEQUEClient();
                    service = new SI_MPRFC_ANULAR_APERTURA_CHEQUEClient();
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    IM_USUARIO = string.IsNullOrEmpty(IM_USUARIO) ? "moaoperaciones" : IM_USUARIO;
                    Log.Info($"SI_MPRFC_ANULAR_APERTURA_CHEQUE Request: {new { IM_CHEQUE, IM_DOCUMENTO, IM_EJERCICIO, IM_FECHA_ANULACION, IM_HORA_ANULACION, IM_SOCIEDAD, IM_USUARIO }}");
                    string response = service.SI_MPRFC_ANULAR_APERTURA_CHEQUE(IM_CHEQUE, IM_DOCUMENTO, IM_EJERCICIO, IM_FECHA_ANULACION, IM_HORA_ANULACION, IM_SOCIEDAD, IM_USUARIO);
                    Log.Info($"SI_MPRFC_ANULAR_APERTURA_CHEQUE Response: {response}");
                    ResultadoGenerico resultado = new ResultadoGenerico();

                    if (response != "Datos actualizados correctamente")
                    {
                        resultado.Error("", response);
                    }

                    return resultado;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }

    public interface IEcheqAnularAperturaChequeConsumerMOA
    {
        ResultadoGenerico Request(string IM_CHEQUE, string IM_DOCUMENTO, string IM_EJERCICIO, string IM_FECHA_ANULACION, string IM_HORA_ANULACION, string IM_SOCIEDAD, string IM_USUARIO);
    }
}
