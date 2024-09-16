using SustitucionMOAFotmatter;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ObtenerRegistroInfoConsumerMOA;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAWS.WSConsumers
{
    public class ObtenerRegistroInfoConsumerMOA : IObtenerRegistroInfoConsumerMOA
    {
        BAPI_INFORECORD_GETLISTPortTypeClient service;

        private readonly IRepositorio repositorio;
        public ObtenerRegistroInfoConsumerMOA(IRepositorio repositorio)
        {
            var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=BAPI_INFORECORD_GETLIST&amp;interfaceNamespace=urn%3Asap-com%3Adocument%3Asap%3Arfc%3Afunctions";
            service = new BAPI_INFORECORD_GETLISTPortTypeClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
            this.repositorio = repositorio;
        }

        List<RegistroInfoDto> IObtenerRegistroInfoConsumerMOA.ObtenerRegistroInfoConsumer(string material, string centro, string organizacionDeCompras, string proveedor)
        {
            try
            {
                BAPIMGVMATNR bAPIMGVMATNR = new BAPIMGVMATNR();
                BAPIEINA[] INFORECORD_GENERAL = new BAPIEINA[] { };
                BAPISEGM[] bAPIEINEs = new BAPISEGM[] { };
                BAPIEINE[] INFORECORD_PURCHORG = new BAPIEINE[] { };
                BAPIRETURN[] bAPIRETURNs = new BAPIRETURN[] { };
                service.BAPI_INFORECORD_GETLIST("", "", "", material, bAPIMGVMATNR, "", "", centro, "", "", "",
                                                ""/*organizacionDeCompras*/, "", proveedor, "", "", "", ref INFORECORD_GENERAL, ref INFORECORD_PURCHORG, ref bAPIEINEs, ref bAPIRETURNs);

                return Map(INFORECORD_GENERAL, INFORECORD_PURCHORG, bAPIRETURNs, centro);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        protected List<RegistroInfoDto> Map(BAPIEINA[] INFORECORD_GENERAL, BAPIEINE[] INFORECORD_PURCHORG, BAPIRETURN[] bAPIRETURNs, string centro)
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