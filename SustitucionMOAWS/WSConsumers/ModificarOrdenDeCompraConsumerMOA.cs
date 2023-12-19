
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Entities;
using SustitucionMOAWS.CredentialService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using SustitucionMOARepositorio.Extensiones;
using SustitucionMOAWS.ModificarOCWebServiceMOA;
using SustitucionMOAWS.Interfaces;
using SustitucionMOARepositorio;

namespace SustitucionMOAWS.WSConsumers
{
    public class ModificarOrdenDeCompraConsumerMOA : IModificarOrdenDeCompraConsumerMOA
    {
        private readonly SI_MMRFC_MODIFICAR_OCClient service;
        private readonly string rutaArchivosXmls = ConfigurationManager.AppSettings["RutaArchivosCompras"];
        private readonly ObtenerOrdenDeCompraConsumerMOA obtenerOrdenDeCompraconsumerMOA;
        private readonly IRepositorio repositorio;

        public ModificarOrdenDeCompraConsumerMOA(IRepositorio repositorio)
        {
            var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=SI_MMRFC_MODIFICAR_OC&amp;interfaceNamespace=urn%3AOPERACIONES";
            service = new SI_MMRFC_MODIFICAR_OCClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
            obtenerOrdenDeCompraconsumerMOA = new ObtenerOrdenDeCompraConsumerMOA(repositorio);
            this.repositorio = repositorio; 
        }

        public CrearPedidoConsumerMOAResponse Request(Adjudicacion adjudicacion)
        {
            ModificarPedidoSAP modificarPedidoSAP = ConvertirAdjudicacion(adjudicacion);

            var serxml = new System.Xml.Serialization.XmlSerializer(modificarPedidoSAP.GetType());
            var ms = new MemoryStream();
            serxml.Serialize(ms, modificarPedidoSAP);
            string xml = Encoding.UTF8.GetString(ms.ToArray());

            var fecha = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");

            var nombreArchivoLlamada = string.Concat(adjudicacion.Solp_Id, " - ", fecha, " - modificar pedido.xml");

            var rutaArchivoLlamada = Path.Combine(rutaArchivosXmls, "XMLS", nombreArchivoLlamada);

            FileInfo fileCrear = new FileInfo(rutaArchivoLlamada);
            fileCrear.Directory.Create();
            File.WriteAllText(fileCrear.FullName, xml);

            BAPIRET2[] result = EditarPedidoRequest(modificarPedidoSAP);


            var respuesta = new CrearPedidoConsumerMOAResponse();

            respuesta.NumeroPedido = adjudicacion.Solp.NroOrdenDeCompraAdicional;
            respuesta.Resultado = "";

            respuesta.Errores = new List<CrearPedidoConsumerMOAError>();

            foreach (var errorSAP in result.Where(e => e.TYPE == "E"))
            {
                var error = new CrearPedidoConsumerMOAError
                {
                    Codigo = errorSAP.FIELD,
                    Mensaje = errorSAP.MESSAGE,
                    Tipo = errorSAP.TYPE
                };

                respuesta.Errores.Add(error);
            }


            serxml = new System.Xml.Serialization.XmlSerializer(result.GetType());
            ms = new MemoryStream();
            serxml.Serialize(ms, result);
            xml = Encoding.UTF8.GetString(ms.ToArray());
            using (StreamWriter writer = File.AppendText(rutaArchivoLlamada))
            {
                writer.WriteLine(xml);
            }


            return respuesta;
        }

        private BAPIRET2[] EditarPedidoRequest(ModificarPedidoSAP modificarPedidoSAP)
        {
            BAPIMEDCM_ALLVERSIONS[] ALLVERSIONS = modificarPedidoSAP.ALLVERSIONS?.ToArray();
            BAPIPAREX[] EXTENSIONIN = modificarPedidoSAP.EXTENSIONIN?.ToArray();
            BAPIPAREX[] EXTENSIONOUT = modificarPedidoSAP.EXTENSIONOUT?.ToArray();
            BAPI_INVOICE_PLAN_HEADER[] INVPLANHEADER = modificarPedidoSAP.INVPLANHEADER?.ToArray();
            BAPI_INVOICE_PLAN_HEADERX[] INVPLANHEADERX = modificarPedidoSAP.INVPLANHEADERX?.ToArray();
            BAPI_INVOICE_PLAN_ITEM[] INVPLANITEM = modificarPedidoSAP.INVPLANITEM?.ToArray();
            BAPI_INVOICE_PLAN_ITEMX[] INVPLANITEMX = modificarPedidoSAP.INVPLANITEMX?.ToArray();
            BAPIMEPOACCOUNT[] POACCOUNT = modificarPedidoSAP.POACCOUNT?.ToArray();
            BAPIMEPOACCOUNTPROFITSEGMENT[] POACCOUNTPROFITSEGMENT = modificarPedidoSAP.POACCOUNTPROFITSEGMENT?.ToArray();
            BAPIMEPOACCOUNTX[] POACCOUNTX = modificarPedidoSAP.POACCOUNTX?.ToArray();
            BAPIMEPOADDRDELIVERY[] POADDRDELIVERY = modificarPedidoSAP.POADDRDELIVERY?.ToArray();
            BAPIMEPOCOMPONENT[] POCOMPONENTS = modificarPedidoSAP.POCOMPONENTS?.ToArray();
            BAPIMEPOCOMPONENTX[] POCOMPONENTSX = modificarPedidoSAP.POCOMPONENTSX?.ToArray();
            BAPIMEPOCOND[] POCOND = modificarPedidoSAP.POCOND?.ToArray();
            BAPIMEPOCONDHEADER[] POCONDHEADER = modificarPedidoSAP.POCONDHEADER?.ToArray();
            BAPIMEPOCONDHEADERX[] POCONDHEADERX = modificarPedidoSAP.POCONDHEADERX?.ToArray();
            BAPIMEPOCONDX[] POCONDX = modificarPedidoSAP.POCONDX?.ToArray();
            BAPIEKES[] POCONFIRMATION = modificarPedidoSAP.POCONFIRMATION?.ToArray();
            BAPIESUCC[] POCONTRACTLIMITS = modificarPedidoSAP.POCONTRACTLIMITS?.ToArray();
            BAPIEIPO[] POEXPIMPITEM = modificarPedidoSAP.POEXPIMPITEM?.ToArray();
            BAPIEIPOX[] POEXPIMPITEMX = modificarPedidoSAP.POEXPIMPITEMX?.ToArray();
            BAPIEKBE[] POHISTORY = modificarPedidoSAP.POHISTORY?.ToArray();
            BAPIEKBE_MA[] POHISTORY_MA = modificarPedidoSAP.POHISTORY_MA?.ToArray();
            BAPIEKBES[] POHISTORY_TOTALS = modificarPedidoSAP.POHISTORY_TOTALS?.ToArray();
            BAPIMEPOITEM[] POITEM = modificarPedidoSAP.POITEM?.ToArray();
            BAPIMEPOITEMX[] POITEMX = modificarPedidoSAP.POITEMX?.ToArray();
            BAPIESUHC[] POLIMITS = modificarPedidoSAP.POLIMITS?.ToArray();
            BAPIEKKOP[] POPARTNER = modificarPedidoSAP.POPARTNER?.ToArray();
            BAPIMEPOSCHEDULE[] POSCHEDULE = modificarPedidoSAP.POSCHEDULE?.ToArray();
            BAPIMEPOSCHEDULX[] POSCHEDULEX = modificarPedidoSAP.POSCHEDULEX?.ToArray();
            BAPIESLLC[] POSERVICES = modificarPedidoSAP.POSERVICES?.ToArray();
            BAPIESLLTX[] POSERVICESTEXT = modificarPedidoSAP.POSERVICESTEXT?.ToArray();
            BAPIITEMSHIP[] POSHIPPING = modificarPedidoSAP.POSHIPPING?.ToArray();
            BAPIMEPOSHIPPEXP[] POSHIPPINGEXP = modificarPedidoSAP.POSHIPPINGEXP?.ToArray();
            BAPIITEMSHIPX[] POSHIPPINGX = modificarPedidoSAP.POSHIPPINGX?.ToArray();
            BAPIESKLC[] POSRVACCESSVALUES = modificarPedidoSAP.POSRVACCESSVALUES?.ToArray();
            BAPIMEPOTEXTHEADER[] POTEXTHEADER = modificarPedidoSAP.POTEXTHEADER?.ToArray();
            BAPIMEPOTEXT[] POTEXTITEM = modificarPedidoSAP.POTEXTITEM?.ToArray();
            BAPIRET2[] RETURN = modificarPedidoSAP.RETURN?.ToArray();
            BAPIMEPOSERIALNO[] SERIALNUMBER = modificarPedidoSAP.SERIALNUMBER?.ToArray();
            BAPIMEPOSERIALNOX[] SERIALNUMBERX = modificarPedidoSAP.SERIALNUMBERX?.ToArray();

            _NFM_BAPIDOCITM[] NFMETALLITMS = modificarPedidoSAP.NFMETALLITMS?.ToArray();
            BAPIMEPOHEADER result = service.SI_MMRFC_MODIFICAR_OC(
                modificarPedidoSAP.MEMORY_COMPLETE,
                modificarPedidoSAP.MEMORY_UNCOMPLETE,
                modificarPedidoSAP.NO_AUTHORITY,
                modificarPedidoSAP.NO_MESSAGE_REQ,
                modificarPedidoSAP.NO_MESSAGING,
                modificarPedidoSAP.NO_PRICE_FROM_PO,
                modificarPedidoSAP.PARK_COMPLETE,
                modificarPedidoSAP.PARK_UNCOMPLETE,
                modificarPedidoSAP.POADDRVENDOR,
                modificarPedidoSAP.POEXPIMPHEADER,
                modificarPedidoSAP.POEXPIMPHEADERX,
                modificarPedidoSAP.POHEADER,
                modificarPedidoSAP.POHEADERX,
                modificarPedidoSAP.PURCHASEORDER,
                modificarPedidoSAP.TESTRUN,
                 modificarPedidoSAP.VERSIONS,
                 ref ALLVERSIONS,
                 ref EXTENSIONIN,
                 ref EXTENSIONOUT,
                 ref INVPLANHEADER,
                 ref INVPLANHEADERX,
                 ref INVPLANITEM,
                 ref INVPLANITEMX,
                 ref NFMETALLITMS,
                 ref POACCOUNT,
                 ref POACCOUNTPROFITSEGMENT,
                 ref POACCOUNTX,
                 ref POADDRDELIVERY,
                 ref POCOMPONENTS,
                 ref POCOMPONENTSX,
                 ref POCOND,
                 ref POCONDHEADER,
                 ref POCONDHEADERX,
                 ref POCONDX,
                 ref POCONFIRMATION,
                 ref POCONTRACTLIMITS,
                 ref POEXPIMPITEM,
                 ref POEXPIMPITEMX,
                 ref POHISTORY,
                 ref POHISTORY_MA,
                 ref POHISTORY_TOTALS,
                 ref POITEM,
                 ref POITEMX,
                 ref POLIMITS,
                 ref POPARTNER,
                 ref POSCHEDULE,
                 ref POSCHEDULEX,
                 ref POSERVICES,
                 ref POSERVICESTEXT,
                 ref POSHIPPING,
                 ref POSHIPPINGEXP,
                 ref POSHIPPINGX,
                 ref POSRVACCESSVALUES,
                 ref POTEXTHEADER,
                 ref POTEXTITEM,
                 ref RETURN,
                 ref SERIALNUMBER,
                 ref SERIALNUMBERX,
                 out BAPIEIKP EXPPOEXPIMPHEADER
            );

            return RETURN;
        }


        private ModificarPedidoSAP ConvertirAdjudicacion(Adjudicacion adjudicacion)
        {
            //TODO: Crear OC ConvertirSOLP - fields hardcodeados o para revisar
            ///DOC_TYPE  ok por ahora. Clase de documento de compras / Estrategia de liberacion hardcore ZPE1 

            ///STREET y STREET_NO ok. no tenemos el campo separado mandamos todo en street            
            ///SERIAL_NO/serialNumber siempre 1 por que se imputa todo a lo mismo sino son imputaciones multiples, en ese caso analizar como se envia.


            var proveedorCodigoDeLaAdjudicacion = adjudicacion.Posiciones.First().CotizacionPosicion.Cotizacion.PeticionDeOfertaUsuario.Usuario.ObtenerCodigoProveedor();
            var usuarioCreadorAdjudicacion = adjudicacion.Usuario.UsuarioSap;
            var usuarioOrganizacionDeCompra = adjudicacion.Usuario.OrganizacionDeCompra;
            var solp = adjudicacion.Solp;
            ModificarPedidoSAP modificarPedidoSAP = new ModificarPedidoSAP();
            modificarPedidoSAP.PURCHASEORDER = adjudicacion.Cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta.Solp.NroOrdenDeCompraAdicional;// "4123001763";

            int numeroPosicion = 0;
            string preqItem = "";
            string poItem = "";
            string numeroDeImputacion = "";
            var PCKG_NO = 1000;
            var numeroDePaquete = 1;

            var ocSAP = obtenerOrdenDeCompraconsumerMOA.ObtenerOrdenDeCompra(modificarPedidoSAP.PURCHASEORDER);
            int nroItemPO = ocSAP.Posiciones.Max(a => a.NumeroItemOC);

            bool esPosicionDeMateriales = solp.Posiciones.First().TipoPosicion.Codigo == "MATERIALES";

            //aca el metodo solo usa las posiciones seleccionadas por el comprador
            var posIds = adjudicacion.Posiciones.Select(x => x.CotizacionPosicion.PeticionDeOfertaSolpPosicion.SolpPosicion_Id).ToList();

            //cabecera del pedido
            //Nombre: ZBAPIMEPOHEADER Denominación:	Cabecera del Pedido de Compras
            //var cabeceraDelPedido = new BAPIMEPOHEADER();
            //var CURRENCY = adjudicacion.Posiciones.Where(a => posIds.Contains( a.CotizacionPosicion.PeticionDeOfertaSolpPosicion.SolpPosicion_Id)).First().CotizacionPosicion.Moneda.Codigo;
            //var PUR_GROUP = solp.Posiciones.Where(a => posIds.Contains(a.Id)).First().GrupoCompras.CodigoSap.ToString();
            //cabeceraDelPedido.COMP_CODE = "MOA"; //COMP_CODE BUKRS   Sociedad
            //cabeceraDelPedido.DOC_TYPE = "ZPE1";//solp.ClaseDocumento.CodigoSap; //DOC_TYPE    ESART Clase de documento de compras
            //cabeceraDelPedido.VENDOR = proveedorCodigoDeLaAdjudicacion; //VENDOR ELIFN   Número de cuenta del proveedor
            //cabeceraDelPedido.PURCH_ORG = usuarioOrganizacionDeCompra; //PURCH_ORG EKORG   Organización de compras
            //cabeceraDelPedido.PUR_GROUP = PUR_GROUP;  //PUR_GROUP   BKGRP Grupo de compras
            //cabeceraDelPedido.CURRENCY = CURRENCY; //CURRENCY WAERS   Clave de moneda
            //cabeceraDelPedido.CREATED_BY = usuarioCreadorAdjudicacion;//CREATED_BY ERNAM   Nombre del responsable que ha añadido el objeto
            //cabeceraDelPedido.DOC_DATE = SAPFormatter.PrepararFecha(DateTime.Now); //DOC_DATE    EBDAT Fecha del documento de compras

            //cabeceraDelPedido.PO_NUMBER = "4123001763";//solp.ocadicional; //PO_NUMBER   EBELN Número del documento de compras
            //cabeceraDelPedido.DELETE_IND = "";//DELETE_IND ELOEK   Indicador de borrado en el documento de compras
            //cabeceraDelPedido.STATUS = ""; //STATUS ESTAK   Status del documento de compras
            //cabeceraDelPedido.CREAT_DATE = ""; //SAPFormatter.PrepararFecha(solp.FechaCreacion); //CREAT_DATE  ERDAT Fecha de creación del registro
            //cabeceraDelPedido.PMNTTRMS = ""; //"BASE"; //PMNTTRMS    DZTERM Clave de condiciones de pago
            //                                 //cabeceraDelPedido.EXCH_RATE = 0; //EXCH_RATE   WKURS Tipo de cambio de moneda
            //cabeceraDelPedido.EX_RATE_FX = ""; //EX_RATE_FX KUFIX   Indicador tipo de cambio fijo


            //modificarPedidoSAP.POHEADER = cabeceraDelPedido;
            //modificarPedidoSAP.POHEADERX = new BAPIMEPOHEADERX
            //{
            //    PO_NUMBER = "",
            //    COMP_CODE = "X",
            //    DOC_TYPE = "X",
            //    DELETE_IND = "",
            //    STATUS = "",
            //    CREAT_DATE = "",
            //    CREATED_BY = "X",
            //    VENDOR = "X",
            //    PMNTTRMS = "",
            //    PURCH_ORG = "X",
            //    PUR_GROUP = "X",
            //    CURRENCY = "X",
            //    EXCH_RATE = "",
            //    EX_RATE_FX = "",
            //    DOC_DATE = "X"

            //};


            foreach (var posicion in solp.Posiciones.Where(a => posIds.Contains(a.Id)).OrderBy(x => x.Id))
            {
                nroItemPO += 1;
                var adjudicacionPosicion = adjudicacion.Posiciones.Where(a => a.CotizacionPosicion.PeticionDeOfertaSolpPosicion.SolpPosicion_Id == posicion.Id).Single();

                numeroPosicion = posicion.Indice ?? 0;
                numeroDePaquete = posicion.Indice ?? 0;
                preqItem = $"{numeroPosicion:00000}";
                numeroDeImputacion = "01";// SERIAL_NO por ahora siempre 01 por que no hay imputaciones multiples
                poItem = $"{nroItemPO:00000}";



                //Nombre: ZBAPIMEPOITEM Denominación:	Posición de PEDIDOS
                var IM_POITEM = new BAPIMEPOITEM();

                IM_POITEM.PO_ITEM = poItem;
                IM_POITEM.SHORT_TEXT = posicion.Tarea;
                IM_POITEM.PLANT = posicion.Centro.CodigoSap.ToString();
                IM_POITEM.MATL_GROUP = posicion.GrupoArticulo?.CodigoSap?.ToString() ?? "";
                //IM_POITEM.MATL_GROUP = posicion.GrupoArticulo.CodigoSap.ToString();
                IM_POITEM.MATERIAL = esPosicionDeMateriales ? posicion.MaterialSolp?.CodigoSap.ToString() : "";
                IM_POITEM.STGE_LOC = posicion.Almacen.CodigoSap.ToString();
                IM_POITEM.ITEM_CAT = posicion.TipoPosicion.Codigo.ToLower() == "servicio" ? "9" : "0";//ITEM_CAT PSTYP   Tipo de posición del documento de compras
                IM_POITEM.TRACKINGNO = posicion.NroNecesidad;
                IM_POITEM.INFO_REC = "";
                IM_POITEM.QUANTITY = esPosicionDeMateriales ? adjudicacionPosicion.Cantidad : 0;
                IM_POITEM.QUANTITYSpecified = esPosicionDeMateriales ? true : false;
                IM_POITEM.PO_UNIT = esPosicionDeMateriales ? adjudicacionPosicion.CotizacionPosicion.UnidadDeMedida.Descripcion : "001";
                IM_POITEM.NET_PRICE = esPosicionDeMateriales ? (decimal)adjudicacionPosicion.CotizacionPosicion.Precio : CalcularPrecioBrutoServicio(posicion, adjudicacionPosicion);
                IM_POITEM.NET_PRICESpecified = true;
                IM_POITEM.PRICE_UNIT = 1;
                IM_POITEM.PRICE_UNITSpecified = true;
                IM_POITEM.GR_PR_TIME = 0;
                //IM_POITEM.GR_PR_TIMESpecified = true; 
                IM_POITEM.DELETE_IND = "";
                IM_POITEM.TAX_CODE = "";
                IM_POITEM.VAL_TYPE = "";
                IM_POITEM.NO_MORE_GR = "";
                IM_POITEM.FINAL_INV = "";



                switch (posicion.TipoImputacion?.Codigo.ToLower())
                {
                    case "centrodecosto":
                        IM_POITEM.ACCTASSCAT = "K";
                        break;
                    case "ordendeot":
                        IM_POITEM.ACCTASSCAT = "F";
                        break;
                    case "ordendeinversion":
                        IM_POITEM.ACCTASSCAT = "F";
                        break;
                    case "siniestrobeneficio":
                        IM_POITEM.ACCTASSCAT = "Y";
                        break;
                }

                IM_POITEM.DISTRIB = "";
                IM_POITEM.PART_INV = "";
                IM_POITEM.GR_IND = "";
                IM_POITEM.GR_NON_VAL = "";
                IM_POITEM.IR_IND = "";
                IM_POITEM.FREE_ITEM = "";
                IM_POITEM.GR_BASEDIV = "";
                IM_POITEM.ACKN_REQD = "";
                IM_POITEM.ACKNOWL_NO = "";
                IM_POITEM.AGREEMENT = "";
                IM_POITEM.AGMT_ITEM = "";
                IM_POITEM.RFQ_NO = "";
                IM_POITEM.RFQ_ITEM = "";
                IM_POITEM.PREQ_NO = solp.NroSolp;
                IM_POITEM.PREQ_ITEM = preqItem;
                IM_POITEM.PCKG_NO = esPosicionDeMateriales ? "" : $"{numeroDePaquete:0000000000}";

                modificarPedidoSAP.POITEM.Add(IM_POITEM);

                modificarPedidoSAP.POITEMX.Add(new BAPIMEPOITEMX
                {
                    PO_ITEM = poItem,
                    DELETE_IND = "",
                    SHORT_TEXT = "X",
                    MATERIAL = "X",
                    PLANT = "X",
                    STGE_LOC = "X",
                    TRACKINGNO = string.IsNullOrEmpty(posicion.NroNecesidad) ? "" : "X",
                    MATL_GROUP = "X",
                    INFO_REC = "",
                    QUANTITY = ((decimal)IM_POITEM.QUANTITY == 0) ? "" : "X",
                    PO_UNIT = "X",
                    NET_PRICE = "X",
                    PRICE_UNIT = "X",
                    GR_PR_TIME = "",
                    TAX_CODE = "",
                    VAL_TYPE = "",
                    NO_MORE_GR = "",
                    FINAL_INV = "",
                    ITEM_CAT = "X",
                    ACCTASSCAT = (IM_POITEM.ACCTASSCAT != null) ? "X" : "",
                    DISTRIB = "",
                    PART_INV = "",
                    GR_IND = "",
                    GR_NON_VAL = "",
                    IR_IND = "",
                    FREE_ITEM = "",
                    GR_BASEDIV = "",
                    ACKN_REQD = "",
                    ACKNOWL_NO = "",
                    AGREEMENT = "",
                    AGMT_ITEM = "",
                    RFQ_NO = "",
                    RFQ_ITEM = "",
                    PREQ_NO = "X",
                    PREQ_ITEM = "X",
                    PCKG_NO = "X"
                });

                modificarPedidoSAP.POCOND.Add(new BAPIMEPOCOND
                {
                    ITM_NUMBER = poItem,  //el número de ítem al que corresponda la condición
                    COND_TYPE = "ZP01",// siempre va el mismo dato
                    COND_VALUE = IM_POITEM.NET_PRICE, //el importe de la condición
                    COND_VALUESpecified = true,
                    CURRENCY = adjudicacionPosicion.CotizacionPosicion.Moneda.Codigo,//moneda de la adjudicacion
                    CHANGE_ID = "U",// siempra va el mismo valor

                });
                modificarPedidoSAP.POCONDX.Add(new BAPIMEPOCONDX
                {
                    ITM_NUMBER = poItem,
                    COND_TYPE = "X",
                    COND_VALUE = "X",
                    CURRENCY = "X",
                    CHANGE_ID = "X",
                });

                //Nombre: ZBAPIMEPOACCOUNT IM_POACCOUNT Denominación:	Imputación


                var imputacion = new BAPIMEPOACCOUNT();
                imputacion.PO_ITEM = poItem;
                imputacion.SERIAL_NO = numeroDeImputacion;
                imputacion.GL_ACCOUNT = ObtenerCuentaMayor(esPosicionDeMateriales, posicion);
                imputacion.QUANTITY = esPosicionDeMateriales ? adjudicacionPosicion.Cantidad : 0;
                imputacion.QUANTITYSpecified = imputacion.QUANTITY > 0;
                imputacion.BUS_AREA = "GENE";
                imputacion.CO_AREA = "MOA";
                imputacion.COSTCENTER = ObtenerImputacion(esPosicionDeMateriales, posicion, new List<string> { "centrodecosto" });
                imputacion.ORDERID = ObtenerImputacion(esPosicionDeMateriales, posicion, new List<string> { "ordendeot", "ordendeinversion" });
                imputacion.PROFIT_CTR = ObtenerImputacion(esPosicionDeMateriales, posicion, new List<string> { "siniestrobeneficio" });
                imputacion.SUB_NUMBER = "";
                imputacion.ASSET_NO = "";
                imputacion.COSTOBJECT = "";
                imputacion.DELETE_IND = "";
                modificarPedidoSAP.POACCOUNT.Add(imputacion);

                modificarPedidoSAP.POACCOUNTX.Add(new BAPIMEPOACCOUNTX
                {
                    PO_ITEM = poItem,
                    SERIAL_NO = numeroDeImputacion,
                    DELETE_IND = "",
                    QUANTITY = "X",
                    GL_ACCOUNT = "X",
                    BUS_AREA = "X",
                    ASSET_NO = "",
                    SUB_NUMBER = "",
                    CO_AREA = "X",
                    COSTOBJECT = "",
                    COSTCENTER = (posicion.TipoImputacion?.Codigo.ToLower() == "centrodecosto") ? "X" : "",
                    ORDERID = (posicion.TipoImputacion?.Codigo.ToLower() == "ordendeot" || posicion.TipoImputacion?.Codigo.ToLower() == "ordendeinversion") ? "X" : "",
                    PROFIT_CTR = (posicion.TipoImputacion?.Codigo.ToLower() == "siniestrobeneficio") ? "X" : ""
                });

                //Nombre: ZBAPIMEPOADDREDELIVERY Denominación:	Direcciones de entrega
                modificarPedidoSAP.POADDRDELIVERY.Add(new BAPIMEPOADDRDELIVERY
                {
                    PO_ITEM = poItem,
                    POSTL_COD1 = posicion.CpEntrega,
                    CITY = posicion.Centro.Descripcion,
                    ADDR_NO = "",
                    NAME = posicion.NombreEntrega,
                    TEL1_NUMBR = "",
                    STREET = posicion.CalleEntrega,
                    STREET_NO = "",//no tenemos el campo separado en calle y altura
                    REGION = ""
                });


                //subposiciones
                if (!esPosicionDeMateriales)
                {

                    var LINE_NO = 1;
                    //cabecera de subposiciones 
                    var cabeceraSubPos = new BAPIESLLC
                    {
                        PCKG_NO = $"{numeroDePaquete:0000000000}",
                        LINE_NO = $"{LINE_NO++:0000000000}",
                        OUTL_IND = "X",
                        OUTL_LEVEL = 0,
                        SUBPCKG_NO = $"{PCKG_NO:0000000000}",
                    };
                    modificarPedidoSAP.POSERVICES.Add(cabeceraSubPos);

                    foreach (var subposicion in posicion.Subposiciones)
                    {
                        CotizacionSubPosicion cotizacionSubPosicion = adjudicacionPosicion.CotizacionPosicion.CotizacionSubPosiciones.Where(a => a.SolpSubPosicion_Id == subposicion.Id).Single();

                        var subposicionSap = new BAPIESLLC();
                        subposicionSap.PCKG_NO = $"{PCKG_NO:0000000000}";
                        subposicionSap.LINE_NO = $"{LINE_NO:0000000000}";
                        subposicionSap.EXT_LINE = $"{LINE_NO * 10:0000000000}";
                        subposicionSap.SERVICE = subposicion.ServicioSolp?.Codigo;
                        subposicionSap.SHORT_TEXT = subposicion.Tarea;
                        subposicionSap.QUANTITY = cotizacionSubPosicion.Cantidad.Value;
                        subposicionSap.QUANTITYSpecified = true;
                        subposicionSap.BASE_UOM = cotizacionSubPosicion.UnidadDeMedida.Codigo;
                        subposicionSap.UOM_ISO = cotizacionSubPosicion.UnidadDeMedida.Codigo;
                        subposicionSap.PRICE_UNIT = 1;
                        subposicionSap.PRICE_UNITSpecified = true;
                        subposicionSap.GR_PRICE = cotizacionSubPosicion.Precio.Value;
                        subposicionSap.GR_PRICESpecified = true;

                        modificarPedidoSAP.POSERVICES.Add(subposicionSap);

                        var imputacionSubPos = new BAPIESKLC()
                        {
                            PCKG_NO = $"{PCKG_NO:0000000000}",
                            LINE_NO = $"{LINE_NO++:0000000000}",
                            PERCENTAGE = 100,
                            SERNO_LINE = $"{numeroPosicion:00}",
                            SERIAL_NO = numeroDeImputacion,
                        };

                        modificarPedidoSAP.POSRVACCESSVALUES.Add(imputacionSubPos);
                    }
                    PCKG_NO++;
                }



            }

            var listaVaciaTexto = new string[] { "" };

            var textosDiccionario = new Dictionary<string, string[]>() {
                {"F01", !string.IsNullOrEmpty(adjudicacion.TextoDeCabecera) ?  adjudicacion.TextoDeCabecera.SplitParagraph(131).Where(x => x != null).ToArray() : listaVaciaTexto},
                {"F05", !string.IsNullOrEmpty(adjudicacion.CondicionesDeEntrega)? adjudicacion.CondicionesDeEntrega.SplitParagraph(131).Where(x => x != null).ToArray() : listaVaciaTexto},
                {"F07", !string.IsNullOrEmpty(adjudicacion.CondicionesDePago) ? adjudicacion.CondicionesDePago.SplitParagraph(131).Where(x => x != null).ToArray() : listaVaciaTexto},
                {"F08", !string.IsNullOrEmpty(adjudicacion.Garantias) ? adjudicacion.Garantias.SplitParagraph(131).Where(x => x != null).ToArray() : listaVaciaTexto},
            };

            foreach (var grupos in textosDiccionario)
            {
                bool todosVacios = grupos.Value.All(string.IsNullOrEmpty);
                if (!todosVacios)
                {
                    foreach (var texto in grupos.Value)
                    {
                        modificarPedidoSAP.POTEXTHEADER.Add(new BAPIMEPOTEXTHEADER
                        {
                            TEXT_ID = grupos.Key,
                            PO_NUMBER = "",
                            PO_ITEM = "0",
                            TEXT_FORM = "*",
                            TEXT_LINE = texto
                        });
                    }
                }

            }


            return modificarPedidoSAP;
        }

        private static string ObtenerImputacion(bool esPosicionDeMateriales, SolpPosicion posicion, List<string> tipos)
        {

            if (tipos.Contains(posicion.TipoImputacion?.Codigo.ToLower()))
            {
                if (esPosicionDeMateriales)
                {
                    return posicion.TipoImputacionSap?.Codigo ?? "";
                }
                else
                {
                    return posicion.Subposiciones.FirstOrDefault()?.TipoImputacionSap?.Codigo ?? "";
                }
            }
            else
            {
                return "";
            }
        }

        private static string ObtenerCuentaMayor(bool esPosicionDeMateriales, SolpPosicion posicion)
        {
            return esPosicionDeMateriales ? (posicion.CuentaMayorSap?.Codigo ?? "") : posicion.Subposiciones.FirstOrDefault()?.CuentaMayorSap?.Codigo ?? "";
        }

        private decimal CalcularPrecioBrutoServicio(SolpPosicion posicion, AdjudicacionPosicion adjudicacionPosicion)
        {
            decimal total = 0;

            foreach (var item in adjudicacionPosicion.CotizacionPosicion.CotizacionSubPosiciones)
            {
                total += item.Cantidad.Value * item.Precio.Value;
            }

            return total;
        }





        public class ModificarPedidoSAP
        {
            public string MEMORY_COMPLETE { get; set; }
            public string MEMORY_UNCOMPLETE { get; set; }
            public string NO_AUTHORITY { get; set; }
            public string NO_MESSAGE_REQ { get; set; }
            public string NO_MESSAGING { get; set; }
            public string NO_PRICE_FROM_PO { get; set; }
            public string PARK_COMPLETE { get; set; }
            public string PARK_UNCOMPLETE { get; set; }
            public BAPIMEPOADDRVENDOR POADDRVENDOR { get; set; }
            public BAPIEIKP POEXPIMPHEADER { get; set; }
            public BAPIEIKPX POEXPIMPHEADERX { get; set; }
            public BAPIMEPOHEADER POHEADER { get; set; }
            public BAPIMEPOHEADERX POHEADERX { get; set; }
            public string PURCHASEORDER { get; set; }
            public string TESTRUN { get; set; }
            public BAPIMEDCM VERSIONS { get; set; }
            public List<BAPIMEDCM_ALLVERSIONS> ALLVERSIONS { get; set; } = new List<BAPIMEDCM_ALLVERSIONS>();
            public List<BAPIPAREX> EXTENSIONIN { get; set; } = new List<BAPIPAREX>();
            public List<BAPIPAREX> EXTENSIONOUT { get; set; } = new List<BAPIPAREX>();
            public List<BAPI_INVOICE_PLAN_HEADER> INVPLANHEADER { get; set; } = new List<BAPI_INVOICE_PLAN_HEADER>();
            public List<BAPI_INVOICE_PLAN_HEADERX> INVPLANHEADERX { get; set; } = new List<BAPI_INVOICE_PLAN_HEADERX>();
            public List<BAPI_INVOICE_PLAN_ITEM> INVPLANITEM { get; set; } = new List<BAPI_INVOICE_PLAN_ITEM>();
            public List<BAPI_INVOICE_PLAN_ITEMX> INVPLANITEMX { get; set; } = new List<BAPI_INVOICE_PLAN_ITEMX>();
            public List<BAPIMEPOACCOUNT> POACCOUNT { get; set; } = new List<BAPIMEPOACCOUNT>();
            public List<BAPIMEPOACCOUNTPROFITSEGMENT> POACCOUNTPROFITSEGMENT { get; set; } = new List<BAPIMEPOACCOUNTPROFITSEGMENT>();
            public List<BAPIMEPOACCOUNTX> POACCOUNTX { get; set; } = new List<BAPIMEPOACCOUNTX>();
            public List<BAPIMEPOADDRDELIVERY> POADDRDELIVERY { get; set; } = new List<BAPIMEPOADDRDELIVERY>();
            public List<BAPIMEPOCOMPONENT> POCOMPONENTS { get; set; } = new List<BAPIMEPOCOMPONENT>();
            public List<BAPIMEPOCOMPONENTX> POCOMPONENTSX { get; set; } = new List<BAPIMEPOCOMPONENTX>();
            public List<BAPIMEPOCOND> POCOND { get; set; } = new List<BAPIMEPOCOND>();
            public List<BAPIMEPOCONDHEADER> POCONDHEADER { get; set; } = new List<BAPIMEPOCONDHEADER>();
            public List<BAPIMEPOCONDHEADERX> POCONDHEADERX { get; set; } = new List<BAPIMEPOCONDHEADERX>();
            public List<BAPIMEPOCONDX> POCONDX { get; set; } = new List<BAPIMEPOCONDX>();
            public List<BAPIEKES> POCONFIRMATION { get; set; } = new List<BAPIEKES>();
            public List<BAPIESUCC> POCONTRACTLIMITS { get; set; } = new List<BAPIESUCC>();
            public List<BAPIEIPO> POEXPIMPITEM { get; set; } = new List<BAPIEIPO>();
            public List<BAPIEIPOX> POEXPIMPITEMX { get; set; } = new List<BAPIEIPOX>();
            public List<BAPIEKBE> POHISTORY { get; set; } = new List<BAPIEKBE>();
            public List<BAPIEKBE_MA> POHISTORY_MA { get; set; } = new List<BAPIEKBE_MA>();
            public List<BAPIEKBES> POHISTORY_TOTALS { get; set; } = new List<BAPIEKBES>();
            public List<BAPIMEPOITEM> POITEM { get; set; } = new List<BAPIMEPOITEM>();
            public List<BAPIMEPOITEMX> POITEMX { get; set; } = new List<BAPIMEPOITEMX>();
            public List<BAPIESUHC> POLIMITS { get; set; } = new List<BAPIESUHC>();
            public List<BAPIEKKOP> POPARTNER { get; set; } = new List<BAPIEKKOP>();
            public List<BAPIMEPOSCHEDULE> POSCHEDULE { get; set; } = new List<BAPIMEPOSCHEDULE>();
            public List<BAPIMEPOSCHEDULX> POSCHEDULEX { get; set; } = new List<BAPIMEPOSCHEDULX>();
            public List<BAPIESLLC> POSERVICES { get; set; } = new List<BAPIESLLC>();
            public List<BAPIESLLTX> POSERVICESTEXT { get; set; } = new List<BAPIESLLTX>();
            public List<BAPIITEMSHIP> POSHIPPING { get; set; } = new List<BAPIITEMSHIP>();
            public List<BAPIMEPOSHIPPEXP> POSHIPPINGEXP { get; set; } = new List<BAPIMEPOSHIPPEXP>();
            public List<BAPIITEMSHIPX> POSHIPPINGX { get; set; } = new List<BAPIITEMSHIPX>();
            public List<BAPIESKLC> POSRVACCESSVALUES { get; set; } = new List<BAPIESKLC>();
            public List<BAPIMEPOTEXTHEADER> POTEXTHEADER { get; set; } = new List<BAPIMEPOTEXTHEADER>();
            public List<BAPIMEPOTEXT> POTEXTITEM { get; set; } = new List<BAPIMEPOTEXT>();
            public List<BAPIRET2> RETURN { get; set; } = new List<BAPIRET2>();
            public List<BAPIMEPOSERIALNO> SERIALNUMBER { get; set; } = new List<BAPIMEPOSERIALNO>();
            public List<BAPIMEPOSERIALNOX> SERIALNUMBERX { get; set; } = new List<BAPIMEPOSERIALNOX>();
            public BAPIEIKP EXPPOEXPIMPHEADER { get; set; }
            public List<_NFM_BAPIDOCITM> NFMETALLITMS { get; set; } = new List<_NFM_BAPIDOCITM>();
        }


    }

    //public class CrearPedidoConsumerMOAResponse
    //{
    //    public string NumeroPedido { get; set; }
    //    public List<CrearPedidoConsumerMOAError> Errores { get; set; }
    //    public string Resultado { get; set; }
    //}

    //public class CrearPedidoConsumerMOAError
    //{
    //    public string Codigo { get; set; }
    //    public string Mensaje { get; set; }
    //    public string Tipo { get; set; }
    //}

    //public class SolpPedidoSAPDto
    //{

    //    public List<ZMPES6830> IM_POACCOUNTList { get; set; }
    //    public List<ZMPES6840> IM_POACCOUNTXList { get; set; }
    //    public List<ZMPES6820> IM_POADDREDELIVERYList { get; set; }
    //    public List<ZMPES6870> IM_POCONDList { get; set; }
    //    public List<ZMPES6850> IM_POCONDHEADERList { get; set; }
    //    public List<ZMPES6860> IM_POCONDHEADERXList { get; set; }
    //    public List<ZMPES6880> IM_POCONDXList { get; set; }
    //    public ZMPES6780 IM_POHEADERList { get; set; }
    //    public ZMPES6790 IM_POHEADERXList { get; set; }
    //    public List<ZMPES6800> IM_POITEMList { get; set; }
    //    public List<ZMPES6810> IM_POITEMXList { get; set; }
    //    public List<BAPIMEPOSCHEDULE> IM_POSCHEDULEList { get; set; }
    //    public List<BAPIMEPOSCHEDULX> IM_POSCHEDULEXList { get; set; }
    //    public List<BAPIESKLC> IM_POSRVACCESSVALUESList { get; set; }
    //    public List<BAPIMEPOTEXTHEADER> IM_POTEXTHEADERList { get; set; }
    //    public List<BAPIMEPOTEXT> IM_POTEXTITEMList { get; set; }
    //    public List<BAPIESLLC> IM_SERVICESList { get; set; }
    //    public string IM_URL { get; set; }




    //    public SolpPedidoSAPDto()
    //    {
    //        IM_POACCOUNTList = new List<ZMPES6830>();
    //        IM_POACCOUNTXList = new List<ZMPES6840>();
    //        IM_POADDREDELIVERYList = new List<ZMPES6820>();
    //        IM_POCONDList = new List<ZMPES6870>();
    //        IM_POCONDHEADERList = new List<ZMPES6850>();
    //        IM_POCONDHEADERXList = new List<ZMPES6860>();
    //        IM_POCONDXList = new List<ZMPES6880>();
    //        IM_POHEADERList = new ZMPES6780();
    //        IM_POHEADERXList = new ZMPES6790();
    //        IM_POITEMList = new List<ZMPES6800>();
    //        IM_POITEMXList = new List<ZMPES6810>();
    //        IM_POSCHEDULEList = new List<BAPIMEPOSCHEDULE>();
    //        IM_POSCHEDULEXList = new List<BAPIMEPOSCHEDULX>();
    //        IM_POSRVACCESSVALUESList = new List<BAPIESKLC>();
    //        IM_POTEXTHEADERList = new List<BAPIMEPOTEXTHEADER>();
    //        IM_POTEXTITEMList = new List<BAPIMEPOTEXT>();
    //        IM_SERVICESList = new List<BAPIESLLC>();
    //        IM_URL = "";
    //    }
    //}

    public interface IModificarOrdenDeCompraConsumerMOA
    {
        CrearPedidoConsumerMOAResponse Request(Adjudicacion adjudicacion);

    }



}
