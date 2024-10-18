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
            try
            {
                ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
                var resultado = from peticionDeOfertaUsuario in contexto.Set<PeticionDeOfertaUsuario>()
                                join peticionDeOferta in contexto.Set<PeticionDeOferta>() on peticionDeOfertaUsuario.PeticionDeOferta_Id equals peticionDeOferta.Id into peticion
                                from peticionDeOferta in peticion.DefaultIfEmpty()
                                join cotizacion in contexto.Set<Cotizacion>() on peticionDeOfertaUsuario.Id equals cotizacion.PeticionDeOfertaUsuario_Id into peticionCotizacion
                                from cotizacion in peticionCotizacion.DefaultIfEmpty()
                                where (string.IsNullOrEmpty(NroSolp) || peticionDeOfertaUsuario.PeticionDeOferta.Posiciones.Select(posi => posi.SolpPosicion.Solp.NroSolp).Contains(NroSolp))
                                && (string.IsNullOrEmpty(NroPo) || peticionDeOfertaUsuario.PeticionDeOferta.Id.ToString().StartsWith(NroPo))
                                && (!NombrePedido.Any() || NombrePedido.All(p => peticionDeOfertaUsuario.PeticionDeOferta.Posiciones.FirstOrDefault().SolpPosicion.Solp.Pliego.NombreObra.ToUpper().Contains(p.ToUpper())))
                                && peticionDeOfertaUsuario.Usuario.CUITRegistro == CuitUsuario && peticionDeOfertaUsuario.PeticionDeOferta.RegistroInfo != true &&
                                (EstadoCotizacion == null || EstadoCotizacion == 0 && cotizacion == null || peticionDeOfertaUsuario.Cotizaciones.Any(c => c.CotizacionEstado_Id == EstadoCotizacion))
                                && peticionDeOfertaUsuario.PeticionDeOferta.Posiciones.Any(p => p.SolpPosicion.Estado == true) && peticionDeOfertaUsuario.PeticionDeOferta.Posiciones.FirstOrDefault().SolpPosicion.Solp.TrabajoYaHecho != true
                                select new PeticionDeOfertaDto
                                {
                                    Id = peticionDeOfertaUsuario.PeticionDeOferta.Id,
                                    NrosSolp = peticionDeOfertaUsuario.PeticionDeOferta.Posiciones.Select(posi => posi.SolpPosicion.Solp.NroSolp),
                                    NombreDeObra = peticionDeOfertaUsuario.PeticionDeOferta.Posiciones.FirstOrDefault().SolpPosicion.Solp.Pliego == null ? "" :
                                    peticionDeOfertaUsuario.PeticionDeOferta.Posiciones.FirstOrDefault().SolpPosicion.Solp.Pliego.NombreObra,
                                    UsuarioCreador_Id = peticionDeOfertaUsuario.PeticionDeOferta.UsuarioCreador_Id,
                                    UsuarioCreador = peticionDeOfertaUsuario.PeticionDeOferta.Usuario.Mail,
                                    CotizacionId = cotizacion != null ? cotizacion.Id : 0,

                                    PlazoDeOfertaCierre = peticionDeOferta
                                        .Cierres
                                        .OrderByDescending(p => p.Fecha)
                                        .FirstOrDefault()
                                        .Fecha,

                                    PlazoDeOfertaOriginal = peticionDeOfertaUsuario
                                        .PeticionDeOferta
                                        .PlazoDeOferta,

                                    FechaCircular = peticionDeOferta
                                        .Usuarios
                                        .Where(u => u.Usuario.CUITRegistro == CuitUsuario)
                                        .GroupBy(x => x)
                                        .SelectMany(x => x.Key.Circulares)
                                        .Where(x => x.Circular.RequiereCambioDeFechas == true && x.Circular.PlazoDeOferta.HasValue)
                                        .OrderByDescending(x => x.Circular.Id)
                                        .FirstOrDefault()
                                        .Circular
                                        .FechaCreacion,

                                    PlazoDeOfertaCircular = peticionDeOferta
                                        .Usuarios
                                        .Where(u => u.Usuario.CUITRegistro == CuitUsuario)
                                        .GroupBy(x => x)
                                        .SelectMany(x => x.Key.Circulares)
                                        .Where(x => x.Circular.RequiereCambioDeFechas == true && x.Circular.PlazoDeOferta.HasValue)
                                        .OrderByDescending(x => x.Circular.Id)
                                        .FirstOrDefault()
                                        .Circular
                                        .PlazoDeOferta,

                                    // PlazoDeOferta se resuelve internamente desde el DTO PeticionDeOfertaDto usando PlazoDeOfertaCierre, PlazoDeOfertaOriginal, FechaCircular y PlazoDeOfertaCircular

                                    // Estado, EstadoColor y Estado_Id se resuelven internamente desde el DTO PeticionDeOfertaDto usando valores de PlazoDeOferta

                                    AdjuntoPliego = peticionDeOfertaUsuario.PeticionDeOferta.AdjuntoPliego,

                                    Usuarios = new List<PeticionDeOfertaUsarioDto> { new PeticionDeOfertaUsarioDto {
                                        Id = peticionDeOfertaUsuario.Id,
                                        PropuestaTecnicaAprobada = peticionDeOfertaUsuario.PropuestaTecnicaAprobada,
                                        RealizoVisita = peticionDeOfertaUsuario.RealizoVisita,
                                        UsuarioId = peticionDeOfertaUsuario.Usuario_Id,
                                        CUIT = peticionDeOfertaUsuario.Usuario.CUITRegistro,
                                        CircularSinLeer = peticionDeOfertaUsuario.Circulares.Any(a=>a.Leida != true),
                                        CircularesSinLeer = peticionDeOfertaUsuario.Circulares.Where(a=>a.Leida != true).Select(a=>a.Id),
                                        EstaHabilitado = peticionDeOfertaUsuario.Usuario.Habilitado
                                    } },
                                    CotizacionEstadoDescripcion = cotizacion == null ? "Sin Cotizar" : cotizacion.CotizacionEstado.Descripcion,
                                    CotizacionEstado_Id = cotizacion == null ? 0 : cotizacion.CotizacionEstado.Id,
                                    ItemPorPagina = Paginacion.ItemsPorPagina,
                                    Pagina = Paginacion.Pagina,
                                    TieneAdjudicacion = cotizacion != null && cotizacion.Adjudicaciones.Any(),
                                    VerCotizar = peticionDeOfertaUsuario.PeticionDeOferta.Posiciones.All(posi => posi.SolpPosicion.Solp.EstadoSolpSap.CodigoSap == "05"),
                                    ChatSinLeer = peticionDeOfertaUsuario.ChatExterno.Any(a => a.Leido == false && a.Usuario.Roles.Any(r => r.Codigo == "SOLP"))
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