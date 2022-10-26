using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Entities;
using SustitucionMOAWS.CrearPedidoWebServiceMOA;
using SustitucionMOAWS.CredentialService;

namespace SustitucionMOAWS.WSConsumers
{
    public class CrearPedidoConsumerMOA : ICrearPedidoConsumerMOA
    {
        private readonly SI_MMRFC_CREAR_PEDIDOClient service;
        private readonly string rutaArchivosXmls = ConfigurationManager.AppSettings["RutaArchivosCompras"];

        public CrearPedidoConsumerMOA()
        {
            service = new SI_MMRFC_CREAR_PEDIDOClient();
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
        }

        public CrearPedidoConsumerMOAResponse Request(Solp solpActual, SolpPosicion postEntitySubPosicionesEliminadas, List<SolpPosicion> proveedorConPosiciones)
        {
            var solpPedidoSAP = ConvertirSOLP(solpActual, postEntitySubPosicionesEliminadas, proveedorConPosiciones);

            var serxml = new System.Xml.Serialization.XmlSerializer(solpPedidoSAP.GetType());
            var ms = new MemoryStream();
            serxml.Serialize(ms, solpPedidoSAP);
            string xml = Encoding.UTF8.GetString(ms.ToArray());

            var fecha = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");

            var nombreArchivoLlamada = string.Concat(solpActual.Id, " - ", fecha, " - llamada crear.xml");
            var nombreArchivoRespuesta = string.Concat(solpActual.Id, " - ", fecha, " - respuesta crear.xml");

            var rutaArchivoLlamada = Path.Combine(rutaArchivosXmls, "XMLS", nombreArchivoLlamada);
            var rutaArchivoRespuesta = Path.Combine(rutaArchivosXmls, "XMLS", nombreArchivoRespuesta);

            FileInfo fileCrear = new FileInfo(rutaArchivoLlamada);
            fileCrear.Directory.Create();
            File.WriteAllText(fileCrear.FullName, xml);

            var result = service.SI_MMRFC_CREAR_PEDIDO( solpPedidoSAP.IM_POACCOUNTList.ToArray(),
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

            var jsonRespuesta = JsonConvert.SerializeObject(respuesta);

            //FileInfo fileRespuesta = new FileInfo(rutaArchivoRespuesta);
            //fileRespuesta.Directory.Create();
            //File.WriteAllText(fileRespuesta.FullName, jsonRespuesta);

            return respuesta;
        }

        private string getCodigoTablaSap(TablaSap imputacion)
        {
            var result = "";

            if (imputacion != null)
            {
                result = imputacion.Codigo;
            }
            return result;
        }

        private string getCodigoTablaGeneral(TablaGeneral imputacion)
        {
            var result = "";

            if (imputacion != null)
            {
                result = imputacion.Codigo;
            }
            return result;
        }

        public SolpPedidoSAPDto ConvertirSOLP(Solp solp, SolpPosicion postEntitySubPosicionesEliminadas, List<SolpPosicion> proveedorConPosiciones)
        {
            SolpPedidoSAPDto solpPedidoSAP = new SolpPedidoSAPDto();
            int numeroPosicion = 0;   
            string numeroPaquete = "";
            string preqItem = "";
            string serialNumber = "";
           
            string docItem = "";
        

            //aca el metodo agruparia las posiciones por el numero del proveedor que tengo cada posicion
            foreach (var posicion in proveedorConPosiciones.OrderBy(x => x.Id))
            {
            
                bool eliminarPosicion = posicion.Subposiciones.Where(item => !Convert.ToBoolean(item.Estado)).Count() == posicion.Subposiciones.Count;
                bool eliminarSubPosicion = posicion.Subposiciones.Where(item => !Convert.ToBoolean(item.Estado)).Count() == posicion.Subposiciones.Count;
                eliminarPosicion = eliminarPosicion ? true : !posicion.Estado;            
                numeroPosicion++;
                preqItem = $"{numeroPosicion:00000}";
                docItem = preqItem;
                numeroPaquete = $"{numeroPosicion:0000000000}";
                serialNumber = $"{numeroPosicion:00}";
                
                //Nombre: ZBAPIMEPOHEADER Denominación:	Cabecera del Pedido de Compras
                solpPedidoSAP.IM_POHEADERList = new ZMPES6780
                {
                    PO_NUMBER = "", //PO_NUMBER   EBELN Número del documento de compras
                    COMP_CODE = "MOA", //COMP_CODE BUKRS   Sociedad
                    DOC_TYPE = solp.ClaseDocumento.CodigoSap, //DOC_TYPE    ESART Clase de documento de compras
                    DELETE_IND = posicion.TipoPosicion.Codigo != "MATERIALES" ? SAPFormatter.FormatearBooleano(eliminarPosicion) : "", //DELETE_IND ELOEK   Indicador de borrado en el documento de compras
                    STATUS = "", //STATUS ESTAK   Status del documento de compras
                    CREAT_DATE = "", //SAPFormatter.PrepararFecha(solp.FechaCreacion), //CREAT_DATE  ERDAT Fecha de creación del registro
                    CREATED_BY = solp.UsuarioCreacion.UsuarioSap, //CREATED_BY ERNAM   Nombre del responsable que ha añadido el objeto
                    VENDOR = posicion.ProveedorFijo, //VENDOR ELIFN   Número de cuenta del proveedor
                    PMNTTRMS = "", //"BASE", //PMNTTRMS    DZTERM Clave de condiciones de pago
                    PURCH_ORG = posicion.OrganizacionCompras, //PURCH_ORG EKORG   Organización de compras
                    PUR_GROUP = posicion.GrupoCompras.CodigoSap.ToString(), //PUR_GROUP   BKGRP Grupo de compras
                    CURRENCY = posicion.Moneda.Codigo, //CURRENCY WAERS   Clave de moneda
                    EXCH_RATE = 0, //EXCH_RATE   WKURS Tipo de cambio de moneda
                    EX_RATE_FX = "", //EX_RATE_FX KUFIX   Indicador tipo de cambio fijo
                    DOC_DATE = SAPFormatter.PrepararFecha(DateTime.Now) //DOC_DATE    EBDAT Fecha del documento de compras
                };

                solpPedidoSAP.IM_POHEADERXList = new ZMPES6790
                {
                    PO_NUMBER = "",
                    COMP_CODE = "X",
                    DOC_TYPE = "X",
                    DELETE_IND = (eliminarPosicion == true) ? "X" : "",
                    STATUS = "",
                    CREAT_DATE = "",
                    CREATED_BY = "X",
                    VENDOR = "X",
                    PMNTTRMS = "",
                    PURCH_ORG = "X",
                    PUR_GROUP = "X",
                    CURRENCY = "X",
                    EXCH_RATE = "X",
                    EX_RATE_FX = "",
                    DOC_DATE = "X"
                };

                //Nombre: ZBAPIMEPOITEM Denominación:	Posición de PEDIDOS
                var IM_POITEM = new ZMPES6800();
        
                IM_POITEM.PO_ITEM = preqItem;
                IM_POITEM.DELETE_IND = posicion.TipoPosicion.Codigo != "MATERIALES" ? SAPFormatter.FormatearBooleano(eliminarPosicion) : ""; //Indica si la posicion esta borrada;
                IM_POITEM.SHORT_TEXT = posicion.Tarea;
                IM_POITEM.MATERIAL = posicion.MaterialSolp != null && posicion.TipoPosicion.Codigo == "MATERIALES" ? posicion.MaterialSolp.CodigoSap.ToString() : "";
                IM_POITEM.PLANT = posicion.Centro.CodigoSap.ToString();
                IM_POITEM.STGE_LOC = posicion.Almacen.CodigoSap.ToString();
                IM_POITEM.TRACKINGNO = posicion.NroNecesidad; ;
                IM_POITEM.MATL_GROUP = posicion.GrupoArticulo.CodigoSap.ToString();
                IM_POITEM.INFO_REC = "";
                IM_POITEM.QUANTITY = (decimal)posicion.Cantidad;
                IM_POITEM.QUANTITYSpecified = true;
                IM_POITEM.PO_UNIT = posicion.Unidad.Descripcion;
                IM_POITEM.NET_PRICE = (decimal)posicion.PrecioBruto;
                IM_POITEM.NET_PRICESpecified = true;
                IM_POITEM.PRICE_UNIT = 1;
                //IM_POITEM.PRICE_UNITSpecified = true;
                IM_POITEM.GR_PR_TIME = 0;
                //IM_POITEM.GR_PR_TIMESpecified = true; 
                IM_POITEM.TAX_CODE = "";
                IM_POITEM.VAL_TYPE = SAPFormatter.FormatearBooleano(eliminarPosicion);
                IM_POITEM.NO_MORE_GR = "";
                IM_POITEM.FINAL_INV = "";

                switch (posicion.TipoPosicion.Codigo.ToLower())
                //ITEM_CAT PSTYP   Tipo de posición del documento de compras
                {
                    case "servicio":
                        IM_POITEM.ITEM_CAT = "9";
                        break;

                    case "materiales":
                    default:
                        IM_POITEM.ITEM_CAT = "0";
                        break;
                }

                switch (posicion.TipoImputacion.Codigo.ToLower())
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
                IM_POITEM.AGREEMENT = posicion.NumeroContratoSuperior; ;
                IM_POITEM.AGMT_ITEM = posicion.NumeroPosicionContratoSuperior; ;
                IM_POITEM.RFQ_NO = "";
                IM_POITEM.RFQ_ITEM = "";
                IM_POITEM.PREQ_NO = "";
                IM_POITEM.PREQ_ITEM = preqItem;
                IM_POITEM.PCKG_NO = numeroPaquete;
                
                solpPedidoSAP.IM_POITEMList.Add(IM_POITEM);
                                    
                solpPedidoSAP.IM_POITEMXList.Add(new ZMPES6810
                {
                    PO_ITEM = preqItem,
                    DELETE_IND = (IM_POITEM.DELETE_IND != null) ? "X" : "",
                    SHORT_TEXT = "X",
                    MATERIAL = "X",
                    PLANT = "X",
                    STGE_LOC = "X",
                    TRACKINGNO = "X",
                    MATL_GROUP = "X",
                    INFO_REC = "",
                    QUANTITY = ((decimal)IM_POITEM.QUANTITY == 0) ? "" : "X",
                    PO_UNIT = "X",
                    NET_PRICE = "X",
                    PRICE_UNIT = "X",
                    GR_PR_TIME = "",
                    TAX_CODE = "",
                    VAL_TYPE = (IM_POITEM.VAL_TYPE != null) ? "X" : "",
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
                    AGREEMENT = "X",
                    AGMT_ITEM = "X",
                    RFQ_NO = "",
                    RFQ_ITEM = "",
                    PREQ_NO = "",
                    PREQ_ITEM = "X",
                    PCKG_NO = "X"
                });

                //Nombre: ZBAPIMEPOACCOUNT Denominación:	Imputación
                solpPedidoSAP.IM_POACCOUNTList.Add(new ZMPES6830
                {
                    PO_ITEM = preqItem,
                    SERIAL_NO = serialNumber,
                    DELETE_IND = SAPFormatter.FormatearBooleano(eliminarPosicion),
                    QUANTITY = 0,
                    GL_ACCOUNT = posicion.CuentaMayorSap.Codigo, //"0000607034",
                    BUS_AREA = "GENE",
                    COSTCENTER = getCodigoTablaSap(posicion.TipoImputacionSap),
                    ASSET_NO = "",
                    SUB_NUMBER = "",
                    ORDERID = getCodigoTablaSap(posicion.TipoImputacionSap),
                    CO_AREA = "MOA",
                    COSTOBJECT = "",
                    PROFIT_CTR = getCodigoTablaSap(posicion.TipoImputacionSap)
                });

                solpPedidoSAP.IM_POACCOUNTXList.Add(new ZMPES6840
                {
                    PO_ITEM = preqItem,
                    SERIAL_NO = serialNumber,
                    DELETE_IND = (IM_POITEM.DELETE_IND != null) ? "X" : "",
                    QUANTITY = "X",
                    GL_ACCOUNT = "X",
                    BUS_AREA = "X",
                    COSTCENTER = (posicion.TipoImputacion.Codigo.ToLower() == "centrodecosto") ? "X" : "",
                    ASSET_NO = "",
                    SUB_NUMBER = "",
                    ORDERID = (posicion.TipoImputacion.Codigo.ToLower() == "ordendeot") ? "X" : "",
                    CO_AREA = "X",
                    COSTOBJECT = "",
                    PROFIT_CTR = (posicion.TipoImputacion.Codigo.ToLower() == "siniestrobeneficio") ? "X" : ""
                });

                //Nombre: ZBAPIMEPOADDREDELIVERY Denominación:	Direcciones de entrega
                solpPedidoSAP.IM_POADDREDELIVERYList.Add(new ZMPES6820
                {
                    PO_ITEM = preqItem,
                    POSTL_COD1 = posicion.CpEntrega,
                    CITY = posicion.Centro.Descripcion,
                    ADDR_NO = posicion.CalleEntrega,
                    NAME = posicion.NombreEntrega,
                    TEL1_NUMBR = "",
                    STREET = posicion.CalleEntrega,
                    STREET_NO = posicion.NumeroEntrega
                });

                //Nombre: ZBAPIMEPOCONDHEADER Denominación:	Posición de Servicio
                //solpPedidoSAP.IM_POCONDHEADERList.Add(new ZMPES6850
                //{
                //    CONDITION_NO = "", //CONDITION_NO    KNUMV Número de la condición de documento
                //    ITM_NUMBER = "", //ITM_NUMBER  KPOSN Número de posición de la condición
                //    COND_ST_NO = null, //COND_ST_NO  STUNR Número de paso
                //    COND_COUNT = null, //COND_COUNT DZAEHK_SHORT    Contador de condiciones(longitud corta)
                //    COND_TYPE = null, //COND_TYPE KSCHA   Clase de condición
                //    COND_VALUE = 0, //COND_VALUE  BAPIKBETR1 Importe de condición
                //    CURRENCY = null, //CURRENCY WAERS   Clase de Moneda
                //    CHANGE_ID = null, //CHANGE_ID   MEINS Unidad de medida base
                //    CALCTYPCON = null, //CALCTYPCON KRECH   Regla de cálculo para la condición
                //    CONDCLASS = null, //CONDCLASS KOAID   Categoría de condición
                //});

                //solpPedidoSAP.IM_POCONDHEADERXList.Add(new ZMPES6860
                //{
                //    CONDITION_NO = "X",
                //    ITM_NUMBER = "X",
                //    COND_ST_NO = "X",
                //    COND_COUNT = "X",
                //    COND_TYPE = "X",
                //    COND_VALUE = "X",
                //    CURRENCY = "X",
                //    CHANGE_ID = "X",
                //    CALCTYPCON = "X",
                //    CONDCLASS = "X"
                //});

                //Nombre: ZBAPIMEPOCOND Denominación:	Posición de Servicio          
                //solpPedidoSAP.IM_POCONDList.Add( new ZMPES6870
                //{
                //    CONDITION_NO = null,  //CONDITION_NO    KNUMV Número de la condición de documento
                //    ITM_NUMBER = null, //ITM_NUMBER  KPOSN Número de posición de la condición
                //    COND_ST_NO = null, //COND_ST_NO  STUNR Número de paso
                //    COND_COUNT = null, //COND_COUNT DZAEHK_SHORT    Contador de condiciones(longitud corta)
                //    COND_TYPE = null, //COND_TYPE KSCHA   Clase de condición
                //    COND_VALUE = 0, //COND_VALUE  BAPIKBETR1 Importe de condición
                //    CURRENCY = null, //CURRENCY WAERS   Clase de Moneda
                //    CHANGE_ID = null, //CHANGE_ID   MEINS Unidad de medida base
                //    CALCTYPCON = null, //CALCTYPCON KRECH   Regla de cálculo para la condición
                //    CONDCLASS = null //CONDCLASS KOAID   Categoría de condición
                //});

                //solpPedidoSAP.IM_POCONDXList.Add(new ZMPES6880
                //{
                //    CONDITION_NO = "X",
                //    ITM_NUMBER = "X",
                //    COND_ST_NO = "X",
                //    COND_COUNT = "X",
                //    COND_TYPE = "X",
                //    COND_VALUE = "X",
                //    CURRENCY = "X",
                //    CHANGE_ID = "X",
                //    CALCTYPCON = "X",
                //    CONDCLASS = "X"
                //});

            }

            return solpPedidoSAP;
        }
    }

    public class CrearPedidoConsumerMOAResponse
    {
        public string NumeroPedido { get; set; }
        public List<CrearPedidoConsumerMOAError> Errores { get; set; }
        public string Resultado { get; internal set; }
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
        CrearPedidoConsumerMOAResponse Request(Solp solpActual, SolpPosicion postEntitySubPosicionesEliminadas, List<SolpPosicion> proveedorConPosiciones);

    }
}
