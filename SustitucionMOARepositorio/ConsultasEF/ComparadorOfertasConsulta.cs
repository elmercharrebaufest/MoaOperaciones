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
                                join adjudicacion in contexto.Set<Adjudicacion>() on po.Id equals adjudicacion.Solp_Id into adjudicacionCotizacion
                                from adjudicacion in adjudicacionCotizacion.DefaultIfEmpty()
                                join adjudicacionPosicion in contexto.Set<AdjudicacionPosicion>() on adjudicacion.Id equals adjudicacionPosicion.Adjudicacion_Id into adjudicacionCotizacionPosicion
                                from adjudicacionPosicion in adjudicacionCotizacionPosicion.DefaultIfEmpty()
                                where po.Id == PeticionOferta_Id
                                select new PeticionDeOfertaDto
                                {
                                    Id = po.Id,
                                    Solp_Id = po.Solp_Id,
                                    FechaCreacion = po.FechaCreacion,
                                    FechaCreacionFormateada = SqlFunctions.DateName("day", po.FechaCreacion) + "/" + SqlFunctions.DatePart("month", po.FechaCreacion) + "/" + SqlFunctions.DateName("year", po.FechaCreacion),
                                    UsuarioCreador_Id = po.UsuarioCreador_Id,
                                    PlazoDeOferta = po.PlazoDeOferta,
                                    Observaciones = po.Observaciones,                              
                                    FechaCreacionSolp = po.Solp.FechaCreacion,
                                    FechaCreacionFormateadaSolp = SqlFunctions.DateName("day", po.Solp.FechaCreacion) + "/" + SqlFunctions.DatePart("month", po.Solp.FechaCreacion) + "/" + SqlFunctions.DateName("year", po.Solp.FechaCreacion),
                                    TipoPosicionCodigo = po.Solp.Posiciones.Select(x => x.TipoPosicion.Codigo).FirstOrDefault(),
                                    NroSolp = po.Solp.NroSolp,
                                    PeticionDeOfertaPosicion = (from pop in contexto.Set<PeticionDeOfertaSolpPosicion>()
                                                                where po.Id == pop.PeticionDeOferta_Id
                                                                
                                                                select new PeticionDeOfertaSolpPosicionDto()
                                                                {
                                                                    Id = pop.Id,
                                                                    PeticionDeOferta_Id = pop.PeticionDeOferta_Id,
                                                                    SolpPosicion_Id = pop.SolpPosicion_Id,
                                                                    Posicion = new SolpPosicionDto
                                                                    {
                                                                        Id = pop.SolpPosicion.Id,
                                                                        Indice = pop.SolpPosicion.Indice,
                                                                        CodigoMaterialSap = new MaterialSolpDto
                                                                        {
                                                                            Descripcion = pop.SolpPosicion.MaterialSolp.Descripcion,
                                                                            Codigo = pop.SolpPosicion.MaterialSolp.Codigo,

                                                                        },
                                                                        Codigo = pop.SolpPosicion.Codigo,
                                                                        Tarea = pop.SolpPosicion.Tarea,
                                                                        TextoSuministro = pop.SolpPosicion.TextoSuministro,
                                                                        Cantidad = pop.SolpPosicion.Cantidad,
                                                                        CantidadPendiente = pop.SolpPosicion.Cantidad - (adjudicacionPosicion != null ? adjudicacionPosicion.Cantidad : 0),
                                                                        CantidadAdjudicacion = adjudicacionPosicion != null ? adjudicacionPosicion.Cantidad : 0,

                                                                        Unidad = new TablaSapDto {
                                                                            Descripcion = pop.SolpPosicion.Unidad.Descripcion
                                                                        },


                                                                        Subposiciones = pop.SolpPosicion.Subposiciones.Select(s => new SolpSubposicionDto
                                                                        {
                                                                            Id = s.Id,
                                                                            CodigoServicioSapId = s.ServicioSolp_Id,
                                                                            ServicioSolpCodigo = s.ServicioSolp.CodigoSap,
                                                                            Tarea = s.Tarea,
                                                                            Cantidad = s.Cantidad,
                                                                            Unidad = new TablaSapDto
                                                                            {
                                                                                Descripcion = s.Unidad.Descripcion
                                                                            },
                                                                            Numero = s.Numero

                                                                        }).ToList()
                                                                    },
                                                                }),
                                    Usuarios = (from u in contexto.Set<PeticionDeOfertaUsuario>()
                                                join cotizacion in contexto.Set<Cotizacion>() on u.Id equals cotizacion.PeticionDeOfertaUsuario_Id into peticionCotizacion
                                                from cotizacion in peticionCotizacion.DefaultIfEmpty()
                                                where po.Id == u.PeticionDeOferta_Id
                                                select new PeticionDeOfertaUsarioDto()            
                                                {
                                                    Id = u.Id,
                                                    UsuarioId = u.Usuario_Id,
                                                    RazonSocial = u.Usuario.Proveedores.Where(p => p.CUIT == u.Usuario.CUITRegistro && u.Usuario.TipoUsuario.Id == p.TipoProveedor.Id).FirstOrDefault() != null ?
                                                        u.Usuario.Proveedores.Where(p => p.CUIT == u.Usuario.CUITRegistro && u.Usuario.TipoUsuario.Id == p.TipoProveedor.Id).FirstOrDefault().RazonSocial : u.Usuario.CUITRegistro,
                                                    PropuestaTecnicaAprobada = u.PropuestaTecnicaAprobada,
                                                    RealizoVisita = u.RealizoVisita,
                                                    EstadoVisita = u.RealizoVisita == true ? "Realizada" : "Sin realizar",
                                                    EstadoVisitaColor = u.RealizoVisita == true ? "Green" : "Red",
                                                    EstadoPropuestaTecnica = u.PropuestaTecnicaAprobada == null ? "Sin analizar" : (u.PropuestaTecnicaAprobada == true ? "Realizada" : "Rechazada"),
                                                    EstadoPropuestaTecnicaColor = u.PropuestaTecnicaAprobada == null ? "Orange" : (u.PropuestaTecnicaAprobada == true ? "Green" : "Red"),                                               
                                                    Cotizacion = cotizacion != null ? new CotizacionDto()
                                                                 {
                                                                    Id = cotizacion.Id,
                                                                    PeticionDeOfertaUsuario_Id = cotizacion.PeticionDeOfertaUsuario_Id,
                                                                    FechaCreacion = u.Cotizaciones.FirstOrDefault().FechaCreacion,
                                                                    FechaCreacionFormateada = SqlFunctions.DateName("day", u.Cotizaciones.FirstOrDefault().FechaCreacion) + "/" + SqlFunctions.DatePart("month", u.Cotizaciones.FirstOrDefault().FechaCreacion) + "/" + SqlFunctions.DateName("year", u.Cotizaciones.FirstOrDefault().FechaCreacion),
                                                                    UsuarioCreador_Id = u.Cotizaciones.FirstOrDefault().UsuarioCreador_Id,
                                                                    CotizacionEstadoDescripcion = u.Cotizaciones.FirstOrDefault().CotizacionEstado.Descripcion,
                                                                    RespetaMateriales = u.Cotizaciones.FirstOrDefault().RespetaMateriales,
                                                                    RespetaServicios = u.Cotizaciones.FirstOrDefault().RespetaServicios,
                                                                    ObservacionEconomica = u.Cotizaciones.FirstOrDefault().ObservacionEconomica,
                                                                    ObservacionTecnica = u.Cotizaciones.FirstOrDefault().ObservacionTecnica,
                                                                    TotalGlobal = 0,
                                                                    TotalPesos = 0,
                                                                    TotalGlobalSubPos = 0,

                                                                    CotizacionPosiciones = cotizacion.CotizacionPosiciones.Select(p => new CotizacionPosicionDto
                                                                    {
                                                                        Id = p.Id,
                                                                        Cotizacion_Id = p.Cotizacion_Id,
                                                                        PeticionDeOfertaSolpPosicion_Id = p.PeticionDeOfertaSolpPosicion_Id,
                                                                        Cantidad = p.Cantidad.Value,    
                                                                        UnidadMedida = new TablaSapDto
                                                                        {
                                                                            Descripcion = p.UnidadDeMedida.Descripcion
                                                                        },
                                                                        Moneda_Id = p.Moneda_Id.Value,
                                                                        MonedaDescripcion = cotizacion != null && p.Moneda != null ? p.Moneda.Codigo : "",
                                                                        FechaDeEntrega = p.FechaDeEntrega,
                                                                        FechaDeEntregaFormateada = SqlFunctions.DateName("day", p.FechaDeEntrega) + "/" + SqlFunctions.DatePart("month", p.FechaDeEntrega) + "/" + SqlFunctions.DateName("year", p.FechaDeEntrega),
                                                                        Precio = p.Precio.Value,
                                                                        PrecioTotal = p.Cantidad.Value * p.Precio.Value,
                                                                        TotalARPCotizacionPosicion = 0,
                                                                        TotalPosicionCotizacion = 0,
                                                                        TotalPesos = 0,
                                                                        TotalPosicionCotizacion = 0,
                                                                        CotizacionSubPosiciones = (from subpos in contexto.Set<CotizacionSubPosicion>()
                                                                                                   where p.Id == subpos.CotizacionPosicion_Id
                                                                                                   select new CotizacionSubPosicionDto()
                                                                                                   //p.CotizacionSubPosiciones == null ? null : p.CotizacionSubPosiciones.Select(s => new CotizacionSubPosicionDto
                                                                                                   {
                                                                                                        Id = subpos.Id,
                                                                                                        CotizacionPosicion_Id = subpos.CotizacionPosicion_Id,
                                                                                                        SolpSubPosicion_Id = subpos.SolpSubPosicion_Id,
                                                                                                        Cantidad = subpos.Cantidad.Value,
                                                                                                        UnidadDeMedida_Id = subpos.UnidadDeMedida_Id.Value,
                                                                                                        UnidadMedida = new TablaSapDto
                                                                                                        {
                                                                                                            Descripcion = subpos.UnidadDeMedida.Descripcion
                                                                                                        },
                                                                                                        MonedaDescripcion = cotizacion != null && subpos.Moneda != null ? subpos.Moneda.Codigo : "",
                                                                                                        PrecioUnidad = subpos.Precio.Value,
                                                                                                        PrecioTotalSubPosCotizacion = subpos.Cantidad.Value * subpos.Precio.Value,
                                                                                                        TotalARPSubPosCotizacion = 0,
                                                                                                        Moneda_Id = subpos.Moneda_Id.Value,                                                                                                      
                                                                                                        TotalPesos = 0,
                                                                                                        PrecioTotalSubPos = subpos.Cantidad.Value * subpos.Precio.Value,
                                                                                                        MonedaDescripcion = cotizacion != null && subpos.Moneda != null ? subpos.Moneda.Codigo : "",
                                                                                                        PrecioUnidad = subpos.Precio.Value,
                                                                                                        PrecioTotalSubPosCotizacion = subpos.Cantidad.Value * subpos.Precio.Value,
                                                                                                        TotalARPSubPosCotizacion = 0,

                                                                                                    }).ToList()

                                                                    }).ToList(),
                                                                    TieneAdjuntos = cotizacion.Archivos.Any()
                                                    } : null,
                                                }).ToList(),
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
