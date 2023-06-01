
namespace SustitucionMOAModel.Models.WebApiMap.ScatoRepositorio
{
    public class Chofer
    {
        public int Id { get; set; }
        public string Apellido { get; set; }
        public string Nombre { get; set; }
        public int TipoDocumentoIdentidad_Id { get; set; }
        public string NumeroDeDocumento { get; set; }
        public string Cuil { get; set; }
    }
}
