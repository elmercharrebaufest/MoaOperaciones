using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.CartaPorte;
using SustitucionMOAWS.AplicacionesWebServiceMOA;
using SustitucionMOAWS.CredentialService;
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
    public class AplicacionesConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserS4"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassS4"];
        public object request(string proveedor, List<FechaWS> fechas, List<string> contratos, string CCPP)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    CCPP = CCPP ?? "";
                    contratos = contratos ?? new List<string>();
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4050[] aplicaciones_out = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4050[] { };
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4060[] contratos_in = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4060[] { };
                    List<WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100> fechasSAP = new List<WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100>() { };

                    foreach (FechaWS fecha in fechas)
                    {
                        fechasSAP.Add(new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100()
                        {
                            FECHA_OP = SAPFormatter.PrepararFecha(fecha.fechaInicio),
                            FECHA_OP_HASTA = SAPFormatter.PrepararFecha(fecha.fechaFin)
                        });
                    }

                    contratos_in = contratos.Select(x => new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4060 { CONTRATO = x }).ToArray();
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100[] fechasSAPArray = fechasSAP.ToArray();

                    var request = new Z_MPMF_MOAOP_APLICACIONES()
                    {
                        IM_CCPP = CCPP,
                        PE_PROVEEDOR = proveedor,
                        T_APLICACIONES_OUT = aplicaciones_out,
                        T_CONTRATOS_IN = contratos_in,
                        T_FECHA_IN = fechasSAPArray
                    };
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_APLICACIONES request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MPMF_MOAOP_APLICACIONES(request);
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_APLICACIONES response");
                    Log.Info(response.ToXml());
                    return MapSinPI(response);

                }
                else
                {
                    SI_MPMF_MOAOP_APLICACIONESClient service = new SI_MPMF_MOAOP_APLICACIONESClient();
                    CCPP = CCPP ?? "";
                    contratos = contratos ?? new List<string>();
                    AplicacionesWebServiceMOA.ZMPES4050[] aplicaciones_out = new AplicacionesWebServiceMOA.ZMPES4050[] { };
                    AplicacionesWebServiceMOA.ZMPES4060[] contratos_in = new AplicacionesWebServiceMOA.ZMPES4060[] { };
                    List<AplicacionesWebServiceMOA.ZMPES4100> fechasSAP = new List<AplicacionesWebServiceMOA.ZMPES4100>() { };

                    foreach (FechaWS fecha in fechas)
                    {
                        fechasSAP.Add(new AplicacionesWebServiceMOA.ZMPES4100()
                        {
                            FECHA_OP = SAPFormatter.PrepararFecha(fecha.fechaInicio),
                            FECHA_OP_HASTA = SAPFormatter.PrepararFecha(fecha.fechaFin)
                        });
                    }

                    contratos_in = contratos.Select(x => new AplicacionesWebServiceMOA.ZMPES4060 { CONTRATO = x }).ToArray();
                    AplicacionesWebServiceMOA.ZMPES4100[] fechasSAPArray = fechasSAP.ToArray();

                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    AplicacionesWebServiceMOA.ZMPES4910 error = service.SI_MPMF_MOAOP_APLICACIONES(CCPP, proveedor, ref aplicaciones_out, ref contratos_in, ref fechasSAPArray);
                    return Map(error, aplicaciones_out);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        protected virtual object MapSinPI(Z_MPMF_MOAOP_APLICACIONESResponse response)
        {
            CartaPorteWSMOAResponse result = new CartaPorteWSMOAResponse();

            if (response.MENSAJE_ERROR != null)
            {
                result.error.codigo = response.MENSAJE_ERROR.CODIGO;
                result.error.descripcion = response.MENSAJE_ERROR.DESCRIPCION;
                result.error.tipo = response.MENSAJE_ERROR.TIPO;
            }
            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4050 aplicacionInfo in response.T_APLICACIONES_OUT)
            {
                result.cartasPorte.Add(new CartaPorteView()
                {
                    fechaDescarga = SAPFormatter.FormatearFecha(aplicacionInfo.FECHA_DESCARGA),
                    fechaDescargaDate = SAPFormatter.GetDateTime(aplicacionInfo.FECHA_DESCARGA),
                    cartaPorte = aplicacionInfo.CARTA_PORTE,
                    producto = aplicacionInfo.PRODUCTO,
                    netoDescontado = aplicacionInfo.NETO_DESCONTADO,
                    netoDescontadoString = SAPFormatter.FormatearCantidad(aplicacionInfo.NETO_DESCONTADO, aplicacionInfo.UNIME_NETO),
                    pendAplicacion = aplicacionInfo.PEND_APLICACION,
                    pendAplicacionString = SAPFormatter.FormatearCantidad(aplicacionInfo.PEND_APLICACION, aplicacionInfo.UNIME_APLIC),
                    aLiquidar = aplicacionInfo.A_LIQUIDAR,
                    aLiquidarString = SAPFormatter.FormatearCantidad(aplicacionInfo.A_LIQUIDAR, aplicacionInfo.UNIME_A_LIQUIDAR),
                    contrnum = aplicacionInfo.CONTRNUM,
                    contrvend = aplicacionInfo.CONTRVEND,
                    idVendedor = aplicacionInfo.ID_VENDEDOR,
                    vendedor = aplicacionInfo.VENDEDOR
                });

            }
            return result;
        }
        protected virtual object Map(AplicacionesWebServiceMOA.ZMPES4910 error, AplicacionesWebServiceMOA.ZMPES4050[] aplicaciones_out)
        {
            CartaPorteWSMOAResponse result = new CartaPorteWSMOAResponse();

            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }
            foreach (AplicacionesWebServiceMOA.ZMPES4050 aplicacionInfo in aplicaciones_out)
            {
                result.cartasPorte.Add(new CartaPorteView()
                {
                    fechaDescarga = SAPFormatter.FormatearFecha(aplicacionInfo.FECHA_DESCARGA),
                    fechaDescargaDate = SAPFormatter.GetDateTime(aplicacionInfo.FECHA_DESCARGA),
                    cartaPorte = aplicacionInfo.CARTA_PORTE,
                    producto = aplicacionInfo.PRODUCTO,
                    netoDescontado = aplicacionInfo.NETO_DESCONTADO,
                    netoDescontadoString = SAPFormatter.FormatearCantidad(aplicacionInfo.NETO_DESCONTADO, aplicacionInfo.UNIME_NETO),
                    pendAplicacion = aplicacionInfo.PEND_APLICACION,
                    pendAplicacionString = SAPFormatter.FormatearCantidad(aplicacionInfo.PEND_APLICACION, aplicacionInfo.UNIME_APLIC),
                    aLiquidar = aplicacionInfo.A_LIQUIDAR,
                    aLiquidarString = SAPFormatter.FormatearCantidad(aplicacionInfo.A_LIQUIDAR, aplicacionInfo.UNIME_A_LIQUIDAR),
                    contrnum = aplicacionInfo.CONTRNUM,
                    contrvend = aplicacionInfo.CONTRVEND,
                    idVendedor = aplicacionInfo.ID_VENDEDOR,
                    vendedor = aplicacionInfo.VENDEDOR
                });

            }
            return result;
        }
    }

    public class AplicacionesExcelConsumerMOA : AplicacionesConsumerMOA
    {

        protected override object Map(AplicacionesWebServiceMOA.ZMPES4910 error, AplicacionesWebServiceMOA.ZMPES4050[] aplicaciones_out)
        {
            CartaPorteExcelWSMOAResponse result = new CartaPorteExcelWSMOAResponse();

            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }

            foreach (AplicacionesWebServiceMOA.ZMPES4050 aplicacionInfo in aplicaciones_out)
            {
                result.cartasPorte.Add(new CartaPorte()
                {
                    fechaDescarga = SAPFormatter.FormatearFecha(aplicacionInfo.FECHA_DESCARGA),
                    cartaPorte = aplicacionInfo.CARTA_PORTE,
                    producto = aplicacionInfo.PRODUCTO,
                    unidadNetoDescontado = aplicacionInfo.UNIME_NETO,
                    netoDescontado = aplicacionInfo.NETO_DESCONTADO,
                    unidadPendAplicacion = aplicacionInfo.UNIME_APLIC,
                    pendAplicacion = aplicacionInfo.PEND_APLICACION,
                    unidadALiquidar = aplicacionInfo.UNIME_A_LIQUIDAR,
                    aLiquidar = aplicacionInfo.A_LIQUIDAR,
                    contrnum = aplicacionInfo.CONTRNUM,
                    contrvend = aplicacionInfo.CONTRVEND,
                    idVendedor = aplicacionInfo.ID_VENDEDOR,
                    vendedor = aplicacionInfo.VENDEDOR
                });
            }
            return result;
        }
        protected virtual object MapSinPI(Z_MPMF_MOAOP_APLICACIONESResponse response)
        {
            CartaPorteExcelWSMOAResponse result = new CartaPorteExcelWSMOAResponse();

            if (response.MENSAJE_ERROR != null)
            {
                result.error.codigo = response.MENSAJE_ERROR.CODIGO;
                result.error.descripcion = response.MENSAJE_ERROR.DESCRIPCION;
                result.error.tipo = response.MENSAJE_ERROR.TIPO;
            }
            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4050 aplicacionInfo in response.T_APLICACIONES_OUT)
            {
                result.cartasPorte.Add(new CartaPorte()
                {
                    fechaDescarga = SAPFormatter.FormatearFecha(aplicacionInfo.FECHA_DESCARGA),
                    cartaPorte = aplicacionInfo.CARTA_PORTE,
                    producto = aplicacionInfo.PRODUCTO,
                    unidadNetoDescontado = aplicacionInfo.UNIME_NETO,
                    netoDescontado = aplicacionInfo.NETO_DESCONTADO,
                    unidadPendAplicacion = aplicacionInfo.UNIME_APLIC,
                    pendAplicacion = aplicacionInfo.PEND_APLICACION,
                    unidadALiquidar = aplicacionInfo.UNIME_A_LIQUIDAR,
                    aLiquidar = aplicacionInfo.A_LIQUIDAR,
                    contrnum = aplicacionInfo.CONTRNUM,
                    contrvend = aplicacionInfo.CONTRVEND,
                    idVendedor = aplicacionInfo.ID_VENDEDOR,
                    vendedor = aplicacionInfo.VENDEDOR
                });
            }
            return result;
        }
    }
}
