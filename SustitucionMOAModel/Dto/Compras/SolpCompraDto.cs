using System;
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
        public DateTime? _plazoDeOfertaTentativo { private get; set; }
        public string PlazoDeOfertaTentativo
        {
            get
            {
                if (_plazoDeOfertaTentativo == null) { return ""; }
                DateTime dbDate = _plazoDeOfertaTentativo.Value;
                DateTime f = new DateTime(dbDate.Year, dbDate.Month, dbDate.Day, 0, 0, 0, DateTimeKind.Local);
                return f.ToString("O") ?? "";
            }
        }
        public bool MostrarSelectorPlazoDeOferta{ get; set; }
    }
}
