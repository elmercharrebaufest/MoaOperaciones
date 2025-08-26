using SustitucionMOAAssets;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models.WSMapMOA.Vendedor.Detalle;
using SustitucionMOAModel.Models.WSMapMOA.Vendedor.Habilitado;
using SustitucionMOARepositorio.Extensiones;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.VendedorHabilitadoWebServiceMOA;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MOAOP;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WSConsumers
{
    public class VendedorHabilitadoConsumerMOA : IVendedorHabilitadoConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserSinPI"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassSinPI"];

        public VendedorHabilitadoConsumerMOA()
        {
        }

        public VendedorHabilitadoWSMOAResponse Request(string cuit, string sociedad, string usuario)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var request = new Z_MPMF_MOAOP_VENDED_HABILIT()
                    {
                        IM_CUIT = cuit,
                        IM_SOCIEDAD = sociedad,
                        IM_USUARIO = usuario
                    };
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_VENDED_HABILIT request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MPMF_MOAOP_VENDED_HABILIT(request);
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_VENDED_HABILIT response");
                    Log.Info(response.ToXml());
                    VendedorHabilitadoWSMOAResponse result = MapSinPI(cuit, response.STATUS, response.T_CABE, response.T_CONVENIO, response.T_EXECIONES, response.PT_ACTIVIDAD_PROV);
                    return result;
                }
                else
                {
                    SI_MPMF_MOAOP_VENDED_HABILITClient service;
                    var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=SI_MPMF_MOAOP_VENDED_HABILIT&amp;interfaceNamespace=urn%3AOPERACIONES";
                    service = new SI_MPMF_MOAOP_VENDED_HABILITClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();

                    VendedorHabilitadoWebServiceMOA.ZMPES4470[] cabeceras  = new VendedorHabilitadoWebServiceMOA.ZMPES4470[] { };
                    VendedorHabilitadoWebServiceMOA.ZMPES4430[] convenios  = new VendedorHabilitadoWebServiceMOA.ZMPES4430[] { };
                    VendedorHabilitadoWebServiceMOA.ZMPES4450[] exenciones = new VendedorHabilitadoWebServiceMOA.ZMPES4450[] { };
                    string status = "";

                    VendedorHabilitadoWebServiceMOA.ZMPTE3420[] actividades = service.SI_MPMF_MOAOP_VENDED_HABILIT(cuit, sociedad, usuario, out status, out cabeceras, out convenios, out exenciones);
                    VendedorHabilitadoWSMOAResponse result = Map(cuit, status, cabeceras, convenios, exenciones, actividades);
                    return result;
                }
            }
            catch (InfoCustomException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private VendedorHabilitadoWSMOAResponse Map(string cuit, string status, VendedorHabilitadoWebServiceMOA.ZMPES4470[] cabeceras, VendedorHabilitadoWebServiceMOA.ZMPES4430[] convenios, VendedorHabilitadoWebServiceMOA.ZMPES4450[] exenciones, VendedorHabilitadoWebServiceMOA.ZMPTE3420[] actividades)
        {
            VendedorHabilitadoWSMOAResponse result = new VendedorHabilitadoWSMOAResponse();

            result.status = status;

            if (cabeceras.Length == 0 && cabeceras.Length == 0 && cabeceras.Length == 0)
            {
                throw new InfoCustomException(InfoMsg.ProveedorSinAlta);
            }

            if (cabeceras.Length == 0)
            {
                result.cabeceras.Add(new Cabecera() { });
            }

            foreach (VendedorHabilitadoWebServiceMOA.ZMPES4470 cabecera in cabeceras)
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

            if (result.cabeceras[0] == null)
            {
                result.cabeceras.Add(new Cabecera());
            }

            if (actividades != null)
            {
                foreach (VendedorHabilitadoWebServiceMOA.ZMPTE3420 actividad in actividades)
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

            if (convenios.Length == 0)
            {
                result.convenios.Add(new Convenio() { });
            }

            foreach (VendedorHabilitadoWebServiceMOA.ZMPES4430 convenio in convenios)
            {
                result.convenios.Add(new Convenio()
                {
                    coeficiente = convenio.COEFICIENTE,
                    descripcion = convenio.DESCRIPCION,
                    proveedor = convenio.PROVEEDOR,
                    provincia = convenio.PROVINCIA
                });
            }

            if (exenciones.Length == 0)
            {
                result.exenciones.Add(new Exencion() { });
            }

            foreach (VendedorHabilitadoWebServiceMOA.ZMPES4450 exencion in exenciones)
            {
                result.exenciones.Add(new Exencion()
                {
                    descripcion = exencion.DESCRIPCION,
                    exencion = exencion.EXENCION,
                    fechaDesde = SAPFormatter.FormatearFecha(exencion.FECHA_DESDE),
                    fechaHasta = SAPFormatter.FormatearFecha(exencion.FECHA_HASTA),
                    fechaHastaDate = exencion.FECHA_HASTA,
                    proveedor = exencion.PROVEEDOR

                });
            }

            return result;
        }
        private VendedorHabilitadoWSMOAResponse MapSinPI(string cuit, string status, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4470[] cabeceras, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4430[] convenios, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4450[] exenciones, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPTE3420[] actividades)
        {
            VendedorHabilitadoWSMOAResponse result = new VendedorHabilitadoWSMOAResponse();

            result.status = status;

            if (cabeceras.Length == 0 && cabeceras.Length == 0 && cabeceras.Length == 0)
            {
                throw new InfoCustomException(InfoMsg.ProveedorSinAlta);
            }

            if (cabeceras.Length == 0)
            {
                result.cabeceras.Add(new Cabecera() { });
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4470 cabecera in cabeceras)
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

            if (result.cabeceras[0] == null)
            {
                result.cabeceras.Add(new Cabecera());
            }

            if (actividades != null)
            {
                foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPTE3420 actividad in actividades)
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

            if (convenios.Length == 0)
            {
                result.convenios.Add(new Convenio() { });
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4430 convenio in convenios)
            {
                result.convenios.Add(new Convenio()
                {
                    coeficiente = convenio.COEFICIENTE,
                    descripcion = convenio.DESCRIPCION,
                    proveedor = convenio.PROVEEDOR,
                    provincia = convenio.PROVINCIA
                });
            }

            if (exenciones.Length == 0)
            {
                result.exenciones.Add(new Exencion() { });
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4450 exencion in exenciones)
            {
                result.exenciones.Add(new Exencion()
                {
                    descripcion = exencion.DESCRIPCION,
                    exencion = exencion.EXENCION,
                    fechaDesde = SAPFormatter.FormatearFecha(exencion.FECHA_DESDE),
                    fechaHasta = SAPFormatter.FormatearFecha(exencion.FECHA_HASTA),
                    fechaHastaDate = exencion.FECHA_HASTA,
                    proveedor = exencion.PROVEEDOR

                });
            }

            return result;
        }

    }
}
