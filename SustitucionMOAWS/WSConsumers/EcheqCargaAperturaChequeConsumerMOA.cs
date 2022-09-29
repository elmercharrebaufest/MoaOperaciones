using System;
using SustitucionMOAModel.Dto;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.EcheqCargaAperturaChequeWebServiceMOA;

namespace SustitucionMOAWS.WSConsumers
{
    class EcheqCargaAperturaChequeConsumerMOA : IEcheqCargaAperturaChequeConsumerMOA
    {
        SI_MPRFC_CARGA_APERTURA_CHEQUEClient service = new SI_MPRFC_CARGA_APERTURA_CHEQUEClient();

        public EcheqCargaAperturaChequeConsumerMOA()
        {
            service = new SI_MPRFC_CARGA_APERTURA_CHEQUEClient();
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
        }

        public ResultadoGenerico Request(string IM_CHEQUE, string IM_CONTRATO, string IM_DOCUMENTO, string IM_EJERCICIO, string IM_FECHA, string IM_HORA, decimal IM_IMPORTE, string IM_MONEDA, string IM_PEDIDO, string IM_PROVEEDOR, string IM_REFERENCIA, string IM_SOCIEDAD, string IM_USUARIO)
        {
            try
            {
                string response = service.SI_MPRFC_CARGA_APERTURA_CHEQUE(IM_CHEQUE, IM_CONTRATO, IM_DOCUMENTO, IM_EJERCICIO, IM_FECHA, IM_HORA, IM_IMPORTE, IM_MONEDA, IM_PEDIDO, IM_PROVEEDOR, IM_REFERENCIA, IM_SOCIEDAD, IM_USUARIO);

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

    internal interface IEcheqCargaAperturaChequeConsumerMOA
    {
    }
}
