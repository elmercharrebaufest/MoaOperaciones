using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.CartaPorte;
using SustitucionMOAWS.AplicacionesWebServiceMOA;
using SustitucionMOAWS.CredentialService;

namespace SustitucionMOAWS.WSConsumers
{
    public class AplicacionesConsumerMOA
    {
        SI_MPMF_MOAOP_APLICACIONESClient service = new SI_MPMF_MOAOP_APLICACIONESClient();

        public object request(string proveedor, List<FechaWS> fechas, List<string> contratos, string CCPP)
        {
            try
            {
                CCPP = CCPP ?? "";
                contratos = contratos ?? new List<string>();
                ZMPES4050[] aplicaciones_out = new ZMPES4050[] { };
                ZMPES4060[] contratos_in = new ZMPES4060[] { };
                List<ZMPES4100> fechasSAP = new List<ZMPES4100>() { };

                foreach (FechaWS fecha in fechas)
                {
                    fechasSAP.Add(new ZMPES4100()
                    {
                        FECHA_OP = SAPFormatter.PrepararFecha(fecha.fechaInicio),
                        FECHA_OP_HASTA = SAPFormatter.PrepararFecha(fecha.fechaFin)
                    });
                }

                contratos_in = contratos.Select(x => new ZMPES4060 { CONTRATO = x }).ToArray();
                ZMPES4100[] fechasSAPArray = fechasSAP.ToArray();

                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                ZMPES4910 error = service.SI_MPMF_MOAOP_APLICACIONES(CCPP, proveedor, ref aplicaciones_out, ref contratos_in, ref fechasSAPArray);
                return map(error, aplicaciones_out);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        protected virtual object map(ZMPES4910 error, ZMPES4050[] aplicaciones_out)
        {
            CartaPorteWSMOAResponse result = new CartaPorteWSMOAResponse();

            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }
            foreach (ZMPES4050 aplicacionInfo in aplicaciones_out)
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

        protected override object map(ZMPES4910 error, ZMPES4050[] aplicaciones_out)
        {
            CartaPorteExcelWSMOAResponse result = new CartaPorteExcelWSMOAResponse();

            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }

            foreach (ZMPES4050 aplicacionInfo in aplicaciones_out)
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
