using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.Compras.PrecargaSolp
{
    public class SolpPosicionPrecargadaDto
    {
        public int Indice { get; set; }

        public int TipoPosicionId { get; set; }
        public TablaGeneralDto TipoPosicion { get; set; }
        
        public string Codigo { get; set; }
        
        public int? TipoImputacionId { get; set; }
        public TablaGeneralDto TipoImputacion { get; set; }

        public string Tarea { get; set; }

        public int? MonedaId { get; set; }
        public TablaSapDto Moneda { get; set; }

        public DateTime? FechaEntregaServicio { get; set; }

        public int? GrupoComprasId { get; set; }
        public TablaSapDto GrupoCompras { get; set; }

        public int? GrupoArticuloId { get; set; }
        public TablaSapDto GrupoArticulo { get; set; }

        public int? CentroId { get; set; }
        public TablaSapDto Centro { get; set; }

        public int? AlmacenId { get; set; }
        public TablaSapDto Almacen { get; set; }

        public decimal? Cantidad { get; set; }

        public int? UnidadId { get; set; }
        public TablaSapDto Unidad { get; set; }

        public TablaSapDto CuentaMayor { get; set; }

        public List<SolpSubposicionPrecargadaDto> Subposiciones { get; set; }
    }
}
