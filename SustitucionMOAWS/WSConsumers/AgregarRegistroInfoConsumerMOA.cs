using Microsoft.Win32;
using Newtonsoft.Json;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAWS.AgregarRegistroInfoServiceWebMOA;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MOAOP;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WSConsumers
{
    public class AgregarRegistroInfoConsumerMOA : IAgregarRegistroInfoConsumerMOA
    {
        private readonly IRepositorio repositorio;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public AgregarRegistroInfoConsumerMOA(IRepositorio repositorio)
        {

            this.repositorio = repositorio;
        }

        public CrearSolpConsumerMOAResponse AgregarRegistroInfo(List<RegistroInfoDto> registrosInfo)
        {

            if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
            {
                var agent = new Z_WS_MOAOP_DIRECTClient();
                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;

                WS_GAQ_sin_PI_DIRECT_MOAOP.BAPIRETURN[] BAPIRETURNE       = new WS_GAQ_sin_PI_DIRECT_MOAOP.BAPIRETURN[] { };
                WS_GAQ_sin_PI_DIRECT_MOAOP.MEWIPIRTEXT[] MEWIPIRTEXTE     = new WS_GAQ_sin_PI_DIRECT_MOAOP.MEWIPIRTEXT[] { };
                WS_GAQ_sin_PI_DIRECT_MOAOP.MEWISCALEQUAN[] MEWISCALEQUANE = new WS_GAQ_sin_PI_DIRECT_MOAOP.MEWISCALEQUAN[] { };
                WS_GAQ_sin_PI_DIRECT_MOAOP.MEWISCALEVAL[] MEWISCALEVALE   = new WS_GAQ_sin_PI_DIRECT_MOAOP.MEWISCALEVAL[] { };
                WS_GAQ_sin_PI_DIRECT_MOAOP.MEWIEINE MEWIEINEE             = new WS_GAQ_sin_PI_DIRECT_MOAOP.MEWIEINE();

                var registrosSap = DevolverDatosSapRegistroSinPI(registrosInfo);
                string xml = "";
                foreach (var item in registrosSap)
                {
                    BAPIRETURNE = new WS_GAQ_sin_PI_DIRECT_MOAOP.BAPIRETURN[] { };
                    MEWIPIRTEXTE = new WS_GAQ_sin_PI_DIRECT_MOAOP.MEWIPIRTEXT[] { };
                    MEWISCALEQUANE = new WS_GAQ_sin_PI_DIRECT_MOAOP.MEWISCALEQUAN[] { };
                    MEWISCALEVALE = new WS_GAQ_sin_PI_DIRECT_MOAOP.MEWISCALEVAL[] { };
                    MEWIEINEE = new WS_GAQ_sin_PI_DIRECT_MOAOP.MEWIEINE();

                    WS_GAQ_sin_PI_DIRECT_MOAOP.MEWICONDITION[] CONDITIONE = item.CONDITION != null ? item.CONDITION.ToArray() : new WS_GAQ_sin_PI_DIRECT_MOAOP.MEWICONDITION[] { };
                    WS_GAQ_sin_PI_DIRECT_MOAOP.MEWIVALIDITY[] MEWIVALIDITYE = item.MEWIVALIDITY != null ? item.MEWIVALIDITY.ToArray() : new WS_GAQ_sin_PI_DIRECT_MOAOP.MEWIVALIDITY[] { };

                    var request = new Z_MMRFC_MANTENER_REGINFO()
                    {
                        I_EINA = item.MEWIEINA,
                        I_EINAX = item.MEWIEINAX,
                        I_EINE = item.MEWIEINE,
                        I_EINEX = item.EINEX,
                        TESTRUN = "",
                        CONDITION = CONDITIONE,
                        COND_VALIDITY = MEWIVALIDITYE,
                        COND_SCALE_QUAN = MEWISCALEQUANE,
                        COND_SCALE_VALUE = MEWISCALEVALE,
                        RETURN = BAPIRETURNE,
                        TXT_LINES = MEWIPIRTEXTE
                    };
                    Log.Info($"SAP sin PI Z_MMRFC_MANTENER_REGINFO request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MMRFC_MANTENER_REGINFO(request);
                    Log.Info($"SAP sin PI Z_MMRFC_MANTENER_REGINFO response");
                    Log.Info(response.ToXml());

                    var serxml = new System.Xml.Serialization.XmlSerializer(item.GetType());
                    var ms = new MemoryStream();
                    serxml.Serialize(ms, item);
                    var xmlReturn = new System.Xml.Serialization.XmlSerializer(response.RETURN.GetType());
                    xmlReturn.Serialize(ms, response.RETURN);
                    xml += Encoding.UTF8.GetString(ms.ToArray());
                }

                var respuesta = new CrearSolpConsumerMOAResponse();
                respuesta.Errores = new List<CrearSolpConsumerMOAError>();

                foreach (var errorSAP in BAPIRETURNE)
                {
                    var error = new CrearSolpConsumerMOAError
                    {
                        Codigo = errorSAP.CODE,
                        Mensaje = errorSAP.MESSAGE,
                        Tipo = errorSAP.TYPE
                    };

                    respuesta.Errores.Add(error);
                }

                Log.ComprasRegistroInfo(xml);
                return respuesta;

            }
            else
            {

                SI_MMRFC_MANTENER_REGINFOClient service;
                var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=SI_MMRFC_MANTENER_REGINFO&amp;interfaceNamespace=urn%3AOPERACIONES";
                service = new SI_MMRFC_MANTENER_REGINFOClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();

                AgregarRegistroInfoServiceWebMOA.BAPIRETURN[]    BAPIRETURNE    = new AgregarRegistroInfoServiceWebMOA.BAPIRETURN[] { };
                AgregarRegistroInfoServiceWebMOA.MEWIPIRTEXT[]   MEWIPIRTEXTE   = new AgregarRegistroInfoServiceWebMOA.MEWIPIRTEXT[] { };
                AgregarRegistroInfoServiceWebMOA.MEWISCALEQUAN[] MEWISCALEQUANE = new AgregarRegistroInfoServiceWebMOA.MEWISCALEQUAN[] { };
                AgregarRegistroInfoServiceWebMOA.MEWISCALEVAL[]  MEWISCALEVALE  = new AgregarRegistroInfoServiceWebMOA.MEWISCALEVAL[] { };
                AgregarRegistroInfoServiceWebMOA.MEWIEINE        MEWIEINEE      = new AgregarRegistroInfoServiceWebMOA.MEWIEINE();

                var registrosSap = DevolverDatosSapRegistro(registrosInfo);
                string xml = "";

                foreach (var item in registrosSap)
                {
                    BAPIRETURNE    = new AgregarRegistroInfoServiceWebMOA.BAPIRETURN[] { };
                    MEWIPIRTEXTE   = new AgregarRegistroInfoServiceWebMOA.MEWIPIRTEXT[] { };
                    MEWISCALEQUANE = new AgregarRegistroInfoServiceWebMOA.MEWISCALEQUAN[] { };
                    MEWISCALEVALE  = new AgregarRegistroInfoServiceWebMOA.MEWISCALEVAL[] { };
                    MEWIEINEE      = new AgregarRegistroInfoServiceWebMOA.MEWIEINE();

                    AgregarRegistroInfoServiceWebMOA.MEWICONDITION[] CONDITIONE = item.CONDITION != null ? item.CONDITION.ToArray() : new AgregarRegistroInfoServiceWebMOA.MEWICONDITION[] { };
                    AgregarRegistroInfoServiceWebMOA.MEWIVALIDITY[] MEWIVALIDITYE = item.MEWIVALIDITY != null ? item.MEWIVALIDITY.ToArray() : new AgregarRegistroInfoServiceWebMOA.MEWIVALIDITY[] { };

                    var result = service.SI_MMRFC_MANTENER_REGINFO(item.MEWIEINA, item.MEWIEINAX, item.MEWIEINE, item.EINEX, "", ref CONDITIONE,
                        ref MEWISCALEQUANE, ref MEWISCALEVALE, ref MEWIVALIDITYE, ref BAPIRETURNE, ref MEWIPIRTEXTE, out MEWIEINEE);

                    var serxml = new System.Xml.Serialization.XmlSerializer(item.GetType());
                    var ms = new MemoryStream();
                    serxml.Serialize(ms, item);
                    var xmlReturn = new System.Xml.Serialization.XmlSerializer(BAPIRETURNE.GetType());
                    xmlReturn.Serialize(ms, BAPIRETURNE);
                    xml += Encoding.UTF8.GetString(ms.ToArray());
                }

                var respuesta = new CrearSolpConsumerMOAResponse();
                respuesta.Errores = new List<CrearSolpConsumerMOAError>();

                foreach (var errorSAP in BAPIRETURNE)
                {
                    var error = new CrearSolpConsumerMOAError
                    {
                        Codigo = errorSAP.CODE,
                        Mensaje = errorSAP.MESSAGE,
                        Tipo = errorSAP.TYPE
                    };

                    respuesta.Errores.Add(error);
                }

                Log.ComprasRegistroInfo(xml);
                return respuesta;
            }
        }

        private List<RegistroInfoSAP> DevolverDatosSapRegistro(List<RegistroInfoDto> registros)
        {
            var hoy = DateTime.Now.Date;
            var registrosSap = new List<RegistroInfoSAP>();
            var unidades = registros.Select(x => x.Unidad).Distinct();
            var unidadesDeMedia = repositorio.Listar<UnidadMedidaSap, UnidadMedidaSapDto>(x => new UnidadMedidaSapDto
            { Comercial = x.Comercial, UM = x.UM }, x => unidades.Contains(x.Comercial));
            foreach (var registro in registros)
            {
                string unidadMedidaCodigo = unidadesDeMedia.First(x => x.Comercial == registro.Unidad).UM;
                var registroInfoSAP = new RegistroInfoSAP
                {
                    MEWIEINA = new AgregarRegistroInfoServiceWebMOA.MEWIEINA
                    {
                        MATERIAL = registro.MaterialCodigo,
                        VENDOR = registro.Cuit,
                        PO_UNIT = unidadMedidaCodigo
                    },
                    MEWIEINAX = new AgregarRegistroInfoServiceWebMOA.MEWIEINAX
                    {
                        MATERIAL = "X",
                        VENDOR = "X",
                        PO_UNIT = "X"
                    },
                    MEWIEINE = new AgregarRegistroInfoServiceWebMOA.MEWIEINE
                    {
                        PURCH_ORG = registro.OrganizacionDeCompra,
                        INFO_TYPE = "0",
                        PUR_GROUP = registro.GrupoDeCompras,
                        PLANT = registro.Centro,
                        CURRENCY = registro.Moneda,
                        MIN_PO_QTY = 0,
                        NRM_PO_QTY = 1,
                        PLND_DELRY = CalcularFecha(registro.FechaVigenciaFormateada, hoy), //es la fecha de vigencia
                        QUOTATION = "LICITACION",
                        QUOT_DATE = CalcularFechaString(registro.FechaVigenciaFormateada, hoy),//es la fecha de vigencia
                        NET_PRICE = registro.Precio,
                        EFF_PRICE = registro.Precio,
                        PRICE_UNIT = 1,
                        ORDERPR_UN = unidadMedidaCodigo,
                        PRICE_DATE = CalcularFechaString(registro.FechaVigenciaFormateada, hoy),//es la fecha de vigencia
                        PERIOD_IND_EXPIRATION_DATE = "D",
                        PRICE_UNITSpecified = true,
                        NRM_PO_QTYSpecified = true,
                        MIN_PO_QTYSpecified = true,
                        PLND_DELRYSpecified = true,
                        NET_PRICESpecified = true,
                        EFF_PRICESpecified = true
                    },
                    EINEX = new AgregarRegistroInfoServiceWebMOA.MEWIEINEX
                    {
                        PURCH_ORG = "X",
                        INFO_TYPE = "X",
                        PLANT = string.IsNullOrEmpty(registro.Centro) ? "" : "X",
                        PUR_GROUP = "X",
                        CURRENCY = "X",
                        MIN_PO_QTY = "X",
                        NRM_PO_QTY = "X",
                        PLND_DELRY = "X",
                        QUOTATION = "X",
                        QUOT_DATE = "X",
                        NET_PRICE = "X",
                        PRICE_UNIT = "X",
                        ORDERPR_UN = "X",
                        PRICE_DATE = "X",

                    },
                };

                if (registro.EsModificar)
                {
                    registroInfoSAP.CONDITION = new List<AgregarRegistroInfoServiceWebMOA.MEWICONDITION>()
                    {
                       new AgregarRegistroInfoServiceWebMOA.MEWICONDITION
                       {
                        SERIAL_ID = "1",
                        COND_COUNT = "01",
                        COND_TYPE = "ZP00",
                        COND_VALUE = registro.Precio,
                        CURRENCY = registro.Moneda,
                        NUMERATOR = 1,
                        DENOMINATOR = 1,
                        BASE_UOM = unidadMedidaCodigo,
                        LOWERLIMIT = 0,
                        UPPERLIMIT = 0,
                        DENOMINATORSpecified = true,
                        LOWERLIMITSpecified = true,
                        UPPERLIMITSpecified = true,
                        NUMERATORSpecified = true,
                        COND_VALUESpecified = true,
                        }
                    };

                    registroInfoSAP.MEWIVALIDITY = new List<AgregarRegistroInfoServiceWebMOA.MEWIVALIDITY>()
                    {
                        new AgregarRegistroInfoServiceWebMOA.MEWIVALIDITY
                        {
                        SERIAL_ID = "1",
                        PLANT = registro.Centro,
                        VALID_FROM = hoy.ToString("yyyy-MM-dd"),
                        VALID_TO = CalcularFechaString(registro.FechaVigenciaFormateada, hoy)
                        }
                    };
                }

                registrosSap.Add(registroInfoSAP);
            }

            return registrosSap;
        }
        private List<RegistroInfoSAPSinPI> DevolverDatosSapRegistroSinPI(List<RegistroInfoDto> registros)
        {
            var hoy = DateTime.Now.Date;
            var registrosSap = new List<RegistroInfoSAPSinPI>();
            var unidades = registros.Select(x => x.Unidad).Distinct();
            var unidadesDeMedia = repositorio.Listar<UnidadMedidaSap, UnidadMedidaSapDto>(x => new UnidadMedidaSapDto
            { Comercial = x.Comercial, UM = x.UM }, x => unidades.Contains(x.Comercial));
            foreach (var registro in registros)
            {
                string unidadMedidaCodigo = unidadesDeMedia.First(x => x.Comercial == registro.Unidad).UM;
                var registroInfoSAP = new RegistroInfoSAPSinPI
                {
                    MEWIEINA = new WS_GAQ_sin_PI_DIRECT_MOAOP.MEWIEINA
                    {
                        MATERIAL = registro.MaterialCodigo,
                        VENDOR = registro.Cuit,
                        PO_UNIT = unidadMedidaCodigo
                    },
                    MEWIEINAX = new WS_GAQ_sin_PI_DIRECT_MOAOP.MEWIEINAX
                    {
                        MATERIAL = "X",
                        VENDOR = "X",
                        PO_UNIT = "X"
                    },
                    MEWIEINE = new WS_GAQ_sin_PI_DIRECT_MOAOP.MEWIEINE
                    {
                        PURCH_ORG = registro.OrganizacionDeCompra,
                        INFO_TYPE = "0",
                        PUR_GROUP = registro.GrupoDeCompras,
                        PLANT = registro.Centro,
                        CURRENCY = registro.Moneda,
                        MIN_PO_QTY = 0,
                        NRM_PO_QTY = 1,
                        PLND_DELRY = CalcularFecha(registro.FechaVigenciaFormateada, hoy), //es la fecha de vigencia
                        QUOTATION = "LICITACION",
                        QUOT_DATE = CalcularFechaString(registro.FechaVigenciaFormateada, hoy),//es la fecha de vigencia
                        NET_PRICE = registro.Precio,
                        EFF_PRICE = registro.Precio,
                        PRICE_UNIT = 1,
                        ORDERPR_UN = unidadMedidaCodigo,
                        PRICE_DATE = CalcularFechaString(registro.FechaVigenciaFormateada, hoy),//es la fecha de vigencia
                        PERIOD_IND_EXPIRATION_DATE = "D",
                        //PRICE_UNITSpecified = true,
                        //NRM_PO_QTYSpecified = true,
                        //MIN_PO_QTYSpecified = true,
                        //PLND_DELRYSpecified = true,
                        //NET_PRICESpecified = true,
                        //EFF_PRICESpecified = true
                    },
                    EINEX = new WS_GAQ_sin_PI_DIRECT_MOAOP.MEWIEINEX
                    {
                        PURCH_ORG = "X",
                        INFO_TYPE = "X",
                        PLANT = string.IsNullOrEmpty(registro.Centro) ? "" : "X",
                        PUR_GROUP = "X",
                        CURRENCY = "X",
                        MIN_PO_QTY = "X",
                        NRM_PO_QTY = "X",
                        PLND_DELRY = "X",
                        QUOTATION = "X",
                        QUOT_DATE = "X",
                        NET_PRICE = "X",
                        PRICE_UNIT = "X",
                        ORDERPR_UN = "X",
                        PRICE_DATE = "X",

                    },
                };

                if (registro.EsModificar)
                {
                    registroInfoSAP.CONDITION = new List<WS_GAQ_sin_PI_DIRECT_MOAOP.MEWICONDITION>()
                    {
                       new WS_GAQ_sin_PI_DIRECT_MOAOP.MEWICONDITION
                       {
                        SERIAL_ID = "1",
                        COND_COUNT = "01",
                        COND_TYPE = "ZP00",
                        COND_VALUE = registro.Precio,
                        CURRENCY = registro.Moneda,
                        NUMERATOR = 1,
                        DENOMINATOR = 1,
                        BASE_UOM = unidadMedidaCodigo,
                        LOWERLIMIT = 0,
                        UPPERLIMIT = 0,
                        //DENOMINATORSpecified = true,
                        //LOWERLIMITSpecified = true,
                        //UPPERLIMITSpecified = true,
                        //NUMERATORSpecified = true,
                        //COND_VALUESpecified = true,
                        }
                    };

                    registroInfoSAP.MEWIVALIDITY = new List<WS_GAQ_sin_PI_DIRECT_MOAOP.MEWIVALIDITY>()
                    {
                        new WS_GAQ_sin_PI_DIRECT_MOAOP.MEWIVALIDITY
                        {
                        SERIAL_ID = "1",
                        PLANT = registro.Centro,
                        VALID_FROM = hoy.ToString("yyyy-MM-dd"),
                        VALID_TO = CalcularFechaString(registro.FechaVigenciaFormateada, hoy)
                        }
                    };
                }

                registrosSap.Add(registroInfoSAP);
            }

            return registrosSap;
        }



        private int CalcularFecha(DateTime fechaVigencia, DateTime hoy)
        {
            TimeSpan diferencia = fechaVigencia - hoy;

            if (diferencia.Days < 30)
            {
                return 30;
            }
            return diferencia.Days;
        }
        private string CalcularFechaString(DateTime fechaVigencia, DateTime hoy)
        {
            TimeSpan diferencia = fechaVigencia - hoy;

            if (diferencia.Days < 30)
            {
                return SAPFormatter.PrepararFecha(hoy.AddDays(30));
            }
            return SAPFormatter.PrepararFecha(fechaVigencia);
        }
    }


    public interface IAgregarRegistroInfoConsumerMOA
    {
        CrearSolpConsumerMOAResponse AgregarRegistroInfo(List<RegistroInfoDto> registrosInfo);

    }

    public class RegistroInfoSAP
    {
        public AgregarRegistroInfoServiceWebMOA.MEWIEINA MEWIEINA { get; set; }
        public AgregarRegistroInfoServiceWebMOA.MEWIEINAX MEWIEINAX { get; set; }
        public AgregarRegistroInfoServiceWebMOA.MEWIEINE MEWIEINE { get; set; }
        public AgregarRegistroInfoServiceWebMOA.MEWIEINEX EINEX { get; set; }

        public List<AgregarRegistroInfoServiceWebMOA.MEWIVALIDITY> MEWIVALIDITY { get; set; }
        public List<AgregarRegistroInfoServiceWebMOA.MEWICONDITION> CONDITION { get; set; }
    }

    public class RegistroInfoSAPSinPI
    {
        public WS_GAQ_sin_PI_DIRECT_MOAOP.MEWIEINA MEWIEINA { get; set; }
        public WS_GAQ_sin_PI_DIRECT_MOAOP.MEWIEINAX MEWIEINAX { get; set; }
        public WS_GAQ_sin_PI_DIRECT_MOAOP.MEWIEINE MEWIEINE { get; set; }
        public WS_GAQ_sin_PI_DIRECT_MOAOP.MEWIEINEX EINEX { get; set; }

        public List<WS_GAQ_sin_PI_DIRECT_MOAOP.MEWIVALIDITY> MEWIVALIDITY { get; set; }
        public List<WS_GAQ_sin_PI_DIRECT_MOAOP.MEWICONDITION> CONDITION { get; set; }
    }


}
