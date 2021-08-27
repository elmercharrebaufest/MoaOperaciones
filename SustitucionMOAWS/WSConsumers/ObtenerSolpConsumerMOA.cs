using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ObtenerSolpWebServiceMOA;
using System;

namespace SustitucionMOAWS.WSConsumers
{
    public class ObtenerSolpConsumerMOA : IObtenerSolpConsumerMOA
    {
        SI_MMRFC_OBTENER_SOLPEDClient service;
        private const string COMP_CODE = "MOA";

        public ObtenerSolpConsumerMOA()
        {
            service = new SI_MMRFC_OBTENER_SOLPEDClient();
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
        }

        public object request(ObtenerSolpRequest req)
        {
            try
            {
                var solicitarTipoImputacion = getBoolSap(req.ObtenerImputacion);
                ZMPES5640[] tipoImputaciones;

                //service.SI_MMRFC_OBTENER_SOLPED(solicitarTipoImputacion,,,,,,,,,,,,, out tipoImputaciones);

                throw new NotImplementedException();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private string getBoolSap(bool value)
        {
            return value ? "X" : string.Empty;
        }
    }

    public class ObtenerSolpRequest
    {
        public bool ObtenerImputacion { get; set; }
    }
}
