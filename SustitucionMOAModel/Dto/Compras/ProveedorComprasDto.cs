namespace SustitucionMOAModel.Dto
{
    public class ProveedorComprasDto
    {
        public int Proveedor_Id { get; set; }//Tabla proveedor
        public int Usuario_Id { get; set; }//Tabla usuario
        public string Mail { get; set; }
        public string RazonSocial { get; set; }
        public string CUIT { get; set; }
        public string CodigoProveedor { get; set; }
        public string Moneda { get; set; }

        public ProveedorComprasDto() { }

    }
}
