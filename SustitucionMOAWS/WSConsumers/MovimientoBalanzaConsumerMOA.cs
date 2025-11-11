using SustitucionMOAModel.Models.WSMapMOA.Balanza;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.MovimientoBalanzaWebServiceMOA;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MOAOP;
using System;
using System.Configuration;

namespace SustitucionMOAWS.WSConsumers
{
    public class MovimientoBalanzaConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserSinPI"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassSinPI"];

        public MovimientoBalanzaMOAResponse request(string fechaMov, string fechaCont, string centro, string almacenOrigen, string almacenSap, string materialSap, decimal cantidad)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var request = new ZMM_RFC_MOV_311()
                    {
                        EX_BLDAT = fechaMov,
                        EX_BUDAT = fechaCont,
                        EX_CHARG = "",
                        EX_LGORT_DEST = almacenSap,
                        EX_LGORT_ORIG = almacenOrigen,
                        EX_MATNR = materialSap,
                        EX_MEINS = "KG",
                        EX_MENGE = cantidad,
                        EX_TCODE = "MB1B",
                        EX_TESTRUN = "",
                        EX_WERKS = centro
                    };
                    Log.Info($"SAP sin PI ZMM_RFC_MOV_311 request");
                    Log.Info(request.ToXml());
                    var response = agent.ZMM_RFC_MOV_311(request);
                    SapLogHelper.LogResponse(response.ToXml(), "ZMM_RFC_MOV_311");
                    MovimientoBalanzaMOAResponse result = Map(response.IM_MATDOCUMENTYEAR, response.IM_MATERIALDOCUMENT, response.IM_MESSAGE);
                    return result;
                }
                else
                {
                    SI_MPMF_MOAOP_MOVIMIENTOS_BALANZAClient service = new SI_MPMF_MOAOP_MOVIMIENTOS_BALANZAClient();
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    string imMaterialdocument = "";
                    string imMessage = "";
                    string imMatdocumentyear = service.SI_MPMF_MOAOP_MOVIMIENTOS_BALANZA(fechaMov, fechaCont, "", almacenSap, almacenOrigen, materialSap, "KG", cantidad, "MB1B", "", centro, out imMaterialdocument, out imMessage);
                    MovimientoBalanzaMOAResponse result = Map(imMatdocumentyear, imMaterialdocument, imMessage);
                    return result;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private MovimientoBalanzaMOAResponse Map(string imMatdocumentyear, string imMaterialdocument, string imMessage)
        {
            MovimientoBalanzaMOAResponse result = new MovimientoBalanzaMOAResponse();

            result.imMatdocumentyear = imMatdocumentyear;
            result.imMaterialdocument = imMaterialdocument;
            result.imMessage = imMessage;

            return result;
        }
    }
}
