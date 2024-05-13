using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.Liquidacion;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.LiquidacionesWebServiceMOA;

namespace SustitucionMOAWS.WSConsumers
{
    public class LiquidacionesConsumerMOA
    {
        SI_MPMF_MOAOP_LIQUIDACIONESClient service = new SI_MPMF_MOAOP_LIQUIDACIONESClient();

        public object request(string proveedor, List<FechaWS> fechas, string contrato, string liquidacion)
        {
            try
            {
                ZMPES4980[] salidas = new ZMPES4980[] { };
                List<ZMPES4100> fechasSAP = new List<ZMPES4100>() { };
                foreach (FechaWS fecha in fechas)
                {
                    fechasSAP.Add(new ZMPES4100()
                    {
                        FECHA_OP = SAPFormatter.PrepararFecha(fecha.fechaInicio),
                        FECHA_OP_HASTA = SAPFormatter.PrepararFecha(fecha.fechaFin)
                    });
                }
                ZMPES4100[] fechasSAPArray = fechasSAP.ToArray();
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                string error = service.SI_MPMF_MOAOP_LIQUIDACIONES(liquidacion,contrato,proveedor, ref fechasSAPArray, ref salidas);
                return map(error, salidas);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        protected virtual object map(string error, ZMPES4980[] salidas)
        {
            LiquidacionWSMOAResponse result = new LiquidacionWSMOAResponse();
            
            result.error = error;

            foreach (ZMPES4980 liquidacion in salidas)
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
        protected override object map(string error, ZMPES4980[] salidas)
        {
            LiquidacionExcelWSMOAResponse result = new LiquidacionExcelWSMOAResponse();

            result.error = error;

            foreach (ZMPES4980 liquidacion in salidas)
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
