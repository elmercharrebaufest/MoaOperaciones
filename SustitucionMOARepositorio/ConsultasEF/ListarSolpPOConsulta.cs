using Molinos.Scato.Repositorio;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio.Extensiones;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace SustitucionMOARepositorio.ConsultasEF
{
    public class ListarSolpPOConsulta : IConsultaPaginada<PeticionDeOfertaDto>
    {
        private readonly Paginacion Paginacion;
        private readonly string NroSolp;
        private readonly string CuitUsuario;
        public ListarSolpPOConsulta(Paginacion paginacion, string nroSolp, string cuitUsuario)
        {
            Paginacion = paginacion;
            NroSolp = nroSolp;
            CuitUsuario = cuitUsuario;
        }
        public ListaPaginada<PeticionDeOfertaDto> Ejecutar(DbContext contexto)
        {
            var hoy = DateTime.Now;
            var ayer = hoy.AddDays(-1);
            var fechas = new List<DateTime?>();
            try
            {
                ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
                var nroDeSolp = NroSolp.Trim();
                var resultado = from x in contexto.Set<PeticionDeOfertaUsuario>()
                                join peticionDeOferta in contexto.Set<PeticionDeOferta>() on x.PeticionDeOferta_Id equals peticionDeOferta.Id into peticion
                                from peticionDeOferta in peticion.DefaultIfEmpty()
                                join cotizacion in contexto.Set<Cotizacion>() on x.Id equals cotizacion.PeticionDeOfertaUsuario_Id into peticionCotizacion
                                from cotizacion in peticionCotizacion.DefaultIfEmpty()
                                where (string.IsNullOrEmpty(nroDeSolp) || x.PeticionDeOferta.Solp.NroSolp.ToUpper().StartsWith(nroDeSolp.ToUpper()))
                                && x.Usuario.CUITRegistro == CuitUsuario && x.PeticionDeOferta.RegistroInfo != true
                                select new PeticionDeOfertaDto
                                {
                                    Id = x.PeticionDeOferta.Id,
                                    NroSolp = x.PeticionDeOferta.Solp.NroSolp,
                                    NombreDeObra = x.PeticionDeOferta.Solp.Pliego == null ? "" : x.PeticionDeOferta.Solp.Pliego.NombreObra,
                                    UsuarioCreador_Id = x.PeticionDeOferta.UsuarioCreador_Id,
                                    UsuarioCreador = x.PeticionDeOferta.Usuario.Mail,
                                    CotizacionId = cotizacion != null ? cotizacion.Id : 0,
                                    PlazoDeOfertaCierre = peticionDeOferta.Cierres.OrderByDescending(p => p.Fecha).FirstOrDefault().Fecha,
                                    PlazoDeOfertaOriginal = x.PeticionDeOferta.PlazoDeOferta,
                                    FechaCircular = peticionDeOferta.Usuarios.Where(u => u.Usuario.CUITRegistro == CuitUsuario).GroupBy(x => x).SelectMany(x => x.Key.Circulares)
                                    .Where(x => x.Circular.RequiereCambioDeFechas == true && x.Circular.PlazoDeOferta.HasValue)
                                    .OrderByDescending(x => x.Circular.Id).FirstOrDefault().Circular.FechaCreacion,
                                    PlazoDeOfertaCircular = peticionDeOferta.Usuarios.Where(u => u.Usuario.CUITRegistro == CuitUsuario).GroupBy(x => x).SelectMany(x => x.Key.Circulares)
                                    .Where(x => x.Circular.RequiereCambioDeFechas == true && x.Circular.PlazoDeOferta.HasValue)
                                    .OrderByDescending(x => x.Circular.Id).FirstOrDefault().Circular.PlazoDeOferta,

                                    PlazoDeOferta = (!peticionDeOferta.Cierres.Any() && !peticionDeOferta.Usuarios
                                        .Where(u => u.Usuario.CUITRegistro == CuitUsuario).GroupBy(x => x).SelectMany(x => x.Key.Circulares)
                                    .Where(x => x.Circular.RequiereCambioDeFechas == true && x.Circular.PlazoDeOferta.HasValue).Any()) ?
                                    x.PeticionDeOferta.PlazoDeOferta :
                                                    peticionDeOferta.Cierres.OrderByDescending(p => p.Fecha).FirstOrDefault().Fecha == null ? peticionDeOferta.Usuarios.Where(u => u.Usuario.CUITRegistro == CuitUsuario).GroupBy(x => x).SelectMany(x => x.Key.Circulares)
                                    .Where(x => x.Circular.RequiereCambioDeFechas == true && x.Circular.PlazoDeOferta.HasValue)
                                    .OrderByDescending(x => x.Circular.Id).FirstOrDefault().Circular.PlazoDeOferta.Value :
                                                    peticionDeOferta.Usuarios.Where(u => u.Usuario.CUITRegistro == CuitUsuario).GroupBy(x => x).SelectMany(x => x.Key.Circulares)
                                    .Where(x => x.Circular.RequiereCambioDeFechas == true && x.Circular.PlazoDeOferta.HasValue)
                                    .OrderByDescending(x => x.Circular.Id).FirstOrDefault().Circular.FechaCreacion == null ? peticionDeOferta.Cierres.OrderByDescending(p => p.Fecha).FirstOrDefault().Fecha :
                                                    peticionDeOferta.Cierres.OrderByDescending(p => p.Fecha).FirstOrDefault().Fecha > peticionDeOferta.Usuarios.Where(u => u.Usuario.CUITRegistro == CuitUsuario).GroupBy(x => x).SelectMany(x => x.Key.Circulares)
                                    .Where(x => x.Circular.RequiereCambioDeFechas == true && x.Circular.PlazoDeOferta.HasValue)
                                    .OrderByDescending(x => x.Circular.Id).FirstOrDefault().Circular.FechaCreacion ?
                                                    peticionDeOferta.Cierres.OrderByDescending(p => p.Fecha).FirstOrDefault().Fecha : peticionDeOferta.Usuarios
                                                    .Where(u => u.Usuario.CUITRegistro == CuitUsuario).GroupBy(x => x).SelectMany(x => x.Key.Circulares)
                                    .Where(x => x.Circular.RequiereCambioDeFechas == true && x.Circular.PlazoDeOferta.HasValue)
                                    .OrderByDescending(x => x.Circular.Id).FirstOrDefault().Circular.PlazoDeOferta.Value,

                                    Estado = ((!peticionDeOferta.Cierres.Any() && !peticionDeOferta.Usuarios
                                        .Where(u => u.Usuario.CUITRegistro == CuitUsuario).GroupBy(x => x).SelectMany(x => x.Key.Circulares)
                                    .Where(x => x.Circular.RequiereCambioDeFechas == true && x.Circular.PlazoDeOferta.HasValue).Any()) ?
                                    x.PeticionDeOferta.PlazoDeOferta :
                                                    peticionDeOferta.Cierres.OrderByDescending(p => p.Fecha).FirstOrDefault().Fecha == null ? peticionDeOferta.Usuarios.Where(u => u.Usuario.CUITRegistro == CuitUsuario).GroupBy(x => x).SelectMany(x => x.Key.Circulares)
                                    .Where(x => x.Circular.RequiereCambioDeFechas == true && x.Circular.PlazoDeOferta.HasValue)
                                    .OrderByDescending(x => x.Circular.Id).FirstOrDefault().Circular.PlazoDeOferta.Value :
                                                    peticionDeOferta.Usuarios.Where(u => u.Usuario.CUITRegistro == CuitUsuario).GroupBy(x => x).SelectMany(x => x.Key.Circulares)
                                    .Where(x => x.Circular.RequiereCambioDeFechas == true && x.Circular.PlazoDeOferta.HasValue)
                                    .OrderByDescending(x => x.Circular.Id).FirstOrDefault().Circular.FechaCreacion == null ? peticionDeOferta.Cierres.OrderByDescending(p => p.Fecha).FirstOrDefault().Fecha :
                                                    peticionDeOferta.Cierres.OrderByDescending(p => p.Fecha).FirstOrDefault().Fecha > peticionDeOferta.Usuarios.Where(u => u.Usuario.CUITRegistro == CuitUsuario).GroupBy(x => x).SelectMany(x => x.Key.Circulares)
                                    .Where(x => x.Circular.RequiereCambioDeFechas == true && x.Circular.PlazoDeOferta.HasValue)
                                    .OrderByDescending(x => x.Circular.Id).FirstOrDefault().Circular.FechaCreacion ?
                                                    peticionDeOferta.Cierres.OrderByDescending(p => p.Fecha).FirstOrDefault().Fecha : peticionDeOferta.Usuarios
                                                    .Where(u => u.Usuario.CUITRegistro == CuitUsuario).GroupBy(x => x).SelectMany(x => x.Key.Circulares)
                                    .Where(x => x.Circular.RequiereCambioDeFechas == true && x.Circular.PlazoDeOferta.HasValue)
                                    .OrderByDescending(x => x.Circular.Id).FirstOrDefault().Circular.PlazoDeOferta.Value) >= hoy ? "Abierto" : "Cerrado",

                                    EstadoColor = ((!peticionDeOferta.Cierres.Any() && !peticionDeOferta.Usuarios
                                        .Where(u => u.Usuario.CUITRegistro == CuitUsuario).GroupBy(x => x).SelectMany(x => x.Key.Circulares)
                                    .Where(x => x.Circular.RequiereCambioDeFechas == true && x.Circular.PlazoDeOferta.HasValue).Any()) ?
                                    x.PeticionDeOferta.PlazoDeOferta :
                                                    peticionDeOferta.Cierres.OrderByDescending(p => p.Fecha).FirstOrDefault().Fecha == null ? peticionDeOferta.Usuarios.Where(u => u.Usuario.CUITRegistro == CuitUsuario).GroupBy(x => x).SelectMany(x => x.Key.Circulares)
                                    .Where(x => x.Circular.RequiereCambioDeFechas == true && x.Circular.PlazoDeOferta.HasValue)
                                    .OrderByDescending(x => x.Circular.Id).FirstOrDefault().Circular.PlazoDeOferta.Value :
                                                    peticionDeOferta.Usuarios.Where(u => u.Usuario.CUITRegistro == CuitUsuario).GroupBy(x => x).SelectMany(x => x.Key.Circulares)
                                    .Where(x => x.Circular.RequiereCambioDeFechas == true && x.Circular.PlazoDeOferta.HasValue)
                                    .OrderByDescending(x => x.Circular.Id).FirstOrDefault().Circular.FechaCreacion == null ? peticionDeOferta.Cierres.OrderByDescending(p => p.Fecha).FirstOrDefault().Fecha :
                                                    peticionDeOferta.Cierres.OrderByDescending(p => p.Fecha).FirstOrDefault().Fecha > peticionDeOferta.Usuarios.Where(u => u.Usuario.CUITRegistro == CuitUsuario).GroupBy(x => x).SelectMany(x => x.Key.Circulares)
                                    .Where(x => x.Circular.RequiereCambioDeFechas == true && x.Circular.PlazoDeOferta.HasValue)
                                    .OrderByDescending(x => x.Circular.Id).FirstOrDefault().Circular.FechaCreacion ?
                                                    peticionDeOferta.Cierres.OrderByDescending(p => p.Fecha).FirstOrDefault().Fecha : peticionDeOferta.Usuarios
                                                    .Where(u => u.Usuario.CUITRegistro == CuitUsuario).GroupBy(x => x).SelectMany(x => x.Key.Circulares)
                                    .Where(x => x.Circular.RequiereCambioDeFechas == true && x.Circular.PlazoDeOferta.HasValue)
                                    .OrderByDescending(x => x.Circular.Id).FirstOrDefault().Circular.PlazoDeOferta.Value) >= hoy ? "Green" : "Red",


                                    Estado_Id = ((!peticionDeOferta.Cierres.Any() && !peticionDeOferta.Usuarios
                                        .Where(u => u.Usuario.CUITRegistro == CuitUsuario).GroupBy(x => x).SelectMany(x => x.Key.Circulares)
                                    .Where(x => x.Circular.RequiereCambioDeFechas == true && x.Circular.PlazoDeOferta.HasValue).Any()) ?
                                    x.PeticionDeOferta.PlazoDeOferta :
                                                    peticionDeOferta.Cierres.OrderByDescending(p => p.Fecha).FirstOrDefault().Fecha == null ? peticionDeOferta.Usuarios.Where(u => u.Usuario.CUITRegistro == CuitUsuario).GroupBy(x => x).SelectMany(x => x.Key.Circulares)
                                    .Where(x => x.Circular.RequiereCambioDeFechas == true && x.Circular.PlazoDeOferta.HasValue)
                                    .OrderByDescending(x => x.Circular.Id).FirstOrDefault().Circular.PlazoDeOferta.Value :
                                                    peticionDeOferta.Usuarios.Where(u => u.Usuario.CUITRegistro == CuitUsuario).GroupBy(x => x).SelectMany(x => x.Key.Circulares)
                                    .Where(x => x.Circular.RequiereCambioDeFechas == true && x.Circular.PlazoDeOferta.HasValue)
                                    .OrderByDescending(x => x.Circular.Id).FirstOrDefault().Circular.FechaCreacion == null ? peticionDeOferta.Cierres.OrderByDescending(p => p.Fecha).FirstOrDefault().Fecha :
                                                    peticionDeOferta.Cierres.OrderByDescending(p => p.Fecha).FirstOrDefault().Fecha > peticionDeOferta.Usuarios.Where(u => u.Usuario.CUITRegistro == CuitUsuario).GroupBy(x => x).SelectMany(x => x.Key.Circulares)
                                    .Where(x => x.Circular.RequiereCambioDeFechas == true && x.Circular.PlazoDeOferta.HasValue)
                                    .OrderByDescending(x => x.Circular.Id).FirstOrDefault().Circular.FechaCreacion ?
                                                    peticionDeOferta.Cierres.OrderByDescending(p => p.Fecha).FirstOrDefault().Fecha : peticionDeOferta.Usuarios
                                                    .Where(u => u.Usuario.CUITRegistro == CuitUsuario).GroupBy(x => x).SelectMany(x => x.Key.Circulares)
                                    .Where(x => x.Circular.RequiereCambioDeFechas == true && x.Circular.PlazoDeOferta.HasValue)
                                    .OrderByDescending(x => x.Circular.Id).FirstOrDefault().Circular.PlazoDeOferta.Value) >= hoy ? 1 : 2,
                                    AdjuntoPliego = x.PeticionDeOferta.AdjuntoPliego,

                                    Usuarios = new List<PeticionDeOfertaUsarioDto> { new PeticionDeOfertaUsarioDto {
                                        Id = x.Id,
                                        PropuestaTecnicaAprobada = x.PropuestaTecnicaAprobada,
                                        RealizoVisita = x.RealizoVisita,
                                        UsuarioId = x.Usuario_Id,
                                        CUIT = x.Usuario.CUITRegistro,
                                        CircularSinLeer = x.Circulares.Any(a=>a.Leida != true),
                                        CircularesSinLeer = x.Circulares.Where(a=>a.Leida != true).Select(a=>a.Id),
                                        EstaHabilitado = x.Usuario.Habilitado
                                    } },
                                    CotizacionEstadoDescripcion = cotizacion == null ? "Sin Cotizar" : cotizacion.CotizacionEstado.Descripcion,
                                    CotizacionEstado_Id = cotizacion == null ? 0 : cotizacion.CotizacionEstado.Id,
                                    ItemPorPagina = Paginacion.ItemsPorPagina,
                                    Pagina = Paginacion.Pagina,
                                    TieneAdjudicacion = cotizacion != null && cotizacion.Adjudicaciones.Any(),

                                };

                var itemsTotales = resultado.Count();

                return resultado.OrdenarPaginarLista(Paginacion);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}