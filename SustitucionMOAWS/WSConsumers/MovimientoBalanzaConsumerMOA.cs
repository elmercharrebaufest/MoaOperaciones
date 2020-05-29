using System;
using SustitucionMOAModel.Models.WSMapMOA.Balanza;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.MovimientoBalanzaWebServiceMOA;

namespace SustitucionMOAWS.WSConsumers
{
    public class MovimientoBalanzaConsumerMOA
    {
        SI_MPMF_MOAOP_MOVIMIENTOS_BALANZAClient service = new SI_MPMF_MOAOP_MOVIMIENTOS_BALANZAClient();

        public MovimientoBalanzaMOAResponse request(string fechaMov, string fechaCont, string centro, string almacenOrigen, string almacenSap, string materialSap, decimal cantidad)
        {
            try
            {
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                string imMaterialdocument = "";
                string imMessage = "";
                string imMatdocumentyear = service.SI_MPMF_MOAOP_MOVIMIENTOS_BALANZA(fechaMov, fechaCont, "", almacenSap, almacenOrigen, materialSap,"KG", cantidad, "MB1B", "", centro, out imMaterialdocument, out imMessage);
                MovimientoBalanzaMOAResponse result = map(imMatdocumentyear, imMaterialdocument, imMessage);
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private MovimientoBalanzaMOAResponse map(string imMatdocumentyear, string imMaterialdocument, string imMessage)
        {
            MovimientoBalanzaMOAResponse result = new MovimientoBalanzaMOAResponse();

            result.imMatdocumentyear = imMatdocumentyear;
            result.imMaterialdocument = imMaterialdocument;
            result.imMessage = imMessage;
            
            return result;
        }
    }
}
