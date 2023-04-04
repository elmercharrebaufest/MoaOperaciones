using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAAssets;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.CartaPorte;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.RecepcionesWebServiceMOA;

namespace SustitucionMOAWS.WSConsumers
{
    public class RecepcionesConsumerMOA
    {
        SI_MPMF_MOAOP_RECEPCIONESClient service = new SI_MPMF_MOAOP_RECEPCIONESClient();

        public object request(string proveedor, List<FechaWS> fechas, List<string> cartaPortes)
        {
            try
            {
                cartaPortes = cartaPortes ?? new List<string>();

                ZMPES4110[] carta_porte_in = new ZMPES4110[] { };
                ZMPES4130[] centros_in = new ZMPES4130[] { };
                ZMPES4090[] materiales_in = new ZMPES4090[] { };
                ZMPES4990[] recepciones_out = new ZMPES4990[] { };
                ZMPES4080[] vendedores_in = new ZMPES4080[] { };
                ZMPES7000[] calidades_out = new ZMPES7000[] { };
                List<ZMPES4100> fechasSAP = new List<ZMPES4100>() { };
                foreach (FechaWS fecha in fechas)
                {
                    fechasSAP.Add(new ZMPES4100()
                    {
                        FECHA_OP = SAPFormatter.PrepararFecha(fecha.fechaInicio),
                        FECHA_OP_HASTA = SAPFormatter.PrepararFecha(fecha.fechaFin)
                    });
                }

                carta_porte_in = cartaPortes.Select(x => new ZMPES4110 { CARTA_PORTE = x }).ToArray();

                ZMPES4100[] fechasSAPArray = fechasSAP.ToArray();
                //ZmprfcGolRecepciones requestInfo = new ZmprfcGolRecepciones { PeProveedor = proveedor, TFechaDescargaIn = fechasSAP.ToArray() };
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                ZMPES4910 error = service.SI_MPMF_MOAOP_RECEPCIONES(
                   proveedor,
                   ref carta_porte_in,
                   ref centros_in,
                   ref fechasSAPArray,
                   ref materiales_in,
                   ref recepciones_out,
                   ref vendedores_in,
                   out calidades_out); 
                return map(error, recepciones_out);
            }
            catch (InfoCustomException e)
            {
                throw new InfoCustomException(e.Message, e);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        protected virtual object map(ZMPES4910 error, ZMPES4990[] recepciones_out)
        {
            CartaPorteDescargaWSMOAResponse result = new CartaPorteDescargaWSMOAResponse();

            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }

            foreach (ZMPES4990 recepcionInfo in recepciones_out)
            {
                result.cartasPorte.Add(new CartaPorteDescargaView()
                {
                    fechaDescarga = SAPFormatter.FormatearFecha(recepcionInfo.FECHA_DESCARGA),
                    fechaDescargaDate = SAPFormatter.GetDateTime(recepcionInfo.FECHA_DESCARGA),
                    cartaPorte = recepcionInfo.CARTA_PORTE,
                    producto = recepcionInfo.PRODUCTO,
                    netoDescontado = recepcionInfo.NETO_DESCONTADO,
                    netoDescontadoString = SAPFormatter.FormatearCantidad(recepcionInfo.NETO_DESCONTADO, recepcionInfo.UNIME_NETO),
                    sust = recepcionInfo.SUST,
                    titular = recepcionInfo.TITULAR,
                    descripcionTitular = recepcionInfo.DESC_TITULAR,
                    vendedor = recepcionInfo.VENDEDOR,
                    vendedorId = recepcionInfo.ID_VENDEDOR,
                    contrnum = recepcionInfo.CONTRNUM,
                    cg = recepcionInfo.CG,
                    pesoBrutoOrigen = recepcionInfo.BRUTO_ORIGEN,
                    taraOrigen = recepcionInfo.TARA_ORIGEN,
                    netoOrigen = recepcionInfo.NETO_ORIGEN,
                    brutoDestino = recepcionInfo.BRUTO,
                    taraDestino = recepcionInfo.TARA,
                    netoDestino = recepcionInfo.NETO,
                    mermas = recepcionInfo.MERMAS,
                });
            }

            return result;
        }
    }

    public class RecepcionesExcelConsumerMOA : RecepcionesConsumerMOA
    {
        protected override object map(ZMPES4910 error, ZMPES4990[] aplicaciones_out)
        {
            CartaPorteDescargaExcelWSMOAResponse result = new CartaPorteDescargaExcelWSMOAResponse();

            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }

            foreach (ZMPES4990 aplicacionInfo in aplicaciones_out)
            {
                result.cartasPorte.Add(new CartaPorteDescarga()
                {
                    fechaDescarga = SAPFormatter.FormatearFecha(aplicacionInfo.FECHA_DESCARGA),
                    cartaPorte = aplicacionInfo.CARTA_PORTE,
                    producto = aplicacionInfo.PRODUCTO,
                    unidadNetoDescontado = aplicacionInfo.UNIME_NETO,
                    netoDescontado = aplicacionInfo.NETO_DESCONTADO,
                    netoDestino = aplicacionInfo.NETO,
                    taraDestino = aplicacionInfo.TARA,
                    brutoDestino = aplicacionInfo.BRUTO,
                    netoOrigen = aplicacionInfo.NETO_ORIGEN,
                    taraOrigen = aplicacionInfo.TARA_ORIGEN,
                    pesoBrutoOrigen = aplicacionInfo.BRUTO_ORIGEN,
                    sust = aplicacionInfo.SUST,
                    titular = aplicacionInfo.TITULAR,
                    descripcionTitular = aplicacionInfo.DESC_TITULAR,
                    vendedor = aplicacionInfo.VENDEDOR,
                    vendedorId = aplicacionInfo.ID_VENDEDOR,
                    contrnum = aplicacionInfo.CONTRNUM,
                    cg = aplicacionInfo.CG,
                    mermas = aplicacionInfo.MERMAS,
                });
            }
            return result;
        }
    }
}
