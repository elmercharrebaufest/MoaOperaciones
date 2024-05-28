using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models.WSMapMOA.Vendedor.Detalle;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.VendedorDetalleWebServiceMOA;

namespace SustitucionMOAWS.WSConsumers
{
    public class VendedorDetalleConsumerMOA
    {
        SI_MPMF_MOAOP_DETALLES_VENDEDORClient service = new SI_MPMF_MOAOP_DETALLES_VENDEDORClient();

        public VendedorDetalleWSMOAResponse request(string vendedor, string proveedor)
        {
            try
            {
                ZMPES4470[] cabeceras = new ZMPES4470[] { };
                ZMPES4430[] convenios = new ZMPES4430[] { };
                ZMPES4460[] cuentas = new ZMPES4460[] { };
                ZMPES4450[] exenciones = new ZMPES4450[] { };
                ZMPTE3420[] actividades = new ZMPTE3420[] { };
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                string error = service.SI_MPMF_MOAOP_DETALLES_VENDEDOR(proveedor, vendedor, ref cabeceras, ref convenios, ref cuentas, ref exenciones, out actividades);
                VendedorDetalleWSMOAResponse result = Map(error, cabeceras, convenios, cuentas, exenciones, actividades);
                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }

        private VendedorDetalleWSMOAResponse Map(string error, ZMPES4470[] cabeceras, ZMPES4430[] convenios, ZMPES4460[] cuentas, ZMPES4450[] exenciones, ZMPTE3420[] actividades)
        {
            VendedorDetalleWSMOAResponse result = new VendedorDetalleWSMOAResponse
            {
                error = error
            };

            foreach (ZMPES4470 cabecera in cabeceras)
            {
                result.cabeceras.Add(new Cabecera()
                {
                    actividadAfip = cabecera.ACTIVIDAD_AFIP,
                    calleEnvio = cabecera.CALLE_ENVIO,
                    calleFiscal = cabecera.CALLE_FISCAL,
                    cateFiscalIibb = cabecera.CATE_FISCAL_IIBB,
                    cateFiscalRet = cabecera.CATE_FISCAL_RET,
                    cpEnvio = cabecera.CP_ENVIO,
                    cpFiscal = cabecera.CP_FISCAL,
                    cuit = cabecera.CUIT,
                    descripcion = cabecera.DESCRIPCION,
                    locaEnvio = cabecera.LOCA_ENVIO,
                    locaFiscal = cabecera.LOCA_FISCAL,
                    nroInscIibb = cabecera.NRO_INSC_IIBB,
                    proveedor = cabecera.PROVEEDOR,
                    provEnvio = cabecera.PROV_ENVIO,
                    provFiscal = cabecera.PROV_FISCAL,
                    rg2300 = cabecera.RG2300,
                    tribuDifPrecio = cabecera.TRIBU_DIF_PRECIO,
                    cm05 = cabecera.CM05,
                    fecha1276 = SAPFormatter.FormatearFecha(cabecera.F1276),
                    fechaLegajo = cabecera.F_LEGA
                });
            }


            if (result.cabeceras == null || result.cabeceras.Count() == 0 || result.cabeceras[0] == null)
            {
                result.cabeceras.Add(new Cabecera());
            }

            if (actividades != null)
            {
                foreach (ZMPTE3420 actividad in actividades)
                {
                    if (actividad.INSCRIPTO.ToUpper() == "X")
                    {
                        result.cabeceras[0].categorias.Add(actividad.CLASIFICACION);
                    }
                }
            }
            else
            {
                result.cabeceras[0].categorias.Add("-");
            }

            foreach (ZMPES4430 convenio in convenios)
            {
                result.convenios.Add(new Convenio()
                {
                    coeficiente = convenio.COEFICIENTE,
                    descripcion = convenio.DESCRIPCION,
                    proveedor = convenio.PROVEEDOR,
                    provincia = convenio.PROVINCIA
                });
            }

            foreach (ZMPES4460 cuenta in cuentas)
            {
                result.cuentas.Add(new Cuenta()
                {
                    banco = cuenta.BANCO,
                    cbu = cuenta.CBU,
                    cuenta = cuenta.CUENTA,
                    proveedor = cuenta.PROVEEDOR,
                    tipoCta = cuenta.TIPO_CTA

                });
            }

            foreach (ZMPES4450 exencion in exenciones)
            {
                result.exenciones.Add(new Exencion()
                {
                    descripcion = exencion.DESCRIPCION,
                    exencion = exencion.EXENCION,
                    fechaDesde = SAPFormatter.FormatearFecha(exencion.FECHA_DESDE),
                    fechaHasta = SAPFormatter.FormatearFecha(exencion.FECHA_HASTA),
                    fechaHastaDate = exencion.FECHA_HASTA,
                    proveedor = exencion.PROVEEDOR,
                    tipoRetencion = exencion.TIPO_RETENCION

                });
            }
            return result;
        }
    }
}
