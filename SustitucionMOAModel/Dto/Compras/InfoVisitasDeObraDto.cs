using System;
using System.Collections.Generic;
using SustitucionMOAModel.Entities;

namespace SustitucionMOAModel.Dto
{
    public class InfoVisitasDeObraDto
    {    
        public int CantidadVisitas { get; set; }
        public List<DetalleVisitaDto> DetalleVisitas { get; set; }
    }

    public class DetalleVisitaDto
    {
        public DateTime? FechaHora { get; set; }
        public List<ProveedorDto> Proveedores { get; set; }
        public string NroSolp { get; set; }
        public int PliegoId { get; set; }
    }
}