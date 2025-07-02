using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.Liquidacion;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.LiquidacionesWebServiceMOA;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MOAOP;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WSConsumers
{
    public class LiquidacionesConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserS4"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassS4"];

        public object request(string proveedor, List<FechaWS> fechas, string contrato, string liquidacion)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4980[] salidas = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4980[] { };
                    List<WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100> fechasSAP = new List<WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100>() { };
                    foreach (FechaWS fecha in fechas)
                    {
                        fechasSAP.Add(new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100()
                        {
                            FECHA_OP = SAPFormatter.PrepararFecha(fecha.fechaInicio),
                            FECHA_OP_HASTA = SAPFormatter.PrepararFecha(fecha.fechaFin)
                        });
                    }
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100[] fechasSAPArray = fechasSAP.ToArray();

                    var request = new Z_MPMF_MOAOP_LIQUIDACIONES()
                    {
                        IM_COE = liquidacion,
                        IM_CONTRATO = contrato,
                        PE_PROVEEDOR = proveedor,
                        T_FECHA_IN = fechasSAPArray,
                        T_SALIDA = salidas
                    };
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_LIQUIDACIONES request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MPMF_MOAOP_LIQUIDACIONES(request);
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_LIQUIDACIONES response");
                    Log.Info(response.ToXml());
                    return MapSinPI(response.PS_RETURN, response.T_SALIDA);
                }
                else
                {
                    SI_MPMF_MOAOP_LIQUIDACIONESClient service = new SI_MPMF_MOAOP_LIQUIDACIONESClient();
                    LiquidacionesWebServiceMOA.ZMPES4980[] salidas = new LiquidacionesWebServiceMOA.ZMPES4980[] { };
                    List<LiquidacionesWebServiceMOA.ZMPES4100> fechasSAP = new List<LiquidacionesWebServiceMOA.ZMPES4100>() { };
                    foreach (FechaWS fecha in fechas)
                    {
                        fechasSAP.Add(new LiquidacionesWebServiceMOA.ZMPES4100()
                        {
                            FECHA_OP = SAPFormatter.PrepararFecha(fecha.fechaInicio),
                            FECHA_OP_HASTA = SAPFormatter.PrepararFecha(fecha.fechaFin)
                        });
                    }
                    LiquidacionesWebServiceMOA.ZMPES4100[] fechasSAPArray = fechasSAP.ToArray();
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    string error = service.SI_MPMF_MOAOP_LIQUIDACIONES(liquidacion, contrato, proveedor, ref fechasSAPArray, ref salidas);
                    return Map(error, salidas);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        protected virtual object Map(string error, LiquidacionesWebServiceMOA.ZMPES4980[] salidas)
        {
            LiquidacionWSMOAResponse result = new LiquidacionWSMOAResponse();
            
            result.error = error;

            foreach (LiquidacionesWebServiceMOA.ZMPES4980 liquidacion in salidas)
            {
                result.liquidaciones.Add(new LiquidacionView()
                {
                    comprobante = liquidacion.COMPROBANTE,
                    contrato = liquidacion.CONTRATO,
                    detallePago = liquidacion.ID_PAGO,
                    emitido = SAPFormatter.FormatearFecha(liquidacion.EMITIDO),
                    emitidoDate = SAPFormatter.GetDateTime(liquidacion.EMITIDO),
                    pago = SAPFormatter.FormatearFecha(liquidacion.FACREDITACION),
                    pagoDate = SAPFormatter.GetDateTime(liquidacion.FACREDITACION),
                    importeString = SAPFormatter.FormatearMonto(liquidacion.IMPORTE, liquidacion.MONEDA),
                    importe = liquidacion.IMPORTE,
                    ivaString = SAPFormatter.FormatearMonto(liquidacion.IVA, liquidacion.MONEDA),
                    iva = liquidacion.IVA,
                    liquidadoString = SAPFormatter.FormatearCantidad(liquidacion.LIQUIDADO, liquidacion.UNIME),
                    liquidado = liquidacion.LIQUIDADO,
                    observaciones = liquidacion.OBSERVACIONES,
                    producto = liquidacion.PRODUCTO,
                    tipo = liquidacion.TIPO,
                    secuencia = liquidacion.SECUENCIA,
                    solapa = liquidacion.SOLAPA,
                    documento = liquidacion.DOCUMENTO,
                    sociedad = liquidacion.SOCIEDAD,
                    ejercicio = liquidacion.EJERCICIO,
                    fijacion = liquidacion.FIJACION,
                    Total = liquidacion.IMPORTE + liquidacion.IVA,
                    TotalStr = SAPFormatter.FormatearMonto(liquidacion.IMPORTE + liquidacion.IVA, liquidacion.MONEDA)
                }
                );
            }
            
            return result;
        }
        protected virtual object MapSinPI(string error, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4980[] salidas)
        {
            LiquidacionWSMOAResponse result = new LiquidacionWSMOAResponse();

            result.error = error;

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4980 liquidacion in salidas)
            {
                result.liquidaciones.Add(new LiquidacionView()
                {
                    comprobante = liquidacion.COMPROBANTE,
                    contrato = liquidacion.CONTRATO,
                    detallePago = liquidacion.ID_PAGO,
                    emitido = SAPFormatter.FormatearFecha(liquidacion.EMITIDO),
                    emitidoDate = SAPFormatter.GetDateTime(liquidacion.EMITIDO),
                    pago = SAPFormatter.FormatearFecha(liquidacion.FACREDITACION),
                    pagoDate = SAPFormatter.GetDateTime(liquidacion.FACREDITACION),
                    importeString = SAPFormatter.FormatearMonto(liquidacion.IMPORTE, liquidacion.MONEDA),
                    importe = liquidacion.IMPORTE,
                    ivaString = SAPFormatter.FormatearMonto(liquidacion.IVA, liquidacion.MONEDA),
                    iva = liquidacion.IVA,
                    liquidadoString = SAPFormatter.FormatearCantidad(liquidacion.LIQUIDADO, liquidacion.UNIME),
                    liquidado = liquidacion.LIQUIDADO,
                    observaciones = liquidacion.OBSERVACIONES,
                    producto = liquidacion.PRODUCTO,
                    tipo = liquidacion.TIPO,
                    secuencia = liquidacion.SECUENCIA,
                    solapa = liquidacion.SOLAPA,
                    documento = liquidacion.DOCUMENTO,
                    sociedad = liquidacion.SOCIEDAD,
                    ejercicio = liquidacion.EJERCICIO,
                    fijacion = liquidacion.FIJACION,
                    Total = liquidacion.IMPORTE + liquidacion.IVA,
                    TotalStr = SAPFormatter.FormatearMonto(liquidacion.IMPORTE + liquidacion.IVA, liquidacion.MONEDA)
                }
                );
            }

            return result;
        }

    }

    public class LiquidacionesExcelConsumerMOA : LiquidacionesConsumerMOA
    {
        protected override object Map(string error, LiquidacionesWebServiceMOA.ZMPES4980[] salidas)
        {
            LiquidacionExcelWSMOAResponse result = new LiquidacionExcelWSMOAResponse();

            result.error = error;

            foreach (LiquidacionesWebServiceMOA.ZMPES4980 liquidacion in salidas)
            {
                result.liquidaciones.Add(new Liquidacion()
                {
                    comprobante = liquidacion.COMPROBANTE,
                    contrato = liquidacion.CONTRATO,
                    emitido = SAPFormatter.FormatearFecha(liquidacion.EMITIDO),
                    moneda = liquidacion.MONEDA,
                    importe = liquidacion.IMPORTE,
                    iva = liquidacion.IVA,
                    unidadLiquidado = liquidacion.UNIME,
                    liquidado = liquidacion.LIQUIDADO,
                    observaciones = liquidacion.OBSERVACIONES,
                    producto = liquidacion.PRODUCTO,
                    tipo = liquidacion.TIPO,
                    secuencia = liquidacion.SECUENCIA,
                    solapa = liquidacion.SOLAPA,
                    fijacion = liquidacion.FIJACION
                }
                );
            }

            return result;
        }
        protected override object MapSinPI(string error, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4980[] salidas)
        {
            LiquidacionExcelWSMOAResponse result = new LiquidacionExcelWSMOAResponse();

            result.error = error;

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4980 liquidacion in salidas)
            {
                result.liquidaciones.Add(new Liquidacion()
                {
                    comprobante = liquidacion.COMPROBANTE,
                    contrato = liquidacion.CONTRATO,
                    emitido = SAPFormatter.FormatearFecha(liquidacion.EMITIDO),
                    moneda = liquidacion.MONEDA,
                    importe = liquidacion.IMPORTE,
                    iva = liquidacion.IVA,
                    unidadLiquidado = liquidacion.UNIME,
                    liquidado = liquidacion.LIQUIDADO,
                    observaciones = liquidacion.OBSERVACIONES,
                    producto = liquidacion.PRODUCTO,
                    tipo = liquidacion.TIPO,
                    secuencia = liquidacion.SECUENCIA,
                    solapa = liquidacion.SOLAPA,
                    fijacion = liquidacion.FIJACION
                }
                );
            }

            return result;
        }
    }
}
