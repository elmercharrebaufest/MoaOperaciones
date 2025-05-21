using Newtonsoft.Json;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.ModificarSolpWebServiceMOA;
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
    public class ModificarSolpConsumerMOA : IModificarSolpConsumerMOA
    {
        private readonly IRepositorio repositorio;
        private readonly string rutaArchivosXmls = ConfigurationManager.AppSettings["RutaArchivosCompras"];
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public ModificarSolpConsumerMOA(IRepositorio repositorio)
        {

            this.repositorio = repositorio;
        }

        public ModificarSolpConsumerMOAResponse Request(SolpSAPDto solpSAP)
        {
            SI_MMRFC_MODIFICAR_SOLPEDClient service;
            var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=SI_MMRFC_MODIFICAR_SOLPED&amp;interfaceNamespace=urn%3AOPERACIONES";
            service = new SI_MMRFC_MODIFICAR_SOLPEDClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();

            var serxml = new System.Xml.Serialization.XmlSerializer(solpSAP.GetType());
            var ms = new MemoryStream();
            serxml.Serialize(ms, solpSAP);
            string xml = Encoding.UTF8.GetString(ms.ToArray());

            var fecha = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");

            var nombreArchivoLlamada = string.Concat(solpSAP.Id, " - ", fecha, " - llamada modificar.xml");
            var nombreArchivoRespuesta = string.Concat(solpSAP.Id, " - ", fecha, " - respuesta modificar.xml");

            var rutaArchivoLlamada = Path.Combine(rutaArchivosXmls, "XMLS", nombreArchivoLlamada);
            var rutaArchivoRespuesta = Path.Combine(rutaArchivosXmls, "XMLS", nombreArchivoRespuesta);

            FileInfo fileCrear = new FileInfo(rutaArchivoLlamada);
            fileCrear.Directory.Create();
            File.WriteAllText(fileCrear.FullName, xml);

            string IM_NUMBER = solpSAP.NroSolp;
            ModificarSolpWebServiceMOA.BAPIMEREQACCOUNT[] IM_PRACCOUNT = solpSAP.IM_PRACCOUNTList
                .Select(a => new ModificarSolpWebServiceMOA.BAPIMEREQACCOUNT
                {
                    ASSET_NO = a.ASSET_NO,
                    BUS_AREA = a.BUS_AREA,
                    COSTCENTER = a.COSTCENTER,
                    COSTOBJECT = a.COSTOBJECT,
                    CO_AREA = a.CO_AREA,
                    GL_ACCOUNT = a.GL_ACCOUNT,
                    ORDERID = a.ORDERID,
                    PREQ_ITEM = a.PREQ_ITEM,
                    PROFIT_CTR = a.PROFIT_CTR,
                    SUB_NUMBER = a.SUB_NUMBER,
                    QUANTITY = a.QUANTITY,
                    QUANTITYSpecified = a.QUANTITYSpecified,
                    SERIAL_NO = a.SERIAL_NO
                })
                .ToArray();
            ModificarSolpWebServiceMOA.BAPIMEREQACCOUNTX[] IM_PRACCOUNTX = solpSAP.IM_PRACCOUNTXList
                .Select(a => new ModificarSolpWebServiceMOA.BAPIMEREQACCOUNTX
                {
                    ASSET_NO = a.ASSET_NO,
                    BUS_AREA = a.BUS_AREA,
                    COSTCENTER = a.COSTCENTER,
                    COSTOBJECT = a.COSTOBJECT,
                    CO_AREA = a.CO_AREA,
                    GL_ACCOUNT = a.GL_ACCOUNT,
                    ORDERID = a.ORDERID,
                    PREQ_ITEM = a.PREQ_ITEM,
                    PROFIT_CTR = a.PROFIT_CTR,
                    QUANTITY = a.QUANTITY,
                    PREQ_ITEMX = a.PREQ_ITEMX,
                    SERIAL_NOX = a.SERIAL_NOX,
                    SUB_NUMBER = a.SUB_NUMBER,
                    SERIAL_NO = a.SERIAL_NO
                })
                .ToArray();
            ModificarSolpWebServiceMOA.ZMPES7110[] IM_PRADDRDELIVERY = solpSAP.IM_PRADDRDELIVERYList
                .Select(a => new ModificarSolpWebServiceMOA.ZMPES7110
                {
                    ADDR_NO = a.ADDR_NO,
                    CITY = a.CITY,
                    HOUSE_NO = a.HOUSE_NO,
                    NAME = a.NAME,
                    POSTL_COD1 = a.POSTL_COD1,
                    PREQ_ITEM = a.PREQ_ITEM,
                    PREQ_NO = a.PREQ_NO,
                    STREET = a.STREET,
                    TEL1_NUMBR = a.TEL1_NUMBR
                })
                .ToArray();
            ModificarSolpWebServiceMOA.BAPIMEREQHEADTEXT[] IM_PRHEADERTEXT = solpSAP.IM_PRHEADERTEXTList
                .Select(a => new ModificarSolpWebServiceMOA.BAPIMEREQHEADTEXT
                {
                    PREQ_ITEM = a.PREQ_ITEM,
                    PREQ_NO = a.PREQ_NO,
                    TEXT_FORM = a.TEXT_FORM,
                    TEXT_ID = a.TEXT_ID,
                    TEXT_LINE = a.TEXT_LINE
                })
                .ToArray();
            ModificarSolpWebServiceMOA.ZMPES7090[] IM_PRITEM = solpSAP.IM_PRITEMList
                .Select(a => new ModificarSolpWebServiceMOA.ZMPES7090
                {
                    ACCTASSCAT = a.ACCTASSCAT,
                    PREQ_ITEM = a.PREQ_ITEM,
                    AGMT_ITEM = a.AGMT_ITEM,
                    AGREEMENT = a.AGREEMENT,
                    CLOSED = a.CLOSED,
                    CREATED_BY = a.CREATED_BY,
                    CURRENCY = a.CURRENCY,
                    CURRENCY_ISO = a.CURRENCY_ISO,
                    DELETE_IND = a.DELETE_IND,
                    DELIV_DATE = a.DELIV_DATE,
                    DES_VENDOR = a.DES_VENDOR,
                    FIXED_VEND = a.FIXED_VEND,
                    GR_PR_TIME = a.GR_PR_TIME,
                    GR_PR_TIMESpecified = a.GR_PR_TIMESpecified,
                    INFO_REC = a.INFO_REC,
                    ITEM_CAT = a.ITEM_CAT,
                    MATERIAL = a.MATERIAL,
                    MATL_GROUP = a.MATL_GROUP,
                    PCKG_NO = a.PCKG_NO,
                    PLANT = a.PLANT,
                    PLND_DELRY = a.PLND_DELRY,
                    PLND_DELRYSpecified = a.PLND_DELRYSpecified,
                    PREQ_DATE = a.PREQ_DATE,
                    PREQ_NAME = a.PREQ_NAME,
                    PREQ_PRICE = a.PREQ_PRICE,
                    PREQ_PRICESpecified = a.PREQ_PRICESpecified,
                    PREQ_UNIT_ISO = a.PREQ_UNIT_ISO,
                    PRICE_UNIT = a.PRICE_UNIT,
                    PRICE_UNITSpecified = a.PRICE_UNITSpecified,
                    PURCH_ORG = a.PURCH_ORG,
                    PUR_GROUP = a.PUR_GROUP,
                    QUANTITY = a.QUANTITY,
                    QUANTITYSpecified = a.QUANTITYSpecified,
                    REL_DATE = a.REL_DATE,
                    SHORT_TEXT = a.SHORT_TEXT,
                    STORE_LOC = a.STORE_LOC,
                    TRACKINGNO = a.TRACKINGNO,
                    UNIT = a.UNIT,
                    VAL_TYPE = a.VAL_TYPE
                })
                .ToArray();
            ModificarSolpWebServiceMOA.BAPIMEREQITEMTEXT[] IM_PRITEMTEXT = solpSAP.IM_PRITEMTEXTList
                .Select(a => new ModificarSolpWebServiceMOA.BAPIMEREQITEMTEXT
                {
                    PREQ_ITEM = a.PREQ_ITEM,
                    PREQ_NO = a.PREQ_NO,
                    TEXT_FORM = a.TEXT_FORM,
                    TEXT_ID = a.TEXT_ID,
                    TEXT_LINE = a.TEXT_LINE
                })
                .ToArray();
            ModificarSolpWebServiceMOA.ZMPES8000[] IM_PRITEMX = solpSAP.IM_PRITEMXList
                .Select(a => new ModificarSolpWebServiceMOA.ZMPES8000
                {
                    ACCTASSCAT = a.ACCTASSCAT,
                    PREQ_ITEM = a.PREQ_ITEM,
                    AGMT_ITEM = a.AGMT_ITEM,
                    AGREEMENT = a.AGREEMENT,
                    CLOSED = a.CLOSED,
                    CREATED_BY = a.CREATED_BY,
                    CURRENCY = a.CURRENCY,
                    CURRENCY_ISO = a.CURRENCY_ISO,
                    DELETE_IND = a.DELETE_IND,
                    DELIV_DATE = a.DELIV_DATE,
                    DES_VENDOR = a.DES_VENDOR,
                    FIXED_VEND = a.FIXED_VEND,
                    GR_PR_TIME = a.GR_PR_TIME,
                    INFO_REC = a.INFO_REC,
                    ITEM_CAT = a.ITEM_CAT,
                    MATERIAL = a.MATERIAL,
                    MATL_GROUP = a.MATL_GROUP,
                    PCKG_NO = a.PCKG_NO,
                    PLANT = a.PLANT,
                    PLND_DELRY = a.PLND_DELRY,
                    PREQ_DATE = a.PREQ_DATE,
                    PREQ_NAME = a.PREQ_NAME,
                    PREQ_PRICE = a.PREQ_PRICE,
                    PREQ_UNIT_ISO = a.PREQ_UNIT_ISO,
                    PRICE_UNIT = a.PRICE_UNIT,
                    PURCH_ORG = a.PURCH_ORG,
                    PUR_GROUP = a.PUR_GROUP,
                    QUANTITY = a.QUANTITY,
                    REL_DATE = a.REL_DATE,
                    SHORT_TEXT = a.SHORT_TEXT,
                    STORE_LOC = a.STORE_LOC,
                    TRACKINGNO = a.TRACKINGNO,
                    UNIT = a.UNIT,
                    VAL_TYPE = a.VAL_TYPE,
                    PREQ_ITEMX = a.PREQ_ITEMX
                })
                .ToArray();
            ModificarSolpWebServiceMOA.BAPI_SRV_ACC_DATA[] IM_SERVICEACCOUNT = solpSAP.IM_SERVICEACCOUNTList
                .Select(a => new ModificarSolpWebServiceMOA.BAPI_SRV_ACC_DATA
                {
                    DEL_IND = a.DEL_IND,
                    DOC_ITEM = a.DOC_ITEM,
                    NET_VALUE = a.NET_VALUE,
                    NET_VALUESpecified = a.NET_VALUESpecified,
                    QUANTITY = a.QUANTITY,
                    OUTLINE = a.OUTLINE,
                    PERCENT = a.PERCENT,
                    PERCENTSpecified = a.PERCENTSpecified,
                    QUANTITYSpecified = a.QUANTITYSpecified,
                    SERIAL_NO = a.SERIAL_NO,
                    SERIAL_NO_ITEM = a.SERIAL_NO_ITEM,
                    SRV_LINE = a.SRV_LINE
                })
                .ToArray();
            ModificarSolpWebServiceMOA.BAPI_SRV_ACC_DATAX[] IM_SERVICEACCOUNTX = solpSAP.IM_SERVICEACCOUNTXList
                .Select(a => new ModificarSolpWebServiceMOA.BAPI_SRV_ACC_DATAX
                {
                    DEL_IND = a.DEL_IND,
                    DOC_ITEM = a.DOC_ITEM,
                    NET_VALUE = a.NET_VALUE,
                    QUANTITY = a.QUANTITY,
                    OUTLINE = a.OUTLINE,
                    PERCENT = a.PERCENT,
                    SERIAL_NO = a.SERIAL_NO,
                    SERIAL_NO_ITEM = a.SERIAL_NO_ITEM,
                    SRV_LINE = a.SRV_LINE
                })
                .ToArray();
            ModificarSolpWebServiceMOA.BAPI_SRV_SERVICE_LINE[] IM_SERVICELINES = solpSAP.IM_SERVICELINESList
                .Select(a => new ModificarSolpWebServiceMOA.BAPI_SRV_SERVICE_LINE
                {
                    SRV_LINE = a.SRV_LINE,
                    QUANTITYSpecified = a.QUANTITYSpecified,
                    OUTLINE = a.OUTLINE,
                    QUANTITY = a.QUANTITY,
                    CURRENCY = a.CURRENCY,
                    DEL_IND = a.DEL_IND,
                    DOC_ITEM = a.DOC_ITEM,
                    GROSS_PRICE = a.GROSS_PRICE,
                    GROSS_PRICESpecified = a.GROSS_PRICESpecified,
                    MATL_GROUP = a.MATL_GROUP,
                    NET_PRICE = a.NET_PRICE,
                    NET_PRICESpecified = a.NET_PRICESpecified,
                    SERVICE = a.SERVICE,
                    SHORT_TEXT = a.SHORT_TEXT,
                    UOM = a.UOM,
                    UOM_ISO = a.UOM_ISO
                })
                .ToArray();
            ModificarSolpWebServiceMOA.BAPI_SRV_SERVICE_LINEX[] IM_SERVICELINESX = solpSAP.IM_SERVICELINESXList
                .Select(a => new ModificarSolpWebServiceMOA.BAPI_SRV_SERVICE_LINEX
                {
                    SRV_LINE = a.SRV_LINE,
                    OUTLINE = a.OUTLINE,
                    QUANTITY = a.QUANTITY,
                    CURRENCY = a.CURRENCY,
                    DEL_IND = a.DEL_IND,
                    DOC_ITEM = a.DOC_ITEM,
                    GROSS_PRICE = a.GROSS_PRICE,
                    MATL_GROUP = a.MATL_GROUP,
                    NET_PRICE = a.NET_PRICE,
                    SERVICE = a.SERVICE,
                    SHORT_TEXT = a.SHORT_TEXT,
                    UOM = a.UOM,
                    UOM_ISO = a.UOM_ISO
                })
                .ToArray();


            var result = service.SI_MMRFC_MODIFICAR_SOLPED(
                                                       IM_NUMBER,
                                                       IM_PRACCOUNT,
                                                       IM_PRACCOUNTX,
                                                       IM_PRADDRDELIVERY,
                                                       IM_PRHEADERTEXT,
                                                       IM_PRITEM,
                                                       IM_PRITEMTEXT,
                                                       IM_PRITEMX,
                                                       IM_SERVICEACCOUNT,
                                                       IM_SERVICEACCOUNTX,
                                                       IM_SERVICELINES,
                                                       IM_SERVICELINESX,
                                                       out ModificarSolpWebServiceMOA.BAPIRETURN[] EX_RETURN);


            var respuesta = new ModificarSolpConsumerMOAResponse();

            respuesta.Resultado = result;
            respuesta.Errores = new List<ModificarSolpConsumerMOAError>();

            foreach (var errorSAP in EX_RETURN)
            {
                var error = new ModificarSolpConsumerMOAError
                {
                    Codigo = errorSAP.CODE,
                    Mensaje = errorSAP.MESSAGE,
                    Tipo = errorSAP.TYPE
                };

                respuesta.Errores.Add(error);
            }

            var jsonRespuesta = JsonConvert.SerializeObject(respuesta);

            FileInfo fileRespuesta = new FileInfo(rutaArchivoRespuesta);
            fileRespuesta.Directory.Create();
            File.WriteAllText(fileRespuesta.FullName, jsonRespuesta);

            return respuesta;
        }

        public ModificarSolpConsumerMOAResponse RequestSinPI(SolpSAPSinPIDto solpSAP)
        {
            var serxml = new System.Xml.Serialization.XmlSerializer(solpSAP.GetType());
            var ms = new MemoryStream();
            serxml.Serialize(ms, solpSAP);
            string xml = Encoding.UTF8.GetString(ms.ToArray());

            var fecha = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");

            var nombreArchivoLlamada = string.Concat(solpSAP.Id, " - ", fecha, " - llamada modificar.xml");
            var nombreArchivoRespuesta = string.Concat(solpSAP.Id, " - ", fecha, " - respuesta modificar.xml");

            var rutaArchivoLlamada = Path.Combine(rutaArchivosXmls, "XMLS", nombreArchivoLlamada);
            var rutaArchivoRespuesta = Path.Combine(rutaArchivosXmls, "XMLS", nombreArchivoRespuesta);

            FileInfo fileCrear = new FileInfo(rutaArchivoLlamada);
            fileCrear.Directory.Create();
            File.WriteAllText(fileCrear.FullName, xml);

            var agent = new Z_WS_MOAOP_COMPRAS_DIRECTClient();
            agent.ClientCredentials.UserName.UserName = UserSap;
            agent.ClientCredentials.UserName.Password = PassSap;

            string IM_NUMBER = solpSAP.NroSolp;
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEREQACCOUNT[] IM_PRACCOUNT = solpSAP.IM_PRACCOUNTList
                .Select(a => new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEREQACCOUNT
                {
                    ASSET_NO = a.ASSET_NO,
                    BUS_AREA = a.BUS_AREA,
                    COSTCENTER = a.COSTCENTER,
                    COSTOBJECT = a.COSTOBJECT,
                    CO_AREA = a.CO_AREA,
                    GL_ACCOUNT = a.GL_ACCOUNT,
                    ORDERID = a.ORDERID,
                    PREQ_ITEM = a.PREQ_ITEM,
                    PROFIT_CTR = a.PROFIT_CTR,
                    SUB_NUMBER = a.SUB_NUMBER,
                    QUANTITY = a.QUANTITY,
                    //QUANTITYSpecified = a.QUANTITYSpecified,
                    SERIAL_NO = a.SERIAL_NO
                })
                .ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEREQACCOUNTX[] IM_PRACCOUNTX = solpSAP.IM_PRACCOUNTXList
                .Select(a => new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEREQACCOUNTX
                {
                    ASSET_NO = a.ASSET_NO,
                    BUS_AREA = a.BUS_AREA,
                    COSTCENTER = a.COSTCENTER,
                    COSTOBJECT = a.COSTOBJECT,
                    CO_AREA = a.CO_AREA,
                    GL_ACCOUNT = a.GL_ACCOUNT,
                    ORDERID = a.ORDERID,
                    PREQ_ITEM = a.PREQ_ITEM,
                    PROFIT_CTR = a.PROFIT_CTR,
                    QUANTITY = a.QUANTITY,
                    PREQ_ITEMX = a.PREQ_ITEMX,
                    SERIAL_NOX = a.SERIAL_NOX,
                    SUB_NUMBER = a.SUB_NUMBER,
                    SERIAL_NO = a.SERIAL_NO
                })
                .ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES7110[] IM_PRADDRDELIVERY = solpSAP.IM_PRADDRDELIVERYList
                .Select(a => new WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES7110
                {
                    ADDR_NO = a.ADDR_NO,
                    CITY = a.CITY,
                    HOUSE_NO = a.HOUSE_NO,
                    NAME = a.NAME,
                    POSTL_COD1 = a.POSTL_COD1,
                    PREQ_ITEM = a.PREQ_ITEM,
                    PREQ_NO = a.PREQ_NO,
                    STREET = a.STREET,
                    TEL1_NUMBR = a.TEL1_NUMBR
                })
                .ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEREQHEADTEXT[] IM_PRHEADERTEXT = solpSAP.IM_PRHEADERTEXTList
                .Select(a => new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEREQHEADTEXT
                {
                    PREQ_ITEM = a.PREQ_ITEM,
                    PREQ_NO = a.PREQ_NO,
                    TEXT_FORM = a.TEXT_FORM,
                    TEXT_ID = a.TEXT_ID,
                    TEXT_LINE = a.TEXT_LINE
                })
                .ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES7090[] IM_PRITEM = solpSAP.IM_PRITEMList
                .Select(a => new WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES7090
                {
                    ACCTASSCAT = a.ACCTASSCAT,
                    PREQ_ITEM = a.PREQ_ITEM,
                    AGMT_ITEM = a.AGMT_ITEM,
                    AGREEMENT = a.AGREEMENT,
                    CLOSED = a.CLOSED,
                    CREATED_BY = a.CREATED_BY,
                    CURRENCY = a.CURRENCY,
                    CURRENCY_ISO = a.CURRENCY_ISO,
                    DELETE_IND = a.DELETE_IND,
                    DELIV_DATE = a.DELIV_DATE,
                    DES_VENDOR = a.DES_VENDOR,
                    FIXED_VEND = a.FIXED_VEND,
                    GR_PR_TIME = a.GR_PR_TIME,
                    //GR_PR_TIMESpecified = a.GR_PR_TIMESpecified,
                    INFO_REC = a.INFO_REC,
                    ITEM_CAT = a.ITEM_CAT,
                    MATERIAL = a.MATERIAL,
                    MATL_GROUP = a.MATL_GROUP,
                    PCKG_NO = a.PCKG_NO,
                    PLANT = a.PLANT,
                    PLND_DELRY = a.PLND_DELRY,
                    //PLND_DELRYSpecified = a.PLND_DELRYSpecified,
                    PREQ_DATE = a.PREQ_DATE,
                    PREQ_NAME = a.PREQ_NAME,
                    PREQ_PRICE = a.PREQ_PRICE,
                    //PREQ_PRICESpecified = a.PREQ_PRICESpecified,
                    PREQ_UNIT_ISO = a.PREQ_UNIT_ISO,
                    PRICE_UNIT = a.PRICE_UNIT,
                    //PRICE_UNITSpecified = a.PRICE_UNITSpecified,
                    PURCH_ORG = a.PURCH_ORG,
                    PUR_GROUP = a.PUR_GROUP,
                    QUANTITY = a.QUANTITY,
                    //QUANTITYSpecified = a.QUANTITYSpecified,
                    REL_DATE = a.REL_DATE,
                    SHORT_TEXT = a.SHORT_TEXT,
                    STORE_LOC = a.STORE_LOC,
                    TRACKINGNO = a.TRACKINGNO,
                    UNIT = a.UNIT,
                    VAL_TYPE = a.VAL_TYPE
                })
                .ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEREQITEMTEXT[] IM_PRITEMTEXT = solpSAP.IM_PRITEMTEXTList
                .Select(a => new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEREQITEMTEXT
                {
                    PREQ_ITEM = a.PREQ_ITEM,
                    PREQ_NO = a.PREQ_NO,
                    TEXT_FORM = a.TEXT_FORM,
                    TEXT_ID = a.TEXT_ID,
                    TEXT_LINE = a.TEXT_LINE
                })
                .ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES8000[] IM_PRITEMX = solpSAP.IM_PRITEMXList
                .Select(a => new WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES8000
                {
                    ACCTASSCAT = a.ACCTASSCAT,
                    PREQ_ITEM = a.PREQ_ITEM,
                    AGMT_ITEM = a.AGMT_ITEM,
                    AGREEMENT = a.AGREEMENT,
                    CLOSED = a.CLOSED,
                    CREATED_BY = a.CREATED_BY,
                    CURRENCY = a.CURRENCY,
                    CURRENCY_ISO = a.CURRENCY_ISO,
                    DELETE_IND = a.DELETE_IND,
                    DELIV_DATE = a.DELIV_DATE,
                    DES_VENDOR = a.DES_VENDOR,
                    FIXED_VEND = a.FIXED_VEND,
                    GR_PR_TIME = a.GR_PR_TIME,
                    INFO_REC = a.INFO_REC,
                    ITEM_CAT = a.ITEM_CAT,
                    MATERIAL = a.MATERIAL,
                    MATL_GROUP = a.MATL_GROUP,
                    PCKG_NO = a.PCKG_NO,
                    PLANT = a.PLANT,
                    PLND_DELRY = a.PLND_DELRY,
                    PREQ_DATE = a.PREQ_DATE,
                    PREQ_NAME = a.PREQ_NAME,
                    PREQ_PRICE = a.PREQ_PRICE,
                    PREQ_UNIT_ISO = a.PREQ_UNIT_ISO,
                    PRICE_UNIT = a.PRICE_UNIT,
                    PURCH_ORG = a.PURCH_ORG,
                    PUR_GROUP = a.PUR_GROUP,
                    QUANTITY = a.QUANTITY,
                    REL_DATE = a.REL_DATE,
                    SHORT_TEXT = a.SHORT_TEXT,
                    STORE_LOC = a.STORE_LOC,
                    TRACKINGNO = a.TRACKINGNO,
                    UNIT = a.UNIT,
                    VAL_TYPE = a.VAL_TYPE,
                    PREQ_ITEMX = a.PREQ_ITEMX
                })
                .ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPI_SRV_ACC_DATA[] IM_SERVICEACCOUNT = solpSAP.IM_SERVICEACCOUNTList
                .Select(a => new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPI_SRV_ACC_DATA
                {
                    DEL_IND = a.DEL_IND,
                    DOC_ITEM = a.DOC_ITEM,
                    NET_VALUE = a.NET_VALUE,
                    //NET_VALUESpecified = a.NET_VALUESpecified,
                    QUANTITY = a.QUANTITY,
                    OUTLINE = a.OUTLINE,
                    PERCENT = a.PERCENT,
                    //PERCENTSpecified = a.PERCENTSpecified,
                    //QUANTITYSpecified = a.QUANTITYSpecified,
                    SERIAL_NO = a.SERIAL_NO,
                    SERIAL_NO_ITEM = a.SERIAL_NO_ITEM,
                    SRV_LINE = a.SRV_LINE
                })
                .ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPI_SRV_ACC_DATAX[] IM_SERVICEACCOUNTX = solpSAP.IM_SERVICEACCOUNTXList
                .Select(a => new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPI_SRV_ACC_DATAX
                {
                    DEL_IND = a.DEL_IND,
                    DOC_ITEM = a.DOC_ITEM,
                    NET_VALUE = a.NET_VALUE,
                    QUANTITY = a.QUANTITY,
                    OUTLINE = a.OUTLINE,
                    PERCENT = a.PERCENT,
                    SERIAL_NO = a.SERIAL_NO,
                    SERIAL_NO_ITEM = a.SERIAL_NO_ITEM,
                    SRV_LINE = a.SRV_LINE
                })
                .ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPI_SRV_SERVICE_LINE[] IM_SERVICELINES = solpSAP.IM_SERVICELINESList
                .Select(a => new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPI_SRV_SERVICE_LINE
                {
                    SRV_LINE = a.SRV_LINE,
                    //QUANTITYSpecified = a.QUANTITYSpecified,
                    OUTLINE = a.OUTLINE,
                    QUANTITY = a.QUANTITY,
                    CURRENCY = a.CURRENCY,
                    DEL_IND = a.DEL_IND,
                    DOC_ITEM = a.DOC_ITEM,
                    GROSS_PRICE = a.GROSS_PRICE,
                    //GROSS_PRICESpecified = a.GROSS_PRICESpecified,
                    MATL_GROUP = a.MATL_GROUP,
                    NET_PRICE = a.NET_PRICE,
                    //NET_PRICESpecified = a.NET_PRICESpecified,
                    SERVICE = a.SERVICE,
                    SHORT_TEXT = a.SHORT_TEXT,
                    UOM = a.UOM,
                    UOM_ISO = a.UOM_ISO
                })
                .ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPI_SRV_SERVICE_LINEX[] IM_SERVICELINESX = solpSAP.IM_SERVICELINESXList
                .Select(a => new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPI_SRV_SERVICE_LINEX
                {
                    SRV_LINE = a.SRV_LINE,
                    OUTLINE = a.OUTLINE,
                    QUANTITY = a.QUANTITY,
                    CURRENCY = a.CURRENCY,
                    DEL_IND = a.DEL_IND,
                    DOC_ITEM = a.DOC_ITEM,
                    GROSS_PRICE = a.GROSS_PRICE,
                    MATL_GROUP = a.MATL_GROUP,
                    NET_PRICE = a.NET_PRICE,
                    SERVICE = a.SERVICE,
                    SHORT_TEXT = a.SHORT_TEXT,
                    UOM = a.UOM,
                    UOM_ISO = a.UOM_ISO
                })
                .ToArray();

            var request = new Z_MMRFC_MODIFICAR_SOLPED()
            {
                IM_NUMBER = IM_NUMBER,
                IM_PRACCOUNT = IM_PRACCOUNT,
                IM_PRACCOUNTX = IM_PRACCOUNTX,
                IM_PRADDRDELIVERY = IM_PRADDRDELIVERY,
                IM_PRHEADERTEXT = IM_PRHEADERTEXT,
                IM_PRITEM = IM_PRITEM,
                IM_PRITEMTEXT = IM_PRITEMTEXT,
                IM_PRITEMX = IM_PRITEMX,
                IM_SERVICEACCOUNT = IM_SERVICEACCOUNT,
                IM_SERVICEACCOUNTX = IM_SERVICEACCOUNTX,
                IM_SERVICELINES = IM_SERVICELINES,
                IM_SERVICELINESX = IM_SERVICELINESX

            };
            Log.Info($"SAP sin PI Z_MMRFC_MODIFICAR_SOLPED request");
            Log.Info(request.ToXml());
            var response = agent.Z_MMRFC_MODIFICAR_SOLPED(request);
            var respuesta = new ModificarSolpConsumerMOAResponse();
            Log.Info($"SAP sin PI Z_MMRFC_MODIFICAR_SOLPED response");
            Log.Info(response.ToXml());
            respuesta.Resultado = response.EX_EXITO;
            respuesta.Errores = new List<ModificarSolpConsumerMOAError>();

            foreach (var errorSAP in response.EX_RETURN)
            {
                var error = new ModificarSolpConsumerMOAError
                {
                    Codigo = errorSAP.CODE,
                    Mensaje = errorSAP.MESSAGE,
                    Tipo = errorSAP.TYPE
                };

                respuesta.Errores.Add(error);
            }

            var jsonRespuesta = JsonConvert.SerializeObject(respuesta);

            FileInfo fileRespuesta = new FileInfo(rutaArchivoRespuesta);
            fileRespuesta.Directory.Create();
            File.WriteAllText(fileRespuesta.FullName, jsonRespuesta);

            return respuesta;
        }

    }
    public class ModificarSolpConsumerMOAResponse
    {
        public string NumeroSolp { get; set; }
        public List<ModificarSolpConsumerMOAError> Errores { get; set; }
        public string Resultado { get; internal set; }
    }

    public class ModificarSolpConsumerMOAError
    {
        public string Codigo { get; set; }
        public string Mensaje { get; set; }
        public string Tipo { get; set; }
    }


    public interface IModificarSolpConsumerMOA
    {
        ModificarSolpConsumerMOAResponse Request(SolpSAPDto solpSAP);
        ModificarSolpConsumerMOAResponse RequestSinPI(SolpSAPSinPIDto solpSAP);
    }

}
