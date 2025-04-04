using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAModel.Dto.PliegoMultiple
{
    public class PliegoPMDto
    {
        public int Id { get; set; }

        public string NombreObra { get; set; }

        public DateTime FechaAlta { get; set; } = DateTime.Now;

        public DateTime? FechaModificacion { get; set; }

        public IEnumerable<string> Solps { get; set; }

        public bool MultipleFinalizado { get; set; }

        public static PliegoPMDto FromPliego(Pliego pliego)
        {
            return new PliegoPMDto
            {
                Id = pliego.Id,
                NombreObra = pliego.NombreObra,
                FechaAlta = pliego.FechaAlta,
                FechaModificacion = pliego.FechaModificacion,
                Solps = pliego.Solps?.Select(s => s.NroSolp),
                MultipleFinalizado = pliego.MultipleFinalizado
            };
        }

        public static explicit operator PliegoPMDto(Pliego v)
        {
            return FromPliego(v);
        }
    }
}
