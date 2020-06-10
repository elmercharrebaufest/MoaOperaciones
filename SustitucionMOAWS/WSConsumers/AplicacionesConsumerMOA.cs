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

        public object request(string proveedor, List<FechaWS> fechas)
        {
            try
            {
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
                ZMPES4100[] fechasSAPArray = fechasSAP.ToArray();

                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                ZMPES4910 error = service.SI_MPMF_MOAOP_APLICACIONES(proveedor, ref aplicaciones_out, ref contratos_in, ref fechasSAPArray);
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
                    FechaDescarga = SAPFormatter.FormatearFecha(aplicacionInfo.FECHA_DESCARGA),
                    FechaDescargaDate = SAPFormatter.GetDateTime(aplicacionInfo.FECHA_DESCARGA),
                    cartaPorte = aplicacionInfo.CARTA_PORTE,
                    Producto = aplicacionInfo.PRODUCTO,
                    NetoDescontado = aplicacionInfo.NETO_DESCONTADO,
                    NetoDescontadoString = SAPFormatter.FormatearCantidad(aplicacionInfo.NETO_DESCONTADO, aplicacionInfo.UNIME_NETO),
                    PendAplicacion = aplicacionInfo.PEND_APLICACION,
                    PendAplicacionString = SAPFormatter.FormatearCantidad(aplicacionInfo.PEND_APLICACION, aplicacionInfo.UNIME_APLIC),
                    ALiquidar = aplicacionInfo.A_LIQUIDAR,
                    ALiquidarString = SAPFormatter.FormatearCantidad(aplicacionInfo.A_LIQUIDAR, aplicacionInfo.UNIME_A_LIQUIDAR),
                    Contrnum = aplicacionInfo.CONTRNUM,
                    Contrvend = aplicacionInfo.CONTRVEND,
                    IdVendedor = aplicacionInfo.ID_VENDEDOR,
                    Vendedor = aplicacionInfo.VENDEDOR
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
                    FechaDescarga = SAPFormatter.FormatearFecha(aplicacionInfo.FECHA_DESCARGA),
                    cartaPorte = aplicacionInfo.CARTA_PORTE,
                    Producto = aplicacionInfo.PRODUCTO,
                    UnidadNetoDescontado = aplicacionInfo.UNIME_NETO,
                    NetoDescontado = aplicacionInfo.NETO_DESCONTADO,
                    UnidadPendAplicacion = aplicacionInfo.UNIME_APLIC,
                    PendAplicacion = aplicacionInfo.PEND_APLICACION,
                    UnidadALiquidar = aplicacionInfo.UNIME_A_LIQUIDAR,
                    ALiquidar = aplicacionInfo.A_LIQUIDAR,
                    Contrnum = aplicacionInfo.CONTRNUM,
                    Contrvend = aplicacionInfo.CONTRVEND,
                    IdVendedor = aplicacionInfo.ID_VENDEDOR,
                    Vendedor = aplicacionInfo.VENDEDOR
                });
            }
            return result;
        }
    }
}
