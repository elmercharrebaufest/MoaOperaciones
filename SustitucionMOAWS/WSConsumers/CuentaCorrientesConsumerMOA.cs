using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.CuentaCorriente;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.CuentaCorrienteWebServiceMOA;

namespace SustitucionMOAWS.WSConsumers
{
    public class CuentaCorrientesConsumerMOA
    {
        SI_MPMF_MOAOP_CUENTA_CORRIENTEClient service = new SI_MPMF_MOAOP_CUENTA_CORRIENTEClient();

        public object request(string compensa, string proveedor, string sociedad, FechaWS fecha, string contrato, string pago, string retencion)
        {
            try
            {
                ZMPES6120[] salidas = new ZMPES6120[] { };
                ZMPES4100 fechaSAP = new ZMPES4100()
                {
                    FECHA_OP = SAPFormatter.PrepararFecha(fecha.fechaInicio),
                    FECHA_OP_HASTA = SAPFormatter.PrepararFecha(fecha.fechaFin)
                };        
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                ZMPES4910 error = service.SI_MPMF_MOAOP_CUENTA_CORRIENTE(compensa, contrato, fechaSAP, pago, proveedor, retencion, sociedad, out salidas);
                return map(salidas, error, fecha.fechaFin);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        protected virtual object map(ZMPES6120[] salidas, ZMPES4910 error, DateTime fechaFin)
        {
            CuentaCorrienteWSMOAResponse result = new CuentaCorrienteWSMOAResponse();
            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }

            int i = 1;

            foreach (ZMPES6120 cuentaCorrienteInfo in salidas)
            {
                result.cuentasCorrientes.Add(new MovimientoView()
                {
                    agrupador = cuentaCorrienteInfo.AGRUPADOR,
                    augbl = cuentaCorrienteInfo.AUGBL,
                    contrato = cuentaCorrienteInfo.CONTRATO,
                    debe = cuentaCorrienteInfo.DEBE,
                    debeString = SAPFormatter.FormatearMonto(cuentaCorrienteInfo.DEBE, "$"),
                    descripcion = cuentaCorrienteInfo.DESCRIP,
                    docDate = SAPFormatter.FormatearFecha(cuentaCorrienteInfo.DOC_DATE),
                    docDateDate = SAPFormatter.GetDateTime(cuentaCorrienteInfo.DOC_DATE),
                    docNo = cuentaCorrienteInfo.DOC_NO,
                    fecVto = SAPFormatter.FormatearFecha(cuentaCorrienteInfo.FEC_VTO),
                    fecVtoDate = SAPFormatter.GetDateTime(cuentaCorrienteInfo.FEC_VTO),
                    fiscYear = cuentaCorrienteInfo.FISC_YEAR,
                    haber = cuentaCorrienteInfo.HABER,
                    haberString = SAPFormatter.FormatearMonto(cuentaCorrienteInfo.HABER, "$"),
                    importeArg = cuentaCorrienteInfo.IMPORTE_ARP,
                    importeArgString = SAPFormatter.FormatearMonto(cuentaCorrienteInfo.IMPORTE_ARP, "$"),
                    moneda = cuentaCorrienteInfo.MONEDA,
                    orden = i++,
                    ukurs = cuentaCorrienteInfo.UKURS,
                    ukursString = SAPFormatter.FormatearTipoCambio(cuentaCorrienteInfo.UKURS),
                    xblnr = cuentaCorrienteInfo.XBLNR
                });
            }

            return result;
        }

    }

    public class CuentaCorrientesExcelConsumerMOA : CuentaCorrientesConsumerMOA
    {
        protected override object map(ZMPES6120[] salidas, ZMPES4910 error, DateTime fechaFin)
        {
            CuentaCorrienteExcelWSMOAResponse result = new CuentaCorrienteExcelWSMOAResponse();
            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }

            foreach (ZMPES6120 cuentaCorrienteInfo in salidas)
            {
                result.cuentasCorrientes.Add(new Movimiento()
                {
                    agrupador = cuentaCorrienteInfo.AGRUPADOR,
                    augbl = cuentaCorrienteInfo.AUGBL,
                    contrato = cuentaCorrienteInfo.CONTRATO,
                    moneda = cuentaCorrienteInfo.MONEDA,
                    debe = cuentaCorrienteInfo.DEBE,
                    descripcion = cuentaCorrienteInfo.DESCRIP,
                    docDate = SAPFormatter.FormatearFecha(cuentaCorrienteInfo.DOC_DATE),
                    docNo = cuentaCorrienteInfo.DOC_NO,
                    fecVto = SAPFormatter.FormatearFecha(cuentaCorrienteInfo.FEC_VTO),
                    fiscYear = cuentaCorrienteInfo.FISC_YEAR,
                    haber = cuentaCorrienteInfo.HABER,
                    importeArg = cuentaCorrienteInfo.IMPORTE_ARP,
                    ukurs= cuentaCorrienteInfo.UKURS,
                    xblnr = cuentaCorrienteInfo.XBLNR
                });
            }

            return result;
        }

    }

    public class CuentaCorrientesAgrupadaConsumerMOA : CuentaCorrientesConsumerMOA
    {
        protected override object map(ZMPES6120[] salidas, ZMPES4910 error, DateTime fechaFin)
        {
            CuentaCorrienteAgrupadaWSMOAResponse result = new CuentaCorrienteAgrupadaWSMOAResponse();
            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }

            result.fechaSaldo = SAPFormatter.FormatearFecha(fechaFin);

            result.cuentasCorrientesSinAgrupar = salidas.Where(x => x.AGRUPADOR == "")
                .GroupBy(x => x.AGRUPADOR)
                .Select(x => new CuentaCorrienteAgrupadaView
                {
                    agrupador = x.Key,
                    cuentasCorrientes = x.Select( z => new CuentaCorrienteView
                    {
                        agrupador = z.AGRUPADOR,
                        augbl = z.AUGBL,
                        contrato = z.CONTRATO,
                        debe = z.DEBE,
                        debeString = SAPFormatter.FormatearMonto(z.DEBE, z.MONEDA),
                        descripcion = z.DESCRIP,
                        docDate = SAPFormatter.FormatearFecha(z.DOC_DATE),
                        docDateDate = SAPFormatter.GetDateTime(z.DOC_DATE),
                        docNo = z.DOC_NO,
                        fecVto = SAPFormatter.FormatearFecha(z.FEC_VTO),
                        fecVtoDate = SAPFormatter.GetDateTime(z.FEC_VTO),
                        fiscYear = z.FISC_YEAR,
                        haber = z.HABER,
                        haberString = SAPFormatter.FormatearMonto(z.HABER, z.MONEDA),
                        importeArg = z.IMPORTE_ARP,
                        importeArgString = SAPFormatter.FormatearMonto(z.IMPORTE_ARP, "$"),
                        moneda = z.MONEDA,
                        saldo = z.SALDO,
                        saldoString = SAPFormatter.FormatearMonto(z.SALDO, z.MONEDA),
                        ukurs = z.UKURS,
                        ukursString = SAPFormatter.FormatearTipoCambio(z.UKURS),
                        xblnr = z.XBLNR,

                    }).ToList(),
                    total = SAPFormatter.FormatearMonto(x.Sum(z => z.IMPORTE_ARP), "$")
                }).FirstOrDefault();

            result.total = result.cuentasCorrientesSinAgrupar != null ? result.cuentasCorrientesSinAgrupar.total : SAPFormatter.FormatearMonto(0, "$");

            result.cuentasCorrientesAgrupadas = salidas.Where(x => x.AGRUPADOR != "")
                .GroupBy(x => x.AGRUPADOR)
                .Select(x => new CuentaCorrienteAgrupadaView
                {
                    agrupador = x.Key,
                    cuentasCorrientes = x.Select(z => new CuentaCorrienteView()
                    {
                        agrupador = z.AGRUPADOR,
                        augbl = z.AUGBL,
                        contrato = z.CONTRATO,
                        debe = z.DEBE,
                        debeString = SAPFormatter.FormatearMonto(z.DEBE, z.MONEDA),
                        descripcion = z.DESCRIP,
                        docDate = SAPFormatter.FormatearFecha(z.DOC_DATE),
                        docDateDate = SAPFormatter.GetDateTime(z.DOC_DATE),
                        docNo = z.DOC_NO,
                        fecVto = SAPFormatter.FormatearFecha(z.FEC_VTO),
                        fecVtoDate = SAPFormatter.GetDateTime(z.FEC_VTO),
                        fiscYear = z.FISC_YEAR,
                        haber = z.HABER,
                        haberString = SAPFormatter.FormatearMonto(z.HABER, z.MONEDA),
                        importeArg = z.IMPORTE_ARP,
                        importeArgString = SAPFormatter.FormatearMonto(z.IMPORTE_ARP, "$"),
                        moneda = z.MONEDA,
                        saldo = z.SALDO,
                        saldoString = SAPFormatter.FormatearMonto(z.SALDO, z.MONEDA),
                        ukurs = z.UKURS,
                        ukursString = SAPFormatter.FormatearTipoCambio(z.UKURS),
                        xblnr = z.XBLNR,

                    }).ToList(),
                    total = SAPFormatter.FormatearMonto(x.Sum(z => z.IMPORTE_ARP), "$")
                }).ToList();

            return result;
        }
    }

    public class CuentaCorrientesAgrupadaExcelConsumerMOA : CuentaCorrientesConsumerMOA
    {
        protected override object map(ZMPES6120[] salidas, ZMPES4910 error, DateTime fechaFin)
        {
            CuentaCorrienteAgrupadaExcelWSMOAResponse result = new CuentaCorrienteAgrupadaExcelWSMOAResponse();
            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }

            result.fechaSaldo = SAPFormatter.FormatearFecha(fechaFin);

            result.cuentasCorrientesSinAgrupar = salidas.Where(x => x.AGRUPADOR == "")
                .GroupBy(x => x.AGRUPADOR)
                .Select(x => new CuentaCorrienteAgrupada
                {
                    agrupador = x.Key,
                    cuentasCorrientes = x.Select(z => new CuentaCorriente
                    {
                        agrupador = z.AGRUPADOR,
                        augbl = z.AUGBL,
                        contrato = z.CONTRATO,
                        debe = z.DEBE,
                        descripcion = z.DESCRIP,
                        docDate = SAPFormatter.FormatearFecha(z.DOC_DATE),
                        docNo = z.DOC_NO,
                        fecVto = SAPFormatter.FormatearFecha(z.FEC_VTO),
                        fiscYear = z.FISC_YEAR,
                        haber = z.HABER,
                        importeArg = z.IMPORTE_ARP,
                        moneda = z.MONEDA,
                        saldo = z.SALDO,
                        ukurs = z.UKURS,
                        xblnr = z.XBLNR,

                    }).ToList(),
                    total = SAPFormatter.FormatearMonto(x.Sum(z => z.IMPORTE_ARP), "$")
                }).FirstOrDefault();

            result.cuentasCorrientesAgrupadas = salidas.Where(x => x.AGRUPADOR != "")
                .GroupBy(x => x.AGRUPADOR)
                .Select(x => new CuentaCorrienteAgrupada
                {
                    agrupador = x.Key,
                    cuentasCorrientes = x.Select(z => new CuentaCorriente()
                    {
                        agrupador = z.AGRUPADOR,
                        augbl = z.AUGBL,
                        contrato = z.CONTRATO,
                        debe =z.DEBE,
                        descripcion = z.DESCRIP,
                        docDate = SAPFormatter.FormatearFecha(z.DOC_DATE),
                        docNo = z.DOC_NO,
                        fecVto = SAPFormatter.FormatearFecha(z.FEC_VTO),
                        fiscYear = z.FISC_YEAR,
                        haber = z.HABER,
                        importeArg = z.IMPORTE_ARP,
                        moneda = z.MONEDA,
                        saldo = z.SALDO, 
                        ukurs = z.UKURS,
                        xblnr = z.XBLNR,

                    }).ToList(),
                    total = SAPFormatter.FormatearMonto(x.Sum(z => z.IMPORTE_ARP), "$")
                }).ToList();

            return result;
        }
    }
}
