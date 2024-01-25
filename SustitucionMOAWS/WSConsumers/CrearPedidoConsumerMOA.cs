using SustitucionMOAFotmatter;
using SustitucionMOAModel.Entities;
using SustitucionMOAWS.CrearPedidoWebServiceMOA;
using SustitucionMOAWS.CredentialService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using SustitucionMOARepositorio.Extensiones;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAModel.Models.WSMapMOA.Compras;

namespace SustitucionMOAWS.WSConsumers
{
    public class CrearPedidoConsumerMOA : ICrearPedidoConsumerMOA
    {
        private readonly SI_MMRFC_CREAR_PEDIDOClient service;
        private readonly string rutaArchivosXmls = ConfigurationManager.AppSettings["RutaArchivosCompras"];
        private readonly IObtenerUnidadesDeMedidaAlternativasConsumerMOA obtenerUnidadesDeMedidaConsumerMOA;

        public CrearPedidoConsumerMOA(IObtenerUnidadesDeMedidaAlternativasConsumerMOA _obtenerUnidadesDeMedidaConsumerMOA)
        {
            var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=SI_MMRFC_CREAR_PEDIDO&amp;interfaceNamespace=urn%3AOPERACIONES";
            service = new SI_MMRFC_CREAR_PEDIDOClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
            obtenerUnidadesDeMedidaConsumerMOA = _obtenerUnidadesDeMedidaConsumerMOA;
        }

        public CrearPedidoConsumerMOAResponse Request(Adjudicacion adjudicacion)
        {
            var solpPedidoSAP = ConvertirSOLP(adjudicacion);

            var serxml = new System.Xml.Serialization.XmlSerializer(solpPedidoSAP.GetType());
            var ms = new MemoryStream();
            serxml.Serialize(ms, solpPedidoSAP);
            string xml = Encoding.UTF8.GetString(ms.ToArray());

            var fecha = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");

            var nombreArchivoLlamada = string.Concat(adjudicacion.Solp_Id, " - ", fecha, " - crear pedido.xml");

            var rutaArchivoLlamada = Path.Combine(rutaArchivosXmls, "XMLS", nombreArchivoLlamada);

            FileInfo fileCrear = new FileInfo(rutaArchivoLlamada);
            fileCrear.Directory.Create();
            File.WriteAllText(fileCrear.FullName, xml);

            var result = service.SI_MMRFC_CREAR_PEDIDO(solpPedidoSAP.IM_POACCOUNTList.ToArray(),
                                                        solpPedidoSAP.IM_POACCOUNTXList.ToArray(),
                                                        solpPedidoSAP.IM_POADDREDELIVERYList.ToArray(),
                                                        solpPedidoSAP.IM_POCONDList.ToArray(),
                                                        solpPedidoSAP.IM_POCONDHEADERList.ToArray(),
                                                        solpPedidoSAP.IM_POCONDHEADERXList.ToArray(),
                                                        solpPedidoSAP.IM_POCONDXList.ToArray(),
                                                        solpPedidoSAP.IM_POHEADERList,
                                                        solpPedidoSAP.IM_POHEADERXList,
                                                        solpPedidoSAP.IM_POITEMList.ToArray(),
                                                        solpPedidoSAP.IM_POITEMXList.ToArray(),
                                                        solpPedidoSAP.IM_POSCHEDULEList.ToArray(),
                                                        solpPedidoSAP.IM_POSCHEDULEXList.ToArray(),
                                                        solpPedidoSAP.IM_POSRVACCESSVALUESList.ToArray(),
                                                        solpPedidoSAP.IM_POTEXTHEADERList.ToArray(),
                                                        solpPedidoSAP.IM_POTEXTITEMList.ToArray(),
                                                        solpPedidoSAP.IM_SERVICESList.ToArray(),
                                                        solpPedidoSAP.IM_URL,
                                                        out string EX_PO_NUMBER,
                                                        out BAPIRET2[] EX_RETURN
                                                        );


            var respuesta = new CrearPedidoConsumerMOAResponse();

            respuesta.NumeroPedido = EX_PO_NUMBER;
            respuesta.Resultado = result;
            respuesta.Errores = new List<CrearPedidoConsumerMOAError>();

            foreach (var errorSAP in EX_RETURN)
            {
                var error = new CrearPedidoConsumerMOAError
                {
                    Codigo = errorSAP.FIELD,
                    Mensaje = errorSAP.MESSAGE,
                    Tipo = errorSAP.TYPE
                };

                respuesta.Errores.Add(error);
            }

            serxml = new System.Xml.Serialization.XmlSerializer(respuesta.GetType());
            ms = new MemoryStream();
            serxml.Serialize(ms, respuesta);
            xml = Encoding.UTF8.GetString(ms.ToArray());
            using (StreamWriter writer = File.AppendText(rutaArchivoLlamada))
            {
                writer.WriteLine(xml);
            }

            return respuesta;
        }

        private SolpPedidoSAPDto ConvertirSOLP(Adjudicacion adjudicacion)
        {
            //TODO: Crear OC ConvertirSOLP - fields hardcodeados o para revisar
            ///DOC_TYPE  ok por ahora. Clase de documento de compras / Estrategia de liberacion hardcore ZPE1 

            ///STREET y STREET_NO ok. no tenemos el campo separado mandamos todo en street            
            ///SERIAL_NO/serialNumber siempre 1 por que se imputa todo a lo mismo sino son imputaciones multiples, en ese caso analizar como se envia.

            var proveedorCodigoDeLaAdjudicacion = adjudicacion.Posiciones.First().CotizacionPosicion.Cotizacion.PeticionDeOfertaUsuario.Usuario.ObtenerCodigoProveedor();
            var usuarioCreadorAdjudicacion = adjudicacion.Usuario.UsuarioSap;
            var usuarioOrganizacionDeCompra = adjudicacion.Usuario.OrganizacionDeCompra;
            var solp = adjudicacion.Solp;
            SolpPedidoSAPDto solpPedidoSAP = new SolpPedidoSAPDto();
            int numeroPosicion = 0;
            string preqItem = "";
            string numeroDeImputacion = "";
            var PCKG_NO = 1000;
            var numeroDePaquete = 1;
            bool esPosicionDeMateriales = solp.Posiciones.First().TipoPosicion.Codigo == "MATERIALES";
            var unidadesDeMedidaSAP = new List<UnidadesDeMedida>();
            if (esPosicionDeMateriales)
            {
                unidadesDeMedidaSAP = obtenerUnidadesDeMedidaConsumerMOA.Request(solp.Posiciones.Select(x => x.MaterialSolp?.Codigo).ToList());
            }

            //aca el metodo solo usa las posiciones seleccionadas por el comprador
            var posIds = adjudicacion.Posiciones.Select(x => x.CotizacionPosicion.PeticionDeOfertaSolpPosicion.SolpPosicion_Id).ToList();
            foreach (var solpPosicion in solp.Posiciones.Where(a => posIds.Contains(a.Id)).OrderBy(x => x.Id))
            {
                var adjudicacionPosicion = adjudicacion.Posiciones.Where(a => a.CotizacionPosicion.PeticionDeOfertaSolpPosicion.SolpPosicion_Id == solpPosicion.Id).Single();
                decimal precioConvertido = adjudicacionPosicion.CotizacionPosicion.Precio ?? 0;
                string unidadDeMedida = esPosicionDeMateriales ? adjudicacionPosicion.CotizacionPosicion.UnidadDeMedida.Descripcion : "001";
                if (esPosicionDeMateriales && solpPosicion.Unidad_Id != adjudicacionPosicion.CotizacionPosicion.UnidadDeMedida_Id)
                {
                    var unidadesDelMaterial = unidadesDeMedidaSAP.Where(x => x.CodigoMaterial == solpPosicion.MaterialSolp.Codigo).ToList();
                    var unidadSolicitada = unidadesDelMaterial.First(x => x.UnidadDeMedida == solpPosicion.Unidad.Descripcion);
                    var unidadCotizada = unidadesDelMaterial.First(x => x.UnidadDeMedida == adjudicacionPosicion.CotizacionPosicion.UnidadDeMedida.Descripcion);
                    unidadDeMedida = unidadSolicitada.UnidadDeMedida;
                    precioConvertido = Math.Round(adjudicacionPosicion.CotizacionPosicion.Precio.Value / (unidadCotizada.Numerador / unidadCotizada.Denominador) * (unidadSolicitada.Numerador / unidadSolicitada.Denominador), 2);
                };
                numeroPosicion = solpPosicion.Indice ?? 0;
                numeroDePaquete = solpPosicion.Indice ?? 0;
                preqItem = $"{numeroPosicion:00000}";
                numeroDeImputacion = "01";// SERIAL_NO por ahora siempre 01 por que no hay imputaciones multiples

                //Nombre: ZBAPIMEPOHEADER Denominación:	Cabecera del Pedido de Compras
                var cabeceraDelPedido = new ZMPES6780();
                cabeceraDelPedido.COMP_CODE = "MOA"; //COMP_CODE BUKRS   Sociedad
                cabeceraDelPedido.DOC_TYPE = "ZPE1";//solp.ClaseDocumento.CodigoSap; //DOC_TYPE    ESART Clase de documento de compras
                cabeceraDelPedido.VENDOR = proveedorCodigoDeLaAdjudicacion;//VENDOR ELIFN   Número de cuenta del proveedor
                cabeceraDelPedido.PURCH_ORG = usuarioOrganizacionDeCompra;//PURCH_ORG EKORG   Organización de compras
                cabeceraDelPedido.PUR_GROUP = solpPosicion.GrupoCompras.CodigoSap.ToString(); //PUR_GROUP   BKGRP Grupo de compras
                cabeceraDelPedido.CURRENCY = adjudicacionPosicion.CotizacionPosicion.Moneda.Codigo; //CURRENCY WAERS   Clave de moneda
                cabeceraDelPedido.CREATED_BY = usuarioCreadorAdjudicacion;//CREATED_BY ERNAM   Nombre del responsable que ha añadido el objeto
                cabeceraDelPedido.DOC_DATE = SAPFormatter.PrepararFecha(DateTime.Now); //DOC_DATE    EBDAT Fecha del documento de compras
                cabeceraDelPedido.PO_NUMBER = ""; //PO_NUMBER   EBELN Número del documento de compras
                cabeceraDelPedido.DELETE_IND = "";//DELETE_IND ELOEK   Indicador de borrado en el documento de compras
                cabeceraDelPedido.STATUS = ""; //STATUS ESTAK   Status del documento de compras
                cabeceraDelPedido.CREAT_DATE = ""; //SAPFormatter.PrepararFecha(solp.FechaCreacion); //CREAT_DATE  ERDAT Fecha de creación del registro
                cabeceraDelPedido.PMNTTRMS = ""; //"BASE"; //PMNTTRMS    DZTERM Clave de condiciones de pago
                //cabeceraDelPedido.EXCH_RATE = 0; //EXCH_RATE   WKURS Tipo de cambio de moneda
                cabeceraDelPedido.EX_RATE_FX = ""; //EX_RATE_FX KUFIX   Indicador tipo de cambio fijo

                solpPedidoSAP.IM_POHEADERList = cabeceraDelPedido;
                solpPedidoSAP.IM_POHEADERXList = new ZMPES6790
                {
                    PO_NUMBER = "",
                    COMP_CODE = "X",
                    DOC_TYPE = "X",
                    DELETE_IND = "",
                    STATUS = "",
                    CREAT_DATE = "",
                    CREATED_BY = "X",
                    VENDOR = "X",
                    PMNTTRMS = "",
                    PURCH_ORG = "X",
                    PUR_GROUP = "X",
                    CURRENCY = "X",
                    EXCH_RATE = "",
                    EX_RATE_FX = "",
                    DOC_DATE = "X"
                };

                //Nombre: ZBAPIMEPOITEM Denominación:	Posición de PEDIDOS
                var IM_POITEM = new ZMPES6800();

                IM_POITEM.PO_ITEM = preqItem;
                IM_POITEM.SHORT_TEXT = solpPosicion.Tarea;
                IM_POITEM.PLANT = solpPosicion.Centro.CodigoSap.ToString();
                IM_POITEM.MATL_GROUP = solpPosicion.GrupoArticulo?.CodigoSap?.ToString() ?? "";
                //IM_POITEM.MATL_GROUP = posicion.GrupoArticulo.CodigoSap.ToString();
                IM_POITEM.MATERIAL = esPosicionDeMateriales ? solpPosicion.MaterialSolp?.CodigoSap.ToString() : "";
                IM_POITEM.STGE_LOC = solpPosicion.Almacen != null ? solpPosicion.Almacen.CodigoSap.ToString() : "";
                IM_POITEM.ITEM_CAT = solpPosicion.TipoPosicion.Codigo.ToLower() == "servicio" ? "9" : "0";//ITEM_CAT PSTYP   Tipo de posición del documento de compras
                IM_POITEM.TRACKINGNO = solpPosicion.NroNecesidad;
                IM_POITEM.INFO_REC = "";
                IM_POITEM.QUANTITY = esPosicionDeMateriales ? adjudicacionPosicion.Cantidad : 0;
                IM_POITEM.QUANTITYSpecified = esPosicionDeMateriales;
                IM_POITEM.PO_UNIT = unidadDeMedida;
                IM_POITEM.NET_PRICE = esPosicionDeMateriales ? precioConvertido : CalcularPrecioBrutoServicio(solpPosicion, adjudicacionPosicion);
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

                switch (solpPosicion.TipoImputacion?.Codigo.ToLower())
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

                solpPedidoSAP.IM_POITEMList.Add(IM_POITEM);

                solpPedidoSAP.IM_POITEMXList.Add(new ZMPES6810
                {
                    PO_ITEM = preqItem,
                    DELETE_IND = "",
                    SHORT_TEXT = "X",
                    MATERIAL = "X",
                    PLANT = "X",
                    STGE_LOC = "X",
                    TRACKINGNO = string.IsNullOrEmpty(solpPosicion.NroNecesidad) ? "" : "X",
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

                solpPedidoSAP.IM_POCONDList.Add(new ZMPES6870
                {
                    ITM_NUMBER = preqItem,  //el número de ítem al que corresponda la condición
                    COND_TYPE = "ZP01",// siempre va el mismo dato
                    COND_VALUE = IM_POITEM.NET_PRICE, //el importe de la condición
                    COND_VALUESpecified = true,
                    CURRENCY = adjudicacionPosicion.CotizacionPosicion.Moneda.Codigo,//moneda de la adjudicacion
                    CHANGE_ID = "U",// siempra va el mismo valor

                });
                solpPedidoSAP.IM_POCONDXList.Add(new ZMPES6880
                {
                    ITM_NUMBER = preqItem,
                    COND_TYPE = "X",
                    COND_VALUE = "X",
                    CURRENCY = "X",
                    CHANGE_ID = "X",
                });

                //Nombre: ZBAPIMEPOACCOUNT IM_POACCOUNT Denominación:	Imputación
                var imputacion = new ZMPES6830();
                imputacion.PO_ITEM = preqItem;
                imputacion.SERIAL_NO = numeroDeImputacion;
                imputacion.GL_ACCOUNT = ObtenerCuentaMayor(esPosicionDeMateriales, solpPosicion);
                imputacion.QUANTITY = esPosicionDeMateriales ? adjudicacionPosicion.Cantidad : 0;
                imputacion.QUANTITYSpecified = imputacion.QUANTITY > 0;
                imputacion.BUS_AREA = "GENE";
                imputacion.CO_AREA = "MOA";
                imputacion.COSTCENTER = ObtenerImputacion(esPosicionDeMateriales, solpPosicion, new List<string> { "centrodecosto" });
                imputacion.ORDERID = ObtenerImputacion(esPosicionDeMateriales, solpPosicion, new List<string> { "ordendeot", "ordendeinversion" });
                imputacion.PROFIT_CTR = ObtenerImputacion(esPosicionDeMateriales, solpPosicion, new List<string> { "siniestrobeneficio" });
                imputacion.SUB_NUMBER = "";
                imputacion.ASSET_NO = "";
                imputacion.COSTOBJECT = "";
                imputacion.DELETE_IND = "";
                solpPedidoSAP.IM_POACCOUNTList.Add(imputacion);

                solpPedidoSAP.IM_POACCOUNTXList.Add(new ZMPES6840
                {
                    PO_ITEM = preqItem,
                    SERIAL_NO = numeroDeImputacion,
                    DELETE_IND = "",
                    QUANTITY = "X",
                    GL_ACCOUNT = "X",
                    BUS_AREA = "X",
                    ASSET_NO = "",
                    SUB_NUMBER = "",
                    CO_AREA = "X",
                    COSTOBJECT = "",
                    COSTCENTER = (solpPosicion.TipoImputacion?.Codigo.ToLower() == "centrodecosto") ? "X" : "",
                    ORDERID = (solpPosicion.TipoImputacion?.Codigo.ToLower() == "ordendeot" || solpPosicion.TipoImputacion?.Codigo.ToLower() == "ordendeinversion") ? "X" : "",
                    PROFIT_CTR = (solpPosicion.TipoImputacion?.Codigo.ToLower() == "siniestrobeneficio") ? "X" : ""
                });

                //Nombre: ZBAPIMEPOADDREDELIVERY Denominación:	Direcciones de entrega
                solpPedidoSAP.IM_POADDREDELIVERYList.Add(new ZMPES6820
                {
                    PO_ITEM = preqItem,
                    POSTL_COD1 = solpPosicion.CpEntrega,
                    CITY = solpPosicion.Centro.Descripcion,
                    ADDR_NO = "",
                    NAME = solpPosicion.NombreEntrega,
                    TEL1_NUMBR = "",
                    STREET = solpPosicion.CalleEntrega,
                    STREET_NO = "",//no tenemos el campo separado en calle y altura
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
                    solpPedidoSAP.IM_SERVICESList.Add(cabeceraSubPos);

                    foreach (var subposicion in solpPosicion.Subposiciones)
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

                        solpPedidoSAP.IM_SERVICESList.Add(subposicionSap);

                        var imputacionSubPos = new BAPIESKLC()
                        {
                            PCKG_NO = $"{PCKG_NO:0000000000}",
                            LINE_NO = $"{LINE_NO++:0000000000}",
                            PERCENTAGE = 100,
                            SERNO_LINE = $"{numeroPosicion:00}",
                            SERIAL_NO = numeroDeImputacion,
                        };

                        solpPedidoSAP.IM_POSRVACCESSVALUESList.Add(imputacionSubPos);
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
                        solpPedidoSAP.IM_POTEXTHEADERList.Add(new BAPIMEPOTEXTHEADER
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
            solpPedidoSAP.IM_URL = ConfigurationManager.AppSettings["SpaUrl"] + "/verLegajoOrdenDeCompra/" + adjudicacion.Id + "/" + adjudicacion.Token;

            return solpPedidoSAP;
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

        //private SolpPedidoSAPDto ConvertirSOLPMateriales(Adjudicacion adjudicacion)
        //{
        //    var proveedorCodigoDeLaAdjudicacion = "0057984261";
        //    var usuarioCreadorAdjudicacion = "RABELLATM";
        //    var solp = adjudicacion.Solp;

        //    SolpPedidoSAPDto solpPedidoSAP = new SolpPedidoSAPDto();
        //    int numeroPosicion = 0;
        //    string numeroPaquete = "";
        //    string preqItem = "";
        //    string serialNumber = "";

        //    //string docItem = "";

        //    bool esPosicionDeMateriales = solp.Posiciones.First().TipoPosicion.Codigo == "MATERIALES";

        //    //aca el metodo solo usa las posiciones seleccionadas por el comprador
        //    var posIds = adjudicacion.Posiciones.Select(x => x.CotizacionPosicion.PeticionDeOfertaSolpPosicion.SolpPosicion_Id).ToList();
        //    foreach (var posicion in solp.Posiciones.Where(a => posIds.Contains(a.Id)).OrderBy(x => x.Id))
        //    {


        //        bool eliminarPosicion = posicion.Subposiciones.Where(item => !Convert.ToBoolean(item.Estado)).Count() == posicion.Subposiciones.Count;
        //        bool eliminarSubPosicion = posicion.Subposiciones.Where(item => !Convert.ToBoolean(item.Estado)).Count() == posicion.Subposiciones.Count;
        //        eliminarPosicion = eliminarPosicion ? true : !posicion.Estado;
        //        numeroPosicion = posicion.Indice ?? 0;
        //        preqItem = $"{numeroPosicion:00000}";
        //        //docItem = preqItem;
        //        numeroPaquete = $"{numeroPosicion:0000000000}";
        //        serialNumber = "01";//$"{numeroPosicion:00}";


        //        //Nombre: ZBAPIMEPOHEADER Denominación:	Cabecera del Pedido de Compras
        //        var cabeceraDelPedido = new ZMPES6780();

        //        cabeceraDelPedido.COMP_CODE = "MOA"; //COMP_CODE BUKRS   Sociedad
        //        cabeceraDelPedido.DOC_TYPE = "ZPE1";//solp.ClaseDocumento.CodigoSap; //DOC_TYPE    ESART Clase de documento de compras
        //        cabeceraDelPedido.VENDOR = proveedorCodigoDeLaAdjudicacion;//posicion.ProveedorFijo; //VENDOR ELIFN   Número de cuenta del proveedor
        //        cabeceraDelPedido.PURCH_ORG = "2029";// posicion.OrganizacionCompras; //PURCH_ORG EKORG   Organización de compras
        //        cabeceraDelPedido.PUR_GROUP = posicion.GrupoCompras.CodigoSap.ToString(); //PUR_GROUP   BKGRP Grupo de compras
        //        cabeceraDelPedido.CURRENCY = posicion.Moneda.Codigo; //CURRENCY WAERS   Clave de moneda
        //        cabeceraDelPedido.CREATED_BY = usuarioCreadorAdjudicacion;// solp.UsuarioCreacion.UsuarioSap; //CREATED_BY ERNAM   Nombre del responsable que ha añadido el objeto
        //        cabeceraDelPedido.DOC_DATE = SAPFormatter.PrepararFecha(DateTime.Now); //DOC_DATE    EBDAT Fecha del documento de compras

        //        cabeceraDelPedido.PO_NUMBER = ""; //PO_NUMBER   EBELN Número del documento de compras
        //        cabeceraDelPedido.DELETE_IND = "";// posicion.TipoPosicion.Codigo != "MATERIALES" ? SAPFormatter.FormatearBooleano(eliminarPosicion) : ""; //DELETE_IND ELOEK   Indicador de borrado en el documento de compras
        //        cabeceraDelPedido.STATUS = ""; //STATUS ESTAK   Status del documento de compras
        //        cabeceraDelPedido.CREAT_DATE = ""; //SAPFormatter.PrepararFecha(solp.FechaCreacion); //CREAT_DATE  ERDAT Fecha de creación del registro
        //        cabeceraDelPedido.PMNTTRMS = ""; //"BASE"; //PMNTTRMS    DZTERM Clave de condiciones de pago
        //        //cabeceraDelPedido.EXCH_RATE = 0; //EXCH_RATE   WKURS Tipo de cambio de moneda
        //        cabeceraDelPedido.EX_RATE_FX = ""; //EX_RATE_FX KUFIX   Indicador tipo de cambio fijo


        //        solpPedidoSAP.IM_POHEADERList = cabeceraDelPedido;
        //        solpPedidoSAP.IM_POHEADERXList = new ZMPES6790
        //        {
        //            PO_NUMBER = "",
        //            COMP_CODE = "X",
        //            DOC_TYPE = "X",
        //            DELETE_IND = "",
        //            STATUS = "",
        //            CREAT_DATE = "",
        //            CREATED_BY = "X",
        //            VENDOR = "X",
        //            PMNTTRMS = "",
        //            PURCH_ORG = "X",
        //            PUR_GROUP = "X",
        //            CURRENCY = "X",
        //            EXCH_RATE = "",
        //            EX_RATE_FX = "",
        //            DOC_DATE = "X"
        //        };

        //        //Nombre: ZBAPIMEPOITEM Denominación:	Posición de PEDIDOS
        //        var IM_POITEM = new ZMPES6800();

        //        IM_POITEM.PO_ITEM = preqItem;
        //        IM_POITEM.DELETE_IND = "";
        //        IM_POITEM.SHORT_TEXT = posicion.Tarea;
        //        IM_POITEM.MATERIAL = esPosicionDeMateriales ? posicion.MaterialSolp.CodigoSap.ToString() : "";
        //        IM_POITEM.PLANT = posicion.Centro.CodigoSap.ToString();
        //        IM_POITEM.STGE_LOC = posicion.Almacen.CodigoSap.ToString();
        //        IM_POITEM.TRACKINGNO = posicion.NroNecesidad;
        //        //IM_POITEM.MATL_GROUP = posicion.GrupoArticulo.CodigoSap.ToString();
        //        IM_POITEM.MATL_GROUP = esPosicionDeMateriales && string.IsNullOrEmpty(posicion.MaterialSolp.CodigoSap) ? posicion.GrupoArticulo.CodigoSap.ToString() : "";
        //        IM_POITEM.INFO_REC = "";
        //        IM_POITEM.QUANTITY = 1;// (decimal)posicion.Cantidad;
        //        IM_POITEM.QUANTITYSpecified = true;
        //        IM_POITEM.PO_UNIT = esPosicionDeMateriales ? posicion.Unidad.Descripcion : "001";
        //        IM_POITEM.NET_PRICE = esPosicionDeMateriales ? (decimal)posicion.PrecioBruto : CalcularPrecioBrutoServicio(posicion, adjudicacion);
        //        IM_POITEM.NET_PRICESpecified = true;
        //        IM_POITEM.PRICE_UNIT = 1;
        //        //IM_POITEM.PRICE_UNITSpecified = true;
        //        IM_POITEM.GR_PR_TIME = 0;
        //        //IM_POITEM.GR_PR_TIMESpecified = true; 
        //        IM_POITEM.TAX_CODE = "";
        //        IM_POITEM.VAL_TYPE = "";
        //        IM_POITEM.NO_MORE_GR = "";
        //        IM_POITEM.FINAL_INV = "";

        //        switch (posicion.TipoPosicion.Codigo.ToLower())
        //        //ITEM_CAT PSTYP   Tipo de posición del documento de compras
        //        {
        //            case "servicio":
        //                IM_POITEM.ITEM_CAT = "9";
        //                break;

        //            case "materiales":
        //            default:
        //                IM_POITEM.ITEM_CAT = "0";
        //                break;
        //        }

        //        switch (posicion.TipoImputacion.Codigo.ToLower())
        //        {
        //            case "centrodecosto":
        //                IM_POITEM.ACCTASSCAT = "K";
        //                break;
        //            case "ordendeot":
        //                IM_POITEM.ACCTASSCAT = "F";
        //                break;
        //            case "ordendeinversion":
        //                IM_POITEM.ACCTASSCAT = "F";
        //                break;
        //            case "siniestrobeneficio":
        //                IM_POITEM.ACCTASSCAT = "Y";
        //                break;
        //        }

        //        IM_POITEM.DISTRIB = "";
        //        IM_POITEM.PART_INV = "";
        //        IM_POITEM.GR_IND = "";
        //        IM_POITEM.GR_NON_VAL = "";
        //        IM_POITEM.IR_IND = "";
        //        IM_POITEM.FREE_ITEM = "";
        //        IM_POITEM.GR_BASEDIV = "";
        //        IM_POITEM.ACKN_REQD = "";
        //        IM_POITEM.ACKNOWL_NO = "";
        //        IM_POITEM.AGREEMENT = "";
        //        IM_POITEM.AGMT_ITEM = "";
        //        IM_POITEM.RFQ_NO = "";
        //        IM_POITEM.RFQ_ITEM = "";
        //        IM_POITEM.PREQ_NO = solp.NroSolp;
        //        IM_POITEM.PREQ_ITEM = preqItem;
        //        IM_POITEM.PCKG_NO = esPosicionDeMateriales ? "" : numeroPaquete;

        //        solpPedidoSAP.IM_POITEMList.Add(IM_POITEM);

        //        solpPedidoSAP.IM_POITEMXList.Add(new ZMPES6810
        //        {
        //            PO_ITEM = preqItem,
        //            DELETE_IND = "",//(IM_POITEM.DELETE_IND != null) ? "X" : "",
        //            SHORT_TEXT = "X",
        //            MATERIAL = "X",
        //            PLANT = "X",
        //            STGE_LOC = "X",
        //            TRACKINGNO = string.IsNullOrEmpty(posicion.NroNecesidad) ? "" : "X",
        //            MATL_GROUP = "X",
        //            INFO_REC = "",
        //            QUANTITY = ((decimal)IM_POITEM.QUANTITY == 0) ? "" : "X",
        //            PO_UNIT = "X",
        //            NET_PRICE = "X",
        //            PRICE_UNIT = "X",
        //            GR_PR_TIME = "",
        //            TAX_CODE = "",
        //            VAL_TYPE = "",//(IM_POITEM.VAL_TYPE != null) ? "X" : "",
        //            NO_MORE_GR = "",
        //            FINAL_INV = "",
        //            ITEM_CAT = "X",
        //            ACCTASSCAT = (IM_POITEM.ACCTASSCAT != null) ? "X" : "",
        //            DISTRIB = "",
        //            PART_INV = "",
        //            GR_IND = "",
        //            GR_NON_VAL = "",
        //            IR_IND = "",
        //            FREE_ITEM = "",
        //            GR_BASEDIV = "",
        //            ACKN_REQD = "",
        //            ACKNOWL_NO = "",
        //            AGREEMENT = "",//"X",
        //            AGMT_ITEM = "",//"X",
        //            RFQ_NO = "",
        //            RFQ_ITEM = "",
        //            PREQ_NO = "X",
        //            PREQ_ITEM = "X",
        //            PCKG_NO = "X"
        //        });

        //        //Nombre: ZBAPIMEPOACCOUNT Denominación:	Imputación
        //        var imputacion = new ZMPES6830();
        //        imputacion.PO_ITEM = preqItem;
        //        imputacion.SERIAL_NO = serialNumber;
        //        imputacion.DELETE_IND = "";
        //        imputacion.QUANTITY = 1;//posicion.Cantidad ?? 1;
        //        imputacion.QUANTITYSpecified = (posicion.Cantidad ?? 0) > 0;
        //        imputacion.GL_ACCOUNT = posicion.CuentaMayorSap.Codigo; //"0000607034";
        //        imputacion.BUS_AREA = "GENE";
        //        imputacion.ASSET_NO = "";
        //        imputacion.SUB_NUMBER = "";
        //        imputacion.CO_AREA = "MOA";
        //        imputacion.COSTOBJECT = "";
        //        imputacion.COSTCENTER = (posicion.TipoImputacion.Codigo.ToLower() == "centrodecosto") ? posicion.TipoImputacionSap?.Codigo : "";
        //        imputacion.ORDERID = (posicion.TipoImputacion.Codigo.ToLower() == "ordendeot") ? posicion.TipoImputacionSap?.Codigo : "";
        //        imputacion.PROFIT_CTR = (posicion.TipoImputacion.Codigo.ToLower() == "siniestrobeneficio") ? posicion.TipoImputacionSap?.Codigo : "";
        //        solpPedidoSAP.IM_POACCOUNTList.Add(imputacion);

        //        solpPedidoSAP.IM_POACCOUNTXList.Add(new ZMPES6840
        //        {
        //            PO_ITEM = preqItem,
        //            SERIAL_NO = serialNumber,
        //            DELETE_IND = "",
        //            QUANTITY = "X",
        //            GL_ACCOUNT = "X",
        //            BUS_AREA = "X",
        //            ASSET_NO = "",
        //            SUB_NUMBER = "",
        //            CO_AREA = "X",
        //            COSTOBJECT = "",
        //            COSTCENTER = (posicion.TipoImputacion.Codigo.ToLower() == "centrodecosto") ? "X" : "",
        //            ORDERID = (posicion.TipoImputacion.Codigo.ToLower() == "ordendeot") ? "X" : "",
        //            PROFIT_CTR = (posicion.TipoImputacion.Codigo.ToLower() == "siniestrobeneficio") ? "X" : ""
        //        });

        //        //Nombre: ZBAPIMEPOADDREDELIVERY Denominación:	Direcciones de entrega
        //        solpPedidoSAP.IM_POADDREDELIVERYList.Add(new ZMPES6820
        //        {
        //            PO_ITEM = preqItem,
        //            POSTL_COD1 = posicion.CpEntrega,
        //            CITY = posicion.Centro.Descripcion,
        //            ADDR_NO = "",
        //            NAME = posicion.NombreEntrega,
        //            TEL1_NUMBR = "",
        //            STREET = "Benielli",//posicion.CalleEntrega,
        //            STREET_NO = "398",//posicion.NumeroEntrega
        //        });

        //        if (!esPosicionDeMateriales)
        //        {
        //            foreach (var subposicion in posicion.Subposiciones)
        //            {
        //                var subposicionSap = new BAPIESLLC
        //                {
        //                    SERVICE = subposicion.ServicioSolp.Codigo,
        //                    SHORT_TEXT = subposicion.Tarea,
        //                    QUANTITY = subposicion.Cantidad ?? 0,
        //                    QUANTITYSpecified = true,
        //                    BASE_UOM = subposicion.Unidad.Codigo,
        //                    UOM_ISO = subposicion.Unidad.Codigo,
        //                    PRICE_UNIT = subposicion.PrecioBruto ?? 0,
        //                    PRICE_UNITSpecified = true,
        //                    GR_PRICE = subposicion.PrecioBruto ?? 0 * subposicion.Cantidad ?? 0,
        //                    GR_PRICESpecified = true,

        //                    PCKG_NO = esPosicionDeMateriales ? "" : numeroPaquete,
        //                    LINE_NO = subposicion.Numero.ToString(),
        //                    EXT_LINE = "",
        //                    OUTL_LEVEL = 1,
        //                    OUTL_LEVELSpecified = true,
        //                    OUTL_NO = "",
        //                    OUTL_IND = "",
        //                    SUBPCKG_NO = "",
        //                    SERV_TYPE = "",
        //                    EDITION = "",
        //                    SSC_ITEM = "",
        //                    EXT_SERV = "",
        //                    OVF_TOL = 1,
        //                    OVF_TOLSpecified = true,
        //                    OVF_UNLIM = "",
        //                    FROM_LINE = "",
        //                    TO_LINE = "",
        //                    DISTRIB = "",
        //                    PERS_NO = "",
        //                    WAGETYPE = "",
        //                    PLN_PCKG = "",
        //                    PLN_LINE = "",
        //                    CON_PCKG = "",
        //                    CON_LINE = "",
        //                    TMP_PCKG = "",
        //                    TMP_LINE = "",
        //                    SSC_LIM = "",
        //                    LIMIT_LINE = "",
        //                    TARGET_VAL = 1,
        //                    TARGET_VALSpecified = true,
        //                    BASLINE_NO = "",
        //                    BASIC_LINE = "",
        //                    ALTERNAT = "",
        //                    BIDDER = "",
        //                    SUPP_LINE = "",
        //                    OPEN_QTY = "",
        //                    INFORM = "",
        //                    BLANKET = "",
        //                    EVENTUAL = "",
        //                    TAX_CODE = "",
        //                    TAXJURCODE = "",
        //                    PRICE_CHG = "",
        //                    MATL_GROUP = "",
        //                    DATE = "",
        //                    BEGINTIME = "",
        //                    ENDTIME = "",
        //                    EXTPERS_NO = "",
        //                    FORMULA = "",
        //                    FORM_VAL1 = 1,
        //                    FORM_VAL1Specified = true,
        //                    FORM_VAL2 = 1,
        //                    FORM_VAL2Specified = true,
        //                    FORM_VAL3 = 1,
        //                    FORM_VAL3Specified = true,
        //                    FORM_VAL4 = 1,
        //                    FORM_VAL4Specified = true,
        //                    FORM_VAL5 = 1,
        //                    FORM_VAL5Specified = true,
        //                    USERF1_NUM = "",
        //                    USERF2_NUM = 1,
        //                    USERF2_NUMSpecified = true,
        //                    USERF1_TXT = "",
        //                    USERF2_TXT = "",
        //                    HI_LINE_NO = "",
        //                    EXTREFKEY = "",
        //                    DELETE_IND = "",
        //                    PER_SDATE = "",
        //                    PER_EDATE = "",
        //                    EXTERNAL_ITEM_ID = "",
        //                    SERVICE_ITEM_KEY = "",
        //                    NET_VALUE = 1,
        //                    NET_VALUESpecified = true,
        //                };
        //                solpPedidoSAP.IM_SERVICESList.Add(subposicionSap);

        //                var imputacionSubPos = new BAPIESKLC()
        //                {
        //                    PCKG_NO = esPosicionDeMateriales ? "" : numeroPaquete,
        //                    LINE_NO = subposicion.Numero.ToString(),
        //                    SERNO_LINE = "",
        //                    PERCENTAGE = 1,
        //                    PERCENTAGESpecified = true,
        //                    SERIAL_NO = "",
        //                    QUANTITY = 1,
        //                    QUANTITYSpecified = true,
        //                    NET_VALUE = 1,
        //                    NET_VALUESpecified = true,
        //                };

        //                solpPedidoSAP.IM_POSRVACCESSVALUESList.Add(imputacionSubPos);

        //            }

        //        }


        //        //Nombre: ZBAPIMEPOCONDHEADER Denominación:	Posición de Servicio
        //        //solpPedidoSAP.IM_POCONDHEADERList.Add(new ZMPES6850
        //        //{
        //        //    CONDITION_NO = "", //CONDITION_NO    KNUMV Número de la condición de documento
        //        //    ITM_NUMBER = "", //ITM_NUMBER  KPOSN Número de posición de la condición
        //        //    COND_ST_NO = null, //COND_ST_NO  STUNR Número de paso
        //        //    COND_COUNT = null, //COND_COUNT DZAEHK_SHORT    Contador de condiciones(longitud corta)
        //        //    COND_TYPE = null, //COND_TYPE KSCHA   Clase de condición
        //        //    COND_VALUE = 0, //COND_VALUE  BAPIKBETR1 Importe de condición
        //        //    CURRENCY = null, //CURRENCY WAERS   Clase de Moneda
        //        //    CHANGE_ID = null, //CHANGE_ID   MEINS Unidad de medida base
        //        //    CALCTYPCON = null, //CALCTYPCON KRECH   Regla de cálculo para la condición
        //        //    CONDCLASS = null, //CONDCLASS KOAID   Categoría de condición
        //        //});

        //        //solpPedidoSAP.IM_POCONDHEADERXList.Add(new ZMPES6860
        //        //{
        //        //    CONDITION_NO = "X",
        //        //    ITM_NUMBER = "X",
        //        //    COND_ST_NO = "X",
        //        //    COND_COUNT = "X",
        //        //    COND_TYPE = "X",
        //        //    COND_VALUE = "X",
        //        //    CURRENCY = "X",
        //        //    CHANGE_ID = "X",
        //        //    CALCTYPCON = "X",
        //        //    CONDCLASS = "X"
        //        //});

        //        //Nombre: ZBAPIMEPOCOND Denominación:	Posición de Servicio          
        //        //solpPedidoSAP.IM_POCONDList.Add( new ZMPES6870
        //        //{
        //        //    CONDITION_NO = null,  //CONDITION_NO    KNUMV Número de la condición de documento
        //        //    ITM_NUMBER = null, //ITM_NUMBER  KPOSN Número de posición de la condición
        //        //    COND_ST_NO = null, //COND_ST_NO  STUNR Número de paso
        //        //    COND_COUNT = null, //COND_COUNT DZAEHK_SHORT    Contador de condiciones(longitud corta)
        //        //    COND_TYPE = null, //COND_TYPE KSCHA   Clase de condición
        //        //    COND_VALUE = 0, //COND_VALUE  BAPIKBETR1 Importe de condición
        //        //    CURRENCY = null, //CURRENCY WAERS   Clase de Moneda
        //        //    CHANGE_ID = null, //CHANGE_ID   MEINS Unidad de medida base
        //        //    CALCTYPCON = null, //CALCTYPCON KRECH   Regla de cálculo para la condición
        //        //    CONDCLASS = null //CONDCLASS KOAID   Categoría de condición
        //        //});

        //        //solpPedidoSAP.IM_POCONDXList.Add(new ZMPES6880
        //        //{
        //        //    CONDITION_NO = "X",
        //        //    ITM_NUMBER = "X",
        //        //    COND_ST_NO = "X",
        //        //    COND_COUNT = "X",
        //        //    COND_TYPE = "X",
        //        //    COND_VALUE = "X",
        //        //    CURRENCY = "X",
        //        //    CHANGE_ID = "X",
        //        //    CALCTYPCON = "X",
        //        //    CONDCLASS = "X"
        //        //});

        //    }

        //    return solpPedidoSAP;
        //}
        private decimal CalcularPrecioBrutoServicio(SolpPosicion posicion, AdjudicacionPosicion adjudicacionPosicion)
        {
            decimal total = 0;

            foreach (var item in adjudicacionPosicion.CotizacionPosicion.CotizacionSubPosiciones)
            {
                total += item.Cantidad.Value * item.Precio.Value;
            }

            return total;
        }
    }

    public class CrearPedidoConsumerMOAResponse
    {
        public string NumeroPedido { get; set; }
        public List<CrearPedidoConsumerMOAError> Errores { get; set; }
        public string Resultado { get; set; }
    }

    public class CrearPedidoConsumerMOAError
    {
        public string Codigo { get; set; }
        public string Mensaje { get; set; }
        public string Tipo { get; set; }
    }

    public class SolpPedidoSAPDto
    {
        public List<ZMPES6830> IM_POACCOUNTList { get; set; }
        public List<ZMPES6840> IM_POACCOUNTXList { get; set; }
        public List<ZMPES6820> IM_POADDREDELIVERYList { get; set; }
        public List<ZMPES6870> IM_POCONDList { get; set; }
        public List<ZMPES6850> IM_POCONDHEADERList { get; set; }
        public List<ZMPES6860> IM_POCONDHEADERXList { get; set; }
        public List<ZMPES6880> IM_POCONDXList { get; set; }
        public ZMPES6780 IM_POHEADERList { get; set; }
        public ZMPES6790 IM_POHEADERXList { get; set; }
        public List<ZMPES6800> IM_POITEMList { get; set; }
        public List<ZMPES6810> IM_POITEMXList { get; set; }
        public List<BAPIMEPOSCHEDULE> IM_POSCHEDULEList { get; set; }
        public List<BAPIMEPOSCHEDULX> IM_POSCHEDULEXList { get; set; }
        public List<BAPIESKLC> IM_POSRVACCESSVALUESList { get; set; }
        public List<BAPIMEPOTEXTHEADER> IM_POTEXTHEADERList { get; set; }
        public List<BAPIMEPOTEXT> IM_POTEXTITEMList { get; set; }
        public List<BAPIESLLC> IM_SERVICESList { get; set; }
        public string IM_URL { get; set; }


        public SolpPedidoSAPDto()
        {
            IM_POACCOUNTList = new List<ZMPES6830>();
            IM_POACCOUNTXList = new List<ZMPES6840>();
            IM_POADDREDELIVERYList = new List<ZMPES6820>();
            IM_POCONDList = new List<ZMPES6870>();
            IM_POCONDHEADERList = new List<ZMPES6850>();
            IM_POCONDHEADERXList = new List<ZMPES6860>();
            IM_POCONDXList = new List<ZMPES6880>();
            IM_POHEADERList = new ZMPES6780();
            IM_POHEADERXList = new ZMPES6790();
            IM_POITEMList = new List<ZMPES6800>();
            IM_POITEMXList = new List<ZMPES6810>();
            IM_POSCHEDULEList = new List<BAPIMEPOSCHEDULE>();
            IM_POSCHEDULEXList = new List<BAPIMEPOSCHEDULX>();
            IM_POSRVACCESSVALUESList = new List<BAPIESKLC>();
            IM_POTEXTHEADERList = new List<BAPIMEPOTEXTHEADER>();
            IM_POTEXTITEMList = new List<BAPIMEPOTEXT>();
            IM_SERVICESList = new List<BAPIESLLC>();
            IM_URL = "";
        }
    }

    public interface ICrearPedidoConsumerMOA
    {
        CrearPedidoConsumerMOAResponse Request(Adjudicacion adjudicacion);

    }

}
