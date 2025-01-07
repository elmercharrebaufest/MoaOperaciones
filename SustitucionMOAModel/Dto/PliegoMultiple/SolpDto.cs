using SustitucionMOAModel.Entities;
using System;

namespace SustitucionMOAModel.Dto.PliegoMultiple
{
    public class SolpDto
    {
        public int Id { get; set; }

        public int TipoSolpSap { get; set; }

        public string NumeroSolp { get; set; }

        public string NombreSolp { get; set; }

        public DateTime FechaCreacion { get; set; }

        public string Creador { get; set; }

        public string Fiscal { get; set; }

        public string Estado { get; set; }

        public static SolpDto FromPliego(Solp solp)
        {
            return new SolpDto
            {
                Id = solp.Id,
                TipoSolpSap = solp.TipoSolpSap ?? 0,
                NumeroSolp = solp.NroSolp,
                NombreSolp = solp.Pliego?.NombreObra,
                FechaCreacion = solp.FechaCreacion,
                Creador = solp.UsuarioCreacion?.ObtenerRazonSocial(),
                Fiscal = solp.Pliego?.FiscalContrato,
                Estado = solp.EstadoSolpSap?.Descripcion,
            };
        }

        public static explicit operator SolpDto(Solp v)
        {
            return FromPliego(v);
        }
    }
}
