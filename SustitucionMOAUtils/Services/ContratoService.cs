using System;
using System.Collections.Generic;
using System.Linq;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.ViewModel;
using SustitucionMOAWS.WSConsumers;
using SustitucionMOAUtils.Export;
using SustitucionMOAModel.Models.WSMapMOA.Contrato.Detalle;
using SustitucionMOAModel.Models.WSMapMOA;
using SustitucionMOAModel.Models.WSMapMOA.Contrato;
using SustitucionMOAModel.Models.WSMapMOA.PDF;
using SustitucionMOAModel.Models.WSMapMOA.Fijacion.Detalle;
using SustitucionMOAUtils.Interfaces;

namespace SustitucionMOAUtils.Services
{
    public class ContratoService : IContratoService
    {
        private readonly string PEND_CAMARA_EXCEL = "PEND. CÁMARA";
        private readonly string CAMARA_EXCEL = "CÁMARA";
        private readonly string CALADO_EXCEL = "CALADO";
        public ContratoService()
        {

        }
        public ContratoViewModel ObtenerVigentes(string proveedor, string fechaInicio, string fechaFin)
        {
            try
            {
                List<FechaWS> fechas = CommonUtil.toDateList(fechaInicio, fechaFin);
                ContratoViewModel dataView = new ContratoViewModel();
                dataView.filtroProducto = new DropdownContent();
                dataView.filtroVendedor = new DropdownContent();
                dataView.filtroTipoContrato = new DropdownContent();
                dataView.data = (ContratosWSMOAResponse)new ContratosConsumerMOA().request(proveedor, fechas);
                ValidarRespuesta(dataView.data);
                try
                {
                    dataView.filtroProducto = new DropdownContent(dataView.data.contratosInfo.GroupBy(i => i.material).Select(x => new DropdownOption { value = x.Key, label = x.Key + " (" + x.Count() + ")" }).ToList());
                    dataView.filtroVendedor = new DropdownContent(dataView.data.contratosInfo.GroupBy(i => i.vendedor).Select(x => new DropdownOption { value = x.Key, label = x.Key + " (" + x.Count() + ")" }).ToList());
                    dataView.filtroTipoContrato = new DropdownContent(dataView.data.contratosInfo.GroupBy(i => i.tipoContrato).Select(x => new DropdownOption { value = x.Key, label = x.Key + " (" + x.Count() + ")" }).ToList());
                }
                catch { }
                return dataView;
            }
            catch (ValidationCustomException e)
            {
                throw e;
            }
            catch (InfoCustomException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public ContratosNoCumplidosViewModel ObtenerContratosNoCumplidos(string proveedor, string fechaInicio, string fechaFin, List<string> contratos, string tipoOperacion, string tipoOperacionMsj)
        {
            try
            {

                List<FechaWS> fechas = CommonUtil.toDateList(fechaInicio, fechaFin);
                ContratosNoCumplidosViewModel dataView = new ContratosNoCumplidosViewModel();
                dataView.filtroProducto = new DropdownContent();
                dataView.filtroVendedor = new DropdownContent();
                dataView.filtroTipoContrato = new DropdownContent();
                dataView.data = (ContratosNoCumplidosWSMOAResponse)new FijacionesConsumerMOA().request(proveedor, contratos, fechas, tipoOperacion);
                ValidarRespuesta(dataView.data, tipoOperacionMsj);
                try
                {
                    dataView.filtroProducto = new DropdownContent(dataView.data.contratosInfo.GroupBy(i => i.material).Select(x => new DropdownOption { value = x.Key, label = x.Key + " (" + x.Count() + ")" }).ToList());
                    dataView.filtroVendedor = new DropdownContent(dataView.data.contratosInfo.GroupBy(i => i.vendedor).Select(x => new DropdownOption { value = x.Key, label = x.Key + " (" + x.Count() + ")" }).ToList());
                    dataView.filtroTipoContrato = new DropdownContent(dataView.data.contratosInfo.GroupBy(i => i.tipoContrato).Select(x => new DropdownOption { value = x.Key, label = x.Key + " (" + x.Count() + ")" }).ToList());
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

        public ContratoDetalleWSMOAResponse ObtenerDetalleContrato(string proveedor, string numeroContrato)
        {
            try
            {
                if (numeroContrato == null || numeroContrato == "")
                {
                    throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "Numero de Contrato"));
                }

                ContratoDetalleWSMOAResponse response = (ContratoDetalleWSMOAResponse)new ContratoDetalleConsumerMOA().request(proveedor, numeroContrato);
                if (response == null || response.error == "06")
                {
                    throw new InfoCustomException(String.Format(InfoMsg.ElementoNoExiste, "Contrato", numeroContrato));
                }

                if (response.error != null && response.error != "" && response.error != "01")
                {
                    throw new ValidationCustomException(ErrorMsg.Error);
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

        public FijacionDetalleWSMOAResponse ObtenerDetalleFijacion(string proveedor, string numeroContrato, string fijacion)
        {
            try
            {
                if (numeroContrato == null || numeroContrato == "")
                {
                    throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "Contrato"));
                }

                if (fijacion == null || fijacion == "")
                {
                    throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "Fijación"));
                }

                FijacionDetalleWSMOAResponse response = new FijacionDetalleConsumerMOA().request(proveedor, numeroContrato, fijacion);
                if (response == null || (response.error != null && response.error.codigo == "06"))
                {
                    throw new InfoCustomException(String.Format(InfoMsg.ElementoNoExiste, "Fijación", fijacion));
                }

                if (response.error != null && response.error != null && response.error.codigo != "00")
                {
                    throw new ValidationCustomException(ErrorMsg.Error);
                }

                response.error = null;
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

        public string DescargarVigentes(string proveedor, string fechaInicio, string fechaFin)
        {
            try
            {
                List<FechaWS> fechas = CommonUtil.toDateList(fechaInicio, fechaFin);
                ContratosExcelWSMOAResponse data = (ContratosExcelWSMOAResponse)new ContratosExcelConsumerMOA().request(proveedor, fechas);
                ValidarRespuesta(data);
                return ExcelExport.ToExcel(data.contratosInfo, new string[] { "ID Vendedor", "Contrato Molinos", "Contrato Proveedor", "Vendedor", "Producto", "Cantidad Kilos", "Unidad Cant. Kilos", "Precio", "Moneda", "Lugar Descarga", "Cosecha", "Estado Boleto ", "Aplicaciones", "Unidad Apliaciones", "Liquidado", "Unidad Liquidado", "Ultimo Movimiento", "Estado" }, "Reporte Contratos Vigentes");

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

        public string DescargarNoCumplidos(string proveedor, string fechaInicio, string fechaFin, string tipoOperacion, string nombreArchivo, string tipoOperacionMsj)
        {
            try
            {
                List<FechaWS> fechas = CommonUtil.toDateList(fechaInicio, fechaFin);
                ContratosNoCumplidosExcelWSMOAResponse data = (ContratosNoCumplidosExcelWSMOAResponse)new FijacionesExcelConsumerMOA().request(proveedor, new List<string>() { }, fechas, tipoOperacion);
                ValidarRespuesta(data, tipoOperacionMsj);
                return ExcelExport.ToExcel(data.contratosInfo, new string[] { "Contrato Molinos", "Contrato Proveedor", "Vendedor", "Producto", "Cantidad Kilos", "Unidad Cant. Kilos", "Kilos Fijados", "Unidad Kilos Fijados", "Ampliado", "Unidad Ampliado", "Anulado", "Unidad Anulado", "Importe", "Moneda", "Fecha de Anulacion", "Liquidado", "Unidad Liquidado", "Total", "Unidad Total", "Estado" }, nombreArchivo);

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

        public string DescargarDetalle(string proveedor, string numeroContrato)
        {
            try
            {
                ContratoDetalleExcelWSMOAResponse data = (ContratoDetalleExcelWSMOAResponse)new ContratoDetalleExcelConsumerMOA().request(proveedor, numeroContrato);


                var calidadesExcel = data.calidad.SelectMany(c =>
                    c.registros.Select(r =>
                    {
                        var detalle = new CalidadContratoDetalleExcel
                        {
                            ccpp = c.ccpp,
                            caract = r.caract,
                            dto = r.dto,
                            kgApli = r.kgApli,
                            kgDto = r.kgDto,
                            kgNetos = r.kgNetos,
                            nroCert = r.certificado,
                            recCert = r.recCert,
                            recResul = r.recResul,
                            unidad = r.unidad
                        };

                        if (r.caract.ToUpper().Contains("HUMEDAD"))
                        {
                            detalle.resultado = $"{r.calaResul}";
                        }
                        else if ((r.kgDtoValor > 0 && c.camaraPendiente) || !c.camaraPendiente)
                        {
                            var value = c.camaraPendiente ? r.calaResul : r.camaResul;
                            var identificadorResultadoCalidad = c.camaraPendiente ? PEND_CAMARA_EXCEL :
                                            c.tieneCertificado ? CAMARA_EXCEL : CALADO_EXCEL;
                            detalle.resultado = identificadorResultadoCalidad == PEND_CAMARA_EXCEL? PEND_CAMARA_EXCEL : $"{value}";
                        }

                        if (string.IsNullOrEmpty(detalle.resultado) && c.camaraPendiente)
                        {
                            detalle.resultado = PEND_CAMARA_EXCEL;
                        }

                        return detalle;
                    }).OrderBy(r => r.ccpp).ThenBy(r => r.caract)).ToList();


                return ExcelExport.ToExcelContratoDetalle(data.ampliacionesAnulaciones, data.aplicaciones, calidadesExcel, data.caracteristicas, data.condicionesPago, data.fijaciones, data.hijos, data.liquidaciones, data.pagos, data.resumen,
                                                           new string[] { "Tipo", "Fecha", "Cantidad", "Unidad", "Importe", "Moneda" },
                                                           new string[] { "Fecha", "CCPP", "Descarga", "Brutos", "Unidad Brutos", "Netos", "Unidad Netos", "Cantidad", "Unidad Cantidad" },
                                                           new string[] { "CCPP", "Caracteristica", "Resultado", "Nro Certificado", "Resul. Rec.", "Cert. Rec.", "Kg. Dto", "Kg. Apli", "Netos Descontado", "Unidad", "Dto" },
                                                           new string[] { "Tipo", "Descarga", "Fecha Concreta", "Cantidad", "Unidad", "Standard Calidad", "Calificacion", "Procedencia", "Cosecha", "Toleria Min", "Toleria Max", "Entrega Min", "Entrega Max", "Estado Bol", "Pago Parcial", "Pizarra Ref", "Condicion Pago Fija", "Fecha Tope Fija", "Fija Diaria Min", "Fija Diaria Max", "Corredor", "Nombre Corredor", "Vendedor", "Nombre Vendedor", "Importe A Precio", "Moneda A Precio", "Porcentaje A Precio", "Importe S Precio", "Moneda S Precio", "Porcentaje S Precio", "Descuento A Carreo", "cdCdg", "Canje", "Retener IVA", "Warrant", "Pago Directo Vendedor", "Cesion", "Confirma" },
                                                           new string[] { "Condiciones de Pago" },
                                                           new string[] { "Fecha", "Nro Fija", "Kilos Fija", "Unidad", "Precio", "Moneda" },
                                                           new string[] { "Fecha", "Contrato Madre", "Contrato Molinos", "Contrato Proveedor", "Cantidad", "Unidad", "Precio", "Moneda" },
                                                           new string[] { "Fecha", "Tipo", "Comprobante", "Cantidad", "Unidad", "Precio", "Moneda Precio", "Total", "Moneda Total", "Pedido" },
                                                           new string[] { "Fecha", "Id Pago", "Comprobante", "Bruto", "IVA", "Retenciones", "Neto", "Moneda" },
                                                           new string[] { "Contrato", "Estado", "Contrato Madre", "Producto", "Cantidad Entre", "Unidad Cant. Entre", "Cantidad Liquidado", "Unidad Cant. Liquidado", "Cantidad Fija", "Unidad Cant. Fija", "Cantidad Pendiente Entre", "Unidad Cant. Pendiente Entre", "Precio", "Moneda" },
                                                           "Reporte Contrato Detalle (Nro. " + numeroContrato + ")");
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


        public Pdf DescargarPDFCalidad(string proveedor, string numeroContrato)
        {
            try
            {
                ContratoDetallePDFWSMOAResponse data = (ContratoDetallePDFWSMOAResponse)new ContratoDetallePDFConsumerMOA().request(proveedor, numeroContrato);

                try
                {
                    PDFResponse pdfExport = PDFExport.ToPDF("Calidad Contrato(" + numeroContrato + ")", new List<string>() { "CCPP", "Característica", "Calado Result.", "Calado Dto.", "Cámara Result.", "Cámara Dto.", "Kg Netos", "Kg Apli" }, data.calidad);
                    if (pdfExport.pdf == null || pdfExport.pdf.data == null || pdfExport.pdf.data.Count() == 0)
                    {
                        throw new ValidationCustomException(ErrorMsg.ErrorDescargaPDF);
                    }


                    return pdfExport.pdf;
                }
                catch
                {
                    throw new ValidationCustomException(ErrorMsg.ErrorDescargaPDF);
                }
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

        public Pdf DescargarBoletoFisico(string proveedor, string contrato)
        {
            try
            {
                PDFResponse data = new ContratoPDFConsumerMOA().request(proveedor, contrato);

                if (data.error != null && data.error.codigo != "00")
                {
                    throw new InfoCustomException(InfoMsg.SinBoletoFisico);
                }

                if (data.pdf == null || data.pdf.data == null || data.pdf.data.Count() == 0)
                {
                    throw new InfoCustomException(InfoMsg.SinBoletoFisico);
                }
                return data.pdf;
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

        public string DescargarDetalleFijacion(string proveedor, string numeroContrato, string fijacion)
        {
            try
            {
                if (numeroContrato == null || numeroContrato == "")
                {
                    throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "Contrato"));
                }

                if (fijacion == null || fijacion == "")
                {
                    throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "Fijación"));
                }

                FijacionDetalleWSMOAResponse response = new FijacionDetalleConsumerMOA().request(proveedor, numeroContrato, fijacion);

                return ExcelExport.ToExcel(response.detalleFijacion, new string[] { "Fecha", "Nro. CCPP", "Kilos Fijados" }, "Reporte Fijaciones Detalle");
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

        private void ValidarRespuesta(ContratosWSMOAResponse data)
        {
            if (data == null)
                throw new ValidationCustomException(ErrorMsg.Error);
            if (data.error != null && data.error.codigo != null && data.error.codigo != "" && data.error.codigo != "11" && data.error.codigo != "16")
                throw new ValidationCustomException(data.error.descripcion);
            if (data.contratosInfo == null || data.contratosInfo.Count == 0)
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Contratos"));
        }

        private void ValidarRespuesta(ContratosNoCumplidosWSMOAResponse data, string tipoOperacionMsj)
        {
            if (data == null)
                throw new ValidationCustomException(ErrorMsg.Error);
            if (data.error != null && data.error.codigo != null && data.error.codigo != "" && data.error.codigo != "11" && data.error.codigo != "37" && data.error.codigo != "00")
                throw new ValidationCustomException(data.error.descripcion);
            if (data.contratosInfo == null || data.contratosInfo.Count == 0)
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, tipoOperacionMsj));
        }

        private void ValidarRespuesta(ContratosExcelWSMOAResponse data)
        {
            if (data == null)
                throw new ValidationCustomException(ErrorMsg.Error);
            if (data.error != null && data.error.codigo != null && data.error.codigo != "" && data.error.codigo != "11" && data.error.codigo != "16")
                throw new ValidationCustomException(data.error.descripcion);
            if (data.contratosInfo == null || data.contratosInfo.Count == 0)
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Contratos"));
        }

        private void ValidarRespuesta(ContratosNoCumplidosExcelWSMOAResponse data, string tipoOperacionMsj)
        {
            if (data == null)
                throw new ValidationCustomException(ErrorMsg.Error);
            if (data.error != null && data.error.codigo != null && data.error.codigo != "" && data.error.codigo != "00" && data.error.codigo != "11" && data.error.codigo != "16" && data.error.codigo != "37")
                throw new ValidationCustomException(data.error.descripcion);
            if (data.contratosInfo == null || data.contratosInfo.Count == 0)
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, tipoOperacionMsj));
        }
    }
}
