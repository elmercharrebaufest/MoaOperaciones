using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.Liquidacion.NoGranos;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.LiquidacionesNGWebServiceMOA;

namespace SustitucionMOAWS.WSConsumers
{
    public class LiquidacionesNGConsumerMOA
    {
        SI_MPMF_MOAOP_LIQUIDACIONES_NGClient service = new SI_MPMF_MOAOP_LIQUIDACIONES_NGClient();

        public object request(string proveedor, List<FechaWS> fechas)
        {
            try
            {
                ZMPES6110[] compras = new ZMPES6110[] { };
                ZMPES6100[] salidas = new ZMPES6100[] { };
                string fechahasta = SAPFormatter.PrepararFecha(DateTime.Now.Date);

                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                if (fechas.FirstOrDefault() == null)
                {
                    fechas.Add(new FechaWS { 
                        fechaFin = DateTime.Now.Date,
                        fechaInicio = DateTime.Now.Date
                    });
                }
                //fechahasta = SAPFormatter.PrepararFecha(fechas.FirstOrDefault().fechaFin);
                ZMPES4910 error = service.SI_MPMF_MOAOP_LIQUIDACIONES_NG(fechahasta, proveedor, "MOA", out compras, out salidas);
                return map(error, compras, salidas, fechas);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        protected virtual object map(ZMPES4910 error, ZMPES6110[] compras, ZMPES6100[] salidas, List<FechaWS> fechas)
        {
            LiquidacionNGWSMOAResponse result = new LiquidacionNGWSMOAResponse();

            List<ZMPES6110> comprasList = compras.ToList();

            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }

            DateTime desde = fechas.FirstOrDefault().fechaInicio;
            DateTime hasta = fechas.FirstOrDefault().fechaFin;

            foreach (ZMPES6100 salida in salidas)
            {
                SalidaView salidaNew = new SalidaView()
                {
                    id = salida.ID,
                    comprobante = salida.COMPROBANTE,
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

                ZMPES6110 compra = comprasList.Find(c => c.ID == salidaNew.id);
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

        protected override object map(ZMPES4910 error, ZMPES6110[] compras, ZMPES6100[] salidas, List<FechaWS> fechas)
        {
            LiquidacionExcelNGWSMOAResponse result = new LiquidacionExcelNGWSMOAResponse();

            List<ZMPES6110> comprasList = compras.ToList();

            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }

            foreach (ZMPES6100 salida in salidas)
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

                ZMPES6110 compra = comprasList.Find(c => c.ID == salidaNew.id);
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
