using System;
using SustitucionMOAModel.Dto;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.EcheqModificarContratoWebServiceMOA;

namespace SustitucionMOAWS.WSConsumers
{
    class EcheqModificarContratoConsumerMOA : IEcheqModificarContratoConsumerMOA
    {
        SI_MPRFC_MOAOP_MOD_CONTRATOClient service = new SI_MPRFC_MOAOP_MOD_CONTRATOClient();

        public EcheqModificarContratoConsumerMOA()
        {
            service = new SI_MPRFC_MOAOP_MOD_CONTRATOClient();
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
        }

        public ResultadoGenerico Request(string IM_CONTRATO, string IM_CUENTA_MRP, string IM_ZLSCH)
        {
            try
            {
                string response = service.SI_MPRFC_MOAOP_MOD_CONTRATO(IM_CONTRATO, IM_CUENTA_MRP, IM_ZLSCH, out string EX_MENSAJE);

                ResultadoGenerico resultado = new ResultadoGenerico();

                if (response != "Ok")
                {
                    resultado.Error("", response);
                }

                return resultado;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    internal interface IEcheqModificarContratoConsumerMOA
    {
    }
}
