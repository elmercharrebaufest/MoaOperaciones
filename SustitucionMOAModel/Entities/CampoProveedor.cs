namespace SustitucionMOAModel.Entities
{
    public class CampoProveedor
    {
        public int CampoCosecha_Id { get; set; }
        public int Proveedor_Id { get; set; }
        public virtual Proveedor Proveedor { get; set; }

        public double HectareasTotales { get; set; }
        public double HectareasSoja { get; set; }
        public double Longitud { get; set; }
        public double Latitud { get; set; }
        
        public int Archivo_Id { get; set; }
        public virtual Archivo Archivo { get; set; }
    }
}
