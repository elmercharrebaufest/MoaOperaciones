using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAAssets;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.ViewModel.Liquidacion;
using SustitucionMOAModel.Models.WSMapMOA.Liquidacion;
using SustitucionMOAModel.Models.WSMapMOA.Liquidacion.NoGranos;
using SustitucionMOAModel.Models.WSMapMOA.PDF;
using SustitucionMOAModel.Models.WSMapMOA.Proforma;
using SustitucionMOAModel.Models.WSMapMOA.Vincula.Detalle;
using SustitucionMOAUtils.Export;
using SustitucionMOAValidator;
using SustitucionMOAWS.WSConsumers;

namespace SustitucionMOAUtils.Services
{
    public class LiquidacionService
    {
        public LiquidacionViewModel getAprobadas(string proveedor, string fechaInicio, string fechaFin)
        {
            return getLiquidaciones(proveedor, "APROBADA", fechaInicio, fechaFin);
        }

        public LiquidacionViewModel getObservadas(string proveedor, string fechaInicio, string fechaFin)
        {
            return getLiquidaciones(proveedor, "OBSERVADA", fechaInicio, fechaFin);
        }

        public LiquidacionViewModel getPagas(string proveedor, string fechaInicio, string fechaFin)
        {
            return getLiquidaciones(proveedor, "PAGA", fechaInicio, fechaFin);
        }

        public LiquidacionViewModel getLiquidaciones(string proveedor, string tipo, string fechaInicio, string fechaFin) {
            try
            {
                List<FechaWS> fechas = CommonService.toDateList(fechaInicio, fechaFin);
                LiquidacionViewModel dataView = new LiquidacionViewModel();
                dataView.filtroProducto = new DropdownContent();
                dataView.filtroObservacion = new DropdownContent();
                dataView.data = (LiquidacionWSMOAResponse) new LiquidacionesConsumerMOA().request(proveedor, fechas);
                validarRespuesta(dataView.data);
                switch (tipo) {
                    case "APROBADA":
                        dataView.data.liquidaciones = dataView.data.liquidaciones.Where(x => x.solapa == "A").ToList();
                        break;
                    case "OBSERVADA":
                        dataView.data.liquidaciones = dataView.data.liquidaciones.Where(x => x.solapa == "O").ToList();
                        break;
                    case "PAGA":
                        dataView.data.liquidaciones = dataView.data.liquidaciones.Where(x => x.solapa == "P").ToList();
                        break;
                }
                if (dataView.data.liquidaciones.Count == 0) // <- Repito consulta por el filtrado que se realiza antes
                    throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Liquidaciones"));
                try
                {
                    dataView.filtroProducto = new DropdownContent(dataView.data.liquidaciones.GroupBy(i => i.producto).Select(x => new DropdownOption { value = x.Key, label = x.Key + " (" + x.Count() + ")" }).ToList());
                    dataView.filtroObservacion = new DropdownContent(dataView.data.liquidaciones.GroupBy(i => i.observaciones).Select(x => new DropdownOption { value = x.Key, label = x.Key + " (" + x.Count() + ")" }).ToList());
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

        public LiquidacionNGViewModel getAprobadasNG(string proveedor, string fechaInicio, string fechaFin)
        {
            return getLiquidacionesNG(proveedor, "APROBADA", fechaInicio, fechaFin);
        }

        public LiquidacionNGViewModel getObservadasNG(string proveedor, string fechaInicio, string fechaFin)
        {
            return getLiquidacionesNG(proveedor, "OBSERVADA", fechaInicio, fechaFin);
        }

        public LiquidacionNGViewModel getPagasNG(string proveedor, string fechaInicio, string fechaFin)
        {
            return getLiquidacionesNG(proveedor, "PAGA", fechaInicio, fechaFin);
        }

        public LiquidacionNGViewModel getLiquidacionesNG(string proveedor, string tipo, string fechaInicio, string fechaFin)
        {
            try
            {
                List<FechaWS> fechas = CommonService.toDateList(fechaInicio, fechaFin);
                LiquidacionNGViewModel dataView = new LiquidacionNGViewModel();
                dataView.filtroObservacion = new DropdownContent();
                dataView.data = (LiquidacionNGWSMOAResponse) new LiquidacionesNGConsumerMOA().request(proveedor, fechas);
                validarRespuestaNG(dataView.data);
                switch (tipo)
                {
                    case "APROBADA":
                        dataView.data.liquidaciones = dataView.data.liquidaciones.Where(x => x.observaciones == "").ToList();
                        break;
                    case "OBSERVADA":
                        dataView.data.liquidaciones = dataView.data.liquidaciones.Where(x => x.observaciones != "").ToList();
                        break;
                    case "PAGA":
                        break;
                }
                if (dataView.data.liquidaciones.Count == 0) // <- Repito consulta por el filtrado que se realiza antes
                    throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Comprobantes"));
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

        public string downloadAprobadas(string proveedor, string fechaInicio, string fechaFin)
        {
            try
            {
                List<FechaWS> fechas = CommonService.toDateList(fechaInicio, fechaFin);
                LiquidacionExcelWSMOAResponse data = (LiquidacionExcelWSMOAResponse) new LiquidacionesExcelConsumerMOA().request(proveedor, fechas);
                validarRespuesta(data);
                data.liquidaciones = data.liquidaciones.Where(x => x.solapa == "A").ToList();
                if (data.liquidaciones.Count == 0)
                    throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Liquidaciones"));
                return ExcelExport.ToExcel(data.liquidaciones, new string[] { "Vencimiento", "Tipo", "Comprobante", "Producto", "Liquidacion", "Unidad Liquidado", "Total", "Iva", "Moneda", "Contrato", "Observacion", "Secuencia", "Estado"}, "Reporte Liquidaciones Aprobadas");
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

        public string downloadObservadas(string proveedor, string fechaInicio, string fechaFin)
        {
            try
            {
                List<FechaWS> fechas = CommonService.toDateList(fechaInicio, fechaFin);
                LiquidacionExcelWSMOAResponse data = (LiquidacionExcelWSMOAResponse)new LiquidacionesExcelConsumerMOA().request(proveedor, fechas);
                validarRespuesta(data);
                data.liquidaciones = data.liquidaciones.Where(x => x.solapa == "O").ToList();
                if (data.liquidaciones.Count == 0)
                    throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Liquidaciones"));
                return ExcelExport.ToExcel(data.liquidaciones, new string[] { "Vencimiento", "Tipo", "Comprobante", "Producto", "Liquidacion", "Unidad Liquidado", "Total", "Iva", "Moneda", "Contrato", "Observacion", "Secuencia", "Estado" }, "Reporte Liquidaciones Observadas");
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

        public string downloadPagas(string proveedor, string fechaInicio, string fechaFin)
        {
            try
            {
                List<FechaWS> fechas = CommonService.toDateList(fechaInicio, fechaFin);
                LiquidacionExcelWSMOAResponse data = (LiquidacionExcelWSMOAResponse)new LiquidacionesExcelConsumerMOA().request(proveedor, fechas);
                validarRespuesta(data);
                data.liquidaciones = data.liquidaciones.Where(x => x.solapa == "P").ToList();
                if (data.liquidaciones.Count == 0)
                    throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Liquidaciones"));
                return ExcelExport.ToExcel(data.liquidaciones, new string[] { "Vencimiento", "Tipo", "Comprobante", "Producto", "Liquidacion", "Unidad Liquidado", "Total", "Iva", "Moneda", "Contrato", "Observacion", "Secuencia", "Estado" }, "Reporte Liquidaciones Pagas");
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

        public string downloadAprobadasNG(string proveedor, string fechaInicio, string fechaFin)
        {
            try
            {
                List<FechaWS> fechas = CommonService.toDateList(fechaInicio, fechaFin);
                LiquidacionExcelNGWSMOAResponse data = (LiquidacionExcelNGWSMOAResponse)new LiquidacionesExcelNGConsumerMOA().request(proveedor, fechas);
                validarRespuestaNG(data);
                data.liquidaciones = data.liquidaciones.Where(x => x.observaciones == "").ToList();
                if (data.liquidaciones.Count == 0)
                    throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Comprobantes"));
                return ExcelExport.ToExcel(data.liquidaciones, new string[] { "ID", "Vencimiento", "Tipo", "Comprobante", "Total", "Moneda", "Orden de Compra", "Observacion" }, "Reporte Comprobantes Aprobados");
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

        public string downloadObservadasNG(string proveedor, string fechaInicio, string fechaFin)
        {
            try
            {
                List<FechaWS> fechas = CommonService.toDateList(fechaInicio, fechaFin);
                LiquidacionExcelNGWSMOAResponse data = (LiquidacionExcelNGWSMOAResponse)new LiquidacionesExcelNGConsumerMOA().request(proveedor, fechas);
                validarRespuestaNG(data);
                data.liquidaciones = data.liquidaciones.Where(x => x.observaciones != "").ToList();
                if (data.liquidaciones.Count == 0)
                    throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Comprobantes"));
                return ExcelExport.ToExcel(data.liquidaciones, new string[] { "ID", "Vencimiento", "Tipo", "Comprobante", "Total", "Moneda", "Orden de Compra", "Observacion" }, "Reporte Comprobantes Observados");
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

        public string downloadPagasNG(string proveedor, string fechaInicio, string fechaFin)
        {
            try
            {
                List<FechaWS> fechas = CommonService.toDateList(fechaInicio, fechaFin);
                LiquidacionExcelNGWSMOAResponse data = (LiquidacionExcelNGWSMOAResponse)new LiquidacionesExcelNGConsumerMOA().request(proveedor, fechas);
                validarRespuestaNG(data);
                return ExcelExport.ToExcel(data.liquidaciones, new string[] { "ID", "Vencimiento", "Tipo", "Comprobante", "Total", "Moneda", "Orden de Compra", "Observacion" }, "Reporte Comprobantes Pagos");
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

        public VinculaDetalleWSMOAResponse getVinculacion(string proveedor, string contrato, string secuencia) {
            try
            {
                VinculaDetalleWSMOAResponse result = (VinculaDetalleWSMOAResponse) new VinculaDetalleConsumerMOA().request(proveedor, contrato, secuencia);
                validarRespuestaVinculacion(result);
                result.error = null;
                return result;
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

        public string descargaVinculacion(string proveedor, string contrato, string secuencia)
        {

            try
            {
                VinculaDetalleExcelWSMOAResponse data = (VinculaDetalleExcelWSMOAResponse)new VinculaDetalleExcelConsumerMOA().request(proveedor, contrato, secuencia);
                validarRespuestaDescargaVinculacion(data);
                return ExcelExport.ToExcel(data.data, new string[] { "Carta Porte", "Fecha", "Recibidos", "Liquidados", "Unidad" }, "Reporte Vinculacion (" + contrato + "-" + secuencia + ")");
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

        public DetalleCteWSMOAResponse getProforma(string proveedor, string fijacion)
        {
            try
            {
                DetalleCteWSMOAResponse data = (DetalleCteWSMOAResponse) new DetalleCteConsumerMOA().request(fijacion, proveedor);
                validarRespuestaProforma(data);
                data.error = null;
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

        public string descargaProforma(string proveedor, string fijacion)
        {
            try
            {
                DetalleCteExcelWSMOAResponse data = (DetalleCteExcelWSMOAResponse)new DetalleCteExcelConsumerMOA().request(fijacion, proveedor);
                validarRespuestaDescargaProforma(data);
                data.salidas.Add(data.subTotal);
                data.salidas.Add(data.pagoACuenta);
                data.salidas.Add(data.saldoAPagar);
                return ExcelExport.ToExcel(data.salidas, new string[] { "Contrato", "Caracteristica", "Moneda", "Importe", "Iva", "Total" }, "Reporte Proforma de Liquidacion (" + fijacion + ")");
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

        public PDFResponse descargaProformaFinal(string contrato, string fijacion)
        {
            try
            {
                PDFResponse data = new PDFProformaFinalConsumerMOA().request(contrato, fijacion);

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

        public ProcedenciaFleteWSMOAResponse getFleteProcedencia(string proveedor, string contrato)
        {
            try
            {
                ProcedenciaFleteWSMOAResponse data = (ProcedenciaFleteWSMOAResponse)new ProformaFleteProcedenciaConsumerMOA().request(proveedor,contrato);
                validarRespuesta(data);
                data.error = null;
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

        public string descargarFleteProcedencia(string proveedor, string contrato)
        {
            try
            {
                ProcedenciaFleteExcelWSMOAResponse data = (ProcedenciaFleteExcelWSMOAResponse)new ProformaFleteProcedenciaExcelConsumerMOA().request(proveedor, contrato);
                validarRespuesta(data);
                return ExcelExport.ToExcel(data.procedenciasFlete, new string[] { "Fecha Ingreso", "Contrato", "Fijacion", "Carta Porte", "Tramo", "Kilos", "Tarifa", "Importe [u$s]", "IVA [u$s]", "Importe [ARP]", "IVA [ARP]" }, "Flete Precedencia (" + contrato + ")");
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

        private void validarRespuesta(LiquidacionExcelWSMOAResponse data)
        {
            if (data == null || (data.error != null && data.error != "" && data.error != "11" && data.error != "01"))
                throw new ValidationCustomException(ErrorMsg.Error);
            if (data.liquidaciones == null || data.liquidaciones.Count == 0)
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Liquidaciones"));
        }

        private void validarRespuesta(LiquidacionWSMOAResponse data) {
            if (data == null || (data.error != null && data.error != "" && data.error != "11" && data.error != "01"))
                throw new ValidationCustomException(ErrorMsg.Error);
            if (data.liquidaciones == null || data.liquidaciones.Count == 0)
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Liquidaciones"));
        }

        private void validarRespuestaNG(LiquidacionNGWSMOAResponse data) {
            if (data == null)
                throw new ValidationCustomException(ErrorMsg.Error);
            if (data.error != null && data.error.codigo != null && data.error.codigo != "" && data.error.codigo != "00" && data.error.codigo != "11")
                throw new ValidationCustomException(data.error.descripcion);
            if (data.liquidaciones == null || data.liquidaciones.Count == 0)
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Comprobantes"));
        }

        private void validarRespuestaNG(LiquidacionExcelNGWSMOAResponse data)
        {
            if (data == null)
                throw new ValidationCustomException(ErrorMsg.Error);
            if (data.error != null && data.error.codigo != null && data.error.codigo != "" && data.error.codigo != "00" && data.error.codigo != "11")
                throw new ValidationCustomException(data.error.descripcion);
            if (data.liquidaciones == null || data.liquidaciones.Count == 0)
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Comprobantes"));
        }

        private void validarRespuestaProforma(DetalleCteWSMOAResponse data)
        {
            if (data == null)
                throw new ValidationCustomException(ErrorMsg.Error);
            if (data.error != null && data.error != "" && data.error != "00")
                throw new ValidationCustomException(data.error);
        }

        private void validarRespuestaDescargaProforma(DetalleCteExcelWSMOAResponse data)
        {
            if (data == null)
                throw new ValidationCustomException(ErrorMsg.Error);
            if (data.error != null && data.error != "" && data.error != "00")
                throw new ValidationCustomException(data.error);
        }

        private void validarRespuestaVinculacion(VinculaDetalleWSMOAResponse data) {
            if (data == null)
                throw new ValidationCustomException(ErrorMsg.Error);
            if (data.error != null && data.error.codigo != "" && data.error.codigo != "00")
                throw new ValidationCustomException(data.error.descripcion);
            if(data.data != null && data.data.Count < 1)
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Cartas de Porte"));
        }

        private void validarRespuestaDescargaVinculacion(VinculaDetalleExcelWSMOAResponse data)
        {
            if (data == null)
                throw new ValidationCustomException(ErrorMsg.Error);
            if (data.error != null && data.error.codigo != "" && data.error.codigo != "00")
                throw new ValidationCustomException(data.error.descripcion);
            if (data.data != null && data.data.Count < 1)
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Cartas de Porte"));
        }

        private void validarRespuesta(ProcedenciaFleteWSMOAResponse data)
        {
            if (data == null)
                throw new ValidationCustomException(ErrorMsg.Error);
            if (data.error == "03")
                throw new ValidationCustomException("El contrato no corresponde al proveedor");
            if (data.error == "04")
                throw new ValidationCustomException("El contrato no existe");
            if (data.error != null && data.error != "" && data.error != "00")
                throw new ValidationCustomException(data.error);
        }

        private void validarRespuesta(ProcedenciaFleteExcelWSMOAResponse data)
        {
            if (data == null)
                throw new ValidationCustomException(ErrorMsg.Error);
            if (data.error == "03")
                throw new ValidationCustomException("El contrato no corresponde al proveedor");
            if (data.error == "04")
                throw new ValidationCustomException("El contrato no existe");
            if (data.error != null && data.error != "" && data.error != "00")
                throw new ValidationCustomException(data.error);
        }
    }
}
