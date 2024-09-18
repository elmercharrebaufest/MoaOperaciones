using System;
using System.Collections.Generic;

namespace SustitucionMOAModel.Dto
{
    public class SolpCompraDto
    {
        public int Id { get; set; }
        public string NroSolp { get; set; }
        public List<SolpPosicionDto> PosicionCompras { get; set; }
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
        public bool MostrarSelectorPlazoDeOferta { get; set; }
    }

    public static class SolpCompraDtoExtensions
    {
        public static bool EsTipoMaterial(this SolpCompraDto solpCompraDto)
            => string.Compare(solpCompraDto.TipoPosicionCodigo, "MATERIALES", true) == 0;

        public static bool EsTipoServicio(this SolpCompraDto solpCompraDto)
            => string.Compare(solpCompraDto.TipoPosicionCodigo, "SERVICIO", true) == 0;
    }
}
