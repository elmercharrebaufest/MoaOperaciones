using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.Echeq;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.EcheqVisualizarPendientePagoWebServiceMOA;
using SustitucionMOAWS.Logger;

namespace SustitucionMOAWS.WSConsumers
{
    public class EcheqVisualizarPendientePagoConsumerMOA : IEcheqVisualizarPendientePagoConsumerMOA
    {
        SI_MPRFC_VISU_PENDIENTE_PAGOClient service = new SI_MPRFC_VISU_PENDIENTE_PAGOClient();

        public EcheqVisualizarPendientePagoConsumerMOA()
        {
            service = new SI_MPRFC_VISU_PENDIENTE_PAGOClient();
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
        }

        public List<EcheqNegocioDto> Request(string proveedor, List<FechaWS> listaFechas, string contrato)
        {
            try
            {
                if (!string.IsNullOrEmpty(contrato))
                {
                    listaFechas.FirstOrDefault().fechaInicio = DateTime.Now.Date.AddYears(-10);
                }

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

                Log.Info($"SI_MPRFC_VISU_PENDIENTE_PAGO Request: {new { contrato, fechas, proveedor }}");
                var response = service.SI_MPRFC_VISU_PENDIENTE_PAGO(contrato ?? "", fechas, proveedor);
                Log.Info($"SI_MPRFC_VISU_PENDIENTE_PAGO Response: {response}");

                return Map(response);

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<EcheqNegocioDto> Map(ZMPES6900[] EX_SALIDA)
        {
            return EX_SALIDA.Select(pagosPendientes =>
                new EcheqNegocioDto()
                {
                    Contrato = pagosPendientes.CONTRATO,
                    Pedido = pagosPendientes.PEDIDO,
                    Kilos = pagosPendientes.KILOS,
                    //kILOSFieldSpecified = pagosPendientes.KILOSSpecified,
                    KilosPagados = pagosPendientes.KILOS_PAGADOS,
                    //kILOS_PAGADOSFieldSpecified = pagosPendientes.KILOS_PAGADOSSpecified,
                    Precio = pagosPendientes.PRECIO,
                    //pRECIOFieldSpecified = pagosPendientes.PRECIOSpecified,
                    Moneda = pagosPendientes.MONEDA,
                    MaterialCodigo = Int32.Parse(pagosPendientes.MATERIAL).ToString(),
                    DescripcionMaterial = pagosPendientes.DESC_MATERIAL,
                    Fecha = SAPFormatter.FormatearFecha(pagosPendientes.FECHA),
                    MarcaCheque = string.IsNullOrWhiteSpace(pagosPendientes.ZLSCH) ? false : true,
                    Clasificacion = pagosPendientes.CLASIFICACION,
                    TipoContrato = string.IsNullOrWhiteSpace(pagosPendientes.PEDIDO) ? "A Precio" : "Fijación",
                    Documentos = pagosPendientes.DOCUMENTOS == null ? new List<EcheqLiquidacionDto>() : pagosPendientes.DOCUMENTOS.Select(x => new EcheqLiquidacionDto
                    {
                        Contrato = x.CONTRATO,
                        Pedido = x.PEDIDO,
                        ImporteEnPesos = x.DMBTR,
                        //DMBTRSpecified = x.DMBTRSpecified,
                        Documento = x.DOCUMENTO,
                        Ejercicio = x.EJERCICIO,
                        Fecha = SAPFormatter.FormatearFecha(x.FECHA),
                        Moneda = x.MONEDA,
                        //Sociedad = x.SOCIEDAD,
                        Solapa = x.SOLAPA,
                        ImporteMonedaDocumento = x.WRBTR,
                        //WRBTRSpecified = x.WRBTRSpecified,
                        NumeroCOE = x.XBLNR,
                        //Clasificacion = x.CLASIFICACION
                        MarcaCheque = string.IsNullOrWhiteSpace(x.ZLSCH) ? false : true
                    }).ToList()
                }).ToList();
        }
    }

    public interface IEcheqVisualizarPendientePagoConsumerMOA
    {
        List<EcheqNegocioDto> Request(string proveedor, List<FechaWS> listaFechas, string contrato);
    }
}
