using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using SustitucionMOA.Utils;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOAModel.Models.WSMapMOA;
using SustitucionMOAModel.Models.WSMapMOA.PDF;
using SustitucionMOARepositorio;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using HttpPostAttribute = System.Web.Mvc.HttpPostAttribute;
using Kendo.DynamicLinq;
using System.Web.Script.Serialization;
using SustitucionMOAUtils.Export;
using System.Data;
using SustitucionMOAModel.Enums;

namespace SustitucionMOA.Controllers
{
    public class CrearContratoController : BaseController
    {
        protected readonly IRepositorio repositorio;
        readonly IDataAgroApiService dataAgroApiService;

        public CrearContratoController(IDataAgroApiService dataAgroApiService, IRepositorio repositorio)
        {
            this.dataAgroApiService = dataAgroApiService;
            this.repositorio = repositorio;
        }

        public ActionResult GetLocalidadCombo(string localidad)
        {
            localidad = localidad.IsNullOrWhiteSpace() ? "" : localidad;

            if (localidad.Length > 2)
            {
                var listadoLocalidad = repositorio.Listar<Localidad, LocalidadCombo>(x => new LocalidadCombo()
                {
                    LocalidadId = x.LocalidadId,
                    ProvinciaId = x.ProvinciaId,
                    Nombre = x.Nombre + " (" + x.Provincia.Nombre + ")",
                },
                 x => x.Nombre.Contains(localidad)
                 , 500).OrderBy(x => x.Nombre).ToList();

                return JsonCustom(listadoLocalidad);
            }

            return JsonCustom("");
        }
        public ActionResult ObteneDatosContrato(int tiponegocio)
        {
            string BolsaAutomatica = dataAgroApiService.ConfiguracionBolsaAutomatica();
            string DatosContrato = dataAgroApiService.ObteneDatosContrato(tiponegocio);
            return JsonCustom(new { DatosContrato, BolsaAutomatica });
        }
        public ActionResult ObtenerDatosCompraNet(int? idProveedorDataAgro)
        {
            if (idProveedorDataAgro.HasValue)
            {
                return JsonCustom(dataAgroApiService.ObtenerDatosCompraNet(idProveedorDataAgro.Value));
            }
            var proveedor = ObtenerProveedor();
            return JsonCustom(dataAgroApiService.ObtenerDatosCompraNet(proveedor.IdDataAgro.Value));
        }
        public ActionResult CrearContratoAPrecio(string contrato)
        {
            contrato = contrato.Replace("nia", "ña");
            var contratoAPrecio = JsonConvert.DeserializeObject<ContratoAPrecio>(contrato);


            var proveedor = ObtenerProveedor();

            if (contratoAPrecio.CorredorId == null)
            {
                contratoAPrecio.ProveedorId = (int)proveedor.IdDataAgro;
            }
            if (contratoAPrecio.ComercialId == 0)
            {
                contratoAPrecio.ComercialId = (int)proveedor.IdComercialDataAgro;
            }
            contratoAPrecio.ProveedorCreadorId = (int)proveedor.IdDataAgro;
            contratoAPrecio.ComercialCreadorId = null;
            contratoAPrecio.MonedaSustentableId = "USDM ";
            contratoAPrecio.ContratoSAP = "";
            contratoAPrecio.CantidadCamiones = contratoAPrecio.CantidadCamiones == 0 ? null : contratoAPrecio.CantidadCamiones;
            contratoAPrecio.UsuarioTercero = ClaimsPrincipalExtension.GetClaimValue("emails");

            string result = dataAgroApiService.CrearContratoAPrecio(contratoAPrecio);

            return JsonCustom(result);
        }
        public ActionResult CrearContratoAFijar(string contrato)
        {
            contrato = contrato.Replace("nia", "ña");
            var contratoAFijar = JsonConvert.DeserializeObject<ContratoAFijar>(contrato);

            var proveedor = ObtenerProveedor();

            if (contratoAFijar.CorredorId == null)
            {
                contratoAFijar.ProveedorId = (int)proveedor.IdDataAgro;
            }
            if (contratoAFijar.ComercialId == 0)
            {
                contratoAFijar.ComercialId = (int)proveedor.IdComercialDataAgro;
            }
            contratoAFijar.ProveedorCreadorId = (int)proveedor.IdDataAgro;
            contratoAFijar.ComercialCreadorId = null;
            contratoAFijar.MonedaSustentableId = "USDM ";
            contratoAFijar.ContratoSAP = "";
            contratoAFijar.CantidadCamiones = contratoAFijar.CantidadCamiones == 0 ? null : contratoAFijar.CantidadCamiones;
            contratoAFijar.UsuarioTercero = ClaimsPrincipalExtension.GetClaimValue("emails");


            string result = dataAgroApiService.CrearContratoAFijar(contratoAFijar);

            return JsonCustom(result);
        }
        public ActionResult ValidarDirecto()
        {
            var proveedor = ObtenerProveedor();

            var directo = dataAgroApiService.ValidarDirecto(proveedor.CUIT);
            int result = 0;
            if (directo == "false")
            {
                result = proveedor.IdDataAgro ?? 0;
            }

            return JsonCustom(result);
        }
        public ActionResult BuscarProveedoresConCorredor(string filtro)
        {
            filtro = filtro.IsNullOrWhiteSpace() ? "" : filtro;

            if (filtro.Length > 2)
            {
                var proveedor = ObtenerProveedor();

                return JsonCustom(dataAgroApiService.BuscarProveedoresConCorredor(filtro, proveedor.CUIT));
            }
            else
            {
                return JsonCustom("");

            }
        }
        public ActionResult Habilitaciones(int material, int tiponegocio)
        {
            string HabilitarPizarra = dataAgroApiService.HabilitarPizarra(material, tiponegocio);
            string HabilitarCampana = dataAgroApiService.HabilitarCampaña(material);
            string TraerPrecioMoa = dataAgroApiService.TraerPrecioMoa(material, tiponegocio);
            string TraerPagosDiferido = dataAgroApiService.TraerPagosDiferido(material, tiponegocio);
            string TraerHabilitarSustentable = dataAgroApiService.TraerHabilitarSustentable();
            var result = new { HabilitarPizarra, HabilitarCampana, TraerPrecioMoa, TraerPagosDiferido, TraerHabilitarSustentable };
            return JsonCustom(result);
        }
        public ActionResult HabilitarCampana(int material)
        {
            return JsonCustom(dataAgroApiService.HabilitarCampaña(material));
        }
        public ActionResult HabilitarPizarra(int material, int tiponegocio)
        {
            return JsonCustom(dataAgroApiService.HabilitarPizarra(material, tiponegocio));
        }
        public ActionResult ObtenerFijacionesAutomaticas(bool esCorredorEnDataAgro, string cuitProveedor, int materialId, string filtro)
        {
            var proveedor = ObtenerProveedor();

            if (esCorredorEnDataAgro)
            {
                return JsonCustom(dataAgroApiService.ObtenerFijacionesAutomaticas(cuitProveedor, proveedor.CUIT, materialId, filtro, 0));
            }
            else
            {
                return JsonCustom(dataAgroApiService.ObtenerFijacionesAutomaticas(proveedor.CUIT, "", materialId, filtro, 0));

            }
        }
        public ActionResult CrearContratoFijacion(string contrato)
        {
            contrato = contrato.Replace("nia", "ña");
            var contratoFijacion = JsonConvert.DeserializeObject<ContratoFijacion>(contrato);


            var proveedor = ObtenerProveedor();


            if (contratoFijacion.CorredorId == null)
            {
                contratoFijacion.ProveedorId = (int)proveedor.IdDataAgro;
            }
            if (contratoFijacion.ComercialId == 0)
            {
                contratoFijacion.ComercialId = (int)proveedor.IdComercialDataAgro;
            }
            contratoFijacion.ProveedorCreadorId = (int)proveedor.IdDataAgro;
            contratoFijacion.ComercialCreadorId = null;
            contratoFijacion.MonedaSustentable = "USDM ";
            contratoFijacion.CantidadCamiones = contratoFijacion.CantidadCamiones == 0 ? null : contratoFijacion.CantidadCamiones;
            contratoFijacion.UsuarioTercero = ClaimsPrincipalExtension.GetClaimValue("emails");


            string result = dataAgroApiService.CrearContratoFijacion(contratoFijacion);

            return JsonCustom(result);
        }
        public ActionResult GetContratos(string fechaDesde, string fechaHasta, string entregaDesde, string entregaHasta, string fijacionHasta, int? corredorId,
            int? proveedorId, int? boletoId, int? clasificacionId, int? destinoId, string estadoId, int? materialId, int? campaniaId, int? tipoNegocioId,
            bool? pagoDiferidoTercero, bool? calidadTercero, bool? dolarizadoTercero, bool? sustentableTercero, string contratoCorredor)
        {
            var proveedor = ObtenerProveedor();

            string result = obteberContratos(fechaDesde, fechaHasta, entregaDesde, entregaHasta, fijacionHasta, corredorId, proveedorId, boletoId, clasificacionId, destinoId, estadoId, materialId, campaniaId, tipoNegocioId, pagoDiferidoTercero, calidadTercero, dolarizadoTercero, proveedor, sustentableTercero, contratoCorredor);

            return JsonCustom(result);
        }
        public ActionResult ExportContratos(string fechaDesde, string fechaHasta, string entregaDesde, string entregaHasta, string fijacionHasta, int? corredorId,
           int? proveedorId, int? boletoId, int? clasificacionId, int? destinoId, string estadoId, int? materialId, int? campaniaId, int? tipoNegocioId,
           bool? pagoDiferidoTercero, bool? calidadTercero, bool? dolarizadoTercero, bool? sustentableTercero, string contratoCorredor)
        {
            var proveedor = ObtenerProveedor();

            string result = obteberContratos(fechaDesde, fechaHasta, entregaDesde, entregaHasta, fijacionHasta, corredorId, proveedorId, boletoId, clasificacionId, destinoId, estadoId, materialId, campaniaId, tipoNegocioId, pagoDiferidoTercero, calidadTercero, dolarizadoTercero, proveedor, sustentableTercero, contratoCorredor);
            System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
            var result2 = (Dictionary<string, object>)ser.DeserializeObject(result);
            var list = ser.Deserialize<List<BasicoContrato>>(ser.Serialize(result2["Data"]));
            foreach (var item in list)
            {
                if (item.FechaDesde.HasValue)
                    item.FechaDesde = item.FechaDesde.Value.AddHours(-3);
                if (item.FechaHasta.HasValue)
                    item.FechaHasta = item.FechaHasta.Value.AddHours(-3);
            }
            var excel = ExcelExport.ToExcel(list, new string[] { "Cuit", "Proveedor", "Corredor", "ContratoCorredor", "TipoNegocio", "Cantidad", "Precio", "Moneda",
                    "Destino", "FechaDesde", "FechaHasta", "Material", "Campaña", "Clasificacion", "Localidad", "Consignatario", "Estado", "Pago Diferido", "Dolarizado", "Calidad", "Sustentable" }, "Reporte Contratos");

            return JsonCustom(excel);
        }

        private string obteberContratos(string fechaDesde, string fechaHasta, string entregaDesde, string entregaHasta, string fijacionHasta, int? corredorId, int? proveedorId, int? boletoId, int? clasificacionId, int? destinoId, string estadoId, int? materialId, int? campaniaId, int? tipoNegocioId, bool? pagoDiferidoTercero, bool? calidadTercero, bool? dolarizadoTercero, Proveedor proveedor, bool? sustentableTercero, string contratoCorredor)
        {
            DataSourceRequest request = new DataSourceRequest();
            request.Filter = new Kendo.DynamicLinq.Filter();
            request.Filter.Logic = "and";
            var filtros = new List<Kendo.DynamicLinq.Filter>();
            if (!string.IsNullOrWhiteSpace(fechaDesde))
            {
                var fecha = DateTime.ParseExact(fechaDesde, "dd/MM/yyyy", null).ToUniversalTime();
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "Fecha", Value = fecha, Operator = "gte" });
            }
            if (!string.IsNullOrWhiteSpace(fechaHasta))
            {
                var fecha = DateTime.ParseExact(fechaHasta, "dd/MM/yyyy", null).ToUniversalTime();
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "Fecha", Value = fecha, Operator = "lte" });
            }
            if (!string.IsNullOrWhiteSpace(entregaDesde))
            {
                var fecha = DateTime.ParseExact(entregaDesde, "dd/MM/yyyy", null).ToUniversalTime();
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "FechaDesde", Value = fecha, Operator = "gte" });
            }
            if (!string.IsNullOrWhiteSpace(entregaHasta))
            {
                var fecha = DateTime.ParseExact(entregaHasta, "dd/MM/yyyy", null).ToUniversalTime();
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "FechaHasta", Value = fecha, Operator = "lte" });
            }
            if (!string.IsNullOrWhiteSpace(fijacionHasta))
            {
                var fecha = DateTime.ParseExact(fijacionHasta, "dd/MM/yyyy", null).ToUniversalTime();
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "HastaFijacion", Value = fecha, Operator = "lte" });
            }
            if (corredorId.HasValue && corredorId > 0)
            {
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "CorredorId", Value = corredorId, Operator = "eq" });
            }
            if (proveedorId.HasValue && proveedorId > 0)
            {
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "ProveedorId", Value = proveedorId, Operator = "eq" });
            }
            if (boletoId.HasValue && boletoId > 0)
            {
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "BoletoId", Value = boletoId, Operator = "eq" });
            }
            if (clasificacionId.HasValue && clasificacionId > 0)
            {
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "ClasificacionId", Value = clasificacionId, Operator = "eq" });
            }
            if (destinoId.HasValue && destinoId > 0)
            {
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "DestinoId", Value = destinoId, Operator = "eq" });
            }
            if (!string.IsNullOrWhiteSpace(estadoId))
            {
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "Estado_Contrato", Value = estadoId, Operator = "eq" });
            }
            if (materialId.HasValue && materialId > 0)
            {
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "MaterialId", Value = materialId, Operator = "eq" });
            }
            if (campaniaId.HasValue && campaniaId > 0)
            {
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "CampanaId", Value = campaniaId, Operator = "eq" });
            }
            if (tipoNegocioId.HasValue && tipoNegocioId > 0)
            {
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "TipoNegocio", Value = tipoNegocioId, Operator = "eq" });
            }
            if (pagoDiferidoTercero.HasValue)
            {
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "PagoDiferidoTercero", Value = pagoDiferidoTercero, Operator = "eq" });
            }
            if (dolarizadoTercero.HasValue)
            {
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "DolarizadoTercero", Value = dolarizadoTercero, Operator = "eq" });
            }
            if (sustentableTercero.HasValue)
            {
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "SustentableTercero", Value = sustentableTercero, Operator = "eq" });
            }
            if (calidadTercero.HasValue)
            {
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "CalidadTercero", Value = calidadTercero, Operator = "eq" });
            }
            if (corredorId == null || corredorId == 0)
            {
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "ProveedorId", Value = (int)proveedor.IdDataAgro, Operator = "eq" });
            }
            if (!string.IsNullOrWhiteSpace(contratoCorredor))
            {
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "ContratoCorredor", Value = contratoCorredor, Operator = "eq" });
            }
            filtros.Add(new Kendo.DynamicLinq.Filter { Field = "ComercialCreadorId", Value = (int)proveedor.IdDataAgro, Operator = "eq" });//es el ProveedorCreadorId en el BasicoContrato
            request.Filter.Filters = filtros;
            string result = dataAgroApiService.GetContratos(request);
            return result;
        }

        public ActionResult ValidarProveedor(string proveedorId)
        {
            if (!string.IsNullOrEmpty(proveedorId) && proveedorId != "0")
            {
                return JsonCustom(dataAgroApiService.ValidarProveedor(proveedorId));
            }
            Proveedor proveedor = ObtenerProveedor();

            return JsonCustom(dataAgroApiService.ValidarProveedor(proveedor.IdDataAgro.Value.ToString()));
        }

        public ActionResult TraerPrecioMoaMateriales(int tipoNegocioId = 0)
        {
            return JsonCustom(dataAgroApiService.TraerPrecioMoaMateriales(tipoNegocioId));
        }

        public ActionResult AnularNegocio(int negocioId, int tipoNegocioId, string motivo)
        {
            return JsonCustom(dataAgroApiService.AnularNegocio(negocioId, tipoNegocioId, motivo));
        }

        public ActionResult TraerContratoCompleto(int negocioId, int tipoNegocioId)
        {
            return JsonCustom(dataAgroApiService.TraerContratoCompleto(negocioId, tipoNegocioId));
        }


        [HttpPost]
        public ActionResult AltaMasivaAcuerdo()
        {
            List<string> errores = new List<string>();

            //parsear excel
            //validar tipo de datos
            //enviar lista
            var proveedor = ObtenerProveedor();


            var contratoAcuerdo = Request.Form.Get("contratoAcuerdo");

            int ncontratoAcuerdo;
            if (!int.TryParse(contratoAcuerdo, out ncontratoAcuerdo))
            {
                errores.Add(string.Concat("Debe seleccionar el contrato acuerdo."));
                return JsonCustom(new { info = errores });
            }

            BasicoContrato acuerdo = dataAgroApiService.TraerContratoCompleto(ncontratoAcuerdo, "acuerdo");
            if (acuerdo.ContratoId == 0)
            {
                errores.Add(string.Concat("El Acuerdo seleccionado no es valido."));
                return JsonCustom(new { info = errores });
            }
            if (Request.Files.Count == 0)
            {
                errores.Add(string.Concat("Debe seleccionar el archivo."));
                return JsonCustom(new { info = errores });
            }
            if (Request.Files.Count > 1)
            {
                errores.Add(string.Concat("Debe seleccionar un solo archivo."));
                return JsonCustom(new { info = errores });
            }

            var fileSubido = Request.Files[0];
            var extension = Path.GetExtension(fileSubido.FileName).ToUpper();
            if (extension != ".XLSX" && extension != ".XLS")
            {
                errores.Add(string.Concat("Archivo no soportado. Debe subir un Excel en formato xlsx."));
                return JsonCustom(new { info = errores });
            }
            if (fileSubido.ContentLength > 0)
            {
                var dsExcel = ExcelImport.LeerExcelDesdeHttpRequest(Request);
                var materiales = dataAgroApiService.BuscarMateriales();
                var centros = dataAgroApiService.BuscarCentros();
                var campanias = dataAgroApiService.BuscarCampanias();
                var validations = GetValidatorContratos(materiales, centros, campanias);
                var validator = new ExcelValidator(validations);

                var resultValidation = validator.Validate(dsExcel.Tables[0], false);

                if (!resultValidation.IsValid)
                {
                    return Json(new { Resume = resultValidation.Resume }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    List<BasicoContrato> contratos = new List<BasicoContrato>();
                    int tiponegocioid = acuerdo.Precio > 0 ? 2 : 1;
                    List<int> rowsOk = resultValidation.RowsResult.Where(a => a.IsValid).Select(a => a.RowNumber).ToList();
                    if (rowsOk.Count == 0)
                    {
                        return Json(new { Resume = resultValidation.Resume }, JsonRequestBehavior.AllowGet);
                    }
                    var rows = dsExcel.Tables[0].AsEnumerable().Select(x => x.ItemArray).Skip(0);
                    for (int ii = 0; ii < rows.Count(); ii++)
                    {
                        if (!rowsOk.Contains(ii))
                            continue;
                        var contrato = new BasicoContrato();
                        contrato.ContratoAcuerdoId = acuerdo.ContratoId;
                        contrato.CorredorId = acuerdo.CorredorId;

                        contrato.ContratoCorredor = rows.ElementAt(ii)[0].ToString().Trim();
                        contrato.ContratoVendedor = rows.ElementAt(ii)[1].ToString().Trim();
                        contrato.MaterialId = materiales.Where(a => a.Descripcion.ToLower() == rows.ElementAt(ii)[2].ToString().Trim().ToLower()).Single().MaterialId;
                        contrato.CampanaId = campanias.Where(a => a.Descripcion.Replace("-", "").ToLower() == rows.ElementAt(ii)[3].ToString().Trim().ToLower()).Single().CampaniaId;
                        contrato.FechaOperacion = DateTime.Parse(rows.ElementAt(ii)[4].ToString().Trim());
                        contrato.FechaDesde = DateTime.Parse(rows.ElementAt(ii)[5].ToString().Trim());
                        contrato.FechaHasta = DateTime.Parse(rows.ElementAt(ii)[6].ToString().Trim());
                        contrato.FechaEntrega = DateTime.Parse(rows.ElementAt(ii)[6].ToString().Trim());
                        contrato.Cantidad = int.Parse(rows.ElementAt(ii)[7].ToString().Trim());
                        contrato.Cuit = rows.ElementAt(ii)[8].ToString().Trim();
                        contrato.ClasificacionId = rows.ElementAt(ii)[9].ToString().Trim().ToLower() == "productor" ? 1 : rows.ElementAt(ii)[9].ToString().Trim().ToLower() == "acopiador" ? 2 : 3;
                        contrato.PlanCanje = rows.ElementAt(ii)[10].ToString().Trim().ToUpper() == "X";
                        contrato.Consignatario = rows.ElementAt(ii)[11].ToString().Trim().ToUpper() == "X";
                        contrato.DestinoId = centros.Where(a => a.Descripcion.ToLower() == rows.ElementAt(ii)[12].ToString().Trim().ToLower()).Single().Id;
                        contrato.LocalidadId = int.Parse(rows.ElementAt(ii)[13].ToString().Trim());
                        contrato.ProvinciaId = int.Parse(rows.ElementAt(ii)[14].ToString().Trim());
                        contrato.Observacion = ii.ToString().Trim();
                        contrato.UsuarioTercero = ClaimsPrincipalExtension.GetClaimValue("emails");


                        contratos.Add(contrato);

                    }

                    validacionContratoFatal(contratos, acuerdo, resultValidation);
                    if (!resultValidation.IsValid)
                    {
                        return Json(new { Resume = resultValidation.Resume }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        List<GrabarContratoResult> resultados = dataAgroApiService.CrearContratoMasivo(contratos);

                        foreach (var item in resultados)
                        {
                            if (item.HayError)
                            {
                                var tipo = item.ListaErrores.Any(a => a.Source == "Fatal") ? ExcelValidationErrorType.Fatal : ExcelValidationErrorType.Error;
                                //item.ContratoId estoy usando ese campo para devolver el numero de row
                                resultValidation.RowsResult[item.ContratoId ?? 0].ItemsResult.Add(new ExcelValidatorItemResult { Errors = item.Errores.Select(a => a.Message).ToList(), Item = new ExcelValidatorItem { ErrorType = tipo, Name = "", Options = null, Position = 1, Required = true, Type = ExcelValidationColumnType.String } });
                            }
                        }
                        return Json(new { Resume = resultValidation.Resume }, JsonRequestBehavior.AllowGet);

                    }
                }


            }
            else
            {
                errores.Add(string.Concat("El archivo ", fileSubido.FileName, " está vacío."));
            }


            if (errores.Count > 0)
            {
                return JsonCustom(new { info = errores });
            }

            return JsonCustom(new { data = SuccessMsg.ArchivoSubidoOK });
        }

        private void validacionContratoFatal(List<BasicoContrato> contratos, BasicoContrato acuerdo, ExcelValidatorResult resultValidation)
        {
            int i = 0;
            foreach (var item in contratos)
            {
                List<ExcelValidatorItemResult> excelValidatorItemResults = new List<ExcelValidatorItemResult>();

                if (item.MaterialId != acuerdo.MaterialId)
                {
                    excelValidatorItemResults.Add(new ExcelValidatorItemResult { Item = new ExcelValidatorItem { Name = "Grano", ErrorType = ExcelValidationErrorType.Fatal }, Errors = new List<string> { "El material no concuerda con el del acuerdo seleccionado. " } });
                }
                if (item.CampanaId != acuerdo.CampanaId)
                {
                    excelValidatorItemResults.Add(new ExcelValidatorItemResult { Item = new ExcelValidatorItem { Name = "Cosecha", ErrorType = ExcelValidationErrorType.Fatal }, Errors = new List<string> { "La cosecha no concuerda con el del acuerdo seleccionado. " } });
                }
                if (item.FechaOperacion.Value.Date != acuerdo.FechaOperacion.Value.Date)
                {
                    excelValidatorItemResults.Add(new ExcelValidatorItemResult { Item = new ExcelValidatorItem { Name = "Fecha Operación", ErrorType = ExcelValidationErrorType.Fatal }, Errors = new List<string> { "La Fecha Operación no concuerda con el del acuerdo seleccionado. " } });
                }
                if (item.FechaOperacion.Value.Date != acuerdo.FechaOperacion.Value.Date)
                {
                    excelValidatorItemResults.Add(new ExcelValidatorItemResult { Item = new ExcelValidatorItem { Name = "Fecha DesdeEntrega", ErrorType = ExcelValidationErrorType.Fatal }, Errors = new List<string> { "La Fecha Desde Entrega no concuerda con el del acuerdo seleccionado. " } });
                }
                if (item.FechaOperacion.Value.Date != acuerdo.FechaOperacion.Value.Date)
                {
                    excelValidatorItemResults.Add(new ExcelValidatorItemResult { Item = new ExcelValidatorItem { Name = "Fecha Vto. Entrega", ErrorType = ExcelValidationErrorType.Fatal }, Errors = new List<string> { "La Fecha Vto. Entrega no concuerda con el del acuerdo seleccionado. " } });
                }
                if (item.DestinoId != acuerdo.DestinoId)
                {
                    excelValidatorItemResults.Add(new ExcelValidatorItemResult { Item = new ExcelValidatorItem { Name = "Destino", ErrorType = ExcelValidationErrorType.Fatal }, Errors = new List<string> { "El Destino no concuerda con el del acuerdo seleccionado. " } });
                }
                if (excelValidatorItemResults.Count > 0)
                {
                    resultValidation.RowsResult[i].ItemsResult.AddRange(excelValidatorItemResults);
                }
                //resultValidation.RowsResult[i].ContratoCorredor = item.ContratoCorredor;
                i++;
            }
        }

        private List<ExcelValidatorItem> GetValidatorContratos(List<MaterialDto> materiales, List<CentroDto> centros, List<CampaniaDto> campanias)
        {
            var ret = new List<ExcelValidatorItem>();
            var pos = 0;

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Contrato Corredor",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Long
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Contrato Vendedor",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = false,
                Type = ExcelValidationColumnType.Long
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Grano",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Options = materiales.Where(a => a.MaterialId < 5).Select(a => a.Descripcion.ToLower()).ToList(),
                Type = ExcelValidationColumnType.List
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Cosecha",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Int
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Fecha Operación",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Date
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Fecha DesdeEntrega",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Date
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Fecha Vto.Entrega",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Date
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "TN",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Int
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "CUIT Vendedor",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Long
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Clasificacion",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = true,
                Options = new List<string>() { "acopiador", "productor", "otros" },
                Type = ExcelValidationColumnType.List
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Plan Canje",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = false,
                Type = ExcelValidationColumnType.Bool
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Consignatario",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = false,
                Type = ExcelValidationColumnType.Bool
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Destino",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Options = centros.Where(a => a.Id != 10).Select(a => a.Descripcion.ToLower()).ToList(),
                Type = ExcelValidationColumnType.List
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "PROCEDENCIA",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Int
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "PROVINCIA",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Int
            });

            //configurar el resto de campos

            return ret;
        }

        public static DateTime FromExcelSerialDate(int SerialDate)
        {
            if (SerialDate > 59) SerialDate -= 1; //Excel/Lotus 2/29/1900 bug   
            return new DateTime(1899, 12, 31).AddDays(SerialDate);
        }

        public ActionResult ObteneContratosAcuerdo()
        {
            var proveedor = ObtenerProveedor();

            return JsonCustom(dataAgroApiService.ObteneContratosAcuerdo(proveedor.IdDataAgro.Value));
        }

        public ActionResult ExcelModeloAltaMasiva()
        {
            var excel = dataAgroApiService.ExcelModeloAltaMasiva();
            PDFResponse result = new PDFResponse
            {
                Pdf = new Pdf()
                {
                    data = excel
                }
            };

            return JsonCustom(result.Pdf);
        }

        private Proveedor ObtenerProveedor()
        {
            Proveedor proveedor;
            string vendedor = SessionPersister.Proveedor;
            string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == userMail);

            if (!usuario.EsAdmin() && !usuario.TienePermiso(PermisoEnum.ElegirTodosVendedores))
            {
                if (!usuario.TieneProveedor(vendedor))
                {
                    throw new ValidationCustomException("Proveedor incorrecto");
                }
                proveedor = usuario.ObtenerProveedorPorCodigo(vendedor);
            }
            else
            {
                proveedor = repositorio.Obtener<Proveedor>(p => p.CodigoProveedor == vendedor);
            }
            return proveedor;
        }


    }
}
