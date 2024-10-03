using BigExcelCreator;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SustitucionMOAModel.Attributes;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Models.WSMapMOA.CartaPorte.Detalle;
using SustitucionMOAModel.Models.WSMapMOA.Contrato.Detalle;
using SustitucionMOAModel.Models.WSMapMOA.CuentaCorriente;
using SustitucionMOAModel.Models.WSMapMOA.Flete;
using SustitucionMOAModel.Models.WSMapMOA.Pago.Detalle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using FontSize = DocumentFormat.OpenXml.Spreadsheet.FontSize;
using X14 = DocumentFormat.OpenXml.Office2010.Excel;
using X15 = DocumentFormat.OpenXml.Office2013.Excel;
namespace SustitucionMOAUtils.Export
{
    public static class ExcelExport
    {
        [Obsolete("Esta funcionalidad está deprecada, ya que utiliza un html en vez de la funcionalidad de OpenXml.")]
        public static string ToExcel(object dataList, string[] headers, string titulo)
        {

            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            if (!string.IsNullOrEmpty(titulo))
            {
                var tituloRow = new System.Data.DataTable("Titulo");
                tituloRow.Columns.Add("Titulo");
                tituloRow.Rows.Add(titulo);
                var gridTitulo = new GridView();
                gridTitulo.DataSource = tituloRow;
                gridTitulo.GridLines = GridLines.None;
                gridTitulo.Font.Bold = true;
                gridTitulo.Font.Size = 14;
                gridTitulo.ShowHeader = false;
                gridTitulo.DataBind();
                gridTitulo.Rows[0].Cells[0].ColumnSpan = headers.Count();
                gridTitulo.RenderControl(htw);
            }

            var gridData = new GridView();
            gridData.DataSource = dataList;
            gridData.DataBind();
            for (int i = 0; i < headers.Length; i++)
            {
                gridData.HeaderRow.Cells[i].Text = headers[i];
            }

            gridData.RenderControl(htw);

            return sw.ToString();
        }
        [Obsolete("Esta funcionalidad está deprecada, ya que utiliza un html en vez de la funcionalidad de OpenXml.")]
        public static string ToExcelCartaPorteDetalle(object entregasDescargasList, object datosCalidadList, object aplicacionesList, string[] entregasDescargasHeaders, string[] datosCalidadHeaders, string[] aplicacionesHeaders, string titulo)
        {

            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            var tituloRow = new System.Data.DataTable("Titulo");
            tituloRow.Columns.Add("Titulo");
            tituloRow.Rows.Add(titulo);
            var gridTitulo = new GridView();
            gridTitulo.DataSource = tituloRow;
            gridTitulo.GridLines = GridLines.None;
            gridTitulo.Font.Bold = true;
            gridTitulo.Font.Size = 14;
            gridTitulo.ShowHeader = false;
            gridTitulo.DataBind();
            gridTitulo.Rows[0].Cells[0].ColumnSpan = entregasDescargasHeaders.Count();
            gridTitulo.RenderControl(htw);

            var gridData = new GridView();
            gridData.DataSource = " ";
            gridData.DataBind();
            gridData.GridLines = GridLines.None;
            gridData.HeaderRow.Cells[0].Text = " ";
            gridData.RenderControl(htw);

            List<EntregaDescarga> entregasDescargas = (List<EntregaDescarga>)entregasDescargasList;

            var subtituloEntregasRow = new System.Data.DataTable("Titulo");
            subtituloEntregasRow.Columns.Add("Titulo");
            subtituloEntregasRow.Rows.Add("Entregas y Descargas");
            var subgridTitulo = new GridView();
            subgridTitulo.DataSource = subtituloEntregasRow;
            subgridTitulo.GridLines = GridLines.None;
            subgridTitulo.Font.Bold = true;
            subgridTitulo.Font.Size = 12;
            subgridTitulo.ShowHeader = false;
            subgridTitulo.DataBind();
            subgridTitulo.Rows[0].Cells[0].ColumnSpan = entregasDescargasHeaders.Count();
            subgridTitulo.RenderControl(htw);

            if (entregasDescargas.Count != 0)
            {

                gridData.DataSource = entregasDescargasList;
                gridData.DataBind();
                gridData.GridLines = GridLines.Both;

                for (int i = 0; i < entregasDescargasHeaders.Length; i++)
                {
                    gridData.HeaderRow.Cells[i].Text = entregasDescargasHeaders[i];
                }

                gridData.RenderControl(htw);
            }

            gridData.DataSource = " ";
            gridData.DataBind();
            gridData.GridLines = GridLines.None;
            gridData.HeaderRow.Cells[0].Text = " ";
            gridData.RenderControl(htw);

            List<SustitucionMOAModel.Models.WSMapMOA.CartaPorte.Detalle.Calidad> datosCalidad = (List<SustitucionMOAModel.Models.WSMapMOA.CartaPorte.Detalle.Calidad>)datosCalidadList;

            var subtituloCalidadRow = new System.Data.DataTable("Titulo");
            subtituloCalidadRow.Columns.Add("Titulo");
            subtituloCalidadRow.Rows.Add("Calidad");
            subgridTitulo.DataSource = subtituloCalidadRow;
            subgridTitulo.GridLines = GridLines.None;
            subgridTitulo.Font.Bold = true;
            subgridTitulo.Font.Size = 12;
            subgridTitulo.ShowHeader = false;
            subgridTitulo.DataBind();
            subgridTitulo.Rows[0].Cells[0].ColumnSpan = entregasDescargasHeaders.Count();
            subgridTitulo.RenderControl(htw);

            if (datosCalidad.Count != 0)
            {
                gridData.DataSource = datosCalidadList;
                gridData.DataBind();
                gridData.GridLines = GridLines.Both;

                for (int i = 0; i < datosCalidadHeaders.Length; i++)
                {
                    gridData.HeaderRow.Cells[i].Text = datosCalidadHeaders[i];
                }

                gridData.RenderControl(htw);
            }

            gridData.DataSource = " ";
            gridData.DataBind();
            gridData.GridLines = GridLines.None;
            gridData.HeaderRow.Cells[0].Text = " ";
            gridData.RenderControl(htw);

            var subtituloAplicacionRow = new System.Data.DataTable("Titulo");
            subtituloAplicacionRow.Columns.Add("Titulo");
            subtituloAplicacionRow.Rows.Add("Aplicacion");
            subgridTitulo.DataSource = subtituloAplicacionRow;
            subgridTitulo.GridLines = GridLines.None;
            subgridTitulo.Font.Bold = true;
            subgridTitulo.Font.Size = 12;
            subgridTitulo.ShowHeader = false;
            subgridTitulo.DataBind();
            subgridTitulo.Rows[0].Cells[0].ColumnSpan = entregasDescargasHeaders.Count();
            subgridTitulo.RenderControl(htw);

            List<SustitucionMOAModel.Models.WSMapMOA.CartaPorte.Detalle.Aplicacion> aplicaciones = (List<SustitucionMOAModel.Models.WSMapMOA.CartaPorte.Detalle.Aplicacion>)aplicacionesList;

            if (aplicaciones.Count != 0)
            {
                gridData.DataSource = aplicacionesList;
                gridData.DataBind();
                gridData.GridLines = GridLines.Both;
                for (int i = 0; i < aplicacionesHeaders.Length; i++)
                {
                    gridData.HeaderRow.Cells[i].Text = aplicacionesHeaders[i];
                }

                gridData.RenderControl(htw);
            }

            return sw.ToString();
        }
        [Obsolete("Esta funcionalidad está deprecada, ya que utiliza un html en vez de la funcionalidad de OpenXml.")]
        public static string ToExcelContratoDetalle(object ampliacionesAnulacionesList, object aplicacionesList, List<CalidadContratoDetalleExcel> calidadList, object caracteristicasList, object condicionesPagoList, object fijacionesList, object hijosList, object liquidacionesList, object pagosList, object resumenList, string[] ampliacionesAnulacionesHeaders, string[] aplicacionesHeaders, string[] calidadHeaders, string[] caracteristicasHeaders, string[] condicionesPagoHeaders, string[] fijacionesHeaders, string[] hijosHeaders, string[] liquidacionesHeaders, string[] pagosHeaders, string[] resumenHeaders, string titulo)
        {

            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            var tituloRow = new System.Data.DataTable("Titulo");
            tituloRow.Columns.Add("Titulo");
            tituloRow.Rows.Add(titulo);
            var gridTitulo = new GridView();
            gridTitulo.DataSource = tituloRow;
            gridTitulo.GridLines = GridLines.None;
            gridTitulo.Font.Bold = true;
            gridTitulo.Font.Size = 14;
            gridTitulo.ShowHeader = false;
            gridTitulo.DataBind();
            gridTitulo.Rows[0].Cells[0].ColumnSpan = ampliacionesAnulacionesHeaders.Count();
            gridTitulo.RenderControl(htw);

            var gridData = new GridView();
            gridData.DataSource = " ";
            gridData.DataBind();
            gridData.GridLines = GridLines.None;
            gridData.HeaderRow.Cells[0].Text = " ";
            gridData.RenderControl(htw);

            var subtituloAmpliacionesRow = new System.Data.DataTable("Titulo");
            subtituloAmpliacionesRow.Columns.Add("Titulo");
            subtituloAmpliacionesRow.Rows.Add("Ampliaciones y Anulaciones");
            var subgridTitulo = new GridView();
            subgridTitulo.DataSource = subtituloAmpliacionesRow;
            subgridTitulo.GridLines = GridLines.None;
            subgridTitulo.Font.Bold = true;
            subgridTitulo.Font.Size = 12;
            subgridTitulo.ShowHeader = false;
            subgridTitulo.DataBind();
            subgridTitulo.Rows[0].Cells[0].ColumnSpan = ampliacionesAnulacionesHeaders.Count();
            subgridTitulo.RenderControl(htw);

            List<AmpliacionAnulacion> ampliaciones = (List<AmpliacionAnulacion>)ampliacionesAnulacionesList;

            if (ampliaciones.Count() == 0)
            {
                ampliaciones.Add(new AmpliacionAnulacion());

                gridData.DataSource = ampliaciones;
            }
            else
            {
                gridData.DataSource = ampliacionesAnulacionesList;
            }

            gridData.DataBind();
            gridData.GridLines = GridLines.Both;

            for (int i = 0; i < ampliacionesAnulacionesHeaders.Length; i++)
            {
                gridData.HeaderRow.Cells[i].Text = ampliacionesAnulacionesHeaders[i];
            }

            gridData.RenderControl(htw);

            gridData.DataSource = " ";
            gridData.DataBind();
            gridData.GridLines = GridLines.None;
            gridData.HeaderRow.Cells[0].Text = " ";
            gridData.RenderControl(htw);

            var subtituloAplicacionesRow = new System.Data.DataTable("Titulo");
            subtituloAplicacionesRow.Columns.Add("Titulo");
            subtituloAplicacionesRow.Rows.Add("Aplicaciones");
            subgridTitulo.DataSource = subtituloAplicacionesRow;
            subgridTitulo.GridLines = GridLines.None;
            subgridTitulo.Font.Bold = true;
            subgridTitulo.Font.Size = 12;
            subgridTitulo.ShowHeader = false;
            subgridTitulo.DataBind();
            subgridTitulo.Rows[0].Cells[0].ColumnSpan = aplicacionesHeaders.Count();
            subgridTitulo.RenderControl(htw);

            List<SustitucionMOAModel.Models.WSMapMOA.Contrato.Detalle.Aplicacion> aplicaciones = (List<SustitucionMOAModel.Models.WSMapMOA.Contrato.Detalle.Aplicacion>)aplicacionesList;

            if (aplicaciones.Count() == 0)
            {
                aplicaciones.Add(new SustitucionMOAModel.Models.WSMapMOA.Contrato.Detalle.Aplicacion());

                gridData.DataSource = aplicaciones;
            }
            else
            {
                gridData.DataSource = aplicacionesList;
            }

            gridData.DataBind();
            gridData.GridLines = GridLines.Both;

            for (int i = 0; i < aplicacionesHeaders.Length; i++)
            {
                gridData.HeaderRow.Cells[i].Text = aplicacionesHeaders[i];
            }

            gridData.RenderControl(htw);

            gridData.DataSource = " ";
            gridData.DataBind();
            gridData.GridLines = GridLines.None;
            gridData.HeaderRow.Cells[0].Text = " ";
            gridData.RenderControl(htw);

            var subtituloCalidadRow = new System.Data.DataTable("Titulo");
            subtituloCalidadRow.Columns.Add("Titulo");
            subtituloCalidadRow.Rows.Add("Calidad");
            subgridTitulo.DataSource = subtituloCalidadRow;
            subgridTitulo.GridLines = GridLines.None;
            subgridTitulo.Font.Bold = true;
            subgridTitulo.Font.Size = 12;
            subgridTitulo.ShowHeader = false;
            subgridTitulo.DataBind();
            subgridTitulo.Rows[0].Cells[0].ColumnSpan = calidadHeaders.Count();
            subgridTitulo.RenderControl(htw);


            if (calidadList.Count() == 0)
            {
                calidadList.Add(new CalidadContratoDetalleExcel());

                gridData.DataSource = calidadList;
            }
            else
            {
                gridData.DataSource = calidadList;
            }

            gridData.DataBind();
            gridData.GridLines = GridLines.Both;
            for (int i = 0; i < calidadHeaders.Length; i++)
            {
                gridData.HeaderRow.Cells[i].Text = calidadHeaders[i];
            }

            gridData.RenderControl(htw);

            gridData.DataSource = " ";
            gridData.DataBind();
            gridData.GridLines = GridLines.None;
            gridData.HeaderRow.Cells[0].Text = " ";
            gridData.RenderControl(htw);

            var subtituloCaracteristicasRow = new System.Data.DataTable("Titulo");
            subtituloCaracteristicasRow.Columns.Add("Titulo");
            subtituloCaracteristicasRow.Rows.Add("Caracteristicas");
            subgridTitulo.DataSource = subtituloCaracteristicasRow;
            subgridTitulo.GridLines = GridLines.None;
            subgridTitulo.Font.Bold = true;
            subgridTitulo.Font.Size = 12;
            subgridTitulo.ShowHeader = false;
            subgridTitulo.DataBind();
            subgridTitulo.Rows[0].Cells[0].ColumnSpan = caracteristicasHeaders.Count();
            subgridTitulo.RenderControl(htw);

            List<Caracteristica> caracteristicas = (List<Caracteristica>)caracteristicasList;

            if (caracteristicas.Count() == 0)
            {
                caracteristicas.Add(new Caracteristica());

                gridData.DataSource = caracteristicas;
            }
            else
            {
                gridData.DataSource = caracteristicasList;
            }

            gridData.DataBind();
            gridData.GridLines = GridLines.Both;
            for (int i = 0; i < caracteristicasHeaders.Length; i++)
            {
                gridData.HeaderRow.Cells[i].Text = caracteristicasHeaders[i];
            }

            gridData.RenderControl(htw);

            gridData.DataSource = " ";
            gridData.DataBind();
            gridData.GridLines = GridLines.None;
            gridData.HeaderRow.Cells[0].Text = " ";
            gridData.RenderControl(htw);

            var subtituloCondicionesPagoRow = new System.Data.DataTable("Titulo");
            subtituloCondicionesPagoRow.Columns.Add("Titulo");
            subtituloCondicionesPagoRow.Rows.Add("Condiciones de Pago");
            subgridTitulo.DataSource = subtituloCondicionesPagoRow;
            subgridTitulo.GridLines = GridLines.None;
            subgridTitulo.Font.Bold = true;
            subgridTitulo.Font.Size = 12;
            subgridTitulo.ShowHeader = false;
            subgridTitulo.DataBind();
            subgridTitulo.Rows[0].Cells[0].ColumnSpan = condicionesPagoHeaders.Count();
            subgridTitulo.RenderControl(htw);

            List<string> condicionesPagos = (List<string>)condicionesPagoList;

            if (condicionesPagos.Count() == 0)
            {
                condicionesPagos.Add("");

                gridData.DataSource = condicionesPagos;
            }
            else
            {
                gridData.DataSource = condicionesPagoList;
            }

            gridData.DataBind();
            gridData.GridLines = GridLines.Both;
            for (int i = 0; i < condicionesPagoHeaders.Length; i++)
            {
                gridData.HeaderRow.Cells[i].Text = condicionesPagoHeaders[i];
            }

            gridData.RenderControl(htw);

            gridData.DataSource = " ";
            gridData.DataBind();
            gridData.GridLines = GridLines.None;
            gridData.HeaderRow.Cells[0].Text = " ";
            gridData.RenderControl(htw);

            var subtituloFijacionesRow = new System.Data.DataTable("Titulo");
            subtituloFijacionesRow.Columns.Add("Titulo");
            subtituloFijacionesRow.Rows.Add("Fijaciones");
            subgridTitulo.DataSource = subtituloFijacionesRow;
            subgridTitulo.GridLines = GridLines.None;
            subgridTitulo.Font.Bold = true;
            subgridTitulo.Font.Size = 12;
            subgridTitulo.ShowHeader = false;
            subgridTitulo.DataBind();
            subgridTitulo.Rows[0].Cells[0].ColumnSpan = fijacionesHeaders.Count();
            subgridTitulo.RenderControl(htw);

            List<Fijacion> fijaciones = (List<Fijacion>)fijacionesList;

            if (fijaciones.Count() == 0)
            {
                fijaciones.Add(new Fijacion());

                gridData.DataSource = fijaciones;
            }
            else
            {
                gridData.DataSource = fijacionesList;
            }

            gridData.DataBind();
            gridData.GridLines = GridLines.Both;
            for (int i = 0; i < fijacionesHeaders.Length; i++)
            {
                gridData.HeaderRow.Cells[i].Text = fijacionesHeaders[i];
            }

            gridData.RenderControl(htw);

            gridData.DataSource = " ";
            gridData.DataBind();
            gridData.GridLines = GridLines.None;
            gridData.HeaderRow.Cells[0].Text = " ";
            gridData.RenderControl(htw);

            var subtituloHijosRow = new System.Data.DataTable("Titulo");
            subtituloHijosRow.Columns.Add("Titulo");
            subtituloHijosRow.Rows.Add("Hijos");
            subgridTitulo.DataSource = subtituloHijosRow;
            subgridTitulo.GridLines = GridLines.None;
            subgridTitulo.Font.Bold = true;
            subgridTitulo.Font.Size = 12;
            subgridTitulo.ShowHeader = false;
            subgridTitulo.DataBind();
            subgridTitulo.Rows[0].Cells[0].ColumnSpan = hijosHeaders.Count();
            subgridTitulo.RenderControl(htw);

            List<Hijo> hijos = (List<Hijo>)hijosList;

            if (hijos.Count() == 0)
            {
                hijos.Add(new Hijo());

                gridData.DataSource = hijos;
            }
            else
            {
                gridData.DataSource = hijosList;
            }

            gridData.DataBind();
            gridData.GridLines = GridLines.Both;
            for (int i = 0; i < hijosHeaders.Length; i++)
            {
                gridData.HeaderRow.Cells[i].Text = hijosHeaders[i];
            }

            gridData.RenderControl(htw);

            gridData.DataSource = " ";
            gridData.DataBind();
            gridData.GridLines = GridLines.None;
            gridData.HeaderRow.Cells[0].Text = " ";
            gridData.RenderControl(htw);

            var subtituloLiquidacionesRow = new System.Data.DataTable("Titulo");
            subtituloLiquidacionesRow.Columns.Add("Titulo");
            subtituloLiquidacionesRow.Rows.Add("Liquidaciones");
            subgridTitulo.DataSource = subtituloLiquidacionesRow;
            subgridTitulo.GridLines = GridLines.None;
            subgridTitulo.Font.Bold = true;
            subgridTitulo.Font.Size = 12;
            subgridTitulo.ShowHeader = false;
            subgridTitulo.DataBind();
            subgridTitulo.Rows[0].Cells[0].ColumnSpan = liquidacionesHeaders.Count();
            subgridTitulo.RenderControl(htw);

            List<Liquidacion> liquidaciones = (List<Liquidacion>)liquidacionesList;

            if (liquidaciones.Count() == 0)
            {
                liquidaciones.Add(new Liquidacion());

                gridData.DataSource = liquidaciones;
            }
            else
            {
                gridData.DataSource = liquidacionesList;
            }

            gridData.DataBind();
            gridData.GridLines = GridLines.Both;
            for (int i = 0; i < liquidacionesHeaders.Length; i++)
            {
                gridData.HeaderRow.Cells[i].Text = liquidacionesHeaders[i];
            }

            gridData.RenderControl(htw);

            gridData.DataSource = " ";
            gridData.DataBind();
            gridData.GridLines = GridLines.None;
            gridData.HeaderRow.Cells[0].Text = " ";
            gridData.RenderControl(htw);

            var subtituloPagosRow = new System.Data.DataTable("Titulo");
            subtituloPagosRow.Columns.Add("Titulo");
            subtituloPagosRow.Rows.Add("Pagos");
            subgridTitulo.DataSource = subtituloPagosRow;
            subgridTitulo.GridLines = GridLines.None;
            subgridTitulo.Font.Bold = true;
            subgridTitulo.Font.Size = 12;
            subgridTitulo.ShowHeader = false;
            subgridTitulo.DataBind();
            subgridTitulo.Rows[0].Cells[0].ColumnSpan = pagosHeaders.Count();
            subgridTitulo.RenderControl(htw);

            List<Pago> pagos = (List<Pago>)pagosList;

            if (pagos.Count() == 0)
            {
                pagos.Add(new Pago());

                gridData.DataSource = pagos;
            }
            else
            {
                gridData.DataSource = pagosList;
            }

            gridData.DataBind();
            gridData.GridLines = GridLines.Both;
            for (int i = 0; i < pagosHeaders.Length; i++)
            {
                gridData.HeaderRow.Cells[i].Text = pagosHeaders[i];
            }

            gridData.RenderControl(htw);

            gridData.DataSource = " ";
            gridData.DataBind();
            gridData.GridLines = GridLines.None;
            gridData.HeaderRow.Cells[0].Text = " ";
            gridData.RenderControl(htw);

            var subtituloResumenRow = new System.Data.DataTable("Titulo");
            subtituloResumenRow.Columns.Add("Titulo");
            subtituloResumenRow.Rows.Add("Resumen");
            subgridTitulo.DataSource = subtituloResumenRow;
            subgridTitulo.GridLines = GridLines.None;
            subgridTitulo.Font.Bold = true;
            subgridTitulo.Font.Size = 12;
            subgridTitulo.ShowHeader = false;
            subgridTitulo.DataBind();
            subgridTitulo.Rows[0].Cells[0].ColumnSpan = resumenHeaders.Count();
            subgridTitulo.RenderControl(htw);

            List<Resumen> resumenes = (List<Resumen>)resumenList;

            if (resumenes.Count() == 0)
            {
                resumenes.Add(new Resumen());

                gridData.DataSource = resumenes;
            }
            else
            {
                gridData.DataSource = resumenList;
            }

            gridData.DataBind();
            gridData.GridLines = GridLines.Both;
            for (int i = 0; i < resumenHeaders.Length; i++)
            {
                gridData.HeaderRow.Cells[i].Text = resumenHeaders[i];
            }

            gridData.RenderControl(htw);

            return sw.ToString();
        }
        [Obsolete("Esta funcionalidad está deprecada, ya que utiliza un html en vez de la funcionalidad de OpenXml.")]
        public static string ToExcelPagoDetalle(object cabecerasList, object salidasList, object vendedoresList, string[] cabecerasHeaders, string[] salidasHeaders, string[] vendedoresHeaders, string titulo)
        {

            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            var tituloRow = new System.Data.DataTable("Titulo");
            tituloRow.Columns.Add("Titulo");
            tituloRow.Rows.Add(titulo);
            var gridTitulo = new GridView();
            gridTitulo.DataSource = tituloRow;
            gridTitulo.GridLines = GridLines.None;
            gridTitulo.Font.Bold = true;
            gridTitulo.Font.Size = 14;
            gridTitulo.ShowHeader = false;
            gridTitulo.DataBind();
            gridTitulo.Rows[0].Cells[0].ColumnSpan = cabecerasHeaders.Count();
            gridTitulo.RenderControl(htw);

            var gridData = new GridView();
            gridData.DataSource = " ";
            gridData.DataBind();
            gridData.GridLines = GridLines.None;
            gridData.HeaderRow.Cells[0].Text = " ";
            gridData.RenderControl(htw);

            var subtituloCabecerasRow = new System.Data.DataTable("Titulo");
            subtituloCabecerasRow.Columns.Add("Titulo");
            subtituloCabecerasRow.Rows.Add("Cabeceras");
            var subgridTitulo = new GridView();
            subgridTitulo.DataSource = subtituloCabecerasRow;
            subgridTitulo.GridLines = GridLines.None;
            subgridTitulo.Font.Bold = true;
            subgridTitulo.Font.Size = 12;
            subgridTitulo.ShowHeader = false;
            subgridTitulo.DataBind();
            subgridTitulo.Rows[0].Cells[0].ColumnSpan = cabecerasHeaders.Count();
            subgridTitulo.RenderControl(htw);

            List<Cabecera> cabeceras = (List<Cabecera>)cabecerasList;

            if (cabeceras.Count() == 0)
            {
                cabeceras.Add(new Cabecera());

                gridData.DataSource = cabeceras;
            }
            else
            {
                gridData.DataSource = cabecerasList;
            }

            gridData.DataBind();
            gridData.GridLines = GridLines.Both;

            for (int i = 0; i < cabecerasHeaders.Length; i++)
            {
                gridData.HeaderRow.Cells[i].Text = cabecerasHeaders[i];
            }

            gridData.RenderControl(htw);

            gridData.DataSource = " ";
            gridData.DataBind();
            gridData.GridLines = GridLines.None;
            gridData.HeaderRow.Cells[0].Text = " ";
            gridData.RenderControl(htw);

            var subtituloSalidasRow = new System.Data.DataTable("Titulo");
            subtituloSalidasRow.Columns.Add("Titulo");
            subtituloSalidasRow.Rows.Add("Salidas");
            subgridTitulo.DataSource = subtituloSalidasRow;
            subgridTitulo.GridLines = GridLines.None;
            subgridTitulo.Font.Bold = true;
            subgridTitulo.Font.Size = 12;
            subgridTitulo.ShowHeader = false;
            subgridTitulo.DataBind();
            subgridTitulo.Rows[0].Cells[0].ColumnSpan = salidasHeaders.Count();
            subgridTitulo.RenderControl(htw);

            List<SalidaExcelDetalle> salidas = (List<SalidaExcelDetalle>)salidasList;

            if (salidas.Count() == 0)
            {
                salidas.Add(new SalidaExcelDetalle());

                gridData.DataSource = salidas;
            }
            else
            {
                gridData.DataSource = salidasList;
            }

            gridData.DataBind();
            gridData.GridLines = GridLines.Both;

            for (int i = 0; i < salidasHeaders.Length; i++)
            {
                gridData.HeaderRow.Cells[i].Text = salidasHeaders[i];
            }

            gridData.RenderControl(htw);

            gridData.DataSource = " ";
            gridData.DataBind();
            gridData.GridLines = GridLines.None;
            gridData.HeaderRow.Cells[0].Text = " ";
            gridData.RenderControl(htw);

            var subtituloVendedoresRow = new System.Data.DataTable("Titulo");
            subtituloVendedoresRow.Columns.Add("Titulo");
            subtituloVendedoresRow.Rows.Add("Vendedores");
            subgridTitulo.DataSource = subtituloVendedoresRow;
            subgridTitulo.GridLines = GridLines.None;
            subgridTitulo.Font.Bold = true;
            subgridTitulo.Font.Size = 12;
            subgridTitulo.ShowHeader = false;
            subgridTitulo.DataBind();
            subgridTitulo.Rows[0].Cells[0].ColumnSpan = vendedoresHeaders.Count();
            subgridTitulo.RenderControl(htw);

            List<Vendedor> vendedores = (List<Vendedor>)vendedoresList;

            if (vendedores.Count() == 0)
            {
                vendedores.Add(new Vendedor());

                gridData.DataSource = vendedores;
            }
            else
            {
                gridData.DataSource = vendedoresList;
            }

            gridData.DataBind();
            gridData.GridLines = GridLines.Both;
            for (int i = 0; i < vendedoresHeaders.Length; i++)
            {
                gridData.HeaderRow.Cells[i].Text = vendedoresHeaders[i];
            }

            gridData.RenderControl(htw);

            return sw.ToString();
        }
        [Obsolete("Esta funcionalidad está deprecada, ya que utiliza un html en vez de la funcionalidad de OpenXml.")]
        public static string ToExcelCuentaCorrienteAgrupada(List<CuentaCorrienteAgrupada> list, string[] headers, string titulo)
        {

            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            var tituloRow = new System.Data.DataTable("Titulo");
            tituloRow.Columns.Add("Titulo");
            tituloRow.Rows.Add(titulo);
            var gridTitulo = new GridView();
            gridTitulo.DataSource = tituloRow;
            gridTitulo.GridLines = GridLines.None;
            gridTitulo.Font.Bold = true;
            gridTitulo.Font.Size = 14;
            gridTitulo.ShowHeader = false;
            gridTitulo.DataBind();
            gridTitulo.Rows[0].Cells[0].ColumnSpan = headers.Count();
            gridTitulo.RenderControl(htw);

            var gridData = new GridView();
            gridData.DataSource = " ";
            gridData.DataBind();
            gridData.GridLines = GridLines.None;
            gridData.HeaderRow.Cells[0].Text = " ";
            gridData.RenderControl(htw);

            foreach (CuentaCorrienteAgrupada ctecta in list)
            {
                var agrupadorRow = new System.Data.DataTable("Agrupador");
                agrupadorRow.Columns.Add("Agrupador");
                agrupadorRow.Rows.Add(ctecta.agrupador);
                var gridAgrupador = new GridView();
                gridAgrupador.DataSource = agrupadorRow;
                gridAgrupador.GridLines = GridLines.None;
                gridAgrupador.Font.Bold = true;
                gridAgrupador.Font.Size = 14;
                gridAgrupador.ShowHeader = false;
                gridAgrupador.DataBind();
                gridAgrupador.Rows[0].Cells[0].ColumnSpan = headers.Count();
                gridAgrupador.RenderControl(htw);

                gridData.DataSource = ctecta.cuentasCorrientes;
                gridData.DataBind();
                gridData.GridLines = GridLines.Both;
                for (int i = 0; i < headers.Length; i++)
                {
                    gridData.HeaderRow.Cells[i].Text = headers[i];
                }
                gridData.RenderControl(htw);

                var totalRow = new System.Data.DataTable("Total");
                totalRow.Columns.Add("Agrupador");
                totalRow.Rows.Add("Total: " + ctecta.total);
                var gridTotal = new GridView();
                gridTotal.DataSource = totalRow;
                gridTotal.GridLines = GridLines.None;
                gridTotal.Font.Bold = true;
                gridTotal.Font.Size = 12;
                gridTotal.ShowHeader = false;
                gridTotal.DataBind();
                gridTotal.Rows[0].Cells[0].ColumnSpan = headers.Count();
                gridTotal.RenderControl(htw);

                gridData.DataSource = " ";
                gridData.DataBind();
                gridData.GridLines = GridLines.None;
                gridData.HeaderRow.Cells[0].Text = " ";
                gridData.RenderControl(htw);

                gridData.DataSource = " ";
                gridData.DataBind();
                gridData.GridLines = GridLines.None;
                gridData.HeaderRow.Cells[0].Text = " ";
                gridData.RenderControl(htw);
            }

            return sw.ToString();
        }
        [Obsolete("Esta funcionalidad está deprecada, ya que utiliza un html en vez de la funcionalidad de OpenXml.")]
        public static string ToExcelCuentaCorrientePartidasAbiertas(List<CuentaCorrienteAgrupada> list, string[] headers, string titulo)
        {

            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            var tituloRow = new System.Data.DataTable("Titulo");
            tituloRow.Columns.Add("Titulo");
            tituloRow.Rows.Add(titulo);
            var gridTitulo = new GridView();
            gridTitulo.DataSource = tituloRow;
            gridTitulo.GridLines = GridLines.None;
            gridTitulo.Font.Bold = true;
            gridTitulo.Font.Size = 14;
            gridTitulo.ShowHeader = false;
            gridTitulo.DataBind();
            gridTitulo.Rows[0].Cells[0].ColumnSpan = headers.Count();
            gridTitulo.RenderControl(htw);

            var gridData = new GridView();
            gridData.DataSource = " ";
            gridData.DataBind();
            gridData.GridLines = GridLines.None;
            gridData.HeaderRow.Cells[0].Text = " ";
            gridData.RenderControl(htw);

            foreach (CuentaCorrienteAgrupada ctecta in list)
            {
                var agrupadorRow = new System.Data.DataTable("Agrupador");
                agrupadorRow.Columns.Add("Agrupador");
                agrupadorRow.Rows.Add(ctecta.agrupador);
                var gridAgrupador = new GridView();
                gridAgrupador.DataSource = agrupadorRow;
                gridAgrupador.GridLines = GridLines.None;
                gridAgrupador.Font.Bold = true;
                gridAgrupador.Font.Size = 14;
                gridAgrupador.ShowHeader = false;
                gridAgrupador.DataBind();
                gridAgrupador.Rows[0].Cells[0].ColumnSpan = headers.Count();
                gridAgrupador.RenderControl(htw);

                gridData.DataSource = ctecta.cuentasCorrientes;
                gridData.DataBind();
                gridData.GridLines = GridLines.Both;
                for (int i = 0; i < headers.Length; i++)
                {
                    gridData.HeaderRow.Cells[i].Text = headers[i];
                }
                gridData.RenderControl(htw);

                var totalRow = new System.Data.DataTable("Total");
                totalRow.Columns.Add("Agrupador");
                totalRow.Rows.Add("Total: " + ctecta.total);
                var gridTotal = new GridView();
                gridTotal.DataSource = totalRow;
                gridTotal.GridLines = GridLines.None;
                gridTotal.Font.Bold = true;
                gridTotal.Font.Size = 12;
                gridTotal.ShowHeader = false;
                gridTotal.DataBind();
                gridTotal.Rows[0].Cells[0].ColumnSpan = headers.Count();
                gridTotal.RenderControl(htw);

                gridData.DataSource = " ";
                gridData.DataBind();
                gridData.GridLines = GridLines.None;
                gridData.HeaderRow.Cells[0].Text = " ";
                gridData.RenderControl(htw);

                gridData.DataSource = " ";
                gridData.DataBind();
                gridData.GridLines = GridLines.None;
                gridData.HeaderRow.Cells[0].Text = " ";
                gridData.RenderControl(htw);
            }

            return sw.ToString();
        }
        [Obsolete("Esta funcionalidad está deprecada, ya que utiliza un html en vez de la funcionalidad de OpenXml.")]
        public static string ToExcelViajesAgrupados(List<ViajeAgrupadoExcel> list, string[] headers, string titulo)
        {

            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            var tituloRow = new System.Data.DataTable("Titulo");
            tituloRow.Columns.Add("Titulo");
            tituloRow.Rows.Add(titulo);
            var gridTitulo = new GridView();
            gridTitulo.DataSource = tituloRow;
            gridTitulo.GridLines = GridLines.None;
            gridTitulo.Font.Bold = true;
            gridTitulo.Font.Size = 14;
            gridTitulo.ShowHeader = false;
            gridTitulo.DataBind();
            gridTitulo.Rows[0].Cells[0].ColumnSpan = headers.Count();
            gridTitulo.RenderControl(htw);

            var gridData = new GridView();
            gridData.DataSource = " ";
            gridData.DataBind();
            gridData.GridLines = GridLines.None;
            gridData.HeaderRow.Cells[0].Text = " ";
            gridData.RenderControl(htw);

            foreach (ViajeAgrupadoExcel viaje in list)
            {
                var proformaRow = new System.Data.DataTable("Proforma");
                proformaRow.Columns.Add("Proforma");
                proformaRow.Rows.Add("Proforma Nro: " + viaje.proforma);
                var gridAgrupador = new GridView();
                gridAgrupador.DataSource = proformaRow;
                gridAgrupador.GridLines = GridLines.None;
                gridAgrupador.Font.Bold = true;
                gridAgrupador.Font.Size = 14;
                gridAgrupador.ShowHeader = false;
                gridAgrupador.DataBind();
                gridAgrupador.Rows[0].Cells[0].ColumnSpan = headers.Count();
                gridAgrupador.RenderControl(htw);

                var regionRow = new System.Data.DataTable("Region");
                regionRow.Columns.Add("Region");
                regionRow.Rows.Add("Region: " + viaje.region);
                var gridRegion = new GridView();
                gridRegion.DataSource = regionRow;
                gridRegion.GridLines = GridLines.None;
                gridRegion.Font.Bold = false;
                gridRegion.Font.Size = 12;
                gridRegion.ShowHeader = false;
                gridRegion.DataBind();
                gridRegion.Rows[0].Cells[0].ColumnSpan = headers.Count();
                gridRegion.RenderControl(htw);

                gridData.DataSource = viaje.viajeItem;
                gridData.DataBind();
                gridData.GridLines = GridLines.Both;
                for (int i = 0; i < headers.Length; i++)
                {
                    gridData.HeaderRow.Cells[i].Text = headers[i];
                }
                gridData.RenderControl(htw);

                var totalImporteRow = new System.Data.DataTable("TotalCantidad");
                totalImporteRow.Columns.Add("TotalCantidad");
                totalImporteRow.Rows.Add("Total Cantidad Kg: " + viaje.totalKg);
                var gridTotalImporte = new GridView();
                gridTotalImporte.DataSource = totalImporteRow;
                gridTotalImporte.GridLines = GridLines.None;
                gridTotalImporte.Font.Bold = true;
                gridTotalImporte.Font.Size = 12;
                gridTotalImporte.ShowHeader = false;
                gridTotalImporte.DataBind();
                gridTotalImporte.Rows[0].Cells[0].ColumnSpan = headers.Count();
                gridTotalImporte.RenderControl(htw);

                var totalCantidadRow = new System.Data.DataTable("TotalImporte");
                totalCantidadRow.Columns.Add("TotalImporte");
                totalCantidadRow.Rows.Add("Total Importe: " + viaje.totalImporte);
                var gridTotalCantidad = new GridView();
                gridTotalCantidad.DataSource = totalCantidadRow;
                gridTotalCantidad.GridLines = GridLines.None;
                gridTotalCantidad.Font.Bold = true;
                gridTotalCantidad.Font.Size = 12;
                gridTotalCantidad.ShowHeader = false;
                gridTotalCantidad.DataBind();
                gridTotalCantidad.Rows[0].Cells[0].ColumnSpan = headers.Count();
                gridTotalCantidad.RenderControl(htw);

                gridData.DataSource = " ";
                gridData.DataBind();
                gridData.GridLines = GridLines.None;
                gridData.HeaderRow.Cells[0].Text = " ";
                gridData.RenderControl(htw);

                gridData.DataSource = " ";
                gridData.DataBind();
                gridData.GridLines = GridLines.None;
                gridData.HeaderRow.Cells[0].Text = " ";
                gridData.RenderControl(htw);
            }

            return sw.ToString();
        }
        //https://www.sirpenski.com/pubs/article/create-excel-file-using-openxml-and-aspnetcore

        public static void CreateExcelFile<T>(List<T> data, string[] headers, string OutPutFileDirectory)
        {
            using (SpreadsheetDocument package = SpreadsheetDocument.Create(OutPutFileDirectory, SpreadsheetDocumentType.Workbook))
            {
                CreatePartsForExcel(package, data, headers);
            }
        }
        public static MemoryStream CreateExcelFileMs<T>(List<T> data, string[] headers)
        {
            MemoryStream ms = new MemoryStream();
            using (SpreadsheetDocument package = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
            {
                CreatePartsForExcel(package, data, headers);
            }
            ms.Seek(0, SeekOrigin.Begin);

            return ms;
        }

        private static void CreatePartsForExcel<T>(SpreadsheetDocument document, List<T> data, string[] headers)
        {
            SheetData partSheetData = GenerateSheetdataForDetails(data, headers);

            Columns columns = AutoSize(partSheetData);
            WorkbookPart workbookPart1 = document.AddWorkbookPart();
            GenerateWorkbookPartContent(workbookPart1);

            WorkbookStylesPart workbookStylesPart1 = workbookPart1.AddNewPart<WorkbookStylesPart>("rId3");
            GenerateWorkbookStylesPartContent(workbookStylesPart1);

            WorksheetPart worksheetPart1 = workbookPart1.AddNewPart<WorksheetPart>("rId1");
            GenerateWorksheetPartContent(worksheetPart1, partSheetData, columns);
        }
        private static void GenerateWorksheetPartContent(WorksheetPart worksheetPart1, SheetData sheetData1, Columns columns)
        {
            Worksheet worksheet = new Worksheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x14ac" } };
            worksheet.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            worksheet.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            worksheet.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");
            SheetDimension sheetDimension1 = new SheetDimension() { Reference = "A1" };

            SheetViews sheetViews1 = new SheetViews();

            SheetView sheetView1 = new SheetView() { TabSelected = true, WorkbookViewId = (UInt32Value)0U };
            Selection selection1 = new Selection() { ActiveCell = "A1", SequenceOfReferences = new ListValue<StringValue>() { InnerText = "A1" } };

            sheetView1.Append(selection1);

            sheetViews1.Append(sheetView1);
            SheetFormatProperties sheetFormatProperties1 = new SheetFormatProperties() { DefaultRowHeight = 15D, DyDescent = 0.25D };

            PageMargins pageMargins1 = new PageMargins() { Left = 0.7D, Right = 0.7D, Top = 0.75D, Bottom = 0.75D, Header = 0.3D, Footer = 0.3D };
            worksheet.Append(sheetDimension1);
            worksheet.Append(sheetViews1);
            worksheet.Append(sheetFormatProperties1);
            worksheet.Append(columns);
            worksheet.Append(sheetData1);
            worksheet.Append(pageMargins1);
            worksheetPart1.Worksheet = worksheet;
        }
        private static void GenerateWorkbookStylesPartContent(WorkbookStylesPart workbookStylesPart1)
        {
            Stylesheet stylesheet1 = new Stylesheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x14ac" } };
            stylesheet1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            stylesheet1.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");

            Fonts fonts1 = new Fonts() { Count = (UInt32Value)2U, KnownFonts = true };

            Font font1 = new Font();
            FontSize fontSize1 = new FontSize() { Val = 11D };
            Color color1 = new Color() { Theme = (UInt32Value)1U };
            FontName fontName1 = new FontName() { Val = "Calibri" };
            FontFamilyNumbering fontFamilyNumbering1 = new FontFamilyNumbering() { Val = 2 };
            FontScheme fontScheme1 = new FontScheme() { Val = FontSchemeValues.Minor };

            font1.Append(fontSize1);
            font1.Append(color1);
            font1.Append(fontName1);
            font1.Append(fontFamilyNumbering1);
            font1.Append(fontScheme1);

            Font font2 = new Font();
            Bold bold1 = new Bold();
            FontSize fontSize2 = new FontSize() { Val = 11D };
            Color color2 = new Color() { Theme = (UInt32Value)1U };
            FontName fontName2 = new FontName() { Val = "Calibri" };
            FontFamilyNumbering fontFamilyNumbering2 = new FontFamilyNumbering() { Val = 2 };
            FontScheme fontScheme2 = new FontScheme() { Val = FontSchemeValues.Minor };

            font2.Append(bold1);
            font2.Append(fontSize2);
            font2.Append(color2);
            font2.Append(fontName2);
            font2.Append(fontFamilyNumbering2);
            font2.Append(fontScheme2);

            fonts1.Append(font1);
            fonts1.Append(font2);

            Fills fills1 = new Fills() { Count = (UInt32Value)2U };

            Fill fill1 = new Fill();
            PatternFill patternFill1 = new PatternFill() { PatternType = PatternValues.None };

            fill1.Append(patternFill1);

            Fill fill2 = new Fill();
            PatternFill patternFill2 = new PatternFill() { PatternType = PatternValues.Gray125 };

            fill2.Append(patternFill2);

            fills1.Append(fill1);
            fills1.Append(fill2);

            Borders borders1 = new Borders() { Count = (UInt32Value)2U };

            Border border1 = new Border();
            LeftBorder leftBorder1 = new LeftBorder();
            RightBorder rightBorder1 = new RightBorder();
            TopBorder topBorder1 = new TopBorder();
            BottomBorder bottomBorder1 = new BottomBorder();
            DiagonalBorder diagonalBorder1 = new DiagonalBorder();

            border1.Append(leftBorder1);
            border1.Append(rightBorder1);
            border1.Append(topBorder1);
            border1.Append(bottomBorder1);
            border1.Append(diagonalBorder1);

            Border border2 = new Border();

            LeftBorder leftBorder2 = new LeftBorder() { Style = BorderStyleValues.Thin };
            Color color3 = new Color() { Indexed = (UInt32Value)64U };

            leftBorder2.Append(color3);

            RightBorder rightBorder2 = new RightBorder() { Style = BorderStyleValues.Thin };
            Color color4 = new Color() { Indexed = (UInt32Value)64U };

            rightBorder2.Append(color4);

            TopBorder topBorder2 = new TopBorder() { Style = BorderStyleValues.Thin };
            Color color5 = new Color() { Indexed = (UInt32Value)64U };

            topBorder2.Append(color5);

            BottomBorder bottomBorder2 = new BottomBorder() { Style = BorderStyleValues.Thin };
            Color color6 = new Color() { Indexed = (UInt32Value)64U };

            bottomBorder2.Append(color6);
            DiagonalBorder diagonalBorder2 = new DiagonalBorder();

            border2.Append(leftBorder2);
            border2.Append(rightBorder2);
            border2.Append(topBorder2);
            border2.Append(bottomBorder2);
            border2.Append(diagonalBorder2);

            borders1.Append(border1);
            borders1.Append(border2);

            CellStyleFormats cellStyleFormats1 = new CellStyleFormats() { Count = (UInt32Value)1U };
            CellFormat cellFormat1 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U };

            cellStyleFormats1.Append(cellFormat1);

            CellFormats cellFormats1 = new CellFormats() { Count = (UInt32Value)3U };
            CellFormat cellFormat2 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U };
            CellFormat cellFormat3 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)1U, FormatId = (UInt32Value)0U, ApplyBorder = true };
            CellFormat cellFormat4 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)1U, FormatId = (UInt32Value)0U, ApplyFont = true, ApplyBorder = true };

            cellFormats1.Append(cellFormat2);
            cellFormats1.Append(cellFormat3);
            cellFormats1.Append(cellFormat4);

            CellStyles cellStyles1 = new CellStyles() { Count = (UInt32Value)1U };
            CellStyle cellStyle1 = new CellStyle() { Name = "Normal", FormatId = (UInt32Value)0U, BuiltinId = (UInt32Value)0U };

            cellStyles1.Append(cellStyle1);
            DifferentialFormats differentialFormats1 = new DifferentialFormats() { Count = (UInt32Value)0U };
            TableStyles tableStyles1 = new TableStyles() { Count = (UInt32Value)0U, DefaultTableStyle = "TableStyleMedium2", DefaultPivotStyle = "PivotStyleLight16" };

            StylesheetExtensionList stylesheetExtensionList1 = new StylesheetExtensionList();

            StylesheetExtension stylesheetExtension1 = new StylesheetExtension() { Uri = "{EB79DEF2-80B8-43e5-95BD-54CBDDF9020C}" };
            stylesheetExtension1.AddNamespaceDeclaration("x14", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/main");
            X14.SlicerStyles slicerStyles1 = new X14.SlicerStyles() { DefaultSlicerStyle = "SlicerStyleLight1" };

            stylesheetExtension1.Append(slicerStyles1);

            StylesheetExtension stylesheetExtension2 = new StylesheetExtension() { Uri = "{9260A510-F301-46a8-8635-F512D64BE5F5}" };
            stylesheetExtension2.AddNamespaceDeclaration("x15", "http://schemas.microsoft.com/office/spreadsheetml/2010/11/main");
            X15.TimelineStyles timelineStyles1 = new X15.TimelineStyles() { DefaultTimelineStyle = "TimeSlicerStyleLight1" };

            stylesheetExtension2.Append(timelineStyles1);

            stylesheetExtensionList1.Append(stylesheetExtension1);
            stylesheetExtensionList1.Append(stylesheetExtension2);

            stylesheet1.Append(fonts1);
            stylesheet1.Append(fills1);
            stylesheet1.Append(borders1);
            stylesheet1.Append(cellStyleFormats1);
            stylesheet1.Append(cellFormats1);
            stylesheet1.Append(cellStyles1);
            stylesheet1.Append(differentialFormats1);
            stylesheet1.Append(tableStyles1);
            stylesheet1.Append(stylesheetExtensionList1);

            workbookStylesPart1.Stylesheet = stylesheet1;
        }
        private static void GenerateWorkbookPartContent(WorkbookPart workbookPart1)
        {
            Workbook workbook1 = new Workbook();
            Sheets sheets1 = new Sheets();
            Sheet sheet1 = new Sheet() { Name = "Sheet1", SheetId = (UInt32Value)1U, Id = "rId1" };
            sheets1.Append(sheet1);
            workbook1.Append(sheets1);
            workbookPart1.Workbook = workbook1;
        }
        private static SheetData GenerateSheetdataForDetails<T>(List<T> data, string[] headers)
        {
            SheetData sheetData1 = new SheetData();
            sheetData1.Append(CreateHeaderRowForExcel(headers));

            foreach (var item in data)
            {
                Row partsRows = GenerateRowForChildPartDetail(item);
                sheetData1.Append(partsRows);
            }
            return sheetData1;
        }
        private static Row CreateHeaderRowForExcel(string[] headers)
        {
            Row workRow = new Row();
            foreach (var item in headers)
            {
                workRow.Append(CreateCell(item, 2U));
            }
            return workRow;
        }
        private static Row GenerateRowForChildPartDetail<T>(T model)
        {
            var oPropRow = model.GetType().GetProperties();

            Row tRow = new Row();
            for (int i = 1; i <= oPropRow.Count(); i++)
            {
                var value = GetPropertyValue<string>(model, oPropRow[i - 1].Name);
                tRow.Append(CreateCell(value));
            };
            return tRow;
        }
        public static T GetPropertyValue<T>(object o, string propertyName)
        {
            return (T)o.GetType().GetProperty(propertyName).GetValue(o, null);
        }
        private static Cell CreateCell(string text)
        {
            Cell cell = new Cell();
            cell.StyleIndex = 1U;
            cell.DataType = ResolveCellDataTypeOnValue(text);
            cell.CellValue = new CellValue(text);
            return cell;
        }
        private static Cell CreateCell(string text, uint styleIndex)
        {
            Cell cell = new Cell();
            cell.StyleIndex = styleIndex;
            cell.DataType = ResolveCellDataTypeOnValue(text);
            cell.CellValue = new CellValue(text);
            return cell;
        }
        private static EnumValue<CellValues> ResolveCellDataTypeOnValue(string text)
        {
            int intVal;
            double doubleVal;
            if (int.TryParse(text, out intVal) || double.TryParse(text, out doubleVal))
            {
                return CellValues.Number;
            }
            else
            {
                return CellValues.String;
            }
        }

        private static Columns AutoSize(SheetData sheetData)
        {
            var maxColWidth = GetMaxCharacterWidth(sheetData);

            Columns columns = new Columns();
            //this is the width of my font - yours may be different
            double maxWidth = 7;
            foreach (var item in maxColWidth)
            {
                //width = Truncate([{Number of Characters} * {Maximum Digit Width} + {5 pixel padding}]/{Maximum Digit Width}*256)/256
                double width = Math.Truncate((item.Value * maxWidth + 5) / maxWidth * 256) / 256;

                //pixels=Truncate(((256 * {width} + Truncate(128/{Maximum Digit Width}))/256)*{Maximum Digit Width})
                double pixels = Math.Truncate(((256 * width + Math.Truncate(128 / maxWidth)) / 256) * maxWidth);

                //character width=Truncate(({pixels}-5)/{Maximum Digit Width} * 100+0.5)/100
                double charWidth = Math.Truncate((pixels - 5) / maxWidth * 100 + 0.5) / 100;

                Column col = new Column() { BestFit = true, Min = (UInt32)(item.Key + 1), Max = (UInt32)(item.Key + 1), CustomWidth = true, Width = (DoubleValue)width };
                columns.Append(col);
            }

            return columns;
        }

        private static Dictionary<int, int> GetMaxCharacterWidth(SheetData sheetData)
        {
            //iterate over all cells getting a max char value for each column
            Dictionary<int, int> maxColWidth = new Dictionary<int, int>();
            var rows = sheetData.Elements<Row>();
            UInt32[] numberStyles = new UInt32[] { 5, 6, 7, 8 }; //styles that will add extra chars
            UInt32[] boldStyles = new UInt32[] { 1, 2, 3, 4, 6, 7, 8 }; //styles that will bold
            foreach (var r in rows)
            {
                var cells = r.Elements<Cell>().ToArray();

                //using cell index as my column
                for (int i = 0; i < cells.Length; i++)
                {
                    var cell = cells[i];
                    var cellValue = cell.CellValue == null ? string.Empty : cell.CellValue.InnerText;
                    var cellTextLength = cellValue.Length;

                    if (cell.StyleIndex != null && numberStyles.Contains(cell.StyleIndex))
                    {
                        int thousandCount = (int)Math.Truncate((double)cellTextLength / 4);

                        //add 3 for '.00' 
                        cellTextLength += (3 + thousandCount);
                    }

                    if (cell.StyleIndex != null && boldStyles.Contains(cell.StyleIndex))
                    {
                        //add an extra char for bold - not 100% acurate but good enough for what i need.
                        cellTextLength += 1;
                    }

                    if (maxColWidth.ContainsKey(i))
                    {
                        var current = maxColWidth[i];
                        if (cellTextLength > current)
                        {
                            maxColWidth[i] = cellTextLength;
                        }
                    }
                    else
                    {
                        maxColWidth.Add(i, cellTextLength);
                    }
                }
            }

            return maxColWidth;
        }
        public static byte[] ComprasHistorialMovimientosToExcel(HistorialDeFechaDto historial)
        {
            var memStream = new MemoryStream();
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(memStream, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                SheetData sheetData = new SheetData();
                worksheetPart.Worksheet = new Worksheet(sheetData);

                Sheets sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());
                Sheet sheet = new Sheet() { Id = document.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Sheet1" };
                sheets.Append(sheet);

                foreach (var solp in historial.ListaSolp)
                {
                    AddRow(sheetData, "Nro de SOLP:", solp.NroSolp);
                    AddRow(sheetData, "Fecha de Creación:", solp.FechaCreacionFormateada);
                    AddRow(sheetData, "Fecha de Liberación:", solp.FechaLiberacionSapFormateada);
                    AddRow(sheetData, "Fecha de creación de PO:", historial.FechaCreacionPOFormateada);
                }

                AddEmptyRow(sheetData);

                foreach (var row in historial.Cuerpo)
                {
                    AddRow(sheetData, row.ToArray());
                }

                workbookPart.Workbook.Save();
            }
            return memStream.ToArray();
        }

        public static void AddRow(SheetData sheetData, params string[] values)
        {
            Row row = new Row();
            foreach (var value in values)
            {
                Cell cell = new Cell() { CellValue = new CellValue(value), DataType = CellValues.String };
                row.Append(cell);
            }
            sheetData.Append(row);
        }

        public static void AddEmptyRow(SheetData sheetData)
        {
            Row row = new Row();
            Cell cell = new Cell() { CellValue = new CellValue(""), DataType = CellValues.String };
            row.Append(cell);
            sheetData.Append(row);
        }

        /// <summary>
        /// Desde una lista de objetos.
        /// Se recomienda usar los atributos ExcelIgnoreAttribute, ExcelColumnNameAttribute y ExcelColumnOrderAttribute para controlar las columnas.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static MemoryStream ExportDtoToSingleStandardExcelSheet<T>(IEnumerable<T> data)
            where T : class
        {
            // probado con POPosicionDto

            /* en caso de requerir algún formato especial, m´sa hojas, o alguna otra cosa, 
             * crear un método nuevo
             * (tal vez sea buena idea evitar usar reflexión)
             */

            MemoryStream stream = new MemoryStream();
            using (BigExcelWriter excelWriter = new BigExcelWriter(stream, SpreadsheetDocumentType.Workbook))
            {
                excelWriter.CreateAndOpenSheet($"PoMUltiple-{DateTime.Today:dd-MM-yyyy}");

                IOrderedEnumerable<PropertyInfo> sortedColumns = typeof(T)
                    .GetProperties()
                    .Where(x => x.GetCustomAttribute<ExcelIgnoreAttribute>() == null)
                    .OrderBy(x => x.GetCustomAttribute<ExcelColumnOrderAttribute>()?.Order ?? int.MaxValue);

                // Encabezado (se usan propiedades / decoradores del dto para definir texto real y orden)
                // también se puede hacer a mano
                IEnumerable<string> columnNames = sortedColumns
                    .Select(x => x.GetCustomAttribute<ExcelColumnNameAttribute>()?.Name ?? x.Name);
                excelWriter.WriteTextRow(columnNames);

                foreach (T filaDto in data)
                {
                    excelWriter.BeginRow();
                    foreach (PropertyInfo columnName in sortedColumns)
                    {
                        ExcelCellType cellType =
                            columnName.GetCustomAttribute<ExcelColumnTypeAttribute>()?.Type ?? ExcelCellType.Text;
                        object cellData = columnName.GetValue(filaDto);

                        switch (cellType)
                        {
                            case ExcelCellType.Number:
                                if (cellData != null)
                                {
                                    float cellDataNumber = Convert.ToSingle(cellData);
                                    excelWriter.WriteNumberCell(cellDataNumber);
                                }
                                else
                                {
                                    excelWriter.WriteTextCell(string.Empty);
                                }
                                break;
                            case ExcelCellType.Formula:
                                string cellDataFormula = cellData?.ToString();
                                if (!string.IsNullOrWhiteSpace(cellDataFormula))
                                {
                                    excelWriter.WriteFormulaCell(cellDataFormula);
                                }
                                else
                                {
                                    excelWriter.WriteTextCell(string.Empty);
                                }
                                break;
                            case ExcelCellType.Text:
                                string cellDataString = cellData?.ToString() ?? "";
                                excelWriter.WriteTextCell(cellDataString);
                                break;
                        }
                    }
                    excelWriter.EndRow();
                }
            }
            return stream;
        }
    }

}
