using System.Collections.Generic;

namespace SustitucionMOAModel.Dto.Compras.POMultiple
{
    public class PeticionDeOfertaDesvincularDto
    {
        public int NroPeticion { get; set; }

        public int CantidadPosiciones { get; set; }

        public bool HayCotizacion { get; set; }
    }
}
