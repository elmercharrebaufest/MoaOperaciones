using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using SustitucionMOAAssets;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.ViewModel;
using SustitucionMOAModel.Models.WSMapMOA;
using SustitucionMOAModel.Models.WSMapMOA.CartaPorte;
using SustitucionMOAModel.Models.WSMapMOA.CartaPorte.Detalle;
using SustitucionMOAModel.Models.WSMapMOA.CartaPorte.Formulario;
using SustitucionMOAModel.Models.WSMapMOA.PDF;
using SustitucionMOAUtils.Export;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ScatoWebService;
using SustitucionMOAWS.WSConsumers; 

namespace SustitucionMOAUtils.Services
{
    public class CartaPorteService : ICartaPorteService
    {
        readonly IScatoConsumer scatoConsumer;

        public CartaPorteService (IScatoConsumer scatoConsumer)
        {
            this.scatoConsumer = scatoConsumer;
        }

        public CartaPorteDescargaViewModel GetDescargas(string proveedor, string fechaInicio, string fechaFin)
        {
            try
            {
                List<FechaWS> fechas = CommonUtil.toDateList(fechaInicio, fechaFin);
                List<string> cartaPorte = new List<string>();
                CartaPorteDescargaViewModel dataView = new CartaPorteDescargaViewModel
                {
                    filtroProducto = new DropdownContent(),
                    filtroVendedor = new DropdownContent(),
                    data = (CartaPorteDescargaWSMOAResponse)new RecepcionesConsumerMOA().request(proveedor, fechas, cartaPorte)
                }; 

                ValidarRespuesta(dataView.data);

                try
                {
                    dataView.filtroProducto = new DropdownContent(
                                                    dataView.data.cartasPorte
                                                        .GroupBy(i => i.producto)
                                                        .Select(x => new DropdownOption { value = x.Key, label = x.Key + " (" + x.Count() + ")" })
                                                        .ToList()
                                                    );

                    dataView.filtroVendedor = new DropdownContent(
                                                    dataView.data.cartasPorte
                                                        .GroupBy(i => i.vendedor)
                                                        .Select(x => new DropdownOption { value = x.Key, label = x.Key + " (" + x.Count() + ")" })
                                                        .ToList()
                                                    );
                }
                catch { }
                return dataView;
            }
            catch (InfoCustomException)
            {
                throw;
            }
            catch (ValidationCustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public CartaPorteViewModel GetAplicaciones(string proveedor, string fechaInicio, string fechaFin)
        {
            try
            {
                List<FechaWS> fechas = CommonUtil.toDateList(fechaInicio, fechaFin);
                List<string> contratos = new List<string>();
                CartaPorteViewModel dataView = new CartaPorteViewModel
                {
                    filtroProducto = new DropdownContent(),
                    filtroVendedor = new DropdownContent(),
                    data = (CartaPorteWSMOAResponse)new AplicacionesConsumerMOA().request(proveedor, fechas, contratos,"")
                };

                ValidarRespuesta(dataView.data);
                try
                {
                    dataView.filtroProducto = new DropdownContent(
                                                    dataView.data.cartasPorte
                                                    .GroupBy(i => i.producto)
                                                    .Select(x => new DropdownOption { value = x.Key, label = x.Key + " (" + x.Count() + ")" })
                                                    .ToList()
                                                );

                    dataView.filtroVendedor = new DropdownContent(
                                                    dataView.data.cartasPorte
                                                    .GroupBy(i => i.vendedor)
                                                    .Select(x => new DropdownOption { value = x.Key, label = x.Key + " (" + x.Count() + ")" })
                                                    .ToList()
                                                );
                }
                catch { }
                return dataView;
            }
            catch (InfoCustomException)
            {
                throw;
            }
            catch (ValidationCustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public string DownloadAplicaciones(string proveedor, string fechaInicio, string fechaFin)
        {
            try
            {
                List<FechaWS> fechas = CommonUtil.toDateList(fechaInicio, fechaFin);
                List<string>  contratos = new List<string>();
                CartaPorteExcelWSMOAResponse data = (CartaPorteExcelWSMOAResponse)new AplicacionesExcelConsumerMOA().request(proveedor, fechas, contratos,"");
                ValidarRespuesta(data);
                return ExcelExport.ToExcel(data.cartasPorte, new string[] { "Carta Porte", "Contrato Molinos", "Contrato Proveedor", "Fecha Descarga", "Producto", "Descargado", "Unidad Descargado", "Pend. Aplic.", "Unidad Pend. Aplic.", "Liquidar", "Unidad Liquidar", "ID Vendedor", "Vendedor", "Sust", "Titular", "Desc. Titular" }, "Reporte Aplicaciones");
            }
            catch (InfoCustomException)
            {
                throw;
            }
            catch (ValidationCustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public string DownloadDescargas(string proveedor, string fechaInicio, string fechaFin)
        {
            try
            {
                List<FechaWS> fechas = CommonUtil.toDateList(fechaInicio, fechaFin);
                List<string> cartaPorte = new List<string>();
                CartaPorteDescargaExcelWSMOAResponse data = (CartaPorteDescargaExcelWSMOAResponse)new RecepcionesExcelConsumerMOA().request(proveedor, fechas, cartaPorte);
                ValidarRespuesta(data);
                return ExcelExport.ToExcel(data.cartasPorte, new string[] { "Carta Porte", "Fecha Descarga", "Producto","Bruto Origen","Tara Origen","Neto Origen","Bruto Destino","Tara Destino", "Neto Destino", "Descargado", "Unidad Descargado", "Mermas", "ID Vendedor", "Vendedor", "Sust", "Titular", "Desc. Titular", "Contrato Molinos", "CG" }, "Reporte Descargas");
            }
            catch (InfoCustomException)
            {
                throw;
            }
            catch (ValidationCustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public CartaPorteDetalleWSMOAResponse GetDetalle(string proveedor, string cartaporteId)
        {
            try
            {
                CartaPorteDetalleWSMOAResponse response = (CartaPorteDetalleWSMOAResponse)new CartaPorteDetalleConsumerMOA().request(proveedor, cartaporteId);

                if (response.error != null && response.error.codigo != "11" && response.error.codigo != "" && response.error.codigo != "00")
                {
                    throw new ValidationCustomException(response.error.descripcion);
                }

                return response;
            }
            catch (InfoCustomException)
            {
                throw;
            }
            catch (ValidationCustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public string DownloadDetalle(string proveedor, string cartaporteId)
        {
            try
            {
                CartaPorteDetalleExcelWSMOAResponse data = (CartaPorteDetalleExcelWSMOAResponse)new CartaPorteDetalleExcelConsumerMOA().request(proveedor, cartaporteId);

                return ExcelExport.ToExcelCartaPorteDetalle(data.entregasDescargas, data.datosCalidad, data.aplicaciones, new string[] { "Vendedor", "Descripcion Vendedor", "Fecha", "Producto", "Descripcion Producto", "Centro", "Descarga Centro", "Procedencia", "Neto Descontado", "Unidad Neto Descontado", "Tipo Vehiculo", "Patente", "Acoplado", "Total Aplicados", "Unidad Total Aplicados" }, new string[] { "Caracteristica", "Resultado Calado", "Resultado Camara", "Certificado", "Resultado Reconsideracion", "Certificado Reconsideracion", "Netos", "Unidad Netos", "Descuento", "Unidad Descuento", "Aplicados", "Unidad Aplicados", "Porcentaje Descuento" }, new string[] { "Fecha", "Contrato", "Kg Aplicados", "Unidad" }, "Reporte Carta de Porte Detalle (Nro. " + cartaporteId + ")");
            }
            catch (InfoCustomException)
            {
                throw;
            }
            catch (ValidationCustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public Pdf DownloadPDFCalidad(string proveedor, string cartaporteId)
        {
            try
            {
                CartaPorteDetallePDFWSMOAResponse data = (CartaPorteDetallePDFWSMOAResponse)new CartaPorteDetallePDFConsumerMOA().request(proveedor, cartaporteId);

                try
                {
                    PDFResponse pdfExport = PDFExport.ToPDF("Calidad Carta de Porte(" + cartaporteId + ")", new List<string>() { "Característica", "Calado Result.", "Calado Dto.", "Cámara Result.", "Cámara Dto.", "Kg Netos", "Kg Apli" }, data.datosCalidad);
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
            catch (InfoCustomException)
            {
                throw;
            }
            catch (ValidationCustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public CartaPorteFormularioDropdownsWSMOAResponse GetFormularioDropdowns()
        {
            try
            {
                CartaPorteFormularioDropdownsWSMOAResponse data = new CartaPorteFormularioDesplegablesConsumerMOA().request();
                return data;
            }
            catch (InfoCustomException)
            {
                throw;
            }
            catch (ValidationCustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }


        public List<CartaPorteFoto> GetFotos(string cartaPorteId)
        {
            try
            {
                return scatoConsumer.ObtenerFotoCartaPorte(cartaPorteId);
            }
            catch (InfoCustomException)
            {
                throw;
            }
            catch (ValidationCustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public List<CartaPorteFoto> GetFotos(List<string> cartaPorteIds)
        {
            try
            {
                return scatoConsumer.ObtenerFotoCartasPorte(cartaPorteIds);
            }
            catch (InfoCustomException)
            {
                throw;
            }
            catch (ValidationCustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }


        public CartaPorteCTGWSMOAResponse GetDataCTG(string valor)
        {
            try
            {
                decimal valorDecimal = 0;

                try
                {
                    valorDecimal = Convert.ToDecimal(valor);
                }
                catch
                {
                    throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorIncorrecto, "Nro. CTG"));
                }

                CartaPorteCTGWSMOAResponse data = new CartaPorteFormularioCTGConsumerMOA().request(valorDecimal);
                ValidarRespuesta(data);
                return data;
            }
            catch (InfoCustomException)
            {
                throw;
            }
            catch (ValidationCustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public byte[] GetCompletedPDFTemplate(CCPPFormulario formulario, int paginaSeleccionada, byte[] archivoBytes)
        {

            try
            {
                List<CCPPFormularioElementPosition> valores = GetPositions(formulario);

                int paginaHasta = paginaSeleccionada + 3;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (PdfReader reader = new PdfReader(archivoBytes))
                    {
                        var pageCount = reader.NumberOfPages;

                        if (paginaHasta > pageCount)
                        {
                            throw new InfoCustomException(String.Format(InfoMsg.RangoPaginasNoValido, pageCount));
                        }

                        var pageSize = reader.GetPageSize(1);
                        reader.SelectPages(paginaSeleccionada + "-" + paginaHasta);

                        using (PdfStamper stamper = new PdfStamper(reader, ms))
                        {
                            foreach (int page in Enumerable.Range(1, 4))
                            {
                                PdfContentByte cb = stamper.GetOverContent(page);

                                foreach (CCPPFormularioElementPosition valor in valores)
                                {
                                    BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

                                    cb.SetColorFill(BaseColor.BLACK);
                                    cb.SetFontAndSize(bf, size: 10);
                                    cb.BeginText();
                                    try
                                    {
                                        cb.ShowTextAligned(Element.ALIGN_LEFT, valor.value, valor.x, (int)(pageSize.Height - valor.y), 0);
                                    }
                                    catch { }
                                    cb.EndText();
                                    cb.Fill();
                                }
                            }

                        }

                    }
                    return ms.ToArray();
                }
            }
            catch (InfoCustomException)
            {
                throw;
            }
            catch (ValidationCustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }


        public byte[] GetTemplate(CCPPFormulario formulario)
        {
            Document document = new Document(PageSize.A4, -1, -1, -1, -1);
            try
            {
                List<CCPPFormularioElementPosition> valores = GetPositions(formulario);

                using (MemoryStream msNewDoc = new MemoryStream())
                {

                    //MP Modifico está lógica para que abra el documento en memoria de otra manera. Hacíendolo así como estaba antes dejó de funcionar
                    //https://stackoverflow.com/questions/24044799/itextsharp-is-giving-me-the-error-pdf-header-signature-not-found
                    //document.Open();
                    ////document.Add(new iTextSharp.text.Chunk(""));
                    //foreach (int page in Enumerable.Range(1, 4))
                    //{
                    //    document.NewPage();
                    //    document.Add(new iTextSharp.text.Chunk(""));
                    //}
                    //document.Close();

                    using (PdfWriter wri = PdfWriter.GetInstance(document, msNewDoc))
                    {
                        document.Open();//Open Document to write
                        foreach (int page in Enumerable.Range(1, 4))
                        {
                            document.NewPage();
                            document.Add(new iTextSharp.text.Chunk(""));
                        }
                        document.Close();

                    }

                    byte[] result = msNewDoc.ToArray();

                    using (MemoryStream ms = new MemoryStream())
                    {

                        using (PdfReader reader = new PdfReader(result))
                        {
                            var pageSize = reader.GetPageSize(1);
                            reader.SelectPages("1-4");

                            using (PdfStamper stamper = new PdfStamper(reader, ms))
                            {
                                foreach (int page in Enumerable.Range(1, 4))
                                {
                                    PdfContentByte cb = stamper.GetOverContent(page);

                                    foreach (CCPPFormularioElementPosition valor in valores)
                                    {
                                        BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

                                        cb.SetColorFill(BaseColor.BLACK);
                                        cb.SetFontAndSize(bf, size: 10);
                                        cb.BeginText();
                                        try
                                        {
                                            cb.ShowTextAligned(Element.ALIGN_LEFT, valor.value, valor.x, (int)(pageSize.Height - valor.y), 0);
                                        }
                                        catch { }
                                        cb.EndText();
                                        cb.Fill();
                                    }
                                }
                            }
                        }
                        return ms.ToArray();
                    }
                }
            }
            catch (InfoCustomException)
            {
                throw;
            }
            catch (ValidationCustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        private void ValidarRespuesta(CartaPorteWSMOAResponse data)
        {
            if (data == null)
            {
                throw new ValidationCustomException(ErrorMsg.Error);
            }

            if (data.error != null && data.error.codigo != null && data.error.codigo != "" && data.error.codigo != "11" && data.error.codigo != "00")
            {
                throw new ValidationCustomException(data.error.descripcion);
            }

            if (data.cartasPorte == null || data.cartasPorte.Count == 0)
            {
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Cartas de Porte"));
            }
        }

        private void ValidarRespuesta(CartaPorteDescargaWSMOAResponse data)
        {
            if (data == null)
            {
                throw new ValidationCustomException(ErrorMsg.Error);
            }
            if (data.error != null && data.error.codigo != null && data.error.codigo != "" && data.error.codigo != "11" && data.error.codigo != "00")
            {
                throw new ValidationCustomException(data.error.descripcion);
            }
            if (data.cartasPorte == null || data.cartasPorte.Count == 0)
            {
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Cartas de Porte"));
            }
        }

        private void ValidarRespuesta(CartaPorteDescargaExcelWSMOAResponse data)
        {
            if (data == null)
            {
                throw new ValidationCustomException(ErrorMsg.Error);
            }

            if (data.error != null && data.error.codigo != null && data.error.codigo != "" && data.error.codigo != "11" && data.error.codigo != "00")
            {
                throw new ValidationCustomException(data.error.descripcion);
            }

            if (data.cartasPorte == null || data.cartasPorte.Count == 0)
            {
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Cartas de Porte"));
            }
        }

        private void ValidarRespuesta(CartaPorteExcelWSMOAResponse data)
        {
            if (data == null)
                throw new ValidationCustomException(ErrorMsg.Error);
            if (data.error != null && data.error.codigo != null && data.error.codigo != "" && data.error.codigo != "11" && data.error.codigo != "00")
                throw new ValidationCustomException(data.error.descripcion);
            if (data.cartasPorte == null || data.cartasPorte.Count == 0)
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Cartas de Porte"));
        }

        private void ValidarRespuesta(CartaPorteCTGWSMOAResponse data)
        {
            if (data == null)
                throw new ValidationCustomException(ErrorMsg.Error);
            if (data.mensaje != null && data.mensaje.Count > 0 && data.mensaje.ElementAt(0).codError == "Z2(099)")
            {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorIncorrecto, "Nro. CTG"));
            }
            if (data.mensaje != null && data.mensaje.Count > 0 && data.mensaje.ElementAt(0).msgError != "")
            {
                throw new ValidationCustomException(data.mensaje.ElementAt(0).msgError);
            }
        }

        private List<CCPPFormularioElementPosition> GetPositions(CCPPFormulario formulario)
        {


            DateTime? fechaDate = null;
            if (formulario.fechaCarga != null && formulario.fechaCarga != "")
            {
                fechaDate = DataFormatter.StringToDateTime(formulario.fechaCarga, "Fecha Carga");
            }

            List<CCPPFormularioElementPosition> valoresPos = new List<CCPPFormularioElementPosition> {
                new CCPPFormularioElementPosition(FormatearValorCantWords(formulario.nroCTG, 25),186,92),
                new CCPPFormularioElementPosition(FormatearValorCantWords(formulario.nroRenspa, 25),299,92),
                new CCPPFormularioElementPosition(GetDiaFecha(fechaDate),508,80),
                new CCPPFormularioElementPosition(GetMesFecha(fechaDate),524,80),
                new CCPPFormularioElementPosition(GetAnioFecha(fechaDate),539,80),
                new CCPPFormularioElementPosition(FormatearValorCantWords(formulario.intermediario, 25),182,158),
                new CCPPFormularioElementPosition(FormatearValorInterCuit(formulario.cuitIntermediario),470,158),
                new CCPPFormularioElementPosition(FormatearValorCantWords(formulario.remitenteComercial, 25),182,177),
                new CCPPFormularioElementPosition(FormatearValorInterCuit(formulario.cuitRemitenteComercial),470,177),
                new CCPPFormularioElementPosition(FormatearValorCantWords(formulario.corredorComprador, 25),182,197),
                new CCPPFormularioElementPosition(FormatearValorInterCuit(formulario.cuitCorredorComprador),470,197),
                new CCPPFormularioElementPosition(FormatearValorCantWords(formulario.mercadoATermino, 25),182,216),
                new CCPPFormularioElementPosition(FormatearValorInterCuit(formulario.cuitMercadoATermino),470,216),
                new CCPPFormularioElementPosition(FormatearValorCantWords(formulario.corredorVendedor, 25),182,236),
                new CCPPFormularioElementPosition(FormatearValorInterCuit(formulario.cuitCorredorVendedor),470,236),
                new CCPPFormularioElementPosition(FormatearValorCantWords(formulario.representanteEntregador, 25),182,257),
                new CCPPFormularioElementPosition(FormatearValorInterCuit(formulario.cuitRepresentanteEntregador),470,257),
                new CCPPFormularioElementPosition(FormatearValorCantWords(formulario.destinatario, 25),182,277),
                new CCPPFormularioElementPosition(FormatearValorInterCuit(formulario.cuitDestinatario),470,277),
                new CCPPFormularioElementPosition(FormatearValorCantWords(formulario.destino, 40),182,297),
                new CCPPFormularioElementPosition(FormatearValorInterCuit(formulario.cuitDestino),470,297),
                new CCPPFormularioElementPosition(FormatearValorCantWords(formulario.intermediarioFlete, 25), 182,317),
                new CCPPFormularioElementPosition(FormatearValorInterCuit(formulario.cuitIntermediarioFlete),470,317),
                new CCPPFormularioElementPosition(FormatearValorCantWords(formulario.transportista, 25),182,336),
                new CCPPFormularioElementPosition(FormatearValorInterCuit(formulario.cuitTransportista),470,336),
                new CCPPFormularioElementPosition(FormatearValorCantWords(formulario.chofer, 25),182,352),
                new CCPPFormularioElementPosition(FormatearValorInterCuit(formulario.cuitCuilChofer),470,352),
                new CCPPFormularioElementPosition(FormatearValorCantWords(formulario.granoEspecie, 8),128,380),
                new CCPPFormularioElementPosition(FormatearValorCantWords(formulario.tipo, 8),232,380),
                new CCPPFormularioElementPosition(FormatearValorCantWords(formulario.cosecha, 8),354,380),
                new CCPPFormularioElementPosition(FormatearValorCantWords(formulario.nroContrato, 8),492,380),
                new CCPPFormularioElementPosition(FormatearCheckBox(formulario.cargaPesadaDestino),133,410),
                new CCPPFormularioElementPosition(FormatearCheckBox(formulario.declaracionCalidad),258,396),
                new CCPPFormularioElementPosition(FormatearCheckBox(formulario.conforme),258,413),
                new CCPPFormularioElementPosition(FormatearCheckBox(formulario.condicional),258,430),
                new CCPPFormularioElementPosition(FormatearValorCantWords(formulario.pesoBruto, 10),350,396),
                new CCPPFormularioElementPosition(FormatearValorCantWords(formulario.pesoTara, 10),350,413),
                new CCPPFormularioElementPosition(FormatearValorCantWords(formulario.pesoNeto, 10),350,431),
                new CCPPFormularioElementPosition(formulario.observaciones,410,408),
                new CCPPFormularioElementPosition(FormatearValorCantWords(formulario.establecimiento, 24),125,430),
                new CCPPFormularioElementPosition(FormatearValorCantWords(formulario.direccion, 26),100,470),
                new CCPPFormularioElementPosition(FormatearValorCantWords(formulario.localidad, 22),422,460),
                new CCPPFormularioElementPosition(FormatearValorCantWords(formulario.provincia, 22),422,475),
                new CCPPFormularioElementPosition(FormatearValorCantWords(formulario.direccionDestino, 24),72,510),
                new CCPPFormularioElementPosition(FormatearValorCantWords(formulario.localidadDestino, 24),356,495),
                new CCPPFormularioElementPosition(FormatearValorCantWords(formulario.provinciaDestino, 24),356,510),
                new CCPPFormularioElementPosition(FormatearValorCantWords(formulario.pagadorFlete,22),345,530),
                new CCPPFormularioElementPosition(FormatearValorCantWords(formulario.camion1, 8),92,547),
                new CCPPFormularioElementPosition(FormatearValorCantWords(formulario.camion2, 8),165,547),
                new CCPPFormularioElementPosition(FormatearValorCantWords(formulario.camion3, 8),235,547),
                new CCPPFormularioElementPosition(FormatearValorCantWords(formulario.acoplado, 6),92,565),
                new CCPPFormularioElementPosition(FormatearValorCantWords(formulario.tarifaReferencia, 6),240,565),
                new CCPPFormularioElementPosition(FormatearValorCantWords(formulario.kmRecorrer, 6),92,582),
                new CCPPFormularioElementPosition(FormatearValorCantWords(formulario.tarifa, 6),240,582),
            };
            return valoresPos;
        }

        private string FormatearValorCantWords(string valor, int cantWords)
        {

            try
            {
                if (valor.Trim().Length > cantWords)
                {
                    string modValor = valor.Substring(0, cantWords - 1).Trim();
                    if (valor.ElementAt(cantWords - 1).Equals(" "))
                    {
                        modValor = modValor.Substring(0, cantWords - 3) + ".";
                    }
                    else
                    {
                        modValor += ".";
                    }
                    return modValor;
                }
            }
            catch { }
            return valor;
        }

        private string FormatearValorInterCuit(string valor)
        {
            try
            {
                return valor.Substring(0, 16).Trim();
            }
            catch { }
            return valor;
        }

        private string GetDiaFecha(DateTime? fecha)
        {
            try
            {
                return fecha.Value.Day.ToString("00");
            }
            catch
            {
                return "";
            }
        }

        private string GetMesFecha(DateTime? fecha)
        {
            try
            {
                return fecha.Value.Month.ToString("00");
            }
            catch
            {
                return "";
            }
        }

        private string GetAnioFecha(DateTime? fecha)
        {
            try
            {
                return fecha.Value.Year.ToString();
            }
            catch
            {
                return "";
            }
        }

        private string FormatearCheckBox(string val)
        {
            return val.ToLowerInvariant() == "true" ? "X" : "";
        }
    }
}
