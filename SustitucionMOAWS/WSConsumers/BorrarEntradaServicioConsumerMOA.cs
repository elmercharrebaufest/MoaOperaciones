using SustitucionMOAModel.Dto.OrdenesCompra;
using SustitucionMOARepositorio;
using SustitucionMOAWS.BorrarEntradaServicioNuevoWebServiceMOA;
using SustitucionMOAWS.BorrarEntradaServicioWebServiceMOA;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS;
using System;
using System.Configuration;
using System.Linq;
using System.Security.Policy;

namespace SustitucionMOAWS.WSConsumers
{
    public class BorrarEntradaServicioConsumerMOA : IBorrarEntradaServicioConsumerMOA
    {

        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        //private const string COMP_CODE = "MOA";
        //private readonly IRepositorio repositorio;

        public BorrarEntradaServicioConsumerMOA()
        {
            //this.repositorio = repositorio;
        }

        public string BorrarEntradaServicio(string nroES, string fechaContabilizacion)
        {
            try
            {

                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_COMPRAS_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;
                    WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIRET2[] RETURN = new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIRET2[] { };

                    var request = new Z_MMRFC_BORRAR_HES()
                    {
                        ENTRYSHEET = nroES,
                        I_BUDATUM = fechaContabilizacion,
                        RETURN = RETURN,
                    };

                    Log.Info($"SAP sin PI Z_MMRFC_BORRAR_HES request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MMRFC_BORRAR_HES(request);
                    Log.Info($"SAP sin PI Z_MMRFC_BORRAR_HES response");
                    Log.Info(response.ToXml());
                    return MapSinPI(response);
                }
                else
                {
                    SI_MMRFC_BORRAR_HESClient service;
                    var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=SI_MMRFC_BORRAR_HES&amp;interfaceNamespace=urn:OPERACIONES";
                    service = new SI_MMRFC_BORRAR_HESClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();

                    string ENTRYSHEET = nroES;
                    BorrarEntradaServicioNuevoWebServiceMOA.BAPIRET2[] RETURN = new BorrarEntradaServicioNuevoWebServiceMOA.BAPIRET2[] { };
                    service.SI_MMRFC_BORRAR_HES(nroES, fechaContabilizacion, ref RETURN);
                    return Map(RETURN);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private string Map(BorrarEntradaServicioNuevoWebServiceMOA.BAPIRET2[] RETURN)
        {
            //string result = RETURN[0].MESSAGE;
            string result = string.Join(Environment.NewLine, RETURN.Select(r => r.MESSAGE));
            //string[] messages = RETURN.Select(r => r.MESSAGE).ToArray();
            return result;
        }
        private string MapSinPI(Z_MMRFC_BORRAR_HESResponse response)
        {
            string result = string.Join(Environment.NewLine, response.RETURN.Select(r => r.MESSAGE));
            return result;
        }
    }

    public interface IBorrarEntradaServicioConsumerMOA
    {
    }
}
