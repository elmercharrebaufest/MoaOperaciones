using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models.WSMapMOA.CartaPorte.Detalle;
using SustitucionMOAWS.CartaPorteDetalleWebServiceMOA;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MOAOP;
using System;
using System.Configuration;
namespace SustitucionMOAWS.WSConsumers
{
    public class CartaPorteDetalleConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserS4"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassS4"];


        public object request(string proveedor, string cartaPorte)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4300[] aplicaciones = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4300[] { };
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4310[] calidades = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4310[] { };
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6190[] entregasDescargas = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6190[] { };

                    var request = new Z_MPMF_MOAOP_DETALLE_CCPP()
                    {
                        NUMCARPOR = cartaPorte,
                        PE_PROVEEDOR = proveedor,
                        T_APLICACIONES = aplicaciones,
                        T_CALIDAD = calidades,
                        T_ZMPTE1000 = entregasDescargas,
                    };
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_DETALLE_CCPP request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MPMF_MOAOP_DETALLE_CCPP(request);
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_DETALLE_CCPP response");
                    Log.Info(response.ToXml());
                    return MapSinPI(response.MENSAJE_ERROR, response.T_APLICACIONES, response.T_CALIDAD, response.T_ZMPTE1000, cartaPorte);
                }
                else
                {
                    SI_MPMF_MOAOP_DETALLE_CCPPClient service = new SI_MPMF_MOAOP_DETALLE_CCPPClient();
                    CartaPorteDetalleWebServiceMOA.ZMPES4300[] aplicaciones = new CartaPorteDetalleWebServiceMOA.ZMPES4300[] { };
                    CartaPorteDetalleWebServiceMOA.ZMPES4310[] calidades = new CartaPorteDetalleWebServiceMOA.ZMPES4310[] { };
                    CartaPorteDetalleWebServiceMOA.ZMPES6190[] entregasDescargas = new CartaPorteDetalleWebServiceMOA.ZMPES6190[] { };
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    CartaPorteDetalleWebServiceMOA.ZMPES4910 error = service.SI_MPMF_MOAOP_DETALLE_CCPP(cartaPorte, proveedor, ref aplicaciones, ref calidades, ref entregasDescargas);
                    return Map(error, aplicaciones, calidades, entregasDescargas, cartaPorte);
                }


            }
            catch (Exception e)
            {
                throw e;
            }

        }

        protected virtual object Map(CartaPorteDetalleWebServiceMOA.ZMPES4910 error, CartaPorteDetalleWebServiceMOA.ZMPES4300[] aplicaciones, CartaPorteDetalleWebServiceMOA.ZMPES4310[] calidades, CartaPorteDetalleWebServiceMOA.ZMPES6190[] entregasDescargas, string cartaPorte)
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

            foreach (CartaPorteDetalleWebServiceMOA.ZMPES4300 aplicacion in aplicaciones)
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

            foreach (CartaPorteDetalleWebServiceMOA.ZMPES4310 calidad in calidades)
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

            foreach (CartaPorteDetalleWebServiceMOA.ZMPES6190 entregaDescarga in entregasDescargas)
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
        protected virtual object MapSinPI(WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4910 error, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4300[] aplicaciones, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4310[] calidades, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6190[] entregasDescargas, string cartaPorte)
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

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4300 aplicacion in aplicaciones)
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

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4310 calidad in calidades)
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

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6190 entregaDescarga in entregasDescargas)
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
        protected override object Map(CartaPorteDetalleWebServiceMOA.ZMPES4910 error, CartaPorteDetalleWebServiceMOA.ZMPES4300[] aplicaciones, CartaPorteDetalleWebServiceMOA.ZMPES4310[] calidades, CartaPorteDetalleWebServiceMOA.ZMPES6190[] entregasDescargas, string cartaPorte)
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

            foreach (CartaPorteDetalleWebServiceMOA.ZMPES4300 aplicacion in aplicaciones)
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

            foreach (CartaPorteDetalleWebServiceMOA.ZMPES4310 calidad in calidades)
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
                    resultadoCamara = calidad.RESULTADO_CAM,
                    resultadoReconsideracion = calidad.RESULTADO_REC
                });

                result.calidadTotalAplicados += calidad.KG_APLIC;
                result.calidadTotalNetos += calidad.KG_NETOS;
                result.calidadTotalAplicadosUnidad = "KG";
                result.calidadTotalNetosUnidad = "KG";
            }

            result.calidadTotalAplicadosString = SAPFormatter.FormatearCantidad(result.calidadTotalAplicados, "KG");
            result.calidadTotalNetosString = SAPFormatter.FormatearCantidad(result.calidadTotalNetos, "KG");

            foreach (CartaPorteDetalleWebServiceMOA.ZMPES6190 entregaDescarga in entregasDescargas)
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
        protected override object MapSinPI(WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4910 error, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4300[] aplicaciones, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4310[] calidades, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6190[] entregasDescargas, string cartaPorte)
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

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4300 aplicacion in aplicaciones)
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

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4310 calidad in calidades)
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
                    resultadoCamara = calidad.RESULTADO_CAM,
                    resultadoReconsideracion = calidad.RESULTADO_REC
                });

                result.calidadTotalAplicados += calidad.KG_APLIC;
                result.calidadTotalNetos += calidad.KG_NETOS;
                result.calidadTotalAplicadosUnidad = "KG";
                result.calidadTotalNetosUnidad = "KG";
            }

            result.calidadTotalAplicadosString = SAPFormatter.FormatearCantidad(result.calidadTotalAplicados, "KG");
            result.calidadTotalNetosString = SAPFormatter.FormatearCantidad(result.calidadTotalNetos, "KG");

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6190 entregaDescarga in entregasDescargas)
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
        protected override object Map(CartaPorteDetalleWebServiceMOA.ZMPES4910 error, CartaPorteDetalleWebServiceMOA.ZMPES4300[] aplicaciones, CartaPorteDetalleWebServiceMOA.ZMPES4310[] calidades, CartaPorteDetalleWebServiceMOA.ZMPES6190[] entregasDescargas, string cartaPorte)
        {
            CartaPorteDetallePDFWSMOAResponse result = new CartaPorteDetallePDFWSMOAResponse();

            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }

            foreach (CartaPorteDetalleWebServiceMOA.ZMPES4310 calidad in calidades)
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
        protected override object MapSinPI(WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4910 error, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4300[] aplicaciones, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4310[] calidades, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6190[] entregasDescargas, string cartaPorte)
        {
            CartaPorteDetallePDFWSMOAResponse result = new CartaPorteDetallePDFWSMOAResponse();

            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4310 calidad in calidades)
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
