using System;
using System.Configuration;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Echeq;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.EcheqModificacionDocumentoChequeWebServiceMOA;
using SustitucionMOAWS.EcheqModificarContratoWebServiceMOA;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MOAOP;
using static Google.Apis.Requests.BatchRequest;

namespace SustitucionMOAWS.WSConsumers
{
    public class EcheqModificacionDocumentoChequeConsumerMOA : IEcheqModificacionDocumentoChequeConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public EcheqModificacionDocumentoChequeConsumerMOA()
        {
        }

        public ResultadoGenerico Request(string IM_CONTRATO, string IM_DOCUMENTO, string IM_EJERCICIO, string IM_FECHA, string IM_HORA, string IM_PEDIDO, string IM_PROVEEDOR, string IM_REFERENCIA, string IM_SOCIEDAD, string IM_USUARIO, string IM_ZLSCH)
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
                    var request = new Z_MPRFC_MODI_DOC_CHEQUE()
                    {
                        IM_CONTRATO = IM_CONTRATO,
                        IM_DOCUMENTO = IM_DOCUMENTO,
                        IM_EJERCICIO = IM_EJERCICIO,
                        IM_FECHA = IM_FECHA,
                        IM_HORA = IM_HORA,
                        IM_PEDIDO = IM_PEDIDO,
                        IM_PROVEEDOR = IM_PROVEEDOR,
                        IM_REFERENCIA = IM_REFERENCIA,
                        IM_SOCIEDAD = IM_SOCIEDAD,
                        IM_USUARIO = IM_USUARIO,
                        IM_ZLSCH = IM_ZLSCH
                    };
                    Log.Info($"SAP sin PI Z_MPRFC_MODI_DOC_CHEQUE request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MPRFC_MODI_DOC_CHEQUE(request);
                    Log.Info($"SAP sin PI Z_MPRFC_MODI_DOC_CHEQUE response");
                    Log.Info(response.ToXml());
                    
                    ResultadoGenerico resultado = new ResultadoGenerico();
                    if (response.EX_SALIDA != "Datos actualizados correctamente")
                    {
                        resultado.Error("", response.EX_SALIDA);
                    }
                    return resultado;
                }
                else
                {
                    SI_MPRFC_MODI_DOC_CHEQUEClient service = new SI_MPRFC_MODI_DOC_CHEQUEClient();
                    service = new SI_MPRFC_MODI_DOC_CHEQUEClient();
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();

                    IM_USUARIO = string.IsNullOrEmpty(IM_USUARIO) ? "moaoperaciones" : IM_USUARIO;
                    Log.Info($"SI_MPRFC_MODI_DOC_CHEQUE Request: {new { IM_CONTRATO, IM_DOCUMENTO, IM_EJERCICIO, IM_FECHA, IM_HORA, IM_PEDIDO, IM_PROVEEDOR, IM_REFERENCIA, IM_SOCIEDAD, IM_USUARIO, IM_ZLSCH }}");
                    string response = service.SI_MPRFC_MODI_DOC_CHEQUE(IM_CONTRATO, IM_DOCUMENTO, IM_EJERCICIO, IM_FECHA, IM_HORA, IM_PEDIDO, IM_PROVEEDOR, IM_REFERENCIA, IM_SOCIEDAD, IM_USUARIO, IM_ZLSCH);
                    Log.Info($"SI_MPRFC_MODI_DOC_CHEQUE Response: {response}");

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

    public interface IEcheqModificacionDocumentoChequeConsumerMOA
    {
        ResultadoGenerico Request(string IM_CONTRATO, string IM_DOCUMENTO, string IM_EJERCICIO, string IM_FECHA, string IM_HORA, string IM_PEDIDO, string IM_PROVEEDOR, string IM_REFERENCIA, string IM_SOCIEDAD, string IM_USUARIO, string IM_ZLSCH);
    }
}
