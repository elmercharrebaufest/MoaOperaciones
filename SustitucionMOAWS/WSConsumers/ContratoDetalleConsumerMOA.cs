using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models.WSMapMOA.Contrato.Detalle;
using SustitucionMOAWS.ContratoDetalleWebServiceMOA;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MOAOP;
using System;
using System.Configuration;
using System.Linq;

namespace SustitucionMOAWS.WSConsumers
{
    public class ContratoDetalleConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserSinPI"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassSinPI"];

        public object request(string proveedor, string contrato)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4390[] ampliaciones_anulaciones = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4390[] { };
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4400[] aplicaciones = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4400[] { };
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4410[] calidad = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4410[] { };
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6200[] caracteristicas = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6200[] { };
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4710[] condiciones_pago = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4710[] { };
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4380[] fijaciones = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4380[] { };
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4420[] hijos = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4420[] { };
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6210[] liquidaciones = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6210[] { };
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4360[] pagos = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4360[] { };
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4520[] resumenes = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4520[] { };

                    var request = new Z_MPMF_MOAOP_DETALLES_CONTRATO()
                    {
                        PE_CONTRATO = contrato?.Length > 10 ? contrato.Substring(0, 10) : contrato,
                        PE_PROVEEDOR = proveedor,
                        T_AMPLI_ANUL = ampliaciones_anulaciones,
                        T_APLICA = aplicaciones,
                        T_CALIDAD = calidad,
                        T_CARACT = caracteristicas,
                        T_COND_PAGO = condiciones_pago,
                        T_FIJA = fijaciones,
                        T_HIJOS = hijos,
                        T_LIQUI = liquidaciones,
                        T_PAGOS = pagos,
                        T_RESUMEN = resumenes
                    };
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_DETALLES_CONTRATO request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MPMF_MOAOP_DETALLES_CONTRATO(request);
                    SapLogHelper.LogResponse(response.ToXml(), "Z_MPMF_MOAOP_DETALLES_CONTRATO");
                    string error = string.Empty;
                    return MapSinPI(contrato, error, response.EX_BOLETOS, response.T_AMPLI_ANUL, response.T_APLICA, response.T_CALIDAD, response.T_CARACT, response.T_COND_PAGO, response.T_FIJA, response.T_HIJOS, response.T_LIQUI, response.T_PAGOS, response.T_RESUMEN);
                }
                else
                {
                    SI_MPMF_MOAOP_DETALLES_CONTRATOClient service = new SI_MPMF_MOAOP_DETALLES_CONTRATOClient();
                    ContratoDetalleWebServiceMOA.ZMPES4390[] ampliaciones_anulaciones = new ContratoDetalleWebServiceMOA.ZMPES4390[] { };
                    ContratoDetalleWebServiceMOA.ZMPES4400[] aplicaciones = new ContratoDetalleWebServiceMOA.ZMPES4400[] { };
                    ContratoDetalleWebServiceMOA.ZMPES4410[] calidad = new ContratoDetalleWebServiceMOA.ZMPES4410[] { };
                    ContratoDetalleWebServiceMOA.ZMPES4410[] calidadExcelDetalle = new ContratoDetalleWebServiceMOA.ZMPES4410[] { };
                    ContratoDetalleWebServiceMOA.ZMPES6200[] caracteristicas = new ContratoDetalleWebServiceMOA.ZMPES6200[] { };
                    ContratoDetalleWebServiceMOA.ZMPES4710[] condiciones_pago = new ContratoDetalleWebServiceMOA.ZMPES4710[] { };
                    ContratoDetalleWebServiceMOA.ZMPES4380[] fijaciones = new ContratoDetalleWebServiceMOA.ZMPES4380[] { };
                    ContratoDetalleWebServiceMOA.ZMPES4420[] hijos = new ContratoDetalleWebServiceMOA.ZMPES4420[] { };
                    ContratoDetalleWebServiceMOA.ZMPES6210[] liquidaciones = new ContratoDetalleWebServiceMOA.ZMPES6210[] { };
                    ContratoDetalleWebServiceMOA.ZMPES4360[] pagos = new ContratoDetalleWebServiceMOA.ZMPES4360[] { };
                    ContratoDetalleWebServiceMOA.ZMPES4520[] resumenes = new ContratoDetalleWebServiceMOA.ZMPES4520[] { };
                    string error;

                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    ContratoDetalleWebServiceMOA.ZMPES5260 boleto = service.SI_MPMF_MOAOP_DETALLES_CONTRATO(contrato, proveedor, ref ampliaciones_anulaciones, ref aplicaciones, ref calidad, ref caracteristicas, ref condiciones_pago, ref fijaciones, ref hijos, ref liquidaciones, ref pagos, ref resumenes, out error);
                    return Map(contrato, error, boleto, ampliaciones_anulaciones, aplicaciones, calidad, caracteristicas, condiciones_pago, fijaciones, hijos, liquidaciones, pagos, resumenes);

                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        protected virtual object Map(string contrato, string error, ContratoDetalleWebServiceMOA.ZMPES5260 boleto, ContratoDetalleWebServiceMOA.ZMPES4390[] ampliaciones_anulaciones, ContratoDetalleWebServiceMOA.ZMPES4400[] aplicaciones, ContratoDetalleWebServiceMOA.ZMPES4410[] calidades, ContratoDetalleWebServiceMOA.ZMPES6200[] caracteristicas, ContratoDetalleWebServiceMOA.ZMPES4710[] condiciones_pago, ContratoDetalleWebServiceMOA.ZMPES4380[] fijaciones, ContratoDetalleWebServiceMOA.ZMPES4420[] hijos, ContratoDetalleWebServiceMOA.ZMPES6210[] liquidaciones, ContratoDetalleWebServiceMOA.ZMPES4360[] pagos, ContratoDetalleWebServiceMOA.ZMPES4520[] resumenes)
        {
            ContratoDetalleWSMOAResponse result = new ContratoDetalleWSMOAResponse();

            result.error = error;
            result.aplicacionesTotalAplicados = 0;
            result.aplicacionesTotalBrutos = 0;
            result.calidadTotalAplicados = 0;
            result.calidadTotalNetos = 0;
            result.contrato = contrato;

            if (boleto != null)
            {
                result.boleto = new Boleto()
                {
                    bolsa = boleto.BOLSA,
                    devAcop = SAPFormatter.FormatearFecha(boleto.DEV_ACOP),
                    envioAfip = SAPFormatter.FormatearFecha(boleto.ENVIO_AFIP),
                    envioBolsa = SAPFormatter.FormatearFecha(boleto.ENVIO_BOLSA),
                    envioSellado = SAPFormatter.FormatearFecha(boleto.ENVIO_SELLADO),
                    estado = boleto.ESTADO,
                    fechaRecepcion = SAPFormatter.FormatearFecha(boleto.FECHA_RECEPCION),
                    obleaAfip = boleto.OBLEA_AFIP,
                    obleaBolsa = boleto.OBLEA_BOLSA,
                    provPlanCanje = boleto.PROV_PLAN_CANJE,
                    tipoBoleto = boleto.TIPO_BOLETO,
                    vueltaAfip = SAPFormatter.FormatearFecha(boleto.VUELTA_AFIP),
                    vueltaBolsa = SAPFormatter.FormatearFecha(boleto.VUELTA_BOLSA),
                    vueltaSellado = SAPFormatter.FormatearFecha(boleto.VUELTA_SELLADO),
                    observacion = boleto.OBSERVACIONES
                };

            }

            foreach (ContratoDetalleWebServiceMOA.ZMPES4390 ampAnul in ampliaciones_anulaciones)
            {
                result.ampliacionesAnulaciones.Add(new AmpliacionAnulacionView()
                {
                    cantidadString = SAPFormatter.FormatearCantidad(ampAnul.CANTIDAD, ampAnul.UNIDAD),
                    cantidad = ampAnul.CANTIDAD,
                    fecha = SAPFormatter.FormatearFecha(ampAnul.FECHA),
                    importeString = SAPFormatter.FormatearMonto(ampAnul.IMPORTE, ampAnul.MON_IMPORTE),
                    importe = ampAnul.IMPORTE,
                    tipo = ampAnul.TIPO
                });
            }

            foreach (ContratoDetalleWebServiceMOA.ZMPES4400 aplicacion in aplicaciones)
            {
                result.aplicaciones.Add(new AplicacionView()
                {
                    cantidadString = SAPFormatter.FormatearCantidad(aplicacion.CANTIDAD, aplicacion.UNIME),
                    cantidad = aplicacion.CANTIDAD,
                    ccpp = aplicacion.CCPP,
                    descarga = aplicacion.DESCARGA,
                    fecha = SAPFormatter.FormatearFecha(aplicacion.FECHA),
                    kgBrutosString = SAPFormatter.FormatearCantidad(aplicacion.KG_BRUTOS, aplicacion.UNIDAD_BRUTOS),
                    kgBrutos = aplicacion.KG_BRUTOS,
                    kgNetosString = SAPFormatter.FormatearCantidad(aplicacion.KG_NETOS, aplicacion.UNIDAD_NETOS),
                    kgNetos = aplicacion.KG_NETOS
                });

                result.aplicacionesTotalAplicados += aplicacion.KG_NETOS;
                result.aplicacionesTotalBrutos += aplicacion.KG_BRUTOS;
                result.aplicacionesTotalAplicadosUnidad = aplicacion.UNIDAD_NETOS;
                result.aplicacionesTotalBrutosUnidad = aplicacion.UNIDAD_BRUTOS;

            }

            result.aplicacionesTotalAplicadosString = SAPFormatter.FormatearCantidad(result.aplicacionesTotalAplicados, result.aplicacionesTotalAplicadosUnidad);
            result.aplicacionesTotalBrutosString = SAPFormatter.FormatearCantidad(result.aplicacionesTotalBrutos, result.aplicacionesTotalBrutosUnidad);


            result.calidad = calidades.GroupBy(x => x.CCPP)
                .Select(x =>
                {
                    var calidad = new Calidad
                    {
                        ccpp = x.Key,
                        kgAplicadosTotal = SAPFormatter.FormatearCantidad(x.Sum(r => r.KG_APLI), "KG"),
                        kgNetosTotal = SAPFormatter.FormatearCantidad(x.Sum(r => r.KG_NETOS), "KG"),
                        kgDtoTotal = SAPFormatter.FormatearCantidad(x.Sum(r => r.KG_DTO), "KG"),
                        dtoPorcTotal = SAPFormatter.FormatearCantidad(x.Sum(r => r.DTO), "%"),
                        camaraAPresent = x.First().CAMARA_A_PRESENT,
                        registros = x.Select(e => new CalidadElement()
                        {
                            calaResul = e.CALA_RESUL,
                            camaResul = e.CARACT.ToUpper().Contains("HUMEDAD") ? e.CALA_RESUL : e.CAMA_RESUL,
                            caract = e.CARACT,
                            dto = e.DTO,
                            kgDtoValor = e.KG_DTO,
                            kgDto = SAPFormatter.FormatearCantidad(e.KG_DTO, e.UNIDAD),
                            certificado = e.NRO_CERT
                        }).ToList()
                    };
                    calidad.SetearEstadoCamara();
                    return calidad;
                }
                ).ToList();

            result.calidadExcelDetalle = calidades.Select(x => new CalidadExcelDetalle
            {
                ccpp = x.CCPP,
                calaResul = x.CALA_RESUL,
                camaResul = x.CAMA_RESUL,
                caract = x.CARACT,
                dto = x.DTO,
                kgApli = x.KG_APLI,
                kgDto = x.KG_DTO,
                kgNetos = x.KG_NETOS,
                nroCert = x.NRO_CERT,
                recCert = x.REC_CERT,
                recResul = x.REC_RESUL,
                unidad = x.UNIDAD

            }).ToList();

            foreach (ContratoDetalleWebServiceMOA.ZMPES4410 calidad in calidades)
            {
                result.calidadTotalAplicados += calidad.KG_APLI;
                result.calidadTotalNetos += calidad.KG_NETOS;
                result.calidadTotalAplicadosUnidad = calidad.UNIDAD;
                result.calidadTotalNetosUnidad = calidad.UNIDAD;
            }

            result.calidadTotalAplicadosString = SAPFormatter.FormatearCantidad(result.calidadTotalAplicados, result.calidadTotalAplicadosUnidad);
            result.calidadTotalNetosString = SAPFormatter.FormatearCantidad(result.calidadTotalNetos, result.calidadTotalNetosUnidad);

            foreach (ContratoDetalleWebServiceMOA.ZMPES6200 caracteristica in caracteristicas)
            {
                result.caracteristicas.Add(new CaracteristicaView()
                {
                    calificacion = caracteristica.CALIFICACION,
                    canje = caracteristica.CANJE,
                    cantidad = caracteristica.CANTIDAD,
                    cantidadString = SAPFormatter.FormatearCantidad(caracteristica.CANTIDAD, caracteristica.UNIDAD),
                    cdCdg = caracteristica.CD_CDG,
                    cesion = caracteristica.CESION,
                    condPagoFija = caracteristica.COND_PAGO_FIJA,
                    corredor = caracteristica.CORREDOR,
                    cosecha = caracteristica.COSECHA,
                    descarga = caracteristica.DESCARGA,
                    descuentoAcarreo = caracteristica.DESCUENTO_ACARREO,
                    entregaMax = SAPFormatter.FormatearFecha(caracteristica.ENTREGA_MAX),
                    entregaMin = SAPFormatter.FormatearFecha(caracteristica.ENTREGA_MIN),
                    estadoBol = caracteristica.ESTADO_BOL,
                    fechaConcerta = SAPFormatter.FormatearFecha(caracteristica.FECHA_CONCERTA),
                    fechaTopeFija = SAPFormatter.FormatearFecha(caracteristica.FECHA_TOPE_FIJA),
                    fijaDiariaMax = caracteristica.FIJA_DIARIA_MAX,
                    fijaDiariaMin = caracteristica.FIJA_DIARIA_MIN,
                    importeAPrecio = caracteristica.IMPORTE_A_PRECIO,
                    importeSPrecio = caracteristica.IMPORTE_S_PRECIO,
                    monedaAPrecio = caracteristica.MONEDA_A_PRECIO,
                    monedaSPrecio = caracteristica.MONEDA_S_PRECIO,
                    nomCorredor = caracteristica.NOM_CORREDOR,
                    nomVendedor = caracteristica.NOM_VENDEDOR,
                    pagoDirVend = caracteristica.PAGO_DIR_VEND,
                    pagoParcial = caracteristica.PAGO_PARCIAL,
                    pizarraRef = caracteristica.PIZARRA_REF,
                    porcAPrecio = caracteristica.PORC_A_PRECIO,
                    porcSPrecio = caracteristica.PORC_S_PRECIO,
                    procedencia = caracteristica.PROCEDENCIA,
                    retenerIva = caracteristica.RETENER_IVA,
                    standardCali = caracteristica.STANDARD_CALI,
                    tipo = caracteristica.TIPO,
                    toleMax = caracteristica.TOLE_MAX,
                    toleMin = caracteristica.TOLE_MIN,
                    vendedor = caracteristica.VENDEDOR,
                    warrant = caracteristica.WARRANT,
                    confirma = caracteristica.CONFIRMA
                });
            }

            foreach (ContratoDetalleWebServiceMOA.ZMPES4710 condicionPago in condiciones_pago)
            {
                result.condicionesPago.Add(condicionPago.MENSAJE);
            }

            foreach (ContratoDetalleWebServiceMOA.ZMPES4380 fijacion in fijaciones)
            {
                result.fijaciones.Add(new FijacionView()
                {
                    fecha = SAPFormatter.FormatearFecha(fijacion.FECHA),
                    kilosFijaString = SAPFormatter.FormatearCantidad(fijacion.KILOS_FIJA, fijacion.UNIDAD),
                    kilosFija = fijacion.KILOS_FIJA,
                    nroFija = fijacion.NRO_FIJA,
                    precioString = SAPFormatter.FormatearMonto(fijacion.PRECIO, fijacion.MON_PRECIO),
                    precio = fijacion.PRECIO,

                });
            }

            foreach (ContratoDetalleWebServiceMOA.ZMPES4420 hijo in hijos)
            {
                result.hijos.Add(new HijoView()
                {
                    cantidadString = SAPFormatter.FormatearCantidad(hijo.CANTIDAD, hijo.UNIDAD),
                    cantidad = hijo.CANTIDAD,
                    contrMadre = hijo.CONTR_MADRE,
                    contrMolinos = hijo.CONTR_MOLINOS,
                    contrProve = hijo.CONTR_PROVE,
                    fecha = SAPFormatter.FormatearFecha(hijo.FECHA),
                    precioString = SAPFormatter.FormatearMonto(hijo.PRECIO, hijo.MON_PRECIO),
                    precio = hijo.PRECIO
                });
            }

            foreach (ContratoDetalleWebServiceMOA.ZMPES6210 hijo in liquidaciones)
            {
                result.liquidaciones.Add(new LiquidacionView()
                {
                    cantidadString = SAPFormatter.FormatearCantidad(hijo.CANTIDAD, hijo.UN_CANTIDAD),
                    cantidad = hijo.CANTIDAD,
                    fecha = SAPFormatter.FormatearFecha(hijo.FECHA),
                    pedido = hijo.PEDIDO,
                    precioString = SAPFormatter.FormatearMonto(hijo.PRECIO, hijo.MON_PRECIO),
                    precio = hijo.PRECIO,
                    tipo = hijo.TIPO,
                    totalString = SAPFormatter.FormatearMonto(hijo.TOTAL, hijo.MON_TOTAL),
                    total = hijo.TOTAL,
                    comprobante = hijo.COMPROBANTE,
                });
            }

            foreach (ContratoDetalleWebServiceMOA.ZMPES4360 pago in pagos)
            {
                result.pagos.Add(new PagoView()
                {
                    brutoString = SAPFormatter.FormatearMonto(pago.BRUTO, pago.MONEDA),
                    bruto = pago.BRUTO,
                    comprobante = pago.COMPROBANTE,
                    fecha = SAPFormatter.FormatearFecha(pago.FECHA),
                    idPago = pago.ID_PAGO,
                    ivaString = SAPFormatter.FormatearMonto(pago.IVA, pago.MONEDA),
                    iva = pago.IVA,
                    netoString = SAPFormatter.FormatearMonto(pago.NETO, pago.MONEDA),
                    neto = pago.NETO,
                    retencionesString = SAPFormatter.FormatearMonto(pago.RETENCIONES, pago.MONEDA),
                    retenciones = pago.RETENCIONES
                });
            }

            foreach (ContratoDetalleWebServiceMOA.ZMPES4520 resumen in resumenes)
            {
                result.resumen.Add(new ResumenView()
                {
                    cantEntreString = SAPFormatter.FormatearCantidad(resumen.CANT_ENTRE, resumen.UNI_ENTRE),
                    cantEntre = resumen.CANT_ENTRE,
                    cantFijaString = SAPFormatter.FormatearCantidad(resumen.CANT_FIJA, resumen.UNI_FIJA),
                    cantFija = resumen.CANT_FIJA,
                    cantLiquiString = SAPFormatter.FormatearCantidad(resumen.CANT_LIQUI, resumen.UNI_LIQUI),
                    cantLiqui = resumen.CANT_LIQUI,
                    cantPendEntreString = SAPFormatter.FormatearCantidad(resumen.CANT_PEND_ENTRE, resumen.UNI_PEND_ENTRE),
                    cantPendEntre = resumen.CANT_PEND_ENTRE,
                    precioString = SAPFormatter.FormatearMonto(resumen.PRECIO, resumen.MONEDA),
                    precio = resumen.PRECIO,
                    contraMadre = resumen.CONTRA_MADRE,
                    contrato = resumen.CONTRATO,
                    estado = resumen.ESTADO,
                    producto = resumen.PRODUCTO
                });
            }


            return result;
        }
        protected virtual object MapSinPI(string contrato, string error, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES5260 boleto, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4390[] ampliaciones_anulaciones, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4400[] aplicaciones, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4410[] calidades, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6200[] caracteristicas, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4710[] condiciones_pago, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4380[] fijaciones, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4420[] hijos, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6210[] liquidaciones, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4360[] pagos, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4520[] resumenes)
        {
            ContratoDetalleWSMOAResponse result = new ContratoDetalleWSMOAResponse();

            result.error = error;
            result.aplicacionesTotalAplicados = 0;
            result.aplicacionesTotalBrutos = 0;
            result.calidadTotalAplicados = 0;
            result.calidadTotalNetos = 0;
            result.contrato = contrato;

            if (boleto != null)
            {
                result.boleto = new Boleto()
                {
                    bolsa = boleto.BOLSA,
                    devAcop = SAPFormatter.FormatearFecha(boleto.DEV_ACOP),
                    envioAfip = SAPFormatter.FormatearFecha(boleto.ENVIO_AFIP),
                    envioBolsa = SAPFormatter.FormatearFecha(boleto.ENVIO_BOLSA),
                    envioSellado = SAPFormatter.FormatearFecha(boleto.ENVIO_SELLADO),
                    estado = boleto.ESTADO,
                    fechaRecepcion = SAPFormatter.FormatearFecha(boleto.FECHA_RECEPCION),
                    obleaAfip = boleto.OBLEA_AFIP,
                    obleaBolsa = boleto.OBLEA_BOLSA,
                    provPlanCanje = boleto.PROV_PLAN_CANJE,
                    tipoBoleto = boleto.TIPO_BOLETO,
                    vueltaAfip = SAPFormatter.FormatearFecha(boleto.VUELTA_AFIP),
                    vueltaBolsa = SAPFormatter.FormatearFecha(boleto.VUELTA_BOLSA),
                    vueltaSellado = SAPFormatter.FormatearFecha(boleto.VUELTA_SELLADO),
                    observacion = boleto.OBSERVACIONES
                };

            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4390 ampAnul in ampliaciones_anulaciones)
            {
                result.ampliacionesAnulaciones.Add(new AmpliacionAnulacionView()
                {
                    cantidadString = SAPFormatter.FormatearCantidad(ampAnul.CANTIDAD, ampAnul.UNIDAD),
                    cantidad = ampAnul.CANTIDAD,
                    fecha = SAPFormatter.FormatearFecha(ampAnul.FECHA),
                    importeString = SAPFormatter.FormatearMonto(ampAnul.IMPORTE, ampAnul.MON_IMPORTE),
                    importe = ampAnul.IMPORTE,
                    tipo = ampAnul.TIPO
                });
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4400 aplicacion in aplicaciones)
            {
                result.aplicaciones.Add(new AplicacionView()
                {
                    cantidadString = SAPFormatter.FormatearCantidad(aplicacion.CANTIDAD, aplicacion.UNIME),
                    cantidad = aplicacion.CANTIDAD,
                    ccpp = aplicacion.CCPP,
                    descarga = aplicacion.DESCARGA,
                    fecha = SAPFormatter.FormatearFecha(aplicacion.FECHA),
                    kgBrutosString = SAPFormatter.FormatearCantidad(aplicacion.KG_BRUTOS, aplicacion.UNIDAD_BRUTOS),
                    kgBrutos = aplicacion.KG_BRUTOS,
                    kgNetosString = SAPFormatter.FormatearCantidad(aplicacion.KG_NETOS, aplicacion.UNIDAD_NETOS),
                    kgNetos = aplicacion.KG_NETOS
                });

                result.aplicacionesTotalAplicados += aplicacion.KG_NETOS;
                result.aplicacionesTotalBrutos += aplicacion.KG_BRUTOS;
                result.aplicacionesTotalAplicadosUnidad = aplicacion.UNIDAD_NETOS;
                result.aplicacionesTotalBrutosUnidad = aplicacion.UNIDAD_BRUTOS;

            }

            result.aplicacionesTotalAplicadosString = SAPFormatter.FormatearCantidad(result.aplicacionesTotalAplicados, result.aplicacionesTotalAplicadosUnidad);
            result.aplicacionesTotalBrutosString = SAPFormatter.FormatearCantidad(result.aplicacionesTotalBrutos, result.aplicacionesTotalBrutosUnidad);


            result.calidad = calidades.GroupBy(x => x.CCPP)
                .Select(x =>
                {
                    var calidad = new Calidad
                    {
                        ccpp = x.Key,
                        kgAplicadosTotal = SAPFormatter.FormatearCantidad(x.Sum(r => r.KG_APLI), "KG"),
                        kgNetosTotal = SAPFormatter.FormatearCantidad(x.Sum(r => r.KG_NETOS), "KG"),
                        kgDtoTotal = SAPFormatter.FormatearCantidad(x.Sum(r => r.KG_DTO), "KG"),
                        dtoPorcTotal = SAPFormatter.FormatearCantidad(x.Sum(r => r.DTO), "%"),
                        camaraAPresent = x.First().CAMARA_A_PRESENT,
                        registros = x.Select(e => new CalidadElement()
                        {
                            calaResul = e.CALA_RESUL,
                            camaResul = e.CARACT.ToUpper().Contains("HUMEDAD") ? e.CALA_RESUL : e.CAMA_RESUL,
                            caract = e.CARACT,
                            dto = e.DTO,
                            kgDtoValor = e.KG_DTO,
                            kgDto = SAPFormatter.FormatearCantidad(e.KG_DTO, e.UNIDAD),
                            certificado = e.NRO_CERT
                        }).ToList()
                    };
                    calidad.SetearEstadoCamara();
                    return calidad;
                }
                ).ToList();

            result.calidadExcelDetalle = calidades.Select(x => new CalidadExcelDetalle
            {
                ccpp = x.CCPP,
                calaResul = x.CALA_RESUL,
                camaResul = x.CAMA_RESUL,
                caract = x.CARACT,
                dto = x.DTO,
                kgApli = x.KG_APLI,
                kgDto = x.KG_DTO,
                kgNetos = x.KG_NETOS,
                nroCert = x.NRO_CERT,
                recCert = x.REC_CERT,
                recResul = x.REC_RESUL,
                unidad = x.UNIDAD

            }).ToList();

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4410 calidad in calidades)
            {
                result.calidadTotalAplicados += calidad.KG_APLI;
                result.calidadTotalNetos += calidad.KG_NETOS;
                result.calidadTotalAplicadosUnidad = calidad.UNIDAD;
                result.calidadTotalNetosUnidad = calidad.UNIDAD;
            }

            result.calidadTotalAplicadosString = SAPFormatter.FormatearCantidad(result.calidadTotalAplicados, result.calidadTotalAplicadosUnidad);
            result.calidadTotalNetosString = SAPFormatter.FormatearCantidad(result.calidadTotalNetos, result.calidadTotalNetosUnidad);

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6200 caracteristica in caracteristicas)
            {
                result.caracteristicas.Add(new CaracteristicaView()
                {
                    calificacion = caracteristica.CALIFICACION,
                    canje = caracteristica.CANJE,
                    cantidad = caracteristica.CANTIDAD,
                    cantidadString = SAPFormatter.FormatearCantidad(caracteristica.CANTIDAD, caracteristica.UNIDAD),
                    cdCdg = caracteristica.CD_CDG,
                    cesion = caracteristica.CESION,
                    condPagoFija = caracteristica.COND_PAGO_FIJA,
                    corredor = caracteristica.CORREDOR,
                    cosecha = caracteristica.COSECHA,
                    descarga = caracteristica.DESCARGA,
                    descuentoAcarreo = caracteristica.DESCUENTO_ACARREO,
                    entregaMax = SAPFormatter.FormatearFecha(caracteristica.ENTREGA_MAX),
                    entregaMin = SAPFormatter.FormatearFecha(caracteristica.ENTREGA_MIN),
                    estadoBol = caracteristica.ESTADO_BOL,
                    fechaConcerta = SAPFormatter.FormatearFecha(caracteristica.FECHA_CONCERTA),
                    fechaTopeFija = SAPFormatter.FormatearFecha(caracteristica.FECHA_TOPE_FIJA),
                    fijaDiariaMax = caracteristica.FIJA_DIARIA_MAX,
                    fijaDiariaMin = caracteristica.FIJA_DIARIA_MIN,
                    importeAPrecio = caracteristica.IMPORTE_A_PRECIO,
                    importeSPrecio = caracteristica.IMPORTE_S_PRECIO,
                    monedaAPrecio = caracteristica.MONEDA_A_PRECIO,
                    monedaSPrecio = caracteristica.MONEDA_S_PRECIO,
                    nomCorredor = caracteristica.NOM_CORREDOR,
                    nomVendedor = caracteristica.NOM_VENDEDOR,
                    pagoDirVend = caracteristica.PAGO_DIR_VEND,
                    pagoParcial = caracteristica.PAGO_PARCIAL,
                    pizarraRef = caracteristica.PIZARRA_REF,
                    porcAPrecio = caracteristica.PORC_A_PRECIO,
                    porcSPrecio = caracteristica.PORC_S_PRECIO,
                    procedencia = caracteristica.PROCEDENCIA,
                    retenerIva = caracteristica.RETENER_IVA,
                    standardCali = caracteristica.STANDARD_CALI,
                    tipo = caracteristica.TIPO,
                    toleMax = caracteristica.TOLE_MAX,
                    toleMin = caracteristica.TOLE_MIN,
                    vendedor = caracteristica.VENDEDOR,
                    warrant = caracteristica.WARRANT,
                    confirma = caracteristica.CONFIRMA
                });
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4710 condicionPago in condiciones_pago)
            {
                result.condicionesPago.Add(condicionPago.MENSAJE);
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4380 fijacion in fijaciones)
            {
                result.fijaciones.Add(new FijacionView()
                {
                    fecha = SAPFormatter.FormatearFecha(fijacion.FECHA),
                    kilosFijaString = SAPFormatter.FormatearCantidad(fijacion.KILOS_FIJA, fijacion.UNIDAD),
                    kilosFija = fijacion.KILOS_FIJA,
                    nroFija = fijacion.NRO_FIJA,
                    precioString = SAPFormatter.FormatearMonto(fijacion.PRECIO, fijacion.MON_PRECIO),
                    precio = fijacion.PRECIO,

                });
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4420 hijo in hijos)
            {
                result.hijos.Add(new HijoView()
                {
                    cantidadString = SAPFormatter.FormatearCantidad(hijo.CANTIDAD, hijo.UNIDAD),
                    cantidad = hijo.CANTIDAD,
                    contrMadre = hijo.CONTR_MADRE,
                    contrMolinos = hijo.CONTR_MOLINOS,
                    contrProve = hijo.CONTR_PROVE,
                    fecha = SAPFormatter.FormatearFecha(hijo.FECHA),
                    precioString = SAPFormatter.FormatearMonto(hijo.PRECIO, hijo.MON_PRECIO),
                    precio = hijo.PRECIO
                });
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6210 hijo in liquidaciones)
            {
                result.liquidaciones.Add(new LiquidacionView()
                {
                    cantidadString = SAPFormatter.FormatearCantidad(hijo.CANTIDAD, hijo.UN_CANTIDAD),
                    cantidad = hijo.CANTIDAD,
                    fecha = SAPFormatter.FormatearFecha(hijo.FECHA),
                    pedido = hijo.PEDIDO,
                    precioString = SAPFormatter.FormatearMonto(hijo.PRECIO, hijo.MON_PRECIO),
                    precio = hijo.PRECIO,
                    tipo = hijo.TIPO,
                    totalString = SAPFormatter.FormatearMonto(hijo.TOTAL, hijo.MON_TOTAL),
                    total = hijo.TOTAL,
                    comprobante = hijo.COMPROBANTE,
                });
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4360 pago in pagos)
            {
                result.pagos.Add(new PagoView()
                {
                    brutoString = SAPFormatter.FormatearMonto(pago.BRUTO, pago.MONEDA),
                    bruto = pago.BRUTO,
                    comprobante = pago.COMPROBANTE,
                    fecha = SAPFormatter.FormatearFecha(pago.FECHA),
                    idPago = pago.ID_PAGO,
                    ivaString = SAPFormatter.FormatearMonto(pago.IVA, pago.MONEDA),
                    iva = pago.IVA,
                    netoString = SAPFormatter.FormatearMonto(pago.NETO, pago.MONEDA),
                    neto = pago.NETO,
                    retencionesString = SAPFormatter.FormatearMonto(pago.RETENCIONES, pago.MONEDA),
                    retenciones = pago.RETENCIONES
                });
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4520 resumen in resumenes)
            {
                result.resumen.Add(new ResumenView()
                {
                    cantEntreString = SAPFormatter.FormatearCantidad(resumen.CANT_ENTRE, resumen.UNI_ENTRE),
                    cantEntre = resumen.CANT_ENTRE,
                    cantFijaString = SAPFormatter.FormatearCantidad(resumen.CANT_FIJA, resumen.UNI_FIJA),
                    cantFija = resumen.CANT_FIJA,
                    cantLiquiString = SAPFormatter.FormatearCantidad(resumen.CANT_LIQUI, resumen.UNI_LIQUI),
                    cantLiqui = resumen.CANT_LIQUI,
                    cantPendEntreString = SAPFormatter.FormatearCantidad(resumen.CANT_PEND_ENTRE, resumen.UNI_PEND_ENTRE),
                    cantPendEntre = resumen.CANT_PEND_ENTRE,
                    precioString = SAPFormatter.FormatearMonto(resumen.PRECIO, resumen.MONEDA),
                    precio = resumen.PRECIO,
                    contraMadre = resumen.CONTRA_MADRE,
                    contrato = resumen.CONTRATO,
                    estado = resumen.ESTADO,
                    producto = resumen.PRODUCTO
                });
            }


            return result;
        }

    }

    public class ContratoDetalleExcelConsumerMOA : ContratoDetalleConsumerMOA
    {
        protected override object Map(string contrato, string error, ContratoDetalleWebServiceMOA.ZMPES5260 boleto, ContratoDetalleWebServiceMOA.ZMPES4390[] ampliaciones_anulaciones, ContratoDetalleWebServiceMOA.ZMPES4400[] aplicaciones, ContratoDetalleWebServiceMOA.ZMPES4410[] calidades, ContratoDetalleWebServiceMOA.ZMPES6200[] caracteristicas, ContratoDetalleWebServiceMOA.ZMPES4710[] condiciones_pago, ContratoDetalleWebServiceMOA.ZMPES4380[] fijaciones, ContratoDetalleWebServiceMOA.ZMPES4420[] hijos, ContratoDetalleWebServiceMOA.ZMPES6210[] liquidaciones, ContratoDetalleWebServiceMOA.ZMPES4360[] pagos, ContratoDetalleWebServiceMOA.ZMPES4520[] resumenes)
        {
            ContratoDetalleExcelWSMOAResponse result = new ContratoDetalleExcelWSMOAResponse();

            result.error = error;
            result.aplicacionesTotalAplicados = 0;
            result.aplicacionesTotalBrutos = 0;
            result.calidadTotalAplicados = 0;
            result.calidadTotalNetos = 0;
            result.contrato = contrato;

            if (boleto != null)
            {
                result.boleto = new Boleto()
                {
                    bolsa = boleto.BOLSA,
                    devAcop = SAPFormatter.FormatearFecha(boleto.DEV_ACOP),
                    envioAfip = SAPFormatter.FormatearFecha(boleto.ENVIO_AFIP),
                    envioBolsa = SAPFormatter.FormatearFecha(boleto.ENVIO_BOLSA),
                    envioSellado = SAPFormatter.FormatearFecha(boleto.ENVIO_SELLADO),
                    estado = boleto.ESTADO,
                    fechaRecepcion = SAPFormatter.FormatearFecha(boleto.FECHA_RECEPCION),
                    obleaAfip = boleto.OBLEA_AFIP,
                    obleaBolsa = boleto.OBLEA_BOLSA,
                    provPlanCanje = boleto.PROV_PLAN_CANJE,
                    tipoBoleto = boleto.TIPO_BOLETO,
                    vueltaAfip = SAPFormatter.FormatearFecha(boleto.VUELTA_AFIP),
                    vueltaBolsa = SAPFormatter.FormatearFecha(boleto.VUELTA_BOLSA),
                    vueltaSellado = SAPFormatter.FormatearFecha(boleto.VUELTA_SELLADO),
                    observacion = boleto.OBSERVACIONES
                };

            }

            foreach (ContratoDetalleWebServiceMOA.ZMPES4390 ampAnul in ampliaciones_anulaciones)
            {
                result.ampliacionesAnulaciones.Add(new AmpliacionAnulacion()
                {
                    unidad = ampAnul.UNIDAD,
                    cantidad = ampAnul.CANTIDAD,
                    fecha = SAPFormatter.FormatearFecha(ampAnul.FECHA),
                    moneda = ampAnul.MON_IMPORTE,
                    importe = ampAnul.IMPORTE,
                    tipo = ampAnul.TIPO
                });
            }

            foreach (ContratoDetalleWebServiceMOA.ZMPES4400 aplicacion in aplicaciones)
            {
                result.aplicaciones.Add(new Aplicacion()
                {
                    unidadCantidad = aplicacion.UNIME,
                    cantidad = aplicacion.CANTIDAD,
                    ccpp = aplicacion.CCPP,
                    descarga = aplicacion.DESCARGA,
                    fecha = SAPFormatter.FormatearFecha(aplicacion.FECHA),
                    unidadBrutos = aplicacion.UNIDAD_BRUTOS,
                    kgBrutos = aplicacion.KG_BRUTOS,
                    unidadNetos = aplicacion.UNIDAD_NETOS,
                    kgNetos = aplicacion.KG_NETOS
                });

                result.aplicacionesTotalAplicados += aplicacion.KG_NETOS;
                result.aplicacionesTotalBrutos += aplicacion.KG_BRUTOS;
                result.aplicacionesTotalAplicadosUnidad = aplicacion.UNIDAD_NETOS;
                result.aplicacionesTotalBrutosUnidad = aplicacion.UNIDAD_BRUTOS;

            }

            result.aplicacionesTotalAplicadosString = SAPFormatter.FormatearCantidad(result.aplicacionesTotalAplicados, result.aplicacionesTotalAplicadosUnidad);
            result.aplicacionesTotalBrutosString = SAPFormatter.FormatearCantidad(result.aplicacionesTotalBrutos, result.aplicacionesTotalBrutosUnidad);


            result.calidad = calidades.GroupBy(x => x.CCPP)
                .Select(x =>
                {
                    var calidad = new Calidad
                    {
                        ccpp = x.Key,
                        kgAplicadosTotal = SAPFormatter.FormatearCantidad(x.Sum(r => r.KG_APLI), "KG"),
                        kgNetosTotal = SAPFormatter.FormatearCantidad(x.Sum(r => r.KG_NETOS), "KG"),
                        kgDtoTotal = SAPFormatter.FormatearCantidad(x.Sum(r => r.KG_DTO), "KG"),
                        dtoPorcTotal = SAPFormatter.FormatearCantidad(x.Sum(r => r.DTO), "%"),
                        camaraAPresent = x.First().CAMARA_A_PRESENT,
                        registros = x.Select(e => new CalidadElement()
                        {
                            calaResul = e.CALA_RESUL,
                            camaResul = e.CARACT.ToUpper().Contains("HUMEDAD") ? e.CALA_RESUL : e.CAMA_RESUL,
                            caract = e.CARACT,
                            dto = e.DTO,
                            kgDto = SAPFormatter.FormatearCantidad(e.KG_DTO, e.UNIDAD),
                            kgDtoValor = e.KG_DTO,
                            certificado = e.NRO_CERT,
                            //Información para excels
                            recResul = e.REC_RESUL,
                            recCert = e.REC_CERT,
                            kgApli = e.KG_APLI,
                            kgNetos = e.KG_NETOS,
                            unidad = e.UNIDAD,
                        }).ToList()
                    };
                    calidad.SetearEstadoCamara();
                    return calidad;
                }).ToList();

            result.calidadExcelDetalle = calidades.Select(x => new CalidadExcelDetalle
            {
                ccpp = x.CCPP,
                calaResul = x.CALA_RESUL,
                camaResul = x.CAMA_RESUL,
                caract = x.CARACT,
                dto = x.DTO,
                kgApli = x.KG_APLI,
                kgDto = x.KG_DTO,
                kgNetos = x.KG_NETOS,
                nroCert = x.NRO_CERT,
                recCert = x.REC_CERT,
                recResul = x.REC_RESUL,
                unidad = x.UNIDAD

            }).ToList();

            foreach (ContratoDetalleWebServiceMOA.ZMPES4410 calidad in calidades)
            {
                result.calidadTotalAplicados += calidad.KG_APLI;
                result.calidadTotalNetos += calidad.KG_NETOS;
                result.calidadTotalAplicadosUnidad = calidad.UNIDAD;
                result.calidadTotalNetosUnidad = calidad.UNIDAD;
            }

            result.calidadTotalAplicadosString = SAPFormatter.FormatearCantidad(result.calidadTotalAplicados, result.calidadTotalAplicadosUnidad);
            result.calidadTotalNetosString = SAPFormatter.FormatearCantidad(result.calidadTotalNetos, result.calidadTotalNetosUnidad);

            foreach (ContratoDetalleWebServiceMOA.ZMPES6200 caracteristica in caracteristicas)
            {
                result.caracteristicas.Add(new Caracteristica()
                {
                    calificacion = caracteristica.CALIFICACION,
                    canje = caracteristica.CANJE,
                    cantidad = caracteristica.CANTIDAD,
                    unidad = caracteristica.UNIDAD,
                    cdCdg = caracteristica.CD_CDG,
                    cesion = caracteristica.CESION,
                    condPagoFija = caracteristica.COND_PAGO_FIJA,
                    corredor = caracteristica.CORREDOR,
                    cosecha = caracteristica.COSECHA,
                    descarga = caracteristica.DESCARGA,
                    descuentoAcarreo = caracteristica.DESCUENTO_ACARREO,
                    entregaMax = SAPFormatter.FormatearFecha(caracteristica.ENTREGA_MAX),
                    entregaMin = SAPFormatter.FormatearFecha(caracteristica.ENTREGA_MIN),
                    estadoBol = caracteristica.ESTADO_BOL,
                    fechaConcerta = SAPFormatter.FormatearFecha(caracteristica.FECHA_CONCERTA),
                    fechaTopeFija = SAPFormatter.FormatearFecha(caracteristica.FECHA_TOPE_FIJA),
                    fijaDiariaMax = caracteristica.FIJA_DIARIA_MAX,
                    fijaDiariaMin = caracteristica.FIJA_DIARIA_MIN,
                    importeAPrecio = caracteristica.IMPORTE_A_PRECIO,
                    importeSPrecio = caracteristica.IMPORTE_S_PRECIO,
                    monedaAPrecio = caracteristica.MONEDA_A_PRECIO,
                    monedaSPrecio = caracteristica.MONEDA_S_PRECIO,
                    nomCorredor = caracteristica.NOM_CORREDOR,
                    nomVendedor = caracteristica.NOM_VENDEDOR,
                    pagoDirVend = caracteristica.PAGO_DIR_VEND,
                    pagoParcial = caracteristica.PAGO_PARCIAL,
                    pizarraRef = caracteristica.PIZARRA_REF,
                    porcAPrecio = caracteristica.PORC_A_PRECIO,
                    porcSPrecio = caracteristica.PORC_S_PRECIO,
                    procedencia = caracteristica.PROCEDENCIA,
                    retenerIva = caracteristica.RETENER_IVA,
                    standardCali = caracteristica.STANDARD_CALI,
                    tipo = caracteristica.TIPO,
                    toleMax = caracteristica.TOLE_MAX,
                    toleMin = caracteristica.TOLE_MIN,
                    vendedor = caracteristica.VENDEDOR,
                    warrant = caracteristica.WARRANT,
                    confirma = caracteristica.CONFIRMA
                });
            }

            foreach (ContratoDetalleWebServiceMOA.ZMPES4710 condicionPago in condiciones_pago)
            {
                result.condicionesPago.Add(condicionPago.MENSAJE);
            }

            foreach (ContratoDetalleWebServiceMOA.ZMPES4380 fijacion in fijaciones)
            {
                result.fijaciones.Add(new Fijacion()
                {
                    fecha = SAPFormatter.FormatearFecha(fijacion.FECHA),
                    unidad = fijacion.UNIDAD,
                    kilosFija = fijacion.KILOS_FIJA,
                    nroFija = fijacion.NRO_FIJA,
                    moneda = fijacion.MON_PRECIO,
                    precio = fijacion.PRECIO
                });
            }

            foreach (ContratoDetalleWebServiceMOA.ZMPES4420 hijo in hijos)
            {
                result.hijos.Add(new Hijo()
                {
                    unidad = hijo.UNIDAD,
                    cantidad = hijo.CANTIDAD,
                    contrMadre = hijo.CONTR_MADRE,
                    contrMolinos = hijo.CONTR_MOLINOS,
                    contrProve = hijo.CONTR_PROVE,
                    fecha = SAPFormatter.FormatearFecha(hijo.FECHA),
                    precio = hijo.PRECIO,
                    moneda = hijo.MON_PRECIO
                });
            }

            foreach (ContratoDetalleWebServiceMOA.ZMPES6210 hijo in liquidaciones)
            {
                result.liquidaciones.Add(new Liquidacion()
                {
                    unidad = hijo.UN_CANTIDAD,
                    cantidad = hijo.CANTIDAD,
                    fecha = SAPFormatter.FormatearFecha(hijo.FECHA),
                    pedido = hijo.PEDIDO,
                    monedaPrecio = hijo.MON_PRECIO,
                    precio = hijo.PRECIO,
                    tipo = hijo.TIPO,
                    monedaTotal = hijo.MON_TOTAL,
                    total = hijo.TOTAL,
                    comprobante = hijo.COMPROBANTE
                });
            }

            foreach (ContratoDetalleWebServiceMOA.ZMPES4360 pago in pagos)
            {
                result.pagos.Add(new Pago()
                {
                    moneda = pago.MONEDA,
                    bruto = pago.BRUTO,
                    comprobante = pago.COMPROBANTE,
                    fecha = SAPFormatter.FormatearFecha(pago.FECHA),
                    idPago = pago.ID_PAGO,
                    iva = pago.IVA,
                    neto = pago.NETO,
                    retenciones = pago.RETENCIONES
                });
            }

            foreach (ContratoDetalleWebServiceMOA.ZMPES4520 resumen in resumenes)
            {
                result.resumen.Add(new Resumen()
                {
                    cantEntre = resumen.CANT_ENTRE,
                    unidadCantEntre = resumen.UNI_ENTRE,
                    cantFija = resumen.CANT_FIJA,
                    unidadCantFija = resumen.UNI_FIJA,
                    cantLiqui = resumen.CANT_LIQUI,
                    unidadCantLiqui = resumen.UNI_LIQUI,
                    cantPendEntre = resumen.CANT_PEND_ENTRE,
                    unidadCantPendEntre = resumen.UNI_PEND_ENTRE,
                    precio = resumen.PRECIO,
                    moneda = resumen.MONEDA,
                    contraMadre = resumen.CONTRA_MADRE,
                    contrato = resumen.CONTRATO,
                    estado = resumen.ESTADO,
                    producto = resumen.PRODUCTO
                });
            }

            return result;
        }
        protected override object MapSinPI(string contrato, string error, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES5260 boleto, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4390[] ampliaciones_anulaciones, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4400[] aplicaciones, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4410[] calidades, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6200[] caracteristicas, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4710[] condiciones_pago, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4380[] fijaciones, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4420[] hijos, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6210[] liquidaciones, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4360[] pagos, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4520[] resumenes)
        {
            ContratoDetalleExcelWSMOAResponse result = new ContratoDetalleExcelWSMOAResponse();

            result.error = error;
            result.aplicacionesTotalAplicados = 0;
            result.aplicacionesTotalBrutos = 0;
            result.calidadTotalAplicados = 0;
            result.calidadTotalNetos = 0;
            result.contrato = contrato;

            if (boleto != null)
            {
                result.boleto = new Boleto()
                {
                    bolsa = boleto.BOLSA,
                    devAcop = SAPFormatter.FormatearFecha(boleto.DEV_ACOP),
                    envioAfip = SAPFormatter.FormatearFecha(boleto.ENVIO_AFIP),
                    envioBolsa = SAPFormatter.FormatearFecha(boleto.ENVIO_BOLSA),
                    envioSellado = SAPFormatter.FormatearFecha(boleto.ENVIO_SELLADO),
                    estado = boleto.ESTADO,
                    fechaRecepcion = SAPFormatter.FormatearFecha(boleto.FECHA_RECEPCION),
                    obleaAfip = boleto.OBLEA_AFIP,
                    obleaBolsa = boleto.OBLEA_BOLSA,
                    provPlanCanje = boleto.PROV_PLAN_CANJE,
                    tipoBoleto = boleto.TIPO_BOLETO,
                    vueltaAfip = SAPFormatter.FormatearFecha(boleto.VUELTA_AFIP),
                    vueltaBolsa = SAPFormatter.FormatearFecha(boleto.VUELTA_BOLSA),
                    vueltaSellado = SAPFormatter.FormatearFecha(boleto.VUELTA_SELLADO),
                    observacion = boleto.OBSERVACIONES
                };

            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4390 ampAnul in ampliaciones_anulaciones)
            {
                result.ampliacionesAnulaciones.Add(new AmpliacionAnulacion()
                {
                    unidad = ampAnul.UNIDAD,
                    cantidad = ampAnul.CANTIDAD,
                    fecha = SAPFormatter.FormatearFecha(ampAnul.FECHA),
                    moneda = ampAnul.MON_IMPORTE,
                    importe = ampAnul.IMPORTE,
                    tipo = ampAnul.TIPO
                });
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4400 aplicacion in aplicaciones)
            {
                result.aplicaciones.Add(new Aplicacion()
                {
                    unidadCantidad = aplicacion.UNIME,
                    cantidad = aplicacion.CANTIDAD,
                    ccpp = aplicacion.CCPP,
                    descarga = aplicacion.DESCARGA,
                    fecha = SAPFormatter.FormatearFecha(aplicacion.FECHA),
                    unidadBrutos = aplicacion.UNIDAD_BRUTOS,
                    kgBrutos = aplicacion.KG_BRUTOS,
                    unidadNetos = aplicacion.UNIDAD_NETOS,
                    kgNetos = aplicacion.KG_NETOS
                });

                result.aplicacionesTotalAplicados += aplicacion.KG_NETOS;
                result.aplicacionesTotalBrutos += aplicacion.KG_BRUTOS;
                result.aplicacionesTotalAplicadosUnidad = aplicacion.UNIDAD_NETOS;
                result.aplicacionesTotalBrutosUnidad = aplicacion.UNIDAD_BRUTOS;

            }

            result.aplicacionesTotalAplicadosString = SAPFormatter.FormatearCantidad(result.aplicacionesTotalAplicados, result.aplicacionesTotalAplicadosUnidad);
            result.aplicacionesTotalBrutosString = SAPFormatter.FormatearCantidad(result.aplicacionesTotalBrutos, result.aplicacionesTotalBrutosUnidad);


            result.calidad = calidades.GroupBy(x => x.CCPP)
                .Select(x =>
                {
                    var calidad = new Calidad
                    {
                        ccpp = x.Key,
                        kgAplicadosTotal = SAPFormatter.FormatearCantidad(x.Sum(r => r.KG_APLI), "KG"),
                        kgNetosTotal = SAPFormatter.FormatearCantidad(x.Sum(r => r.KG_NETOS), "KG"),
                        kgDtoTotal = SAPFormatter.FormatearCantidad(x.Sum(r => r.KG_DTO), "KG"),
                        dtoPorcTotal = SAPFormatter.FormatearCantidad(x.Sum(r => r.DTO), "%"),
                        camaraAPresent = x.First().CAMARA_A_PRESENT,
                        registros = x.Select(e => new CalidadElement()
                        {
                            calaResul = e.CALA_RESUL,
                            camaResul = e.CARACT.ToUpper().Contains("HUMEDAD") ? e.CALA_RESUL : e.CAMA_RESUL,
                            caract = e.CARACT,
                            dto = e.DTO,
                            kgDto = SAPFormatter.FormatearCantidad(e.KG_DTO, e.UNIDAD),
                            kgDtoValor = e.KG_DTO,
                            certificado = e.NRO_CERT,
                            //Información para excels
                            recResul = e.REC_RESUL,
                            recCert = e.REC_CERT,
                            kgApli = e.KG_APLI,
                            kgNetos = e.KG_NETOS,
                            unidad = e.UNIDAD,
                        }).ToList()
                    };
                    calidad.SetearEstadoCamara();
                    return calidad;
                }).ToList();

            result.calidadExcelDetalle = calidades.Select(x => new CalidadExcelDetalle
            {
                ccpp = x.CCPP,
                calaResul = x.CALA_RESUL,
                camaResul = x.CAMA_RESUL,
                caract = x.CARACT,
                dto = x.DTO,
                kgApli = x.KG_APLI,
                kgDto = x.KG_DTO,
                kgNetos = x.KG_NETOS,
                nroCert = x.NRO_CERT,
                recCert = x.REC_CERT,
                recResul = x.REC_RESUL,
                unidad = x.UNIDAD

            }).ToList();

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4410 calidad in calidades)
            {
                result.calidadTotalAplicados += calidad.KG_APLI;
                result.calidadTotalNetos += calidad.KG_NETOS;
                result.calidadTotalAplicadosUnidad = calidad.UNIDAD;
                result.calidadTotalNetosUnidad = calidad.UNIDAD;
            }

            result.calidadTotalAplicadosString = SAPFormatter.FormatearCantidad(result.calidadTotalAplicados, result.calidadTotalAplicadosUnidad);
            result.calidadTotalNetosString = SAPFormatter.FormatearCantidad(result.calidadTotalNetos, result.calidadTotalNetosUnidad);

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6200 caracteristica in caracteristicas)
            {
                result.caracteristicas.Add(new Caracteristica()
                {
                    calificacion = caracteristica.CALIFICACION,
                    canje = caracteristica.CANJE,
                    cantidad = caracteristica.CANTIDAD,
                    unidad = caracteristica.UNIDAD,
                    cdCdg = caracteristica.CD_CDG,
                    cesion = caracteristica.CESION,
                    condPagoFija = caracteristica.COND_PAGO_FIJA,
                    corredor = caracteristica.CORREDOR,
                    cosecha = caracteristica.COSECHA,
                    descarga = caracteristica.DESCARGA,
                    descuentoAcarreo = caracteristica.DESCUENTO_ACARREO,
                    entregaMax = SAPFormatter.FormatearFecha(caracteristica.ENTREGA_MAX),
                    entregaMin = SAPFormatter.FormatearFecha(caracteristica.ENTREGA_MIN),
                    estadoBol = caracteristica.ESTADO_BOL,
                    fechaConcerta = SAPFormatter.FormatearFecha(caracteristica.FECHA_CONCERTA),
                    fechaTopeFija = SAPFormatter.FormatearFecha(caracteristica.FECHA_TOPE_FIJA),
                    fijaDiariaMax = caracteristica.FIJA_DIARIA_MAX,
                    fijaDiariaMin = caracteristica.FIJA_DIARIA_MIN,
                    importeAPrecio = caracteristica.IMPORTE_A_PRECIO,
                    importeSPrecio = caracteristica.IMPORTE_S_PRECIO,
                    monedaAPrecio = caracteristica.MONEDA_A_PRECIO,
                    monedaSPrecio = caracteristica.MONEDA_S_PRECIO,
                    nomCorredor = caracteristica.NOM_CORREDOR,
                    nomVendedor = caracteristica.NOM_VENDEDOR,
                    pagoDirVend = caracteristica.PAGO_DIR_VEND,
                    pagoParcial = caracteristica.PAGO_PARCIAL,
                    pizarraRef = caracteristica.PIZARRA_REF,
                    porcAPrecio = caracteristica.PORC_A_PRECIO,
                    porcSPrecio = caracteristica.PORC_S_PRECIO,
                    procedencia = caracteristica.PROCEDENCIA,
                    retenerIva = caracteristica.RETENER_IVA,
                    standardCali = caracteristica.STANDARD_CALI,
                    tipo = caracteristica.TIPO,
                    toleMax = caracteristica.TOLE_MAX,
                    toleMin = caracteristica.TOLE_MIN,
                    vendedor = caracteristica.VENDEDOR,
                    warrant = caracteristica.WARRANT,
                    confirma = caracteristica.CONFIRMA
                });
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4710 condicionPago in condiciones_pago)
            {
                result.condicionesPago.Add(condicionPago.MENSAJE);
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4380 fijacion in fijaciones)
            {
                result.fijaciones.Add(new Fijacion()
                {
                    fecha = SAPFormatter.FormatearFecha(fijacion.FECHA),
                    unidad = fijacion.UNIDAD,
                    kilosFija = fijacion.KILOS_FIJA,
                    nroFija = fijacion.NRO_FIJA,
                    moneda = fijacion.MON_PRECIO,
                    precio = fijacion.PRECIO
                });
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4420 hijo in hijos)
            {
                result.hijos.Add(new Hijo()
                {
                    unidad = hijo.UNIDAD,
                    cantidad = hijo.CANTIDAD,
                    contrMadre = hijo.CONTR_MADRE,
                    contrMolinos = hijo.CONTR_MOLINOS,
                    contrProve = hijo.CONTR_PROVE,
                    fecha = SAPFormatter.FormatearFecha(hijo.FECHA),
                    precio = hijo.PRECIO,
                    moneda = hijo.MON_PRECIO
                });
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6210 hijo in liquidaciones)
            {
                result.liquidaciones.Add(new Liquidacion()
                {
                    unidad = hijo.UN_CANTIDAD,
                    cantidad = hijo.CANTIDAD,
                    fecha = SAPFormatter.FormatearFecha(hijo.FECHA),
                    pedido = hijo.PEDIDO,
                    monedaPrecio = hijo.MON_PRECIO,
                    precio = hijo.PRECIO,
                    tipo = hijo.TIPO,
                    monedaTotal = hijo.MON_TOTAL,
                    total = hijo.TOTAL,
                    comprobante = hijo.COMPROBANTE
                });
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4360 pago in pagos)
            {
                result.pagos.Add(new Pago()
                {
                    moneda = pago.MONEDA,
                    bruto = pago.BRUTO,
                    comprobante = pago.COMPROBANTE,
                    fecha = SAPFormatter.FormatearFecha(pago.FECHA),
                    idPago = pago.ID_PAGO,
                    iva = pago.IVA,
                    neto = pago.NETO,
                    retenciones = pago.RETENCIONES
                });
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4520 resumen in resumenes)
            {
                result.resumen.Add(new Resumen()
                {
                    cantEntre = resumen.CANT_ENTRE,
                    unidadCantEntre = resumen.UNI_ENTRE,
                    cantFija = resumen.CANT_FIJA,
                    unidadCantFija = resumen.UNI_FIJA,
                    cantLiqui = resumen.CANT_LIQUI,
                    unidadCantLiqui = resumen.UNI_LIQUI,
                    cantPendEntre = resumen.CANT_PEND_ENTRE,
                    unidadCantPendEntre = resumen.UNI_PEND_ENTRE,
                    precio = resumen.PRECIO,
                    moneda = resumen.MONEDA,
                    contraMadre = resumen.CONTRA_MADRE,
                    contrato = resumen.CONTRATO,
                    estado = resumen.ESTADO,
                    producto = resumen.PRODUCTO
                });
            }

            return result;
        }

    }

    public class ContratoDetallePDFConsumerMOA : ContratoDetalleConsumerMOA
    {
        protected override object Map(string contrato, string error, ContratoDetalleWebServiceMOA.ZMPES5260 boleto, ContratoDetalleWebServiceMOA.ZMPES4390[] ampliaciones_anulaciones, ContratoDetalleWebServiceMOA.ZMPES4400[] aplicaciones, ContratoDetalleWebServiceMOA.ZMPES4410[] calidades, ContratoDetalleWebServiceMOA.ZMPES6200[] caracteristicas, ContratoDetalleWebServiceMOA.ZMPES4710[] condiciones_pago, ContratoDetalleWebServiceMOA.ZMPES4380[] fijaciones, ContratoDetalleWebServiceMOA.ZMPES4420[] hijos, ContratoDetalleWebServiceMOA.ZMPES6210[] liquidaciones, ContratoDetalleWebServiceMOA.ZMPES4360[] pagos, ContratoDetalleWebServiceMOA.ZMPES4520[] resumenes)
        {
            ContratoDetallePDFWSMOAResponse result = new ContratoDetallePDFWSMOAResponse();

            foreach (ContratoDetalleWebServiceMOA.ZMPES4410 calidad in calidades)
            {
                result.calidad.Add(new CalidadPDFDetalle()
                {
                    ccpp = calidad.CCPP,
                    calaResul = SAPFormatter.FormatearCantidad(calidad.CALA_RESUL, "%"),
                    camaResul = SAPFormatter.FormatearCantidad(calidad.CAMA_RESUL, "%"),
                    caract = calidad.CARACT,
                    dto = SAPFormatter.FormatearCantidad(calidad.DTO, "%"),
                    kgApli = SAPFormatter.FormatearCantidad(calidad.KG_APLI, "KG"),
                    kgDto = SAPFormatter.FormatearCantidad(calidad.KG_DTO, "KG"),
                    kgNetos = SAPFormatter.FormatearCantidad(calidad.KG_NETOS, "KG"),
                    unidad = calidad.UNIDAD
                });
            }

            return result;
        }
        protected override object MapSinPI(string contrato, string error, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES5260 boleto, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4390[] ampliaciones_anulaciones, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4400[] aplicaciones, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4410[] calidades, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6200[] caracteristicas, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4710[] condiciones_pago, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4380[] fijaciones, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4420[] hijos, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6210[] liquidaciones, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4360[] pagos, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4520[] resumenes)
        {
            ContratoDetallePDFWSMOAResponse result = new ContratoDetallePDFWSMOAResponse();

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4410 calidad in calidades)
            {
                result.calidad.Add(new CalidadPDFDetalle()
                {
                    ccpp = calidad.CCPP,
                    calaResul = SAPFormatter.FormatearCantidad(calidad.CALA_RESUL, "%"),
                    camaResul = SAPFormatter.FormatearCantidad(calidad.CAMA_RESUL, "%"),
                    caract = calidad.CARACT,
                    dto = SAPFormatter.FormatearCantidad(calidad.DTO, "%"),
                    kgApli = SAPFormatter.FormatearCantidad(calidad.KG_APLI, "KG"),
                    kgDto = SAPFormatter.FormatearCantidad(calidad.KG_DTO, "KG"),
                    kgNetos = SAPFormatter.FormatearCantidad(calidad.KG_NETOS, "KG"),
                    unidad = calidad.UNIDAD
                });
            }

            return result;
        }

    }
}
