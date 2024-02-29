using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.ViewModel.Flete;
using SustitucionMOAModel.Models.WSMapMOA;
using SustitucionMOAModel.Models.WSMapMOA.Flete;
using SustitucionMOAModel.Models.WSMapMOA.PDF;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Export;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.WSConsumers;

namespace SustitucionMOAUtils.Services
{
    public class FleteService : IFleteService
    {
        public FleteService()
        {

        }
        public FleteViewModel ObtenerViajesPendientes(string proveedor, string fechaInicio, string fechaFin)
        {
            try
            {
                FechaWS fechas = CommonUtil.toDate(fechaInicio, fechaFin);
                FleteViewModel dataView = new FleteViewModel();
                dataView.filtroProducto = new DropdownContent();
                dataView.data = (FletesWSMOAResponse) new FletesPendientesConsumerMOA().request(proveedor, fechas.fechaInicio, fechas.fechaFin);
                ValidarRespuesta(dataView.data);
                try
                {
                    dataView.filtroProducto = new DropdownContent(dataView.data.viajes.GroupBy(i => i.descMat).Select(x => new DropdownOption { value = x.Key, label = x.Key + " (" + x.Count() + ")" }).ToList());
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

        public string DescargarViajesPendientes(string proveedor, string fechaInicio, string fechaFin)
        {
            try
            {
                FechaWS fechas = CommonUtil.toDate(fechaInicio, fechaFin);
                FletesExcelWSMOAResponse data = (FletesExcelWSMOAResponse)new FletesPendientesExcelConsumerMOA().request(proveedor, fechas.fechaInicio, fechas.fechaFin);
                ValidarRespuesta(data);
                return ExcelExport.ToExcel(data.viajes, new string[] { "Fecha", "Nro Proforma", "CCPP", "Patente", "Cantidad [Kg]", "Material", "Origen", "Destino", "Tarifa", "Peaje", "Playa", "Importe", "Status", "Factura"}, "Reporte Viajes Pendientes");

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

        public FleteAgrupadosViewModel ObtenerViajesAFacturar(string proveedor, string fechaInicio, string fechaFin)
        {
            try
            {
                FechaWS fechas = CommonUtil.toDate(fechaInicio, fechaFin);
                FleteAgrupadosViewModel dataView = new FleteAgrupadosViewModel();
                dataView = (FleteAgrupadosViewModel)new FletesAgrupadosAFacturarConsumerMOA().request(proveedor, fechas.fechaInicio, fechas.fechaFin);
                ValidarRespuesta(dataView.data);
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

        public string DescargarViajesAFacturar(string proveedor, string fechaInicio, string fechaFin)
        {
            try
            {
                FechaWS fechas = CommonUtil.toDate(fechaInicio, fechaFin);
                FletesAgrupadosExcelWSMOAResponse data = (FletesAgrupadosExcelWSMOAResponse)new FletesAgrupadosAFacturarExcelConsumerMOA().request(proveedor, fechas.fechaInicio, fechas.fechaFin);
                ValidarRespuesta(data);
                return ExcelExport.ToExcelViajesAgrupados(data.viajes, new string[] { "Fecha", "Nro Proforma", "CCPP", "Patente", "Cantidad [Kg]", "Material", "Origen", "Destino", "Tarifa", "Peaje", "Playa", "Importe", "Status", "Factura" }, "Reporte Viajes A Facturar");

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

        public FleteAgrupadosViewModel ObtenerViajesFacturados(string proveedor, string fechaInicio, string fechaFin)
        {
            try
            {
                FechaWS fechas = CommonUtil.toDate(fechaInicio, fechaFin);
                FleteAgrupadosViewModel dataView = new FleteAgrupadosViewModel();
                dataView = (FleteAgrupadosViewModel)new FletesAgrupadosFacturadoConsumerMOA().request(proveedor, fechas.fechaInicio, fechas.fechaFin);
                ValidarRespuesta(dataView.data);
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

        public string DescargarViajesFacturados(string proveedor, string fechaInicio, string fechaFin)
        {
            try
            {
                FechaWS fechas = CommonUtil.toDate(fechaInicio, fechaFin);
                FletesAgrupadosExcelWSMOAResponse data = (FletesAgrupadosExcelWSMOAResponse)new FletesAgrupadosFacturadoExcelConsumerMOA().request(proveedor, fechas.fechaInicio, fechas.fechaFin);
                ValidarRespuesta(data);
                return ExcelExport.ToExcelViajesAgrupados(data.viajes, new string[] { "Fecha", "Nro Proforma", "CCPP", "Patente", "Cantidad [Kg]", "Material", "Origen", "Destino", "Tarifa", "Peaje", "Playa", "Importe", "Status", "Factura" }, "Reporte Viajes Facturados");

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

        public Pdf ExportarPDFAFacturar(string proveedor, string fechaInicio, string fechaFin, string proforma) {

            try
            {
                FechaWS fechas = CommonUtil.toDate(fechaInicio, fechaFin);
                FleteAgrupadosViewModel dataView = new FleteAgrupadosViewModel();
                dataView = (FleteAgrupadosViewModel)new FletesAgrupadosAFacturarConsumerMOA().request(proveedor, fechas.fechaInicio, fechas.fechaFin);
                ValidarRespuesta(dataView.data);

                ViajeAgrupado proformaViaje = dataView.data.viajes.Where(v => v.proforma == proforma).FirstOrDefault();

                try
                {

                    List<string> headers = new List<string>() { "CCPP", "FECHA", "PATENTE", "KG CCPP", "DESC.", "ORIGEN", "DESTINO", "TARIFA", "PEAJE", "PLAYAS", "IMPORTE" };
                    List<PDFDetalle> detalles = new List<PDFDetalle>() {
                        new PDFDetalle() { label = "Proforma N°", value= proformaViaje.proforma},
                        new PDFDetalle() { label = "Proveedor", value = proformaViaje.proveedor},
                        new PDFDetalle() { label = "Numero Proveedor", value= proformaViaje.proveedorId},
                        new PDFDetalle() { label = "Region", value= proformaViaje.region},
                        new PDFDetalle() { label = "Fecha", value= proformaViaje.fecha},
                        new PDFDetalle() { label = "Total KG", value= proformaViaje.totalKg},
                        new PDFDetalle() { label = "Total Importe", value= proformaViaje.totalImporteString}
                    };

                    PDFResponse data = PDFExport.ToPDF(detalles, headers, proformaViaje.viajeItem);
                    if (data.Pdf == null || data.Pdf.Data == null || data.Pdf.Data.Count() == 0)
                    {
                        throw new ValidationCustomException(ErrorMsg.ErrorDescargaPDF);
                    }


                    return data.Pdf;
                } catch
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

        public Pdf ExportarPDFFacturado(string proveedor, string fechaInicio, string fechaFin, string proforma)
        {

            try
            {

                FechaWS fechas = CommonUtil.toDate(fechaInicio, fechaFin);
                FleteAgrupadosViewModel dataView = new FleteAgrupadosViewModel();
                dataView = (FleteAgrupadosViewModel)new FletesAgrupadosFacturadoConsumerMOA().request(proveedor, fechas.fechaInicio, fechas.fechaFin);
                ValidarRespuesta(dataView.data);

                ViajeAgrupado proformaViaje = dataView.data.viajes.Where(v => v.proforma == proforma).FirstOrDefault();

                try
                {

                    List<string> headers = new List<string>() { "CCPP", "FECHA", "PATENTE", "KG CCPP", "DESC.", "ORIGEN", "DESTINO", "TARIFA", "PEAJE", "PLAYAS", "IMPORTE" };
                    List<PDFDetalle> detalles = new List<PDFDetalle>() {
                        new PDFDetalle() { label = "Proforma N°", value= proformaViaje.proforma},
                        new PDFDetalle() { label = "Proveedor", value = proformaViaje.proveedor},
                        new PDFDetalle() { label = "Numero Proveedor", value= proformaViaje.proveedorId},
                        new PDFDetalle() { label = "Region", value= proformaViaje.region},
                        new PDFDetalle() { label = "Fecha", value= proformaViaje.fecha},
                        new PDFDetalle() { label = "Total KG", value= proformaViaje.totalKg},
                        new PDFDetalle() { label = "Total Importe", value= proformaViaje.totalImporteString}
                    };

                    PDFResponse data = PDFExport.ToPDF(detalles, headers, proformaViaje.viajeItem);
                    if (data.Pdf == null || data.Pdf.Data == null || data.Pdf.Data.Count() == 0)
                    {
                        throw new ValidationCustomException(ErrorMsg.ErrorDescargaPDF);
                    }


                    return data.Pdf;
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

        public string ValidarImporte(decimal importe, string proforma, string proveedor) {

            try
            {
                decimal importeDecimal = 0;
                try {
                    importeDecimal = Math.Round(importe,3);
                } catch {
                    throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorIncorrecto, "Importe"));
                }
                ErrorWS error = new FleteValidarImporteConsumerMOA().request(importeDecimal, proforma, proveedor);
                ValidarRespuestaImporte(error);
                return SuccessMsg.FleteImporteOK;
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

        public string ObtenerRelacion(string factura, string fechaEmision, decimal importe, byte[] PDF, string pdfName, string proforma, string proveedor)
        {

            try
            {
                DateTime fechaEmisionDate = DateTime.Now;
                try {
                    fechaEmisionDate = CommonUtil.toDateFecha(fechaEmision, "Fecha Emision");
                }catch (ValidationCustomException e)
                {
                    throw e;
                }
                catch(Exception e)
                {
                    throw e;
                }
                ErrorWS error = new FleteRelacionConsumerMOA().request(factura.ToUpper(), fechaEmisionDate, importe, proforma, proveedor);
                ValidarRespuestaRelacion(error);
                if (error.codigo == "03")
                {
                    try
                    {
                        EmailSender.sendFleteEmail(proveedor, factura, proforma, importe.ToString(), PDF, pdfName);
                    }
                    catch { }
                }
                return "";
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

        private void ValidarRespuesta(FletesWSMOAResponse data)
        {
            if (data == null)
                throw new ValidationCustomException(ErrorMsg.Error);
            if (data.error != null && data.error.codigo != null && data.error.codigo != "" && data.error.codigo != "00" && data.error.codigo != "02")
                throw new ValidationCustomException(data.error.descripcion);
            if (data.viajes == null || data.viajes.Count == 0)
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Fletes"));
        }

        private void ValidarRespuesta(FletesAgrupadosWSMOAResponse data)
        {
            if (data == null)
                throw new ValidationCustomException(ErrorMsg.Error);
            if (data.error != null && data.error.codigo != null && data.error.codigo != "" && data.error.codigo != "00" && data.error.codigo != "02")
                throw new ValidationCustomException(data.error.descripcion);
            if (data.viajes == null || data.viajes.Count == 0)
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Fletes"));
        }

        private void ValidarRespuesta(FletesExcelWSMOAResponse data) {
            if (data == null)
                throw new ValidationCustomException(ErrorMsg.Error);
            if (data.error != null && data.error.codigo != null && data.error.codigo != "" && data.error.codigo != "00" && data.error.codigo != "02")
                throw new ValidationCustomException(data.error.descripcion);
            if (data.viajes == null || data.viajes.Count == 0)
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Fletes"));
        }

        private void ValidarRespuesta(FletesAgrupadosExcelWSMOAResponse data)
        {
            if (data == null)
                throw new ValidationCustomException(ErrorMsg.Error);
            if (data.error != null && data.error.codigo != null && data.error.codigo != "" && data.error.codigo != "00" && data.error.codigo != "02")
                throw new ValidationCustomException(data.error.descripcion);
            if (data.viajes == null || data.viajes.Count == 0)
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Fletes"));
        }

        private void ValidarRespuestaImporte(ErrorWS error)
        {
            if (error != null && error.codigo != null && error.codigo != "" && error.codigo != "03")
                throw new ValidationCustomException(error.descripcion);
        }

        private void ValidarRespuestaRelacion(ErrorWS error)
        {
            if (error != null && error.codigo != null && error.codigo != "" && error.codigo != "03")
                throw new ValidationCustomException(error.descripcion);
        }
    }
}
