using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models.WSMapMOA.CartaPorte.Detalle;
using SustitucionMOAWS.CartaPorteDetalleWebServiceMOA;
using SustitucionMOAWS.CredentialService;

namespace SustitucionMOAWS.WSConsumers
{
    public class CartaPorteDetalleConsumerMOA
    {
        SI_MPMF_MOAOP_DETALLE_CCPPClient service = new SI_MPMF_MOAOP_DETALLE_CCPPClient();

        public object request(string proveedor, string cartaPorte)
        {
            try
            {
                ZMPES4300[] aplicaciones = new ZMPES4300[] { };
                ZMPES4310[] calidades = new ZMPES4310[] { };
                ZMPES6190[] entregasDescargas = new ZMPES6190[] { };
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                ZMPES4910 error = service.SI_MPMF_MOAOP_DETALLE_CCPP(cartaPorte, proveedor, ref aplicaciones, ref calidades, ref entregasDescargas);
                return map(error, aplicaciones, calidades, entregasDescargas, cartaPorte);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        protected virtual object map(ZMPES4910 error, ZMPES4300[] aplicaciones, ZMPES4310[] calidades, ZMPES6190[] entregasDescargas, string cartaPorte)
        {
            CartaPorteDetalleWSMOAResponse result = new CartaPorteDetalleWSMOAResponse();

            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }

            result.ccpp = cartaPorte;
            result.aplicacionesTotalAplicados = 0;
            result.calidadTotalAplicados = 0;
            result.calidadTotalNetos = 0;

            foreach (ZMPES4300 aplicacion in aplicaciones)
            {
                result.aplicaciones.Add(new AplicacionView()
                {
                    fecha = SAPFormatter.FormatearFecha(aplicacion.FECHA_APLIC),
                    contrato = aplicacion.CONTRATO,
                    kgAplicadosString = SAPFormatter.FormatearCantidad(aplicacion.KG_APLICADOS, aplicacion.UNIME)
                });

                result.aplicacionesTotalAplicados += aplicacion.KG_APLICADOS;
                result.aplicacionesTotalAplicadosUnidad = aplicacion.UNIME;
            }

            result.aplicacionesTotalAplicadosString = SAPFormatter.FormatearCantidad(result.aplicacionesTotalAplicados, result.aplicacionesTotalAplicadosUnidad);

            foreach (ZMPES4310 calidad in calidades)
            {
                result.datosCalidad.Add(new CalidadView()
                {
                    caracteristica = calidad.CARACT,
                    certificado = calidad.CERTIFICADO,
                    certificadoReconsideracion = calidad.CERTIFICADO_REC,
                    kgAplicadosString = SAPFormatter.FormatearCantidad(calidad.KG_APLIC, "KG"),
                    kgDescuentoString = SAPFormatter.FormatearCantidad(calidad.KG_DESC, "KG"),
                    kgNetosString = SAPFormatter.FormatearCantidad(calidad.KG_NETOS, "KG"),
                    kgAplicados = calidad.KG_APLIC,
                    kgDescuento = calidad.KG_DESC,
                    kgNetos = calidad.KG_NETOS,
                    porcentajeDescuento = calidad.PORC_DESC,
                    resultadoCalado = calidad.RESULTADO_CAL,
                    resultadoCamara = calidad.CARACT.ToUpper().Contains("HUMEDAD") ? calidad.RESULTADO_CAL : calidad.RESULTADO_CAM,
                    resultadoReconsideracion = calidad.RESULTADO_REC
                });

                result.camaraAPresent = calidad.CAMARA_A_PRESENT;
                result.calidadTotalAplicados += calidad.KG_APLIC;
                result.calidadTotalNetos += calidad.KG_NETOS + calidad.KG_DESC;
                result.calidadTotalNetosDescontados += calidad.KG_NETOS;
                result.calidadTotalAplicadosUnidad = "KG";
                result.calidadTotalNetosUnidad = "KG";
            }

            result.calidadTotalAplicadosString = SAPFormatter.FormatearCantidad(result.calidadTotalAplicados, "KG");
            result.calidadTotalNetosString = SAPFormatter.FormatearCantidad(Math.Round(result.calidadTotalNetos), "KG");
            result.calidadTotalNetosDescontadosString = SAPFormatter.FormatearCantidad(result.calidadTotalNetosDescontados, "KG");

            foreach (ZMPES6190 entregaDescarga in entregasDescargas)
            {
                result.entregasDescargas.Add(new EntregaDescargaView()
                {
                    acoplado = entregaDescarga.ACOPLADO,
                    centro = entregaDescarga.CENTRO,
                    descargaCentro = entregaDescarga.DESC_CENTRO,
                    descripcionProducto = entregaDescarga.DESC_PRODUCTO,
                    descripcionVendedor = entregaDescarga.DESC_VENDEDOR,
                    fecha = SAPFormatter.FormatearFecha(entregaDescarga.FECHA),
                    netoDescontado = entregaDescarga.NETO_DESCONTADO,
                    netoDescontadoString = SAPFormatter.FormatearCantidad(entregaDescarga.NETO_DESCONTADO, "KG"),
                    patente = entregaDescarga.PATENTE,
                    procedencia = entregaDescarga.PROCEDENCIA,
                    producto = entregaDescarga.PRODUCTO,
                    tipoVehiculo = SAPFormatter.FormatearTipoVehiculo(entregaDescarga.TIP_VEHI),
                    totalAplicados = entregaDescarga.TOTAL_APLICADOS,
                    totalAplicadosString = SAPFormatter.FormatearCantidad(entregaDescarga.TOTAL_APLICADOS, "KG"),

                    neto = entregaDescarga.NETO,
                    netoString = SAPFormatter.FormatearCantidad(entregaDescarga.NETO, "KG"),
                    vendedor = entregaDescarga.VENDEDOR,
                    cg = entregaDescarga.CG
                });

                result.NetoDescontadoTotal += entregaDescarga.NETO_DESCONTADO;
            }

            result.aplicacionesTotalExcedentes = result.aplicacionesTotalAplicados - result.NetoDescontadoTotal;
            result.aplicacionesTotalExcedentesUnidad = "KG";
            result.aplicacionesTotalExcedentesString = SAPFormatter.FormatearCantidad(result.aplicacionesTotalExcedentes, "KG");

            return result;
        }
    }

    public class CartaPorteDetalleExcelConsumerMOA : CartaPorteDetalleConsumerMOA
    {
        protected override object map(ZMPES4910 error, ZMPES4300[] aplicaciones, ZMPES4310[] calidades, ZMPES6190[] entregasDescargas, string cartaPorte)
        {
            CartaPorteDetalleExcelWSMOAResponse result = new CartaPorteDetalleExcelWSMOAResponse();

            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }

            result.ccpp = cartaPorte;
            result.aplicacionesTotalAplicados = 0;
            result.calidadTotalAplicados = 0;
            result.calidadTotalNetos = 0;

            foreach (ZMPES4300 aplicacion in aplicaciones)
            {
                result.aplicaciones.Add(new Aplicacion()
                {
                    fecha = SAPFormatter.FormatearFecha(aplicacion.FECHA_APLIC),
                    contrato = aplicacion.CONTRATO,
                    unidadKgAplicados = aplicacion.UNIME,
                    kgAplicados = aplicacion.KG_APLICADOS
                });

                result.aplicacionesTotalAplicados += aplicacion.KG_APLICADOS;
                result.aplicacionesTotalAplicadosUnidad = aplicacion.UNIME;
            }

            result.aplicacionesTotalAplicadosString = SAPFormatter.FormatearCantidad(result.aplicacionesTotalAplicados, result.aplicacionesTotalAplicadosUnidad);

            foreach (ZMPES4310 calidad in calidades)
            {
                result.datosCalidad.Add(new Calidad()
                {
                    caracteristica = calidad.CARACT,
                    certificado = calidad.CERTIFICADO,
                    certificadoReconsideracion = calidad.CERTIFICADO_REC,
                    unidadAplicados = "KG",
                    kgAplicados = calidad.KG_APLIC,
                    unidadDescuento = "KG",
                    kgDescuento = calidad.KG_DESC,
                    unidadNetos = "KG",
                    kgNetos = calidad.KG_NETOS,
                    porcentajeDescuento = calidad.PORC_DESC,
                    resultadoCalado = calidad.RESULTADO_CAL,
                    resultadoCamara = calidad.CARACT.ToUpper().Contains("HUMEDAD") ? calidad.RESULTADO_CAL : calidad.RESULTADO_CAM,
                    resultadoReconsideracion = calidad.RESULTADO_REC,
                });
                result.camaraAPresent = calidad.CAMARA_A_PRESENT;
                result.calidadTotalAplicados += calidad.KG_APLIC;
                result.calidadTotalNetos += calidad.KG_NETOS + calidad.KG_DESC;
                result.calidadTotalNetosDescontados += calidad.KG_NETOS;
                result.calidadTotalAplicadosUnidad = "KG";
                result.calidadTotalNetosUnidad = "KG";
            }

            result.calidadTotalAplicadosString = SAPFormatter.FormatearCantidad(result.calidadTotalAplicados, "KG");
            result.calidadTotalNetosString = SAPFormatter.FormatearCantidad(result.calidadTotalNetos, "KG");

            foreach (ZMPES6190 entregaDescarga in entregasDescargas)
            {
                result.entregasDescargas.Add(new EntregaDescarga()
                {
                    acoplado = entregaDescarga.ACOPLADO,
                    centro = entregaDescarga.CENTRO,
                    descargaCentro = entregaDescarga.DESC_CENTRO,
                    descripcionProducto = entregaDescarga.DESC_PRODUCTO,
                    descripcionVendedor = entregaDescarga.DESC_VENDEDOR,
                    fecha = SAPFormatter.FormatearFecha(entregaDescarga.FECHA),
                    unidadNetoDescontado = "KG",
                    netoDescontado = entregaDescarga.NETO_DESCONTADO,
                    patente = entregaDescarga.PATENTE,
                    procedencia = entregaDescarga.PROCEDENCIA,
                    producto = entregaDescarga.PRODUCTO,
                    tipoVehiculo = SAPFormatter.FormatearTipoVehiculo(entregaDescarga.TIP_VEHI),
                    unidadTotalAplicados = "KG",
                    totalAplicados = entregaDescarga.TOTAL_APLICADOS,
                    vendedor = entregaDescarga.VENDEDOR
                });

                result.NetoDescontadoTotal += entregaDescarga.NETO_DESCONTADO;
            }

            result.aplicacionesTotalExcedentes = result.aplicacionesTotalAplicados - result.NetoDescontadoTotal;
            result.aplicacionesTotalExcedentesUnidad = "KG";
            result.aplicacionesTotalExcedentesString = SAPFormatter.FormatearCantidad(result.aplicacionesTotalExcedentes, "KG");

            return result;
        }
    }

    public class CartaPorteDetallePDFConsumerMOA : CartaPorteDetalleConsumerMOA
    {
        protected override object map(ZMPES4910 error, ZMPES4300[] aplicaciones, ZMPES4310[] calidades, ZMPES6190[] entregasDescargas, string cartaPorte)
        {
            CartaPorteDetallePDFWSMOAResponse result = new CartaPorteDetallePDFWSMOAResponse();

            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }

            foreach (ZMPES4310 calidad in calidades)
            {
                result.datosCalidad.Add(new CalidadPDF()
                {
                    caracteristica = calidad.CARACT,
                    kgAplicadosString = SAPFormatter.FormatearCantidad(calidad.KG_APLIC, "KG"),
                    kgDescuentoString = SAPFormatter.FormatearCantidad(calidad.KG_DESC, "KG"),
                    kgNetosString = SAPFormatter.FormatearCantidad(calidad.KG_NETOS, "KG"),
                    porcentajeDescuentoString = SAPFormatter.FormatearCantidad(calidad.PORC_DESC, "%"),
                    resultadoCaladoString = SAPFormatter.FormatearCantidad(calidad.RESULTADO_CAL, "%"),
                    resultadoCamaraString = SAPFormatter.FormatearCantidad(calidad.RESULTADO_CAM, "%"),
                });
            }

            return result;
        }
    }
}
