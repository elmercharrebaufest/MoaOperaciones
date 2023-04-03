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
    public class ListarSolpPOConsulta : IConsultaPaginada<PeticionDeOfertaDto>
    {
        private readonly Paginacion paginacion;
        private readonly string NroSolp;
        public ListarSolpPOConsulta(Paginacion paginacion, string nroSolp)
        {
            this.paginacion = paginacion;
            this.NroSolp = nroSolp;


        }
        public ListaPaginada<PeticionDeOfertaDto> Ejecutar(DbContext contexto)
        {
            var hoy = DateTime.Now.Date;
            var ayer = hoy.AddDays(-1);
            try
            {
                ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
                var resultado = from x in contexto.Set<PeticionDeOferta>()
                                join cotizacion in contexto.Set<Cotizacion>() on x.Id equals cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta_Id into peticionCotizacion
                                from cotizacion in peticionCotizacion.DefaultIfEmpty()
                                where (string.IsNullOrEmpty(NroSolp) || x.Solp.NroSolp.ToUpper().StartsWith(NroSolp.ToUpper()))
                                select new PeticionDeOfertaDto
                                {
                                    Id = x.Id,
                                    NroSolp = x.Solp.NroSolp,
                                    NombreDeObra = x.Solp.Pliego == null ? "" : x.Solp.Pliego.NombreObra,
                                    UsuarioCreador_Id = x.UsuarioCreador_Id,
                                    UsuarioCreador = x.Usuario.Mail,                    
                                    Circular = contexto.Set<CircularPeticionDeOfertaUsuario>().Any(cp => cp.PeticionDeOfertaUsuario.PeticionDeOferta_Id == x.Id && cp.Circular.PlazoDeOferta != null) ?  (from circularPO in contexto.Set<CircularPeticionDeOfertaUsuario>()
                                                     where circularPO.PeticionDeOfertaUsuario.PeticionDeOferta_Id == x.Id
                                                     orderby circularPO.Circular.PlazoDeOferta descending
                                                      select new CircularDto { 
                                                          PlazoDeOferta = circularPO.Circular.PlazoDeOferta,
                                                          Estado = circularPO.Circular.PlazoDeOferta >= hoy ? "Abierto" : "Cerrado",
                                                          EstadoColor = circularPO.Circular.PlazoDeOferta >= hoy ? "Green" : "Red",
                                                          Estado_Id = circularPO.Circular.PlazoDeOferta >= hoy ? 1 : 2,

                                                      }

                                                     ).FirstOrDefault() : new CircularDto { 
                                                         PlazoDeOferta = x.PlazoDeOferta,
                                                         Estado = x.PlazoDeOferta >= hoy ? "Abierto" : "Cerrado",
                                                         EstadoColor = x.PlazoDeOferta >= hoy ? "Green" : "Red",
                                                         Estado_Id = x.PlazoDeOferta >= hoy ? 1 : 2,
                                                     },

                                    TieneVisitaObra = x.Solp.Pliego.TieneVisitaObra == null ? "No requiere visita" : "Requiere visita a coordinar",
                                    CotizacionEstadoDescripcion = cotizacion == null ? "Sin Cotizar" : cotizacion.CotizacionEstado.Descripcion,
                                    CotizacionEstado_Id = cotizacion == null ? 0 : cotizacion.CotizacionEstado.Id,
                                    ItemPorPagina = paginacion.ItemsPorPagina,
                                    Pagina = paginacion.Pagina,
                                    
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
