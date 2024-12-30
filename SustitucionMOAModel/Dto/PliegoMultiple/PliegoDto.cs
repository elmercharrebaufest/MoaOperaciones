using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAModel.Dto.PliegoMultiple
{
    public class PliegoDto
    {
        public int Id { get; set; }

        public string NombreObra { get; set; }

        public DateTime FechaAlta { get; set; } = DateTime.Now;

        public DateTime? FechaModificacion { get; set; }

        public IEnumerable<string> Solps { get; set; }

        public static PliegoDto FromPliego(Pliego pliego)
        {
            return new PliegoDto
            {
                Id = pliego.Id,
                NombreObra = pliego.NombreObra,
                FechaAlta = pliego.FechaAlta,
                FechaModificacion = pliego.FechaModificacion,
                Solps = pliego.Solps?.Select(s => s.NroSolp),
            };
        }

        public static explicit operator PliegoDto(Pliego v)
        {
            return FromPliego(v);
        }
    }
}
