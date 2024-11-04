using System;
using System.Collections.Generic;
using System.Linq;
using SustitucionMOAAssets;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.ViewModel.Pago;
using SustitucionMOAModel.Models.WSMapMOA.Pago;
using SustitucionMOAModel.Models.WSMapMOA.Pago.Comprobante;
using SustitucionMOAModel.Models.WSMapMOA.Pago.Detalle;
using SustitucionMOAModel.Models.WSMapMOA.Pago.NoGranos;
using SustitucionMOAUtils.Export;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAValidator;
using SustitucionMOAWS.WSConsumers;

namespace SustitucionMOAUtils.Services
{
    public class PagoService : IPagoService
    {
        public PagoService()
        {

        }
        public PagoViewModel ObtenerEmitidos(string proveedor, string fechaInicio, string fechaFin)
        {
            return ObtenerPagos(proveedor, "EMITIDO", fechaInicio, fechaFin);
        }

        public PagoNGViewModel ObtenerEmitidosNG(string proveedor, string fechaInicio, string fechaFin, string sociedad)
        {
            return ObtenerPagosNG(proveedor, "EMITIDO", fechaInicio, fechaFin, sociedad);
        }

        public PagoViewModel ObtenerPagos(string proveedor, string tipo, string fechaInicio, string fechaFin)
        {
            try
            {
                List<FechaWS> fechas = CommonUtil.toDateList(fechaInicio, fechaFin);
                PagoViewModel dataView = new PagoViewModel();
                dataView.filtroID = new DropdownContent();
                dataView.filtroTitular = new DropdownContent();
                dataView.filtroContratoMolinos = new DropdownContent();
                dataView.filtroContratoProveedores = new DropdownContent();
                dataView.data = (PagosWSMOAReponse) new PagosConsumerMOA().request(proveedor, fechas);
                ValidarRespuesta(dataView.data);
                try
                {
                    dataView.filtroID = new DropdownContent(dataView.data.pagos.GroupBy(i => i.idPago).Select(x => new DropdownOption { value = x.Key, label = x.Key + " (" + x.Count() + ")" }).ToList());
                  
                }
                catch { }
                return dataView;
            }
            catch (InfoCustomException e)
            {
                throw e;
            }
            catch (ValidationCustomException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public PagoNGViewModel ObtenerPagosNG(string proveedor, string tipo, string fechaInicio, string fechaFin, string sociedad)
        {
            try
            {
                List<FechaWS> fechas = CommonUtil.toDateList(fechaInicio, fechaFin);
                PagoNGViewModel dataView = new PagoNGViewModel();
                dataView.filtroID = new DropdownContent();
                dataView.filtroTitular = new DropdownContent();
                dataView.filtroContratoMolinos = new DropdownContent();
                dataView.filtroContratoProveedores = new DropdownContent();
                dataView.data = (PagosNGWSMOAResponse) new PagosNGConsumerMOA().request(proveedor, fechas, sociedad);
                ValidarRespuesta(dataView.data);
                try
                {
                    dataView.filtroID = new DropdownContent(dataView.data.pagos.GroupBy(i => i.numeroPago).Select(x => new DropdownOption { value = x.Key, label = x.Key + " (" + x.Count() + ")" }).ToList());

                }
                catch { }
                dataView.data.pagos = dataView.data.pagos.OrderByDescending(p => p.fechaPagoDate).ToList();
                return dataView;
            }
            catch (InfoCustomException e)
            {
                throw e;
            }
            catch (ValidationCustomException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public PagoComprobanteWSMOAResponse ObtenerComprobantes(string documento, string fecha, string sociedad, string fiscalYear)
        {
            try
            {
                DateTime fechaDate = DataFormatter.StringToDateTime(fecha, "Fecha");
                PagoComprobanteWSMOAResponse data = new PagoComprobantesConsumerMOA().request(documento, fechaDate, sociedad, fiscalYear);
                ValidarRespuesta(data);
                return data;
            }
            catch (InfoCustomException e)
            {
                throw e;
            }
            catch (ValidationCustomException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public string DescargarEmitidos(string proveedor, string fechaInicio, string fechaFin)
        {
            try
            {
                List<FechaWS> fechas = CommonUtil.toDateList(fechaInicio, fechaFin);
                PagosExcelWSMOAReponse data = (PagosExcelWSMOAReponse)new PagosExcelConsumerMOA().request(proveedor, fechas);
                ValidarRespuesta(data);
                return ExcelExport.ToExcel(data.pagos, new string[] { "Proveedor", "Fecha de Acreditacion", "ID Pago", "Moneda", "Total Mercaderia", "IVA", "Retencion", "Monto", "Contrato Molinos", "Contrato Proveedor", "Fecha Pago", "Comprobante", "Tipo Comprobante", "Concepto"}, "Reporte Pagos Emitidos");
            }
            catch (InfoCustomException e)
            {
                throw e;
            }
            catch (ValidationCustomException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public string DescargarEmitidosNG(string proveedor, string fechaInicio, string fechaFin, string sociedad)
        {
            try
            {
                List<FechaWS> fechas = CommonUtil.toDateList(fechaInicio, fechaFin);
                PagosNGExcelWSMOAReponse data = (PagosNGExcelWSMOAReponse)new PagosExcelNGConsumerMOA().request(proveedor, fechas, sociedad);
                ValidarRespuesta(data);
                return ExcelExport.ToExcel(data.pagos, new string[] { "Fecha Pago", "Numero Pago", "Periodo Fiscal", "Via Pago", "Moneda", "Total Mercaderia", "Retencion",  "Monto" }, "Reporte Pagos Emitidos");
            }
            catch (InfoCustomException e)
            {
                throw e;
            }
            catch (ValidationCustomException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public PagoDetalleWSMOAResponse ObtenerDetalle(string proveedor, string numeroPago)
        {
            try
            {
                if (numeroPago == null || numeroPago == "")
                {
                    throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "Numero de Contrato"));
                }

                PagoDetalleWSMOAResponse response = (PagoDetalleWSMOAResponse) new PagoDetalleConsumerMOA().request(proveedor, numeroPago);
                if (response == null || response.cabeceras.Count < 1)
                {
                    throw new InfoCustomException(String.Format(InfoMsg.ElementoNoExiste, "Pago", numeroPago));
                }
                return response;
            }
            catch (InfoCustomException e)
            {
                throw e;
            }
            catch (ValidationCustomException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public string DescargarDetalle(string proveedor, string numeroPago)
        {
            try
            {
                PagoDetalleExcelWSMOAResponse data = (PagoDetalleExcelWSMOAResponse) new PagoDetalleExcelConsumerMOA().request(proveedor, numeroPago);

                return ExcelExport.ToExcelPagoDetalle(data.cabeceras, data.salidasExcelDetalle, data.vendedores,
                                                           new string[] { "Fecha", "Id Pago", "Bruto", "IVA", "Retenciones", "Neto", "Moneda" },
                                                           new string[] { "Contrato", "Tipo", "Total", "Total IVA", "Moneda", "Nro Legal", "Caract", "Bruto", "IVA", "Ret IVA", "Imp. IIBB", "Impuesto Ganancias", "CBU IVA", "Remanente IVA" },
                                                           new string[] { "Contrato", "Id Vendedor", "Vendedor"},
                                                           "Reporte Pago Detalle (Nro. " + numeroPago + ")");
            }
            catch (InfoCustomException e)
            {
                throw e;
            }
            catch (ValidationCustomException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }


        private void ValidarRespuesta(PagosWSMOAReponse data) {
            if(data == null)
                throw new ValidationCustomException(ErrorMsg.ErrorWS);
            if (data.error != null && data.error.codigo != null && data.error.codigo != "" && data.error.codigo != "06" && data.error.codigo != "00" && data.error.codigo != "01")
                throw new ValidationCustomException(data.error.descripcion != null && data.error.descripcion != "" ? data.error.descripcion : ErrorMsg.ErrorWS);
            if (data.pagos == null || data.pagos.Count == 0)
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Pagos"));
        }

        private void ValidarRespuesta(PagosExcelWSMOAReponse data)
        {
            if (data == null)
                throw new ValidationCustomException(ErrorMsg.ErrorWS);
            if (data.error != null && data.error.codigo != null && data.error.codigo != "" && data.error.codigo != "06" && data.error.codigo != "00" && data.error.codigo != "01")
                throw new ValidationCustomException(data.error.descripcion != null && data.error.descripcion != "" ? data.error.descripcion : ErrorMsg.ErrorWS);
            if (data.pagos == null || data.pagos.Count == 0)
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Pagos"));
        }

        private void ValidarRespuesta(PagosNGWSMOAResponse data)
        {
            if (data == null)
                throw new ValidationCustomException(ErrorMsg.ErrorWS);
            if (data.pagos == null || data.pagos.Count == 0)
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Pagos"));
        }

        private void ValidarRespuesta(PagosNGExcelWSMOAReponse data)
        {
            if (data == null)
                throw new ValidationCustomException(ErrorMsg.ErrorWS);
            if (data.pagos == null || data.pagos.Count == 0)
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Pagos"));
        }


        private void ValidarRespuesta(PagoComprobanteWSMOAResponse data) {
            if (data == null)
                throw new ValidationCustomException(ErrorMsg.ErrorWS);
            if (data.comprobantes == null || data.comprobantes.Count == 0)
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Comprobantes"));
        }
    }
}
