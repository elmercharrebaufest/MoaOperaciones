using SustitucionMOAFotmatter;
using SustitucionMOAModel.Dto;
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

        public ObtenerRegistroInfoConsumerMOA()
        {
            service = new BAPI_INFORECORD_GETLISTPortTypeClient();
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
        }

        List<RegistroInfoDto> IObtenerRegistroInfoConsumerMOA.ObtenerRegistroInfoConsumer(string material, string centro, string grupoDeCompras)
        {
            try
            {
                BAPIMGVMATNR bAPIMGVMATNR = new BAPIMGVMATNR();
                BAPIEINA[] INFORECORD_GENERAL = new BAPIEINA[] { };
                BAPISEGM[] bAPIEINEs = new BAPISEGM[] { };
                BAPIEINE[] INFORECORD_PURCHORG = new BAPIEINE[] { };
                BAPIRETURN[] bAPIRETURNs = new BAPIRETURN[] { };

                service.BAPI_INFORECORD_GETLIST("", "", "", material, bAPIMGVMATNR, "", "", centro, "", "", "",
                                                ""/*grupoDeCompras*/, "", "", "", "", "", ref INFORECORD_GENERAL, ref INFORECORD_PURCHORG, ref bAPIEINEs, ref bAPIRETURNs);

                return Map(INFORECORD_GENERAL, INFORECORD_PURCHORG, bAPIRETURNs);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        protected List<RegistroInfoDto> Map(BAPIEINA[] INFORECORD_GENERAL, BAPIEINE[] INFORECORD_PURCHORG, BAPIRETURN[] bAPIRETURNs)
        {
            var registros = new List<RegistroInfoDto>();
            var hoy = DateTime.Now.Date;
            foreach (var info in INFORECORD_GENERAL)
            {
                foreach (var purch in INFORECORD_PURCHORG.Where(a => a.INFO_REC == info.INFO_REC))
                {
                    var registroInfo = new RegistroInfoDto
                    {
                        Cantidad = purch.NRM_PO_QTY,
                        Precio = purch.EFF_PRICE,
                        Unidad = info.PO_UNIT,
                        Moneda = purch.CURRENCY,
                        Vendedor = info.VENDOR,
                        FechaVigencia = purch.PRICE_DATE,
                        FechaUltimaCompra = purch.LAST_PO,
                        Id = info.INFO_REC,
                        FechaFormateada = !string.IsNullOrEmpty(purch.PRICE_DATE) ? SAPFormatter.GetDateTime(purch.PRICE_DATE) : (DateTime?)null,
                        MaterialCodigo = info.MATERIAL,
                        NumeroOrdenDeCompra = purch.PO_NUMBER
                    };

                    registros.Add(registroInfo);
                }
            }

            return registros;
        }

        public static string PrepararFecha(DateTime fecha)
        {
            return fecha.ToString("yyyy-MM-dd");
        }
    }
}