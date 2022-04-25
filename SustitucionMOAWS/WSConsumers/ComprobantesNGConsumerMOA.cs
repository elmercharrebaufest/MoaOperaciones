using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.ComprobantesNGWebServiceMOA;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Models.WSMapMOA.Liquidacion.NoGranos;
using SustitucionMOAModel.Enums;

namespace SustitucionMOAWS.WSConsumers
{
    public class ComprobantesNGConsumerMOA
    {
        SI_MPMF_MOAOP_COMPROB_NOGRANOSClient service = new SI_MPMF_MOAOP_COMPROB_NOGRANOSClient();

        public object request(string proveedor, List<FechaWS> listaFechas)
        {
            try
            {
                ZMPES4100[] fechas = new ZMPES4100[] { };

                if (listaFechas.FirstOrDefault() != null)
                {
                    fechas = new ZMPES4100[] {
                        new ZMPES4100 {
                            FECHA_OP = SAPFormatter.PrepararFecha(listaFechas.FirstOrDefault().fechaInicio),
                            FECHA_OP_HASTA = SAPFormatter.PrepararFecha(listaFechas.FirstOrDefault().fechaFin)
                        }
                    };
                }

                BAPIRET2[] error = new BAPIRET2[] { };
                string fechahasta = "";
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                if (listaFechas.FirstOrDefault() != null)
                {
                    fechahasta = SAPFormatter.PrepararFecha(listaFechas.FirstOrDefault().fechaFin);
                }
                ZMPES5940[] comprobantes = service.SI_MPMF_MOAOP_COMPROB_NOGRANOS(fechas, proveedor, out error);
                return map(comprobantes, error);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        protected virtual object map(ZMPES5940[] comprobantes, BAPIRET2[] error)
        {
            ComprobantesNGWSMOAResponse result = new ComprobantesNGWSMOAResponse();

            if (error != null && error.Length > 0)
            {
                result.error.codigo = error[0].MESSAGE;
                result.error.descripcion = error[0].MESSAGE;
                //result.error.tipo = error[0].TIPO;
            }

            comprobantes = comprobantes.Where(x => (x.DELREASON == "01" && x.STATUS == "10") || x.STATUS != "10").ToArray();

            foreach (var comprobante in comprobantes)
            {
                ComprobanteView comprobanteView = new ComprobanteView()
                {
                    Sociedad = comprobante.BUKRS, //BUKRS: corresponde a la sociedad MOA que no se utilizará para la web
                    CodigoProveedorSAP = comprobante.LIFNR, //LIFNR: corresponde al código de proveedor en SAP
                    RazonSocialProveedorSAP = comprobante.VEND_NAME, //VEND_NAME: corresponde a la razón social del proveedor en SAP
                    FechaDocumento = SAPFormatter.FormatearFecha(comprobante.BLDAT), //BLDAT: corresponde a la fecha de documento del documento // Fecha comprobante
                    TipoDocumento = comprobante.BLART, //BLART: corresponde al tipo de documento
                    DescripcionTipoDocumento = comprobante.LTEXT,  //LTEXT: corresponde a la descripción del tipo de documento
                    NumeroLegalDocumento = comprobante.XBLNR, //XBLNR: corresponde al número legal del documento
                    CodigoEstadoDocumento = (SustitucionMOAModel.Enums.EstadoComprobantesNG)int.Parse(comprobante.STATUS), //STATUS: corresponde al código de estado del documento
                    CodigoEstadoDocumentoDescripcion = EstadoComprobantesNGExtensions.ToFriendlyString((EstadoComprobantesNG)int.Parse(comprobante.STATUS)),
                    CodigoRolDocumento = comprobante.CURR_ROLE, //CURR_ROLE: corresponde al código del rol que tiene asignado este documento
                    CodigoMotivoRechazo = comprobante.DELREASON, //DELREASON: corresponde al código del motivo de rechazo
                    OrdenDeCompra = comprobante.EBELN, //EBELN: corresponde a la orden de compra
                    ImporteMercaderiaDocumento = comprobante.NET_AMOUNT, //NET_AMOUNT: corresponde al importe de la mercadería del documento
                    ImporteImpuestosDocumento = comprobante.VAT_AMOUNT, //VAT_AMOUNT: corresponde al importe de los impuestos del documento
                    TotalDocumento = comprobante.GROSS_AMOUNT, //GROSS_AMOUNT: corresponde al total del documento
                    MonedaDocumento = comprobante.WAERS, //WAERS: corresponde a la moneda del documento
                    TotalMasMoneda = SAPFormatter.FormatearMonto(comprobante.GROSS_AMOUNT, comprobante.WAERS),
                    ColorEstado = EstadoComprobantesNGExtensions.ObtenerColorEstado((EstadoComprobantesNG)int.Parse(comprobante.STATUS))
                };

                result.comprobantes.Add(comprobanteView);
            }

            if (result.comprobantes.Any(x => x.CodigoEstadoDocumento == EstadoComprobantesNG.ListoValidacion))
            {
                result.TieneModal = true;
                var contador = result.comprobantes.Count(x => x.CodigoEstadoDocumento == EstadoComprobantesNG.ListoValidacion);
                result.MensajeModal = "Tiene <strong>" + contador + " comprobantes</strong> pendientes de procesar. El plazo estimado es de 48hs.";

            }


            result.comprobantes = result.comprobantes.Where(x => x.CodigoEstadoDocumento != EstadoComprobantesNG.ListoValidacion).ToList();
            return result;
        }
    }

    public class ComprobantesExcelNGConsumerMOA : ComprobantesNGConsumerMOA
    {

        protected override object map(ZMPES5940[] comprobantes, BAPIRET2[] error)
        {
            ComprobantesExcelNGWSMOAResponse result = new ComprobantesExcelNGWSMOAResponse();


            if (error != null && error.Length > 0)
            {
                result.error.codigo = error[0].MESSAGE;
                result.error.descripcion = error[0].MESSAGE;
                //result.error.tipo = error[0].TIPO;
            }

            foreach (ZMPES5940 comprobante in comprobantes)
            {
                ComprobanteNGLista comprobanteView = new ComprobanteNGLista()
                {
                    //{ "Fecha de comprobante", "Tipo", "Comprobante", "Total", "Orden de Compra", "Estado" }

                    FechaDocumento = SAPFormatter.FormatearFecha(comprobante.BLDAT), //BLDAT: corresponde a la fecha de documento del documento // Fecha comprobante
                    DescripcionTipoDocumento = comprobante.LTEXT, //BLART: corresponde al tipo de documento
                    NumeroLegalDocumento = comprobante.XBLNR, //XBLNR: corresponde al número legal del documento
                    TotalMasMoneda = SAPFormatter.FormatearMonto(comprobante.GROSS_AMOUNT, comprobante.WAERS),
                    OrdenDeCompra = comprobante.EBELN, //EBELN: corresponde a la orden de compra
                    CodigoEstadoDocumentoDescripcion = EstadoComprobantesNGExtensions.ToFriendlyString((EstadoComprobantesNG)int.Parse(comprobante.STATUS)),

                };

                result.comprobantes.Add(comprobanteView);
            }
            return result;
        }
    }
}
