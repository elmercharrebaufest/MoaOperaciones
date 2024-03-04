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
        private readonly string NroPo;
        private readonly string[] NombrePedido;
        private readonly string CuitUsuario;
        private readonly int? EstadoCotizacion;
        private readonly int? EstadoLicitacion;
        private readonly DateTime? FechaDesde;
        private readonly DateTime? FechaHasta;

        public ListarSolpPOConsulta(Paginacion paginacion, string nroSolp, string nroPo, string[] nombrePedido, string cuitUsuario, int? estadoCotizacion, int? estadoLicitacion, DateTime? desde, DateTime? hasta)
        {
            Paginacion = paginacion;
            NroSolp = nroSolp.Trim();
            NroPo = nroPo.Trim();
            NombrePedido = nombrePedido;
            CuitUsuario = cuitUsuario;
            EstadoCotizacion = estadoCotizacion;
            EstadoLicitacion = estadoLicitacion;
            FechaDesde = desde;
            FechaHasta = hasta.HasValue ? hasta.Value.AddDays(1) : hasta;
        }
        public ListaPaginada<PeticionDeOfertaDto> Ejecutar(DbContext contexto)
        {
            var hoy = DateTime.Now;
            var ayer = hoy.AddDays(-1);
            try
            {
                ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
                var resultado = from x in contexto.Set<PeticionDeOfertaUsuario>()
                                join peticionDeOferta in contexto.Set<PeticionDeOferta>() on x.PeticionDeOferta_Id equals peticionDeOferta.Id into peticion
                                from peticionDeOferta in peticion.DefaultIfEmpty()
                                join cotizacion in contexto.Set<Cotizacion>() on x.Id equals cotizacion.PeticionDeOfertaUsuario_Id into peticionCotizacion
                                from cotizacion in peticionCotizacion.DefaultIfEmpty()
                                where (string.IsNullOrEmpty(NroSolp) || x.PeticionDeOferta.Posiciones.Select(posi => posi.SolpPosicion.Solp.NroSolp).Contains(NroSolp))
                                && (string.IsNullOrEmpty(NroPo) || x.PeticionDeOferta.Id.ToString().StartsWith(NroPo))
                                && (!NombrePedido.Any() || NombrePedido.All(p => x.PeticionDeOferta.Posiciones.FirstOrDefault().SolpPosicion.Solp.Pliego.NombreObra.ToUpper().Contains(p.ToUpper())))
                                && x.Usuario.CUITRegistro == CuitUsuario && x.PeticionDeOferta.RegistroInfo != true &&
                                (EstadoCotizacion == null || EstadoCotizacion == 0 && cotizacion == null || x.Cotizaciones.Any(c => c.CotizacionEstado_Id == EstadoCotizacion))
                                && x.PeticionDeOferta.Posiciones.Any(p => p.SolpPosicion.Estado == true) && x.PeticionDeOferta.Posiciones.FirstOrDefault().SolpPosicion.Solp.TrabajoYaHecho != true
                                select new PeticionDeOfertaDto
                                {
                                    Id = x.PeticionDeOferta.Id,
                                    NrosSolp = x.PeticionDeOferta.Posiciones.Select(posi => posi.SolpPosicion.Solp.NroSolp),
                                    NombreDeObra = x.PeticionDeOferta.Posiciones.FirstOrDefault().SolpPosicion.Solp.Pliego == null ? "" : 
                                    x.PeticionDeOferta.Posiciones.FirstOrDefault().SolpPosicion.Solp.Pliego.NombreObra,
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
                if (FechaDesde.HasValue || FechaHasta.HasValue || EstadoLicitacion.HasValue)
                {
                    resultado = resultado.Where(po =>
                    (!FechaDesde.HasValue || po.PlazoDeOferta >= FechaDesde.Value) &&
                    (!FechaHasta.HasValue || po.PlazoDeOferta <= FechaHasta.Value) &&
                    (!EstadoLicitacion.HasValue || po.Estado_Id == EstadoLicitacion.Value));
                }
                return resultado.OrdenarPaginarLista(Paginacion);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}