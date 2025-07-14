using SustitucionMOAFotmatter;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.ObtenerRegistroInfoConsumerMOA;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MEWI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace SustitucionMOAWS.WSConsumers
{
    public class ObtenerRegistroInfoConsumerMOA : IObtenerRegistroInfoConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserS4"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassS4"];
        private readonly IRepositorio repositorio;
        public ObtenerRegistroInfoConsumerMOA(IRepositorio repositorio)
        {

            this.repositorio = repositorio;
        }

        List<RegistroInfoDto> IObtenerRegistroInfoConsumerMOA.ObtenerRegistroInfoConsumer(string material, string centro, string organizacionDeCompras, string proveedor)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_BAPI_DIRECT_MEWIClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;
                    var request = new BAPI_INFORECORD_GETLIST()
                    {
                        DELETED_INFORECORDS = string.Empty,
                        GENERAL_DATA = "X",
                        INFO_TYPE = string.Empty,
                        MATERIAL = material,
                        MATERIAL_EVG = new WS_GAQ_sin_PI_DIRECT_MEWI.BAPIMGVMATNR(),
                        MATERIAL_LONG = string.Empty,
                        MAT_GRP = string.Empty,
                        PLANT = centro,
                        PURCHASINGINFOREC = string.Empty,
                        PURCHORG_DATA = "X",
                        PURCHORG_VEND = "X",
                        PUR_GROUP = string.Empty,
                        VENDOR = proveedor,
                        VEND_MAT = string.Empty,
                        VEND_MATG = string.Empty,
                        VEND_PART = string.Empty,
                        INFORECORD_GENERAL = new WS_GAQ_sin_PI_DIRECT_MEWI.BAPIEINA[] {},
                        INFORECORD_PURCHORG = new WS_GAQ_sin_PI_DIRECT_MEWI.BAPIEINE[] {},
                        INFORECORD_SEGMENT = new WS_GAQ_sin_PI_DIRECT_MEWI.BAPISEGM[] {},
                        PURCH_ORG = string.Empty,
                        RETURN = new WS_GAQ_sin_PI_DIRECT_MEWI.BAPIRETURN[] { },
                    };

                    Log.Info($"SAP sin PI BAPI_INFORECORD_GETLIST request");
                    Log.Info(request.ToXml());
                    var response = agent.BAPI_INFORECORD_GETLIST(request);
                    Log.Info($"SAP sin PI BAPI_INFORECORD_GETLIST response");
                    Log.Info(response.ToXml());

                    return MapSinPI(response, centro);
                }
                else
                {
                    BAPI_INFORECORD_GETLISTPortTypeClient service;
                    var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=BAPI_INFORECORD_GETLIST&amp;interfaceNamespace=urn%3Asap-com%3Adocument%3Asap%3Arfc%3Afunctions";
                    service = new BAPI_INFORECORD_GETLISTPortTypeClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();

                    SustitucionMOAWS.ObtenerRegistroInfoConsumerMOA.BAPIMGVMATNR bAPIMGVMATNR      = new SustitucionMOAWS.ObtenerRegistroInfoConsumerMOA.BAPIMGVMATNR();
                    SustitucionMOAWS.ObtenerRegistroInfoConsumerMOA.BAPIEINA[] INFORECORD_GENERAL  = new SustitucionMOAWS.ObtenerRegistroInfoConsumerMOA.BAPIEINA[] { };
                    SustitucionMOAWS.ObtenerRegistroInfoConsumerMOA.BAPISEGM[] bAPIEINEs           = new SustitucionMOAWS.ObtenerRegistroInfoConsumerMOA.BAPISEGM[] { };
                    SustitucionMOAWS.ObtenerRegistroInfoConsumerMOA.BAPIEINE[] INFORECORD_PURCHORG = new SustitucionMOAWS.ObtenerRegistroInfoConsumerMOA.BAPIEINE[] { };
                    SustitucionMOAWS.ObtenerRegistroInfoConsumerMOA.BAPIRETURN[] bAPIRETURNs       = new SustitucionMOAWS.ObtenerRegistroInfoConsumerMOA.BAPIRETURN[] { };
                    service.BAPI_INFORECORD_GETLIST("", "", "", material, bAPIMGVMATNR, "", "", centro, "", "", "",
                                                    ""/*organizacionDeCompras*/, "", proveedor, "", "", "", ref INFORECORD_GENERAL, ref INFORECORD_PURCHORG, ref bAPIEINEs, ref bAPIRETURNs);

                    return Map(INFORECORD_GENERAL, INFORECORD_PURCHORG, bAPIRETURNs, centro);
                }


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        protected List<RegistroInfoDto> MapSinPI(WS_GAQ_sin_PI_DIRECT_MEWI.BAPI_INFORECORD_GETLISTResponse response, string centro)
        {
            var registros = new List<RegistroInfoDto>();
            var hoy = DateTime.Now.Date;
            var unidadMedidaSap = repositorio.Listar<UnidadMedidaSap>();
            foreach (var info in response.INFORECORD_GENERAL)
            {
                foreach (var purch in response.INFORECORD_PURCHORG.Where(a => a.INFO_REC == info.INFO_REC && a.PLANT == centro))
                {
                    var codigoUnidad = unidadMedidaSap.Where(a => a.UM == info.PO_UNIT).Single().Comercial;

                    var registroInfo = new RegistroInfoDto
                    {
                        Cantidad = purch.NRM_PO_QTY,
                        Precio = purch.NET_PRICE,
                        Unidad = codigoUnidad,
                        Moneda = purch.CURRENCY,
                        Vendedor = info.VENDOR,
                        FechaVigencia = purch.PRICE_DATE,
                        FechaUltimaCompra = purch.LAST_PO,
                        Id = info.INFO_REC,
                        FechaFormateada = !string.IsNullOrEmpty(purch.PRICE_DATE) ? SAPFormatter.GetDateTime(purch.PRICE_DATE) : (DateTime?)null,
                        MaterialCodigo = info.MATERIAL,
                        NumeroOrdenDeCompra = purch.PO_NUMBER,
                        GrupoDeCompras = purch.PUR_GROUP
                    };

                    registros.Add(registroInfo);
                }
            }
            return registros;
        }

        protected List<RegistroInfoDto> Map(SustitucionMOAWS.ObtenerRegistroInfoConsumerMOA.BAPIEINA[] INFORECORD_GENERAL, SustitucionMOAWS.ObtenerRegistroInfoConsumerMOA.BAPIEINE[] INFORECORD_PURCHORG, SustitucionMOAWS.ObtenerRegistroInfoConsumerMOA.BAPIRETURN[] bAPIRETURNs, string centro)
        {
            var registros = new List<RegistroInfoDto>();
            var hoy = DateTime.Now.Date;
            var unidadMedidaSap = repositorio.Listar<UnidadMedidaSap>();
            foreach (var info in INFORECORD_GENERAL)
            {
                foreach (var purch in INFORECORD_PURCHORG.Where(a => a.INFO_REC == info.INFO_REC && a.PLANT == centro))
                {
                    var codigoUnidad = unidadMedidaSap.Where(a => a.UM == info.PO_UNIT).Single().Comercial;

                    var registroInfo = new RegistroInfoDto
                    {
                        Cantidad = purch.NRM_PO_QTY,
                        Precio = purch.NET_PRICE,
                        Unidad = codigoUnidad,
                        Moneda = purch.CURRENCY,
                        Vendedor = info.VENDOR,
                        FechaVigencia = purch.PRICE_DATE,
                        FechaUltimaCompra = purch.LAST_PO,
                        Id = info.INFO_REC,
                        FechaFormateada = !string.IsNullOrEmpty(purch.PRICE_DATE) ? SAPFormatter.GetDateTime(purch.PRICE_DATE) : (DateTime?)null,
                        MaterialCodigo = info.MATERIAL,
                        NumeroOrdenDeCompra = purch.PO_NUMBER,
                        GrupoDeCompras = purch.PUR_GROUP
                    };

                    registros.Add(registroInfo);
                }
            }
            return registros;
        }
    }
}