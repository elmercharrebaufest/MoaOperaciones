using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Dto
{
    public class CotizacionDto
    {
        public int Id { get; set; }
        public int UsuarioCreador_Id { get; set; }
        public int CotizacionEstado_Id { get; set; }
        public int PeticionDeOfertaUsuario_Id { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool? RespetaMateriales { get; set; }
        public bool? RespetaServicios { get; set; }
        public string ObservacionTecnica { get; set; }
        public string ObservacionEconomica { get; set; }
        public int Revision { get; set; }
        public string CotizacionEstadoDescripcion { get; set; }
        public List<LegajoDto> Archivos { get; set; } = new List<LegajoDto>();        
        public bool TieneObservacionTecnica { get; set; }
        public List<CotizacionPosicionDto> CotizacionPosiciones { get; set; } = new List<CotizacionPosicionDto>();
        public bool TieneAdjuntos { get; set; }
    }

    public  class CotizacionPosicionDto
    {
        public int Id { get; set; }
        public List<CotizacionSubposicionDto> CotizacionSubPosiciones { get; set; } = new List<CotizacionSubposicionDto>();
        public decimal Precio { get; set; }
        public int PeticionDeOfertaSolpPosicion_Id { get; set; }
    }

    public class CotizacionSubposicionDto
    {
        public int Id { get; set; }
        public decimal Precio { get; set; }
    }
}
