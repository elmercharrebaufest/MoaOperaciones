using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Entities;

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
