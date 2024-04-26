using Molinos.Scato.Repositorio;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAModel.Models.WSMapMOA.Pago.NoGranos;
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
        private readonly Paginacion Paginacion;
        private readonly List<string> Solps;
        private readonly DateTime? FechaDesde;
        private readonly DateTime? FechaHasta;
        private readonly bool Sap;
        private readonly bool Mantenimiento;
        private readonly bool Web;
        private readonly bool ReposicionAutomatica;
        private readonly List<int> Usuarios;
        private readonly List<int> Estados;
        private readonly List<int> Centros;
        private readonly List<int> GrupoDeCompras;
        private readonly int Usuario_Id;
        private readonly List<int> ClaseDocumento;
        private readonly List<string> TipoImputacion;
        private readonly List<int> ValorTipoImputacion;
        private readonly bool? ListarPendiente;
        private readonly bool ContratoMarco;

        public ListarSolpConsulta(Paginacion paginacion, List<string> solps, DateTime? desde, DateTime? hasta, bool sap, bool mantenimiento, bool web, bool repoAutomatica, bool listarPendiente, bool contratoMarco, List<int> usuarios, List<int> estados, List<int> centros, List<int> grupoDeCompras, int usuario_Id, List<int> claseDocumento = null, List<string> tipoImputacion = null, List<int> valorTipoImputacion = null)
        {
            Paginacion = paginacion;
            Solps = solps;
            FechaDesde = desde;
            FechaHasta = hasta.HasValue ? hasta.Value.AddDays(1) : hasta;
            Sap = sap;
            Mantenimiento = mantenimiento;
            Web = web;
            ReposicionAutomatica = repoAutomatica;
            ContratoMarco = contratoMarco;
            Usuarios = usuarios;
            Estados = estados;
            Centros = centros;
            GrupoDeCompras = grupoDeCompras;
            Usuario_Id = usuario_Id;
            ClaseDocumento = claseDocumento;
            TipoImputacion = tipoImputacion;
            ValorTipoImputacion = valorTipoImputacion;
            ListarPendiente = listarPendiente;
        }
        public ListaPaginada<SolpDto> Ejecutar(DbContext contexto)
        {
            var hoy = DateTime.Now;
            var ayer = hoy.AddDays(-1);

            try
            {
                ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
                Usuario usuario = (from u in contexto.Set<Usuario>()
                                   where u.Id == Usuario_Id
                                   select u).First();
                var rol = usuario.Roles.Any(r => r.Codigo == "COMPRADOR") ? "SOLP" : "COMPRADOR";
                var visualizarEditarOC = usuario.ObtenerPermisos().Contains("EDITAR OC");
                var sinSolps = !Solps.Any();
                var resultado = from x in contexto.Set<Solp>()
                                where (sinSolps || Solps.Contains(x.NroSolp)) &&
                                (!Usuarios.Any() || (x.UsuarioCreacion_Id != null && Usuarios.Contains((int)x.UsuarioCreacion_Id))) &&
                                (!Estados.Any() || (x.EstadoSolpSap_Id != null && Estados.Contains((int)x.EstadoSolpSap_Id))) &&
                                (!Centros.Any() || x.Posiciones.Any(c => Centros.Contains(c.Centro_Id))) &&
                                (!GrupoDeCompras.Any() || x.Posiciones.Any(gc => GrupoDeCompras.Contains((int)gc.GrupoCompras_Id))) &&
                                (Sap && x.TipoSolpSap == 3 || Mantenimiento && x.TipoSolpSap == 2 || ReposicionAutomatica && x.TipoSolpSap == 4 ||
                                (Web && (x.TipoSolpSap == null || x.TipoSolpSap == 1)) || (!Sap && !Mantenimiento && !Web && !ReposicionAutomatica)) &&
                                (FechaDesde == null || x.FechaCreacion >= FechaDesde.Value) && (FechaHasta == null || x.FechaCreacion <= FechaHasta.Value) &&
                                (!ContratoMarco || x.Posiciones.Any(p => !string.IsNullOrEmpty(p.NumeroContratoSuperior))) &&
                                (!ClaseDocumento.Any() || x.EstadoSolpSap_Id != null && ClaseDocumento.Contains((int)x.ClaseDocumento_Id)) &&
                                (!TipoImputacion.Any() || x.Posiciones.Any(c => TipoImputacion.Contains(c.TipoImputacion.Codigo))) &&
                                (!ValorTipoImputacion.Any() || x.Posiciones.Any(c => ValorTipoImputacion.Contains((int)c.ValorTipoImputacion_Id)) || x.Posiciones.Any(p => p.Subposiciones.Any(c => ValorTipoImputacion.Contains((int)c.TipoImputacion_Id))))
                                select new SolpDto
                                {
                                    VerEditarOC = visualizarEditarOC,
                                    UsuarioActual = new UsuarioDto { Mail = x.UsuarioCreacion != null ? x.UsuarioCreacion.Mail : "" },
                                    Id = x.Id,
                                    NroSolp = x.NroSolp,
                                    NombreDeObra = x.Pliego == null ? "" : x.Pliego.NombreObra,
                                    FechaCreacionFormateada = SqlFunctions.DateName("day", x.FechaCreacion) + "/" + SqlFunctions.DatePart("month", x.FechaCreacion) + "/" + SqlFunctions.DateName("year", x.FechaCreacion),
                                    FechaCreacion = x.FechaCreacion,
                                    TipoSolp = new TablaGeneralDto { Descripcion = x.TipoSolp != null ? x.TipoSolp.Descripcion : "", Codigo = x.TipoSolp != null ? x.TipoSolp.Codigo : "" },
                                    TipoSolpSap = x.TipoSolpSap,
                                    Adicional = x.Adicional,
                                    NroOrdenDeCompraAdicional = x.NroOrdenDeCompraAdicional,
                                    TrabajoYaHecho = x.TrabajoYaHecho,
                                    Urgencia = x.Urgencia,
                                    CondEspProveedorAsignado = x.CondEspProveedorAsignado,
                                    SolpConAdjuntos = x.Pliego.Archivos.Where(r => r.FileKey == FileKeys.AdjuntoCotizacionesSolp || r.FileKey == FileKeys.AdjuntoSolp).Any(),
                                    PosicionCompras = (from posicion in contexto.Set<SolpPosicion>()
                                                       where posicion.Solp_Id == x.Id
                                                       select new SolpPosicionDto()
                                                       {
                                                           GrupoComprasDescripcion = posicion.GrupoCompras != null ? posicion.GrupoCompras.Descripcion : "",
                                                           CentroComprasDescripcion = posicion.Centro != null ? posicion.Centro.Descripcion : "",
                                                           GrupoComprasId = posicion.GrupoCompras != null ? posicion.GrupoCompras.Id : (int?)null,
                                                           CentroId = posicion.Centro != null ? posicion.Centro.Id : (int?)null,
                                                           GrupoComprasCodigo = posicion.GrupoCompras != null ? posicion.GrupoCompras.Codigo : (string)null,
                                                           CentroCodigo = posicion.Centro != null ? posicion.Centro.Codigo : (string)null,
                                                           Cantidad = posicion.Cantidad,
                                                           TipoPosicion = new TablaGeneralDto { Descripcion = posicion.TipoPosicion.Descripcion, Codigo = posicion.TipoPosicion.Codigo },
                                                           NroSolp = posicion.Solp.NroSolp,
                                                           NumeroContratoSuperior = posicion.NumeroContratoSuperior,
                                                       }),
                                    ItemPorPagina = Paginacion.ItemsPorPagina,
                                    Pagina = Paginacion.Pagina,
                                    EstadoSolpSap = x.EstadoSolpSap_Id != null ? new TablaSapDto { Id = x.EstadoSolpSap_Id ?? 0, CodigoSap = x.EstadoSolpSap.CodigoSap, Descripcion = x.EstadoSolpSap.Descripcion } : new TablaSapDto { Id = 0, CodigoSap = "", Descripcion = "" },
                                    TodasLasPosicionesBorradas = x.Posiciones.All(p => p.Estado == false),
                                    VerPublicar = ((x.EstadoSolpSap.CodigoSap == "05" || x.EstadoSolpSap.CodigoSap == "02") && x.TrabajoYaHecho != true && x.Adicional != true)
                                                    || (!string.IsNullOrEmpty(x.NroSolp) && x.Posiciones.Any(p => p.TipoPosicion.Codigo == "SERVICIO") && x.Urgencia == true && x.Adicional != true),
                                    VerCircular = x.TrabajoYaHecho == null || x.TrabajoYaHecho == false,
                                    FechaLiberacionSapFormateada = x.FechaLiberacionSap == null ? "" : SqlFunctions.DateName("day", x.FechaLiberacionSap) + "/" + SqlFunctions.DatePart("month", x.FechaLiberacionSap) + "/" + SqlFunctions.DateName("year", x.FechaLiberacionSap),
                                    FechaLiberacionSap = x.FechaLiberacionSap,
                                    ChatSinLeer = x.ChatInternoCompras.Any(a => a.Leido == false && a.Usuario.Roles.Any(r => r.Codigo == rol)),
                                    PeticionesDeOferta = (from po in contexto.Set<PeticionDeOferta>()
                                                              //where po.Posiciones.FirstOrDefault().SolpPosicion.Solp_Id == x.Id && po.RegistroInfo != true
                                                          where po.Posiciones.Select(so => so.SolpPosicion.Solp_Id).Contains(x.Id) && po.RegistroInfo != true

                                                          select new PeticionDeOfertaDto()
                                                          {
                                                              Id = po.Id,
                                                              FechaCreacion = po.FechaCreacion,
                                                              UsuarioCreador_Id = po.UsuarioCreador_Id,
                                                              PlazoDeOfertaOriginal = po.PlazoDeOferta,
                                                              PlazoDeOfertaCircular = po.Usuarios.GroupBy(p => p).SelectMany(p => p.Key.Circulares)
                                                                .Where(p => p.Circular.RequiereCambioDeFechas == true && p.Circular.PlazoDeOferta.HasValue)
                                                                .OrderByDescending(p => p.Circular.Id).FirstOrDefault().Circular.PlazoDeOferta,
                                                              FechaCircular = po.Usuarios.GroupBy(p => p).SelectMany(p => p.Key.Circulares)
                                                                .Where(p => p.Circular.RequiereCambioDeFechas == true && p.Circular.PlazoDeOferta.HasValue)
                                                                .OrderByDescending(p => p.Circular.Id).FirstOrDefault().Circular.FechaCreacion,
                                                              PlazoDeOfertaCierre = po.Cierres.Any() ? po.Cierres.OrderByDescending(p => p.Fecha).FirstOrDefault().Fecha : (DateTime?)null,
                                                              RevisionFinalizada = po.RevisionTecnica != null && po.RevisionTecnica.Finalizada,
                                                              Observaciones = po.Observaciones,
                                                              RecotizacionEconomica = po.RevisionTecnica != null && po.RevisionTecnica.RecotizacionEconomica,
                                                              NrosSolp = po.Posiciones.Select(posi => posi.SolpPosicion.Solp.NroSolp)

                                                          })
                                };
                var pagina = Paginacion;
                if (ListarPendiente == true)
                {
                    pagina = new Paginacion("Id", DirOrden.Asc, 1, resultado.Count());
                }
                var result = resultado.OrdenarPaginarLista(pagina);
                return result;
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}