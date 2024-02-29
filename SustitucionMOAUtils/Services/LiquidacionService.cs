using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using SustitucionMOAAssets;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.ViewModel.Liquidacion;
using SustitucionMOAModel.Models.WSMapMOA.Liquidacion;
using SustitucionMOAModel.Models.WSMapMOA.Liquidacion.NoGranos;
using SustitucionMOAModel.Models.WSMapMOA.PDF;
using SustitucionMOAModel.Models.WSMapMOA.Proforma;
using SustitucionMOAModel.Models.WSMapMOA.Vincula.Detalle;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Export;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAValidator;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WSConsumers;

namespace SustitucionMOAUtils.Services
{
    public class LiquidacionService: ILiquidacionService
    {
        protected readonly IRepositorio repositorio;
        protected readonly IAzureService azureService;
        private readonly IListarPesificacionesConsumer pesificacionesConsumer;
        private readonly string[] formatosDeArchivoValidos = new string[] { ".pdf", ".png", ".jpg" };
        private const string FECHA_REGEX = @"([0-2]?[0-9]|(3)[0-1])(\/)(((0)[0-9])|((1)[0-2]))(\/)\d{4}";
        private const string COE_REGEX = @"C.*O.*E.*:";

        public LiquidacionService(
            IRepositorio repositorio,
            IAzureService azureService,
            IListarPesificacionesConsumer pesificacionesConsumer)
        {
            this.repositorio = repositorio;
            this.azureService = azureService;
            this.pesificacionesConsumer = pesificacionesConsumer;
        }

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

        public LiquidacionViewModel TodasLiquidaciones(string proveedor, string fechaInicio, string fechaFin, string palabra)
        {
            try
            {
                List<FechaWS> fechas = CommonUtil.toDateList(fechaInicio, fechaFin);
                LiquidacionViewModel dataView = new LiquidacionViewModel();
                dataView.filtroProducto = new DropdownContent();
                dataView.filtroObservacion = new DropdownContent();
                dataView.data = (LiquidacionWSMOAResponse)new LiquidacionesConsumerMOA().request(proveedor, fechas, palabra,"");
               
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

        public LiquidacionViewModel getLiquidaciones(string proveedor, string tipo, string fechaInicio, string fechaFin) {
            try
            {
                List<FechaWS> fechas = CommonUtil.toDateList(fechaInicio, fechaFin);
                LiquidacionViewModel dataView = new LiquidacionViewModel();
                dataView.filtroProducto = new DropdownContent();
                dataView.filtroObservacion = new DropdownContent();
                dataView.data = (LiquidacionWSMOAResponse) new LiquidacionesConsumerMOA().request(proveedor, fechas,"","");
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

        public LiquidacionNGViewModel getRegistradosNG(string proveedor, string fechaInicio, string fechaFin)
        {
            return getLiquidacionesNG(proveedor, "REGISTRADO", fechaInicio, fechaFin);
        }

        public LiquidacionNGViewModel getPendienteRegistroNG(string proveedor, string fechaInicio, string fechaFin)
        {
            return getLiquidacionesNG(proveedor, "PENDIENTE-REGISTRO", fechaInicio, fechaFin);
        }

        public LiquidacionNGViewModel getPagasNG(string proveedor, string fechaInicio, string fechaFin)
        {
            return getLiquidacionesNG(proveedor, "PAGA", fechaInicio, fechaFin);
        }

        public LiquidacionNGViewModel getLiquidacionesNG(string proveedor, string tipo, string fechaInicio, string fechaFin)
        {
            try
            {
                
                List<FechaWS> fechas = CommonUtil.toDateList(fechaInicio, fechaFin);
                LiquidacionNGViewModel dataView = new LiquidacionNGViewModel();
               
                dataView.filtroObservacion = new DropdownContent();

                if (tipo == "PENDIENTE-REGISTRO")
                {
                    LiquidacionNGViewModel liquidacionView = new LiquidacionNGViewModel();
                    var comprobantes = (ComprobantesNGWSMOAResponse)new ComprobantesNGConsumerMOA().request(proveedor, fechas);
                    validarRespuestaNG(comprobantes);
                    liquidacionView.comprobantes = comprobantes; 
                    return liquidacionView;
                }
              
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
                    case "REGISTRADO":
                        break;
                    case "PENDIENTE-REGISTRO":
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

        public ComprobantesNGWSMOAResponse getComprobantesNG(string proveedor, string fechaInicio, string fechaFin)
        {
            try
            {
                List<FechaWS> fechas = CommonUtil.toDateList(fechaInicio, fechaFin);

                ComprobantesNGWSMOAResponse result =  (ComprobantesNGWSMOAResponse)new ComprobantesNGConsumerMOA().request(proveedor, fechas);

                if (result.comprobantes.Count == 0) // <- Repito consulta por el filtrado que se realiza antes
                    throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Comprobantes"));
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


        public string downloadAprobadas(string proveedor, string fechaInicio, string fechaFin)
        {
            try
            {
                List<FechaWS> fechas = CommonUtil.toDateList(fechaInicio, fechaFin);
                LiquidacionExcelWSMOAResponse data = (LiquidacionExcelWSMOAResponse) new LiquidacionesExcelConsumerMOA().request(proveedor, fechas,"","");
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
                List<FechaWS> fechas = CommonUtil.toDateList(fechaInicio, fechaFin);
                LiquidacionExcelWSMOAResponse data = (LiquidacionExcelWSMOAResponse)new LiquidacionesExcelConsumerMOA().request(proveedor, fechas,"","");
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
                List<FechaWS> fechas = CommonUtil.toDateList(fechaInicio, fechaFin);
                LiquidacionExcelWSMOAResponse data = (LiquidacionExcelWSMOAResponse)new LiquidacionesExcelConsumerMOA().request(proveedor, fechas,"","");
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
                List<FechaWS> fechas = CommonUtil.toDateList(fechaInicio, fechaFin);
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
                List<FechaWS> fechas = CommonUtil.toDateList(fechaInicio, fechaFin);
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

        public string downloadRegistradosNG(string proveedor, string fechaInicio, string fechaFin)
        {
            try
            {
                List<FechaWS> fechas = CommonUtil.toDateList(fechaInicio, fechaFin);
                LiquidacionExcelNGWSMOAResponse data = (LiquidacionExcelNGWSMOAResponse)new LiquidacionesExcelNGConsumerMOA().request(proveedor, fechas);
                validarRespuestaNG(data);
                data.liquidaciones = data.liquidaciones.ToList();
                if (data.liquidaciones.Count == 0)
                    throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Comprobantes"));
                return ExcelExport.ToExcel(data.liquidaciones, new string[] { "ID", "Vencimiento", "Tipo", "Comprobante", "Total", "Moneda", "Orden de Compra", "Observacion" }, "Reporte Comprobantes Registrados");
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

        public string downloadPendienteRegistroNG(string proveedor, string fechaInicio, string fechaFin)
        {
            try
            {
                List<FechaWS> fechas = CommonUtil.toDateList(fechaInicio, fechaFin);
                ComprobantesExcelNGWSMOAResponse data = (ComprobantesExcelNGWSMOAResponse)new ComprobantesExcelNGConsumerMOA().request(proveedor, fechas);
                validarRespuestaNG(data);
                data.comprobantes = data.comprobantes.ToList();
                if (data.comprobantes.Count == 0)
                    throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Comprobantes"));
                return ExcelExport.ToExcel(data.comprobantes, new string[] { "Fecha de comprobante", "Tipo", "Comprobante", "Total", "Orden de Compra", "Estado" }, "Reporte Comprobantes Pendientes de Registro");
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
                List<FechaWS> fechas = CommonUtil.toDateList(fechaInicio, fechaFin);
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
                var data = new DetalleCteConsumerMOA().Request(fijacion, proveedor);
                validarRespuestaProforma(data);
                data.error = null;

                if ((!data.LiquidacionParcialEmitida || data.FaltanDatosDeCalidad) &&
                    data.cabecera.moneda != ConstanteSAP.MONEDA_PESOS)
                {
                    var pesificacionesResponse = pesificacionesConsumer.Request(proveedor);

                    var contrato = data.salidas?.Count > 0 ? data.salidas[0].contrato : "";
                    data.Pesificaciones = pesificacionesResponse.Pesificaciones
                        .Where(x => x.Contrato == contrato || x.Fijacion == fijacion)
                        .ToList();
                }

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
                var data = new DetalleCteExcelConsumerMOA().Request(fijacion, proveedor);
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
                PDFResponse data = new PDFProformaFinalConsumerMOA().request(contrato, fijacion.EndsWith("00") ? null : fijacion);

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

        public PDFResponse descargaComprobantesNG(string CodigoProveedorSAP, string FechaDocumento, string NumeroLegalDocumento)
        {
            try
            {
                PDFResponse data = new PDFComprobantesNGConsumerMOA().request(CodigoProveedorSAP, FechaDocumento, NumeroLegalDocumento);

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

        public async Task NotificarLiquidacionesAsync(HttpFileCollectionBase liquidaciones, string codigoProveedor)
        {
            List<string> archivosNoProcesados = new List<string>();
            var proveedor = repositorio.Obtener<Proveedor>(p => p.CodigoProveedor == codigoProveedor);
            //No existe proveedor para la sesión, termino
            if (proveedor == null) return;

            for (int i = 0; i < liquidaciones.Count; i++)
            {
                var liquidacion = liquidaciones[i];
                //En caso que nos venga un archivo no valido, lo skipeamos
                if (liquidacion.ContentLength > 0 && formatosDeArchivoValidos.Contains(System.IO.Path.GetExtension(liquidacion.FileName).ToLower()))
                {
                    var operacionOCRId = await azureService.AnalizarImagenAsync(liquidacion);

                    //Segun la documentación de Microsoft es necesario este sleep para poder obtener la info de la lectura OCR. Validar si lo queremos hacer en el momento o en background
                    Thread.Sleep(2000);

                    var resultadosOCR = await azureService.ObtenerResultadoOCRAsync(operacionOCRId);
                    var resultadoCoe = resultadosOCR.FirstOrDefault(rocr => Regex.Match(rocr, COE_REGEX).Success);
                    //Asumimos por default que no se encontró el COE
                    var coe = string.Empty;
                    if (!string.IsNullOrEmpty(resultadoCoe))
                    {
                        var inlineCoe = resultadoCoe.Split(':');
                        if (inlineCoe.Length > 0 && !string.IsNullOrEmpty(inlineCoe[1].Trim()))
                        {
                            coe = inlineCoe[1].Trim();
                        }
                        //Puede que venga el C.O.E: separado del código. En ese caso tomar el resultado siguiente
                        else if (string.IsNullOrEmpty(coe))
                        {
                            coe = resultadosOCR[resultadosOCR.IndexOf(resultadoCoe) + 1].Trim();
                        }
                    }
                    //TODO: validar si el chequeo de coe hace falta y de ser así si deberíamos validar si ya existe ese coe asociado al proveedor o el coe es único globalmente
                    if (!string.IsNullOrEmpty(coe))
                    {
                        if(!repositorio.Existe<LiquidacionInformada>(li => li.COE == coe))
                        {
                            var fechaStr = resultadosOCR.Select(rocr => Regex.Match(rocr, FECHA_REGEX)).Where(rm => !string.IsNullOrEmpty(rm.Value)).Select(m => m.Value).FirstOrDefault();

                            DateTime? fecha = null;
                            if (!string.IsNullOrEmpty(fechaStr))
                            {
                                fecha = DateTime.Parse(fechaStr, CultureInfo.CurrentCulture);
                            }

                            var liquidacionInformada = new LiquidacionInformada()
                            {
                                Id = Guid.Parse(operacionOCRId),
                                Proveedor_Id = proveedor.Id,
                                COE = coe,
                                FechaComprobante = fecha,
                                FechaInformada = DateTime.Now
                            };

                            repositorio.Agregar(liquidacionInformada);
                            repositorio.GuardarCambios();

                            //Subo el archivo al blob storage una vez procesado
                            await azureService.SubirArchivoABlobStorageAsync(liquidacion, coe);
                        }
                        else
                        {
                            archivosNoProcesados.Add($"El documento {liquidacion.FileName} correponde a un COE ya informado");
                        }
                    }
                    else
                    {
                        archivosNoProcesados.Add($"No se pudo obtener el valor de COE para el documento {liquidacion.FileName}");
                    }
                }
            }

            if (archivosNoProcesados.Any()) throw new InfoCustomException(string.Join(". ", archivosNoProcesados));
        }

        public IList<LiquidacionInformadaDto> GetLiquidacionInformadas(string codigoProveedor)
        {
            var proveedor = repositorio.Obtener<Proveedor>(p => p.CodigoProveedor == codigoProveedor);
            //No existe proveedor para la sesión, termino
            if (proveedor == null) throw new InfoCustomException("No existe un proveedor registrado para el código seleccionado");

            var liquidacionesAux = repositorio.Listar<LiquidacionInformada, LiquidacionInformadaDto>(x => new LiquidacionInformadaDto()
            {
                Id = x.Id,
                Proveedor_Id = x.Proveedor_Id,
                COE = x.COE,
                FechaComprobante = x.FechaComprobante.ToString(),
                FechaInformada = x.FechaInformada.ToString(),
            }, x => x.Proveedor_Id == proveedor.Id).ToList();

            var liquidacionesDto = liquidacionesAux.Select(x => new LiquidacionInformadaDto()
            {
                Id = x.Id,
                Proveedor_Id = x.Proveedor_Id,
                COE = x.COE,
                FechaComprobante = Convert.ToDateTime(x.FechaComprobante).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                FechaInformada = Convert.ToDateTime(x.FechaInformada).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            }).ToList();

            if (!liquidacionesDto.Any()) throw new InfoCustomException("No se encontraron liquidaciones informadas");

            return liquidacionesDto;
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

        private void validarRespuestaNG(ComprobantesNGWSMOAResponse data) {
            if (data == null)
                throw new ValidationCustomException(ErrorMsg.Error);
            if (data.error != null && data.error.codigo != null && data.error.codigo != "" && data.error.codigo != "00" && data.error.codigo != "11")
                throw new ValidationCustomException(data.error.descripcion);
            if (data.comprobantes == null || data.comprobantes.Count == 0)
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Comprobantes"));
        }

        private void validarRespuestaNG(ComprobantesExcelNGWSMOAResponse data)
        {
            if (data == null)
                throw new ValidationCustomException(ErrorMsg.Error);
            if (data.error != null && data.error.codigo != null && data.error.codigo != "" && data.error.codigo != "00" && data.error.codigo != "11")
                throw new ValidationCustomException(data.error.descripcion);
            if (data.comprobantes == null || data.comprobantes.Count == 0)
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Comprobantes"));
        }

        private void validarRespuestaNG(LiquidacionNGWSMOAResponse data)
        {
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
