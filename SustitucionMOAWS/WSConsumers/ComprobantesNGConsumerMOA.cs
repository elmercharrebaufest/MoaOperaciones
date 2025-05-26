using SustitucionMOAFotmatter;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.Liquidacion.NoGranos;
using SustitucionMOAWS.ComprobantesNGWebServiceMOA;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.ScatoComandosWebService;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MOAOP;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SustitucionMOAWS.WSConsumers
{
    public class ComprobantesNGConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public object request(string proveedor, List<FechaWS> listaFechas)
        {
            try
            {

                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100[] fechas = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100[] { };

                    if (listaFechas.FirstOrDefault() != null)
                    {
                        fechas = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100[] {
                        new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100 {
                            FECHA_OP = SAPFormatter.PrepararFecha(listaFechas.FirstOrDefault().fechaInicio),
                            FECHA_OP_HASTA = SAPFormatter.PrepararFecha(listaFechas.FirstOrDefault().fechaFin)
                        }
                    };
                    }

                    WS_GAQ_sin_PI_DIRECT_MOAOP.BAPIRET2[] error = new WS_GAQ_sin_PI_DIRECT_MOAOP.BAPIRET2[] { };
                    string fechahasta = "";

                    if (listaFechas.FirstOrDefault() != null)
                    {
                        fechahasta = SAPFormatter.PrepararFecha(listaFechas.FirstOrDefault().fechaFin);
                    }

                    var request = new Z_MPRFC_MOAOP_COMPROB_NOGRANOS()
                    {
                        IM_FECHA = fechas,
                        IM_PROVEEDOR = proveedor,
                    };
                    Log.Info($"SAP sin PI Z_MPRFC_MOAOP_COMPROB_NOGRANOS request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MPRFC_MOAOP_COMPROB_NOGRANOS(request);
                    Log.Info($"SAP sin PI Z_MPRFC_MOAOP_COMPROB_NOGRANOS response");
                    Log.Info(response.ToXml());
                    return MapSinPI(response.EX_COMPRB,response.EX_RETURN);
                }
                else
                {
                    SI_MPMF_MOAOP_COMPROB_NOGRANOSClient service = new SI_MPMF_MOAOP_COMPROB_NOGRANOSClient();
                    ComprobantesNGWebServiceMOA.ZMPES4100[] fechas = new ComprobantesNGWebServiceMOA.ZMPES4100[] { };

                    if (listaFechas.FirstOrDefault() != null)
                    {
                        fechas = new ComprobantesNGWebServiceMOA.ZMPES4100[] {
                        new ComprobantesNGWebServiceMOA.ZMPES4100 {
                            FECHA_OP = SAPFormatter.PrepararFecha(listaFechas.FirstOrDefault().fechaInicio),
                            FECHA_OP_HASTA = SAPFormatter.PrepararFecha(listaFechas.FirstOrDefault().fechaFin)
                        }
                    };
                    }

                    ComprobantesNGWebServiceMOA.BAPIRET2[] error = new ComprobantesNGWebServiceMOA.BAPIRET2[] { };
                    string fechahasta = "";
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    if (listaFechas.FirstOrDefault() != null)
                    {
                        fechahasta = SAPFormatter.PrepararFecha(listaFechas.FirstOrDefault().fechaFin);
                    }
                    ComprobantesNGWebServiceMOA.ZMPES5940[] comprobantes = service.SI_MPMF_MOAOP_COMPROB_NOGRANOS(fechas, proveedor, out error);
                    return Map(comprobantes, error);
                }


            }
            catch (Exception e)
            {
                throw e;
            }

        }

        protected virtual object Map(ComprobantesNGWebServiceMOA.ZMPES5940[] comprobantes, ComprobantesNGWebServiceMOA.BAPIRET2[] error)
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
                    FechaComprobanteDate = SAPFormatter.GetDateTime(comprobante.BLDAT),
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
                result.MensajeModal = "Existen <strong>" + contador + " comprobante/s </strong> recibido/s en proceso de lectura de datos. El plazo estimado de procesamiento es de 72 hrs.";
            }


            result.comprobantes = result.comprobantes.Where(x => x.CodigoEstadoDocumento != EstadoComprobantesNG.ListoValidacion).ToList();
            return result;
        }
        protected virtual object MapSinPI(WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES5940[] comprobantes, WS_GAQ_sin_PI_DIRECT_MOAOP.BAPIRET2[] error)
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
                    FechaComprobanteDate = SAPFormatter.GetDateTime(comprobante.BLDAT),
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
                result.MensajeModal = "Existen <strong>" + contador + " comprobante/s </strong> recibido/s en proceso de lectura de datos. El plazo estimado de procesamiento es de 72 hrs.";
            }


            result.comprobantes = result.comprobantes.Where(x => x.CodigoEstadoDocumento != EstadoComprobantesNG.ListoValidacion).ToList();
            return result;
        }


    }

    public class ComprobantesExcelNGConsumerMOA : ComprobantesNGConsumerMOA
    {
        protected override object Map(ComprobantesNGWebServiceMOA.ZMPES5940[] comprobantes, ComprobantesNGWebServiceMOA.BAPIRET2[] error)
        {
            ComprobantesExcelNGWSMOAResponse result = new ComprobantesExcelNGWSMOAResponse();


            if (error != null && error.Length > 0)
            {
                result.error.codigo = error[0].MESSAGE;
                result.error.descripcion = error[0].MESSAGE;
                //result.error.tipo = error[0].TIPO;
            }

            foreach (ComprobantesNGWebServiceMOA.ZMPES5940 comprobante in comprobantes)
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

        protected override object MapSinPI(WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES5940[] comprobantes, WS_GAQ_sin_PI_DIRECT_MOAOP.BAPIRET2[] error)
        {
            ComprobantesExcelNGWSMOAResponse result = new ComprobantesExcelNGWSMOAResponse();


            if (error != null && error.Length > 0)
            {
                result.error.codigo = error[0].MESSAGE;
                result.error.descripcion = error[0].MESSAGE;
                //result.error.tipo = error[0].TIPO;
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES5940 comprobante in comprobantes)
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
