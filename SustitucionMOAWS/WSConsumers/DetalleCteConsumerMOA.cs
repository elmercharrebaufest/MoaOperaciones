using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models.WSMapMOA.Proforma;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.DetalleCteWebServiceMOA;

namespace SustitucionMOAWS.WSConsumers
{
    public class DetalleCteConsumerMOA
    {
        SI_MPMF_MOAOP_DETALLE_CTEClient service = new SI_MPMF_MOAOP_DETALLE_CTEClient();

        public object request(string contrato, string proveedor)
        {
            try
            {
                ZMPES5450 cabecera = new ZMPES5450() { };
                ZMPES4480[] salidas = new ZMPES4480[] { };
                string vendedor = "";
                string returnString = "";
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                string idVendedor = service.SI_MPMF_MOAOP_DETALLE_CTE(contrato, proveedor, ref salidas, out cabecera, out vendedor, out returnString);
                return map(contrato, cabecera, salidas, vendedor, returnString, idVendedor);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        protected virtual object map(string contrato, ZMPES5450 cabecera, ZMPES4480[] salidas, string vendedores, string returnString, string idVendedor)
        {
            DetalleCteWSMOAResponse result = new DetalleCteWSMOAResponse();

            string moneda = "";

            result.fijacion = contrato;
            result.vendedores = vendedores;
            result.error = returnString;

            if(cabecera != null)
            {
                result.cabecera = new CabeceraView()
                {
                    fecha = SAPFormatter.FormatearFecha(cabecera.FECHA_LIQ),
                    compradosString = SAPFormatter.FormatearCantidad(cabecera.KILOS_COMPRADOS, "KG"),
                    recibidosString = SAPFormatter.FormatearCantidad(cabecera.KILOS_RECIBIDOS, "KG"),
                    precioString = SAPFormatter.FormatearMonto(cabecera.PRECIO, cabecera.MONEDA),
                    precioPactadoString = SAPFormatter.FormatearMonto(cabecera.PRECIO_PACTADO, cabecera.MONEDA),
                    moneda = cabecera.MONEDA,
                    precioNetoString = SAPFormatter.FormatearMonto(cabecera.PRECIO_NETO, cabecera.MONEDA),
                    tarifaFleteString = SAPFormatter.FormatearMonto(cabecera.TARIFA_FLETE, cabecera.MONEDA)
                };
            }

            foreach (ZMPES4480 salida in salidas) {
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
                    result.salidas.Add(new SalidaView()
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

    public class DetalleCteExcelConsumerMOA : DetalleCteConsumerMOA
    {
        
        protected override object map(string contrato, ZMPES5450 cabecera, ZMPES4480[] salidas, string vendedores, string returnString, string idVendedor)
        {
            DetalleCteExcelWSMOAResponse result = new DetalleCteExcelWSMOAResponse();

            string moneda = "";

            result.fijacion = contrato;
            result.vendedores = vendedores;
            result.error = returnString;

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
                        total = salida.TOTAL,

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
