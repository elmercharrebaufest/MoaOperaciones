using SustitucionMOAFotmatter;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOARepositorio;
using SustitucionMOARepositorio.Extensiones;
using SustitucionMOAWS.CrearPedidoWebServiceMOA;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace SustitucionMOAWS.WSConsumers
{
    public class CrearPedidoConsumerMOA : ICrearPedidoConsumerMOA
    {
        private readonly string rutaArchivosXmls = ConfigurationManager.AppSettings["RutaArchivosCompras"];
        private readonly IObtenerUnidadesDeMedidaAlternativasConsumerMOA obtenerUnidadesDeMedidaConsumerMOA;
        private readonly IObtenerTipoCambioConsumerMOA obtenerTipoCambioConsumerMOA;
        private readonly IRepositorio repositorio;
        private readonly IObtenerRegistroInfoConsumerMOA obtenerRegistroInfoConsumerMOA;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserS4"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassS4"];

        public CrearPedidoConsumerMOA(IObtenerUnidadesDeMedidaAlternativasConsumerMOA _obtenerUnidadesDeMedidaConsumerMOA, IObtenerTipoCambioConsumerMOA _obtenerTipoCambioConsumerMOA, IRepositorio _repositorio, IObtenerRegistroInfoConsumerMOA _obtenerRegistroInfoConsumerMOA)
        {
            obtenerUnidadesDeMedidaConsumerMOA = _obtenerUnidadesDeMedidaConsumerMOA;
            obtenerTipoCambioConsumerMOA = _obtenerTipoCambioConsumerMOA;
            repositorio = _repositorio;
            obtenerRegistroInfoConsumerMOA = _obtenerRegistroInfoConsumerMOA;
        }

        public CrearPedidoConsumerMOAResponse Request(Adjudicacion adjudicacion, bool creadoAutomatico = false)
        {
            var respuesta = new CrearPedidoConsumerMOAResponse();

            if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
            {
                var agent = new Z_WS_MOAOP_COMPRAS_DIRECTClient();
                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;

                var solpPedidoSAP = ConvertirOCSinPI(adjudicacion, creadoAutomatico);
                var serxml = new System.Xml.Serialization.XmlSerializer(solpPedidoSAP.GetType());
                var ms = new MemoryStream();
                serxml.Serialize(ms, solpPedidoSAP);
                string xml = Encoding.UTF8.GetString(ms.ToArray());

                var fecha = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");

                var nombreArchivoLlamada = string.Concat(adjudicacion.Id, " - ", fecha, " - crear pedido.xml");

                var rutaArchivoLlamada = Path.Combine(rutaArchivosXmls, "XMLS", nombreArchivoLlamada);

                FileInfo fileCrear = new FileInfo(rutaArchivoLlamada);
                fileCrear.Directory.Create();
                File.WriteAllText(fileCrear.FullName, xml);

                var request = new Z_MMRFC_CREAR_PEDIDO()
                {
                    IM_POACCOUNT = solpPedidoSAP.IM_POACCOUNTList.ToArray(),
                    IM_POACCOUNTX = solpPedidoSAP.IM_POACCOUNTXList.ToArray(),
                    IM_POADDREDELIVERY = solpPedidoSAP.IM_POADDREDELIVERYList.ToArray(),
                    IM_POCOND = solpPedidoSAP.IM_POCONDList.ToArray(),
                    IM_POCONDHEADER = solpPedidoSAP.IM_POCONDHEADERList.ToArray(),
                    IM_POCONDHEADERX = solpPedidoSAP.IM_POCONDHEADERXList.ToArray(),
                    IM_POCONDX = solpPedidoSAP.IM_POCONDXList.ToArray(),
                    IM_POHEADER = solpPedidoSAP.IM_POHEADERList,
                    IM_POHEADERX = solpPedidoSAP.IM_POHEADERXList,
                    IM_POITEM = solpPedidoSAP.IM_POITEMList.ToArray(),
                    IM_POITEMX = solpPedidoSAP.IM_POITEMXList.ToArray(),
                    IM_POSCHEDULE = solpPedidoSAP.IM_POSCHEDULEList.ToArray(),
                    IM_POSCHEDULEX = solpPedidoSAP.IM_POSCHEDULEXList.ToArray(),
                    IM_POSRVACCESSVALUES = solpPedidoSAP.IM_POSRVACCESSVALUESList.ToArray(),
                    IM_POTEXTHEADER = solpPedidoSAP.IM_POTEXTHEADERList.ToArray(),
                    IM_POTEXTITEM = solpPedidoSAP.IM_POTEXTITEMList.ToArray(),
                    IM_SERVICES = solpPedidoSAP.IM_SERVICESList.ToArray(),
                    IM_URL = solpPedidoSAP.IM_URL
                };
                Log.Info($"SAP sin PI Z_MMRFC_CREAR_PEDIDO request");
                Log.Info(request.ToXml());
                var response = agent.Z_MMRFC_CREAR_PEDIDO(request);
                Log.Info($"SAP sin PI Z_MMRFC_CREAR_PEDIDO response");
                Log.Info(response.ToXml());
                respuesta.NumeroPedido = response.EX_PO_NUMBER;
                respuesta.Resultado = response.EX_EXITO;
                respuesta.Errores = new List<CrearPedidoConsumerMOAError>();

                foreach (var errorSAP in response.EX_RETURN)
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
            else
            {

                SI_MMRFC_CREAR_PEDIDOClient service;
                var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=SI_MMRFC_CREAR_PEDIDO&amp;interfaceNamespace=urn%3AOPERACIONES";
                service = new SI_MMRFC_CREAR_PEDIDOClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();

                var solpPedidoSAP = ConvertirOC(adjudicacion, creadoAutomatico);
                var serxml = new System.Xml.Serialization.XmlSerializer(solpPedidoSAP.GetType());
                var ms = new MemoryStream();
                serxml.Serialize(ms, solpPedidoSAP);
                string xml = Encoding.UTF8.GetString(ms.ToArray());

                var fecha = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");

                var nombreArchivoLlamada = string.Concat(adjudicacion.Id, " - ", fecha, " - crear pedido.xml");

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
                                                            out CrearPedidoWebServiceMOA.BAPIRET2[] EX_RETURN
                                                            );


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


        }

        private SolpPedidoSAPDto ConvertirOC(Adjudicacion adjudicacion, bool creadoAutomatico = false)
        {
            // TODO: Crear OC ConvertirSOLP - fields hardcodeados o para revisar
            ///DOC_TYPE  ok por ahora. Clase de documento de compras / Estrategia de liberacion hardcore ZPE1 

            ///STREET y STREET_NO ok. no tenemos el campo separado mandamos todo en street            
            ///SERIAL_NO/serialNumber siempre 1 por que se imputa todo a lo mismo sino son imputaciones multiples, en ese caso analizar como se envia.

            var proveedorCodigoDeLaAdjudicacion = adjudicacion.Posiciones.First().CotizacionPosicion.Cotizacion.PeticionDeOfertaUsuario.Usuario.ObtenerCodigoProveedor();
            var usuarioCreadorAdjudicacion = adjudicacion.Usuario.UsuarioSap;
            var usuarioOrganizacionDeCompra = adjudicacion.Usuario.OrganizacionDeCompra;

            SolpPedidoSAPDto solpPedidoSAP = new SolpPedidoSAPDto();

            var poItem = 0;
            var PCKG_NO = 1000;
            var numeroDePaquete = 1;
            bool esPosicionDeMateriales = adjudicacion.Posiciones.First().Posicion.TipoPosicion.Codigo == "MATERIALES";
            var unidadesDeMedidaSAP = new List<UnidadesDeMedida>();
            var fecha = DateTime.Now;
            var posicionAdjudicacion = adjudicacion.Posiciones.Select(x => x.SolpPosicion_Id);
            var posicionesSolp = repositorio.Listar<SolpPosicion>(posi => posicionAdjudicacion.Contains(posi.Id));

            if (esPosicionDeMateriales)
            {
                unidadesDeMedidaSAP = obtenerUnidadesDeMedidaConsumerMOA.Request(posicionesSolp.Select(x => x.MaterialSolp?.Codigo).ToList());
            }

            var unidadesCodigoSap = adjudicacion.Posiciones
                .SelectMany(p => new[] { p.CotizacionPosicion.UnidadDeMedida?.CodigoSap }
                    .Concat(p.CotizacionPosicion.CotizacionSubPosiciones?.Select(sp => sp.UnidadDeMedida?.CodigoSap) ?? Enumerable.Empty<string>()))
                .Where(codigo => codigo != null)
                .Distinct();


            var unidadesMedidaSap = repositorio.Listar<UnidadMedidaSap>().ToList();

            foreach (var solpPosicion in posicionesSolp.OrderBy(x => x.Id))
            {
                var adjudicacionPosicion = adjudicacion.Posiciones.Single(a => a.CotizacionPosicion.PeticionDeOfertaSolpPosicion.SolpPosicion_Id == solpPosicion.Id);
                decimal precioConvertido = adjudicacionPosicion.Monto ?? 0;
                decimal nuevaCantidad = adjudicacionPosicion.Cantidad;
                string unidadDeMedida = esPosicionDeMateriales ? unidadesMedidaSap?.Find(u => u.Comercial == solpPosicion.Unidad.CodigoSap).UM : "001";
                if (esPosicionDeMateriales && solpPosicion.MaterialSolp != null && !string.IsNullOrWhiteSpace(solpPosicion.MaterialSolp.Codigo))
                {
                    var registros = obtenerRegistroInfoConsumerMOA.ObtenerRegistroInfoConsumer(solpPosicion.MaterialSolp.Codigo, solpPosicion.Centro.Codigo, solpPosicion.GrupoCompras.Codigo, proveedorCodigoDeLaAdjudicacion)
                                                   /*.Where(x => x.NumeroOrdenDeCompra != null).OrderByDescending(x => x.FechaUltimaCompra)*/;

                    if (registros.Any())
                    {
                        var ultimoRegistroInfo = registros[0];
                        precioConvertido = ultimoRegistroInfo.Precio;
                        if (solpPosicion.Unidad.CodigoSap != ultimoRegistroInfo.Unidad)
                        {
                            var unidadesDelMaterial = unidadesDeMedidaSAP.Where(x => x.CodigoMaterial == solpPosicion.MaterialSolp.Codigo).ToList();
                            var unidadRegistroInfo = unidadesDelMaterial.First(x => x.UnidadDeMedida == ultimoRegistroInfo.Unidad);
                            unidadDeMedida = unidadRegistroInfo.UnidadDeMedida;

                            nuevaCantidad = adjudicacionPosicion.Cantidad * unidadRegistroInfo.Denominador / unidadRegistroInfo.Numerador;
                        }
                        if (adjudicacion.Moneda.Codigo != ultimoRegistroInfo.Moneda)
                        {
                            precioConvertido = precioConvertido * obtenerTipoCambioConsumerMOA.Request(fecha.ToString("yyyy-MM-dd"), adjudicacion.Moneda.Codigo, ultimoRegistroInfo.Moneda).TipoCambio;
                        }
                    }
                    else
                    {
                        if (solpPosicion.Unidad.CodigoSap != adjudicacionPosicion.CotizacionPosicion.UnidadDeMedida.CodigoSap)
                        {
                            var unidadesDelMaterial = unidadesDeMedidaSAP.Where(x => x.CodigoMaterial == solpPosicion.MaterialSolp.Codigo).ToList();
                            var unidadCotizacion = unidadesDelMaterial.First(x => x.UnidadDeMedida == adjudicacionPosicion.CotizacionPosicion.UnidadDeMedida.CodigoSap);

                            precioConvertido = precioConvertido * unidadCotizacion.Denominador / unidadCotizacion.Numerador;
                        }
                        if (adjudicacion.Moneda.Codigo != adjudicacionPosicion.CotizacionPosicion.Moneda.Codigo)
                        {
                            precioConvertido = precioConvertido * obtenerTipoCambioConsumerMOA.Request(fecha.ToString("yyyy-MM-dd"), adjudicacion.Moneda.Codigo, adjudicacionPosicion.CotizacionPosicion.Moneda.Codigo).TipoCambio;
                        }

                    }

                }


                poItem++;
                numeroDePaquete++;

                //Nombre: ZBAPIMEPOHEADER Denominación:	Cabecera del Pedido de Compras
                var cabeceraDelPedido = new CrearPedidoWebServiceMOA.BAPIMEPOHEADER();
                cabeceraDelPedido.COMP_CODE = "MOA"; //COMP_CODE BUKRS   Sociedad
                cabeceraDelPedido.DOC_TYPE = "ZPE1";//solp.ClaseDocumento.CodigoSap; //DOC_TYPE    ESART Clase de documento de compras
                cabeceraDelPedido.VENDOR = proveedorCodigoDeLaAdjudicacion;//VENDOR ELIFN   Número de cuenta del proveedor
                cabeceraDelPedido.PURCH_ORG = usuarioOrganizacionDeCompra;//PURCH_ORG EKORG   Organización de compras
                cabeceraDelPedido.PUR_GROUP = solpPosicion.GrupoCompras.CodigoSap.ToString(); //PUR_GROUP   BKGRP Grupo de compras
                cabeceraDelPedido.CURRENCY = adjudicacion.Moneda.Codigo; //CURRENCY WAERS   Clave de moneda
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
                solpPedidoSAP.IM_POHEADERXList = new CrearPedidoWebServiceMOA.BAPIMEPOHEADERX
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
                var IM_POITEM = new CrearPedidoWebServiceMOA.BAPIMEPOITEM();

                IM_POITEM.PO_ITEM = $"{poItem:00000}";
                IM_POITEM.SHORT_TEXT = solpPosicion.Tarea;
                IM_POITEM.PLANT = solpPosicion.Centro.CodigoSap.ToString();
                IM_POITEM.MATL_GROUP = solpPosicion.GrupoArticulo?.CodigoSap?.ToString() ?? "";
                IM_POITEM.MATERIAL = esPosicionDeMateriales ? solpPosicion.MaterialSolp?.CodigoSap.ToString() : "";
                IM_POITEM.STGE_LOC = solpPosicion.Almacen != null ? solpPosicion.Almacen.CodigoSap.ToString() : "";
                IM_POITEM.ITEM_CAT = solpPosicion.TipoPosicion.Codigo.ToLower() == "servicio" ? "9" : "0";//ITEM_CAT PSTYP   Tipo de posición del documento de compras
                IM_POITEM.TRACKINGNO = solpPosicion.NroNecesidad;
                IM_POITEM.INFO_REC = "";
                IM_POITEM.QUANTITY = esPosicionDeMateriales ? nuevaCantidad : 0;
                IM_POITEM.QUANTITYSpecified = esPosicionDeMateriales;
                IM_POITEM.PO_UNIT = unidadDeMedida;
                IM_POITEM.NET_PRICE = esPosicionDeMateriales ? precioConvertido : adjudicacionPosicion.Monto.Value;
                IM_POITEM.NET_PRICESpecified = true;
                IM_POITEM.PRICE_UNIT = 1;
                IM_POITEM.PRICE_UNITSpecified = true;
                IM_POITEM.GR_PR_TIME = 0;
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
                IM_POITEM.PREQ_NO = solpPosicion.Solp.NroSolp;
                IM_POITEM.PREQ_ITEM = $"{solpPosicion.Indice ?? 0:00000}";
                IM_POITEM.PCKG_NO = esPosicionDeMateriales ? "" : $"{numeroDePaquete:0000000000}";

                solpPedidoSAP.IM_POITEMList.Add(IM_POITEM);

                solpPedidoSAP.IM_POITEMXList.Add(new CrearPedidoWebServiceMOA.BAPIMEPOITEMX
                {
                    PO_ITEM = $"{poItem:00000}",
                    DELETE_IND = "",
                    SHORT_TEXT = "X",
                    MATERIAL = "X",
                    PLANT = "X",
                    STGE_LOC = "X",
                    TRACKINGNO = string.IsNullOrEmpty(solpPosicion.NroNecesidad) ? "" : "X",
                    MATL_GROUP = "X",
                    INFO_REC = "",
                    QUANTITY = (IM_POITEM.QUANTITY == 0) ? "" : "X",
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

                solpPedidoSAP.IM_POCONDList.Add(new CrearPedidoWebServiceMOA.BAPIMEPOCOND
                {
                    ITM_NUMBER = $"{poItem:00000}",  //el número de ítem al que corresponda la condición
                    COND_TYPE = creadoAutomatico ? "ZP00" : "ZP01",
                    //ZP00 toma los datos del registro info
                    //ZP01 toma los datos de la adjudicacion
                    COND_VALUE = IM_POITEM.NET_PRICE, //el importe de la condición
                    COND_VALUESpecified = true,
                    CURRENCY = adjudicacion.Moneda.Codigo /*adjudicacionPosicion.CotizacionPosicion.Moneda.Codigo*/,//moneda de la adjudicacion
                    CHANGE_ID = "U",// siempra va el mismo valor

                });
                solpPedidoSAP.IM_POCONDXList.Add(new CrearPedidoWebServiceMOA.BAPIMEPOCONDX
                {
                    ITM_NUMBER = $"{poItem:00000}",
                    COND_TYPE = "X",
                    COND_VALUE = "X",
                    CURRENCY = "X",
                    CHANGE_ID = "X",
                });

                if (esPosicionDeMateriales)
                {
                    //Nombre: ZBAPIMEPOACCOUNT IM_POACCOUNT Denominación:	Imputación
                    var imputacion = new CrearPedidoWebServiceMOA.BAPIMEPOACCOUNT();
                    imputacion.PO_ITEM = $"{poItem:00000}";
                    imputacion.SERIAL_NO = "01";
                    imputacion.GL_ACCOUNT = ObtenerCuentaMayor(esPosicionDeMateriales, solpPosicion);
                    imputacion.QUANTITY = esPosicionDeMateriales ? nuevaCantidad : 0;
                    imputacion.QUANTITYSpecified = imputacion.QUANTITY > 0;
                    imputacion.BUS_AREA = "GENE";
                    imputacion.CO_AREA = "MOA";
                    imputacion.COSTCENTER = ObtenerImputacion(esPosicionDeMateriales, solpPosicion, new List<string> { "centrodecosto" });
                    imputacion.ORDERID = ObtenerImputacion(esPosicionDeMateriales, solpPosicion, new List<string> { "ordendeot", "ordendeinversion" });
                    imputacion.PROFIT_CTR = "";
                    imputacion.SUB_NUMBER = "";
                    imputacion.ASSET_NO = "";
                    imputacion.COSTOBJECT = "";
                    imputacion.DELETE_IND = "";
                    solpPedidoSAP.IM_POACCOUNTList.Add(imputacion);

                    solpPedidoSAP.IM_POACCOUNTXList.Add(new CrearPedidoWebServiceMOA.BAPIMEPOACCOUNTX
                    {
                        PO_ITEM = $"{poItem:00000}",
                        SERIAL_NO = "01",
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
                        PROFIT_CTR = ""
                    });
                }

                //Nombre: ZBAPIMEPOADDREDELIVERY Denominación:	Direcciones de entrega
                solpPedidoSAP.IM_POADDREDELIVERYList.Add(new CrearPedidoWebServiceMOA.BAPIMEPOADDRDELIVERY
                {
                    PO_ITEM = $"{poItem:00000}",
                    POSTL_COD1 = solpPosicion.CpEntrega,
                    CITY = solpPosicion.Centro.Descripcion,
                    ADDR_NO = "",
                    NAME = solpPosicion.NombreEntrega,
                    TEL1_NUMBR = "",
                    STREET = solpPosicion.CalleEntrega,
                    STREET_NO = "",//no tenemos el campo separado en calle y altura
                    REGION = adjudicacion.RegionSap.CodigoSap
                });

                //subposiciones
                if (!esPosicionDeMateriales)
                {
                    var LINE_NO = 1;
                    //cabecera de subposiciones 
                    var cabeceraSubPos = new CrearPedidoWebServiceMOA.BAPIESLLC
                    {
                        PCKG_NO = $"{numeroDePaquete:0000000000}",
                        LINE_NO = $"{LINE_NO++:0000000000}",
                        OUTL_IND = "X",
                        OUTL_LEVEL = 0,
                        SUBPCKG_NO = $"{PCKG_NO:0000000000}",
                    };
                    solpPedidoSAP.IM_SERVICESList.Add(cabeceraSubPos);

                    int numeroDeImputacion = 0;
                    foreach (var subposicion in solpPosicion.Subposiciones)
                    {
                        CotizacionSubPosicion cotizacionSubPosicion = adjudicacionPosicion.CotizacionPosicion.CotizacionSubPosiciones.Single(a => a.SolpSubPosicion_Id == subposicion.Id);

                        var subposicionSap = new CrearPedidoWebServiceMOA.BAPIESLLC();
                        subposicionSap.PCKG_NO = $"{PCKG_NO:0000000000}";
                        subposicionSap.LINE_NO = $"{LINE_NO:0000000000}";
                        subposicionSap.EXT_LINE = $"{LINE_NO * 10:0000000000}";
                        subposicionSap.SERVICE = subposicion.ServicioSolp?.Codigo;
                        subposicionSap.SHORT_TEXT = subposicion.Tarea;
                        subposicionSap.QUANTITY = cotizacionSubPosicion.Cantidad.Value;
                        subposicionSap.QUANTITYSpecified = true;
                        subposicionSap.BASE_UOM = unidadesMedidaSap.Find(u => u.Comercial == cotizacionSubPosicion.UnidadDeMedida.CodigoSap).UM;
                        subposicionSap.UOM_ISO = unidadesMedidaSap.Find(u => u.Comercial == cotizacionSubPosicion.UnidadDeMedida.CodigoSap).UM;
                        subposicionSap.PRICE_UNIT = 1;
                        subposicionSap.PRICE_UNITSpecified = true;
                        subposicionSap.GR_PRICE = cotizacionSubPosicion.Precio.Value;
                        if (adjudicacion.Moneda.Codigo != cotizacionSubPosicion.Moneda.Codigo)
                        {
                            subposicionSap.GR_PRICE = cotizacionSubPosicion.Precio.Value * obtenerTipoCambioConsumerMOA.Request(fecha.ToString("yyyy-MM-dd"), adjudicacion.Moneda.Codigo, cotizacionSubPosicion.Moneda.Codigo).TipoCambio;
                        }

                        subposicionSap.GR_PRICESpecified = true;

                        solpPedidoSAP.IM_SERVICESList.Add(subposicionSap);


                        if (!solpPedidoSAP.IM_POACCOUNTList.Exists(x =>
                                x.PO_ITEM == $"{poItem:00000}" &&
                                x.GL_ACCOUNT == getCodigoTablaSap(subposicion.CuentaMayorSap) &&
                                x.COSTCENTER == getCodigoTablaSap(subposicion.TipoImputacionSap) &&
                                x.ORDERID == getCodigoTablaSap(subposicion.TipoImputacionSap) &&
                                x.PROFIT_CTR == ""
                            ))
                        {

                            var imputacion = new CrearPedidoWebServiceMOA.BAPIMEPOACCOUNT();
                            numeroDeImputacion++;
                            imputacion.PO_ITEM = $"{poItem:00000}";
                            imputacion.SERIAL_NO = $"{numeroDeImputacion:00}";
                            imputacion.GL_ACCOUNT = ObtenerCuentaMayor(esPosicionDeMateriales, solpPosicion);
                            imputacion.QUANTITY = subposicion.Cantidad.Value;
                            imputacion.QUANTITYSpecified = true;
                            imputacion.BUS_AREA = "GENE";
                            imputacion.CO_AREA = "MOA";
                            imputacion.COSTCENTER = (getCodigoTablaGeneral(solpPosicion.TipoImputacion).ToLower() == "centrodecosto") ?
                                getCodigoTablaSap(subposicion.TipoImputacionSap) : "";
                            imputacion.ORDERID = (getCodigoTablaGeneral(solpPosicion.TipoImputacion).ToLower() == "ordendeot" || getCodigoTablaGeneral(solpPosicion.TipoImputacion).ToLower() == "ordendeinversion") ?
                                getCodigoTablaSap(subposicion.TipoImputacionSap) : "";
                            imputacion.PROFIT_CTR = "";
                            imputacion.SUB_NUMBER = "";
                            imputacion.ASSET_NO = "";
                            imputacion.COSTOBJECT = "";
                            imputacion.DELETE_IND = "";
                            solpPedidoSAP.IM_POACCOUNTList.Add(imputacion);

                            solpPedidoSAP.IM_POACCOUNTXList.Add(new CrearPedidoWebServiceMOA.BAPIMEPOACCOUNTX
                            {
                                PO_ITEM = $"{poItem:00000}",
                                SERIAL_NO = $"{numeroDeImputacion:00}",
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
                                PROFIT_CTR = ""
                            });

                            var imputacionSubPos = new CrearPedidoWebServiceMOA.BAPIESKLC()
                            {
                                PCKG_NO = $"{PCKG_NO:0000000000}",
                                LINE_NO = $"{LINE_NO++:0000000000}",
                                PERCENTAGE = 100,
                                PERCENTAGESpecified = true,
                                SERNO_LINE = $"{poItem:00}",
                                SERIAL_NO = $"{numeroDeImputacion:00}",
                            };
                            solpPedidoSAP.IM_POSRVACCESSVALUESList.Add(imputacionSubPos);
                        }
                        else
                        {
                            var imputacionUsada = solpPedidoSAP.IM_POACCOUNTList.Find(x =>
                                x.PO_ITEM == $"{poItem:00000}" &&
                                x.GL_ACCOUNT == getCodigoTablaSap(subposicion.CuentaMayorSap) &&
                                x.COSTCENTER == getCodigoTablaSap(subposicion.TipoImputacionSap) &&
                                x.ORDERID == getCodigoTablaSap(subposicion.TipoImputacionSap) &&
                                x.PROFIT_CTR == ""
                                );
                            imputacionUsada.QUANTITY += subposicion.Cantidad.Value;

                            var imputacionSubPos = new CrearPedidoWebServiceMOA.BAPIESKLC()
                            {
                                PCKG_NO = $"{PCKG_NO:0000000000}",
                                LINE_NO = $"{LINE_NO++:0000000000}",
                                PERCENTAGE = 100,
                                PERCENTAGESpecified = true,
                                SERNO_LINE = $"{poItem:00}",
                                SERIAL_NO = $"{imputacionUsada.SERIAL_NO:00}",
                            };
                            solpPedidoSAP.IM_POSRVACCESSVALUESList.Add(imputacionSubPos);
                        }




                    }
                    PCKG_NO++;
                }

                solpPedidoSAP.IM_POSCHEDULEList.Add(new CrearPedidoWebServiceMOA.BAPIMEPOSCHEDULE
                {
                    DELIVERY_DATE = adjudicacionPosicion.PlazoDeEntrega.ToString("dd.MM.yyyy"),
                    PO_ITEM = $"{poItem:00000}",
                    SCHED_LINE = "1"
                });

                solpPedidoSAP.IM_POSCHEDULEXList.Add(new CrearPedidoWebServiceMOA.BAPIMEPOSCHEDULX
                {
                    DELIVERY_DATE = "X",
                    PO_ITEM = $"{poItem:00000}",
                    SCHED_LINE = "1"
                });
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
                        solpPedidoSAP.IM_POTEXTHEADERList.Add(new CrearPedidoWebServiceMOA.BAPIMEPOTEXTHEADER
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

            if (adjudicacion.Posiciones.First().Posicion.Solp.Urgencia == true)
            {
                solpPedidoSAP.IM_POTEXTITEMList.Add(new CrearPedidoWebServiceMOA.BAPIMEPOTEXT
                {
                    TEXT_ID = "F12",
                    PO_NUMBER = "",
                    PO_ITEM = $"{poItem:00000}",
                    TEXT_FORM = "*",
                    TEXT_LINE = "Urgencia"
                });
            }

            solpPedidoSAP.IM_URL = ConfigurationManager.AppSettings["SpaUrl"] + "/verLegajoOrdenDeCompra/" + adjudicacion.Id + "/" + adjudicacion.Token;

            return solpPedidoSAP;
        }


        private SolpPedidoSAPSinPIDto ConvertirOCSinPI(Adjudicacion adjudicacion, bool creadoAutomatico = false)
        {
            // TODO: Crear OC ConvertirSOLP - fields hardcodeados o para revisar
            ///DOC_TYPE  ok por ahora. Clase de documento de compras / Estrategia de liberacion hardcore ZPE1 

            ///STREET y STREET_NO ok. no tenemos el campo separado mandamos todo en street            
            ///SERIAL_NO/serialNumber siempre 1 por que se imputa todo a lo mismo sino son imputaciones multiples, en ese caso analizar como se envia.


            var proveedorCodigoDeLaAdjudicacion = adjudicacion.Posiciones.First().CotizacionPosicion.Cotizacion.PeticionDeOfertaUsuario.Usuario.ObtenerCodigoProveedor();
            var usuarioCreadorAdjudicacion = adjudicacion.Usuario.UsuarioSap;
            var usuarioOrganizacionDeCompra = adjudicacion.Usuario.OrganizacionDeCompra;

            SolpPedidoSAPSinPIDto solpPedidoSAP = new SolpPedidoSAPSinPIDto();

            var poItem = 0;
            var PCKG_NO = 1000;
            var numeroDePaquete = 1;
            bool esPosicionDeMateriales = adjudicacion.Posiciones.First().Posicion.TipoPosicion.Codigo == "MATERIALES";
            var unidadesDeMedidaSAP = new List<UnidadesDeMedida>();
            var fecha = DateTime.Now;
            var posicionAdjudicacion = adjudicacion.Posiciones.Select(x => x.SolpPosicion_Id);
            var posicionesSolp = repositorio.Listar<SolpPosicion>(posi => posicionAdjudicacion.Contains(posi.Id));

            if (esPosicionDeMateriales)
            {
                unidadesDeMedidaSAP = obtenerUnidadesDeMedidaConsumerMOA.Request(posicionesSolp.Select(x => x.MaterialSolp?.Codigo).ToList());
            }

            var unidadesCodigoSap = adjudicacion.Posiciones
                .SelectMany(p => new[] { p.CotizacionPosicion.UnidadDeMedida?.CodigoSap }
                    .Concat(p.CotizacionPosicion.CotizacionSubPosiciones?.Select(sp => sp.UnidadDeMedida?.CodigoSap) ?? Enumerable.Empty<string>()))
                .Where(codigo => codigo != null)
                .Distinct();


            var unidadesMedidaSap = repositorio.Listar<UnidadMedidaSap>().ToList();

            foreach (var solpPosicion in posicionesSolp.OrderBy(x => x.Id))
            {
                var adjudicacionPosicion = adjudicacion.Posiciones.Single(a => a.CotizacionPosicion.PeticionDeOfertaSolpPosicion.SolpPosicion_Id == solpPosicion.Id);
                decimal precioConvertido = adjudicacionPosicion.Monto ?? 0;
                decimal nuevaCantidad = adjudicacionPosicion.Cantidad;
                string unidadDeMedida = esPosicionDeMateriales ? unidadesMedidaSap?.Find(u => u.Comercial == solpPosicion.Unidad.CodigoSap).UM : "001";
                if (esPosicionDeMateriales && solpPosicion.MaterialSolp != null && !string.IsNullOrWhiteSpace(solpPosicion.MaterialSolp.Codigo))
                {
                    var registros = obtenerRegistroInfoConsumerMOA.ObtenerRegistroInfoConsumer(solpPosicion.MaterialSolp.Codigo, solpPosicion.Centro.Codigo, solpPosicion.GrupoCompras.Codigo, proveedorCodigoDeLaAdjudicacion)
                                                   /*.Where(x => x.NumeroOrdenDeCompra != null).OrderByDescending(x => x.FechaUltimaCompra)*/;

                    if (registros.Any())
                    {
                        var ultimoRegistroInfo = registros[0];
                        precioConvertido = ultimoRegistroInfo.Precio;
                        if (solpPosicion.Unidad.CodigoSap != ultimoRegistroInfo.Unidad)
                        {
                            var unidadesDelMaterial = unidadesDeMedidaSAP.Where(x => x.CodigoMaterial == solpPosicion.MaterialSolp.Codigo).ToList();
                            var unidadRegistroInfo = unidadesDelMaterial.First(x => x.UnidadDeMedida == ultimoRegistroInfo.Unidad);
                            unidadDeMedida = unidadRegistroInfo.UnidadDeMedida;

                            nuevaCantidad = adjudicacionPosicion.Cantidad * unidadRegistroInfo.Denominador / unidadRegistroInfo.Numerador;
                        }
                        if (adjudicacion.Moneda.Codigo != ultimoRegistroInfo.Moneda)
                        {
                            precioConvertido = precioConvertido * obtenerTipoCambioConsumerMOA.Request(fecha.ToString("yyyy-MM-dd"), adjudicacion.Moneda.Codigo, ultimoRegistroInfo.Moneda).TipoCambio;
                        }
                    }
                    else
                    {
                        if (solpPosicion.Unidad.CodigoSap != adjudicacionPosicion.CotizacionPosicion.UnidadDeMedida.CodigoSap)
                        {
                            var unidadesDelMaterial = unidadesDeMedidaSAP.Where(x => x.CodigoMaterial == solpPosicion.MaterialSolp.Codigo).ToList();
                            var unidadCotizacion = unidadesDelMaterial.First(x => x.UnidadDeMedida == adjudicacionPosicion.CotizacionPosicion.UnidadDeMedida.CodigoSap);

                            precioConvertido = precioConvertido * unidadCotizacion.Denominador / unidadCotizacion.Numerador;
                        }
                        if (adjudicacion.Moneda.Codigo != adjudicacionPosicion.CotizacionPosicion.Moneda.Codigo)
                        {
                            precioConvertido = precioConvertido * obtenerTipoCambioConsumerMOA.Request(fecha.ToString("yyyy-MM-dd"), adjudicacion.Moneda.Codigo, adjudicacionPosicion.CotizacionPosicion.Moneda.Codigo).TipoCambio;
                        }

                    }

                }


                poItem++;
                numeroDePaquete++;

                //Nombre: ZBAPIMEPOHEADER Denominación:	Cabecera del Pedido de Compras
                var cabeceraDelPedido = new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOHEADER();
                cabeceraDelPedido.COMP_CODE = "MOA"; //COMP_CODE BUKRS   Sociedad
                cabeceraDelPedido.DOC_TYPE = "ZPE1";//solp.ClaseDocumento.CodigoSap; //DOC_TYPE    ESART Clase de documento de compras
                cabeceraDelPedido.VENDOR = proveedorCodigoDeLaAdjudicacion;//VENDOR ELIFN   Número de cuenta del proveedor
                cabeceraDelPedido.PURCH_ORG = usuarioOrganizacionDeCompra;//PURCH_ORG EKORG   Organización de compras
                cabeceraDelPedido.PUR_GROUP = solpPosicion.GrupoCompras.CodigoSap.ToString(); //PUR_GROUP   BKGRP Grupo de compras
                cabeceraDelPedido.CURRENCY = adjudicacion.Moneda.Codigo; //CURRENCY WAERS   Clave de moneda
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
                solpPedidoSAP.IM_POHEADERXList = new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOHEADERX
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
                var IM_POITEM = new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOITEM();

                IM_POITEM.PO_ITEM = $"{poItem:00000}";
                IM_POITEM.SHORT_TEXT = solpPosicion.Tarea;
                IM_POITEM.PLANT = solpPosicion.Centro.CodigoSap.ToString();
                IM_POITEM.MATL_GROUP = solpPosicion.GrupoArticulo?.CodigoSap?.ToString() ?? "";
                IM_POITEM.MATERIAL = esPosicionDeMateriales ? solpPosicion.MaterialSolp?.CodigoSap.ToString() : "";
                IM_POITEM.STGE_LOC = solpPosicion.Almacen != null ? solpPosicion.Almacen.CodigoSap.ToString() : "";
                IM_POITEM.ITEM_CAT = solpPosicion.TipoPosicion.Codigo.ToLower() == "servicio" ? "9" : "0";//ITEM_CAT PSTYP   Tipo de posición del documento de compras
                IM_POITEM.TRACKINGNO = solpPosicion.NroNecesidad;
                IM_POITEM.INFO_REC = "";
                IM_POITEM.QUANTITY = esPosicionDeMateriales ? nuevaCantidad : 0;
                //IM_POITEM.QUANTITYSpecified = esPosicionDeMateriales;
                IM_POITEM.PO_UNIT = unidadDeMedida;
                IM_POITEM.NET_PRICE = Math.Round((esPosicionDeMateriales ? precioConvertido : adjudicacionPosicion.Monto.Value), 4);
                //IM_POITEM.NET_PRICESpecified = true;
                IM_POITEM.PRICE_UNIT = 1;
                //IM_POITEM.PRICE_UNITSpecified = true;
                IM_POITEM.GR_PR_TIME = 0;
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
                IM_POITEM.PREQ_NO = solpPosicion.Solp.NroSolp;
                IM_POITEM.PREQ_ITEM = $"{solpPosicion.Indice ?? 0:00000}";
                IM_POITEM.PCKG_NO = esPosicionDeMateriales ? "" : $"{numeroDePaquete:0000000000}";

                solpPedidoSAP.IM_POITEMList.Add(IM_POITEM);

                solpPedidoSAP.IM_POITEMXList.Add(new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOITEMX
                {
                    PO_ITEM = $"{poItem:00000}",
                    DELETE_IND = "",
                    SHORT_TEXT = "X",
                    MATERIAL = "X",
                    PLANT = "X",
                    STGE_LOC = "X",
                    TRACKINGNO = string.IsNullOrEmpty(solpPosicion.NroNecesidad) ? "" : "X",
                    MATL_GROUP = "X",
                    INFO_REC = "",
                    QUANTITY = (IM_POITEM.QUANTITY == 0) ? "" : "X",
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

                solpPedidoSAP.IM_POCONDList.Add(new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCOND
                {
                    ITM_NUMBER = $"{poItem:00000}",  //el número de ítem al que corresponda la condición
                    COND_TYPE = creadoAutomatico ? "ZP00" : "ZP01",
                    //ZP00 toma los datos del registro info
                    //ZP01 toma los datos de la adjudicacion
                    COND_VALUE = Math.Round(IM_POITEM.NET_PRICE, 4), //el importe de la condición
                    //COND_VALUESpecified = true,
                    CURRENCY = adjudicacion.Moneda.Codigo /*adjudicacionPosicion.CotizacionPosicion.Moneda.Codigo*/,//moneda de la adjudicacion
                    CHANGE_ID = "U",// siempra va el mismo valor

                });
                solpPedidoSAP.IM_POCONDXList.Add(new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCONDX
                {
                    ITM_NUMBER = $"{poItem:00000}",
                    COND_TYPE = "X",
                    COND_VALUE = "X",
                    CURRENCY = "X",
                    CHANGE_ID = "X",
                });

                if (esPosicionDeMateriales)
                {
                    //Nombre: ZBAPIMEPOACCOUNT IM_POACCOUNT Denominación:	Imputación
                    var imputacion = new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOACCOUNT();
                    imputacion.PO_ITEM = $"{poItem:00000}";
                    imputacion.SERIAL_NO = "01";
                    imputacion.GL_ACCOUNT = ObtenerCuentaMayor(esPosicionDeMateriales, solpPosicion);
                    imputacion.QUANTITY = esPosicionDeMateriales ? nuevaCantidad : 0;
                    //imputacion.QUANTITYSpecified = imputacion.QUANTITY > 0;
                    imputacion.BUS_AREA = "GENE";
                    imputacion.CO_AREA = "MOA";
                    imputacion.COSTCENTER = ObtenerImputacion(esPosicionDeMateriales, solpPosicion, new List<string> { "centrodecosto" });
                    imputacion.ORDERID = ObtenerImputacion(esPosicionDeMateriales, solpPosicion, new List<string> { "ordendeot", "ordendeinversion" });
                    imputacion.PROFIT_CTR = "";
                    imputacion.SUB_NUMBER = "";
                    imputacion.ASSET_NO = "";
                    imputacion.COSTOBJECT = "";
                    imputacion.DELETE_IND = "";
                    solpPedidoSAP.IM_POACCOUNTList.Add(imputacion);

                    solpPedidoSAP.IM_POACCOUNTXList.Add(new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOACCOUNTX
                    {
                        PO_ITEM = $"{poItem:00000}",
                        SERIAL_NO = "01",
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
                        PROFIT_CTR = ""
                    });
                }

                //Nombre: ZBAPIMEPOADDREDELIVERY Denominación:	Direcciones de entrega
                solpPedidoSAP.IM_POADDREDELIVERYList.Add(new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOADDRDELIVERY
                {
                    PO_ITEM = $"{poItem:00000}",
                    POSTL_COD1 = solpPosicion.CpEntrega,
                    CITY = solpPosicion.Centro.Descripcion,
                    ADDR_NO = "",
                    NAME = solpPosicion.NombreEntrega,
                    TEL1_NUMBR = "",
                    STREET = solpPosicion.CalleEntrega,
                    STREET_NO = "",//no tenemos el campo separado en calle y altura
                    REGION = adjudicacion.RegionSap.CodigoSap
                });

                //subposiciones
                if (!esPosicionDeMateriales)
                {
                    var LINE_NO = 1;
                    //cabecera de subposiciones 
                    var cabeceraSubPos = new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIESLLC
                    {
                        PCKG_NO = $"{numeroDePaquete:0000000000}",
                        LINE_NO = $"{LINE_NO++:0000000000}",
                        OUTL_IND = "X",
                        OUTL_LEVEL = 0,
                        SUBPCKG_NO = $"{PCKG_NO:0000000000}",
                    };
                    solpPedidoSAP.IM_SERVICESList.Add(cabeceraSubPos);

                    int numeroDeImputacion = 0;
                    foreach (var subposicion in solpPosicion.Subposiciones)
                    {
                        CotizacionSubPosicion cotizacionSubPosicion = adjudicacionPosicion.CotizacionPosicion.CotizacionSubPosiciones.Single(a => a.SolpSubPosicion_Id == subposicion.Id);

                        var subposicionSap = new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIESLLC();
                        subposicionSap.PCKG_NO = $"{PCKG_NO:0000000000}";
                        subposicionSap.LINE_NO = $"{LINE_NO:0000000000}";
                        subposicionSap.EXT_LINE = $"{LINE_NO * 10:0000000000}";
                        subposicionSap.SERVICE = subposicion.ServicioSolp?.Codigo;
                        subposicionSap.SHORT_TEXT = subposicion.Tarea;
                        subposicionSap.QUANTITY = cotizacionSubPosicion.Cantidad.Value;
                        //ubposicionSap.QUANTITYSpecified = true;
                        subposicionSap.BASE_UOM = unidadesMedidaSap.Find(u => u.Comercial == cotizacionSubPosicion.UnidadDeMedida.CodigoSap).UM;
                        subposicionSap.UOM_ISO = unidadesMedidaSap.Find(u => u.Comercial == cotizacionSubPosicion.UnidadDeMedida.CodigoSap).UM;
                        subposicionSap.PRICE_UNIT = 1;
                        //subposicionSap.PRICE_UNITSpecified = true;
                        subposicionSap.GR_PRICE = Math.Round(cotizacionSubPosicion.Precio.Value, 4);
                        if (adjudicacion.Moneda.Codigo != cotizacionSubPosicion.Moneda.Codigo)
                        {
                            subposicionSap.GR_PRICE = cotizacionSubPosicion.Precio.Value * obtenerTipoCambioConsumerMOA.Request(fecha.ToString("yyyy-MM-dd"), adjudicacion.Moneda.Codigo, cotizacionSubPosicion.Moneda.Codigo).TipoCambio;
                        }
                        subposicionSap.GR_PRICE = Math.Round(subposicionSap.GR_PRICE, 4);

                        //subposicionSap.GR_PRICESpecified = true;

                        subposicionSap.BEGINTIME = "00:00:00";
                        subposicionSap.ENDTIME = "00:00:00";

                        solpPedidoSAP.IM_SERVICESList.Add(subposicionSap);

                        int MAX_CARACTERES_TIPO_INPUTACION = 12;

                        string ORDER_ID_SEL = getCodigoTablaSap(subposicion.TipoImputacionSap).Length > MAX_CARACTERES_TIPO_INPUTACION ? getCodigoTablaSap(subposicion.TipoImputacionSap).Substring(0, MAX_CARACTERES_TIPO_INPUTACION) : getCodigoTablaSap(subposicion.TipoImputacionSap);
                        string PROFIT_CTR_SEL = getCodigoTablaSap(subposicion.TipoImputacionSap).Length > MAX_CARACTERES_TIPO_INPUTACION ? getCodigoTablaSap(subposicion.TipoImputacionSap).Substring(0, MAX_CARACTERES_TIPO_INPUTACION) : getCodigoTablaSap(subposicion.TipoImputacionSap);

                        if (!solpPedidoSAP.IM_POACCOUNTList.Exists(x =>
                                x.PO_ITEM == $"{poItem:00000}" &&
                                x.GL_ACCOUNT == getCodigoTablaSap(subposicion.CuentaMayorSap) &&
                                x.COSTCENTER == getCodigoTablaSap(subposicion.TipoImputacionSap) &&
                                //x.ORDERID == getCodigoTablaSap(subposicion.TipoImputacionSap) &&
                                //x.PROFIT_CTR == getCodigoTablaSap(subposicion.TipoImputacionSap)
                                x.ORDERID == ORDER_ID_SEL &&
                                x.PROFIT_CTR == ""
                            ))
                        {

                            var imputacion = new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOACCOUNT();
                            numeroDeImputacion++;
                            imputacion.PO_ITEM = $"{poItem:00000}";
                            imputacion.SERIAL_NO = $"{numeroDeImputacion:00}";
                            imputacion.GL_ACCOUNT = ObtenerCuentaMayor(esPosicionDeMateriales, solpPosicion);
                            imputacion.QUANTITY = subposicion.Cantidad.Value;
                            //imputacion.QUANTITYSpecified = true;
                            imputacion.BUS_AREA = "GENE";
                            imputacion.CO_AREA = "MOA";
                            imputacion.COSTCENTER = (getCodigoTablaGeneral(solpPosicion.TipoImputacion).ToLower() == "centrodecosto") ?
                                getCodigoTablaSap(subposicion.TipoImputacionSap) : "";

                            string ORDERID_GEN = (getCodigoTablaGeneral(solpPosicion.TipoImputacion).ToLower() == "ordendeot" || getCodigoTablaGeneral(solpPosicion.TipoImputacion).ToLower() == "ordendeinversion") ?
                                getCodigoTablaSap(subposicion.TipoImputacionSap) : "";
                            string PROFIT_CTR_GEN = getCodigoTablaSap(subposicion.TipoImputacionSap);

                            imputacion.ORDERID = ORDERID_GEN.Length > MAX_CARACTERES_TIPO_INPUTACION ? ORDERID_GEN.Substring(0, MAX_CARACTERES_TIPO_INPUTACION) : ORDERID_GEN;
                            imputacion.PROFIT_CTR = "";

                            imputacion.SUB_NUMBER = "";
                            imputacion.ASSET_NO = "";
                            imputacion.COSTOBJECT = "";
                            imputacion.DELETE_IND = "";
                            solpPedidoSAP.IM_POACCOUNTList.Add(imputacion);

                            solpPedidoSAP.IM_POACCOUNTXList.Add(new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOACCOUNTX
                            {
                                PO_ITEM = $"{poItem:00000}",
                                SERIAL_NO = $"{numeroDeImputacion:00}",
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
                                PROFIT_CTR = ""
                            });

                            var imputacionSubPos = new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIESKLC()
                            {
                                PCKG_NO = $"{PCKG_NO:0000000000}",
                                LINE_NO = $"{LINE_NO++:0000000000}",
                                PERCENTAGE = 100,
                                //PERCENTAGESpecified = true,
                                SERNO_LINE = $"{poItem:00}",
                                SERIAL_NO = $"{numeroDeImputacion:00}",
                            };
                            solpPedidoSAP.IM_POSRVACCESSVALUESList.Add(imputacionSubPos);
                        }
                        else
                        {
                            var imputacionUsada = solpPedidoSAP.IM_POACCOUNTList.Find(x =>
                                x.PO_ITEM == $"{poItem:00000}" &&
                                x.GL_ACCOUNT == getCodigoTablaSap(subposicion.CuentaMayorSap) &&
                                x.COSTCENTER == getCodigoTablaSap(subposicion.TipoImputacionSap) &&
                                x.ORDERID == getCodigoTablaSap(subposicion.TipoImputacionSap) &&
                                x.PROFIT_CTR == ""
                                );
                            imputacionUsada.QUANTITY += subposicion.Cantidad.Value;

                            var imputacionSubPos = new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIESKLC()
                            {
                                PCKG_NO = $"{PCKG_NO:0000000000}",
                                LINE_NO = $"{LINE_NO++:0000000000}",
                                PERCENTAGE = 100,
                                //PERCENTAGESpecified = true,
                                SERNO_LINE = $"{poItem:00}",
                                SERIAL_NO = $"{imputacionUsada.SERIAL_NO:00}",
                            };
                            solpPedidoSAP.IM_POSRVACCESSVALUESList.Add(imputacionSubPos);
                        }




                    }
                    PCKG_NO++;
                }

                solpPedidoSAP.IM_POSCHEDULEList.Add(new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOSCHEDULE
                {
                    DELIVERY_DATE = adjudicacionPosicion.PlazoDeEntrega.ToString("dd.MM.yyyy"),
                    PO_ITEM = $"{poItem:00000}",
                    SCHED_LINE = "1",
                    DELIV_TIME = "00:00:00",
                    MS_TIME = "00:00:00",
                    LOAD_TIME = "00:00:00",
                    TP_TIME = "00:00:00",
                    GI_TIME = "00:00:00",
                    GR_END_TIME = "00:00:00",
                    HANDOVERTIME = "00:00:00",
                });

                solpPedidoSAP.IM_POSCHEDULEXList.Add(new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOSCHEDULX
                {
                    DELIVERY_DATE = "X",
                    PO_ITEM = $"{poItem:00000}",
                    SCHED_LINE = "1"
                });
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
                        solpPedidoSAP.IM_POTEXTHEADERList.Add(new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOTEXTHEADER
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

            if (adjudicacion.Posiciones.First().Posicion.Solp.Urgencia == true)
            {
                solpPedidoSAP.IM_POTEXTITEMList.Add(new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOTEXT
                {
                    TEXT_ID = "F12",
                    PO_NUMBER = "",
                    PO_ITEM = $"{poItem:00000}",
                    TEXT_FORM = "*",
                    TEXT_LINE = "Urgencia"
                });
            }

            solpPedidoSAP.IM_URL = ConfigurationManager.AppSettings["SpaUrl"] + "/verLegajoOrdenDeCompra/" + adjudicacion.Id + "/" + adjudicacion.Token;

            return solpPedidoSAP;
        }

        private static string getCodigoTablaGeneral(TablaGeneral imputacion)
        {
            var result = "";

            if (imputacion != null)
            {
                result = imputacion.Codigo;
            }
            return result;
        }

        private static string getCodigoTablaSap(TablaSap imputacion)
        {
            var result = "";

            if (imputacion != null)
            {
                result = imputacion.Codigo;
            }
            return result;
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
        public List<CrearPedidoWebServiceMOA.BAPIMEPOACCOUNT> IM_POACCOUNTList { get; set; }
        public List<CrearPedidoWebServiceMOA.BAPIMEPOACCOUNTX> IM_POACCOUNTXList { get; set; }
        public List<CrearPedidoWebServiceMOA.BAPIMEPOADDRDELIVERY> IM_POADDREDELIVERYList { get; set; }
        public List<CrearPedidoWebServiceMOA.BAPIMEPOCOND> IM_POCONDList { get; set; }
        public List<CrearPedidoWebServiceMOA.BAPIMEPOCONDHEADER> IM_POCONDHEADERList { get; set; }
        public List<CrearPedidoWebServiceMOA.BAPIMEPOCONDHEADERX> IM_POCONDHEADERXList { get; set; }
        public List<CrearPedidoWebServiceMOA.BAPIMEPOCONDX> IM_POCONDXList { get; set; }
        public CrearPedidoWebServiceMOA.BAPIMEPOHEADER IM_POHEADERList { get; set; }
        public CrearPedidoWebServiceMOA.BAPIMEPOHEADERX IM_POHEADERXList { get; set; }
        public List<CrearPedidoWebServiceMOA.BAPIMEPOITEM> IM_POITEMList { get; set; }
        public List<CrearPedidoWebServiceMOA.BAPIMEPOITEMX> IM_POITEMXList { get; set; }
        public List<CrearPedidoWebServiceMOA.BAPIMEPOSCHEDULE> IM_POSCHEDULEList { get; set; }
        public List<CrearPedidoWebServiceMOA.BAPIMEPOSCHEDULX> IM_POSCHEDULEXList { get; set; }
        public List<CrearPedidoWebServiceMOA.BAPIESKLC> IM_POSRVACCESSVALUESList { get; set; }
        public List<CrearPedidoWebServiceMOA.BAPIMEPOTEXTHEADER> IM_POTEXTHEADERList { get; set; }
        public List<CrearPedidoWebServiceMOA.BAPIMEPOTEXT> IM_POTEXTITEMList { get; set; }
        public List<CrearPedidoWebServiceMOA.BAPIESLLC> IM_SERVICESList { get; set; }
        public string IM_URL { get; set; }


        public SolpPedidoSAPDto()
        {
            IM_POACCOUNTList = new List<CrearPedidoWebServiceMOA.BAPIMEPOACCOUNT>();
            IM_POACCOUNTXList = new List<CrearPedidoWebServiceMOA.BAPIMEPOACCOUNTX>();
            IM_POADDREDELIVERYList = new List<CrearPedidoWebServiceMOA.BAPIMEPOADDRDELIVERY>();
            IM_POCONDList = new List<CrearPedidoWebServiceMOA.BAPIMEPOCOND>();
            IM_POCONDHEADERList = new List<CrearPedidoWebServiceMOA.BAPIMEPOCONDHEADER>();
            IM_POCONDHEADERXList = new List<CrearPedidoWebServiceMOA.BAPIMEPOCONDHEADERX>();
            IM_POCONDXList = new List<CrearPedidoWebServiceMOA.BAPIMEPOCONDX>();
            IM_POHEADERList = new CrearPedidoWebServiceMOA.BAPIMEPOHEADER();
            IM_POHEADERXList = new CrearPedidoWebServiceMOA.BAPIMEPOHEADERX();
            IM_POITEMList = new List<CrearPedidoWebServiceMOA.BAPIMEPOITEM>();
            IM_POITEMXList = new List<CrearPedidoWebServiceMOA.BAPIMEPOITEMX>();
            IM_POSCHEDULEList = new List<CrearPedidoWebServiceMOA.BAPIMEPOSCHEDULE>();
            IM_POSCHEDULEXList = new List<CrearPedidoWebServiceMOA.BAPIMEPOSCHEDULX>();
            IM_POSRVACCESSVALUESList = new List<CrearPedidoWebServiceMOA.BAPIESKLC>();
            IM_POTEXTHEADERList = new List<CrearPedidoWebServiceMOA.BAPIMEPOTEXTHEADER>();
            IM_POTEXTITEMList = new List<CrearPedidoWebServiceMOA.BAPIMEPOTEXT>();
            IM_SERVICESList = new List<CrearPedidoWebServiceMOA.BAPIESLLC>();
            IM_URL = "";
        }
    }
    public class SolpPedidoSAPSinPIDto
    {
        public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOACCOUNT> IM_POACCOUNTList { get; set; }
        public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOACCOUNTX> IM_POACCOUNTXList { get; set; }
        public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOADDRDELIVERY> IM_POADDREDELIVERYList { get; set; }
        public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCOND> IM_POCONDList { get; set; }
        public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCONDHEADER> IM_POCONDHEADERList { get; set; }
        public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCONDHEADERX> IM_POCONDHEADERXList { get; set; }
        public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCONDX> IM_POCONDXList { get; set; }
        public WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOHEADER IM_POHEADERList { get; set; }
        public WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOHEADERX IM_POHEADERXList { get; set; }
        public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOITEM> IM_POITEMList { get; set; }
        public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOITEMX> IM_POITEMXList { get; set; }
        public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOSCHEDULE> IM_POSCHEDULEList { get; set; }
        public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOSCHEDULX> IM_POSCHEDULEXList { get; set; }
        public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIESKLC> IM_POSRVACCESSVALUESList { get; set; }
        public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOTEXTHEADER> IM_POTEXTHEADERList { get; set; }
        public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOTEXT> IM_POTEXTITEMList { get; set; }
        public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIESLLC> IM_SERVICESList { get; set; }
        public string IM_URL { get; set; }


        public SolpPedidoSAPSinPIDto()
        {
            IM_POACCOUNTList = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOACCOUNT>();
            IM_POACCOUNTXList = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOACCOUNTX>();
            IM_POADDREDELIVERYList = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOADDRDELIVERY>();
            IM_POCONDList = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCOND>();
            IM_POCONDHEADERList = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCONDHEADER>();
            IM_POCONDHEADERXList = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCONDHEADERX>();
            IM_POCONDXList = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCONDX>();
            IM_POHEADERList = new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOHEADER();
            IM_POHEADERXList = new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOHEADERX();
            IM_POITEMList = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOITEM>();
            IM_POITEMXList = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOITEMX>();
            IM_POSCHEDULEList = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOSCHEDULE>();
            IM_POSCHEDULEXList = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOSCHEDULX>();
            IM_POSRVACCESSVALUESList = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIESKLC>();
            IM_POTEXTHEADERList = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOTEXTHEADER>();
            IM_POTEXTITEMList = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOTEXT>();
            IM_SERVICESList = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIESLLC>();
            IM_URL = "";
        }
    }


    public interface ICrearPedidoConsumerMOA
    {
        CrearPedidoConsumerMOAResponse Request(Adjudicacion adjudicacion, bool creadoAutomatico = false);

    }

}
