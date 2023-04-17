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
        private readonly int usuario_id;
        public ListarSolpPOConsulta(Paginacion paginacion, string nroSolp, int usuario_id)
        {
            this.paginacion = paginacion;
            this.NroSolp = nroSolp;
            this.usuario_id = usuario_id;


        }
        public ListaPaginada<PeticionDeOfertaDto> Ejecutar(DbContext contexto)
        {
            var hoy = DateTime.Now.Date;
            var ayer = hoy.AddDays(-1);
            try
            {
                ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
                var resultado = from x in contexto.Set<PeticionDeOfertaUsuario>()
                                join peticionDeOferta in contexto.Set<PeticionDeOferta>() on x.PeticionDeOferta_Id equals peticionDeOferta.Id into peticionDeOferta
                                join cotizacion in contexto.Set<Cotizacion>() on x.Id equals cotizacion.PeticionDeOfertaUsuario_Id into peticionCotizacion
                                from cotizacion in peticionCotizacion.DefaultIfEmpty()
                                where (string.IsNullOrEmpty(NroSolp) || x.PeticionDeOferta.Solp.NroSolp.ToUpper().StartsWith(NroSolp.ToUpper()))
                                && x.Usuario_Id == usuario_id
                                select new PeticionDeOfertaDto
                                {
                                    Id = x.PeticionDeOferta.Id,
                                    NroSolp = x.PeticionDeOferta.Solp.NroSolp,
                                    NombreDeObra = x.PeticionDeOferta.Solp.Pliego == null ? "" : x.PeticionDeOferta.Solp.Pliego.NombreObra,
                                    UsuarioCreador_Id = x.PeticionDeOferta.UsuarioCreador_Id,
                                    UsuarioCreador = x.PeticionDeOferta.Usuario.Mail,
                                    Circular = contexto.Set<CircularPeticionDeOfertaUsuario>().Any(cp => cp.PeticionDeOfertaUsuario_Id == x.Id && cp.Circular.PlazoDeOferta != null) ?
                                    (from circularPO in contexto.Set<CircularPeticionDeOfertaUsuario>()
                                     where circularPO.PeticionDeOfertaUsuario_Id == x.Id
                                     orderby circularPO.Circular.Id descending
                                     select new CircularDto
                                     {
                                         PlazoDeOferta = circularPO.Circular.PlazoDeOferta,
                                         Estado = circularPO.Circular.PlazoDeOferta >= hoy ? "Abierto" : "Cerrado",
                                         EstadoColor = circularPO.Circular.PlazoDeOferta >= hoy ? "Green" : "Red",
                                         Estado_Id = circularPO.Circular.PlazoDeOferta >= hoy ? 1 : 2,                                         

                                     }).FirstOrDefault() : new CircularDto
                                     {
                                         PlazoDeOferta = x.PeticionDeOferta.PlazoDeOferta,
                                         Estado = x.PeticionDeOferta.PlazoDeOferta >= hoy ? "Abierto" : "Cerrado",
                                         EstadoColor = x.PeticionDeOferta.PlazoDeOferta >= hoy ? "Green" : "Red",
                                         Estado_Id = x.PeticionDeOferta.PlazoDeOferta >= hoy ? 1 : 2,
                                     },
                                    Usuarios = new List<PeticionDeOfertaUsarioDto> { new PeticionDeOfertaUsarioDto {
                                        Id = x.Id,
                                        PropuestaTecnicaAprobada = x.PropuestaTecnicaAprobada,
                                        RealizoVisita = x.RealizoVisita,
                                        UsuarioId = x.Usuario_Id,
                                        CUIT = x.Usuario.CUITRegistro,
                                        CircularSinLeer = x.Circulares.Any(a=>a.Leida != true)
                                    } },
                                    TieneVisitaObra = x.PeticionDeOferta.Solp.Pliego.TieneVisitaObra == null ? "No requiere visita" : "Requiere visita a coordinar",
                                    CotizacionEstadoDescripcion = cotizacion == null ? "Sin Cotizar" : cotizacion.CotizacionEstado.Descripcion,
                                    CotizacionEstado_Id = cotizacion == null ? 0 : cotizacion.CotizacionEstado.Id,
                                    ItemPorPagina = paginacion.ItemsPorPagina,
                                    Pagina = paginacion.Pagina,

                                };
                //var resultado = from x in contexto.Set<PeticionDeOferta>()
                //                join cotizacion in contexto.Set<Cotizacion>() on x.Id equals cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta_Id into peticionCotizacion
                //                from cotizacion in peticionCotizacion.DefaultIfEmpty()
                //                where (string.IsNullOrEmpty(NroSolp) || x.Solp.NroSolp.ToUpper().StartsWith(NroSolp.ToUpper()))
                //                select new PeticionDeOfertaDto
                //                {
                //                    Id = x.Id,
                //                    NroSolp = x.Solp.NroSolp,
                //                    NombreDeObra = x.Solp.Pliego == null ? "" : x.Solp.Pliego.NombreObra,
                //                    UsuarioCreador_Id = x.UsuarioCreador_Id,
                //                    UsuarioCreador = x.Usuario.Mail,                    
                //                    Circular = contexto.Set<CircularPeticionDeOfertaUsuario>().Any(cp => cp.PeticionDeOfertaUsuario.PeticionDeOferta_Id == x.Id && cp.Circular.PlazoDeOferta != null) ?  (from circularPO in contexto.Set<CircularPeticionDeOfertaUsuario>()
                //                                     where circularPO.PeticionDeOfertaUsuario.PeticionDeOferta_Id == x.Id
                //                                     orderby circularPO.Circular.PlazoDeOferta descending
                //                                      select new CircularDto { 
                //                                          PlazoDeOferta = circularPO.Circular.PlazoDeOferta,
                //                                          Estado = circularPO.Circular.PlazoDeOferta >= hoy ? "Abierto" : "Cerrado",
                //                                          EstadoColor = circularPO.Circular.PlazoDeOferta >= hoy ? "Green" : "Red",
                //                                          Estado_Id = circularPO.Circular.PlazoDeOferta >= hoy ? 1 : 2,

                //                                      }

                //                                     ).FirstOrDefault() : new CircularDto { 
                //                                         PlazoDeOferta = x.PlazoDeOferta,
                //                                         Estado = x.PlazoDeOferta >= hoy ? "Abierto" : "Cerrado",
                //                                         EstadoColor = x.PlazoDeOferta >= hoy ? "Green" : "Red",
                //                                         Estado_Id = x.PlazoDeOferta >= hoy ? 1 : 2,
                //                                     },

                //                    TieneVisitaObra = x.Solp.Pliego.TieneVisitaObra == null ? "No requiere visita" : "Requiere visita a coordinar",
                //                    CotizacionEstadoDescripcion = cotizacion == null ? "Sin Cotizar" : cotizacion.CotizacionEstado.Descripcion,
                //                    CotizacionEstado_Id = cotizacion == null ? 0 : cotizacion.CotizacionEstado.Id,
                //                    ItemPorPagina = paginacion.ItemsPorPagina,
                //                    Pagina = paginacion.Pagina,

                //                };

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
