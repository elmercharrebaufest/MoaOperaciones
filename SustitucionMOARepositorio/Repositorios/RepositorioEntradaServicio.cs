using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOARepositorio.Repositorios
{
    public class RepositorioEntradaServicio : RepositorioEF, IRepositorioEntradaServicio
    {
        public RepositorioEntradaServicio(DbContext context) : base(context) { }

        public List<Solp> ObtenerSolpsAutocertificablesDeOC(string nroOC)
        {
            var solpsQry =
                from adjudicacion in Set<Adjudicacion>()
                join cotizacion in Set<Cotizacion>() on adjudicacion.Cotizacion_Id equals cotizacion.Id
                join peticionDeOfertaUsuario in Set<PeticionDeOfertaUsuario>() on cotizacion.PeticionDeOfertaUsuario_Id equals peticionDeOfertaUsuario.Id
                join peticionDeOferta in Set<PeticionDeOferta>() on peticionDeOfertaUsuario.PeticionDeOferta_Id equals peticionDeOferta.Id
                join peticionDeOfertaSolpPosicion in Set<PeticionDeOfertaSolpPosicion>() on peticionDeOferta.Id equals peticionDeOfertaSolpPosicion.PeticionDeOferta_Id
                join solpPosicion in Set<SolpPosicion>() on peticionDeOfertaSolpPosicion.SolpPosicion_Id equals solpPosicion.Id
                join solp in Set<Solp>() on solpPosicion.Solp_Id equals solp.Id
                where
                    adjudicacion.NumeroOrdenDeCompra == nroOC &&
                    solp.CertificacionAutomatica
                select solp;

            return solpsQry.ToList();
        }
    }
}
