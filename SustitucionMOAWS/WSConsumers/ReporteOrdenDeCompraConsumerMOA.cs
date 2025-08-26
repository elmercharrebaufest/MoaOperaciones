using SustitucionMOAFotmatter;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.ReporteOCWebServiceMOA;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MEWP;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace SustitucionMOAWS.WSConsumers
{
    public class ReporteOrdenDeCompraConsumerMOA : IReporteOrdenDeCompraConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserSinPI"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassSinPI"];
        public ReporteOrdenDeCompraConsumerMOA()
        {

        }

        public List<OrdenDeCompraSAPDto> Request(string nroOC, string fecha, string codigoProveedor)
        {

            if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
            {
                var agent = new Z_WS_BAPI_DIRECT_MEWPClient();
                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;

                string ACCTASSCAT = "";
                string CREATED_BY = "";
                string DELETED_ITEMS = "";
                string DOC_DATE = fecha;
                string DOC_TYPE = "";
                string ITEMS_OPEN_FOR_RECEIPT = "";
                string ITEM_CAT = "";
                string MATERIAL = "";
                WS_GAQ_sin_PI_DIRECT_MEWP.BAPIMGVMATNR MATERIAL_EVG = new WS_GAQ_sin_PI_DIRECT_MEWP.BAPIMGVMATNR();
                string MATERIAL_LONG = "";
                string MAT_GRP = "";
                string PLANT = "";
                string PREQ_NAME = "";
                string PURCHASEORDER = nroOC;
                string PURCH_ORG = "";
                string PUR_GROUP = "";
                string PUR_MAT = "";
                WS_GAQ_sin_PI_DIRECT_MEWP.BAPIMGVMATNR PUR_MAT_EVG = new WS_GAQ_sin_PI_DIRECT_MEWP.BAPIMGVMATNR();
                string PUR_MAT_LONG = "";
                string SHORT_TEXT = "";
                string SUPPL_PLANT = "";
                string TRACKINGNO = "";
                string VENDOR = codigoProveedor;
                string WITH_PO_HEADERS = "X";
                WS_GAQ_sin_PI_DIRECT_MEWP.BAPIEKKOL[] PO_HEADERS = new WS_GAQ_sin_PI_DIRECT_MEWP.BAPIEKKOL[] { };
                WS_GAQ_sin_PI_DIRECT_MEWP.BAPIEKPOC[] PO_ITEMS = new WS_GAQ_sin_PI_DIRECT_MEWP.BAPIEKPOC[] { };
                WS_GAQ_sin_PI_DIRECT_MEWP.BAPIRETURN[] RETURN = new WS_GAQ_sin_PI_DIRECT_MEWP.BAPIRETURN[] { };

                var request = new BAPI_PO_GETITEMS()
                {
                    ACCTASSCAT = ACCTASSCAT,
                    CREATED_BY = CREATED_BY,
                    DELETED_ITEMS = DELETED_ITEMS,
                    DOC_DATE = DOC_DATE,
                    DOC_TYPE = DOC_TYPE,
                    ITEMS_OPEN_FOR_RECEIPT = ITEMS_OPEN_FOR_RECEIPT,
                    ITEM_CAT = ITEM_CAT,
                    MATERIAL = MATERIAL,
                    MATERIAL_EVG = MATERIAL_EVG,
                    MATERIAL_LONG = MATERIAL_LONG,
                    MAT_GRP = MAT_GRP,
                    PLANT = PLANT,
                    PREQ_NAME = PREQ_NAME,
                    PURCHASEORDER = PURCHASEORDER,
                    PURCH_ORG = PURCH_ORG,
                    PUR_GROUP = PUR_GROUP,
                    PUR_MAT = PUR_MAT,
                    PUR_MAT_EVG = PUR_MAT_EVG,
                    PUR_MAT_LONG = PUR_MAT_LONG,
                    SHORT_TEXT = SHORT_TEXT,
                    SUPPL_PLANT = SUPPL_PLANT,
                    TRACKINGNO = TRACKINGNO,
                    VENDOR = VENDOR,
                    WITH_PO_HEADERS = WITH_PO_HEADERS,
                    PO_HEADERS = PO_HEADERS,
                    PO_ITEMS = PO_ITEMS,
                    RETURN = RETURN
                };
                Log.Info($"SAP sin PI BAPI_PO_GETITEMS request");
                Log.Info(request.ToXml());

                var response = agent.BAPI_PO_GETITEMS(request);
                Log.Info($"SAP sin PI BAPI_PO_GETITEMS response");
                Log.Info(response.ToXml());
                return MapSinPI(response);
            }
            else
            {
                BAPI_PO_GETITEMSPortTypeClient service;
                var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=BAPI_PO_GETITEMS&amp;interfaceNamespace=urn%3Asap-com%3Adocument%3Asap%3Arfc%3Afunctions";
                service = new BAPI_PO_GETITEMSPortTypeClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();

                string ACCTASSCAT = "";
                string CREATED_BY = "";
                string DELETED_ITEMS = "";
                string DOC_DATE = fecha;
                string DOC_TYPE = "";
                string ITEMS_OPEN_FOR_RECEIPT = "";
                string ITEM_CAT = "";
                string MATERIAL = "";
                ReporteOCWebServiceMOA.BAPIMGVMATNR MATERIAL_EVG = new ReporteOCWebServiceMOA.BAPIMGVMATNR();
                string MATERIAL_LONG = "";
                string MAT_GRP = "";
                string PLANT = "";
                string PREQ_NAME = "";
                string PURCHASEORDER = nroOC;
                string PURCH_ORG = "";
                string PUR_GROUP = "";
                string PUR_MAT = "";
                ReporteOCWebServiceMOA.BAPIMGVMATNR PUR_MAT_EVG = new ReporteOCWebServiceMOA.BAPIMGVMATNR();
                string PUR_MAT_LONG = "";
                string SHORT_TEXT = "";
                string SUPPL_PLANT = "";
                string TRACKINGNO = "";
                string VENDOR = codigoProveedor;
                string WITH_PO_HEADERS = "X";
                ReporteOCWebServiceMOA.BAPIEKKOL[] PO_HEADERS = new ReporteOCWebServiceMOA.BAPIEKKOL[] { };
                ReporteOCWebServiceMOA.BAPIEKPOC[] PO_ITEMS   = new ReporteOCWebServiceMOA.BAPIEKPOC[] { };
                ReporteOCWebServiceMOA.BAPIRETURN[] RETURN    = new ReporteOCWebServiceMOA.BAPIRETURN[] { };

                service.BAPI_PO_GETITEMS(ACCTASSCAT,
                                        CREATED_BY,
                                        DELETED_ITEMS,
                                        DOC_DATE, DOC_TYPE,
                                        ITEMS_OPEN_FOR_RECEIPT,
                                        ITEM_CAT,
                                        MATERIAL,
                                        MATERIAL_EVG,
                                        MATERIAL_LONG,
                                        MAT_GRP,
                                        PLANT,
                                        PREQ_NAME,
                                        PURCHASEORDER,
                                        PURCH_ORG,
                                        PUR_GROUP,
                                        PUR_MAT,
                                        PUR_MAT_EVG,
                                        PUR_MAT_LONG,
                                        SHORT_TEXT,
                                        SUPPL_PLANT,
                                        TRACKINGNO,
                                        VENDOR,
                                        WITH_PO_HEADERS,
                                        ref PO_HEADERS,
                                        ref PO_ITEMS,
                                        ref RETURN);
                return Map(PO_HEADERS, PO_ITEMS, RETURN);
            }
        }

        private List<OrdenDeCompraSAPDto> Map(ReporteOCWebServiceMOA.BAPIEKKOL[] PO_HEADERS, ReporteOCWebServiceMOA.BAPIEKPOC[] PO_ITEMS, ReporteOCWebServiceMOA.BAPIRETURN[] RETURN)
        {
            List<OrdenDeCompraSAPDto> result = new List<OrdenDeCompraSAPDto>();
            var resultado = "";
            if (RETURN.Any(x => x.TYPE == "E"))
            {
                resultado = RETURN.FirstOrDefault().MESSAGE;
            }
            foreach (var item in PO_HEADERS.Where(x => string.IsNullOrEmpty(x.SUBJ_TO_R)))
            {
                result.Add(new OrdenDeCompraSAPDto
                {
                    Cabecera = new OrdenDeCompraSAPCabecera
                    {
                        OrdenDeCompra = item.PO_NUMBER,
                        CodigoProveedor = item.VENDOR,
                        RazonSocialProveedor = item.VEND_NAME,
                        //CUITProveedor = item.STCD1,
                        Moneda = item.CURRENCY,
                        MontoTotal = PO_ITEMS.Where(x => x.PO_NUMBER == item.PO_NUMBER).Sum(a => a.NET_PRICE),
                        CreadoPor = item.CREATED_BY,
                        ClaseDocumento = item.DOC_TYPE,
                        FechaCreacion = SAPFormatter.GetDateTime(item.CREATED_ON),
                        Tipo = PO_ITEMS.Select(x => x.ITEM_CAT).FirstOrDefault() == "0" ? "Materiales" : "Servicio",
                        //TipoDocCompras = item.DOC_TYPE,

                    },
                    Mensaje = resultado
                });
            }
            return result;
        }

        private List<OrdenDeCompraSAPDto> MapSinPI(WS_GAQ_sin_PI_DIRECT_MEWP.BAPI_PO_GETITEMSResponse response)
        {
            List<OrdenDeCompraSAPDto> result = new List<OrdenDeCompraSAPDto>();
            var resultado = "";
            if (response.RETURN.Any(x => x.TYPE == "E"))
            {
                resultado = response.RETURN.FirstOrDefault().MESSAGE;
            }
            foreach (var item in response.PO_HEADERS.Where(x => string.IsNullOrEmpty(x.SUBJ_TO_R)))
            {
                result.Add(new OrdenDeCompraSAPDto
                {
                    Cabecera = new OrdenDeCompraSAPCabecera
                    {
                        OrdenDeCompra = item.PO_NUMBER,
                        CodigoProveedor = item.VENDOR,
                        RazonSocialProveedor = item.VEND_NAME,
                        //CUITProveedor = item.STCD1,
                        Moneda = item.CURRENCY,
                        MontoTotal = response.PO_ITEMS.Where(x => x.PO_NUMBER == item.PO_NUMBER).Sum(a => a.NET_PRICE),
                        CreadoPor = item.CREATED_BY,
                        ClaseDocumento = item.DOC_TYPE,
                        FechaCreacion = SAPFormatter.GetDateTime(item.CREATED_ON),
                        Tipo = response.PO_ITEMS.Select(x => x.ITEM_CAT).FirstOrDefault() == "0" ? "Materiales" : "Servicio",
                        //TipoDocCompras = item.DOC_TYPE,
                    },
                    Mensaje = resultado
                });
            }
            return result;
        }

    }

    public interface IReporteOrdenDeCompraConsumerMOA
    {
        List<OrdenDeCompraSAPDto> Request(string nroOC, string fecha, string codigoProveedor);
    }
}