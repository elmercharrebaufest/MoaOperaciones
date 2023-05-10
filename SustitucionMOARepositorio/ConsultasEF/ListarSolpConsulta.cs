using Molinos.Scato.Repositorio;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio.Extensiones;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Linq;

namespace SustitucionMOARepositorio.ConsultasEF
{
    public class ListarSolpConsulta : IConsultaPaginada<SolpDto>
    {
        private readonly Paginacion paginacion;
        private readonly string NroSolp;
        public ListarSolpConsulta(Paginacion paginacion, string nroSolp)
        {
            this.paginacion = paginacion;
            this.NroSolp = nroSolp;


        }
        public ListaPaginada<SolpDto> Ejecutar(DbContext contexto)
        {
            var hoy = DateTime.Now;
            var ayer = hoy.AddDays(-1);
            try
            {
                ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
                var resultado = from x in contexto.Set<Solp>()
                                where (string.IsNullOrEmpty(NroSolp) || x.NroSolp.ToUpper().StartsWith(NroSolp.ToUpper())) &&
                                x.Posiciones.All(p => p.NumeroContratoSuperior == "" || p.NumeroContratoSuperior == null)
                                select new SolpDto
                                {
                                    UsuarioActual = new UsuarioDto { Mail = x.UsuarioCreacion != null ? x.UsuarioCreacion.Mail : "" },
                                    Id = x.Id,
                                    NroSolp = x.NroSolp,
                                    NombreDeObra = x.Pliego == null ? "" : x.Pliego.NombreObra,
                                    FechaCreacionFormateada = SqlFunctions.DateName("day", x.FechaCreacion) + "/" + SqlFunctions.DatePart("month", x.FechaCreacion) + "/" + SqlFunctions.DateName("year", x.FechaCreacion),
                                    FechaCreacion = x.FechaCreacion,
                                    TipoSolp = new TablaGeneralDto { Descripcion = x.TipoSolp != null ? x.TipoSolp.Descripcion : "" },
                                    TipoSolpSap = x.TipoSolpSap,
                                    PosicionCompras = (from posicion in contexto.Set<SolpPosicion>()
                                                       where posicion.Solp_Id == x.Id
                                                       select new SolpPosicionDto()
                                                       {
                                                           GrupoComprasDescripcion = posicion.GrupoCompras != null ? posicion.GrupoCompras.Descripcion : "",
                                                           CentroComprasDescripcion = posicion.Centro != null ? posicion.Centro.Descripcion : "",
                                                           GrupoComprasId = posicion.GrupoCompras != null ? posicion.GrupoCompras.Id : (int?)null,
                                                           CentroId = posicion.Centro != null ? posicion.Centro.Id : (int?)null
                                                       }),
                                    ItemPorPagina = paginacion.ItemsPorPagina,
                                    Pagina = paginacion.Pagina,
                                    EstadoSolpSap = x.EstadoSolpSap_Id != null ? new TablaSapDto { Id = x.EstadoSolpSap_Id ?? 0, CodigoSap = x.EstadoSolpSap.CodigoSap, Descripcion = x.EstadoSolpSap.Descripcion } : new TablaSapDto { Id = 0, CodigoSap = "", Descripcion = "" },
                                    FechaLiberacionSapFormateada = x.FechaLiberacionSap == null ? "" : SqlFunctions.DateName("day", x.FechaLiberacionSap) + "/" + SqlFunctions.DatePart("month", x.FechaLiberacionSap) + "/" + SqlFunctions.DateName("year", x.FechaLiberacionSap),
                                    FechaLiberacionSap = x.FechaLiberacionSap,
                                    PeticionesDeOferta = (from po in contexto.Set<PeticionDeOferta>()
                                                          where po.Solp_Id == x.Id
                                                          select new PeticionDeOfertaDto()
                                                          {
                                                              Id = po.Id,
                                                              Solp_Id = po.Solp_Id,
                                                              FechaCreacion = po.FechaCreacion,
                                                              UsuarioCreador_Id = po.UsuarioCreador_Id,
                                                              PlazoDeOferta =
                                                              po.Usuarios.GroupBy(p => p).SelectMany(p => p.Key.Circulares)
                                                               .Any(p => p.Circular.RequiereCambioDeFechas == true && p.Circular.PlazoDeOferta.HasValue) ?
                                                               po.Usuarios.GroupBy(p => p).SelectMany(p => p.Key.Circulares)
                                                               .Where(p => p.Circular.RequiereCambioDeFechas == true && p.Circular.PlazoDeOferta.HasValue)
                                                               .OrderByDescending(p => p.Circular.PlazoDeOferta).FirstOrDefault().Circular.PlazoDeOferta.Value :
                                                                po.PlazoDeOferta,
                                                              Observaciones = po.Observaciones,
                                                          }),
                                };

                var itemsTotales = resultado.Count();

                return resultado.OrdenarPaginarLista(paginacion);

            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}
