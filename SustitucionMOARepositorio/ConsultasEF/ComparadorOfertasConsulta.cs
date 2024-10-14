using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Linq;

namespace SustitucionMOARepositorio.ConsultasEF
{
    public class ComparadorOfertasConsulta : IConsultaEscalar<PeticionDeOfertaDto>
    {
        private readonly int PeticionOferta_Id;
        public ComparadorOfertasConsulta(int PeticionOferta_Id)
        {
            this.PeticionOferta_Id = PeticionOferta_Id;
        }
        public PeticionDeOfertaDto Ejecutar(DbContext contexto)
        {
            var hoy = DateTime.Now;
            var ayer = hoy.AddDays(-1);
            try
            {
                ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
                var resultado = from po in contexto.Set<PeticionDeOferta>()
                                    //join adjudicacion in contexto.Set<Adjudicacion>() on po.Solp.Id equals adjudicacion.Solp_Id into adjudicacionCotizacion
                                    //from adjudicacion in adjudicacionCotizacion.DefaultIfEmpty()
                                    //join adjudicacionPosicion in contexto.Set<AdjudicacionPosicion>() on adjudicacion.Id equals adjudicacionPosicion.Adjudicacion_Id into adjudicacionCotizacionPosicion
                                    //from adjudicacionPosicion in adjudicacionCotizacionPosicion.DefaultIfEmpty()
                                where po.Id == PeticionOferta_Id
                                select new PeticionDeOfertaDto
                                {
                                    Id = po.Id,
                                    SolpDto = new SolpDto
                                    {
                                        Urgencia = po.Posiciones.FirstOrDefault().SolpPosicion.Solp.Urgencia,
                                        TrabajoYaHecho = po.Posiciones.FirstOrDefault().SolpPosicion.Solp.TrabajoYaHecho,
                                        Adicional = po.Posiciones.FirstOrDefault().SolpPosicion.Solp.Adicional,
                                        CondEspProveedorAsignado = po.Posiciones.FirstOrDefault().SolpPosicion.Solp.CondEspProveedorAsignado,
                                        ObservacionesCotizacionLista = 
                                        po.Posiciones.Select(p => p.SolpPosicion.Solp.Pliego.ObservacionesCotizacionCondEsp).Distinct()
                                    },
                                    FechaCreacion = po.FechaCreacion,
                                    FechaCreacionFormateada = SqlFunctions.DateName("day", po.FechaCreacion) + "/" + SqlFunctions.DatePart("month", po.FechaCreacion) + "/" + SqlFunctions.DateName("year", po.FechaCreacion),
                                    UsuarioCreador_Id = po.UsuarioCreador_Id,
                                    // PlazoDeOferta se resuelve internamente desde el DTO PeticionDeOfertaDto usando PlazoDeOfertaCierre, PlazoDeOfertaOriginal, FechaCircular y PlazoDeOfertaCircular
                                    Observaciones = po.Observaciones,
                                    TienePosicionesEliminadas = po.Posiciones.Any(x => x.SolpPosicion.Estado != true),
                                    FechaCreacionSolp = po.Posiciones.Select(x => x.SolpPosicion.Solp).Select(solp => solp.FechaCreacion).OrderBy(fc => fc).FirstOrDefault(),
                                    FechaCreacionFormateadaSolp = SqlFunctions.DateName("day", po.Posiciones.Select(x => x.SolpPosicion.Solp).Select(solp => solp.FechaCreacion).OrderBy(fc => fc).FirstOrDefault()) + "/"
                                    + SqlFunctions.DatePart("month", po.Posiciones.Select(x => x.SolpPosicion.Solp).Select(solp => solp.FechaCreacion).OrderBy(fc => fc).FirstOrDefault()) + "/"
                                    + SqlFunctions.DateName("year", po.Posiciones.Select(x => x.SolpPosicion.Solp).Select(solp => solp.FechaCreacion).OrderBy(fc => fc).FirstOrDefault()),
                                    TipoPosicionCodigo = po.Posiciones.FirstOrDefault().SolpPosicion.TipoPosicion.Codigo,
                                    NroSolp = po.Posiciones.FirstOrDefault().SolpPosicion.Solp.NroSolp,
                                    Adicional = po.Posiciones.FirstOrDefault().SolpPosicion.Solp.Adicional,
                                    Urgencia = po.Posiciones.FirstOrDefault().SolpPosicion.Solp.Urgencia,
                                    TrabajoHecho = po.Posiciones.FirstOrDefault().SolpPosicion.Solp.TrabajoYaHecho,
                                    NroOrdenDeCompraAdicional = po.Posiciones.FirstOrDefault().SolpPosicion.Solp.NroOrdenDeCompraAdicional,
                                    EstaLiberado = po.Posiciones.Select(x => x.SolpPosicion.Solp).All(solp => solp.EstadoSolpSap.CodigoSap == "05" || solp.EstadoSolpSap.CodigoSap == "02"),
                                    RevisionFinalizada = po.RevisionTecnica == null ? false : po.RevisionTecnica.Finalizada,
                                    PlazoDeOfertaCierre = po.Cierres.Any() ? po.Cierres.OrderByDescending(p => p.Fecha).FirstOrDefault().Fecha : (DateTime?)null,
                                    NrosSolp = po.Posiciones.Select(posi => posi.SolpPosicion.Solp.NroSolp).Distinct(),
                                    PeticionDeOfertaPosicion = (from pop in contexto.Set<PeticionDeOfertaSolpPosicion>()
                                                                where po.Id == pop.PeticionDeOferta_Id && pop.SolpPosicion.EsConcluido == true
                                                                select new PeticionDeOfertaSolpPosicionDto()
                                                                {
                                                                    Id = pop.Id,
                                                                    PeticionDeOferta_Id = pop.PeticionDeOferta_Id,
                                                                    SolpPosicion_Id = pop.SolpPosicion_Id,
                                                                    EstaEliminado = pop.SolpPosicion.Estado != true,
                                                                    SolpId = pop.SolpPosicion.Solp_Id,
                                                                    Posicion = new SolpPosicionDto
                                                                    {
                                                                        Id = pop.SolpPosicion.Id,
                                                                        Indice = pop.SolpPosicion.Indice,
                                                                        FechaEntregaServicio = pop.SolpPosicion.FechaEntregaServicio != null ? pop.SolpPosicion.FechaEntregaServicio : null,
                                                                        CodigoMaterialSap = new MaterialSolpDto
                                                                        {
                                                                            Descripcion = pop.SolpPosicion.MaterialSolp.Descripcion,
                                                                            Codigo = pop.SolpPosicion.MaterialSolp.Codigo,
                                                                        },
                                                                        Codigo = pop.SolpPosicion.Codigo,
                                                                        Tarea = pop.SolpPosicion.Tarea,
                                                                        TextoSuministro = pop.SolpPosicion.TextoSuministro,
                                                                        Cantidad = pop.SolpPosicion.Cantidad,
                                                                        Solp_Id = pop.SolpPosicion.Solp_Id,
                                                                        NroSolp = pop.SolpPosicion.Solp.NroSolp,
                                                                        //CantidadPendiente = pop.SolpPosicion.Cantidad - (adjudicacion != null ? adjudicacion.Posiciones.Where(posicion => posicion.SolpPosicion_Id == pop.SolpPosicion_Id).FirstOrDefault().Cantidad : 0),
                                                                        //CantidadAdjudicacion = pop.SolpPosicion.Cantidad - (adjudicacion != null ? adjudicacion.Posiciones.Where(posicion => posicion.SolpPosicion_Id == pop.SolpPosicion_Id).FirstOrDefault().Cantidad : 0),
                                                                        SolpTipo = po.Posiciones.FirstOrDefault().SolpPosicion.TipoPosicion.Codigo,
                                                                        MonedaSolpDescripcion = pop.SolpPosicion.Moneda.Codigo,
                                                                        MonedaId = pop.SolpPosicion.Moneda_Id,
                                                                        Unidad = new TablaSapDto
                                                                        {
                                                                            Descripcion = pop.SolpPosicion.Unidad.Descripcion,
                                                                            CodigoSap = pop.SolpPosicion.Unidad.CodigoSap
                                                                        },
                                                                        Centro = new TablaSapDto
                                                                        {
                                                                            Descripcion = pop.SolpPosicion.Centro.Descripcion,
                                                                            CodigoSap = pop.SolpPosicion.Centro.CodigoSap
                                                                        },
                                                                        CentroId = pop.SolpPosicion.Centro_Id,
                                                                        GrupoCompras = new TablaSapDto
                                                                        {
                                                                            Descripcion = pop.SolpPosicion.GrupoCompras.Descripcion,
                                                                            CodigoSap = pop.SolpPosicion.GrupoCompras.CodigoSap,
                                                                        },
                                                                        Subposiciones = pop.SolpPosicion.Subposiciones.Select(s => new SolpSubposicionDto
                                                                        {
                                                                            Id = s.Id,
                                                                            CodigoServicioSapId = s.ServicioSolp_Id,
                                                                            ServicioSolpCodigo = s.ServicioSolp == null ? 0 : s.ServicioSolp.CodigoSap,
                                                                            Tarea = s.Tarea,
                                                                            Cantidad = s.Cantidad,
                                                                            Unidad = new TablaSapDto
                                                                            {
                                                                                Descripcion = s.Unidad.Descripcion,
                                                                                CodigoSap = s.Unidad.Descripcion
                                                                            },
                                                                            Numero = s.Numero

                                                                        }).OrderBy(s => s.Numero).ToList()
                                                                    },
                                                                }).OrderBy(x => x.Posicion.Indice),
                                    Usuarios = (from u in contexto.Set<PeticionDeOfertaUsuario>()
                                                join cotizacion in contexto.Set<Cotizacion>() on u.Id equals cotizacion.PeticionDeOfertaUsuario_Id into peticionCotizacion
                                                from cotizacion in peticionCotizacion.DefaultIfEmpty()
                                                where po.Id == u.PeticionDeOferta_Id
                                                select new PeticionDeOfertaUsarioDto()
                                                {
                                                    Id = u.Id,
                                                    VisibleSolicitante = u.VisibleSolicitante == true ? true : false,
                                                    EstaHabilitado = u.Usuario.Habilitado,
                                                    CodigoProveedor = u.Usuario.Proveedores.Where(p => p.CUIT == u.Usuario.CUITRegistro && u.Usuario.TipoUsuario.Id == p.TipoProveedor.Id).FirstOrDefault() != null ?
                                                        u.Usuario.Proveedores.Where(p => p.CUIT == u.Usuario.CUITRegistro && u.Usuario.TipoUsuario.Id == p.TipoProveedor.Id).FirstOrDefault().CodigoProveedor : "",
                                                    UsuarioId = u.Usuario_Id,
                                                    RazonSocial = u.Usuario.Proveedores.Where(p => p.CUIT == u.Usuario.CUITRegistro && u.Usuario.TipoUsuario.Id == p.TipoProveedor.Id).FirstOrDefault() != null ?
                                                        u.Usuario.Proveedores.Where(p => p.CUIT == u.Usuario.CUITRegistro && u.Usuario.TipoUsuario.Id == p.TipoProveedor.Id).FirstOrDefault().RazonSocial : u.Usuario.CUITRegistro,
                                                    CUIT = u.Usuario.Proveedores.Where(p => p.CUIT == u.Usuario.CUITRegistro && u.Usuario.TipoUsuario.Id == p.TipoProveedor.Id).FirstOrDefault() != null ?
                                                        u.Usuario.Proveedores.Where(p => p.CUIT == u.Usuario.CUITRegistro && u.Usuario.TipoUsuario.Id == p.TipoProveedor.Id).FirstOrDefault().CUIT : u.Usuario.CUITRegistro,
                                                    Mail = u.Usuario.Proveedores.Where(p => p.CUIT == u.Usuario.CUITRegistro && u.Usuario.TipoUsuario.Id == p.TipoProveedor.Id).FirstOrDefault() != null ?
                                                        u.Usuario.Proveedores.Where(p => p.CUIT == u.Usuario.CUITRegistro && u.Usuario.TipoUsuario.Id == p.TipoProveedor.Id).FirstOrDefault().Mail : u.Usuario.CUITRegistro,
                                                    PropuestaTecnicaAprobada = u.PropuestaTecnicaAprobada,
                                                    RealizoVisita = u.RealizoVisita,
                                                    THCategoria = po.Posiciones.FirstOrDefault().SolpPosicion.Solp.THProveedorDirecto == true ? "Proveedor directo" : (po.Posiciones.FirstOrDefault().SolpPosicion.Solp.THAjustePolinomica == true ? "Ajuste polinómica" : "Servicio permanente"),
                                                    EstadoVisita = u.RealizoVisita == true ? "Realizada" : po.Posiciones.FirstOrDefault().SolpPosicion.Solp.TrabajoYaHecho == true || (po.Posiciones.Select(posi => posi.SolpPosicion.Solp.Pliego).All(pliego => pliego.TieneVisitaObraMasiva != true) && po.Posiciones.Select(posi => posi.SolpPosicion.Solp.Pliego).All(pliego => pliego.TieneVisitaObra != true)) ? "No requerida" : "Sin realizar",
                                                    EstadoVisitaColor = u.RealizoVisita == true ? "Green" : po.Posiciones.FirstOrDefault().SolpPosicion.Solp.TrabajoYaHecho == true || (po.Posiciones.Select(posi => posi.SolpPosicion.Solp.Pliego).All(pliego => pliego.TieneVisitaObraMasiva != true) && po.Posiciones.Select(posi => posi.SolpPosicion.Solp.Pliego).All(pliego => pliego.TieneVisitaObra != true)) ? "Green" : "Red",
                                                    EstadoPropuestaTecnica = u.PropuestaTecnicaAprobada == null && po.Posiciones.FirstOrDefault().SolpPosicion.Solp.TrabajoYaHecho != true ? "Sin analizar" : po.Posiciones.FirstOrDefault().SolpPosicion.Solp.TrabajoYaHecho == true ? "Trabajo ya hecho" : (u.PropuestaTecnicaAprobada == true && po.Posiciones.FirstOrDefault().SolpPosicion.Solp.TrabajoYaHecho != true ? "Aprobada" : "Rechazada"),
                                                    ObservacionNoCumple = u.ObservacionNoCumple,
                                                    EstadoPropuestaTecnicaColor = u.PropuestaTecnicaAprobada == null && po.Posiciones.FirstOrDefault().SolpPosicion.Solp.TrabajoYaHecho != true ? "Orange" : po.Posiciones.FirstOrDefault().SolpPosicion.Solp.TrabajoYaHecho == true ? "Green" : (u.PropuestaTecnicaAprobada == true ? "Green" : "Red"),

                                                    //PlazoDeOferta = u.Circulares.Any(circu => circu.Circular.RequiereCambioDeFechas == true && circu.Circular.PlazoDeOferta.HasValue) ?
                                                    //u.Circulares.Where(circu => circu.Circular.RequiereCambioDeFechas == true && circu.Circular.PlazoDeOferta.HasValue)
                                                    //                .OrderByDescending(circu => circu.Circular.PlazoDeOferta).FirstOrDefault().Circular.PlazoDeOferta.Value :
                                                    //                 po.PlazoDeOferta,

                                                    PlazoDeOfertaOriginal = po.PlazoDeOferta,
                                                    PlazoDeOfertaCircular = u.Circulares
                                                                .Where(p => p.Circular.RequiereCambioDeFechas == true && p.Circular.PlazoDeOferta.HasValue)
                                                                .OrderByDescending(p => p.Circular.Id).FirstOrDefault().Circular.PlazoDeOferta,
                                                    FechaCircular = u.Circulares
                                                                .Where(p => p.Circular.RequiereCambioDeFechas == true && p.Circular.PlazoDeOferta.HasValue)
                                                                .OrderByDescending(p => p.Circular.Id).FirstOrDefault().Circular.FechaCreacion,
                                                    PlazoDeOfertaCierre = po.Cierres.Any() ? po.Cierres.OrderByDescending(p => p.Fecha).FirstOrDefault().Fecha : (DateTime?)null,
                                                    CotizacionEstado = cotizacion.CotizacionEstado.Descripcion,
                                                    Cotizacion = cotizacion != null ? new CotizacionDto()
                                                    {
                                                        Id = cotizacion.Id,
                                                        PeticionDeOfertaUsuario_Id = cotizacion.PeticionDeOfertaUsuario_Id,
                                                        FechaCreacion = u.Cotizaciones.FirstOrDefault().FechaCreacion,
                                                        FechaCreacionFormateada = SqlFunctions.DateName("day", u.Cotizaciones.FirstOrDefault().FechaCreacion) + "/" + SqlFunctions.DatePart("month", u.Cotizaciones.FirstOrDefault().FechaCreacion) + "/" + SqlFunctions.DateName("year", u.Cotizaciones.FirstOrDefault().FechaCreacion),
                                                        UsuarioCreador_Id = u.Cotizaciones.FirstOrDefault().UsuarioCreador_Id,
                                                        CotizacionEstadoDescripcion = cotizacion.CotizacionEstado.Descripcion,
                                                        CotizacionEstado_Id = cotizacion.CotizacionEstado_Id,
                                                        RespetaMaterialesDescripcion = u.Cotizaciones.FirstOrDefault().RespetaMateriales == true ? "Respeta" : "No Respeta",
                                                        RespetaMateriales = u.Cotizaciones.FirstOrDefault().RespetaMateriales,
                                                        RespetaServicios = u.Cotizaciones.FirstOrDefault().RespetaServicios,
                                                        ObservacionEconomica = u.Cotizaciones.FirstOrDefault().ObservacionEconomica,
                                                        ObservacionTecnica = u.Cotizaciones.FirstOrDefault().ObservacionTecnica,
                                                        TotalGlobal = 0,
                                                        TotalPesos = 0,
                                                        TotalGlobalSubPos = 0,
                                                        RespetaMaterialesColor = u.Cotizaciones.FirstOrDefault().RespetaMateriales == true ? "Green" : "Red",
                                                        PorcentajeDeHoras = cotizacion.PorcentajeDeHoras ?? 0,
                                                        CotizacionesHoras = cotizacion.CotizacionesHoras.Select(ch => new CotizacionHorasDto
                                                        {
                                                            CantidadPersonas = ch.CantidadPersonas,
                                                            Categoria = ch.Categoria,
                                                            Cotizacion_Id = ch.Cotizacion_Id,
                                                            Gremio = ch.Gremio,
                                                            HorasExtras = ch.HorasExtras,
                                                            HorasNocturnas = ch.HorasNocturnas,
                                                            HorasNormales = ch.HorasNormales,
                                                            Id = ch.Id
                                                        }).ToList(),

                                                        CotizacionPosiciones = cotizacion.CotizacionPosiciones.Where(posic => posic.PeticionDeOfertaSolpPosicion.SolpPosicion.EsConcluido == true)
                                                        .Select(p => new CotizacionPosicionDto
                                                        {
                                                            Id = p.Id,
                                                            PosicionId = p.PeticionDeOfertaSolpPosicion.SolpPosicion.Id,
                                                            EstaEliminado = p.PeticionDeOfertaSolpPosicion.SolpPosicion.Estado != true,
                                                            Cotizacion_Id = p.Cotizacion_Id,
                                                            PeticionDeOfertaSolpPosicion_Id = p.PeticionDeOfertaSolpPosicion_Id,
                                                            Cantidad = p.Cantidad ?? 1,
                                                            UnidadMedida = new TablaSapDto
                                                            {
                                                                Descripcion = p.UnidadDeMedida.Descripcion,
                                                                CodigoSap = p.UnidadDeMedida.CodigoSap
                                                            },
                                                            Moneda_Id = p.PeticionDeOfertaSolpPosicion.SolpPosicion.TipoPosicion_Id == 9 ? p.PeticionDeOfertaSolpPosicion.SolpPosicion.Moneda_Id ?? 0 : p.Moneda_Id ?? 0,
                                                            MonedaDescripcion = cotizacion != null && p.Moneda != null ? p.Moneda.Codigo : "",
                                                            FechaDeEntrega = p.FechaDeEntrega,
                                                            FechaDeEntregaFormateada = SqlFunctions.DateName("day", p.FechaDeEntrega) + "/" + SqlFunctions.DatePart("month", p.FechaDeEntrega) + "/" + SqlFunctions.DateName("year", p.FechaDeEntrega),
                                                            Precio = p.Precio ?? 0,
                                                            PrecioTotal = p.Cantidad != null && p.Precio != null ? p.Cantidad.Value * p.Precio.Value : 0,
                                                            TotalARPCotizacionPosicion = 0,
                                                            TotalPosicionCotizacion = 0,
                                                            TotalPesos = 0,
                                                            NoDisponible = cotizacion != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == p.PeticionDeOfertaSolpPosicion.Id).FirstOrDefault().NoDisponible : null,
                                                            PrimerPlazoDeOferta = cotizacion != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == p.PeticionDeOfertaSolpPosicion.Id).FirstOrDefault().PrimerPlazoDeOferta : null,
                                                            SegundoPlazoDeOferta = cotizacion != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == p.PeticionDeOfertaSolpPosicion.Id).FirstOrDefault().SegundoPlazoDeOferta : null,
                                                            TercerPlazoDeOferta = cotizacion != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == p.PeticionDeOfertaSolpPosicion.Id).FirstOrDefault().TercerPlazoDeOferta : null,
                                                            PrimeraCantidad = cotizacion != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == p.PeticionDeOfertaSolpPosicion.Id).FirstOrDefault().PrimeraCantidad : null,
                                                            SegundaCantidad = cotizacion != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == p.PeticionDeOfertaSolpPosicion.Id).FirstOrDefault().SegundaCantidad : null,
                                                            TerceraCantidad = cotizacion != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == p.PeticionDeOfertaSolpPosicion.Id).FirstOrDefault().TerceraCantidad : null,
                                                            Completado = true,
                                                            CotizacionSubPosiciones = (from subpos in contexto.Set<CotizacionSubPosicion>()
                                                                                       where p.Id == subpos.CotizacionPosicion_Id
                                                                                       select new CotizacionSubPosicionDto()
                                                                                       {
                                                                                           Id = subpos.Id,
                                                                                           Completado = true,
                                                                                           CotizacionPosicion_Id = subpos.CotizacionPosicion_Id,
                                                                                           SolpSubPosicion_Id = subpos.SolpSubPosicion_Id,
                                                                                           Cantidad = subpos.Cantidad ?? 0,
                                                                                           UnidadDeMedida_Id = subpos.UnidadDeMedida_Id ?? 0,
                                                                                           UnidadMedida = new TablaSapDto
                                                                                           {
                                                                                               Descripcion = subpos.UnidadDeMedida.Descripcion,
                                                                                               CodigoSap = subpos.UnidadDeMedida.CodigoSap
                                                                                           },
                                                                                           MonedaDescripcion = subpos.Moneda != null ? subpos.Moneda.Codigo : "",
                                                                                           PrecioUnidad = subpos.Precio ?? 0,
                                                                                           PrecioTotalSubPosCotizacion = subpos.Cantidad != null && subpos.Precio != null ? subpos.Cantidad.Value * subpos.Precio.Value : 0,
                                                                                           TotalARPSubPosCotizacion = 0,
                                                                                           Moneda_Id = subpos.Moneda_Id ?? 0,
                                                                                           TotalPesos = 0,
                                                                                           PrecioTotalSubPos = subpos.Cantidad != null && subpos.Precio != null ? subpos.Cantidad.Value * subpos.Precio.Value : 0,
                                                                                       }).ToList()

                                                        }).ToList(),
                                                        TieneAdjuntos = cotizacion.Archivos.Any(),
                                                        Adjudicaciones = cotizacion.Adjudicaciones.Select(a => new AdjudicacionDto
                                                        {
                                                            CondicionesDeEntrega = a.CondicionesDeEntrega,
                                                            CondicionesDePago = a.CondicionesDePago,
                                                            Garantias = a.Garantias,
                                                            TextoDeCabecera = a.TextoDeCabecera
                                                        }).ToList(),
                                                    } : null,
                                                }).ToList()
                                };

                return resultado.First();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}