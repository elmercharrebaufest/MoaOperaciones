using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SustitucionMOAModel.Models.WSMapMOA.CartaPorte.Detalle;
using SustitucionMOAModel.Models.WSMapMOA.Contrato.Detalle;
using SustitucionMOAModel.Models.WSMapMOA.CuentaCorriente;
using SustitucionMOAModel.Models.WSMapMOA.Flete;
using SustitucionMOAModel.Models.WSMapMOA.Pago.Detalle;

namespace SustitucionMOAUtils.Export
{
    public static class ExcelExport
    {
        public static string ToExcel(object dataList, string[] headers, string titulo) {

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

            List<EntregaDescarga> entregasDescargas = (List<EntregaDescarga>) entregasDescargasList;

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

            if (datosCalidad.Count != 0) {
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

            if (aplicaciones.Count != 0) {
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

        public static string ToExcelContratoDetalle(object ampliacionesAnulacionesList, object aplicacionesList, object calidadList, object caracteristicasList, object condicionesPagoList, object fijacionesList, object hijosList, object liquidacionesList, object pagosList, object resumenList, string[] ampliacionesAnulacionesHeaders, string[] aplicacionesHeaders, string[] calidadHeaders, string[] caracteristicasHeaders, string[] condicionesPagoHeaders, string[] fijacionesHeaders, string[] hijosHeaders, string[] liquidacionesHeaders, string[] pagosHeaders, string[] resumenHeaders, string titulo)
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

            List<CalidadExcelDetalle> calidades = (List<CalidadExcelDetalle>)calidadList;

            if (calidades.Count() == 0)
            {
                calidades.Add(new CalidadExcelDetalle());

                gridData.DataSource = calidades;
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

            foreach (CuentaCorrienteAgrupada ctecta in list) {
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

        public static void CreateExcelDoc<T>(List<T> data, string[] headers, string fileName)
        {
            SpreadsheetDocument document = SpreadsheetDocument.Create(fileName, SpreadsheetDocumentType.Workbook);

            WorkbookPart workbookPart = document.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

            Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "DATOS" };


            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();

            //Row header = new Row();
            //header.RowIndex = (UInt32)1;
            uint index = 0;
            foreach (string head in headers)
            {
                string cellName = _cellReferences[index];
                Cell headerCell = InsertCellInWorksheet(cellName, 1, sheetData);
                CellValue value = new CellValue(head);
                headerCell.CellValue = value;
                index += 1;
            }
            worksheet.Save();
            document.Save();
            //return document.ToString();
        }

        private static Cell InsertCellInWorksheet(string columnName, uint rowIndex, SheetData sheetData)
        {
            string cellReference = columnName + rowIndex;

            // If the worksheet does not contain a row with the specified row index, insert one.
            Row row;
            if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
            {
                row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
            }
            else
            {
                row = new Row() { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            // If there is not a cell with the specified column name, insert one.  
            if (row.Elements<Cell>().Where(c => c.CellReference.Value == columnName + rowIndex).Count() > 0)
            {
                return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
            }
            else
            {
                // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
                Cell refCell = null;
                foreach (Cell cell in row.Elements<Cell>())
                {
                    if (cell.CellReference.Value.Length == cellReference.Length)
                    {
                        if (string.Compare(cell.CellReference.Value, cellReference, true) > 0)
                        {
                            refCell = cell;
                            break;
                        }
                    }
                }

                Cell newCell = new Cell() { CellReference = cellReference };
                row.InsertBefore(newCell, refCell);

                return newCell;
            }
        }

        private static string[] _cellReferences = {
            "A","B","C","D","E","F","G","H","I","J","K"
        };
    }
}
