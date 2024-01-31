using System;
using SustitucionMOAModel.Dto;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.EcheqModificarContratoWebServiceMOA;
using SustitucionMOAWS.Logger;

namespace SustitucionMOAWS.WSConsumers
{
    public class EcheqModificarContratoConsumerMOA : IEcheqModificarContratoConsumerMOA
    {
        SI_MPRFC_MOAOP_MOD_CONTRATOClient service = new SI_MPRFC_MOAOP_MOD_CONTRATOClient();

        public EcheqModificarContratoConsumerMOA()
        {
            service = new SI_MPRFC_MOAOP_MOD_CONTRATOClient();
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
        }

        //IM_CUENTA_MRP: es el CBU por ahora se envia un string vacio
        //IM_ZLSCH: es para indicar si tiene la marca o no de cheque. En el caso de que tengo se envia un "=" y sino tiene "" (string vacio);

        public ResultadoGenerico Request(string IM_CONTRATO, string IM_CUENTA_MRP, string IM_ZLSCH)
        {
            try
            {
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


