using System.Collections.Generic;

namespace SustitucionMOAModel.Dto
{
    public class DeclaracionCampoSustentableDto
    {
        public string RazonSocial { get; set; }
        public string CUIT { get; set; }
        public string Fecha { get; set; }
        public string Cosecha { get; set; }
        public double CantidadParteSoja { get; set; }
        public List<CamposSustentableReporte> Campos { get; set; }
        public string DirectivaDDJJCampoSustentable { get; set; }
    }
}
