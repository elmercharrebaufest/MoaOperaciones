using System;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.EcheqModificarFijacionWebServiceMOA;
using SustitucionMOAModel.Dto;
using SustitucionMOAWS.Logger;

namespace SustitucionMOAWS.WSConsumers
{
    public class EcheqModificarFijacionConsumerMOA : IEcheqModificarFijacionConsumerMOA
    {
        SI_MPRFC_MOAOP_MOD_FIJACIONClient service = new SI_MPRFC_MOAOP_MOD_FIJACIONClient();

        public EcheqModificarFijacionConsumerMOA()
        {
            service = new SI_MPRFC_MOAOP_MOD_FIJACIONClient();
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
        }

        public ResultadoGenerico Request(string IM_CONTRATO, string IM_CUENTA_MRP, string IM_FIJACION, string IM_ZLSCH)
        {
            try
            {
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
