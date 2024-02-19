using System;
using System.Collections.Generic;

namespace SustitucionMOAModel.Dto
{
    public class AdjudicacionPosicionEditarDto
    {   
        
        public List<AdjudicacionSubPosicionEditarDto> SubPosiciones { get; set; } = new List<AdjudicacionSubPosicionEditarDto>();
        public int Indice { get; set; }       
        public decimal PrecioUnidadCodigo { get; set; }        
        public bool Eliminado { get; set; }
        public string RegionCodigo { get; set; }
        public string PaisCodigo { get; set; }
        public DateTime FechaEntrega { get; set; }
        public bool EntregaFinal { get; set; }
        public decimal Cantidad { get; set; }
    }
}
