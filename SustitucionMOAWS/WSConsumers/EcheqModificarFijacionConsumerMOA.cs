using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.EcheqModificarFijacionWebServiceMOA;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MOAOP;
using System;
using System.Configuration;

namespace SustitucionMOAWS.WSConsumers
{
    public class EcheqModificarFijacionConsumerMOA : IEcheqModificarFijacionConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserSinPI"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassSinPI"];

        public EcheqModificarFijacionConsumerMOA()
        {

        }

        public ResultadoGenerico Request(string IM_CONTRATO, string IM_CUENTA_MRP, string IM_FIJACION, string IM_ZLSCH)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var request = new Z_MPRFC_MOAOP_MOD_FIJACION()
                    {
                        IM_CONTRATO =IM_CONTRATO,
                        IM_CUENTA_MRP = IM_CUENTA_MRP,
                        IM_FIJACION = IM_FIJACION,
                        IM_ZLSCH = IM_ZLSCH
                    };

                    Log.Info($"SAP sin PI Z_MPRFC_MOAOP_MOD_FIJACION request");
                    Log.Info(request.ToXml());

                    var response = agent.Z_MPRFC_MOAOP_MOD_FIJACION(request);
                    Log.Info($"SAP sin PI Z_MPRFC_MOAOP_MOD_FIJACION response");
                    Log.Info(response.ToXml());

                    ResultadoGenerico resultado = new ResultadoGenerico();
                    if (response.EX_MENSAJE != "Se actualizaron los datos correctamente")
                    {
                        resultado.Error("", response.EX_MENSAJE);
                    }
                    return resultado;

                }
                else
                {
                    SI_MPRFC_MOAOP_MOD_FIJACIONClient service = new SI_MPRFC_MOAOP_MOD_FIJACIONClient();
                    service = new SI_MPRFC_MOAOP_MOD_FIJACIONClient();
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();

                    Log.Info($"SI_MPRFC_MOAOP_MOD_FIJACION Request: {new { IM_CONTRATO, IM_CUENTA_MRP, IM_FIJACION, IM_ZLSCH }}");
                    string response = service.SI_MPRFC_MOAOP_MOD_FIJACION(IM_CONTRATO, IM_CUENTA_MRP, IM_FIJACION, IM_ZLSCH);
                    Log.Info($"SI_MPRFC_MOAOP_MOD_FIJACION Response: {response}");

                    ResultadoGenerico resultado = new ResultadoGenerico();
                    if (response != "Se actualizaron los datos correctamente")
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

    public interface IEcheqModificarFijacionConsumerMOA
    {
        ResultadoGenerico Request(string IM_CONTRATO, string IM_CUENTA_MRP, string IM_FIJACION, string IM_ZLSCH);
    }
}
