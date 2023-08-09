using System.Linq;

namespace SustitucionMOAModel.Models.DataAgro
{
    public partial class MaterialDto
    {
        public int MaterialId { get; set; }
        public string Descripcion { get; set; }
        public string CampaniaActual { get; set; }
        public int CampaniaIdActual { get; set; }
        public string CampaniaTablero { get; set; }
        public int CampaniaTableroId { get; set; }
        public string Codigo { get; set; }
        public string CodigoSap { get; set; }
        public bool ValidaSisaRuca { get; set; }
        public string Abreviacion { get; set; }

        public MaterialDto() { }

        public string NombreProducto
        {
            get
            {
                try
                {
                    if (string.IsNullOrEmpty(Descripcion))
                        return "";
                    var splited = Descripcion.Split('-');
                    return splited.LastOrDefault();
                }
                catch
                {
                    return "";
                }
            }
        }
    }
}
