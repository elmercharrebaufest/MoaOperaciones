using System;
using System.Collections.Generic;

namespace SustitucionMOAModel.Dto.Compras
{
    public class ActualizarFechaVigenciaRegistroInfoDto
    {
        public List<IdentificadorRegistroInfo> RegistrosInfo { get; set; }
        public DateTime NuevaFechaVigencia { get; set; }
    }
    public class IdentificadorRegistroInfo
    {
        public int CotizacionPosicion_Id { get; set; }
        public int SolpPosicion_Id { get; set; }
    }
}
