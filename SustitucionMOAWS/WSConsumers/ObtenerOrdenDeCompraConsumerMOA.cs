using SustitucionMOAFotmatter;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenesCompra;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.ObtenerOrdenDeCompraWebServiceMOA;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_2012;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;

namespace SustitucionMOAWS.WSConsumers
{
    /// <summary>
    /// Obtener detalle de una Orden de Compra. Consula por numero de Orden de Compra
    /// </summary>
    public class ObtenerOrdenDeCompraConsumerMOA : IObtenerOrdenDeCompraConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserSinPI"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassSinPI"];

        BAPI_PO_GETDETAIL1PortTypeClient service;
        private const string COMP_CODE = "MOA";
        private readonly IRepositorio repositorio;
        /// <summary>
        /// MMSN-491 - Modificar el formato de fecha. DD/MM/AAAA
        /// </summary>
        private string dateTimeFormat = "dd/MM/yyyy";

        /// <summary>
        /// //MMSN-491 - Ponerle separador de miles a la columna “Monto Total”. - Separador de miles ( , ) coma - Separador decimal ( . ) punto
        /// </summary>
        private string currencyFormat = "#,##0.00";

        public ObtenerOrdenDeCompraConsumerMOA(IRepositorio repositorio)
        {
            var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=BAPI_PO_GETDETAIL1&amp;interfaceNamespace=urn%3Asap-com%3Adocument%3Asap%3Arfc%3Afunctions";
            service = new BAPI_PO_GETDETAIL1PortTypeClient(SAPCredential.CrearSapLongBinding(), SAPCredential.DevolverEndpoint(url));
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
            this.repositorio = repositorio;
        }

        public OrdenDeCompraSAPDto ObtenerOrdenDeCompra(string nroOC)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var response = ObtenerOcSapSinPI(nroOC);
                    return MapOrdenDeCompraSAPSinPIDto(response);
                }
                else
                {
                    ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOITEM[] POITEM;
                    ObtenerOrdenDeCompraWebServiceMOA.BAPIRET2[] RETURN;
                    ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOHEADER POHEADER;
                    ObtenerOrdenDeCompraWebServiceMOA.BAPIEIKP result;
                    ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOTEXTHEADER[] POTEXTHEADER;
                    ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOTEXT[] POTEXTITEM;
                    ObtenerOrdenDeCompraWebServiceMOA.BAPIESLLC[] POSERVICES;
                    ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOSCHEDULE[] POSCHEDULE;
                    ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOADDRDELIVERY[] POADDRDELIVERY;
                    ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOCOND[] POCOND;
                    ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOACCOUNT[] POACCOUNT;
                    ObtenerOrdenDeCompraWebServiceMOA.BAPIESKLC[] POSRVACCESSVALUES;
                    ObtenerOrdenDeCompraWebServiceMOA.BAPIEKBE[] POHISTORY;
                    ObtenerOcSap(nroOC, out POITEM, out RETURN, out POHEADER, out result, out POTEXTHEADER, out POTEXTITEM, out POSERVICES, out POSCHEDULE, out POADDRDELIVERY, out POCOND, out POACCOUNT, out POSRVACCESSVALUES, out POHISTORY);

                    var resultado = MapOrdenDeCompraSAPDto(result, POHEADER, RETURN, POITEM, POTEXTHEADER, POTEXTITEM, POSERVICES, POSCHEDULE, POADDRDELIVERY, POCOND, POSRVACCESSVALUES, POHISTORY);
                    // Filtrar certificaciones con saldo mayor a 0
                    resultado.Certificaciones = resultado.Certificaciones.Where(x => x.Saldo > 0).ToList();
                    Log.Info("BAPI_PO_GETDETAIL1PortTypeClient" + resultado.ToJson());
                    return resultado;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public AdjudicacionDto ObtenerOrdenDeCompraAdjudicacion(string nroOC)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var response = ObtenerOcSapSinPI(nroOC);
                    return MapAdjudicacionSinPIDto(response);
                }
                else
                {
                    ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOITEM[] POITEM;
                    ObtenerOrdenDeCompraWebServiceMOA.BAPIRET2[] RETURN;
                    ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOHEADER POHEADER;
                    ObtenerOrdenDeCompraWebServiceMOA.BAPIEIKP result;
                    ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOTEXTHEADER[] POTEXTHEADER;
                    ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOTEXT[] POTEXTITEM;
                    ObtenerOrdenDeCompraWebServiceMOA.BAPIESLLC[] POSERVICES;
                    ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOSCHEDULE[] POSCHEDULE;
                    ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOADDRDELIVERY[] POADDRDELIVERY;
                    ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOCOND[] POCOND;
                    ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOACCOUNT[] POACCOUNT;
                    ObtenerOrdenDeCompraWebServiceMOA.BAPIESKLC[] POSRVACCESSVALUES;
                    ObtenerOrdenDeCompraWebServiceMOA.BAPIEKBE[] POHISTORY;
                    ObtenerOcSap(nroOC, out POITEM, out RETURN, out POHEADER, out result, out POTEXTHEADER, out POTEXTITEM, out POSERVICES,
                        out POSCHEDULE, out POADDRDELIVERY, out POCOND, out POACCOUNT, out POSRVACCESSVALUES, out POHISTORY);


                    return MapAdjudicacionDto(result, POHEADER, RETURN, POITEM, POTEXTHEADER, POTEXTITEM, POSERVICES, POSCHEDULE, POADDRDELIVERY, POSRVACCESSVALUES);
                }


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ResultBAPI_PO_GETDETAIL1SinPI ObtenerOrdenDeCompraRFCSinPI(string nroOC)
        {
            try
            {
                var response = ObtenerOcSapSinPI(nroOC);
                return new ResultBAPI_PO_GETDETAIL1SinPI(response);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ResultBAPI_PO_GETDETAIL1 ObtenerOrdenDeCompraRFC(string nroOC)
        {
            try
            {
                ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOITEM[] POITEM;
                ObtenerOrdenDeCompraWebServiceMOA.BAPIRET2[] RETURN;
                ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOHEADER POHEADER;
                ObtenerOrdenDeCompraWebServiceMOA.BAPIEIKP result;
                ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOTEXTHEADER[] POTEXTHEADER;
                ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOTEXT[] POTEXTITEM;
                ObtenerOrdenDeCompraWebServiceMOA.BAPIESLLC[] POSERVICES;
                ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOSCHEDULE[] POSCHEDULE;
                ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOADDRDELIVERY[] POADDRDELIVERY;
                ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOCOND[] POCOND;
                ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOACCOUNT[] POACCOUNT;
                ObtenerOrdenDeCompraWebServiceMOA.BAPIESKLC[] POSRVACCESSVALUES;
                ObtenerOrdenDeCompraWebServiceMOA.BAPIEKBE[] POHISTORY;
                ObtenerOcSap(nroOC, out POITEM, out RETURN, out POHEADER, out result, out POTEXTHEADER, out POTEXTITEM, out POSERVICES,
                    out POSCHEDULE, out POADDRDELIVERY, out POCOND, out POACCOUNT, out POSRVACCESSVALUES, out POHISTORY);

                return new ResultBAPI_PO_GETDETAIL1(result, POHEADER, RETURN, POITEM, POTEXTHEADER, POTEXTITEM, POSERVICES, POSCHEDULE, POADDRDELIVERY, POCOND, POACCOUNT, POSRVACCESSVALUES);

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void ObtenerOcSap(
           string nroOC, out ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOITEM[] POITEM, out ObtenerOrdenDeCompraWebServiceMOA.BAPIRET2[] RETURN, out ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOHEADER POHEADER, out ObtenerOrdenDeCompraWebServiceMOA.BAPIEIKP result,
          out ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOTEXTHEADER[] POTEXTHEADER, out ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOTEXT[] POTEXTITEM, out ObtenerOrdenDeCompraWebServiceMOA.BAPIESLLC[] POSERVICES, out ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOSCHEDULE[] POSCHEDULE,
          out ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOADDRDELIVERY[] POADDRDELIVERY, out ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOCOND[] POCOND, out ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOACCOUNT[] POACCOUNT, out ObtenerOrdenDeCompraWebServiceMOA.BAPIESKLC[] POSRVACCESSVALUES,
          out ObtenerOrdenDeCompraWebServiceMOA.BAPIEKBE[] POHISTORY
           )
        {
            string ACCOUNT_ASSIGNMENT = "X";
            string DELIVERY_ADDRESS = "X";
            string HEADER_TEXT = "X";
            string INVOICEPLAN = "X";
            string ITEM_TEXT = "X";
            string PURCHASEORDER = nroOC;
            string SERIALNUMBERS = "X";
            string SERVICES = "X";
            string VERSION = "X";

            POACCOUNT = new ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOACCOUNT[] { };
            POADDRDELIVERY = new ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOADDRDELIVERY[] { };
            POCOND = new ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOCOND[] { };
            POITEM = new ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOITEM[] { };
            POTEXTHEADER = new ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOTEXTHEADER[] { };
            POTEXTITEM = new ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOTEXT[] { };
            RETURN = new ObtenerOrdenDeCompraWebServiceMOA.BAPIRET2[] { };
            POSERVICES = new ObtenerOrdenDeCompraWebServiceMOA.BAPIESLLC[] { };
            POHEADER = new ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOHEADER { };
            ObtenerOrdenDeCompraWebServiceMOA.BAPI_INVOICE_PLAN_HEADER[] INVPLANHEADER = new ObtenerOrdenDeCompraWebServiceMOA.BAPI_INVOICE_PLAN_HEADER[] { };
            ObtenerOrdenDeCompraWebServiceMOA.BAPIMEDCM_ALLVERSIONS[] ALLVERSIONS = new ObtenerOrdenDeCompraWebServiceMOA.BAPIMEDCM_ALLVERSIONS[] { };
            ObtenerOrdenDeCompraWebServiceMOA.BAPIPAREX[] EXTENSIONOUT = new ObtenerOrdenDeCompraWebServiceMOA.BAPIPAREX[] { };
            ObtenerOrdenDeCompraWebServiceMOA.BAPI_INVOICE_PLAN_ITEM[] INVPLANITEM = new ObtenerOrdenDeCompraWebServiceMOA.BAPI_INVOICE_PLAN_ITEM[] { };
            ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOCOMPONENT[] POCOMPONENTS = new ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOCOMPONENT[] { };
            ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOCONDHEADER[] POCONDHEADER = new ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOCONDHEADER[] { };
            ObtenerOrdenDeCompraWebServiceMOA.BAPIEKES[] POCONFIRMATION = new ObtenerOrdenDeCompraWebServiceMOA.BAPIEKES[] { };
            ObtenerOrdenDeCompraWebServiceMOA.BAPIESUCC[] POCONTRACTLIMITS = new ObtenerOrdenDeCompraWebServiceMOA.BAPIESUCC[] { };
            ObtenerOrdenDeCompraWebServiceMOA.BAPIEIPO[] POEXPIMPITEM = new ObtenerOrdenDeCompraWebServiceMOA.BAPIEIPO[] { };
            POHISTORY = new ObtenerOrdenDeCompraWebServiceMOA.BAPIEKBE[] { };
            ObtenerOrdenDeCompraWebServiceMOA.BAPIEKBE_MA[] POHISTORY_MA = new ObtenerOrdenDeCompraWebServiceMOA.BAPIEKBE_MA[] { };
            ObtenerOrdenDeCompraWebServiceMOA.BAPIEKBES[] POHISTORY_TOTALS = new ObtenerOrdenDeCompraWebServiceMOA.BAPIEKBES[] { };
            ObtenerOrdenDeCompraWebServiceMOA.BAPIESUHC[] POLIMITS = new ObtenerOrdenDeCompraWebServiceMOA.BAPIESUHC[] { };
            ObtenerOrdenDeCompraWebServiceMOA.BAPIEKKOP[] POPARTNER = new ObtenerOrdenDeCompraWebServiceMOA.BAPIEKKOP[] { };
            POSCHEDULE = new ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOSCHEDULE[] { };
            ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOSHIPPEXP[] POSHIPPINGEXP = new ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOSHIPPEXP[] { };
            POSRVACCESSVALUES = new ObtenerOrdenDeCompraWebServiceMOA.BAPIESKLC[] { };
            ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOSERIALNO[] SERIALNUMBER = new ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOSERIALNO[] { };

            result = service.BAPI_PO_GETDETAIL1(ACCOUNT_ASSIGNMENT,
                DELIVERY_ADDRESS,
                HEADER_TEXT,
                INVOICEPLAN,
                ITEM_TEXT,
                PURCHASEORDER,
                SERIALNUMBERS,
                SERVICES,
                VERSION,
                ref ALLVERSIONS,
                ref EXTENSIONOUT,
                ref INVPLANHEADER,
                ref INVPLANITEM,
                ref POACCOUNT,
                ref POADDRDELIVERY,
                ref POCOMPONENTS,
                ref POCOND,
                ref POCONDHEADER,
                ref POCONFIRMATION,
                ref POCONTRACTLIMITS,
                ref POEXPIMPITEM,
                ref POHISTORY,
                ref POHISTORY_MA,
                ref POHISTORY_TOTALS,
                ref POITEM,
                ref POLIMITS,
                ref POPARTNER,
                ref POSCHEDULE,
                ref POSERVICES,
                ref POSHIPPINGEXP,
                ref POSRVACCESSVALUES,
                ref POTEXTHEADER,
                ref POTEXTITEM,
                ref RETURN,
                ref SERIALNUMBER,
                out POHEADER);
        }

        private WS_GAQ_sin_PI_DIRECT_2012.BAPI_PO_GETDETAIL1Response ObtenerOcSapSinPI(string nroOC)
        {
            var agent = new Z_WS_BAPI_DIRECT_2012Client();
            agent.ClientCredentials.UserName.UserName = UserSap;
            agent.ClientCredentials.UserName.Password = PassSap;
            string ACCOUNT_ASSIGNMENT = "X";
            string DELIVERY_ADDRESS = "X";
            string HEADER_TEXT = "X";
            string INVOICEPLAN = "X";
            string ITEM_TEXT = "X";
            string PURCHASEORDER = nroOC;
            string SERIALNUMBERS = "X";
            string SERVICES = "X";
            string VERSION = "X";

            var POACCOUNT = new WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOACCOUNT[] { };
            var POADDRDELIVERY = new WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOADDRDELIVERY[] { };
            var POCOND = new WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOCOND[] { };
            var POITEM = new WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOITEM[] { };
            var POTEXTHEADER = new WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOTEXTHEADER[] { };
            var POTEXTITEM = new WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOTEXT[] { };
            var RETURN = new WS_GAQ_sin_PI_DIRECT_2012.BAPIRET2[] { };
            var POSERVICES = new WS_GAQ_sin_PI_DIRECT_2012.BAPIESLLC[] { };
            var POHEADER = new WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOHEADER { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPI_INVOICE_PLAN_HEADER[] INVPLANHEADER = new WS_GAQ_sin_PI_DIRECT_2012.BAPI_INVOICE_PLAN_HEADER[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPIMEDCM_ALLVERSIONS[] ALLVERSIONS = new WS_GAQ_sin_PI_DIRECT_2012.BAPIMEDCM_ALLVERSIONS[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPIPAREX[] EXTENSIONOUT = new WS_GAQ_sin_PI_DIRECT_2012.BAPIPAREX[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPI_INVOICE_PLAN_ITEM[] INVPLANITEM = new WS_GAQ_sin_PI_DIRECT_2012.BAPI_INVOICE_PLAN_ITEM[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOCOMPONENT[] POCOMPONENTS = new WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOCOMPONENT[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOCONDHEADER[] POCONDHEADER = new WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOCONDHEADER[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPIEKES[] POCONFIRMATION = new WS_GAQ_sin_PI_DIRECT_2012.BAPIEKES[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPIESUCC[] POCONTRACTLIMITS = new WS_GAQ_sin_PI_DIRECT_2012.BAPIESUCC[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPIEIPO[] POEXPIMPITEM = new WS_GAQ_sin_PI_DIRECT_2012.BAPIEIPO[] { };
            var POHISTORY = new WS_GAQ_sin_PI_DIRECT_2012.BAPIEKBE[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPIEKBE_MA[] POHISTORY_MA = new WS_GAQ_sin_PI_DIRECT_2012.BAPIEKBE_MA[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPIEKBES[] POHISTORY_TOTALS = new WS_GAQ_sin_PI_DIRECT_2012.BAPIEKBES[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPIESUHC[] POLIMITS = new WS_GAQ_sin_PI_DIRECT_2012.BAPIESUHC[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPIEKKOP[] POPARTNER = new WS_GAQ_sin_PI_DIRECT_2012.BAPIEKKOP[] { };
            var POSCHEDULE = new WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOSCHEDULE[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOSHIPPEXP[] POSHIPPINGEXP = new WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOSHIPPEXP[] { };
            var POSRVACCESSVALUES = new WS_GAQ_sin_PI_DIRECT_2012.BAPIESKLC[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOSERIALNO[] SERIALNUMBER = new WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOSERIALNO[] { };

            var request = new BAPI_PO_GETDETAIL1()
            {
                ACCOUNT_ASSIGNMENT = ACCOUNT_ASSIGNMENT,
                DELIVERY_ADDRESS = DELIVERY_ADDRESS,
                HEADER_TEXT = HEADER_TEXT,
                INVOICEPLAN = INVOICEPLAN,
                ITEM_TEXT = ITEM_TEXT,
                PURCHASEORDER = PURCHASEORDER,
                SERIALNUMBERS = SERIALNUMBERS,
                SERVICES = SERVICES,
                VERSION = VERSION,
                ALLVERSIONS = ALLVERSIONS,
                EXTENSIONOUT = EXTENSIONOUT,
                INVPLANHEADER = INVPLANHEADER,
                INVPLANITEM = INVPLANITEM,
                POACCOUNT = POACCOUNT,
                POADDRDELIVERY = POADDRDELIVERY,
                POCOMPONENTS = POCOMPONENTS,
                POCOND = POCOND,
                POCONDHEADER = POCONDHEADER,
                POCONFIRMATION = POCONFIRMATION,
                POCONTRACTLIMITS = POCONTRACTLIMITS,
                POEXPIMPITEM = POEXPIMPITEM,
                POHISTORY = POHISTORY,
                POHISTORY_MA = POHISTORY_MA,
                POHISTORY_TOTALS = POHISTORY_TOTALS,
                POITEM = POITEM,
                POLIMITS = POLIMITS,
                POPARTNER = POPARTNER,
                POSCHEDULE = POSCHEDULE,
                POSERVICES = POSERVICES,
                POSHIPPINGEXP = POSHIPPINGEXP,
                POSRVACCESSVALUES = POSRVACCESSVALUES,
                POTEXTHEADER = POTEXTHEADER,
                POTEXTITEM = POTEXTITEM,
                RETURN = RETURN,
                SERIALNUMBER = SERIALNUMBER,
            };
            Log.Info($"SAP sin PI BAPI_PO_GETDETAIL1 request");
            Log.Info(request.ToXml());
            var response = agent.BAPI_PO_GETDETAIL1(request);
            Log.Info($"SAP sin PI BAPI_PO_GETDETAIL1 response");
            Log.Info(response.ToXml());
            return response;
        }

        private OrdenDeCompraSAPDto MapOrdenDeCompraSAPDto(ObtenerOrdenDeCompraWebServiceMOA.BAPIEIKP result, ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOHEADER POHEADER,
            ObtenerOrdenDeCompraWebServiceMOA.BAPIRET2[] RETURN, ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOITEM[] POITEM, ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOTEXTHEADER[] POTEXTHEADER,
            ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOTEXT[] POTEXTITEM, ObtenerOrdenDeCompraWebServiceMOA.BAPIESLLC[] POSERVICES, ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOSCHEDULE[] POSCHEDULE,
            ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOADDRDELIVERY[] POADDRDELIVERY, ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOCOND[] POCOND, ObtenerOrdenDeCompraWebServiceMOA.BAPIESKLC[] POSRVACCESSVALUES,
            ObtenerOrdenDeCompraWebServiceMOA.BAPIEKBE[] POHISTORY)
        {
            OrdenDeCompraSAPDto resultado = new OrdenDeCompraSAPDto();

            if (RETURN != null)
            {
                if (RETURN.Length > 0)
                {
                    resultado.Error = new ErrorOC
                    {
                        Mensaje = RETURN[0].MESSAGE,
                        Tipo = RETURN[0].TYPE
                    };
                }
            }
            if (resultado.Error == null)
            {

                resultado.Cabecera = new OrdenDeCompraSAPCabecera
                {
                    OrdenDeCompra = POHEADER.PO_NUMBER,
                    CodigoProveedor = POHEADER.VENDOR,
                    UsuarioComprasSAP = POHEADER.CREATED_BY,
                    Moneda = POHEADER.CURRENCY,
                    FechaCreacion = DateTime.ParseExact(POHEADER.CREAT_DATE, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
                    FechaCreacionString = DateTime.ParseExact(POHEADER.CREAT_DATE, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture).ToShortDateString(),
                    OrganizacionDeComprasCodigo = POHEADER.PURCH_ORG,
                    SaldoDisponible = CalcularSaldoDisponible(POHISTORY)
                };

                foreach (var pos in POITEM.ToList())
                {
                    resultado.Cabecera.MontoTotal += pos.NET_PRICE * pos.QUANTITY;
                    var region = POADDRDELIVERY.SingleOrDefault(x => x.PO_ITEM == pos.PO_ITEM);
                    var plazo = POSCHEDULE.SingleOrDefault(x => x.PO_ITEM == pos.PO_ITEM);
                    resultado.Posiciones.Add(new OrdenDeCompraSAPPosicion
                    {
                        Indice = pos.PO_ITEM,
                        IndiceSolp = pos.PREQ_ITEM,
                        RegistroInfo = pos.INFO_REC,
                        NroSolp = pos.PREQ_NO,
                        TipoPosicion = pos.ITEM_CAT == "9" ? "SERVICIO" : "MATERIALES",
                        DireccionDeEntrega = new OrdenDeCompraSAPPosicionDireccionDeEntrega
                        {
                            RegionSap = region?.REGION
                        },
                        PlazoDeOferta = !string.IsNullOrEmpty(plazo?.DELIVERY_DATE) ?
                             DateTime.ParseExact(plazo.DELIVERY_DATE, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) : (DateTime?)null

                    });
                }

                //Certificaciones
                foreach (var poh in POHISTORY.Where(x => x.PROCESS_ID == "9" && x.HIST_TYPE == "D"))
                {
                    resultado.Certificaciones.Add(new OrdenDeCompraSAPCertificacion
                    {
                        NroCertificacion = poh.MAT_DOC,
                        Saldo = poh.VAL_FORCUR,
                        Moneda = poh.CURRENCY,
                        MontoFormateado = SAPFormatter.FormatearMonto(poh.VAL_FORCUR, poh.CURRENCY)
                        //importe = poh.importe // Nos tienen que decir el nombre de este campo
                    });
                }

                foreach (var poh in POHISTORY.Where(x => (x.PROCESS_ID == "2" && x.HIST_TYPE == "Q") || (x.PROCESS_ID == "3" && x.HIST_TYPE == "N")))
                {
                    var certificacion = resultado.Certificaciones.First(x => x.NroCertificacion == poh.REF_DOC);
                    switch (poh.HIST_TYPE)
                    {
                        case "Q":
                            certificacion.Saldo -= poh.VAL_LOCCUR;
                            certificacion.MontoFormateado = SAPFormatter.FormatearMonto(certificacion.Saldo, certificacion.Moneda);
                            break;
                        case "N":
                            certificacion.Saldo += poh.VAL_LOCCUR;
                            certificacion.MontoFormateado = SAPFormatter.FormatearMonto(certificacion.Saldo, certificacion.Moneda);
                            break;
                        default:
                            break;
                    }
                }
                //resultado.Certificaciones = resultado.Certificaciones.Where(x => x.Saldo > 0).ToList();
            }

            return resultado;
        }
        private OrdenDeCompraSAPDto MapOrdenDeCompraSAPSinPIDto(WS_GAQ_sin_PI_DIRECT_2012.BAPI_PO_GETDETAIL1Response response)
        {
            OrdenDeCompraSAPDto resultado = new OrdenDeCompraSAPDto();

            if (response.RETURN != null)
            {
                if (response.RETURN.Length > 0)
                {
                    resultado.Error = new ErrorOC
                    {
                        Mensaje = response.RETURN[0].MESSAGE,
                        Tipo = response.RETURN[0].TYPE
                    };
                }
            }
            if (resultado.Error == null)
            {

                resultado.Cabecera = new OrdenDeCompraSAPCabecera
                {
                    OrdenDeCompra = response.POHEADER.PO_NUMBER,
                    CodigoProveedor = response.POHEADER.VENDOR,
                    UsuarioComprasSAP = response.POHEADER.CREATED_BY,
                    Moneda = response.POHEADER.CURRENCY,
                    FechaCreacion = DateTime.ParseExact(response.POHEADER.CREAT_DATE, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
                    FechaCreacionString = DateTime.ParseExact(response.POHEADER.CREAT_DATE, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture).ToShortDateString(),
                    OrganizacionDeComprasCodigo = response.POHEADER.PURCH_ORG,
                    SaldoDisponible = CalcularSaldoDisponibleSinPI(response.POHISTORY)
                };

                foreach (var pos in response.POITEM.ToList())
                {
                    resultado.Cabecera.MontoTotal += pos.NET_PRICE * pos.QUANTITY;
                    var region = response.POADDRDELIVERY.SingleOrDefault(x => x.PO_ITEM == pos.PO_ITEM);
                    var plazo = response.POSCHEDULE.SingleOrDefault(x => x.PO_ITEM == pos.PO_ITEM);
                    resultado.Posiciones.Add(new OrdenDeCompraSAPPosicion
                    {
                        Indice = pos.PO_ITEM,
                        IndiceSolp = pos.PREQ_ITEM,
                        RegistroInfo = pos.INFO_REC,
                        NroSolp = pos.PREQ_NO,
                        TipoPosicion = pos.ITEM_CAT == "9" ? "SERVICIO" : "MATERIALES",
                        DireccionDeEntrega = new OrdenDeCompraSAPPosicionDireccionDeEntrega
                        {
                            RegionSap = region?.REGION
                        },
                        PlazoDeOferta = !string.IsNullOrEmpty(plazo?.DELIVERY_DATE) ?
                             DateTime.ParseExact(plazo.DELIVERY_DATE, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) : (DateTime?)null

                    });
                }

                //Certificaciones
                foreach (var poh in response.POHISTORY.Where(x => x.PROCESS_ID == "9" && x.HIST_TYPE == "D"))
                {
                    resultado.Certificaciones.Add(new OrdenDeCompraSAPCertificacion
                    {
                        NroCertificacion = poh.MAT_DOC,
                        Saldo = poh.VAL_LOCCUR,
                        Moneda = poh.CURRENCY,
                        //importe = poh.importe // Nos tienen que decir el nombre de este campo
                    });
                }

                foreach (var poh in response.POHISTORY.Where(x => (x.PROCESS_ID == "2" && x.HIST_TYPE == "Q") || (x.PROCESS_ID == "3" && x.HIST_TYPE == "N")))
                {
                    var certificacion = resultado.Certificaciones.First(x => x.NroCertificacion == poh.REF_DOC);
                    switch (poh.HIST_TYPE)
                    {
                        case "Q":
                            certificacion.Saldo -= poh.VAL_LOCCUR;
                            break;
                        case "N":
                            certificacion.Saldo += poh.VAL_LOCCUR;
                            break;
                        default:
                            break;
                    }
                }
            }

            return resultado;
        }

        private decimal CalcularSaldoDisponible(ObtenerOrdenDeCompraWebServiceMOA.BAPIEKBE[] POHISTORY)
        {
            var registros = POHISTORY.ToList();
            // Suma de registros con process_id = 9 y hist_type = 'D'
            var sumaProcess9HistD = registros
                .Where(r => r.PROCESS_ID == "9" && r.HIST_TYPE == "D")
                .Sum(r => r.VAL_LOCCUR);

            // Suma de registros con process_id = 2 y (hist_type = 'Q' o hist_type = 'R')
            var sumaProcess2HistQR = registros
                .Where(r => r.PROCESS_ID == "2" && (r.HIST_TYPE == "Q" || r.HIST_TYPE == "R"))
                .Sum(r => r.VAL_LOCCUR);
            return sumaProcess9HistD - sumaProcess2HistQR;
        }
        private decimal CalcularSaldoDisponibleSinPI(WS_GAQ_sin_PI_DIRECT_2012.BAPIEKBE[] POHISTORY)
        {
            var registros = POHISTORY.ToList();
            // Suma de registros con process_id = 9 y hist_type = 'D'
            var sumaProcess9HistD = registros
                .Where(r => r.PROCESS_ID == "9" && r.HIST_TYPE == "D")
                .Sum(r => r.VAL_LOCCUR);

            // Suma de registros con process_id = 2 y (hist_type = 'Q' o hist_type = 'R')
            var sumaProcess2HistQR = registros
                .Where(r => r.PROCESS_ID == "2" && (r.HIST_TYPE == "Q" || r.HIST_TYPE == "R"))
                .Sum(r => r.VAL_LOCCUR);
            return sumaProcess9HistD - sumaProcess2HistQR;
        }

        private AdjudicacionDto MapAdjudicacionDto(ObtenerOrdenDeCompraWebServiceMOA.BAPIEIKP result, ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOHEADER POHEADER, ObtenerOrdenDeCompraWebServiceMOA.BAPIRET2[] RETURN,
                                                   ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOITEM[] POITEM, ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOTEXTHEADER[] POTEXTHEADER, ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOTEXT[] POTEXTITEM,
                                                   ObtenerOrdenDeCompraWebServiceMOA.BAPIESLLC[] POSERVICES, ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOSCHEDULE[] POSCHEDULE, ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOADDRDELIVERY[] POADDRDELIVERY,
                                                   ObtenerOrdenDeCompraWebServiceMOA.BAPIESKLC[] POSRVACCESSVALUES)
        {
            AdjudicacionDto adjudicacion = new AdjudicacionDto();
            var codigoMateriales = POITEM.Select(a => a.MATERIAL).ToList();
            var materiales = repositorio.Listar<MaterialSolp>(x => codigoMateriales.Contains(x.CodigoSap));

            if (RETURN == null)
            {
                return null;
            }

            var monedas = repositorio.Listar<TablaSap>(a => a.Tabla == "Moneda");
            var unidades = repositorio.Listar<TablaSap>(a => a.Tabla == "Unidad");
            var condicionesDePago = repositorio.Listar<TablaSap>(a => a.Tabla == TablasSap.CondicionesDePago);
            var condicionesDeImportacion = repositorio.Listar<TablaSap>(a => a.Tabla == TablasSap.CondicionesDeImportacion);
            var unidadesSAP = repositorio.Listar<UnidadMedidaSap>();
            List<RegionSap> regiones = repositorio.Listar<RegionSap>(x => x.CodigoPais == "AR").ToList();
            var nroSolp = POITEM.FirstOrDefault().PREQ_NO;
            var solp = repositorio.Obtener<Solp, int>(x => x.NroSolp == nroSolp, x => x.Id);
            var servicios = new List<ServicioSolp>();
            if (POITEM.First().ITEM_CAT == "9")
            {
                var codigoServicios = POSERVICES.Select(a => a.SERVICE).ToList();
                servicios = repositorio.Listar<ServicioSolp>(x => codigoServicios.Contains(x.Codigo));
            }
            adjudicacion.Solp_Id = solp;

            adjudicacion.Id = 0;
            adjudicacion.TipoPosicionCodigo = POITEM.First().ITEM_CAT == "9" ? "SERVICIO" : "MATERIALES";
            adjudicacion.NumeroOrdenDeCompra = POHEADER.PO_NUMBER;
            adjudicacion.Proveedor = POHEADER.VENDOR;
            adjudicacion.Centro = POADDRDELIVERY.FirstOrDefault()?.NAME;
            adjudicacion.CalleEntrega = POADDRDELIVERY.FirstOrDefault()?.STREET;
            adjudicacion.CodigoPostal = POADDRDELIVERY.FirstOrDefault()?.POSTL_COD1;
            adjudicacion.PrecioFinal = POITEM.Where(a => a.DELETE_IND != "L").Sum(a => a.QUANTITY * a.NET_PRICE);
            adjudicacion.TextoDeCabecera = string.Join(" ", POTEXTHEADER.Where(a => a.TEXT_ID == "F01").Select(a => a.TEXT_LINE));
            adjudicacion.CondicionesDeEntrega = string.Join(" ", POTEXTHEADER.Where(a => a.TEXT_ID == "F05").Select(a => a.TEXT_LINE));
            adjudicacion.CondicionesDePago = string.Join(" ", POTEXTHEADER.Where(a => a.TEXT_ID == "F07").Select(a => a.TEXT_LINE));
            adjudicacion.Garantias = string.Join(" ", POTEXTHEADER.Where(a => a.TEXT_ID == "F08").Select(a => a.TEXT_LINE));
            adjudicacion.Moneda_Id = monedas.First(a => a.Codigo == POHEADER.CURRENCY).Id;
            adjudicacion.MonedaDescripcion = monedas.First(a => a.Codigo == POHEADER.CURRENCY).Descripcion;
            adjudicacion.FechaCreacion = DateTime.ParseExact(POHEADER.CREAT_DATE, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            adjudicacion.CondicionDePago = new CondicionDePagoDto
            {
                Id = condicionesDePago.FirstOrDefault(x => x.CodigoSap == POHEADER.PMNTTRMS)?.Id,
                Codigo = POHEADER.PMNTTRMS,
                CodigoDescripcion = POHEADER.PMNTTRMS + " - " + condicionesDePago.FirstOrDefault(x => x.CodigoSap == POHEADER.PMNTTRMS)?.Descripcion,
                Descripcion = condicionesDePago.FirstOrDefault(x => x.CodigoSap == POHEADER.PMNTTRMS)?.Descripcion
            };
            adjudicacion.PagoEn1 = POHEADER.DSCNT1_TO;
            adjudicacion.PagoEn2 = POHEADER.DSCNT2_TO;
            adjudicacion.PagoEn3 = POHEADER.DSCNT3_TO;
            adjudicacion.PagoEn1Porcentaje = POHEADER.DSCT_PCT1;
            adjudicacion.PagoEn2Porcentaje = POHEADER.DSCT_PCT2;
            adjudicacion.CondicionDeImportacion = new CondicionDeImportacionDto
            {
                Codigo = POHEADER.INCOTERMS1,
                Descripcion = POHEADER.INCOTERMS2,
                Id = condicionesDeImportacion.FirstOrDefault(x => x.CodigoSap == POHEADER.PMNTTRMS)?.Id
            };

            adjudicacion.CondicionDeImportacionDescripcion = POHEADER.INCOTERMS2;
            adjudicacion.AdjudicacionPosiciones = new List<AdjudicacionPosicionDto>();

            foreach (var posicion in POITEM.Where(a => a.DELETE_IND != "L"))
            {
                AdjudicacionPosicionDto pos = new AdjudicacionPosicionDto();
                var region = POADDRDELIVERY.Where(x => x.PO_ITEM == posicion.PO_ITEM).SingleOrDefault();

                pos.DireccionDeEntrega = new OrdenDeCompraSAPPosicionDireccionDeEntrega
                {
                    RegionSap = region.REGION,
                    PaisSap = region.COUNTRY,
                    Id = regiones.FirstOrDefault(x => x.CodigoSap == region.REGION)?.Id,
                    CodigoSap = region.REGION,
                    Descripcion = region.COUNTRY
                };
                pos.RegionCodigo = region.REGION;
                pos.PaisSap = region.COUNTRY;
                pos.RegionId = regiones.FirstOrDefault(x => x.CodigoSap == region.REGION)?.Id;

                pos.SolpPosicion_Id = 0;
                pos.Id = 0;
                pos.MaterialComprasCodigo = !string.IsNullOrEmpty(posicion.MATERIAL) ? materiales.Where(x => x.CodigoSap == posicion.MATERIAL).FirstOrDefault()?.CodigoSap : "";
                pos.MaterialComprasDescripcion = !string.IsNullOrEmpty(posicion.MATERIAL) ? materiales.Where(x => x.CodigoSap == posicion.MATERIAL).FirstOrDefault()?.Descripcion : "";
                pos.MaterialTextoAmpliado = !string.IsNullOrEmpty(posicion.MATERIAL) ? materiales.Where(x => x.CodigoSap == posicion.MATERIAL).FirstOrDefault()?.TextoAmpliado : "";
                pos.Indice = int.Parse(posicion.PO_ITEM);
                pos.Tarea = posicion.SHORT_TEXT;
                pos.TextoSuministro = string.Join(" ", POTEXTITEM.Where(a => a.PO_ITEM == posicion.PO_ITEM && a.TEXT_ID == "F02").Select(a => a.TEXT_LINE));
                pos.Modelo = "";// posicion.Posicion.Modelo;
                pos.Cantidad = posicion.QUANTITY;
                pos.PrecioUnidad = posicion.NET_PRICE;
                pos.MonedaId = monedas.FirstOrDefault(a => a.Codigo == POHEADER.CURRENCY)?.Id;
                var unidadSap = unidadesSAP.FirstOrDefault(a => a.UM == posicion.PO_UNIT);
                var unidad = unidades.FirstOrDefault(a => a.Codigo == unidadSap?.Comercial);
                pos.UnidadId = unidad.Id;
                pos.UnidadDescripcion = unidad?.Descripcion ?? "";
                pos.UnidadCodigo = unidad?.CodigoSap ?? "";
                pos.MonedaDescripcion = monedas.FirstOrDefault(a => a.Codigo == POHEADER.CURRENCY)?.Descripcion;
                pos.MonedaCodigo = monedas.FirstOrDefault(a => a.Codigo == POHEADER.CURRENCY)?.CodigoSap;
                pos.PrecioTotal = posicion.QUANTITY * posicion.NET_PRICE;
                pos.CentroComprasCodigo = posicion.PLANT;
                pos.Eliminado = posicion.DELETE_IND == "L";
                pos.EntregaFinal = posicion.NO_MORE_GR == "X";
                pos.Moneda = new TablaSapDto
                {
                    Codigo = pos.MonedaCodigo,
                    Descripcion = pos.MonedaDescripcion
                };


                try
                {
                    var fecha = POSCHEDULE.First(a => a.PO_ITEM == posicion.PO_ITEM).DELIVERY_DATE;
                    pos.FechaEntregaServicio = DateTime.ParseExact(fecha, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    pos.FechaEntregaServicioFormateado = fecha;

                }
                catch
                {
                    pos.FechaEntregaServicio = null;
                }

                pos.PlazoDeOferta = Decimal.ToInt32(posicion.PLAN_DEL);
                if (POITEM.First().ITEM_CAT == "9")
                {
                    pos.SubposicionesCompras = new List<SolpSubposicionDto>();
                    var SUBPCKG_NO = POSERVICES.First(a => a.PCKG_NO == posicion.PCKG_NO).SUBPCKG_NO;
                    foreach (var subpos in POSERVICES.Where(a => a.PCKG_NO == SUBPCKG_NO))
                    {
                        SolpSubposicionDto sub = new SolpSubposicionDto();
                        sub.Numero = int.Parse(subpos.LINE_NO);
                        sub.Tarea = subpos.SHORT_TEXT;
                        sub.CodigoSolp = servicios.FirstOrDefault(a => a.Codigo == subpos.SERVICE)?.CodigoSap ?? 0;
                        sub.Cantidad = subpos.QUANTITY;
                        sub.PrecioBruto = subpos.NET_VALUE / subpos.QUANTITY;
                        var unidadSapsp = unidadesSAP.FirstOrDefault(a => a.Comercial == subpos.BASE_UOM);
                        var unidadsp = unidades.FirstOrDefault(a => a.Codigo == unidadSapsp?.Comercial);
                        sub.UnidadComprasDescripcion = unidadsp?.Descripcion ?? "";
                        sub.MonedaCotizacionDescripcion = POHEADER.CURRENCY;
                        sub.MonedaCotizacionCodigo = monedas.FirstOrDefault(a => a.Codigo == POHEADER.CURRENCY)?.CodigoSap;
                        sub.PrecioTotalSubPosicion = subpos.NET_VALUE;
                        sub.Eliminado = subpos.DELETE_IND == "L";
                        pos.SubposicionesCompras.Add(sub);
                    }
                }
                else
                {
                    foreach (var subpos in POSERVICES.Where(a => a.PCKG_NO == posicion.PCKG_NO).Skip(1))
                    {
                        SolpSubposicionDto sub = new SolpSubposicionDto();
                        sub.Numero = int.Parse(subpos.LINE_NO);
                        sub.Tarea = subpos.SHORT_TEXT;
                        sub.CodigoSolp = servicios.FirstOrDefault(a => a.Codigo == subpos.SERVICE)?.CodigoSap ?? 0;
                        sub.Cantidad = subpos.QUANTITY;
                        sub.PrecioBruto = subpos.NET_VALUE / subpos.PRICE_UNIT;
                        sub.UnidadComprasDescripcion = unidades.FirstOrDefault(a => a.Codigo == subpos.BASE_UOM)?.Descripcion ?? "";
                        sub.MonedaCotizacionDescripcion = POHEADER.CURRENCY;
                        sub.PrecioTotalSubPosicion = sub.Cantidad ?? 0 * sub.PrecioBruto ?? 0;
                        sub.Eliminado = subpos.DELETE_IND == "L";
                        pos.SubposicionesCompras.Add(sub);
                    }
                }

                adjudicacion.AdjudicacionPosiciones.Add(pos);
                if (adjudicacion.AdjudicacionPosiciones.Count > 0)
                {
                    adjudicacion.AdjudicacionPosiciones[0].MonedaCodigo = monedas.FirstOrDefault(a => a.Codigo == POHEADER.CURRENCY)?.CodigoSap;
                    adjudicacion.AdjudicacionPosiciones[0].MonedaDescripcion = POHEADER.CURRENCY;

                }

            }

            return adjudicacion;
        }

        private AdjudicacionDto MapAdjudicacionSinPIDto(WS_GAQ_sin_PI_DIRECT_2012.BAPI_PO_GETDETAIL1Response response)
        {
            AdjudicacionDto adjudicacion = new AdjudicacionDto();
            var codigoMateriales = response.POITEM.Select(a => a.MATERIAL).ToList();
            var materiales = repositorio.Listar<MaterialSolp>(x => codigoMateriales.Contains(x.CodigoSap));

            if (response.RETURN == null)
            {
                return null;
            }

            var monedas = repositorio.Listar<TablaSap>(a => a.Tabla == "Moneda");
            var unidades = repositorio.Listar<TablaSap>(a => a.Tabla == "Unidad");
            var condicionesDePago = repositorio.Listar<TablaSap>(a => a.Tabla == TablasSap.CondicionesDePago);
            var condicionesDeImportacion = repositorio.Listar<TablaSap>(a => a.Tabla == TablasSap.CondicionesDeImportacion);
            var unidadesSAP = repositorio.Listar<UnidadMedidaSap>();
            List<RegionSap> regiones = repositorio.Listar<RegionSap>(x => x.CodigoPais == "AR").ToList();
            var nroSolp = response.POITEM.FirstOrDefault().PREQ_NO;
            var solp = repositorio.Obtener<Solp, int>(x => x.NroSolp == nroSolp, x => x.Id);
            var servicios = new List<ServicioSolp>();
            if (response.POITEM.First().ITEM_CAT == "9")
            {
                var codigoServicios = response.POSERVICES.Select(a => a.SERVICE).ToList();
                servicios = repositorio.Listar<ServicioSolp>(x => codigoServicios.Contains(x.Codigo));
            }
            adjudicacion.Solp_Id = solp;

            adjudicacion.Id = 0;
            adjudicacion.TipoPosicionCodigo = response.POITEM.First().ITEM_CAT == "9" ? "SERVICIO" : "MATERIALES";
            adjudicacion.NumeroOrdenDeCompra = response.POHEADER.PO_NUMBER;
            adjudicacion.Proveedor = response.POHEADER.VENDOR;
            adjudicacion.Centro = response.POADDRDELIVERY.FirstOrDefault()?.NAME;
            adjudicacion.CalleEntrega = response.POADDRDELIVERY.FirstOrDefault()?.STREET;
            adjudicacion.CodigoPostal = response.POADDRDELIVERY.FirstOrDefault()?.POSTL_COD1;
            adjudicacion.PrecioFinal = response.POITEM.Where(a => a.DELETE_IND != "L").Sum(a => a.QUANTITY * a.NET_PRICE);
            adjudicacion.TextoDeCabecera = string.Join(" ", response.POTEXTHEADER.Where(a => a.TEXT_ID == "F01").Select(a => a.TEXT_LINE));
            adjudicacion.CondicionesDeEntrega = string.Join(" ", response.POTEXTHEADER.Where(a => a.TEXT_ID == "F05").Select(a => a.TEXT_LINE));
            adjudicacion.CondicionesDePago = string.Join(" ", response.POTEXTHEADER.Where(a => a.TEXT_ID == "F07").Select(a => a.TEXT_LINE));
            adjudicacion.Garantias = string.Join(" ", response.POTEXTHEADER.Where(a => a.TEXT_ID == "F08").Select(a => a.TEXT_LINE));
            adjudicacion.Moneda_Id = monedas.First(a => a.Codigo == response.POHEADER.CURRENCY).Id;
            adjudicacion.MonedaDescripcion = monedas.First(a => a.Codigo == response.POHEADER.CURRENCY).Descripcion;
            adjudicacion.FechaCreacion = DateTime.ParseExact(response.POHEADER.CREAT_DATE, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            adjudicacion.CondicionDePago = new CondicionDePagoDto
            {
                Id = condicionesDePago.FirstOrDefault(x => x.CodigoSap == response.POHEADER.PMNTTRMS)?.Id,
                Codigo = response.POHEADER.PMNTTRMS,
                CodigoDescripcion = response.POHEADER.PMNTTRMS + " - " + condicionesDePago.FirstOrDefault(x => x.CodigoSap == response.POHEADER.PMNTTRMS)?.Descripcion,
                Descripcion = condicionesDePago.FirstOrDefault(x => x.CodigoSap == response.POHEADER.PMNTTRMS)?.Descripcion
            };
            adjudicacion.PagoEn1 = response.POHEADER.DSCNT1_TO;
            adjudicacion.PagoEn2 = response.POHEADER.DSCNT2_TO;
            adjudicacion.PagoEn3 = response.POHEADER.DSCNT3_TO;
            adjudicacion.PagoEn1Porcentaje = response.POHEADER.DSCT_PCT1;
            adjudicacion.PagoEn2Porcentaje = response.POHEADER.DSCT_PCT2;
            adjudicacion.CondicionDeImportacion = new CondicionDeImportacionDto
            {
                Codigo = response.POHEADER.INCOTERMS1,
                Descripcion = response.POHEADER.INCOTERMS2,
                Id = condicionesDeImportacion.FirstOrDefault(x => x.CodigoSap == response.POHEADER.PMNTTRMS)?.Id
            };

            adjudicacion.CondicionDeImportacionDescripcion = response.POHEADER.INCOTERMS2;
            adjudicacion.AdjudicacionPosiciones = new List<AdjudicacionPosicionDto>();

            foreach (var posicion in response.POITEM.Where(a => a.DELETE_IND != "L"))
            {
                AdjudicacionPosicionDto pos = new AdjudicacionPosicionDto();
                var region = response.POADDRDELIVERY.Where(x => x.PO_ITEM == posicion.PO_ITEM).SingleOrDefault();

                pos.DireccionDeEntrega = new OrdenDeCompraSAPPosicionDireccionDeEntrega
                {
                    RegionSap = region.REGION,
                    PaisSap = region.COUNTRY,
                    Id = regiones.FirstOrDefault(x => x.CodigoSap == region.REGION)?.Id,
                    CodigoSap = region.REGION,
                    Descripcion = region.COUNTRY
                };
                pos.RegionCodigo = region.REGION;
                pos.PaisSap = region.COUNTRY;
                pos.RegionId = regiones.FirstOrDefault(x => x.CodigoSap == region.REGION)?.Id;

                pos.SolpPosicion_Id = 0;
                pos.Id = 0;
                pos.MaterialComprasCodigo = !string.IsNullOrEmpty(posicion.MATERIAL) ? materiales.Where(x => x.CodigoSap == posicion.MATERIAL).FirstOrDefault()?.CodigoSap : "";
                pos.MaterialComprasDescripcion = !string.IsNullOrEmpty(posicion.MATERIAL) ? materiales.Where(x => x.CodigoSap == posicion.MATERIAL).FirstOrDefault()?.Descripcion : "";
                pos.MaterialTextoAmpliado = !string.IsNullOrEmpty(posicion.MATERIAL) ? materiales.Where(x => x.CodigoSap == posicion.MATERIAL).FirstOrDefault()?.TextoAmpliado : "";
                pos.Indice = int.Parse(posicion.PO_ITEM);
                pos.Tarea = posicion.SHORT_TEXT;
                pos.TextoSuministro = string.Join(" ", response.POTEXTITEM.Where(a => a.PO_ITEM == posicion.PO_ITEM && a.TEXT_ID == "F02").Select(a => a.TEXT_LINE));
                pos.Modelo = "";// posicion.Posicion.Modelo;
                pos.Cantidad = posicion.QUANTITY;
                pos.PrecioUnidad = posicion.NET_PRICE;
                pos.MonedaId = monedas.FirstOrDefault(a => a.Codigo == response.POHEADER.CURRENCY)?.Id;
                var unidadSap = unidadesSAP.FirstOrDefault(a => a.UM == posicion.PO_UNIT);
                var unidad = unidades.FirstOrDefault(a => a.Codigo == unidadSap?.Comercial);
                pos.UnidadId = unidad.Id;
                pos.UnidadDescripcion = unidad?.Descripcion ?? "";
                pos.UnidadCodigo = unidad?.CodigoSap ?? "";
                pos.MonedaDescripcion = monedas.FirstOrDefault(a => a.Codigo == response.POHEADER.CURRENCY)?.Descripcion;
                pos.MonedaCodigo = monedas.FirstOrDefault(a => a.Codigo == response.POHEADER.CURRENCY)?.CodigoSap;
                pos.PrecioTotal = posicion.QUANTITY * posicion.NET_PRICE;
                pos.CentroComprasCodigo = posicion.PLANT;
                pos.Eliminado = posicion.DELETE_IND == "L";
                pos.EntregaFinal = posicion.NO_MORE_GR == "X";
                pos.Moneda = new TablaSapDto
                {
                    Codigo = pos.MonedaCodigo,
                    Descripcion = pos.MonedaDescripcion
                };


                try
                {
                    var fecha = response.POSCHEDULE.First(a => a.PO_ITEM == posicion.PO_ITEM).DELIVERY_DATE;
                    pos.FechaEntregaServicio = DateTime.ParseExact(fecha, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    pos.FechaEntregaServicioFormateado = fecha;

                }
                catch
                {
                    pos.FechaEntregaServicio = null;
                }

                pos.PlazoDeOferta = Decimal.ToInt32(posicion.PLAN_DEL);
                if (response.POITEM.First().ITEM_CAT == "9")
                {
                    pos.SubposicionesCompras = new List<SolpSubposicionDto>();
                    var SUBPCKG_NO = response.POSERVICES.First(a => a.PCKG_NO == posicion.PCKG_NO).SUBPCKG_NO;
                    foreach (var subpos in response.POSERVICES.Where(a => a.PCKG_NO == SUBPCKG_NO))
                    {
                        SolpSubposicionDto sub = new SolpSubposicionDto();
                        sub.Numero = int.Parse(subpos.LINE_NO);
                        sub.Tarea = subpos.SHORT_TEXT;
                        sub.CodigoSolp = servicios.FirstOrDefault(a => a.Codigo == subpos.SERVICE)?.CodigoSap ?? 0;
                        sub.Cantidad = subpos.QUANTITY;
                        sub.PrecioBruto = subpos.NET_VALUE / subpos.QUANTITY;
                        var unidadSapsp = unidadesSAP.FirstOrDefault(a => a.Comercial == subpos.BASE_UOM);
                        var unidadsp = unidades.FirstOrDefault(a => a.Codigo == unidadSapsp?.Comercial);
                        sub.UnidadComprasDescripcion = unidadsp?.Descripcion ?? "";
                        sub.MonedaCotizacionDescripcion = response.POHEADER.CURRENCY;
                        sub.MonedaCotizacionCodigo = monedas.FirstOrDefault(a => a.Codigo == response.POHEADER.CURRENCY)?.CodigoSap;
                        sub.PrecioTotalSubPosicion = subpos.NET_VALUE;
                        sub.Eliminado = subpos.DELETE_IND == "L";
                        pos.SubposicionesCompras.Add(sub);
                    }
                }
                else
                {
                    foreach (var subpos in response.POSERVICES.Where(a => a.PCKG_NO == posicion.PCKG_NO).Skip(1))
                    {
                        SolpSubposicionDto sub = new SolpSubposicionDto();
                        sub.Numero = int.Parse(subpos.LINE_NO);
                        sub.Tarea = subpos.SHORT_TEXT;
                        sub.CodigoSolp = servicios.FirstOrDefault(a => a.Codigo == subpos.SERVICE)?.CodigoSap ?? 0;
                        sub.Cantidad = subpos.QUANTITY;
                        sub.PrecioBruto = subpos.NET_VALUE / subpos.PRICE_UNIT;
                        sub.UnidadComprasDescripcion = unidades.FirstOrDefault(a => a.Codigo == subpos.BASE_UOM)?.Descripcion ?? "";
                        sub.MonedaCotizacionDescripcion = response.POHEADER.CURRENCY;
                        sub.PrecioTotalSubPosicion = sub.Cantidad ?? 0 * sub.PrecioBruto ?? 0;
                        sub.Eliminado = subpos.DELETE_IND == "L";
                        pos.SubposicionesCompras.Add(sub);
                    }
                }

                adjudicacion.AdjudicacionPosiciones.Add(pos);
                if (adjudicacion.AdjudicacionPosiciones.Count > 0)
                {
                    adjudicacion.AdjudicacionPosiciones[0].MonedaCodigo = monedas.FirstOrDefault(a => a.Codigo == response.POHEADER.CURRENCY)?.CodigoSap;
                    adjudicacion.AdjudicacionPosiciones[0].MonedaDescripcion = response.POHEADER.CURRENCY;

                }

            }

            return adjudicacion;
        }






        /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Obtiene Detalle de una Orden de Compra
        /// Con Posición, item o línea, Entradas de Servicio si las tuviera, Historial de Entradas de Servicio.
        /// </summary>
        /// <param name="numeroDeOrdenCompra"></param>
        /// <returns></returns>
        public DetalleOrdenDeCompraDto ObtenerDetalleDeOrdenDeCompra(string numeroDeOrdenCompra, List<TablaSap> centro, List<TablaSap> almacen, bool usuarioSolp)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var response = ObtenerDetalleDeOrdenDeCompraSapSinPI(numeroDeOrdenCompra);
                    return MapSinPI(response, centro, almacen, usuarioSolp);
                }
                else
                {
                    ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOITEM[] POITEM;
                    ObtenerOrdenDeCompraWebServiceMOA.BAPIRET2[] RETURN;
                    ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOHEADER POHEADER;//
                    ObtenerOrdenDeCompraWebServiceMOA.BAPIEIKP result;
                    ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOTEXTHEADER[] POTEXTHEADER;
                    ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOTEXT[] POTEXTITEM;
                    ObtenerOrdenDeCompraWebServiceMOA.BAPIESLLC[] POSERVICES;
                    ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOSCHEDULE[] POSCHEDULE;
                    ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOADDRDELIVERY[] POADDRDELIVERY;
                    ObtenerOrdenDeCompraWebServiceMOA.BAPIEKBE[] POHISTORY;
                    ObtenerDetalleDeOrdenDeCompraSap(numeroDeOrdenCompra, out POITEM, out RETURN, out POHEADER, out result, out POTEXTHEADER, out POTEXTITEM, out POSERVICES, out POSCHEDULE, out POADDRDELIVERY, out POHISTORY);
                    return Map(result, POHEADER, RETURN, POITEM, POTEXTHEADER, POTEXTITEM, POSERVICES, POSCHEDULE, POADDRDELIVERY, POHISTORY, centro, almacen, usuarioSolp);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Mapea Detalle de una Orden de Compra
        /// </summary>
        private DetalleOrdenDeCompraDto Map(ObtenerOrdenDeCompraWebServiceMOA.BAPIEIKP result, ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOHEADER POHEADER, ObtenerOrdenDeCompraWebServiceMOA.BAPIRET2[] RETURN,
                                            ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOITEM[] POITEM, ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOTEXTHEADER[] POTEXTHEADER, ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOTEXT[] POTEXTITEM,
                                            ObtenerOrdenDeCompraWebServiceMOA.BAPIESLLC[] POSERVICES, ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOSCHEDULE[] POSCHEDULE, ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOADDRDELIVERY[] POADDRDELIVERY,
                                            ObtenerOrdenDeCompraWebServiceMOA.BAPIEKBE[] POHISTORY, List<TablaSap> centros, List<TablaSap> almacenes, bool usuarioSolp)
        {
            DetalleOrdenDeCompraDto detalleOrdenDeCompra = new DetalleOrdenDeCompraDto();

            //Buscar CUIT en tabla de Proveedores con el Codigo de Proveedor.
            //List<Proveedor> _proveedores = repositorio.Listar<Proveedor>(a => a.CodigoProveedor == POHEADER.VENDOR);
            //Proveedor _proveedor = _proveedores.FirstOrDefault(p => p.CodigoProveedor == POHEADER.VENDOR);
            //Como hay codigos de proveedor repetidos.
            //Proveedor _proveedor = repositorio.Listar<Proveedor>(a => a.CodigoProveedor == POHEADER.VENDOR).FirstOrDefault();

            Proveedor _proveedor = repositorio.Listar<Proveedor>(p =>
                p.CodigoProveedor == POHEADER.VENDOR &&
                p.CodigoProveedor.Substring(p.CodigoProveedor.Length - 8) == p.CUIT.Substring(2, 8)
            ).FirstOrDefault();

            detalleOrdenDeCompra.NumeroOrdenDeCompra = POHEADER.PO_NUMBER;
            detalleOrdenDeCompra.Proveedor = POHEADER.VENDOR;
            //detalleOrdenDeCompra.NombreProveedor = POHEADER.
            detalleOrdenDeCompra.MontoTotal = Math.Round(POITEM.Sum(a => a.QUANTITY * a.NET_PRICE), 4);
            //MMSN-491 - Ponerle separador de miles a la columna “Monto Total”. - Separador de miles ( , ) coma - Separador decimal ( . ) punto           
            detalleOrdenDeCompra.MontoTotalString = detalleOrdenDeCompra.MontoTotal.ToString(currencyFormat, System.Globalization.CultureInfo.InvariantCulture);

            ////MMSN-491 - Modificar el formato de fecha. DD/MM/AAAA
            DateTime toFormat = DateTime.ParseExact(POHEADER.CREAT_DATE, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            detalleOrdenDeCompra.FechaCreacion = toFormat.ToString(dateTimeFormat);

            detalleOrdenDeCompra.UsuarioCreador = POHEADER.CREATED_BY;
            detalleOrdenDeCompra.Cuit = _proveedor != null ? DataFormatter.CuitConGuion(_proveedor.CUIT) : "-";
            detalleOrdenDeCompra.Posiciones = new List<PosicionDto>();

            //var centros = repositorio.Listar<TablaSap>(a => a.Tabla == "Centro");
            //var almacenes = repositorio.Listar<TablaSap>(a => a.Tabla == "Almacen");

            //Obtiene las entradas de servicio de la orden de compra, si las tuviera, para luego asignarlas a las posiciones.
            List<EntradaServicioDto> ListaDeEntradasDeServicio = new List<EntradaServicioDto>();
            ListaDeEntradasDeServicio = ObtenerTodasEntradasDeServicio_Nuevo(POSERVICES, POHISTORY);


            /// Recorre cada Posicion en busqueda de itemsOC
            foreach (var posicion in POITEM)
            {
                if (!usuarioSolp)
                {
                    if (posicion.DELETE_IND == "L" || posicion.DELETE_IND == "S")
                        continue;
                }

                PosicionDto pos = new PosicionDto();

                pos.Id = int.Parse(posicion.PCKG_NO);
                pos.NumeroPosicion = long.Parse(posicion.PO_ITEM);
                pos.CodigoMaterial = posicion.MATERIAL?.TrimStart('0');
                pos.Descripcion = posicion.SHORT_TEXT;
                pos.Cantidad = posicion.QUANTITY;
                pos.PrecioUnidad = Math.Round(posicion.NET_PRICE, 4);

                if (pos.PrecioUnidad != null)
                {
                    decimal notNullValue = (decimal)pos.PrecioUnidad;
                    pos.PrecioUnidadString = notNullValue.ToString(currencyFormat, System.Globalization.CultureInfo.InvariantCulture);
                }

                pos.PrecioTotal = Math.Round(posicion.QUANTITY * posicion.NET_PRICE, 4);
                pos.CentroComprasCodigo = posicion.PLANT;
                pos.UM = posicion.PO_UNIT;
                pos.GrupoArticulos = posicion.MATL_GROUP;
                TablaSap centro = centros.FirstOrDefault(a => a.Codigo == posicion.PLANT);
                TablaSap Almacen = almacenes.FirstOrDefault(a => a.Codigo == posicion.STGE_LOC);

                if (centro == null)
                {
                    pos.Centro = posicion.PLANT + "- ";
                }
                ;

                if (centro != null)
                {
                    pos.Centro = posicion.PLANT + "-" + centro.Descripcion;
                }
                ;

                if (Almacen == null)
                {
                    pos.Almacen = posicion.STGE_LOC + "- ";
                }
                ;

                if (Almacen != null)
                {
                    pos.Almacen = posicion.STGE_LOC + "-" + Almacen.Descripcion;
                }
                ;


                //pos.Centro = posicion.PLANT;
                //pos.Almacen = posicion.STGE_LOC;

                pos.NumeroSolp = posicion.PREQ_NO;
                pos.Contrato = posicion.AGREEMENT;
                pos.Solicitante = posicion.PREQ_NAME;
                pos.NoMoreGR = posicion.NO_MORE_GR;

                //pos.MonedaId = posicion.CURRENCY;
                pos.MonedaDescripcion = POHEADER.CURRENCY_ISO;

                pos.NroOrdenCompra = POHEADER.PO_NUMBER;

                /// Obtine los Items de la position
                pos.Items = ObtenerItemsdelaPosicion(POSERVICES, POHISTORY, POHEADER, pos, ListaDeEntradasDeServicio);

                pos.Bloqueada = usuarioSolp && (posicion.DELETE_IND == "L" || posicion.DELETE_IND == "S");

                detalleOrdenDeCompra.Posiciones.Add(pos);
            }

            return detalleOrdenDeCompra;
        }

        private DetalleOrdenDeCompraDto MapSinPI(WS_GAQ_sin_PI_DIRECT_2012.BAPI_PO_GETDETAIL1Response response, List<TablaSap> centros, List<TablaSap> almacenes, bool usuarioSolp)
        {
            DetalleOrdenDeCompraDto detalleOrdenDeCompra = new DetalleOrdenDeCompraDto();

            //Buscar CUIT en tabla de Proveedores con el Codigo de Proveedor.
            //List<Proveedor> _proveedores = repositorio.Listar<Proveedor>(a => a.CodigoProveedor == POHEADER.VENDOR);
            //Proveedor _proveedor = _proveedores.FirstOrDefault(p => p.CodigoProveedor == POHEADER.VENDOR);
            //Como hay codigos de proveedor repetidos.
            //Proveedor _proveedor = repositorio.Listar<Proveedor>(a => a.CodigoProveedor == POHEADER.VENDOR).FirstOrDefault();

            Proveedor _proveedor = repositorio.Listar<Proveedor>(p =>
                p.CodigoProveedor == response.POHEADER.VENDOR &&
                p.CodigoProveedor.Substring(p.CodigoProveedor.Length - 8) == p.CUIT.Substring(2, 8)
            ).FirstOrDefault();

            detalleOrdenDeCompra.NumeroOrdenDeCompra = response.POHEADER.PO_NUMBER;
            detalleOrdenDeCompra.Proveedor = response.POHEADER.VENDOR;
            //detalleOrdenDeCompra.NombreProveedor = POHEADER.
            detalleOrdenDeCompra.MontoTotal = Math.Round(response.POITEM.Sum(a => a.QUANTITY * a.NET_PRICE), 4);
            //MMSN-491 - Ponerle separador de miles a la columna “Monto Total”. - Separador de miles ( , ) coma - Separador decimal ( . ) punto           
            detalleOrdenDeCompra.MontoTotalString = detalleOrdenDeCompra.MontoTotal.ToString(currencyFormat, System.Globalization.CultureInfo.InvariantCulture);

            ////MMSN-491 - Modificar el formato de fecha. DD/MM/AAAA
            DateTime toFormat = DateTime.ParseExact(response.POHEADER.CREAT_DATE, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            detalleOrdenDeCompra.FechaCreacion = toFormat.ToString(dateTimeFormat);
            detalleOrdenDeCompra.UsuarioCreador = response.POHEADER.CREATED_BY;
            detalleOrdenDeCompra.Cuit = _proveedor != null ? DataFormatter.CuitConGuion(_proveedor.CUIT) : "-";
            detalleOrdenDeCompra.Posiciones = new List<PosicionDto>();

            //var centros = repositorio.Listar<TablaSap>(a => a.Tabla == "Centro");
            //var almacenes = repositorio.Listar<TablaSap>(a => a.Tabla == "Almacen");

            //Obtiene las entradas de servicio de la orden de compra, si las tuviera, para luego asignarlas a las posiciones.
            List<EntradaServicioDto> ListaDeEntradasDeServicio = new List<EntradaServicioDto>();
            ListaDeEntradasDeServicio = ObtenerTodasEntradasDeServicio_NuevoSinPI(response.POSERVICES, response.POHISTORY);


            /// Recorre cada Posicion en busqueda de itemsOC
            foreach (var posicion in response.POITEM)
            {
                if (!usuarioSolp)
                {
                    if (posicion.DELETE_IND == "L" || posicion.DELETE_IND == "S")
                        continue;
                }

                PosicionDto pos = new PosicionDto();
                pos.Id = int.Parse(posicion.PCKG_NO);
                pos.NumeroPosicion = long.Parse(posicion.PO_ITEM);
                pos.CodigoMaterial = posicion.MATERIAL?.TrimStart('0');
                pos.Descripcion = posicion.SHORT_TEXT;
                pos.Cantidad = posicion.QUANTITY;
                pos.PrecioUnidad = Math.Round(posicion.NET_PRICE, 4);

                if (pos.PrecioUnidad != null)
                {
                    decimal notNullValue = (decimal)pos.PrecioUnidad;
                    pos.PrecioUnidadString = notNullValue.ToString(currencyFormat, System.Globalization.CultureInfo.InvariantCulture);
                }

                pos.PrecioTotal = Math.Round(posicion.QUANTITY * posicion.NET_PRICE, 4);
                pos.CentroComprasCodigo = posicion.PLANT;
                pos.UM = posicion.PO_UNIT;
                pos.GrupoArticulos = posicion.MATL_GROUP;
                TablaSap centro = centros.FirstOrDefault(a => a.Codigo == posicion.PLANT);
                TablaSap Almacen = almacenes.FirstOrDefault(a => a.Codigo == posicion.STGE_LOC);

                if (centro == null)
                {
                    pos.Centro = posicion.PLANT + "- ";
                }

                if (centro != null)
                {
                    pos.Centro = posicion.PLANT + "-" + centro.Descripcion;
                }

                if (Almacen == null)
                {
                    pos.Almacen = posicion.STGE_LOC + "- ";
                }

                if (Almacen != null)
                {
                    pos.Almacen = posicion.STGE_LOC + "-" + Almacen.Descripcion;
                }

                //pos.Centro = posicion.PLANT;
                //pos.Almacen = posicion.STGE_LOC;
                pos.NumeroSolp = posicion.PREQ_NO;
                pos.Contrato = posicion.AGREEMENT;
                pos.Solicitante = posicion.PREQ_NAME;
                pos.NoMoreGR = posicion.NO_MORE_GR;

                //pos.MonedaId = posicion.CURRENCY;
                pos.MonedaDescripcion = response.POHEADER.CURRENCY_ISO;
                pos.NroOrdenCompra = response.POHEADER.PO_NUMBER;

                /// Obtine los Items de la position
                pos.Items = ObtenerItemsdelaPosicionSinPI(response.POSERVICES, response.POHISTORY, response.POHEADER, pos, ListaDeEntradasDeServicio);
                pos.Bloqueada = usuarioSolp && (posicion.DELETE_IND == "L" || posicion.DELETE_IND == "S");
                detalleOrdenDeCompra.Posiciones.Add(pos);
            }

            return detalleOrdenDeCompra;
        }

        /// <summary>
        /// Obtener los Items de la position
        /// </summary>
        private List<ItemDto> ObtenerItemsdelaPosicion(ObtenerOrdenDeCompraWebServiceMOA.BAPIESLLC[] pOSERVICES, ObtenerOrdenDeCompraWebServiceMOA.BAPIEKBE[] pOHISTORY, ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOHEADER POHEADER,
                                                        PosicionDto Posicion,
                                                        List<EntradaServicioDto> ListaDeEntradasDeServicio)
        {
            List<ItemDto> itemsDeLaPosicion = new List<ItemDto>();
            string idPosicionString = Posicion.Id.ToString("D10");
            var itemsValidos = pOSERVICES.Where(x => x.PCKG_NO == idPosicionString).FirstOrDefault();

            if (itemsValidos == null)
                return itemsDeLaPosicion;

            var items = pOSERVICES.Where(x => x.PCKG_NO == itemsValidos.SUBPCKG_NO);

            foreach (var item in items)
            {

                ItemDto itemDto = new ItemDto();

                itemDto.Id = item.PCKG_NO;
                itemDto.LINE_NO = item.LINE_NO;
                itemDto.NumeroLinea = int.Parse(item.EXT_LINE);
                itemDto.Descripcion = item.SHORT_TEXT;
                itemDto.Cantidad = item.QUANTITY;
                itemDto.PosicionId = Convert.ToInt32(item.PCKG_NO);
                itemDto.PrecioBruto = Math.Round((item.QUANTITY != 0) ? item.NET_VALUE / item.QUANTITY : 0, 4);
                itemDto.ServicioNumero = item.SERVICE != "" ? long.Parse(item.SERVICE) : 0;
                itemDto.UM = item.BASE_UOM;
                itemDto.Importe = Math.Round((item.QUANTITY != 0) ? item.NET_VALUE / item.QUANTITY : 0, 4);

                if (itemDto.Importe != null)
                {
                    decimal notNullValue = (decimal)itemDto.Importe;
                    itemDto.ImporteString = notNullValue.ToString(currencyFormat, System.Globalization.CultureInfo.InvariantCulture);
                }

                //MMSN-460 - Moneda
                itemDto.Moneda = POHEADER.CURRENCY;
                //MMSN-460 - Nro de servicio
                itemDto.ServicioNumero = item.SERVICE != "" ? int.Parse(item.SERVICE) : 0;
                //MMSN-460 - Porcentaje (inicialización - necesaria para FE)
                itemDto.Porcentaje = "0"; // si no tiene entradas de servicios asociadas el porcentaje es 0
                itemDto.CantidadReal = 0; // si no tiene entras de servicios asociadas la cantidad real es = 0

                itemDto.EntradasServicio = ObtenerEntradasDeServicioDelItem_Nuevo(pOSERVICES, pOHISTORY, itemDto.Id, itemDto.LINE_NO, ListaDeEntradasDeServicio);

                if (itemDto.EntradasServicio.Count > 0)
                {
                    itemDto = CalcularCampos(itemDto);
                }

                itemDto.NroOrdenCompra = POHEADER.PO_NUMBER;
                itemDto.NroPosicion = Posicion.NumeroPosicion.ToString();

                itemsDeLaPosicion.Add(itemDto);
            }

            return itemsDeLaPosicion;
        }

        private List<ItemDto> ObtenerItemsdelaPosicionSinPI(WS_GAQ_sin_PI_DIRECT_2012.BAPIESLLC[] pOSERVICES, WS_GAQ_sin_PI_DIRECT_2012.BAPIEKBE[] pOHISTORY, WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOHEADER POHEADER,
                                                PosicionDto Posicion,
                                                List<EntradaServicioDto> ListaDeEntradasDeServicio)
        {
            List<ItemDto> itemsDeLaPosicion = new List<ItemDto>();
            string idPosicionString = Posicion.Id.ToString("D10");
            var itemsValidos = pOSERVICES.Where(x => x.PCKG_NO == idPosicionString).FirstOrDefault();

            if (itemsValidos == null)
                return itemsDeLaPosicion;

            var items = pOSERVICES.Where(x => x.PCKG_NO == itemsValidos.SUBPCKG_NO);

            foreach (var item in items)
            {

                ItemDto itemDto = new ItemDto();

                itemDto.Id = item.PCKG_NO;
                itemDto.LINE_NO = item.LINE_NO;
                itemDto.NumeroLinea = int.Parse(item.EXT_LINE);
                itemDto.Descripcion = item.SHORT_TEXT;
                itemDto.Cantidad = item.QUANTITY;
                itemDto.PosicionId = Convert.ToInt32(item.PCKG_NO);
                itemDto.PrecioBruto = Math.Round((item.QUANTITY != 0) ? item.NET_VALUE / item.QUANTITY : 0, 4);
                itemDto.ServicioNumero = item.SERVICE != "" ? long.Parse(item.SERVICE) : 0;
                itemDto.UM = item.BASE_UOM;
                itemDto.Importe = Math.Round((item.QUANTITY != 0) ? item.NET_VALUE / item.QUANTITY : 0, 4);

                if (itemDto.Importe != null)
                {
                    decimal notNullValue = (decimal)itemDto.Importe;
                    itemDto.ImporteString = notNullValue.ToString(currencyFormat, System.Globalization.CultureInfo.InvariantCulture);
                }

                //MMSN-460 - Moneda
                itemDto.Moneda = POHEADER.CURRENCY;
                //MMSN-460 - Nro de servicio
                itemDto.ServicioNumero = item.SERVICE != "" ? int.Parse(item.SERVICE) : 0;
                //MMSN-460 - Porcentaje (inicialización - necesaria para FE)
                itemDto.Porcentaje = "0"; // si no tiene entradas de servicios asociadas el porcentaje es 0
                itemDto.CantidadReal = 0; // si no tiene entras de servicios asociadas la cantidad real es = 0

                itemDto.EntradasServicio = ObtenerEntradasDeServicioDelItem_NuevoSinPI(pOSERVICES, pOHISTORY, itemDto.Id, itemDto.LINE_NO, ListaDeEntradasDeServicio);

                if (itemDto.EntradasServicio.Count > 0)
                {
                    itemDto = CalcularCampos(itemDto);
                }

                itemDto.NroOrdenCompra = POHEADER.PO_NUMBER;
                itemDto.NroPosicion = Posicion.NumeroPosicion.ToString();

                itemsDeLaPosicion.Add(itemDto);
            }

            return itemsDeLaPosicion;
        }

        /// <summary>
        /// MMSN-460: Calculo de Cantidad Real y Porcentaje para ItemDTO
        /// </summary>
        /// <param name="itemDto"></param>
        /// <returns></returns>
        private ItemDto CalcularCampos(ItemDto itemDto)
        {
            bool calcularPorcentaje = false;

            //MMSN-460 - Cantidad Real
            //Inicializar en 0 si hay elementos en ES
            if (itemDto.EntradasServicio.Count > 0)
            {
                itemDto.CantidadReal = 0;
                calcularPorcentaje = true;
            }

            try
            {
                //recorrer la lista de Entradas de Servicio, y contabilizar la cantidad
                foreach (var es in itemDto.EntradasServicio)
                {
                    if (es.Cantidad != null)
                    {
                        itemDto.CantidadReal = itemDto.CantidadReal + es.Cantidad;
                    }

                }

                //MMSN-460 - Porcentaje (% del item = cantidadReal x 100 / cantidad)
                if (itemDto.Cantidad != null && itemDto.Cantidad != 0)
                {
                    if (calcularPorcentaje == true && (itemDto.CantidadReal != null && itemDto.CantidadReal != 0))
                    {
                        double res = Convert.ToDouble((itemDto.CantidadReal * 100) / itemDto.Cantidad);
                        itemDto.Porcentaje = res.ToString("0.##", CultureInfo.InvariantCulture);

                        if (itemDto.Porcentaje.EndsWith(".00"))
                        {
                            var redondeo = Math.Round(res);
                            itemDto.Porcentaje = res.ToString(CultureInfo.InvariantCulture);
                        }
                    }
                }
            }
            catch (Exception)
            {
                //Evito detener ejecución
            }

            return itemDto;
        }
        private void ObtenerDetalleDeOrdenDeCompraSap(string nroOC, out ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOITEM[] POITEM, out ObtenerOrdenDeCompraWebServiceMOA.BAPIRET2[] RETURN,
                                                  out ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOHEADER POHEADER, out ObtenerOrdenDeCompraWebServiceMOA.BAPIEIKP result, out ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOTEXTHEADER[] POTEXTHEADER,
                                                  out ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOTEXT[] POTEXTITEM, out ObtenerOrdenDeCompraWebServiceMOA.BAPIESLLC[] POSERVICES, out ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOSCHEDULE[] POSCHEDULE,
                                                  out ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOADDRDELIVERY[] POADDRDELIVERY, out ObtenerOrdenDeCompraWebServiceMOA.BAPIEKBE[] POHISTORY
            )
        {
            string ACCOUNT_ASSIGNMENT = "X";
            string DELIVERY_ADDRESS = "X";
            string HEADER_TEXT = "X";
            string INVOICEPLAN = "X";
            string ITEM_TEXT = "X";
            string PURCHASEORDER = nroOC;
            string SERIALNUMBERS = "X";
            string SERVICES = "X";
            string VERSION = "X";

            ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOACCOUNT[] POACCOUNT = new ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOACCOUNT[] { };
            POADDRDELIVERY = new ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOADDRDELIVERY[] { };
            ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOCOND[] POCOND = new ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOCOND[] { };
            POITEM = new ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOITEM[] { };
            POTEXTHEADER = new ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOTEXTHEADER[] { };
            POTEXTITEM = new ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOTEXT[] { };
            RETURN = new ObtenerOrdenDeCompraWebServiceMOA.BAPIRET2[] { };
            POSERVICES = new ObtenerOrdenDeCompraWebServiceMOA.BAPIESLLC[] { };
            POHEADER = new ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOHEADER { };
            POHISTORY = new ObtenerOrdenDeCompraWebServiceMOA.BAPIEKBE[] { };
            ObtenerOrdenDeCompraWebServiceMOA.BAPI_INVOICE_PLAN_HEADER[] INVPLANHEADER = new ObtenerOrdenDeCompraWebServiceMOA.BAPI_INVOICE_PLAN_HEADER[] { };
            ObtenerOrdenDeCompraWebServiceMOA.BAPIMEDCM_ALLVERSIONS[] ALLVERSIONS = new ObtenerOrdenDeCompraWebServiceMOA.BAPIMEDCM_ALLVERSIONS[] { };
            ObtenerOrdenDeCompraWebServiceMOA.BAPIPAREX[] EXTENSIONOUT = new ObtenerOrdenDeCompraWebServiceMOA.BAPIPAREX[] { };
            ObtenerOrdenDeCompraWebServiceMOA.BAPI_INVOICE_PLAN_ITEM[] INVPLANITEM = new ObtenerOrdenDeCompraWebServiceMOA.BAPI_INVOICE_PLAN_ITEM[] { };
            ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOCOMPONENT[] POCOMPONENTS = new ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOCOMPONENT[] { };
            ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOCONDHEADER[] POCONDHEADER = new ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOCONDHEADER[] { };
            ObtenerOrdenDeCompraWebServiceMOA.BAPIEKES[] POCONFIRMATION = new ObtenerOrdenDeCompraWebServiceMOA.BAPIEKES[] { };
            ObtenerOrdenDeCompraWebServiceMOA.BAPIESUCC[] POCONTRACTLIMITS = new ObtenerOrdenDeCompraWebServiceMOA.BAPIESUCC[] { };
            ObtenerOrdenDeCompraWebServiceMOA.BAPIEIPO[] POEXPIMPITEM = new ObtenerOrdenDeCompraWebServiceMOA.BAPIEIPO[] { };
            //BAPIEKBE[] POHISTORY = new BAPIEKBE[] { };
            ObtenerOrdenDeCompraWebServiceMOA.BAPIEKBE_MA[] POHISTORY_MA = new ObtenerOrdenDeCompraWebServiceMOA.BAPIEKBE_MA[] { };
            ObtenerOrdenDeCompraWebServiceMOA.BAPIEKBES[] POHISTORY_TOTALS = new ObtenerOrdenDeCompraWebServiceMOA.BAPIEKBES[] { };
            ObtenerOrdenDeCompraWebServiceMOA.BAPIESUHC[] POLIMITS = new ObtenerOrdenDeCompraWebServiceMOA.BAPIESUHC[] { };
            ObtenerOrdenDeCompraWebServiceMOA.BAPIEKKOP[] POPARTNER = new ObtenerOrdenDeCompraWebServiceMOA.BAPIEKKOP[] { };
            POSCHEDULE = new ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOSCHEDULE[] { };
            ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOSHIPPEXP[] POSHIPPINGEXP = new ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOSHIPPEXP[] { };
            ObtenerOrdenDeCompraWebServiceMOA.BAPIESKLC[] POSRVACCESSVALUES = new ObtenerOrdenDeCompraWebServiceMOA.BAPIESKLC[] { };
            ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOSERIALNO[] SERIALNUMBER = new ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOSERIALNO[] { };

            result = service.BAPI_PO_GETDETAIL1(ACCOUNT_ASSIGNMENT,
                DELIVERY_ADDRESS,
                HEADER_TEXT,
                INVOICEPLAN,
                ITEM_TEXT,
                PURCHASEORDER,
                SERIALNUMBERS,
                SERVICES,
                VERSION,
                ref ALLVERSIONS,
                ref EXTENSIONOUT,
                ref INVPLANHEADER,
                ref INVPLANITEM,
                ref POACCOUNT,
                ref POADDRDELIVERY,
                ref POCOMPONENTS,
                ref POCOND,
                ref POCONDHEADER,
                ref POCONFIRMATION,
                ref POCONTRACTLIMITS,
                ref POEXPIMPITEM,
                ref POHISTORY,
                ref POHISTORY_MA,
                ref POHISTORY_TOTALS,
                ref POITEM,
                ref POLIMITS,
                ref POPARTNER,
                ref POSCHEDULE,
                ref POSERVICES,
                ref POSHIPPINGEXP,
                ref POSRVACCESSVALUES,
                ref POTEXTHEADER,
                ref POTEXTITEM,
                ref RETURN,
                ref SERIALNUMBER,
                out POHEADER);
        }

        private WS_GAQ_sin_PI_DIRECT_2012.BAPI_PO_GETDETAIL1Response ObtenerDetalleDeOrdenDeCompraSapSinPI(string nroOC)
        {
            var agent = new Z_WS_BAPI_DIRECT_2012Client();
            agent.ClientCredentials.UserName.UserName = UserSap;
            agent.ClientCredentials.UserName.Password = PassSap;

            string ACCOUNT_ASSIGNMENT = "X";
            string DELIVERY_ADDRESS = "X";
            string HEADER_TEXT = "X";
            string INVOICEPLAN = "X";
            string ITEM_TEXT = "X";
            string PURCHASEORDER = nroOC;
            string SERIALNUMBERS = "X";
            string SERVICES = "X";
            string VERSION = "X";

            WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOACCOUNT[] POACCOUNT = new WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOACCOUNT[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOADDRDELIVERY[] POADDRDELIVERY = new WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOADDRDELIVERY[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOCOND[] POCOND = new WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOCOND[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOITEM[] POITEM = new WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOITEM[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOTEXTHEADER[] POTEXTHEADER = new WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOTEXTHEADER[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOTEXT[] POTEXTITEM = new WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOTEXT[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPIRET2[] RETURN = new WS_GAQ_sin_PI_DIRECT_2012.BAPIRET2[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPIESLLC[] POSERVICES = new WS_GAQ_sin_PI_DIRECT_2012.BAPIESLLC[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOHEADER POHEADER = new WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOHEADER { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPIEKBE[] POHISTORY = new WS_GAQ_sin_PI_DIRECT_2012.BAPIEKBE[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPI_INVOICE_PLAN_HEADER[] INVPLANHEADER = new WS_GAQ_sin_PI_DIRECT_2012.BAPI_INVOICE_PLAN_HEADER[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPIMEDCM_ALLVERSIONS[] ALLVERSIONS = new WS_GAQ_sin_PI_DIRECT_2012.BAPIMEDCM_ALLVERSIONS[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPIPAREX[] EXTENSIONOUT = new WS_GAQ_sin_PI_DIRECT_2012.BAPIPAREX[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPI_INVOICE_PLAN_ITEM[] INVPLANITEM = new WS_GAQ_sin_PI_DIRECT_2012.BAPI_INVOICE_PLAN_ITEM[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOCOMPONENT[] POCOMPONENTS = new WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOCOMPONENT[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOCONDHEADER[] POCONDHEADER = new WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOCONDHEADER[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPIEKES[] POCONFIRMATION = new WS_GAQ_sin_PI_DIRECT_2012.BAPIEKES[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPIESUCC[] POCONTRACTLIMITS = new WS_GAQ_sin_PI_DIRECT_2012.BAPIESUCC[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPIEIPO[] POEXPIMPITEM = new WS_GAQ_sin_PI_DIRECT_2012.BAPIEIPO[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPIEKBE_MA[] POHISTORY_MA = new WS_GAQ_sin_PI_DIRECT_2012.BAPIEKBE_MA[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPIEKBES[] POHISTORY_TOTALS = new WS_GAQ_sin_PI_DIRECT_2012.BAPIEKBES[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPIESUHC[] POLIMITS = new WS_GAQ_sin_PI_DIRECT_2012.BAPIESUHC[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPIEKKOP[] POPARTNER = new WS_GAQ_sin_PI_DIRECT_2012.BAPIEKKOP[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOSCHEDULE[] POSCHEDULE = new WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOSCHEDULE[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOSHIPPEXP[] POSHIPPINGEXP = new WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOSHIPPEXP[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPIESKLC[] POSRVACCESSVALUES = new WS_GAQ_sin_PI_DIRECT_2012.BAPIESKLC[] { };
            WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOSERIALNO[] SERIALNUMBER = new WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOSERIALNO[] { };

            var request = new BAPI_PO_GETDETAIL1()
            {
                ACCOUNT_ASSIGNMENT = ACCOUNT_ASSIGNMENT,
                DELIVERY_ADDRESS = DELIVERY_ADDRESS,
                HEADER_TEXT = HEADER_TEXT,
                INVOICEPLAN = INVOICEPLAN,
                ITEM_TEXT = ITEM_TEXT,
                PURCHASEORDER = PURCHASEORDER,
                SERIALNUMBERS = SERIALNUMBERS,
                SERVICES = SERVICES,
                VERSION = VERSION,
                ALLVERSIONS = ALLVERSIONS,
                EXTENSIONOUT = EXTENSIONOUT,
                INVPLANHEADER = INVPLANHEADER,
                INVPLANITEM = INVPLANITEM,
                POACCOUNT = POACCOUNT,
                POADDRDELIVERY = POADDRDELIVERY,
                POCOMPONENTS = POCOMPONENTS,
                POCOND = POCOND,
                POCONDHEADER = POCONDHEADER,
                POCONFIRMATION = POCONFIRMATION,
                POCONTRACTLIMITS = POCONTRACTLIMITS,
                POEXPIMPITEM = POEXPIMPITEM,
                POHISTORY = POHISTORY,
                POHISTORY_MA = POHISTORY_MA,
                POHISTORY_TOTALS = POHISTORY_TOTALS,
                POITEM = POITEM,
                POLIMITS = POLIMITS,
                POPARTNER = POPARTNER,
                POSCHEDULE = POSCHEDULE,
                POSERVICES = POSERVICES,
                POSHIPPINGEXP = POSHIPPINGEXP,
                POSRVACCESSVALUES = POSRVACCESSVALUES,
                POTEXTHEADER = POTEXTHEADER,
                POTEXTITEM = POTEXTITEM,
                RETURN = RETURN,
                SERIALNUMBER = SERIALNUMBER,
            };
            Log.Info($"SAP sin PI BAPI_PO_GETDETAIL1 request");
            Log.Info(request.ToXml());
            var response = agent.BAPI_PO_GETDETAIL1(request);
            Log.Info($"SAP sin PI BAPI_PO_GETDETAIL1 response");
            Log.Info(response.ToXml());
            return response;
        }

        /// <summary>
        /// MMSN-480: Devuelve verdadero si la entrada de servicio tiene factura
        /// </summary>
        /// <param name="nrosEntradasServicioFacturadas"></param>
        /// <param name="nroES"></param>
        /// <returns></returns>
        private bool EntradaServicioTieneFactura(List<string> nrosEntradasServicioFacturadas, string nroES)
        {
            bool entradaServicioFacturada = nrosEntradasServicioFacturadas.Any(x => x == nroES);

            return entradaServicioFacturada;
        }

        /// <summary>
        /// MMSN-480: Devuelve verdadero si la fecha actual está dentro del período SAP (Dias del mes actual y mes anterior completo)
        /// </summary>
        /// <param name="fechaInicial"></param>
        /// <param name="fechaActual"></param>
        /// <returns></returns>
        private bool DentroPeriodoSAP(DateTime fechaInicial, DateTime fechaActual)
        {
            int diferenciaEnMeses = ((fechaActual.Year - fechaInicial.Year) * 12) + fechaActual.Month - fechaInicial.Month;

            return diferenciaEnMeses < 2;
        }


        /// <summary>
        /// Obtiene detalle de todas las entradas de servicio de una OC
        /// deja solo las que son útiles para alguno de los items,
        /// y las guarda en un objeto
        /// </summary>
        private List<EntradaServicioDto> ObtenerTodasEntradasDeServicio_Nuevo(ObtenerOrdenDeCompraWebServiceMOA.BAPIESLLC[] pOSERVICES, ObtenerOrdenDeCompraWebServiceMOA.BAPIEKBE[] pOHISTORY)
        {
            List<EntradaServicioDto> EntradasDeServicioDelItem = new List<EntradaServicioDto>();
            List<ObtenerOrdenDeCompraWebServiceMOA.BAPIEKBE> entradasDeServicioPotenciales = pOHISTORY.Where(x => x.PROCESS_ID == "9" && x.HIST_TYPE == "D").ToList();

            List<string> ListaDeEntradasDeServicioFacturadas = pOHISTORY
                .Where(x => (x.HIST_TYPE == "Q" || x.HIST_TYPE == "R") && x.PROCESS_ID == "2")
                .Select(x => x.REF_DOC)
                .ToList();


            foreach (var entradaServicioCompleta in entradasDeServicioPotenciales)
            {
                string _nroES = entradaServicioCompleta.MAT_DOC;

                EntradaServicioDto entradaServicioSAP = new ObtenerEntradaDeServicioPorNumeroConsumerMOA().ObtenerEntradaServicio(_nroES);

                List<ItemEntradaServicioDto> _itemsDeEntradaServicio = entradaServicioSAP.Items;
                bool entradaServicioFacturada = EntradaServicioTieneFactura(ListaDeEntradasDeServicioFacturadas, _nroES);

                foreach (ItemEntradaServicioDto itemES in _itemsDeEntradaServicio)
                {
                    EntradaServicioDto entradaServicioDto = new EntradaServicioDto();
                    DateTime _fechaContabilizacion = SAPFormatter.GetDateTime(entradaServicioSAP.FechaContabilizacion);
                    bool entradaServicioDentroDePeriodoSAP = DentroPeriodoSAP(_fechaContabilizacion, DateTime.Now);

                    entradaServicioDto.Id = int.Parse(itemES.Id);
                    entradaServicioDto.itemNumero = itemES.ItemNumero;
                    entradaServicioDto.TextoBreve = itemES.Descripcion;
                    entradaServicioDto.Cantidad = itemES.Cantidad;
                    entradaServicioDto.ESS_PCKG_NO = itemES.PCKG_NO;
                    entradaServicioDto.ESS_LINE_NO = itemES.PLN_LINE;
                    entradaServicioDto.ESS_EXT_LINE = itemES.EXT_LINE;
                    entradaServicioDto.Fecha = entradaServicioSAP.Fecha; //MMSN-460 - Informacion de Cabecera p/ FE
                    entradaServicioDto.FechaDocumentoString = entradaServicioSAP.FechaDocumentoString;
                    entradaServicioDto.FechaContabilizacion = entradaServicioSAP.FechaContabilizacion;
                    entradaServicioDto.Referencia = entradaServicioSAP.Referencia;
                    entradaServicioDto.ImporteARPUSD = itemES.ImporteARPUSD;
                    entradaServicioDto.SePuedeBorrar = !entradaServicioFacturada && entradaServicioDentroDePeriodoSAP;

                    EntradasDeServicioDelItem.Add(entradaServicioDto);
                }
            }

            return EntradasDeServicioDelItem;
        }

        private List<EntradaServicioDto> ObtenerTodasEntradasDeServicio_NuevoSinPI(WS_GAQ_sin_PI_DIRECT_2012.BAPIESLLC[] pOSERVICES, WS_GAQ_sin_PI_DIRECT_2012.BAPIEKBE[] pOHISTORY)
        {
            List<EntradaServicioDto> EntradasDeServicioDelItem = new List<EntradaServicioDto>();
            List<WS_GAQ_sin_PI_DIRECT_2012.BAPIEKBE> entradasDeServicioPotenciales = pOHISTORY.Where(x => x.PROCESS_ID == "9" && x.HIST_TYPE == "D").ToList();

            List<string> ListaDeEntradasDeServicioFacturadas = pOHISTORY
                .Where(x => (x.HIST_TYPE == "Q" || x.HIST_TYPE == "R") && x.PROCESS_ID == "2")
                .Select(x => x.REF_DOC)
                .ToList();


            foreach (var entradaServicioCompleta in entradasDeServicioPotenciales)
            {
                string _nroES = entradaServicioCompleta.MAT_DOC;

                EntradaServicioDto entradaServicioSAP = new ObtenerEntradaDeServicioPorNumeroConsumerMOA().ObtenerEntradaServicio(_nroES);

                List<ItemEntradaServicioDto> _itemsDeEntradaServicio = entradaServicioSAP.Items;
                bool entradaServicioFacturada = EntradaServicioTieneFactura(ListaDeEntradasDeServicioFacturadas, _nroES);

                foreach (ItemEntradaServicioDto itemES in _itemsDeEntradaServicio)
                {
                    EntradaServicioDto entradaServicioDto = new EntradaServicioDto();
                    DateTime _fechaContabilizacion = SAPFormatter.GetDateTime(entradaServicioSAP.FechaContabilizacion);
                    bool entradaServicioDentroDePeriodoSAP = DentroPeriodoSAP(_fechaContabilizacion, DateTime.Now);

                    entradaServicioDto.Id = int.Parse(itemES.Id);
                    entradaServicioDto.itemNumero = itemES.ItemNumero;
                    entradaServicioDto.TextoBreve = itemES.Descripcion;
                    entradaServicioDto.Cantidad = itemES.Cantidad;
                    entradaServicioDto.ESS_PCKG_NO = itemES.PCKG_NO;
                    entradaServicioDto.ESS_LINE_NO = itemES.PLN_LINE;
                    entradaServicioDto.ESS_EXT_LINE = itemES.EXT_LINE;
                    entradaServicioDto.Fecha = entradaServicioSAP.Fecha; //MMSN-460 - Informacion de Cabecera p/ FE
                    entradaServicioDto.FechaDocumentoString = entradaServicioSAP.FechaDocumentoString;
                    entradaServicioDto.FechaContabilizacion = entradaServicioSAP.FechaContabilizacion;
                    entradaServicioDto.Referencia = entradaServicioSAP.Referencia;
                    entradaServicioDto.ImporteARPUSD = itemES.ImporteARPUSD;
                    entradaServicioDto.SePuedeBorrar = !entradaServicioFacturada && entradaServicioDentroDePeriodoSAP;

                    EntradasDeServicioDelItem.Add(entradaServicioDto);
                }
            }

            return EntradasDeServicioDelItem;
        }

        /// <summary>
        /// Obtiene las Entradas de Servicio de un Item
        /// De las entradas de servicios ya parseadas en un objeto
        /// </summary>
        private List<EntradaServicioDto> ObtenerEntradasDeServicioDelItem_Nuevo(ObtenerOrdenDeCompraWebServiceMOA.BAPIESLLC[] pOSERVICES, ObtenerOrdenDeCompraWebServiceMOA.BAPIEKBE[] pOHISTORY,
                                                                                string claveItemOC, string subClaveItemOC,
                                                                                List<EntradaServicioDto> ListaDeEntradasDeServicio)
        {
            List<EntradaServicioDto> EntradasDeServicioDelItem = new List<EntradaServicioDto>();
            //List<BAPIEKBE> entradasDeServicioPotenciales = pOHISTORY.Where(x => x.PROCESS_ID == "9" && x.HIST_TYPE == "D").ToList();
            List<string> ListaDeEntradasDeServicioFacturadas = pOHISTORY
                .Where(x => (x.HIST_TYPE == "Q" || x.HIST_TYPE == "R") && x.PROCESS_ID == "2")
                .Select(x => x.REF_DOC)
                .ToList();


            foreach (var entradaServicioCompleta in ListaDeEntradasDeServicio)
            {
                string _nroES = entradaServicioCompleta.Id.ToString();
                EntradaServicioDto entradaServicioDto = new EntradaServicioDto();
                bool entradaServicioFacturada = EntradaServicioTieneFactura(ListaDeEntradasDeServicioFacturadas, _nroES);

                if (entradaServicioCompleta.itemNumero == claveItemOC && entradaServicioCompleta.ESS_LINE_NO == subClaveItemOC)
                {
                    DateTime _fechaContabilizacion = SAPFormatter.GetDateTime(entradaServicioCompleta.FechaContabilizacion);
                    bool entradaServicioDentroDePeriodoSAP = DentroPeriodoSAP(_fechaContabilizacion, DateTime.Now);

                    entradaServicioDto.Id = entradaServicioCompleta.Id;
                    entradaServicioDto.itemNumero = entradaServicioCompleta.itemNumero;
                    entradaServicioDto.TextoBreve = entradaServicioCompleta.TextoBreve;
                    entradaServicioDto.Cantidad = entradaServicioCompleta.Cantidad;
                    entradaServicioDto.ESS_PCKG_NO = entradaServicioCompleta.itemNumero;
                    entradaServicioDto.ESS_LINE_NO = entradaServicioCompleta.ESS_LINE_NO;
                    entradaServicioDto.ESS_EXT_LINE = entradaServicioCompleta.ESS_EXT_LINE;

                    //MMSN-460 - Informacion de Cabecera p/ FE
                    entradaServicioDto.Fecha = entradaServicioCompleta.Fecha;
                    entradaServicioDto.FechaDocumentoString = entradaServicioCompleta.FechaDocumentoString;
                    entradaServicioDto.FechaContabilizacion = entradaServicioCompleta.FechaContabilizacion;
                    entradaServicioDto.Referencia = entradaServicioCompleta.Referencia;
                    entradaServicioDto.ImporteARPUSD = entradaServicioCompleta.ImporteARPUSD;
                    entradaServicioDto.SePuedeBorrar = !entradaServicioFacturada && entradaServicioDentroDePeriodoSAP;

                    EntradasDeServicioDelItem.Add(entradaServicioDto);
                }
            }

            return EntradasDeServicioDelItem;
        }

        private List<EntradaServicioDto> ObtenerEntradasDeServicioDelItem_NuevoSinPI(WS_GAQ_sin_PI_DIRECT_2012.BAPIESLLC[] pOSERVICES, WS_GAQ_sin_PI_DIRECT_2012.BAPIEKBE[] pOHISTORY,
                                                                        string claveItemOC, string subClaveItemOC,
                                                                        List<EntradaServicioDto> ListaDeEntradasDeServicio)
        {
            List<EntradaServicioDto> EntradasDeServicioDelItem = new List<EntradaServicioDto>();
            //List<BAPIEKBE> entradasDeServicioPotenciales = pOHISTORY.Where(x => x.PROCESS_ID == "9" && x.HIST_TYPE == "D").ToList();
            List<string> ListaDeEntradasDeServicioFacturadas = pOHISTORY
                .Where(x => (x.HIST_TYPE == "Q" || x.HIST_TYPE == "R") && x.PROCESS_ID == "2")
                .Select(x => x.REF_DOC)
                .ToList();


            foreach (var entradaServicioCompleta in ListaDeEntradasDeServicio)
            {
                string _nroES = entradaServicioCompleta.Id.ToString();
                EntradaServicioDto entradaServicioDto = new EntradaServicioDto();
                bool entradaServicioFacturada = EntradaServicioTieneFactura(ListaDeEntradasDeServicioFacturadas, _nroES);

                if (entradaServicioCompleta.itemNumero == claveItemOC && entradaServicioCompleta.ESS_LINE_NO == subClaveItemOC)
                {
                    DateTime _fechaContabilizacion = SAPFormatter.GetDateTime(entradaServicioCompleta.FechaContabilizacion);
                    bool entradaServicioDentroDePeriodoSAP = DentroPeriodoSAP(_fechaContabilizacion, DateTime.Now);

                    entradaServicioDto.Id = entradaServicioCompleta.Id;
                    entradaServicioDto.itemNumero = entradaServicioCompleta.itemNumero;
                    entradaServicioDto.TextoBreve = entradaServicioCompleta.TextoBreve;
                    entradaServicioDto.Cantidad = entradaServicioCompleta.Cantidad;
                    entradaServicioDto.ESS_PCKG_NO = entradaServicioCompleta.itemNumero;
                    entradaServicioDto.ESS_LINE_NO = entradaServicioCompleta.ESS_LINE_NO;
                    entradaServicioDto.ESS_EXT_LINE = entradaServicioCompleta.ESS_EXT_LINE;

                    //MMSN-460 - Informacion de Cabecera p/ FE
                    entradaServicioDto.Fecha = entradaServicioCompleta.Fecha;
                    entradaServicioDto.FechaDocumentoString = entradaServicioCompleta.FechaDocumentoString;
                    entradaServicioDto.FechaContabilizacion = entradaServicioCompleta.FechaContabilizacion;
                    entradaServicioDto.Referencia = entradaServicioCompleta.Referencia;
                    entradaServicioDto.ImporteARPUSD = entradaServicioCompleta.ImporteARPUSD;
                    entradaServicioDto.SePuedeBorrar = !entradaServicioFacturada && entradaServicioDentroDePeriodoSAP;

                    EntradasDeServicioDelItem.Add(entradaServicioDto);
                }
            }

            return EntradasDeServicioDelItem;
        }
    }

    public class ResultBAPI_PO_GETDETAIL1
    {
        public ObtenerOrdenDeCompraWebServiceMOA.BAPIEIKP Result { get; }
        public ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOHEADER POHEADER { get; }
        public ObtenerOrdenDeCompraWebServiceMOA.BAPIRET2[] RETURN { get; }
        public ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOITEM[] POITEM { get; }
        public ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOTEXTHEADER[] POTEXTHEADER { get; }
        public ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOTEXT[] POTEXTITEM { get; }
        public ObtenerOrdenDeCompraWebServiceMOA.BAPIESLLC[] POSERVICES { get; }
        public ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOSCHEDULE[] POSCHEDULE { get; }
        public ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOADDRDELIVERY[] POADDRDELIVERY { get; }
        public ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOCOND[] POCOND { get; }
        public ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOACCOUNT[] POACCOUNT { get; set; }
        public ObtenerOrdenDeCompraWebServiceMOA.BAPIESKLC[] POSRVACCESSVALUES { get; set; }

        public ResultBAPI_PO_GETDETAIL1(ObtenerOrdenDeCompraWebServiceMOA.BAPIEIKP result, ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOHEADER pOHEADER, ObtenerOrdenDeCompraWebServiceMOA.BAPIRET2[] rETURN,
                                        ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOITEM[] pOITEM, ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOTEXTHEADER[] pOTEXTHEADER, ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOTEXT[] pOTEXTITEM,
                                        ObtenerOrdenDeCompraWebServiceMOA.BAPIESLLC[] pOSERVICES, ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOSCHEDULE[] pOSCHEDULE, ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOADDRDELIVERY[] pOADDRDELIVERY,
                                        ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOCOND[] pOCOND, ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOACCOUNT[] pOACCOUNT, ObtenerOrdenDeCompraWebServiceMOA.BAPIESKLC[] pOSRVACCESSVALUES)
        {
            Result = result;
            POHEADER = pOHEADER;
            RETURN = rETURN;
            POITEM = pOITEM;
            POTEXTHEADER = pOTEXTHEADER;
            POTEXTITEM = pOTEXTITEM;
            POSERVICES = pOSERVICES;
            POSCHEDULE = pOSCHEDULE;
            POADDRDELIVERY = pOADDRDELIVERY;
            POCOND = pOCOND;
            POACCOUNT = pOACCOUNT;
            POSRVACCESSVALUES = pOSRVACCESSVALUES;
        }

        public override bool Equals(object obj)
        {
            return obj is ResultBAPI_PO_GETDETAIL1 other &&
                   EqualityComparer<ObtenerOrdenDeCompraWebServiceMOA.BAPIEIKP>.Default.Equals(Result, other.Result) &&
                   EqualityComparer<ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOHEADER>.Default.Equals(POHEADER, other.POHEADER) &&
                   EqualityComparer<ObtenerOrdenDeCompraWebServiceMOA.BAPIRET2[]>.Default.Equals(RETURN, other.RETURN) &&
                   EqualityComparer<ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOITEM[]>.Default.Equals(POITEM, other.POITEM) &&
                   EqualityComparer<ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOTEXTHEADER[]>.Default.Equals(POTEXTHEADER, other.POTEXTHEADER) &&
                   EqualityComparer<ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOTEXT[]>.Default.Equals(POTEXTITEM, other.POTEXTITEM) &&
                   EqualityComparer<ObtenerOrdenDeCompraWebServiceMOA.BAPIESLLC[]>.Default.Equals(POSERVICES, other.POSERVICES) &&
                   EqualityComparer<ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOSCHEDULE[]>.Default.Equals(POSCHEDULE, other.POSCHEDULE) &&
                   EqualityComparer<ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOADDRDELIVERY[]>.Default.Equals(POADDRDELIVERY, other.POADDRDELIVERY);
        }

        public override int GetHashCode()
        {
            int hashCode = 996742197;
            hashCode = hashCode * -1521134295 + EqualityComparer<ObtenerOrdenDeCompraWebServiceMOA.BAPIEIKP>.Default.GetHashCode(Result);
            hashCode = hashCode * -1521134295 + EqualityComparer<ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOHEADER>.Default.GetHashCode(POHEADER);
            hashCode = hashCode * -1521134295 + EqualityComparer<ObtenerOrdenDeCompraWebServiceMOA.BAPIRET2[]>.Default.GetHashCode(RETURN);
            hashCode = hashCode * -1521134295 + EqualityComparer<ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOITEM[]>.Default.GetHashCode(POITEM);
            hashCode = hashCode * -1521134295 + EqualityComparer<ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOTEXTHEADER[]>.Default.GetHashCode(POTEXTHEADER);
            hashCode = hashCode * -1521134295 + EqualityComparer<ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOTEXT[]>.Default.GetHashCode(POTEXTITEM);
            hashCode = hashCode * -1521134295 + EqualityComparer<ObtenerOrdenDeCompraWebServiceMOA.BAPIESLLC[]>.Default.GetHashCode(POSERVICES);
            hashCode = hashCode * -1521134295 + EqualityComparer<ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOSCHEDULE[]>.Default.GetHashCode(POSCHEDULE);
            hashCode = hashCode * -1521134295 + EqualityComparer<ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOADDRDELIVERY[]>.Default.GetHashCode(POADDRDELIVERY);
            return hashCode;
        }
    }

    public class ResultBAPI_PO_GETDETAIL1SinPI
    {
        public WS_GAQ_sin_PI_DIRECT_2012.BAPIEIKP Result { get; }
        public WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOHEADER POHEADER { get; }
        public WS_GAQ_sin_PI_DIRECT_2012.BAPIRET2[] RETURN { get; }
        public WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOITEM[] POITEM { get; }
        public WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOTEXTHEADER[] POTEXTHEADER { get; }
        public WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOTEXT[] POTEXTITEM { get; }
        public WS_GAQ_sin_PI_DIRECT_2012.BAPIESLLC[] POSERVICES { get; }
        public WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOSCHEDULE[] POSCHEDULE { get; }
        public WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOADDRDELIVERY[] POADDRDELIVERY { get; }
        public WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOCOND[] POCOND { get; }
        public WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOACCOUNT[] POACCOUNT { get; set; }
        public WS_GAQ_sin_PI_DIRECT_2012.BAPIESKLC[] POSRVACCESSVALUES { get; set; }

        public ResultBAPI_PO_GETDETAIL1SinPI(WS_GAQ_sin_PI_DIRECT_2012.BAPI_PO_GETDETAIL1Response response)
        {
            Result = response.POEXPIMPHEADER;
            POHEADER = response.POHEADER;
            RETURN = response.RETURN;
            POITEM = response.POITEM;
            POTEXTHEADER = response.POTEXTHEADER;
            POTEXTITEM = response.POTEXTITEM;
            POSERVICES = response.POSERVICES;
            POSCHEDULE = response.POSCHEDULE;
            POADDRDELIVERY = response.POADDRDELIVERY;
            POCOND = response.POCOND;
            POACCOUNT = response.POACCOUNT;
            POSRVACCESSVALUES = response.POSRVACCESSVALUES;
        }

        public override bool Equals(object obj)
        {
            return obj is ResultBAPI_PO_GETDETAIL1SinPI other &&
                   EqualityComparer<WS_GAQ_sin_PI_DIRECT_2012.BAPIEIKP>.Default.Equals(Result, other.Result) &&
                   EqualityComparer<WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOHEADER>.Default.Equals(POHEADER, other.POHEADER) &&
                   EqualityComparer<WS_GAQ_sin_PI_DIRECT_2012.BAPIRET2[]>.Default.Equals(RETURN, other.RETURN) &&
                   EqualityComparer<WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOITEM[]>.Default.Equals(POITEM, other.POITEM) &&
                   EqualityComparer<WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOTEXTHEADER[]>.Default.Equals(POTEXTHEADER, other.POTEXTHEADER) &&
                   EqualityComparer<WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOTEXT[]>.Default.Equals(POTEXTITEM, other.POTEXTITEM) &&
                   EqualityComparer<WS_GAQ_sin_PI_DIRECT_2012.BAPIESLLC[]>.Default.Equals(POSERVICES, other.POSERVICES) &&
                   EqualityComparer<WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOSCHEDULE[]>.Default.Equals(POSCHEDULE, other.POSCHEDULE) &&
                   EqualityComparer<WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOADDRDELIVERY[]>.Default.Equals(POADDRDELIVERY, other.POADDRDELIVERY);
        }

        public override int GetHashCode()
        {
            int hashCode = 996742197;
            hashCode = hashCode * -1521134295 + EqualityComparer<WS_GAQ_sin_PI_DIRECT_2012.BAPIEIKP>.Default.GetHashCode(Result);
            hashCode = hashCode * -1521134295 + EqualityComparer<WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOHEADER>.Default.GetHashCode(POHEADER);
            hashCode = hashCode * -1521134295 + EqualityComparer<WS_GAQ_sin_PI_DIRECT_2012.BAPIRET2[]>.Default.GetHashCode(RETURN);
            hashCode = hashCode * -1521134295 + EqualityComparer<WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOITEM[]>.Default.GetHashCode(POITEM);
            hashCode = hashCode * -1521134295 + EqualityComparer<WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOTEXTHEADER[]>.Default.GetHashCode(POTEXTHEADER);
            hashCode = hashCode * -1521134295 + EqualityComparer<WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOTEXT[]>.Default.GetHashCode(POTEXTITEM);
            hashCode = hashCode * -1521134295 + EqualityComparer<WS_GAQ_sin_PI_DIRECT_2012.BAPIESLLC[]>.Default.GetHashCode(POSERVICES);
            hashCode = hashCode * -1521134295 + EqualityComparer<WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOSCHEDULE[]>.Default.GetHashCode(POSCHEDULE);
            hashCode = hashCode * -1521134295 + EqualityComparer<WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOADDRDELIVERY[]>.Default.GetHashCode(POADDRDELIVERY);
            return hashCode;
        }
    }
}
