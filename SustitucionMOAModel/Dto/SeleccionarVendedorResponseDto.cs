using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Noticia;

namespace SustitucionMOAModel.Dto
{
    public class SeleccionarVendedorResponseDto
    {
        public string CodigoVendedor { get; set; }
        public string Descripcion { get; set; }
        public NoticiasDetallesWSMOAResponse Noticias { get; set; }
        public bool EsCodigoCorredor { get; set; }

        public string TipoUsuario { get; set; }
        public int ProveedorId { get; set; }

        public SeleccionarVendedorResponseDto(Proveedor proveedor, NoticiasDetallesWSMOAResponse noticias)
        {
            CodigoVendedor = proveedor.CodigoProveedor;
            Descripcion = proveedor.RazonSocial;
            Noticias = noticias;
            EsCodigoCorredor = proveedor.TipoProveedor.EsCorredor;
            TipoUsuario = proveedor.TipoProveedor.NombreCorto;
            ProveedorId = proveedor.Id;
        }
    }
}
