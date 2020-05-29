using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.ViewModel.Flete;
using SustitucionMOAModel.Models.WSMapMOA.Flete;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.FletesWebServiceMOA;

namespace SustitucionMOAWS.WSConsumers
{
    public class FletesConsumerMOA
    {
        SI_MPMF_MOAOP_VIAJESClient service = new SI_MPMF_MOAOP_VIAJESClient();

        public object request(string proveedor, DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                ZMPES4870[] viajes = new ZMPES4870[] { };
                ZMPES4890[] ccpps = new ZMPES4890[] { };
                ZMPES4880[] proformas = new ZMPES4880[] { };
                ZMPES4970[] proformasSalidas = new ZMPES4970[] { };
                string fechaInicioString = SAPFormatter.PrepararFecha(fechaInicio);
                string fechaFinString = SAPFormatter.PrepararFecha(fechaFin);
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                ZMPES4910 error = service.SI_MPMF_MOAOP_VIAJES(ccpps, fechaInicioString, fechaFinString, proformas, proveedor, out proformasSalidas, out viajes);
                return map(error, proformasSalidas, viajes);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        protected virtual object map(ZMPES4910 error, ZMPES4970[] proformasSalidas, ZMPES4870[] viajes)
        {
            FletesWSMOAResponse result = new FletesWSMOAResponse();

            if (error != null) {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }

            foreach (ZMPES4870 viaje in viajes)
            {
                result.viajes.Add(new Viaje()
                {
                    ccpp = SAPFormatter.FormatearCCPP(viaje.CCPP),
                    descMat = viaje.DESC_MAT,
                    destino = viaje.DESTINO,
                    factura = viaje.FACTURA,
                    fechaCCPP = SAPFormatter.FormatearFecha(viaje.FECHA_CCPP),
                    fechaCCPPDate = SAPFormatter.GetDateTime(viaje.FECHA_CCPP),
                    kg = viaje.KG,
                    kgString = SAPFormatter.FormatearCantidad(viaje.KG, "KG"),
                    nroProforma = viaje.NRO_PROFORMA,
                    origen = viaje.ORIGEN,
                    patente = viaje.PATENTE,
                    tarifa = viaje.TARIFA,
                    tarifaString = SAPFormatter.FormatearMontoTarifas(viaje.TARIFA, "$/Tns"),
                    peaje = viaje.PEAJE,
                    peajeString = SAPFormatter.FormatearMonto(viaje.PEAJE, "$"),
                    playa = viaje.PLAYA,
                    playaString = SAPFormatter.FormatearMonto(viaje.PLAYA, "$"),
                    importe = viaje.IMPORTE,
                    importeString = SAPFormatter.FormatearMonto(viaje.IMPORTE, ""),
                    status = viaje.STATUS

                });
            }

            foreach (ZMPES4970 proformaSalida in proformasSalidas)
            {
                result.proformas.Add(new Proforma()
                {
                    nroProforma = proformaSalida.NRO_PROFORMA,
                    descRegion = proformaSalida.DESC_REGION,
                    fechaFC = proformaSalida.FECHA_FC,
                    region = proformaSalida.REGION

                });
            }

            return result;
        }

        protected virtual string getStatus()
        {
            return "2";
        }
    }

    public class FletesPendientesConsumerMOA : FletesConsumerMOA
    {
        protected override object map(ZMPES4910 error, ZMPES4970[] proformasSalidas, ZMPES4870[] viajes)
        {
            FletesWSMOAResponse result = new FletesWSMOAResponse();

            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }

            foreach (ZMPES4870 viaje in viajes)
            {
                if (viaje.STATUS == getStatus())
                {
                    result.viajes.Add(new Viaje()
                    {
                        ccpp = SAPFormatter.FormatearCCPP(viaje.CCPP),
                        descMat = viaje.DESC_MAT,
                        destino = viaje.DESTINO,
                        factura = viaje.FACTURA,
                        fechaCCPP = SAPFormatter.FormatearFecha(viaje.FECHA_CCPP),
                        fechaCCPPDate = SAPFormatter.GetDateTime(viaje.FECHA_CCPP),
                        kg = viaje.KG,
                        kgString = SAPFormatter.FormatearCantidad(viaje.KG, "KG"),
                        nroProforma = viaje.NRO_PROFORMA,
                        origen = viaje.ORIGEN,
                        patente = viaje.PATENTE,
                        tarifa = viaje.TARIFA,
                        tarifaString = SAPFormatter.FormatearMontoTarifas(viaje.TARIFA, "$/Tns"),
                        peaje = viaje.PEAJE,
                        peajeString = SAPFormatter.FormatearMonto(viaje.PEAJE, "$"),
                        playa = viaje.PLAYA,
                        playaString = SAPFormatter.FormatearMonto(viaje.PLAYA, "$"),
                        importe = viaje.IMPORTE,
                        importeString = SAPFormatter.FormatearMonto(viaje.IMPORTE, ""),
                        status = viaje.STATUS

                    });
                }
            }

            foreach (ZMPES4970 proformaSalida in proformasSalidas)
            {
                result.proformas.Add(new Proforma()
                {
                    nroProforma = proformaSalida.NRO_PROFORMA,
                    descRegion = proformaSalida.DESC_REGION,
                    fechaFC = proformaSalida.FECHA_FC,
                    region = proformaSalida.REGION

                });
            }

            return result;
        }
    }

    public class FletesPendientesExcelConsumerMOA : FletesConsumerMOA
    {
        protected override object map(ZMPES4910 error, ZMPES4970[] proformasSalidas, ZMPES4870[] viajes)
        {
            FletesExcelWSMOAResponse result = new FletesExcelWSMOAResponse();

            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }

            foreach (ZMPES4870 viaje in viajes)
            {
                if (viaje.STATUS == getStatus())
                {
                    result.viajes.Add(new ViajeExcel()
                    {
                        ccpp = SAPFormatter.FormatearCCPP(viaje.CCPP),
                        descMat = viaje.DESC_MAT,
                        destino = viaje.DESTINO,
                        factura = viaje.FACTURA,
                        fechaCCPP = SAPFormatter.FormatearFecha(viaje.FECHA_CCPP),
                        kg = viaje.KG,
                        nroProforma = viaje.NRO_PROFORMA,
                        origen = viaje.ORIGEN,
                        patente = viaje.PATENTE,
                        tarifa = viaje.TARIFA,
                        peaje = viaje.PEAJE,
                        playa = viaje.PLAYA,
                        importe = viaje.IMPORTE,
                        status = viaje.STATUS

                    });
                }
            }

            foreach (ZMPES4970 proformaSalida in proformasSalidas)
            {
                result.proformas.Add(new Proforma()
                {
                    nroProforma = proformaSalida.NRO_PROFORMA,
                    descRegion = proformaSalida.DESC_REGION,
                    fechaFC = proformaSalida.FECHA_FC,
                    region = proformaSalida.REGION

                });
            }

            return result;
        }
    }

    public class FletesAgrupadosConsumerMOA : FletesConsumerMOA
    {
        protected override object map(ZMPES4910 error, ZMPES4970[] proformasSalidas, ZMPES4870[] viajes)
        {
            FleteAgrupadosViewModel result = new FleteAgrupadosViewModel();

            FletesAgrupadosWSMOAResponse data = new FletesAgrupadosWSMOAResponse();

            if (error != null)
            {
                data.error.codigo = error.CODIGO;
                data.error.descripcion = error.DESCRIPCION;
                data.error.tipo = error.TIPO;
            }

            foreach (ZMPES4970 proformaSalida in proformasSalidas)
            {
                data.proformas.Add(new Proforma()
                {
                    nroProforma = proformaSalida.NRO_PROFORMA,
                    descRegion = proformaSalida.DESC_REGION,
                    fechaFC = proformaSalida.FECHA_FC,
                    region = proformaSalida.REGION

                });
            }

            data.viajes = viajes
                .Where(x => x.STATUS.Equals(getStatus()))
                .GroupBy(x => x.NRO_PROFORMA)
                .Select(x => new ViajeAgrupado
                {
                    proforma = x.Key,
                    proveedor = "", //Falta los datos
                    proveedorId = "", //Faltan los datos
                    fecha = "", //Se agregan con los datos de ProformasSalidas
                    region = "",//Se agregan con los datos de ProformasSalidas
                    regionId = "",//Se agregan con los datos de ProformasSalidas
                    totalImporteString = SAPFormatter.FormatearMonto(x.Sum(v => v.IMPORTE), "$"),
                    totalImporte = x.Sum(v => v.IMPORTE),
                    totalKg = SAPFormatter.FormatearCantidad(x.Sum(v => v.KG), "KG"),
                    factura = x.First().FACTURA,
                    fechaEmision = SAPFormatter.FormatearFecha(x.First().FECHA_FC),
                    viajeItem = x.Select(v => new Viaje
                    {
                        ccpp = SAPFormatter.FormatearCCPP(v.CCPP),
                        descMat = v.DESC_MAT,
                        destino = v.DESTINO,
                        factura = v.FACTURA,
                        fechaCCPP = SAPFormatter.FormatearFecha(v.FECHA_CCPP),
                        fechaCCPPDate = SAPFormatter.GetDateTime(v.FECHA_CCPP),
                        kg = v.KG,
                        kgString = SAPFormatter.FormatearCantidad(v.KG, "KG"),
                        nroProforma = v.NRO_PROFORMA,
                        origen = v.ORIGEN,
                        patente = v.PATENTE,
                        tarifa = v.TARIFA,
                        tarifaString = SAPFormatter.FormatearMontoTarifas(v.TARIFA, "$/Tns"),
                        peaje = v.PEAJE,
                        peajeString = SAPFormatter.FormatearMonto(v.PEAJE, "$"),
                        playa = v.PLAYA,
                        playaString = SAPFormatter.FormatearMonto(v.PLAYA, "$"),
                        importe = v.IMPORTE,
                        importeString = SAPFormatter.FormatearMonto(v.IMPORTE, ""),
                        status = v.STATUS
                    }).ToList()

                })
                .ToList();

            foreach (ViajeAgrupado proforma in data.viajes) {
                Proforma proformaInfo = data.proformas.Where(x => x.nroProforma == proforma.proforma).FirstOrDefault();
                if (proformaInfo != null) {
                    proforma.fecha = proformaInfo.fechaFC;
                    proforma.region = proformaInfo.descRegion;
                    proforma.regionId = proformaInfo.region;
                }
            }

            result.data = data;
            result.filtroProducto = new DropdownContent(viajes.Where(x => x.STATUS.Equals(getStatus())).GroupBy(i => i.DESC_MAT).Select(x => new DropdownOption { value = x.Key, label = x.Key + " (" + x.Count() + ")" }).ToList());

            return result;
        }

        protected override string getStatus() {
            return "1";
        }
    }

    public class FletesAgrupadosAFacturarConsumerMOA : FletesAgrupadosConsumerMOA
    {
        protected override string getStatus()
        {
            return "1";
        }
    }

    public class FletesAgrupadosFacturadoConsumerMOA : FletesAgrupadosConsumerMOA
    {
        protected override string getStatus()
        {
            return "3";
        }
    }

    public class FletesAgrupadosExcelConsumerMOA : FletesConsumerMOA
    {
        protected override object map(ZMPES4910 error, ZMPES4970[] proformasSalidas, ZMPES4870[] viajes)
        {
            FletesAgrupadosExcelWSMOAResponse data = new FletesAgrupadosExcelWSMOAResponse();

            if (error != null)
            {
                data.error.codigo = error.CODIGO;
                data.error.descripcion = error.DESCRIPCION;
                data.error.tipo = error.TIPO;
            }

            foreach (ZMPES4970 proformaSalida in proformasSalidas)
            {
                data.proformas.Add(new Proforma()
                {
                    nroProforma = proformaSalida.NRO_PROFORMA,
                    descRegion = proformaSalida.DESC_REGION,
                    fechaFC = proformaSalida.FECHA_FC,
                    region = proformaSalida.REGION

                });
            }

            data.viajes = viajes
                .Where(x => x.STATUS.Equals(getStatus()))
                .GroupBy(x => x.NRO_PROFORMA)
                .Select(x => new ViajeAgrupadoExcel
                {
                    proforma = x.Key,
                    proveedor = "", //Falta los datos
                    proveedorId = "", //Faltan los datos
                    fecha = "", //Se agregan con los datos de ProformasSalidas
                    region = "",//Se agregan con los datos de ProformasSalidas
                    regionId = "",//Se agregan con los datos de ProformasSalidas
                    totalImporte = x.Sum(v => v.IMPORTE),
                    totalKg = x.Sum(v => v.KG).ToString(),
                    factura = x.First().FACTURA,
                    viajeItem = x.Select(v => new ViajeExcel
                    {
                        ccpp = SAPFormatter.FormatearCCPP(v.CCPP),
                        descMat = v.DESC_MAT,
                        destino = v.DESTINO,
                        factura = v.FACTURA,
                        fechaCCPP = SAPFormatter.FormatearFecha(v.FECHA_CCPP),
                        kg = v.KG,
                        nroProforma = v.NRO_PROFORMA,
                        origen = v.ORIGEN,
                        patente = v.PATENTE,
                        tarifa = v.TARIFA,
                        peaje = v.PEAJE,
                        playa = v.PLAYA,
                        importe = v.IMPORTE,
                        status = v.STATUS
                    }).ToList()

                })
                .ToList();

            foreach (ViajeAgrupadoExcel proforma in data.viajes)
            {
                Proforma proformaInfo = data.proformas.Where(x => x.nroProforma == proforma.proforma).FirstOrDefault();
                if (proformaInfo != null)
                {
                    proforma.fecha = proformaInfo.fechaFC;
                    proforma.region = proformaInfo.descRegion;
                    proforma.regionId = proformaInfo.region;
                }
            }


            return data;
        }

        protected override string getStatus()
        {
            return "1";
        }
    }

    public class FletesAgrupadosAFacturarExcelConsumerMOA : FletesAgrupadosExcelConsumerMOA
    {
        protected override string getStatus()
        {
            return "1";
        }
    }

    public class FletesAgrupadosFacturadoExcelConsumerMOA : FletesAgrupadosExcelConsumerMOA
    {
        protected override string getStatus()
        {
            return "3";
        }
    }
}
