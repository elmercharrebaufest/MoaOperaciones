using SustitucionMOAFotmatter;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.CartaPorte;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.RecepcionesWebServiceMOA;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MOAOP;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace SustitucionMOAWS.WSConsumers
{
    public class RecepcionesConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserSinPI"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassSinPI"];


        public object request(string proveedor, List<FechaWS> fechas, List<string> cartaPortes)
        {
            try
            {

                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    cartaPortes = cartaPortes ?? new List<string>();

                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4110[] carta_porte_in = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4110[] { };
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4130[] centros_in = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4130[] { };
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4090[] materiales_in = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4090[] { };
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4990[] recepciones_out = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4990[] { };
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4080[] vendedores_in = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4080[] { };
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES7000[] calidades_out = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES7000[] { };
                    List<WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100> fechasSAP = new List<WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100>() { };
                    foreach (FechaWS fecha in fechas)
                    {
                        fechasSAP.Add(new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100()
                        {
                            FECHA_OP = SAPFormatter.PrepararFecha(fecha.fechaInicio),
                            FECHA_OP_HASTA = SAPFormatter.PrepararFecha(fecha.fechaFin)
                        });
                    }

                    carta_porte_in = cartaPortes.Select(x => new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4110 { CARTA_PORTE = x }).ToArray();

                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100[] fechasSAPArray = fechasSAP.ToArray();


                    var request = new Z_MPMF_MOAOP_RECEPCIONES()
                    {
                        T_CARTA_PORTE_IN = carta_porte_in,
                        T_CENTRO_IN = centros_in,
                        T_MATERIAL_IN = materiales_in,
                        T_RECEPCIONES_OUT = recepciones_out,
                        T_VENDEDOR_IN = vendedores_in,
                        PE_PROVEEDOR = proveedor,
                        T_FECHA_DESCARGA_IN = fechasSAPArray
                    };
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_RECEPCIONES request");
                    Log.Info(request.ToXml());

                    var response = agent.Z_MPMF_MOAOP_RECEPCIONES(request);
                    SapLogHelper.LogResponse(response.ToXml(), "Z_MPMF_MOAOP_RECEPCIONES");

                    return MapSinPI(response);

                }
                else
                {
                    SI_MPMF_MOAOP_RECEPCIONESClient service = new SI_MPMF_MOAOP_RECEPCIONESClient();

                    cartaPortes = cartaPortes ?? new List<string>();

                    RecepcionesWebServiceMOA.ZMPES4110[] carta_porte_in = new RecepcionesWebServiceMOA.ZMPES4110[] { };
                    RecepcionesWebServiceMOA.ZMPES4130[] centros_in = new RecepcionesWebServiceMOA.ZMPES4130[] { };
                    RecepcionesWebServiceMOA.ZMPES4090[] materiales_in = new RecepcionesWebServiceMOA.ZMPES4090[] { };
                    RecepcionesWebServiceMOA.ZMPES4990[] recepciones_out = new RecepcionesWebServiceMOA.ZMPES4990[] { };
                    RecepcionesWebServiceMOA.ZMPES4080[] vendedores_in = new RecepcionesWebServiceMOA.ZMPES4080[] { };
                    RecepcionesWebServiceMOA.ZMPES7000[] calidades_out = new RecepcionesWebServiceMOA.ZMPES7000[] { };
                    List<RecepcionesWebServiceMOA.ZMPES4100> fechasSAP = new List<RecepcionesWebServiceMOA.ZMPES4100>() { };
                    foreach (FechaWS fecha in fechas)
                    {
                        fechasSAP.Add(new RecepcionesWebServiceMOA.ZMPES4100()
                        {
                            FECHA_OP = SAPFormatter.PrepararFecha(fecha.fechaInicio),
                            FECHA_OP_HASTA = SAPFormatter.PrepararFecha(fecha.fechaFin)
                        });
                    }

                    carta_porte_in = cartaPortes.Select(x => new RecepcionesWebServiceMOA.ZMPES4110 { CARTA_PORTE = x }).ToArray();

                    RecepcionesWebServiceMOA.ZMPES4100[] fechasSAPArray = fechasSAP.ToArray();
                    //ZmprfcGolRecepciones requestInfo = new ZmprfcGolRecepciones { PeProveedor = proveedor, TFechaDescargaIn = fechasSAP.ToArray() };
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    RecepcionesWebServiceMOA.ZMPES4910 error = service.SI_MPMF_MOAOP_RECEPCIONES(
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

        protected virtual object map(RecepcionesWebServiceMOA.ZMPES4910 error, RecepcionesWebServiceMOA.ZMPES4990[] recepciones_out)
        {
            CartaPorteDescargaWSMOAResponse result = new CartaPorteDescargaWSMOAResponse();

            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }

            foreach (RecepcionesWebServiceMOA.ZMPES4990 recepcionInfo in recepciones_out)
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
        protected virtual object MapSinPI(Z_MPMF_MOAOP_RECEPCIONESResponse response)
        {
            CartaPorteDescargaWSMOAResponse result = new CartaPorteDescargaWSMOAResponse();

            if (response.MENSAJE_ERROR != null)
            {
                result.error.codigo = response.MENSAJE_ERROR.CODIGO;
                result.error.descripcion = response.MENSAJE_ERROR.DESCRIPCION;
                result.error.tipo = response.MENSAJE_ERROR.TIPO;
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4990 recepcionInfo in response.T_RECEPCIONES_OUT)
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
        protected override object map(RecepcionesWebServiceMOA.ZMPES4910 error, RecepcionesWebServiceMOA.ZMPES4990[] aplicaciones_out)
        {
            CartaPorteDescargaExcelWSMOAResponse result = new CartaPorteDescargaExcelWSMOAResponse();

            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }

            foreach (RecepcionesWebServiceMOA.ZMPES4990 aplicacionInfo in aplicaciones_out)
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

        protected override object MapSinPI(Z_MPMF_MOAOP_RECEPCIONESResponse response)
        {
            CartaPorteDescargaExcelWSMOAResponse result = new CartaPorteDescargaExcelWSMOAResponse();

            if (response.MENSAJE_ERROR != null)
            {
                result.error.codigo = response.MENSAJE_ERROR.CODIGO;
                result.error.descripcion = response.MENSAJE_ERROR.DESCRIPCION;
                result.error.tipo = response.MENSAJE_ERROR.TIPO;
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4990 aplicacionInfo in response.T_RECEPCIONES_OUT)
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
