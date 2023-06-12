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
            var fechas = new List<DateTime?>();
            try
            {
                ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
                var resultado = from x in contexto.Set<PeticionDeOfertaUsuario>()
                                join peticionDeOferta in contexto.Set<PeticionDeOferta>() on x.PeticionDeOferta_Id equals peticionDeOferta.Id into peticion
                                from peticionDeOferta in peticion.DefaultIfEmpty()
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
                                    CotizacionId = cotizacion != null ? cotizacion.Id : 0,

                                    PlazoDeOferta = peticionDeOferta.Usuarios.Where(u=>u.Usuario_Id == usuario_id).GroupBy(x => x).SelectMany(x => x.Key.Circulares)
                                    .Any(x => x.Circular.RequiereCambioDeFechas == true && x.Circular.PlazoDeOferta.HasValue) ?
                                    peticionDeOferta.Usuarios.Where(u => u.Usuario_Id == usuario_id).GroupBy(x => x).SelectMany(x => x.Key.Circulares)
                                    .Where(x => x.Circular.RequiereCambioDeFechas == true && x.Circular.PlazoDeOferta.HasValue)
                                    .OrderByDescending(x => x.Circular.PlazoDeOferta).FirstOrDefault().Circular.PlazoDeOferta.Value :
                                     x.PeticionDeOferta.PlazoDeOferta,

                                    Estado = (peticionDeOferta.Usuarios.Where(u => u.Usuario_Id == usuario_id).GroupBy(x => x).SelectMany(x => x.Key.Circulares)
                                    .Any(x => x.Circular.RequiereCambioDeFechas == true && x.Circular.PlazoDeOferta.HasValue) ?
                                    peticionDeOferta.Usuarios.Where(u => u.Usuario_Id == usuario_id).GroupBy(x => x).SelectMany(x => x.Key.Circulares)
                                    .Where(x => x.Circular.RequiereCambioDeFechas == true && x.Circular.PlazoDeOferta.HasValue)
                                    .OrderByDescending(x => x.Circular.PlazoDeOferta).FirstOrDefault().Circular.PlazoDeOferta.Value :
                                     x.PeticionDeOferta.PlazoDeOferta) >= hoy ? "Abierto" : "Cerrado",

                                    EstadoColor = (peticionDeOferta.Usuarios.Where(u => u.Usuario_Id == usuario_id).GroupBy(x => x).SelectMany(x => x.Key.Circulares)
                                    .Any(x => x.Circular.RequiereCambioDeFechas == true && x.Circular.PlazoDeOferta.HasValue) ?
                                    peticionDeOferta.Usuarios.Where(u => u.Usuario_Id == usuario_id).GroupBy(x => x).SelectMany(x => x.Key.Circulares)
                                    .Where(x => x.Circular.RequiereCambioDeFechas == true && x.Circular.PlazoDeOferta.HasValue)
                                    .OrderByDescending(x => x.Circular.PlazoDeOferta).FirstOrDefault().Circular.PlazoDeOferta.Value :
                                     x.PeticionDeOferta.PlazoDeOferta) >= hoy ? "Green" : "Red",

                                    Estado_Id = (peticionDeOferta.Usuarios.Where(u => u.Usuario_Id == usuario_id).GroupBy(x => x).SelectMany(x => x.Key.Circulares)
                                    .Any(x => x.Circular.RequiereCambioDeFechas == true && x.Circular.PlazoDeOferta.HasValue) ?
                                    peticionDeOferta.Usuarios.Where(u => u.Usuario_Id == usuario_id).GroupBy(x => x).SelectMany(x => x.Key.Circulares)
                                    .Where(x => x.Circular.RequiereCambioDeFechas == true && x.Circular.PlazoDeOferta.HasValue)
                                    .OrderByDescending(x => x.Circular.PlazoDeOferta).FirstOrDefault().Circular.PlazoDeOferta.Value :
                                     x.PeticionDeOferta.PlazoDeOferta) >= hoy ? 1 : 2,

                                    Usuarios = new List<PeticionDeOfertaUsarioDto> { new PeticionDeOfertaUsarioDto {
                                        Id = x.Id,
                                        PropuestaTecnicaAprobada = x.PropuestaTecnicaAprobada,
                                        RealizoVisita = x.RealizoVisita,
                                        UsuarioId = x.Usuario_Id,
                                        CUIT = x.Usuario.CUITRegistro,
                                        CircularSinLeer = x.Circulares.Any(a=>a.Leida != true),
                                        CircularesSinLeer = x.Circulares.Where(a=>a.Leida != true).Select(a=>a.Id),
                                    } },
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
