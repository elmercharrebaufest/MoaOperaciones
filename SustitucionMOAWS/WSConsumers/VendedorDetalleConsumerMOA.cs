using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models.WSMapMOA.Vendedor.Detalle;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.VendedorDetalleWebServiceMOA;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MOAOP;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WSConsumers
{
    public class VendedorDetalleConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public VendedorDetalleWSMOAResponse request(string vendedor, string proveedor)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {

                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4470[] cabeceras   = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4470[] { };
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4430[] convenios   = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4430[] { };
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4460[] cuentas     = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4460[] { };
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4450[] exenciones  = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4450[] { };
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPTE3420[] actividades = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPTE3420[] { };

                    var request = new Z_MPMF_MOAOP_DETALLES_VENDEDOR()
                    {
                        PE_PROVEEDOR = proveedor,
                        PE_PROVEEDOR_DETALLE = vendedor,
                        T_CABE = cabeceras,
                        T_CONVENIO = convenios,
                        T_CUENTA = cuentas,
                        T_EXECIONES = exenciones
                    };

                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_DETALLES_VENDEDOR request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MPMF_MOAOP_DETALLES_VENDEDOR(request);
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_DETALLES_VENDEDOR response");
                    Log.Info(response.ToXml());

                    VendedorDetalleWSMOAResponse result = MapSinPI(response);
                    return result;

                }
                else
                {
                    SI_MPMF_MOAOP_DETALLES_VENDEDORClient service = new SI_MPMF_MOAOP_DETALLES_VENDEDORClient();

                    VendedorDetalleWebServiceMOA.ZMPES4470[] cabeceras   = new VendedorDetalleWebServiceMOA.ZMPES4470[] { };
                    VendedorDetalleWebServiceMOA.ZMPES4430[] convenios   = new VendedorDetalleWebServiceMOA.ZMPES4430[] { };
                    VendedorDetalleWebServiceMOA.ZMPES4460[] cuentas     = new VendedorDetalleWebServiceMOA.ZMPES4460[] { };
                    VendedorDetalleWebServiceMOA.ZMPES4450[] exenciones  = new VendedorDetalleWebServiceMOA.ZMPES4450[] { };
                    VendedorDetalleWebServiceMOA.ZMPTE3420[] actividades = new VendedorDetalleWebServiceMOA.ZMPTE3420[] { };
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    string error = service.SI_MPMF_MOAOP_DETALLES_VENDEDOR(proveedor, vendedor, ref cabeceras, ref convenios, ref cuentas, ref exenciones, out actividades);
                    VendedorDetalleWSMOAResponse result = Map(error, cabeceras, convenios, cuentas, exenciones, actividades);
                    return result;
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        private VendedorDetalleWSMOAResponse MapSinPI(Z_MPMF_MOAOP_DETALLES_VENDEDORResponse response)
        {
            VendedorDetalleWSMOAResponse result = new VendedorDetalleWSMOAResponse
            {
                error = response.PS_RETURN
            };

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4470 cabecera in response.T_CABE)
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

            if (response.PT_ACTIVIDAD_PROV != null)
            {
                foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPTE3420 actividad in response.PT_ACTIVIDAD_PROV)
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

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4430 convenio in response.T_CONVENIO)
            {
                result.convenios.Add(new Convenio()
                {
                    coeficiente = convenio.COEFICIENTE,
                    descripcion = convenio.DESCRIPCION,
                    proveedor = convenio.PROVEEDOR,
                    provincia = convenio.PROVINCIA
                });
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4460 cuenta in response.T_CUENTA)
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

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4450 exencion in response.T_EXECIONES)
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

        private VendedorDetalleWSMOAResponse Map(string error, VendedorDetalleWebServiceMOA.ZMPES4470[] cabeceras, VendedorDetalleWebServiceMOA.ZMPES4430[] convenios, VendedorDetalleWebServiceMOA.ZMPES4460[] cuentas, VendedorDetalleWebServiceMOA.ZMPES4450[] exenciones, VendedorDetalleWebServiceMOA.ZMPTE3420[] actividades)
        {
            VendedorDetalleWSMOAResponse result = new VendedorDetalleWSMOAResponse
            {
                error = error
            };

            foreach (VendedorDetalleWebServiceMOA.ZMPES4470 cabecera in cabeceras)
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
                foreach (VendedorDetalleWebServiceMOA.ZMPTE3420 actividad in actividades)
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

            foreach (VendedorDetalleWebServiceMOA.ZMPES4430 convenio in convenios)
            {
                result.convenios.Add(new Convenio()
                {
                    coeficiente = convenio.COEFICIENTE,
                    descripcion = convenio.DESCRIPCION,
                    proveedor = convenio.PROVEEDOR,
                    provincia = convenio.PROVINCIA
                });
            }

            foreach (VendedorDetalleWebServiceMOA.ZMPES4460 cuenta in cuentas)
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

            foreach (VendedorDetalleWebServiceMOA.ZMPES4450 exencion in exenciones)
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
