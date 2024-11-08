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
using SustitucionMOAWS.Logger;

namespace SustitucionMOAWS.WSConsumers
{
    public abstract class CuentaCorrientesConsumerMOABase<T>
    {
        private readonly SI_MPMF_MOAOP_CUENTA_CORRIENTEClient service = new SI_MPMF_MOAOP_CUENTA_CORRIENTEClient();

        public T Request(string compensa, string proveedor, string sociedad, FechaWS fecha, string contrato, string pago, string retencion)
        {
            try
            {
                var fechaSAP = new ZMPES4100
                {
                    FECHA_OP = SAPFormatter.PrepararFecha(fecha.fechaInicio),
                    FECHA_OP_HASTA = SAPFormatter.PrepararFecha(fecha.fechaFin)
                };
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();

                compensa = string.IsNullOrWhiteSpace(pago) ? compensa : "X";
                
                var error = service.SI_MPMF_MOAOP_CUENTA_CORRIENTE(compensa, contrato, fechaSAP, pago, proveedor, retencion, sociedad, out ZMPES6120[] salidas);
                return Map(salidas, error, fecha.fechaFin);
            }
            catch (Exception e)
            {
                Log.Error(e, $"Error en llamada SAP SI_MPMF_MOAOP_CUENTA_CORRIENTE [{compensa}, {contrato}, {pago}, {proveedor}, {retencion}, {sociedad}].");
                throw e;
            }
        }

        protected abstract T Map(ZMPES6120[] salidas, ZMPES4910 error, DateTime fechaFin);
    }

    public class CuentaCorrientesConsumerMOA : CuentaCorrientesConsumerMOABase<CuentaCorrienteWSMOAResponse>
    {
        protected override CuentaCorrienteWSMOAResponse Map(ZMPES6120[] salidas, ZMPES4910 error, DateTime fechaFin)
        {
            var result = new CuentaCorrienteWSMOAResponse();
            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }

            var orden = 1;

            foreach (ZMPES6120 cuentaCorrienteInfo in salidas)
            {
                var fechaVencimientoStr = string.IsNullOrEmpty(cuentaCorrienteInfo.PAGO_DIF_ARP)
                    ? cuentaCorrienteInfo.FEC_VTO
                    : cuentaCorrienteInfo.FECHA_PAGO_DIF_ARP;

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
                    fecVto = SAPFormatter.FormatearFecha(fechaVencimientoStr),
                    fecVtoDate = SAPFormatter.GetDateTime(fechaVencimientoStr),
                    fiscYear = cuentaCorrienteInfo.FISC_YEAR,
                    haber = cuentaCorrienteInfo.HABER,
                    haberString = SAPFormatter.FormatearMonto(cuentaCorrienteInfo.HABER, "$"),
                    importeArg = cuentaCorrienteInfo.IMPORTE_ARP,
                    importeArgString = SAPFormatter.FormatearMonto(cuentaCorrienteInfo.IMPORTE_ARP, "$"),
                    saldo = cuentaCorrienteInfo.SALDO,
                    saldoString = SAPFormatter.FormatearMonto(cuentaCorrienteInfo.SALDO, "$"),
                    moneda = cuentaCorrienteInfo.MONEDA,
                    orden = orden++,
                    ukurs = cuentaCorrienteInfo.UKURS,
                    ukursString = SAPFormatter.FormatearTipoCambio(cuentaCorrienteInfo.UKURS),
                    xblnr = cuentaCorrienteInfo.XBLNR
                });
            }

            return result;
        }

    }

    public class CuentaCorrientesExcelConsumerMOA : CuentaCorrientesConsumerMOABase<CuentaCorrienteExcelWSMOAResponse>
    {
        protected override CuentaCorrienteExcelWSMOAResponse Map(ZMPES6120[] salidas, ZMPES4910 error, DateTime fechaFin)
        {
            var result = new CuentaCorrienteExcelWSMOAResponse();
            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }

            foreach (ZMPES6120 cuentaCorrienteInfo in salidas)
            {
                var fechaVencimientoStr = string.IsNullOrEmpty(cuentaCorrienteInfo.PAGO_DIF_ARP)
                    ? cuentaCorrienteInfo.FEC_VTO
                    : cuentaCorrienteInfo.FECHA_PAGO_DIF_ARP;

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
                    fecVto = SAPFormatter.FormatearFecha(fechaVencimientoStr),
                    fiscYear = cuentaCorrienteInfo.FISC_YEAR,
                    saldo = cuentaCorrienteInfo.SALDO,
                    haber = cuentaCorrienteInfo.HABER,
                    importeArg = cuentaCorrienteInfo.IMPORTE_ARP,
                    ukurs = cuentaCorrienteInfo.UKURS,
                    xblnr = cuentaCorrienteInfo.XBLNR
                });
            }

            return result;
        }

    }

    public class CuentaCorrientesAgrupadaConsumerMOA : CuentaCorrientesConsumerMOABase<CuentaCorrienteAgrupadaWSMOAResponse>
    {
        protected override CuentaCorrienteAgrupadaWSMOAResponse Map(ZMPES6120[] salidas, ZMPES4910 error, DateTime fechaFin)
        {
            var result = new CuentaCorrienteAgrupadaWSMOAResponse();
            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }

            result.fechaSaldo = SAPFormatter.FormatearFecha(fechaFin);

            result.cuentasCorrientesSinAgrupar = salidas
                .Where(x =>
                    x.AGRUPADOR == ""
                )
                .GroupBy(x => x.AGRUPADOR)
                .Select(x => new CuentaCorrienteAgrupadaView
                {
                    agrupador = x.Key,
                    cuentasCorrientes = x.Select(z => new CuentaCorrienteView
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
                        fecVto = string.IsNullOrEmpty(z.PAGO_DIF_ARP)
                            ? SAPFormatter.FormatearFecha(z.FEC_VTO)
                            : SAPFormatter.FormatearFecha(z.FECHA_PAGO_DIF_ARP),
                        fecVtoDate = string.IsNullOrEmpty(z.PAGO_DIF_ARP)
                            ? SAPFormatter.GetDateTime(z.FEC_VTO)
                            : SAPFormatter.GetDateTime(z.FECHA_PAGO_DIF_ARP),
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

    public class CuentaCorrientesAgrupadaExcelConsumerMOA : CuentaCorrientesConsumerMOABase<CuentaCorrienteAgrupadaExcelWSMOAResponse>
    {
        protected override CuentaCorrienteAgrupadaExcelWSMOAResponse Map(ZMPES6120[] salidas, ZMPES4910 error, DateTime fechaFin)
        {
            var result = new CuentaCorrienteAgrupadaExcelWSMOAResponse();
            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }

            result.fechaSaldo = SAPFormatter.FormatearFecha(fechaFin);

            result.cuentasCorrientesSinAgrupar = salidas
                .Where(x =>
                    x.AGRUPADOR == "" &&
                    (string.IsNullOrEmpty(x.PAGO_DIF_ARP) || string.IsNullOrEmpty(x.DOC_PAGO_DIF_ARP))
                )
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
                        fecVto = string.IsNullOrEmpty(z.PAGO_DIF_ARP)
                            ? SAPFormatter.FormatearFecha(z.FEC_VTO)
                            : SAPFormatter.FormatearFecha(z.FECHA_PAGO_DIF_ARP),
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
                }).ToList();

            return result;
        }
    }
}
