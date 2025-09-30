using SustitucionMOAModel.Dto;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.ObtenerOrdenesDeCompraWebServiceMOA;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MEWP;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;


namespace SustitucionMOAWS.WSConsumers
{
    /// <summary>
    /// Listado de Ordenes de Compra
    /// </summary>
    public class ObtenerOrdenesDeCompraConsumerMOA : IObtenerOrdenesDeCompraConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserS4"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassS4"];
        public ObtenerOrdenesDeCompraConsumerMOA()
        {

        }

        public List<OrdenCompraDto> Request(OrderParamsDto parametros, bool usuarioSolp = false)
        {

            string fechaInicio = parametros.fechaInicio;
            string vendedor = parametros.vendedor;
            string OC = parametros.OrdenCompraId;
            //MMSN - 574: Si ingresa OC o proveedor, son independientes de la fecha. De no coincidir OC y vendedor, retornar el error. 
            if (!String.IsNullOrEmpty(OC) || !String.IsNullOrEmpty(vendedor))
            {
                fechaInicio = "";
            }
            string categoria = "9"; // 9 = Servicios

            if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
            {
                var agent = new Z_WS_BAPI_DIRECT_MEWPClient();
                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;

                WS_GAQ_sin_PI_DIRECT_MEWP.BAPIEKKOL[] cabeceras = new WS_GAQ_sin_PI_DIRECT_MEWP.BAPIEKKOL[] { };
                WS_GAQ_sin_PI_DIRECT_MEWP.BAPIEKPOC[] detalle = new WS_GAQ_sin_PI_DIRECT_MEWP.BAPIEKPOC[] { };
                WS_GAQ_sin_PI_DIRECT_MEWP.BAPIRETURN[] bapiReturn = new WS_GAQ_sin_PI_DIRECT_MEWP.BAPIRETURN[] { };

                var request = new BAPI_PO_GETITEMS()
                {
                    ACCTASSCAT = string.Empty,
                    CREATED_BY = string.Empty,
                    DELETED_ITEMS = usuarioSolp ? "X" : "",
                    DOC_DATE = fechaInicio,
                    DOC_TYPE = string.Empty,
                    ITEMS_OPEN_FOR_RECEIPT = string.Empty,
                    ITEM_CAT = categoria,
                    MATERIAL = string.Empty,
                    MATERIAL_EVG = new WS_GAQ_sin_PI_DIRECT_MEWP.BAPIMGVMATNR(),
                    MATERIAL_LONG = string.Empty,
                    MAT_GRP = string.Empty,
                    PLANT = string.Empty,
                    PREQ_NAME = string.Empty,
                    PURCHASEORDER = OC,
                    PURCH_ORG = string.Empty,
                    PUR_GROUP = string.Empty,
                    PUR_MAT = string.Empty,
                    PUR_MAT_EVG = new WS_GAQ_sin_PI_DIRECT_MEWP.BAPIMGVMATNR(),
                    PUR_MAT_LONG = string.Empty,
                    SHORT_TEXT = string.Empty,
                    SUPPL_PLANT = string.Empty,
                    TRACKINGNO = string.Empty,
                    VENDOR = !string.IsNullOrEmpty(vendedor) && vendedor.Length > 10 ? vendedor.Substring(0, 10) : vendedor,
                    WITH_PO_HEADERS = "X",
                    PO_HEADERS = cabeceras,
                    PO_ITEMS = detalle,
                    RETURN = bapiReturn
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

                ObtenerOrdenesDeCompraWebServiceMOA.BAPIEKKOL[] cabeceras = new ObtenerOrdenesDeCompraWebServiceMOA.BAPIEKKOL[] { };
                ObtenerOrdenesDeCompraWebServiceMOA.BAPIEKPOC[] detalle = new ObtenerOrdenesDeCompraWebServiceMOA.BAPIEKPOC[] { };
                ObtenerOrdenesDeCompraWebServiceMOA.BAPIRETURN[] bapiReturn = new ObtenerOrdenesDeCompraWebServiceMOA.BAPIRETURN[] { };
                service.BAPI_PO_GETITEMS("", "", usuarioSolp ? "X" : "", fechaInicio, "", "", categoria, "", new ObtenerOrdenesDeCompraWebServiceMOA.BAPIMGVMATNR(), "",
                                         "", "", "", OC, "", "", "", new ObtenerOrdenesDeCompraWebServiceMOA.BAPIMGVMATNR(), "", "",
                                         "", "", vendedor, "X",
                                         ref cabeceras,
                                         ref detalle,
                                         ref bapiReturn
                                        );
                return Map(cabeceras);
            }
        }


        /// <summary>
        /// Parsea los datos de las cabeceras de las ordenes de compra
        /// </summary>
        /// <param name="cabeceras"></param>
        /// <returns></returns>
        private List<OrdenCompraDto> Map(ObtenerOrdenesDeCompraWebServiceMOA.BAPIEKKOL[] cabeceras)
        {
            List<OrdenCompraDto> result = new List<OrdenCompraDto>();
            List<OrdenCompraDto> posiciones = new List<OrdenCompraDto>();

            foreach (var item in cabeceras)
            {
                //Suma de los netos de las posiciones
                //decimal montoTotal = detalle
                //    .Where(det => det.PO_NUMBER == item.PO_NUMBER)
                //    .Sum(det => det.NET_PRICE);

                // Parsear la fecha y formatearla
                DateTime fecha = DateTime.ParseExact(item.DOC_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                string fechaFormateada = fecha.ToString("dd/MM/yyyy");

                result.Add(new OrdenCompraDto
                {
                    Id = long.Parse(item.PO_NUMBER),
                    Fecha = fechaFormateada,
                    ProveedorNombre = item.VEND_NAME,
                    MonedaDescripcion = item.CURRENCY,
                    SUBJ_TO_R = item.SUBJ_TO_R,
                    ProveedorNumero = item.VENDOR
                });
            }

            return result;
        }

        private List<OrdenCompraDto> MapSinPI(WS_GAQ_sin_PI_DIRECT_MEWP.BAPI_PO_GETITEMSResponse response)
        {
            List<OrdenCompraDto> result = new List<OrdenCompraDto>();
            List<OrdenCompraDto> posiciones = new List<OrdenCompraDto>();

            foreach (var item in response.PO_HEADERS)
            {
                //Suma de los netos de las posiciones
                //decimal montoTotal = detalle
                //    .Where(det => det.PO_NUMBER == item.PO_NUMBER)
                //    .Sum(det => det.NET_PRICE);

                // Parsear la fecha y formatearla
                DateTime fecha = DateTime.ParseExact(item.DOC_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                string fechaFormateada = fecha.ToString("dd/MM/yyyy");

                result.Add(new OrdenCompraDto
                {
                    Id = long.Parse(item.PO_NUMBER),
                    Fecha = fechaFormateada,
                    ProveedorNombre = item.VEND_NAME,
                    MonedaDescripcion = item.CURRENCY,
                    SUBJ_TO_R = item.SUBJ_TO_R,
                    ProveedorNumero = item.VENDOR
                });
            }

            return result;
        }

    }
}
