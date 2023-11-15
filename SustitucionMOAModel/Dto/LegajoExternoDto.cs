using System.Collections.Generic;

namespace SustitucionMOAModel.Dto
{
    public class LegajoExternoDto
    {        
        public List<LegajoDto> ListaLegajos { get; set; }
        public string NroOrdenDeCompra { get; set; }
        public string NroSolp { get; set; }
        public UsuarioDto Proveedor { get; set; }
        public string FechaAdjudicacionFormateado { get; set; }
    }
}
