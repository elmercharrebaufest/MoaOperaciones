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

        public CrearPedidoConsumerMOAResponse Request(Solp solpActual, SolpPosicion postEntitySubPosicionesEliminadas)
        {
            var solpPedidoSAP = ConvertirSOLP(solpActual, postEntitySubPosicionesEliminadas);

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

            respuesta.NumeroSolp = EX_PO_NUMBER;
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

        public SolpPedidoSAPDto ConvertirSOLP(Solp solpActual, SolpPosicion postEntitySubPosicionesEliminadas)
        {
            SolpPedidoSAPDto solpPedidoSAP = new SolpPedidoSAPDto();
            int numeroPosicion = 0;   
            string numeroPaquete = "";
            string preqItem = "";
            string serialNumber = "";
           
            string docItem = "";
        
            foreach (var posicion in solpActual.Posiciones.OrderBy(x => x.Id))
            {
            
                bool eliminarPosicion = posicion.Subposiciones.Where(item => !Convert.ToBoolean(item.Estado)).Count() == posicion.Subposiciones.Count;
                bool eliminarSubPosicion = posicion.Subposiciones.Where(item => !Convert.ToBoolean(item.Estado)).Count() == posicion.Subposiciones.Count;
                eliminarPosicion = eliminarPosicion ? true : !posicion.Estado;            
                numeroPosicion++;
                preqItem = $"{numeroPosicion:00000}";
                docItem = preqItem;
                numeroPaquete = $"{numeroPosicion:0000000000}";
                serialNumber = $"{numeroPosicion:00}";

        
                var IM_POITEM = new ZMPES6800();
        
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

                IM_POITEM.PO_ITEM = preqItem;
                IM_POITEM.DELETE_IND = "";
                IM_POITEM.SHORT_TEXT = "";
                IM_POITEM.MATERIAL =  posicion.MaterialSolp.CodigoSap.ToString();
                IM_POITEM.PLANT = posicion.Centro.CodigoSap.ToString();
                IM_POITEM.STGE_LOC = "";
                IM_POITEM.TRACKINGNO = "";
                IM_POITEM.MATL_GROUP = posicion.GrupoArticulo.CodigoSap.ToString();
                IM_POITEM.INFO_REC = "";
                IM_POITEM.QUANTITY = (decimal)posicion.Cantidad;
                IM_POITEM.PO_UNIT = posicion.Unidad.Descripcion;
                IM_POITEM.NET_PRICE = (decimal)posicion.PrecioBruto;
                IM_POITEM.PRICE_UNIT = 1;
                IM_POITEM.GR_PR_TIME = 0;
                IM_POITEM.TAX_CODE = "";
                IM_POITEM.VAL_TYPE = "";
                IM_POITEM.NO_MORE_GR = "";
                IM_POITEM.FINAL_INV = "";
                IM_POITEM.ITEM_CAT = "";
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
                IM_POITEM.PREQ_NO = "";
                IM_POITEM.PREQ_ITEM = "";
                IM_POITEM.PCKG_NO = "";
                IM_POITEM.QUANTITYSpecified = true;

                solpPedidoSAP.IM_POITEMList.Add(IM_POITEM);
                                    
                solpPedidoSAP.IM_POITEMXList.Add(new ZMPES6810
                {
                    PO_ITEM = preqItem,
                    DELETE_IND = "",
                    SHORT_TEXT = "",
                    MATERIAL = "X",
                    PLANT = "X",
                    STGE_LOC = "",
                    TRACKINGNO = "",
                    MATL_GROUP = "X",
                    INFO_REC = "",
                    QUANTITY = "X",
                    PO_UNIT = "X",
                    NET_PRICE = "X",
                    PRICE_UNIT = "X",
                    GR_PR_TIME = "",
                    TAX_CODE = "",
                    VAL_TYPE = "",
                    NO_MORE_GR = "",
                    FINAL_INV = "",
                    ITEM_CAT = "",
                    ACCTASSCAT = "X",
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
                    PREQ_NO = "",
                    PREQ_ITEM = "",
                    PCKG_NO = ""
                   
                });
         

                solpPedidoSAP.IM_POACCOUNTList.Add(new ZMPES6830
                {
                    PO_ITEM = preqItem,
                    SERIAL_NO = serialNumber,
                    DELETE_IND = "",
                    //QUANTITY = 0,
                    GL_ACCOUNT = posicion.CuentaMayorSap.Codigo, //"0000607034",
                    BUS_AREA = "GENE",
                    COSTCENTER = "",
                    ASSET_NO = "",
                    SUB_NUMBER = "",
                    ORDERID = "",
                    CO_AREA = "MOA",
                    COSTOBJECT = "",
                    PROFIT_CTR = posicion.TipoImputacionSap.Codigo
                });

                solpPedidoSAP.IM_POACCOUNTXList.Add(new ZMPES6840
                {
                    PO_ITEM = preqItem,
                    SERIAL_NO = serialNumber,
                    DELETE_IND = "",
                    //QUANTITY = "X",
                    GL_ACCOUNT = "X", //"0000607034",
                    BUS_AREA = "X",
                    COSTCENTER = "",
                    ASSET_NO = "",
                    SUB_NUMBER = "",
                    ORDERID = "",
                    CO_AREA = "X",
                    COSTOBJECT = "",
                    PROFIT_CTR = (posicion.TipoImputacion.Codigo.ToLower() == "siniestrobeneficio") ? "X" : ""
                });

                solpPedidoSAP.IM_POHEADERList = new ZMPES6780
                {

                    PO_NUMBER = "",
                    COMP_CODE = "MOA",
                    DOC_TYPE ="ZPE1",
                    DELETE_IND = "",
                    STATUS = "",
                    CREAT_DATE = SAPFormatter.PrepararFecha(solpActual.FechaCreacion),
                    CREATED_BY = solpActual.UsuarioCreacion.UsuarioSap,
                    VENDOR = "0070947667",
                    PMNTTRMS = "BASE",
                    PURCH_ORG = "1600",//organizacion de compra
                    PUR_GROUP = posicion.GrupoCompras.CodigoSap.ToString(),
                    CURRENCY = posicion.Moneda.Codigo,
                    //EXCH_RATE = 0,
                    EX_RATE_FX = "",
                    DOC_DATE = ""

                };

                solpPedidoSAP.IM_POHEADERXList = new ZMPES6790
                {
                    PO_NUMBER = "",
                    COMP_CODE = "X",
                    DOC_TYPE = "X",
                    DELETE_IND = "",
                    STATUS = "",
                    CREAT_DATE = "X",
                    CREATED_BY = "X",
                    VENDOR = "X",
                    PMNTTRMS = "X",
                    PURCH_ORG = "X",
                    PUR_GROUP = "X",
                    CURRENCY = "X",
                    //EXCH_RATE = "",
                    EX_RATE_FX = "",
                    DOC_DATE = ""

                };

                //solpPedidoSAP.IM_POADDREDELIVERYList.Add(new ZMPES6820
                //{
                //    PO_ITEM = preqItem,
                //    POSTL_COD1 = posicion.CpEntrega,
                //    CITY = posicion.Centro.Descripcion,
                //    ADDR_NO = posicion.CalleEntrega,
                //    NAME = posicion.NombreEntrega,
                //    //TEL1_NUMBR = posicion.n,
                //    STREET = posicion.CalleEntrega,
                //    STREET_NO = posicion.NumeroEntrega

                //});




                // FIN de tabla  ZBAPIMEPOITEM



                //if (!solpPedidoSAP.IM_POACCOUNTList.Any(x =>
                //           x.PREQ_ITEM == preqItem &&
                //           x.SERIAL_NO == serialNumber &&
                //           x.GL_ACCOUNT == posicion.CuentaMayorSap.Codigo &&//"0000607034" && 
                //           x.COSTCENTER == posicion.TipoImputacionSap.Codigo &&
                //           x.ORDERID == posicion.TipoImputacionSap.Codigo &&
                //           x.PROFIT_CTR == posicion.TipoImputacionSap.Codigo

                //       ))
                //{
                //    numeroSerialNumberItem++;

                //    serialNumberItem = $"{numeroSerialNumberItem:00}";


                //    solpPedidoSAP.IM_POACCOUNTList.Add(new ZMPES6830
                //    {
                //        PREQ_ITEM = preqItem,
                //        SERIAL_NO = serialNumberItem,
                //        QUANTITY = posicion.Cantidad.Value,
                //        GL_ACCOUNT = posicion.CuentaMayorSap.Codigo, //"0000607034",
                //        COSTCENTER = posicion.TipoImputacionSap.Codigo,
                //        ORDERID = posicion.TipoImputacionSap.Codigo,
                //        PROFIT_CTR = posicion.TipoImputacionSap.Codigo
                //    }); ;


                //    solpPedidoSAP.IM_POACCOUNTXList.Add(new ZMPES5680
                //    {
                //        PREQ_ITEM = preqItem,
                //        SERIAL_NO = serialNumberItem,
                //        PREQ_ITEMX = "X",
                //        SERIAL_NOX = "X",
                //        QUANTITY = "X",
                //        GL_ACCOUNT = "X",
                //        COSTCENTER = (posicion.TipoImputacion.Codigo.ToLower() == "centrodecosto") ? "X" : "",
                //        ORDERID = (posicion.TipoImputacion.Codigo.ToLower() == "ordendeot" || posicion.TipoImputacion.Codigo.ToLower() == "ordendeinversion") ? "X" : "",
                //        PROFIT_CTR = (posicion.TipoImputacion.Codigo.ToLower() == "siniestrobeneficio") ? "X" : ""
                //    });
                //}
                //else
                //{
                //    serialNumberItem = solpPedidoSAP.IM_POACCOUNTList.FirstOrDefault(x =>
                //        x.PREQ_ITEM == preqItem &&
                //        x.SERIAL_NO == serialNumber &&
                //        x.GL_ACCOUNT == posicion.CuentaMayorSap.Codigo &&//"0000607034" && 
                //        x.COSTCENTER == posicion.TipoImputacionSap.Codigo &&
                //        x.ORDERID == posicion.TipoImputacionSap.Codigo &&
                //        x.PROFIT_CTR == posicion.TipoImputacionSap.Codigo
                //    ).SERIAL_NO;
                //}


            }

                return solpPedidoSAP;
        }
    }

        




    public class CrearPedidoConsumerMOAResponse
    {
        public string NumeroSolp { get; set; }
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
        CrearPedidoConsumerMOAResponse Request(Solp solpActual, SolpPosicion postEntitySubPosicionesEliminadas);

    }
}


//Nombre Campo    Tipo Tipo    Opcional Texto
//IM_POHEADER ZBAPIMEPOHEADER Estructura Si  Cabecera de PEDIDO
//IM_POHEADERX    ZBAPIMEPOHEADERX Estructura  SI Cabecera de PEDIDO(ind de contenido)
//IM_POITEM ZBAPIMEPOITEM   Tabla Si  Posición del Pedido
//IM_POITEMX  ZBAPIMEPOITEMX Tabla   Si Posición del Pedido(Ind.de contenido)
//IM_POACCOUNT ZBAPIMEPOACCOUNT    Tabla Si  Asignación de Imputación
//IM_POACCOUNTX   ZBAPIMEPOACCOUNTX Tabla   Si Asignación de Imputación(Ind.Cont)
//IM_POADDREDELIVERY ZBAPIMEPOADDREDELIVERY  Tabla Si  Dirección entrega
//IM_POCONDHEADER ZBAPIMEPOCONDHEADER Tabla Si  Condiciones a nivel Cabecera
//IM_POCONDHEADERX ZBAPIMEPOCONDHEADERX    Tabla Si  Condiciones a nivel Cabecera(ind cont)
//IM_POCOND ZBAPIMEPOCOND   Tabla Si  Condiciones de posición
//IM_POCONDX  ZBAPIMEPOCONDX Tabla   Si Condiciones de posición(ind.Cont)
//IM_POSCHEDULE ZBAPIMEPOSCHEDULE   Tabla Si  Reparto
//IM_POSCHEDULEX  ZBAPIMEPOSCHEDULEX Tabla   Si Reparto(Ind.Cont)
//IM_SERVICES BAPIESLLC   Tabla Si  Servicios
//IM_POSRVACCESSVALUES    BAPIESKLC Tabla   Si Valores de Servicios
//IM_POTEXTHEADER BAPIMEPOTEXTHEADER  Tabla SI  Textos Cabecera
//IM_POTEXTITEM BAPIMEPOTEXT    Tabla SI  Textos Posición
//IM_URL STRING  String SI  Texto largo para url


//Nombre Campo    Tipo Texto
//EX_Exito INT1    Resultado Ejecución[200 = Existo / 400 = Error]
//EX_RETURN BAPIRET2    Return parameters
//EX_PO_NUMBER EBELN   Numero de Pedido   


//Nombre: ZBAPIMEPOHEADER Denominación:	Cabecera del Pedido de Compras
//Nombre  Dominio / Tipo  Denominación
//PO_NUMBER   EBELN Número del documento de compras
//COMP_CODE BUKRS   Sociedad
//DOC_TYPE    ESART Clase de documento de compras
//DELETE_IND ELOEK   Indicador de borrado en el documento de compras
//STATUS ESTAK   Status del documento de compras
//CREAT_DATE  ERDAT Fecha de creación del registro
//CREATED_BY ERNAM   Nombre del responsable que ha añadido el objeto
//VENDOR ELIFN   Número de cuenta del proveedor
//PMNTTRMS    DZTERM Clave de condiciones de pago
//PURCH_ORG EKORG   Organización de compras
//PUR_GROUP   BKGRP Grupo de compras
//CURRENCY WAERS   Clave de moneda
//EXCH_RATE   WKURS Tipo de cambio de moneda
//EX_RATE_FX KUFIX   Indicador tipo de cambio fijo
//DOC_DATE    EBDAT Fecha del documento de compras


//Nombre: ZBAPIMEPOHEADERX Denominación:	Cabecera del Pedido de Compras
//Nombre  Dominio / Tipo  Denominación
//PO_NUMBER   BAPIUPDATE Número del documento de compras
//COMP_CODE BAPIUPDATE  Sociedad
//DOC_TYPE    BAPIUPDATE Clase de documento de compras
//DELETE_IND BAPIUPDATE  Indicador de borrado en el documento de compras
//STATUS BAPIUPDATE  Status del documento de compras
//CREAT_DATE  BAPIUPDATE Fecha de creación del registro
//CREATED_BY BAPIUPDATE  Nombre del responsable que ha añadido el objeto
//VENDOR BAPIUPDATE  Número de cuenta del proveedor
//PMNTTRMS    BAPIUPDATE Clave de condiciones de pago
//PURCH_ORG BAPIUPDATE  Organización de compras
//PUR_GROUP   BAPIUPDATE Grupo de compras
//CURRENCY BAPIUPDATE  Clave de moneda
//EXCH_RATE   BAPIUPDATE Tipo de cambio de moneda
//EX_RATE_FX BAPIUPDATE  Indicador tipo de cambio fijo
//DOC_DATE    BAPIUPDATE Fecha del documento de compras

//Nombre: ZBAPIMEPOITEM Denominación:	Posición de PEDIDOS
//Nombre  Dominio / Tipo  Denominación
//PO_ITEM EBELP Número de posición del documento de compras
//DELETE_IND ELOEK   Indicador de borrado en el documento de compras
//SHORT_TEXT TXZ01   Texto breve
//MATERIAL MATNR18 Número de material(18 caracteres)
//PLANT EWERK   Centro
//STGE_LOC    LGORT_D Almacén
//TRACKINGNO BEDNR   Número de necesidad
//MATL_GROUP  MATKL Grupo de artículos
//TRACKINGNO BEDNR   Número de necesidad
//INFO_REC    INFNR Número del registro info de compras
//QUANTITY    BSTMG Cantidad de pedido
//PO_UNIT BSTME   Unidad de medida de pedido
//NET_PRICE   BAPICUREXT Importe de moneda para BAPIs(con 9 decimales)
//PRICE_UNIT EPEIN   Cantidad base
//GR_PR_TIME WEBAZ   Tiempo de tratamiento para la entrada de mercancía en días
//TAX_CODE MWSKZ   Indicador IVA
//VAL_TYPE BWTAR_D Clase de valoración
//NO_MORE_GR  ELIKZ Indicador de entrega final
//FINAL_INV   EREKZ Indicador de factura final
//ITEM_CAT    PSTYP Tipo de posición del documento de compras
//ACCTASSCAT KNTTP   Tipo de imputación
//DISTRIB VRTKZ Indicador de distribución en la imputación múltiple
//PART_INV TWRKZ   Indicador de factura parcial
//GR_IND WEPOS   Indicador de entrada de mercancías
//GR_NON_VAL  WEUNB Entrada de mercancías no valorada
//IR_IND REPOS   Indicador de recepción de factura
//FREE_ITEM   UMSON Posición sin cargo
//GR_BASEDIV WEBRE   Indicador p.verificación de facturas sobre la base de la EM
//ACKN_REQD   KZABS Indicador de obligación de confirmación de pedido
//ACKNOWL_NO LABNR   Número de confirmación de pedido
//AGREEMENT   KONNR Número del contrato superior
//AGMT_ITEM   KTPNR Número de posición del contrato superior
//RFQ_NO  ANFNR Núm.petición oferta
//RFQ_ITEM ANFPS   Número de posición de la petición de oferta
//PREQ_NO BANFN   Número de la solicitud de pedido
//PREQ_ITEM BNFPO   Número de posición de la solicitud de pedido
//PCKG_NO PACKNO  Numero de Paquete

//Nombre: ZBAPIMEPOITEMx Denominación:	Posición de PEDIDOS
//Nombre  Dominio / Tipo  Denominación
//PO_ITEM EBELP Número de posición del documento de compras
//PO_ITEMX BAPIUPDATE  Indicadores de edición o agregado de la posición
//DELETE_IND BAPIUPDATE  Indicador de borrado en el documento de compras
//SHORT_TEXT BAPIUPDATE  Texto breve
//MATERIAL BAPIUPDATE  Número de material(18 caracteres)
//PLANT BAPIUPDATE  Centro
//STGE_LOC    BAPIUPDATE Almacén
//TRACKINGNO BAPIUPDATE  Número de necesidad
//MATL_GROUP  BAPIUPDATE Grupo de artículos
//TRACKINGNO BAPIUPDATE  Número de necesidad
//INFO_REC    BAPIUPDATE Número del registro info de compras
//QUANTITY    BAPIUPDATE Cantidad de pedido
//PO_UNIT BAPIUPDATE  Unidad de medida de pedido
//NET_PRICE   BAPIUPDATE Importe de moneda para BAPIs(con 9 decimales)
//PRICE_UNIT BAPIUPDATE  Cantidad base
//GR_PR_TIME BAPIUPDATE  Tiempo de tratamiento para la entrada de mercancía en días
//TAX_CODE BAPIUPDATE  Indicador IVA
//VAL_TYPE BAPIUPDATE  Clase de valoración
//NO_MORE_GR  BAPIUPDATE Indicador de entrega final
//FINAL_INV   BAPIUPDATE Indicador de factura final
//ITEM_CAT    BAPIUPDATE Tipo de posición del documento de compras
//ACCTASSCAT BAPIUPDATE  Tipo de imputación
//DISTRIB BAPIUPDATE Indicador de distribución en la imputación múltiple
//PART_INV BAPIUPDATE  Indicador de factura parcial
//GR_IND BAPIUPDATE  Indicador de entrada de mercancías
//GR_NON_VAL  BAPIUPDATE Entrada de mercancías no valorada
//IR_IND BAPIUPDATE  Indicador de recepción de factura
//FREE_ITEM   BAPIUPDATE Posición sin cargo
//GR_BASEDIV BAPIUPDATE  Indicador p.verificación de facturas sobre la base de la EM
//ACKN_REQD   BAPIUPDATE Indicador de obligación de confirmación de pedido
//ACKNOWL_NO BAPIUPDATE  Número de confirmación de pedido
//AGREEMENT   BAPIUPDATE Número del contrato superior
//AGMT_ITEM   BAPIUPDATE Número de posición del contrato superior
//RFQ_NO  BAPIUPDATE Núm.petición oferta
//RFQ_ITEM BAPIUPDATE  Número de posición de la petición de oferta
//PREQ_NO BAPIUPDATE  Número de la solicitud de pedido
//PREQ_ITEM BAPIUPDATE  Número de posición de la solicitud de pedido
//PCKG_NO BAPIUPDATE  Numero de Paquete

//Nombre: ZBAPIMEPOADDREDELIVERY Denominación:	Direcciones de entrega
//Nombre  Dominio / Tipo  Denominación
//PO_ITEM EBELP Número de posición de pedido
//ADDR_NO AD_ADDRNUM  Nº dirección
//NAME AD_NAME1    Nombre 1
//POSTL_COD1 AD_PSTCD1   Código postal de la población
//CITY    AD_CITY1 Población
//STREET AD_STREET   Calle
//STREET_NO   AD_STRNUM Codificación de la calle para fichero de población y calle
//TEL1_NUMBR  AD_TLNMBR1 Primer número teléfono: Prefijo + número


//Nombre: ZBAPIMEPOACCOUNT Denominación:	Imputación
//Nombre  Dominio / Tipo  Denominación
//PI_ITEM EBELP Numero de posición del pedido
//SERIAL_NO DZEKKN  Número actual de la imputación
//DELETE_IND  KLOEK Indicador de borrado imputación del documento de compras
//QUANTITY    MENGE_D Cantidad
//GL_ACCOUNT SAKNR   Número de la cuenta de mayor
//BUS_AREA GSBER   División
//COSTCENTER  KOSTL Centro de coste
//ASSET_NO ANLN1   Número principal de activo fijo
//SUB_NUMBER  ANLN2 Subnúmero de activo fijo
//ORDERID AUFNR Número de orden
//CO_AREA KOKRS   Sociedad CO
//COSTOBJECT KSTRG   Objeto de coste
//PROFIT_CTR  PRCTR Centro de beneficio


//Nombre: ZBAPIMEREQACCOUNTX Denominación:	Change Toolbar for Enjoy Purchase Req. - Imputación
//Nombre  Dominio / Tipo  Denominación
//PO_ITEM EBELP   Número de posición de pedido
//SERIAL_NO   DZEKKN  Número actual de la imputación
//PO_ITEMX    BAPIUPDATE  Posición con cambios
//SERIAL_NOX  BAPIUPDATE
//DELETE_IND  BAPIUPDATE  Indicador de borrado imputación del documento de compras
//QUANTITY    BAPIUPDATE  Cantidad
//GL_ACCOUNT  BAPIUPDATE  Número de la cuenta de mayor
//BUS_AREA    BAPIUPDATE  División
//COSTCENTER  BAPIUPDATE  Centro de coste
//ASSET_NO    BAPIUPDATE  Número principal de activo fijo
//SUB_NUMBER  BAPIUPDATE  Subnúmero de activo fijo
//ORDERID BAPIUPDATE  Número de orden
//CO_AREA BAPIUPDATE  Sociedad CO
//COSTOBJECT  BAPIUPDATE  Objeto de coste


//Nombre: ZBAPIMEPOCONDHEADER Denominación:	Posición de Servicio
//Nombre  Dominio / Tipo  Denominación
//CONDITION_NO    KNUMV Número de la condición de documento
//ITM_NUMBER  KPOSN Número de posición de la condición
//COND_ST_NO  STUNR Número de paso
//COND_COUNT DZAEHK_SHORT    Contador de condiciones(longitud corta)
//COND_TYPE KSCHA   Clase de condición
//COND_VALUE  BAPIKBETR1 Importe de condición
//CURRENCY WAERS   Clase de Moneda
//CHANGE_ID   MEINS Unidad de medida base
//CALCTYPCON KRECH   Regla de cálculo para la condición
//CONDCLASS KOAID   Categoría de condición



//Nombre: ZBAPIMEPOCONDHEADERX Denominación:	Posición de Servicio
//Nombre  Dominio / Tipo  Denominación
//CONDITION_NO    KNUMV Número de la condición de documento
//ITM_NUMBER  KPOSN Número de posición de la condición
//COND_ST_NO  STUNR Número de paso
//CONDITION_NOX BAPIUPDATE
//COND_COUNT BAPIUPDATE  Contador de condiciones(longitud corta)
//COND_TYPE BAPIUPDATE  Clase de condición
//COND_VALUE  BAPIUPDATE Importe de condición
//CURRENCY BAPIUPDATE  Clase de Moneda
//CHANGE_ID   BAPIUPDATE Unidad de medida base
//CALCTYPCON BAPIUPDATE  Regla de cálculo para la condición
//CONDCLASS BAPIUPDATE  Categoría de condición


//Nombre: ZBAPIMEPOCOND Denominación:	Posición de Servicio
//Nombre  Dominio / Tipo  Denominación
//CONDITION_NO    KNUMV Número de la condición de documento
//ITM_NUMBER  KPOSN Número de posición de la condición
//COND_ST_NO  STUNR Número de paso
//COND_COUNT DZAEHK_SHORT    Contador de condiciones(longitud corta)
//COND_TYPE KSCHA   Clase de condición
//COND_VALUE  BAPIKBETR1 Importe de condición
//CURRENCY WAERS   Clase de Moneda
//CHANGE_ID   MEINS Unidad de medida base
//CALCTYPCON KRECH   Regla de cálculo para la condición
//CONDCLASS KOAID   Categoría de condición


//Nombre: ZBAPIMEPOCONDX       Denominación: Posición de Servicio
//Nombre  Dominio / Tipo  Denominación
//CONDITION_NO    KNUMV Número de la condición de documento
//ITM_NUMBER  KPOSN Número de posición de la condición
//COND_ST_NO  STUNR Número de paso
//CONDITION_NOX BAPIUPDATE
//COND_COUNT BAPIUPDATE  Contador de condiciones(longitud corta)
//COND_TYPE BAPIUPDATE  Clase de condición
//COND_VALUE  BAPIUPDATE Importe de condición
//CURRENCY BAPIUPDATE  Clase de Moneda
//CHANGE_ID   BAPIUPDATE Unidad de medida base
//CALCTYPCON BAPIUPDATE  Regla de cálculo para la condición
//CONDCLASS BAPIUPDATE  Categoría de condición




