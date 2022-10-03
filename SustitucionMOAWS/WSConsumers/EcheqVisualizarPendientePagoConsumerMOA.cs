using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.Echeq;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.EcheqVisualizarPendientePagoWebServiceMOA;

namespace SustitucionMOAWS.WSConsumers
{
    public class echeqVisualizarPendientePagoConsumerMOA : IEcheqVisualizarPendientePagoConsumerMOA
    {
        SI_MPRFC_VISU_PENDIENTE_PAGOClient service = new SI_MPRFC_VISU_PENDIENTE_PAGOClient();

        public echeqVisualizarPendientePagoConsumerMOA()
        {
            service = new SI_MPRFC_VISU_PENDIENTE_PAGOClient();
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
        }

        public List<EcheqVisualizacionPendientePago> Request(string proveedor, List<FechaWS> listaFechas)
        {
            try
            {
                ZMPES4100[] fechas = new ZMPES4100[] { };

                if (listaFechas.FirstOrDefault() != null)
                {
                    fechas = new ZMPES4100[] {
                        new ZMPES4100 {
                            FECHA_OP = SAPFormatter.PrepararFecha(listaFechas.FirstOrDefault().fechaInicio),
                            FECHA_OP_HASTA = SAPFormatter.PrepararFecha(listaFechas.FirstOrDefault().fechaFin)
                        }
                    };
                }

                string fechahasta = "";

                if (listaFechas.FirstOrDefault() != null)
                {
                    fechahasta = SAPFormatter.PrepararFecha(listaFechas.FirstOrDefault().fechaFin);
                }

                string IM_CONTRATO = "";


                var response = service.SI_MPRFC_VISU_PENDIENTE_PAGO(IM_CONTRATO, fechas, proveedor);
                return Map(response);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<EcheqVisualizacionPendientePago> Map(ZMPES6900[] EX_SALIDA)
        {
            List<EcheqVisualizacionPendientePago> listaPendientesPago = new List<EcheqVisualizacionPendientePago>() { };


            foreach (ZMPES6900 pagosPendientes in EX_SALIDA)
            {
                listaPendientesPago.Add(new EcheqVisualizacionPendientePago()
                {
                    Contrato = pagosPendientes.CONTRATO,
                    Pedido = pagosPendientes.PEDIDO,
                    Kilos = pagosPendientes.KILOS,
                    kILOSFieldSpecified = pagosPendientes.KILOSSpecified,
                    KilosPagados = pagosPendientes.KILOS_PAGADOS,
                    kILOS_PAGADOSFieldSpecified = pagosPendientes.KILOS_PAGADOSSpecified,
                    Precio = pagosPendientes.PRECIO,
                    pRECIOFieldSpecified = pagosPendientes.PRECIOSpecified,
                    Moneda = pagosPendientes.MONEDA,
                    Material = pagosPendientes.MATERIAL,
                    DescripcionMaterial = pagosPendientes.DESC_MATERIAL,
                    Fecha = SAPFormatter.FormatearFecha(pagosPendientes.FECHA),
                    zLSCHField = pagosPendientes.ZLSCH,
                    Documentos = pagosPendientes.DOCUMENTOS == null ? new List<EcheqDocumento>() : pagosPendientes.DOCUMENTOS.Select(x => new EcheqDocumento
                    {
                        Contrato = x.CONTRATO,
                        Pedido = x.PEDIDO,
                        ImporteEnPesos = x.DMBTR,
                        DMBTRSpecified = x.DMBTRSpecified,
                        Documento = x.DOCUMENTO,
                        Ejercicio = x.EJERCICIO,
                        Fecha = x.FECHA,
                        Moneda = x.MONEDA,
                        Sociedad = x.SOCIEDAD,
                        Solapa = x.SOLAPA,
                        ImporteMonedaDocumento = x.WRBTR,
                        WRBTRSpecified = x.WRBTRSpecified,
                        NumeroCOE = x.XBLNR
                    }).ToList()
                });
            }
            return listaPendientesPago;
        }
    }

    public interface IEcheqVisualizarPendientePagoConsumerMOA
    {
        List<EcheqVisualizacionPendientePago> Request(string proveedor, List<FechaWS> listaFechas);
    }
}
