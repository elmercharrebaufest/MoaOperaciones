using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.Liquidacion.NoGranos;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.LiquidacionesNGWebServiceMOA;
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
    public class LiquidacionesNGConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserS4"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassS4"];
        SI_MPMF_MOAOP_LIQUIDACIONES_NGClient service = new SI_MPMF_MOAOP_LIQUIDACIONES_NGClient();

        public object request(string proveedor, List<FechaWS> fechas)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;
                    string fechahasta = SAPFormatter.PrepararFecha(DateTime.Now.Date);
                    if (fechas.FirstOrDefault() == null)
                    {
                        fechas.Add(new FechaWS
                        {
                            fechaFin = DateTime.Now.Date,
                            fechaInicio = DateTime.Now.Date
                        });
                    }

                    var request = new Z_MPMF_MOAOP_LIQUIDACIONES_NG()
                    {
                        PE_FECHA = fechahasta,
                        PE_PROVEEDOR = proveedor,
                        PE_SOCIEDAD = "MOA"
                    };
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_LIQUIDACIONES_NG request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MPMF_MOAOP_LIQUIDACIONES_NG(request);
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_LIQUIDACIONES_NG response");
                    Log.Info(response.ToXml());
                    return MapSinPI(response.MENSAJE_ERROR, response.T_OCOMPRA, response.T_SALIDA, fechas);
                        
                }
                else
                {
                    LiquidacionesNGWebServiceMOA.ZMPES6110[] compras = new LiquidacionesNGWebServiceMOA.ZMPES6110[] { };
                    LiquidacionesNGWebServiceMOA.ZMPES6100[] salidas = new LiquidacionesNGWebServiceMOA.ZMPES6100[] { };
                    string fechahasta = SAPFormatter.PrepararFecha(DateTime.Now.Date);

                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    if (fechas.FirstOrDefault() == null)
                    {
                        fechas.Add(new FechaWS
                        {
                            fechaFin = DateTime.Now.Date,
                            fechaInicio = DateTime.Now.Date
                        });
                    }
                    LiquidacionesNGWebServiceMOA.ZMPES4910 error = service.SI_MPMF_MOAOP_LIQUIDACIONES_NG(fechahasta, proveedor, "MOA", out compras, out salidas);
                    return Map(error, compras, salidas, fechas);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        protected virtual object Map(LiquidacionesNGWebServiceMOA.ZMPES4910 error, LiquidacionesNGWebServiceMOA.ZMPES6110[] compras, LiquidacionesNGWebServiceMOA.ZMPES6100[] salidas, List<FechaWS> fechas)
        {
            LiquidacionNGWSMOAResponse result = new LiquidacionNGWSMOAResponse();

            List<LiquidacionesNGWebServiceMOA.ZMPES6110> comprasList = compras.ToList();

            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }

            DateTime desde = fechas.FirstOrDefault().fechaInicio;
            DateTime hasta = fechas.FirstOrDefault().fechaFin;

            foreach (LiquidacionesNGWebServiceMOA.ZMPES6100 salida in salidas)
            {
                SalidaView salidaNew = new SalidaView()
                {
                    id = salida.ID,
                    comprobante = salida.COMPROBANTE,
                    fechaComprobanteDate = SAPFormatter.GetDateTime(salida.FECHA_DOC),
                    importeString = SAPFormatter.FormatearMonto(salida.IMPORTE, salida.MONEDA),
                    importe = salida.IMPORTE,
                    observaciones = salida.OBSERVACIONES,
                    tipo = salida.TIPO,
                    vencimiento = SAPFormatter.FormatearFecha(salida.VENCIMIENTO),
                    vencimientoDate = SAPFormatter.GetDateTime(salida.VENCIMIENTO),
                    compra = "",
                    fechaDocumento = SAPFormatter.FormatearFecha(salida.FECHA_DOC),
                    fechaDocFiltro = SAPFormatter.GetDateTime(salida.FECHA_DOC)
                };

                LiquidacionesNGWebServiceMOA.ZMPES6110 compra = comprasList.Find(c => c.ID == salidaNew.id);
                if (compra != null)
                {
                    salidaNew.compra = compra.OCOMPRA;
                }

                if (salidaNew.fechaDocFiltro >= desde && salidaNew.fechaDocFiltro <= hasta)
                {
                    result.liquidaciones.Add(salidaNew);
                }

            }


            return result;
        }
        protected virtual object MapSinPI(WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4910 error, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6110[] compras, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6100[] salidas, List<FechaWS> fechas)
        {
            LiquidacionNGWSMOAResponse result = new LiquidacionNGWSMOAResponse();

            List<WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6110> comprasList = compras.ToList();

            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }

            DateTime desde = fechas.FirstOrDefault().fechaInicio;
            DateTime hasta = fechas.FirstOrDefault().fechaFin;

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6100 salida in salidas)
            {
                SalidaView salidaNew = new SalidaView()
                {
                    id = salida.ID,
                    comprobante = salida.COMPROBANTE,
                    fechaComprobanteDate = SAPFormatter.GetDateTime(salida.FECHA_DOC),
                    importeString = SAPFormatter.FormatearMonto(salida.IMPORTE, salida.MONEDA),
                    importe = salida.IMPORTE,
                    observaciones = salida.OBSERVACIONES,
                    tipo = salida.TIPO,
                    vencimiento = SAPFormatter.FormatearFecha(salida.VENCIMIENTO),
                    vencimientoDate = SAPFormatter.GetDateTime(salida.VENCIMIENTO),
                    compra = "",
                    fechaDocumento = SAPFormatter.FormatearFecha(salida.FECHA_DOC),
                    fechaDocFiltro = SAPFormatter.GetDateTime(salida.FECHA_DOC)
                };

                WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6110 compra = comprasList.Find(c => c.ID == salidaNew.id);
                if (compra != null)
                {
                    salidaNew.compra = compra.OCOMPRA;
                }

                if (salidaNew.fechaDocFiltro >= desde && salidaNew.fechaDocFiltro <= hasta)
                {
                    result.liquidaciones.Add(salidaNew);
                }

            }


            return result;
        }

    }

    public class LiquidacionesExcelNGConsumerMOA : LiquidacionesNGConsumerMOA
    {
        protected override object Map(LiquidacionesNGWebServiceMOA.ZMPES4910 error, LiquidacionesNGWebServiceMOA.ZMPES6110[] compras, LiquidacionesNGWebServiceMOA.ZMPES6100[] salidas, List<FechaWS> fechas)
        {
            LiquidacionExcelNGWSMOAResponse result = new LiquidacionExcelNGWSMOAResponse();

            List<LiquidacionesNGWebServiceMOA.ZMPES6110> comprasList = compras.ToList();

            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }

            foreach (LiquidacionesNGWebServiceMOA.ZMPES6100 salida in salidas)
            {

                Salida salidaNew = new Salida()
                {
                    id = salida.ID,
                    comprobante = salida.COMPROBANTE,
                    moneda = salida.MONEDA,
                    importe = salida.IMPORTE,
                    observaciones = salida.OBSERVACIONES,
                    tipo = salida.TIPO,
                    compra = "",
                    vencimiento = SAPFormatter.FormatearFecha(salida.VENCIMIENTO),
                    fechaDocumento = SAPFormatter.FormatearFecha(salida.FECHA_DOC)
                };

                LiquidacionesNGWebServiceMOA.ZMPES6110 compra = comprasList.Find(c => c.ID == salidaNew.id);
                if (compra != null)
                {
                    salidaNew.compra = compra.OCOMPRA;
                }

                result.liquidaciones.Add(salidaNew);
            }

            return result;
        }
        protected override object MapSinPI(WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4910 error, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6110[] compras, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6100[] salidas, List<FechaWS> fechas)
        {
            LiquidacionExcelNGWSMOAResponse result = new LiquidacionExcelNGWSMOAResponse();

            List<WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6110> comprasList = compras.ToList();

            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6100 salida in salidas)
            {

                Salida salidaNew = new Salida()
                {
                    id = salida.ID,
                    comprobante = salida.COMPROBANTE,
                    moneda = salida.MONEDA,
                    importe = salida.IMPORTE,
                    observaciones = salida.OBSERVACIONES,
                    tipo = salida.TIPO,
                    compra = "",
                    vencimiento = SAPFormatter.FormatearFecha(salida.VENCIMIENTO),
                    fechaDocumento = SAPFormatter.FormatearFecha(salida.FECHA_DOC)
                };

                WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6110 compra = comprasList.Find(c => c.ID == salidaNew.id);
                if (compra != null)
                {
                    salidaNew.compra = compra.OCOMPRA;
                }

                result.liquidaciones.Add(salidaNew);
            }

            return result;
        }

    }
}
