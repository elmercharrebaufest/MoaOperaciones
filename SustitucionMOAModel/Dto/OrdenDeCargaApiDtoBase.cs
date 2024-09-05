using System.Linq;

namespace SustitucionMOAModel.Dto
{
    public abstract class OrdenDeCargaApiDtoBase
    {
        public long Id { get; set; }
        public string Cliente { get; set; }
        public string CodigoProducto { get; set; }
        public string CUILChofer { get; set; }
        public string CUITCliente { get; set; }
        public string CUITTransporte { get; set; }
        public string DescripcionProducto { get; set; }
        public string DomicilioDescr { get; set; }
        public short? DomicilioOrden { get; set; }
        public string DomicilioTipo { get; set; }
        public string FechaCreacion { get; set; }
        public string LocalidadDescripcion { get; set; }
        public int LocalidadId { get; set; }
        public string NombreChofer { get; set; }
        public string ApellidoChofer { get; set; }
        public string Observacion { get; set; }
        public string PatenteAcoplado { get; set; }
        public string PatenteChasis { get; set; }
        public string PlantaCodigo { get; set; }
        public string RazonSocialTransporte { get; set; }
        public string TipoOrden { get; set; }


        protected static string ParseNombreProducto(string nombreMaterial)
        {
            try
            {
                var split = nombreMaterial.Split('-');

                return split.LastOrDefault()?.Trim();
            }
            catch
            {
                return "";
            }
        }
    }

    internal static class TipoOrdenes
    {
        internal static readonly string FAS = "FAS";
        internal static readonly string FASON = "FASON";
        internal static readonly string RESIDUOS = "RESIDUOS";
    }
}
