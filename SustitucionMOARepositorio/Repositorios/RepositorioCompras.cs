using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.Compras;
using SustitucionMOAModel.Dto.Compras.POMultiple;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOARepositorio.Repositorios
{
    public class RepositorioCompras : RepositorioEF, IRepositorioCompras
    {
        public RepositorioCompras(DbContext context) : base(context) { }

        public List<MaterialSolpDto> BuscarMaterialesCatalogadosPorCodigoSap(string codigoSapMatch, int centroId)
        {
            var materiales = Listar<MaterialSolp>(
                m =>
                    m.Centro_Id == centroId &&
                    m.CodigoSap.Contains(codigoSapMatch) &&
                    m.Estado);

            return materiales.ConvertAll(m => new MaterialSolpDto(m));
        }

        public List<ServicioSolpDto> BuscarServiciosCatalogadosPorCodigoSap(string codigoSapMatch)
        {
            var servicios = Listar<ServicioSolp>(s => s.CodigoSap.ToString().Contains(codigoSapMatch));

            return servicios.ConvertAll(s => new ServicioSolpDto(s));
        }

        public List<PeticionDeOfertaDesvincularDto> ListarPOsDesvinculablesDePosicionMaterial(int solpPosicionId)
        {
            var peticionesDesvinculables = (
                from solpPos in Set<SolpPosicion>()
                join posp in Set<PeticionDeOfertaSolpPosicion>() on solpPos.Id equals posp.SolpPosicion_Id
                join po in Set<PeticionDeOferta>() on posp.PeticionDeOferta_Id equals po.Id
                join pou in Set<PeticionDeOfertaUsuario>() on po.Id equals pou.PeticionDeOferta_Id
                where
                    solpPos.Id == solpPosicionId
                select new PeticionDeOfertaDesvincularDto
                {
                    NroPeticion = posp.PeticionDeOferta_Id,
                    CantidadPosiciones = po.Posiciones.Count,
                    HayCotizacion = pou.Cotizaciones.Any()
                }
                ).ToList();

            return peticionesDesvinculables;
        }

        public List<PeticionDeOfertaDesvincularDto> ListarPOsDesvinculablesDeSolpServicio(int solpId)
        {
            var peticionesDesvinculables = (
                from solp in Set<Solp>()
                join solpPos in Set<SolpPosicion>() on solp.Id equals solpPos.Solp_Id
                join posp in Set<PeticionDeOfertaSolpPosicion>() on solpPos.Id equals posp.SolpPosicion_Id
                join po in Set<PeticionDeOferta>() on posp.PeticionDeOferta_Id equals po.Id
                join pou in Set<PeticionDeOfertaUsuario>() on po.Id equals pou.PeticionDeOferta_Id
                where
                    solp.Id == solpId
                select new PeticionDeOfertaDesvincularDto
                {
                    NroPeticion = posp.PeticionDeOferta_Id,
                    CantidadPosiciones = po.Posiciones.Select(p => p.SolpPosicion).Select(sp => sp.Solp_Id).Distinct().Count(),
                    HayCotizacion = pou.Cotizaciones.Any()
                }
                )
                .Distinct()
                .ToList();

            return peticionesDesvinculables;
        }

        public List<TrabajoYaHechoReporte> ObtenerSolpsReporteTrabajoYaHecho(ICollection<string> nrosSolps)
        {
            var trabajosHechos =
                Set<Solp>()
                .Where(solp =>
                    solp.TrabajoYaHecho == true &&
                    nrosSolps.Contains(solp.NroSolp))
                .Select(solp => new
                {
                    solp.NroSolp,
                    solp.UsuarioCreacion.Mail,
                    solp.FechaCreacion
                })
                .AsEnumerable()
                .Select(solp => new TrabajoYaHechoReporte
                {
                    SolpNro = solp.NroSolp,
                    SolpCreador = solp.Mail,
                    SolpFecha = solp.FechaCreacion.ToString("dd/MM/yyyy")
                })
                .ToList();

            return trabajosHechos;
        }

        public Dictionary<string, DateTime> ObtenerFechasLiberacionOcs(ICollection<string> nrosOcs)
        {
            var fechasLiberacionPorOc =
                Set<Adjudicacion>()
                .Where(adj =>
                    adj.FechaLiberacionSap.HasValue &&
                    nrosOcs.Contains(adj.NumeroOrdenDeCompra))
                .GroupBy(adj => adj.NumeroOrdenDeCompra)
                .ToDictionary(
                    g => g.Key,
                    g => g.Max(x => x.FechaLiberacionSap.Value)
                );
            return fechasLiberacionPorOc;
        }
    }
}
