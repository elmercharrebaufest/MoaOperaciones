using SustitucionMOAModel.Dto;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.EcheqModificarContratoWebServiceMOA;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MOAOP;
using System;
using System.Configuration;

namespace SustitucionMOAWS.WSConsumers
{
    public class EcheqModificarContratoConsumerMOA : IEcheqModificarContratoConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserS4"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassS4"];

        public EcheqModificarContratoConsumerMOA()
        {

        }

        //IM_CUENTA_MRP: es el CBU por ahora se envia un string vacio
        //IM_ZLSCH: es para indicar si tiene la marca o no de cheque. En el caso de que tengo se envia un "=" y sino tiene "" (string vacio);

        public ResultadoGenerico Request(string IM_CONTRATO, string IM_CUENTA_MRP, string IM_ZLSCH)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var request = new Z_MPRFC_MOAOP_MOD_CONTRATO()
                    {
                        IM_CONTRATO = IM_CONTRATO,
                        IM_CUENTA_MRP = IM_CUENTA_MRP,
                        IM_ZLSCH = IM_ZLSCH
                    };

                    Log.Info($"SAP sin PI Z_MPRFC_MOAOP_MOD_CONTRATO request");
                    Log.Info(request.ToXml());

                    var response = agent.Z_MPRFC_MOAOP_MOD_CONTRATO(request);

                    Log.Info($"SAP sin PI Z_MPRFC_MOAOP_MOD_CONTRATO response");
                    Log.Info(response.ToXml());
                    ResultadoGenerico resultado = new ResultadoGenerico();
                    if (response.EX_MENSAJE != "Ok")
                    {
                        resultado.Error("", response.EX_MENSAJE);
                    }
                    return resultado;
                }
                else
                {
                    SI_MPRFC_MOAOP_MOD_CONTRATOClient service = new SI_MPRFC_MOAOP_MOD_CONTRATOClient();
                    service = new SI_MPRFC_MOAOP_MOD_CONTRATOClient();
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();

                    Log.Info($"SI_MPRFC_MOAOP_MOD_CONTRATO Request: {new { IM_CONTRATO, IM_CUENTA_MRP, IM_ZLSCH }}");
                    string response = service.SI_MPRFC_MOAOP_MOD_CONTRATO(IM_CONTRATO, IM_CUENTA_MRP, IM_ZLSCH, out string EX_MENSAJE);
                    Log.Info($"SI_MPRFC_MOAOP_MOD_CONTRATO Response: {response}");

                    ResultadoGenerico resultado = new ResultadoGenerico();

                    if (EX_MENSAJE != "Ok")
                    {
                        resultado.Error("", EX_MENSAJE);
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

    public interface IEcheqModificarContratoConsumerMOA
    {
        ResultadoGenerico Request(string IM_CONTRATO, string IM_CUENTA_MRP, string IM_ZLSCH);
    }
}


