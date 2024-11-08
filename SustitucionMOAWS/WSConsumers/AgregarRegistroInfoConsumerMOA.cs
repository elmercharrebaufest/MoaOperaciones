using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using Newtonsoft.Json;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAWS.AgregarRegistroInfoServiceWebMOA;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;

namespace SustitucionMOAWS.WSConsumers
{
    public class AgregarRegistroInfoConsumerMOA : IAgregarRegistroInfoConsumerMOA
    {
        private readonly IRepositorio repositorio;
        private readonly SI_MMRFC_MANTENER_REGINFOClient service;

        public AgregarRegistroInfoConsumerMOA(IRepositorio repositorio)
        {
            var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=SI_MMRFC_MANTENER_REGINFO&amp;interfaceNamespace=urn%3AOPERACIONES";

            service = new SI_MMRFC_MANTENER_REGINFOClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));

            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
            this.repositorio = repositorio;
        }

        public CrearSolpConsumerMOAResponse AgregarRegistroInfo(List<RegistroInfoDto> registrosInfo)
        {
            BAPIRETURN[] BAPIRETURNE = new BAPIRETURN[] { };
            MEWIPIRTEXT[] MEWIPIRTEXTE = new MEWIPIRTEXT[] { };
            MEWISCALEQUAN[] MEWISCALEQUANE = new MEWISCALEQUAN[] { };
            MEWISCALEVAL[] MEWISCALEVALE = new MEWISCALEVAL[] { };
            MEWIEINE MEWIEINEE = new MEWIEINE();

            var registrosSap = DevolverDatosSapRegistro(registrosInfo);
            string xml = "";

            foreach (var item in registrosSap)
            {
                BAPIRETURNE = new BAPIRETURN[] { };
                MEWIPIRTEXTE = new MEWIPIRTEXT[] { };
                MEWISCALEQUANE = new MEWISCALEQUAN[] { };
                MEWISCALEVALE = new MEWISCALEVAL[] { };
                MEWIEINEE = new MEWIEINE();

                MEWICONDITION[] CONDITIONE = item.CONDITION != null ? item.CONDITION.ToArray() : new MEWICONDITION[] { };
                MEWIVALIDITY[] MEWIVALIDITYE = item.MEWIVALIDITY != null ? item.MEWIVALIDITY.ToArray() : new MEWIVALIDITY[] { };

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

        private List<RegistroInfoSAP> DevolverDatosSapRegistro(List<RegistroInfoDto> registros)
        {
            var hoy = DateTime.Now.Date;
            var registrosSap = new List<RegistroInfoSAP>();
            var unidades = registros.Select(x => x.Unidad).Distinct();
            var unidadesDeMedia = repositorio.Listar<UnidadMedidaSap, UnidadMedidaSapDto>(x => new UnidadMedidaSapDto
            { Comercial = x.Comercial, UM = x.UM }, x => unidades.Contains(x.Comercial));
            foreach (var registro in registros)
            {
                string unidadMedidaCodigo = unidadesDeMedia.First(x => x.Comercial == registro.Unidad).Comercial;
                var registroInfoSAP = new RegistroInfoSAP
                {
                    MEWIEINA = new MEWIEINA
                    {
                        MATERIAL = registro.MaterialCodigo,
                        VENDOR = registro.Cuit,
                        PO_UNIT = unidadMedidaCodigo
                    },
                    MEWIEINAX = new MEWIEINAX
                    {
                        MATERIAL = "X",
                        VENDOR = "X",
                        PO_UNIT = "X"
                    },
                    MEWIEINE = new MEWIEINE
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
                    EINEX = new MEWIEINEX
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
                    registroInfoSAP.CONDITION = new List<MEWICONDITION>()
                    {
                       new MEWICONDITION
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

                    registroInfoSAP.MEWIVALIDITY = new List<MEWIVALIDITY>()
                    {
                        new MEWIVALIDITY
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
        public MEWIEINA MEWIEINA { get; set; }
        public MEWIEINAX MEWIEINAX { get; set; }
        public MEWIEINE MEWIEINE { get; set; }
        public MEWIEINEX EINEX { get; set; }

        public List<MEWIVALIDITY> MEWIVALIDITY { get; set; }
        public List<MEWICONDITION> CONDITION { get; set; }
    }
}
