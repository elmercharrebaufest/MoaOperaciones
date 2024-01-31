using System;
using SustitucionMOAModel.Dto;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.EcheqAnularAperturaChequeWebServiceMOA;
using SustitucionMOAWS.Logger;

namespace SustitucionMOAWS.WSConsumers
{
    public class EcheqAnularAperturaChequeConsumerMOA : IEcheqAnularAperturaChequeConsumerMOA
    {
        SI_MPRFC_ANULAR_APERTURA_CHEQUEClient service = new SI_MPRFC_ANULAR_APERTURA_CHEQUEClient();

        public EcheqAnularAperturaChequeConsumerMOA()
        {
            service = new SI_MPRFC_ANULAR_APERTURA_CHEQUEClient();
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
        }

        public ResultadoGenerico Request(string IM_CHEQUE, string IM_DOCUMENTO, string IM_EJERCICIO, string IM_FECHA_ANULACION, string IM_HORA_ANULACION, string IM_SOCIEDAD, string IM_USUARIO)
        {
            try
            {
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
