using Excel;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.Compras;
using SustitucionMOAModel.Dto.Compras.PrecargaSolp;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.Services
{
    public class ComprasArchivosImportService : IComprasArchivosImportService
    {
        private readonly IRepositorioCompras repositorioCompras;

        public ComprasArchivosImportService(IRepositorioCompras repositorioCompras)
        {
            this.repositorioCompras = repositorioCompras;
        }

        public ProcesarPrecargaSolpResponse ProcesarArchivoPrecargaSolp(
            HttpPostedFileBase archivo,
            List<TablaGeneralDto>
            tiposImputaciones,
            List<TablaSapDto> monedas,
            List<TablaSapDto> gruposCompras,
            List<TablaSapDto> gruposArticulos,
            List<TablaSapDto> centros,
            List<TablaSapDto> almacenes,
            List<TablaSapDto> unidades,
            List<TablaSapDto> cuentasMayor,
            List<TablaSapDto> imputaciones,
            TablaGeneralDto tipoPosicion)
        {
            var response = new ProcesarPrecargaSolpResponse();
            if (!ArchivoPrecargaSolpEsValido(archivo, response))
            {
                return response;
            }

            var filas = LeerFilasPrecargaSolp(archivo);
            if (filas.Count() == 0)
            {
                response.ErroresValidacion.Add("El archivo no contiene datos");
                return response;
            }

            var registrosPrecarga = filas.Select(f => LeerRegistroPrecargaSolp(f)).ToList();

            var esMateriales = tipoPosicion.Codigo == "MATERIALES";
            response.ErroresValidacion = ValidarRegistrosPrecargaSolp(registrosPrecarga, tiposImputaciones, monedas, gruposCompras, gruposArticulos, centros, almacenes, unidades, cuentasMayor, imputaciones, esMateriales);

            if (response.ErroresValidacion.Count() > 0)
            {
                return response;
            }

            var posiciones = new List<SolpPosicionPrecargadaDto>();

            foreach (var reg in registrosPrecarga)
            {
                try
                {
                    var indice = int.Parse(reg.NroPosicion);
                    var unidad = unidades.FirstOrDefault(x => x.Codigo == reg.Unidad);
                    var cuentaMayor = cuentasMayor.FirstOrDefault(x => x.CodigoSap == reg.CuentaMayor);
                    var imputacion = imputaciones.FirstOrDefault(x => x.CodigoSap == reg.Imputacion);

                    if (esMateriales || !posiciones.Any(x => x.Indice == indice))
                    {
                        var tipoImputacion = tiposImputaciones.FirstOrDefault(x => x.Descripcion == reg.TipoImputacion);
                        var moneda = monedas.FirstOrDefault(x => x.Codigo == reg.Moneda);
                        var grupoCompras = gruposCompras.FirstOrDefault(x => x.Codigo == reg.GrupoCompras);
                        var grupoArticulo = gruposArticulos.FirstOrDefault(x => x.Codigo == reg.GrupoArticulo);
                        var centro = centros.FirstOrDefault(x => x.Codigo == reg.CentroCodigo);
                        var almacen = almacenes.FirstOrDefault(x => x.Codigo == reg.AlmacenId);

                        var materialCatalogado = ObtenerMaterialCatalogado(esMateriales, reg.CodigoMaterial, centro);

                        var posicion = new SolpPosicionPrecargadaDto
                        {
                            Indice = int.Parse(reg.NroPosicion),
                            TipoPosicionId = tipoPosicion.Id,
                            TipoPosicion = tipoPosicion,
                            TipoImputacionId = tipoImputacion?.Id,
                            TipoImputacion = tipoImputacion,
                            CentroId = centro?.Id,
                            Centro = centro,
                            Codigo = esMateriales ? reg.CodigoMaterial : string.Empty,
                            MaterialCatalogado = materialCatalogado,
                            Tarea = reg.DescripcionItem,
                            MonedaId = moneda?.Id,
                            Moneda = moneda,
                            FechaEntregaServicio = DateTime.Parse(reg.FechaEntrega),
                            GrupoComprasId = grupoCompras?.Id,
                            GrupoCompras = grupoCompras,
                            GrupoArticuloId = grupoArticulo?.Id,
                            GrupoArticulo = grupoArticulo,
                            AlmacenId = almacen?.Id,
                            Almacen = almacen,
                            Cantidad = decimal.Parse(reg.Cantidad),
                            UnidadId = unidad?.Id,
                            Unidad = unidad,
                            CuentaMayor = cuentaMayor,
                            Imputacion = imputacion,
                            Subposiciones = new List<SolpSubposicionPrecargadaDto>()
                        };
                        posiciones.Add(posicion);
                    }
                    if (!esMateriales)
                    {
                        var posicion = posiciones.First(x => x.Indice == indice);
                        var servicioCatalogado = ObtenerServicioCatalogado(esMateriales, reg.CodigoServicio);
                        var subposicion = new SolpSubposicionPrecargadaDto
                        {
                            Numero = int.Parse(reg.NroSubpos),
                            Tarea = reg.NombreServicio,
                            Codigo = reg.CodigoServicio,
                            Cantidad = decimal.Parse(reg.Cantidad),
                            UnidadId = unidad?.Id,
                            Unidad = unidad,
                            CuentaMayor = cuentaMayor,
                            ServicioCatalogado = servicioCatalogado
                        };
                        posicion.Subposiciones.Add(subposicion);
                    }
                }
                catch (Exception ex)
                {
                    response.ErroresValidacion.Add($"Nro posición: {reg.NroPosicion}. {ex.Message}");
                }
            }
            response.Posiciones = posiciones;

            return response;
        }

        private bool ArchivoPrecargaSolpEsValido(HttpPostedFileBase archivo, ProcesarPrecargaSolpResponse response)
        {
            var extension = Path.GetExtension(archivo.FileName);
            if (extension != ".xlsx" && extension != ".xls")
            {
                response.ErroresValidacion.Add("Tipo de archivo no soportado. Debe cargar una planilla Excel (formato xls o xlsx)");
                return false;
            }
            if (archivo.ContentLength == 0)
            {
                response.ErroresValidacion.Add("El archivo está vacío");
                return false;
            }
            return true;
        }

        private IEnumerable<DataRow> LeerFilasPrecargaSolp(HttpPostedFileBase archivo)
        {
            var fileStream = archivo.InputStream;

            var excelReader = archivo.FileName.EndsWith("xlsx") ? ExcelReaderFactory.CreateOpenXmlReader(fileStream) : ExcelReaderFactory.CreateBinaryReader(fileStream);
            excelReader.IsFirstRowAsColumnNames = true;

            var dataSetExcel = excelReader.AsDataSet();

            excelReader.Close();

            var dataRowsExcel = dataSetExcel.Tables[0].Rows
                .Cast<DataRow>()
                .Where(row =>
                    row.ItemArray.Any(celda =>
                        !(celda is DBNull) &&
                        celda != null &&
                        !string.IsNullOrWhiteSpace(celda.ToString())
                    ) &&
                    ((row.ItemArray[IndicePrecargaSolp.LeyendaAmbos] is DBNull ||
                        row.ItemArray[IndicePrecargaSolp.LeyendaAmbos] == null ||
                        row.ItemArray[IndicePrecargaSolp.LeyendaAmbos].ToString() != "Para ambos") &&
                    (row.ItemArray[IndicePrecargaSolp.LeyendaMaterial] is DBNull ||
                        row.ItemArray[IndicePrecargaSolp.LeyendaMaterial] == null ||
                        row.ItemArray[IndicePrecargaSolp.LeyendaMaterial].ToString() != "Para Material") &&
                    (row.ItemArray[IndicePrecargaSolp.LeyendaServicios] is DBNull ||
                        row.ItemArray[IndicePrecargaSolp.LeyendaServicios] == null ||
                        row.ItemArray[IndicePrecargaSolp.LeyendaServicios].ToString() != "Para servicios"))
                );
            return dataRowsExcel;
        }

        private RegistroPrecargaSolp LeerRegistroPrecargaSolp(DataRow fila)
        {
            var registroPrecargaSolp = new RegistroPrecargaSolp
            {
                NroPosicion = (fila[IndicePrecargaSolp.NroPosicion] is DBNull || fila[IndicePrecargaSolp.NroPosicion] == null) ? null : fila[IndicePrecargaSolp.NroPosicion].ToString(),
                TipoImputacion = (fila[IndicePrecargaSolp.TipoImputacion] is DBNull || fila[IndicePrecargaSolp.TipoImputacion] == null) ? null : fila[IndicePrecargaSolp.TipoImputacion].ToString(),
                CodigoMaterial = (fila[IndicePrecargaSolp.CodigoMaterial] is DBNull || fila[IndicePrecargaSolp.CodigoMaterial] == null) ? null : fila[IndicePrecargaSolp.CodigoMaterial].ToString(),
                DescripcionItem = (fila[IndicePrecargaSolp.DescripcionItem] is DBNull || fila[IndicePrecargaSolp.DescripcionItem] == null) ? null : fila[IndicePrecargaSolp.DescripcionItem].ToString(),
                Moneda = (fila[IndicePrecargaSolp.Moneda] is DBNull || fila[IndicePrecargaSolp.Moneda] == null) ? null : fila[IndicePrecargaSolp.Moneda].ToString(),
                FechaEntrega = (fila[IndicePrecargaSolp.FechaEntrega] is DBNull || fila[IndicePrecargaSolp.FechaEntrega] == null) ? null : fila[IndicePrecargaSolp.FechaEntrega].ToString(),
                GrupoCompras = (fila[IndicePrecargaSolp.GrupoCompras] is DBNull || fila[IndicePrecargaSolp.GrupoCompras] == null) ? null : fila[IndicePrecargaSolp.GrupoCompras].ToString(),
                GrupoArticulo = (fila[IndicePrecargaSolp.GrupoArticulo] is DBNull || fila[IndicePrecargaSolp.GrupoArticulo] == null) ? null : fila[IndicePrecargaSolp.GrupoArticulo].ToString(),
                CentroCodigo = (fila[IndicePrecargaSolp.CentroId] is DBNull || fila[IndicePrecargaSolp.CentroId] == null) ? null : fila[IndicePrecargaSolp.CentroId].ToString(),
                AlmacenId = (fila[IndicePrecargaSolp.AlmacenId] is DBNull || fila[IndicePrecargaSolp.AlmacenId] == null) ? null : fila[IndicePrecargaSolp.AlmacenId].ToString(),
                NroSubpos = (fila[IndicePrecargaSolp.NroSubpos] is DBNull || fila[IndicePrecargaSolp.NroSubpos] == null) ? null : fila[IndicePrecargaSolp.NroSubpos].ToString(),
                CodigoServicio = (fila[IndicePrecargaSolp.CodigoServicio] is DBNull || fila[IndicePrecargaSolp.CodigoServicio] == null) ? null : fila[IndicePrecargaSolp.CodigoServicio].ToString(),
                NombreServicio = (fila[IndicePrecargaSolp.NombreServicio] is DBNull || fila[IndicePrecargaSolp.NombreServicio] == null) ? null : fila[IndicePrecargaSolp.NombreServicio].ToString(),
                Cantidad = (fila[IndicePrecargaSolp.Cantidad] is DBNull || fila[IndicePrecargaSolp.Cantidad] == null) ? null : fila[IndicePrecargaSolp.Cantidad].ToString(),
                Unidad = (fila[IndicePrecargaSolp.Unidad] is DBNull || fila[IndicePrecargaSolp.Unidad] == null) ? null : fila[IndicePrecargaSolp.Unidad].ToString(),
                CuentaMayor = (fila[IndicePrecargaSolp.CuentaMayor] is DBNull || fila[IndicePrecargaSolp.CuentaMayor] == null) ? null : fila[IndicePrecargaSolp.CuentaMayor].ToString(),
                Imputacion = (fila[IndicePrecargaSolp.Imputacion] is DBNull || fila[IndicePrecargaSolp.Imputacion] == null) ? null : fila[IndicePrecargaSolp.Imputacion].ToString()
            };
            return registroPrecargaSolp;
        }

        private List<string> ValidarRegistrosPrecargaSolp(List<RegistroPrecargaSolp> registrosPrecarga,
            List<TablaGeneralDto> tiposImputaciones,
            List<TablaSapDto> monedas,
            List<TablaSapDto> gruposCompras,
            List<TablaSapDto> gruposArticulos,
            List<TablaSapDto> centros,
            List<TablaSapDto> almacenes,
            List<TablaSapDto> unidades,
            List<TablaSapDto> cuentasMayor,
            List<TablaSapDto> imputaciones,
            bool esMateriales)
        {
            var errores = new List<string>();
            var ordenFila = 1;

            foreach (var reg in registrosPrecarga)
            {
                if (!int.TryParse(reg.NroPosicion, out int _))
                {
                    errores.Add($"Orden: {ordenFila}. El número de posición no es válido");
                }
                if (!tiposImputaciones.Any(x => x.Descripcion == reg.TipoImputacion))
                {
                    errores.Add($"Orden: {ordenFila}. El tipo de imputación no es válido");
                }
                if (!monedas.Any(x => x.Codigo == reg.Moneda))
                {
                    errores.Add($"Orden: {ordenFila}. La moneda no es válida");
                }
                if (!DateTime.TryParse(reg.FechaEntrega, out DateTime _))
                {
                    errores.Add($"Orden: {ordenFila}. La fecha no es válida");
                }
                if (!gruposCompras.Any(x => x.CodigoSap == reg.GrupoCompras))
                {
                    errores.Add($"Orden: {ordenFila}. El grupo de compras no es válido");
                }
                if (!gruposArticulos.Any(x => x.Codigo == reg.GrupoArticulo))
                {
                    errores.Add($"Orden: {ordenFila}. El grupo de artículo no es válido");
                }
                if (!centros.Any(x => x.Codigo == reg.CentroCodigo))
                {
                    errores.Add($"Orden: {ordenFila}. El centro no es válido");
                }
                if (!almacenes.Any(x => x.Codigo == reg.AlmacenId))
                {
                    errores.Add($"Orden: {ordenFila}. El almacén no es válido");
                }
                if (!decimal.TryParse(reg.Cantidad, out decimal _))
                {
                    errores.Add($"Orden: {ordenFila}. La cantidad no es válida");
                }
                if (!unidades.Any(x => x.Codigo == reg.Unidad))
                {
                    errores.Add($"Orden: {ordenFila}. La unidad no es válida");
                }
                if (!string.IsNullOrWhiteSpace(reg.CuentaMayor) && !cuentasMayor.Any(x => x.CodigoSap == reg.CuentaMayor))
                {
                    errores.Add($"Orden: {ordenFila}. La cuenta de mayor no es válida");
                }

                if (esMateriales)
                {
                    if (!string.IsNullOrWhiteSpace(reg.NroSubpos) || !string.IsNullOrWhiteSpace(reg.CodigoServicio) || !string.IsNullOrWhiteSpace(reg.NombreServicio))
                    {
                        errores.Add($"Orden: {ordenFila}. La posición tiene datos en campos de Servicio");
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(reg.CodigoMaterial))
                    {
                        errores.Add($"Orden: {ordenFila}. La posición tiene datos en campos de Materiales");
                    }
                    if (!int.TryParse(reg.NroSubpos, out int _))
                    {
                        errores.Add($"Orden: {ordenFila}. El número de subposición no es válido");
                    }
                }
                ordenFila++;
            }

            return errores;
        }

        private MaterialSolpDto ObtenerMaterialCatalogado(bool esMateriales, string codigoMaterial, TablaSapDto centro)
        {
            if (!esMateriales || centro == null || string.IsNullOrWhiteSpace(codigoMaterial)) { return null; }

            var materialesCatalogados = repositorioCompras.BuscarMaterialesCatalogadosPorCodigoSap(codigoMaterial, centro.Id);

            if (materialesCatalogados.Count() > 1)
            {
                throw new Exception("Existe más de un material con el código " + codigoMaterial);
            }
            return materialesCatalogados.FirstOrDefault();
        }

        private ServicioSolpDto ObtenerServicioCatalogado(bool esMateriales, string codigoServicio)
        {
            if (esMateriales || string.IsNullOrWhiteSpace(codigoServicio)) { return null; }

            var serviciosCatalogados = repositorioCompras.BuscarServiciosCatalogadosPorCodigoSap(codigoServicio);

            if (serviciosCatalogados.Count() > 1)
            {
                throw new Exception("Existe más de un servicio con el código " + codigoServicio);
            }
            return serviciosCatalogados.FirstOrDefault();
        }
    }

    internal class RegistroPrecargaSolp
    {
        public string NroPosicion { get; set; }
        public string TipoImputacion { get; set; }
        public string CodigoMaterial { get; set; }
        public string DescripcionItem { get; set; }
        public string Moneda { get; set; }
        public string FechaEntrega { get; set; }
        public string GrupoCompras { get; set; }
        public string GrupoArticulo { get; set; }
        public string CentroCodigo { get; set; }
        public string AlmacenId { get; set; }
        public string NroSubpos { get; set; }
        public string CodigoServicio { get; set; }
        public string NombreServicio { get; set; }
        public string Cantidad { get; set; }
        public string Unidad { get; set; }
        public string CuentaMayor { get; set; }
        public string Imputacion { get; set; }
    }

    internal static class IndicePrecargaSolp
    {
        internal const int NroPosicion = 0;
        internal const int TipoImputacion = 1;
        internal const int CodigoMaterial = 2;
        internal const int DescripcionItem = 3;
        internal const int Moneda = 4;
        internal const int FechaEntrega = 5;
        internal const int GrupoCompras = 6;
        internal const int GrupoArticulo = 7;
        internal const int CentroId = 8;
        internal const int AlmacenId = 9;
        internal const int NroSubpos = 10;
        internal const int CodigoServicio = 11;
        internal const int NombreServicio = 12;
        internal const int Cantidad = 13;
        internal const int Unidad = 14;
        internal const int CuentaMayor = 15;
        internal const int Imputacion = 16;

        internal const int LeyendaAmbos = 11;
        internal const int LeyendaMaterial = 12;
        internal const int LeyendaServicios = 13;
    }
}
