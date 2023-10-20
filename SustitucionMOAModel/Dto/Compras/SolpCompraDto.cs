using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAModel.Dto
{
    public class SolpCompraDto
    {
        public int Id { get; set; }
        public string NroSolp { get; set; }
        public IQueryable<SolpPosicionDto> PosicionCompras { get; set; }
        public string TipoPosicionCodigo { get; set; }
        public List<RegistroInfoDto> RegistrosInfo { get; set; }
        public bool? Adicional { get; set; }
    }
}
