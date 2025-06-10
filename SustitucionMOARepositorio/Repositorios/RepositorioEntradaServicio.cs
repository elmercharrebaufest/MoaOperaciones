using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SustitucionMOARepositorio.Repositorios
{
    public class RepositorioEntradaServicio : RepositorioEF, IRepositorioEntradaServicio
    {
        public RepositorioEntradaServicio(DbContext context) : base(context) { }

        public List<Solp> ObtenerSolpsAutocertificablesDeOC(List<string> nroSolps)
        {
            var solpsQry =
                from solp in Set<Solp>()
                where
                    nroSolps.Contains(solp.NroSolp)
                    && solp.CertificacionAutomatica
                    && solp.TipoSolpSap != 2
                select solp;

            return solpsQry.ToList();
        }

        public Adjudicacion ObtenerUltimaAdjudicacionOC(string nroOC)
        {
            var adjudicacionQry =
                from adjudicacion in Set<Adjudicacion>()
                where adjudicacion.NumeroOrdenDeCompra == nroOC
                orderby adjudicacion.FechaCreacion descending
                select adjudicacion;

            return adjudicacionQry.FirstOrDefault();
        }
    }
}
