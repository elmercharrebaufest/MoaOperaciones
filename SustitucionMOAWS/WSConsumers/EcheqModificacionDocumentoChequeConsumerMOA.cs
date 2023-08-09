using System;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Echeq;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.EcheqModificacionDocumentoChequeWebServiceMOA;
using SustitucionMOAWS.EcheqModificarContratoWebServiceMOA;
using SustitucionMOAWS.Logger;

namespace SustitucionMOAWS.WSConsumers
{
    public class EcheqModificacionDocumentoChequeConsumerMOA : IEcheqModificacionDocumentoChequeConsumerMOA
    {
        SI_MPRFC_MODI_DOC_CHEQUEClient service = new SI_MPRFC_MODI_DOC_CHEQUEClient();

        public EcheqModificacionDocumentoChequeConsumerMOA()
        {
            service = new SI_MPRFC_MODI_DOC_CHEQUEClient();
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
        }

        public ResultadoGenerico Request(string IM_CONTRATO, string IM_DOCUMENTO, string IM_EJERCICIO, string IM_FECHA, string IM_HORA, string IM_PEDIDO, string IM_PROVEEDOR, string IM_REFERENCIA, string IM_SOCIEDAD, string IM_USUARIO, string IM_ZLSCH)
        {
            try
            {
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
