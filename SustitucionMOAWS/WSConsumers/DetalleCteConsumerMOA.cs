using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models.WSMapMOA.Proforma;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.DetalleCteWebServiceMOA;
using SustitucionMOAWS.Logger;

namespace SustitucionMOAWS.WSConsumers
{
    public abstract class DetalleCteConsumerMOABase<T>
    {
        protected const string conceptoPagoACuenta = "Pago a Cuenta";
        protected const string conceptoFaltaLiquidacionParcial = "Falta registrar la Liquidación parcial";
        private readonly SI_MPMF_MOAOP_DETALLE_CTEClient service = new SI_MPMF_MOAOP_DETALLE_CTEClient();

        public T Request(string contrato, string proveedor)
        {
            try
            {
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                var cabecera = new ZMPES5450() { };
                var salidas = new ZMPES4480[] { };
                
                var idVendedor = service.SI_MPMF_MOAOP_DETALLE_CTE(contrato, proveedor, ref salidas, out cabecera,
                    out string vendedor, out string returnString);
                
                return Map(contrato, cabecera, salidas, vendedor, returnString, idVendedor);
            }
            catch (Exception e)
            {
                Log.Error(e, $"Error en llamada SAP SI_MPMF_MOAOP_DETALLE_CTE, contrato {contrato} y proveedor {proveedor}.");
                throw e;
            }
        }

        protected abstract T Map(string contrato, ZMPES5450 cabecera, ZMPES4480[] salidas, string vendedores,
            string returnString, string idVendedor);
    }

    public class DetalleCteConsumerMOA : DetalleCteConsumerMOABase<DetalleCteWSMOAResponse>
    {
        protected override DetalleCteWSMOAResponse Map(string contrato, ZMPES5450 cabecera, ZMPES4480[] salidas, string vendedores, string returnString, string idVendedor)
        {
            var result = new DetalleCteWSMOAResponse
            {
                fijacion = contrato,
                vendedores = vendedores,
                error = returnString,
                LiquidacionParcialEmitida = true
            };

            var moneda = "";

            if (cabecera != null)
            {
                result.cabecera = new CabeceraView
                {
                    fecha = SAPFormatter.FormatearFecha(cabecera.FECHA_LIQ),
                    compradosString = SAPFormatter.FormatearCantidad(cabecera.KILOS_COMPRADOS, "KG"),
                    recibidosString = SAPFormatter.FormatearCantidad(cabecera.KILOS_RECIBIDOS, "KG"),
                    precioString = SAPFormatter.FormatearMonto(cabecera.PRECIO, cabecera.MONEDA),
                    precioPactadoString = SAPFormatter.FormatearMonto(cabecera.PRECIO_PACTADO, cabecera.MONEDA),
                    moneda = cabecera.MONEDA,
                    precioNetoString = SAPFormatter.FormatearMonto(cabecera.PRECIO_NETO, cabecera.MONEDA),
                    tarifaFleteString = SAPFormatter.FormatearMonto(cabecera.TARIFA_FLETE, cabecera.MONEDA),
                    PorcentajePagoParcial = cabecera.PORCPARCIAL
                };
            }

            foreach (ZMPES4480 salida in salidas)
            {
                moneda = salida.MONEDA;
                switch (salida.CARACT)
                {
                    case conceptoPagoACuenta:
                        result.pagoACuenta = new SalidaView()
                        {
                            caracteristica = salida.CARACT,
                            contrato = salida.CONTRATO,
                            importeString = SAPFormatter.FormatearMonto(salida.IMPORTE, salida.MONEDA),
                            ivaString = SAPFormatter.FormatearMonto(salida.IVA, salida.MONEDA),
                            totalString = SAPFormatter.FormatearMonto(salida.TOTAL, salida.MONEDA),
                            importe = salida.IMPORTE,
                            iva = salida.IVA,
                            total = salida.TOTAL
                        };
                        result.saldoAPagar.importe += salida.IMPORTE;
                        result.saldoAPagar.iva += salida.IVA;
                        result.saldoAPagar.total += salida.TOTAL;
                        break;

                    case conceptoFaltaLiquidacionParcial:
                        // El orden de una proforma es:
                        // 1) Proforma parcial -> Se liquida (por lo que pasa a ser Proforma final)
                        // 2) Proforma final -> Se liquida
                        result.LiquidacionParcialEmitida = false;
                        break;

                    default:
                        result.salidas.Add(new SalidaView
                        {
                            caracteristica = salida.CARACT,
                            contrato = salida.CONTRATO,
                            importeString = SAPFormatter.FormatearMonto(salida.IMPORTE, salida.MONEDA),
                            ivaString = SAPFormatter.FormatearMonto(salida.IVA, salida.MONEDA),
                            totalString = SAPFormatter.FormatearMonto(salida.TOTAL, salida.MONEDA)
                        });

                        result.subTotal.importe += salida.IMPORTE;
                        result.subTotal.iva += salida.IVA;
                        result.subTotal.total += salida.TOTAL;

                        result.saldoAPagar.importe += salida.IMPORTE;
                        result.saldoAPagar.iva += salida.IVA;
                        result.saldoAPagar.total += salida.TOTAL;
                        break;
                }
            }

            result.subTotal.importeString = SAPFormatter.FormatearMonto(result.subTotal.importe, moneda);
            result.subTotal.ivaString = SAPFormatter.FormatearMonto(result.subTotal.iva, moneda);
            result.subTotal.totalString = SAPFormatter.FormatearMonto(result.subTotal.total, moneda);

            result.saldoAPagar.importeString = SAPFormatter.FormatearMonto(result.saldoAPagar.importe, moneda);
            result.saldoAPagar.ivaString = SAPFormatter.FormatearMonto(result.saldoAPagar.iva, moneda);
            result.saldoAPagar.totalString = SAPFormatter.FormatearMonto(result.saldoAPagar.total, moneda);

            return result;
        }
    }

    public class DetalleCteExcelConsumerMOA : DetalleCteConsumerMOABase<DetalleCteExcelWSMOAResponse>
    {
        protected override DetalleCteExcelWSMOAResponse Map(string contrato, ZMPES5450 cabecera, ZMPES4480[] salidas, string vendedores, string returnString, string idVendedor)
        {
            var result = new DetalleCteExcelWSMOAResponse
            {
                fijacion = contrato,
                vendedores = vendedores,
                error = returnString
            };

            var moneda = "";

            if (cabecera != null)
            {
                result.cabecera = new Cabecera()
                {
                    fecha = SAPFormatter.FormatearFecha(cabecera.FECHA_LIQ),
                    comprados = cabecera.KILOS_COMPRADOS,
                    recibidos = cabecera.KILOS_RECIBIDOS, 
                    precio = cabecera.PRECIO,
                    precioPactado = cabecera.PRECIO_PACTADO,
                    moneda = cabecera.MONEDA,
                    precioNeto = cabecera.PRECIO_NETO,
                    tarifaFlete = cabecera.TARIFA_FLETE
                };
            }

            foreach (ZMPES4480 salida in salidas)
            {
                moneda = salida.MONEDA;
                if (salida.CARACT.Equals("Pago a Cuenta"))
                {
                    result.pagoACuenta = new SalidaView()
                    {
                        caracteristica = salida.CARACT,
                        contrato = salida.CONTRATO,
                        importeString = SAPFormatter.FormatearMonto(salida.IMPORTE, salida.MONEDA),
                        ivaString = SAPFormatter.FormatearMonto(salida.IVA, salida.MONEDA),
                        totalString = SAPFormatter.FormatearMonto(salida.TOTAL, salida.MONEDA),
                        importe = salida.IMPORTE,
                        iva = salida.IVA,
                        total = salida.TOTAL
                    };

                    result.saldoAPagar.importe += salida.IMPORTE;
                    result.saldoAPagar.iva += salida.IVA;
                    result.saldoAPagar.total += salida.TOTAL;
                }
                else
                {
                    result.salidas.Add(new Salida()
                    {
                        caracteristica = salida.CARACT,
                        contrato = salida.CONTRATO,
                        moneda = salida.MONEDA,
                        importe = salida.IMPORTE,
                        iva = salida.IVA,
                        total = salida.TOTAL
                    });

                    result.subTotal.importe += salida.IMPORTE;
                    result.subTotal.iva += salida.IVA;
                    result.subTotal.total += salida.TOTAL;

                    result.saldoAPagar.importe += salida.IMPORTE;
                    result.saldoAPagar.iva += salida.IVA;
                    result.saldoAPagar.total += salida.TOTAL;
                }
            }

            result.saldoAPagar.moneda = moneda;
            result.subTotal.moneda = moneda;

            return result;
        }
    }
}
