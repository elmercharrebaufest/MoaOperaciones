using System.Collections.Generic;
using System.Linq;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.ReporteOCWebServiceMOA;

namespace SustitucionMOAWS.WSConsumers
{
    public class ReporteOrdenDeCompraConsumerMOA : IReporteOrdenDeCompraConsumerMOA
    {
        BAPI_PO_GETITEMSPortTypeClient service;

        public ReporteOrdenDeCompraConsumerMOA()
        {

        }

        public List<OrdenDeCompraSAPDto> Request(string nroOC, string fecha, string codigoProveedor)
        {
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
            BAPIMGVMATNR MATERIAL_EVG = new BAPIMGVMATNR();
            string MATERIAL_LONG = "";
            string MAT_GRP = "";
            string PLANT = "";
            string PREQ_NAME = "";
            string PURCHASEORDER = nroOC;
            string PURCH_ORG = "";
            string PUR_GROUP = "";
            string PUR_MAT = "";
            BAPIMGVMATNR PUR_MAT_EVG = new BAPIMGVMATNR();
            string PUR_MAT_LONG = "";
            string SHORT_TEXT = "";
            string SUPPL_PLANT = "";
            string TRACKINGNO = "";
            string VENDOR = codigoProveedor;
            string WITH_PO_HEADERS = "X";
            BAPIEKKOL[] PO_HEADERS = new BAPIEKKOL[] { };
            BAPIEKPOC[] PO_ITEMS = new BAPIEKPOC[] { };
            BAPIRETURN[] RETURN = new BAPIRETURN[] { };

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

        private List<OrdenDeCompraSAPDto> Map(BAPIEKKOL[] PO_HEADERS, BAPIEKPOC[] PO_ITEMS, BAPIRETURN[] RETURN)
        {
            List<OrdenDeCompraSAPDto> result = new List<OrdenDeCompraSAPDto>();

            var resultado = "";

            if (RETURN.Any(x => x.TYPE == "E"))
            {
                resultado = RETURN.FirstOrDefault().MESSAGE;
            }

            //if (PO_HEADERS.Length == 0)
            //{
            //    result.Add(new OrdenDeCompraSAPDto { Mensaje = resultado });
            //}
            //else 
            //{
                foreach (var item in PO_HEADERS)
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
                            Tipo = PO_ITEMS.Select(x => x.ITEM_CAT).FirstOrDefault() == "0" ? "Materiales" : "Servicios",
                            //TipoDocCompras = item.DOC_TYPE,

                        },
                        Mensaje = resultado
                    });
                }


            //}
            return result;
        }
    }

    public interface IReporteOrdenDeCompraConsumerMOA
    {
        List<OrdenDeCompraSAPDto> Request(string nroOC, string fecha, string codigoProveedor);
    }
}