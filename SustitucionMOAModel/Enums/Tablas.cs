using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Enums
{
    public static class TablasEstado
    {
        public const string EstadoDocumento = "EstadoDocumento";
    }

    public static class TablasSap
    {
        public const string ClaseDocumento = "ClaseDocumento";
        public const string Almacen = "Almacen";
        public const string GrupoCompras = "GrupoCompras";
        public const string GrupoArticulo = "GrupoArticulo";
        public const string Moneda = "Moneda";
        public const string Unidad = "Unidad";
        public const string CodigoServicioSap = "CodigoServicioSap";
        public const string EstadoSolpSap = "EstadoSolpSap";
    }

    public static class TablasGenerales
    {
        public const string TipoImputacionSolp = "TipoImputacionSolp";
        public const string TipoFiltroSolpProveedor = "TipoFiltroSolpProveedor";
        public const string TipoPliegoArchivo = "TipoPliegoArchivo";
        public const string CamposObligatoriosCabeceraSolp = "CamposObligatoriosCabeceraSolp";
    }
}
