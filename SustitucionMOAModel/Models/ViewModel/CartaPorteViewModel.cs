using SustitucionMOAModel.Models.WSMapMOA.CartaPorte;

namespace SustitucionMOAModel.Models.ViewModel
{
    public class CartaPorteViewModel
    {
        public CartaPorteWSMOAResponse data { get; set; }
        public DropdownContent filtroProducto { get; set; }
        public DropdownContent filtroVendedor { get; set; } 
    }

    public class CartaPorteDescargaViewModel
    {
        public CartaPorteDescargaWSMOAResponse data { get; set; }
        public DropdownContent filtroProducto { get; set; }
        public DropdownContent filtroVendedor { get; set; }
    }

    public class CartaPorteAplicacionViewModel
    {
        public CartaPorteAplicacionWSMOAResponse data { get; set; }
        public DropdownContent filtroProducto { get; set; }
        public DropdownContent filtroVendedor { get; set; }
    }
}
