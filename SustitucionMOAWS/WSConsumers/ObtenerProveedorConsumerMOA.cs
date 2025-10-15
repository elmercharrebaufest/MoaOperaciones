using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.ObtenerProveedorWebServiceMOA;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS;
using System;
using System.Configuration;
using System.Linq;
using System.Security.Policy;

namespace SustitucionMOAWS.WSConsumers
{
    public class ObtenerProveedorConsumerMOA : IObtenerProveedorConsumerMOA
    {
        private const string COMP_CODE = "MOA";
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserSinPI"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassSinPI"];

        public ObtenerProveedorConsumerMOA()
        {
        }


        public ObtenerProveedorWSMOAResponse ObtenerProveedor(string codigoProveedor)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_COMPRAS_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES5980[] IM_COMP_CODE = new WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES5980[] { new WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES5980 { SIGN = "I", OPTION = "EQ", LOW = "MOA" } };
                    WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES5880[] IM_NAME = new WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES5880[] { };
                    WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES6000[] IM_PURCH_ORG = new WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES6000[] { };
                    WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES5970[] IM_SORTL = new WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES5970[] { };
                    string IM_TYPE = "PROV";
                    WS_GAQ_sin_PI_DIRECT_COMPRAS.ROIJ_LIFNR_RSTR[] IM_VENDOR = new WS_GAQ_sin_PI_DIRECT_COMPRAS.ROIJ_LIFNR_RSTR[] { new WS_GAQ_sin_PI_DIRECT_COMPRAS.ROIJ_LIFNR_RSTR { OPTION = "EQ", SIGN = "I", LOW = codigoProveedor } };

                    Log.Info($"SAP sin PI Z_MMRFC_OBTENER_PROVEEDOR request");

                    var request = new Z_MMRFC_OBTENER_PROVEEDOR()
                    {
                        IM_COMP_CODE = IM_COMP_CODE,
                        IM_NAME = IM_NAME,
                        IM_PURCH_ORG = IM_PURCH_ORG,
                        IM_SORTL = IM_SORTL,
                        IM_TYPE = IM_TYPE,
                        IM_VENDOR = IM_VENDOR
                    };
                    Log.Info(request.ToXml());

                    var response = agent.Z_MMRFC_OBTENER_PROVEEDOR(request);
                    Log.Info($"SAP sin PI Z_MMRFC_OBTENER_PROVEEDOR response");

                    Log.Info(response.ToXml());

                    return MapSinPI(response);

                }
                else
                {
                    SI_MMRFC_OBTENER_PROVClient service;
                    var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=SI_MMRFC_OBTENER_PROV&amp;interfaceNamespace=urn%3AOPERACIONES";
                    service = new SI_MMRFC_OBTENER_PROVClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();

                    Log.Info($"ObtenerProveedor SI_MMRFC_OBTENER_PROVC: {codigoProveedor}");

                    ObtenerProveedorWebServiceMOA.ZMPES5980[] IM_COMP_CODE = new ObtenerProveedorWebServiceMOA.ZMPES5980[] { new ObtenerProveedorWebServiceMOA.ZMPES5980 { SIGN = "I", OPTION = "EQ", LOW = "MOA" } };
                    ObtenerProveedorWebServiceMOA.ZMPES5880[] IM_NAME = new ObtenerProveedorWebServiceMOA.ZMPES5880[] { };
                    ObtenerProveedorWebServiceMOA.ZMPES6000[] IM_PURCH_ORG = new ObtenerProveedorWebServiceMOA.ZMPES6000[] { };
                    ObtenerProveedorWebServiceMOA.ZMPES5970[] IM_SORTL = new ObtenerProveedorWebServiceMOA.ZMPES5970[] { };
                    string IM_TYPE = "PROV";
                    ObtenerProveedorWebServiceMOA.ROIJ_LIFNR_RSTR[] IM_VENDOR = new ObtenerProveedorWebServiceMOA.ROIJ_LIFNR_RSTR[] { new ObtenerProveedorWebServiceMOA.ROIJ_LIFNR_RSTR { OPTION = "EQ", SIGN = "I", LOW = codigoProveedor } };

                    var result = service.SI_MMRFC_OBTENER_PROV(
                        IM_COMP_CODE,
                        IM_NAME,
                        IM_PURCH_ORG,
                        IM_SORTL,
                         IM_TYPE,
                         IM_VENDOR,
                        out ObtenerProveedorWebServiceMOA.BAPIRET2[] EX_RETURN,
                        out ObtenerProveedorWebServiceMOA.ZMPES5990[] EX_VENDOR
                        );

                    return Map(result, EX_RETURN, EX_VENDOR);
                }

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private ObtenerProveedorWSMOAResponse MapSinPI(Z_MMRFC_OBTENER_PROVEEDORResponse response)
        {
            ObtenerProveedorWSMOAResponse resultado = null;
            if (response.EX_EXITO != "200" || response.EX_VENDOR.Length == 0)
                return null;


            resultado = new ObtenerProveedorWSMOAResponse
            {
                LAND1    = response.EX_VENDOR.First().LAND1,
                VENDOR   = response.EX_VENDOR.First().VENDOR,
                COMPCODE = response.EX_VENDOR.First().COMP_CODE,
                COUNTRY  = response.EX_VENDOR.First().COUNTRY,
                CURRENCY = response.EX_VENDOR.First().CURRENCY,
                KTOKK    = response.EX_VENDOR.First().KTOKK,
                MAIL     = response.EX_VENDOR.First().MAIL,
                NAME     = response.EX_VENDOR.First().NAME,
                PMNTTRMS = response.EX_VENDOR.First().PMNTTRMS,
                PURCHORG = response.EX_VENDOR.First().PURCH_ORG,
                SORT1    = response.EX_VENDOR.First().SORT1,
                SORT2    = response.EX_VENDOR.First().SORT2,
                STREET   = response.EX_VENDOR.First().STREET,
                TELEFONO = response.EX_VENDOR.First().TELEFONO,
                VERIFFEM = response.EX_VENDOR.First().VERIF_F_EM
            };

            return resultado;
        }
        private ObtenerProveedorWSMOAResponse Map(string result, ObtenerProveedorWebServiceMOA.BAPIRET2[] RETURN, ObtenerProveedorWebServiceMOA.ZMPES5990[] VENDOR)
        {
            ObtenerProveedorWSMOAResponse resultado = null;
            if (result != "200" || VENDOR.Length == 0)
                return null;


            resultado = new ObtenerProveedorWSMOAResponse
            {
                LAND1 = VENDOR.First().LAND1,
                VENDOR = VENDOR.First().VENDOR,
                COMPCODE = VENDOR.First().COMP_CODE,
                COUNTRY = VENDOR.First().COUNTRY,
                CURRENCY = VENDOR.First().CURRENCY,
                KTOKK = VENDOR.First().KTOKK,
                MAIL = VENDOR.First().MAIL,
                NAME = VENDOR.First().NAME,
                PMNTTRMS = VENDOR.First().PMNTTRMS,
                PURCHORG = VENDOR.First().PURCH_ORG,
                SORT1 = VENDOR.First().SORT1,
                SORT2 = VENDOR.First().SORT2,
                STREET = VENDOR.First().STREET,
                TELEFONO = VENDOR.First().TELEFONO,
                VERIFFEM = VENDOR.First().VERIF_F_EM
            };

            return resultado;
        }
    }


}
