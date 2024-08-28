using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Linq;

namespace SustitucionMOARepositorio.ConsultasEF
{
    public class TraerCotizacionConsulta : IConsultaEscalar<PeticionDeOfertaDto>
    {
        private readonly int PeticionDeOfertaUsuario_Id;
        public TraerCotizacionConsulta(int peticionDeOfertaUsuario_Id)
        {
            PeticionDeOfertaUsuario_Id = peticionDeOfertaUsuario_Id;
        }
        public PeticionDeOfertaDto Ejecutar(DbContext contexto)
        {
            try
            {
                ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
                var resultado = from po in contexto.Set<PeticionDeOfertaUsuario>()
                                join cotizacion in contexto.Set<Cotizacion>() on po.Id equals cotizacion.PeticionDeOfertaUsuario.Id into peticionCotizacion
                                from cotizacion in peticionCotizacion.DefaultIfEmpty()
                                where po.Id == PeticionDeOfertaUsuario_Id
                                select new PeticionDeOfertaDto
                                {
                                    Id = po.Id,
                                    PersonalHoras = po.PeticionDeOferta.Posiciones.FirstOrDefault().SolpPosicion.Solp.Pliego != null ? po.PeticionDeOferta.Posiciones.FirstOrDefault().SolpPosicion.Solp.Pliego.TieneGrillaPersonal ?? false : false,
                                    NrosSolp = po.PeticionDeOferta.Posiciones.Select(x => x.SolpPosicion.Solp.NroSolp),
                                    FechaCreacion = po.PeticionDeOferta.FechaCreacion,
                                    UsuarioCreador_Id = po.PeticionDeOferta.UsuarioCreador_Id,
                                    PlazoDeOferta = po.PeticionDeOferta.PlazoDeOferta,
                                    Cotizacion = new CotizacionDto(),
                                    ObservacionCotizacion = po.PeticionDeOferta.Posiciones.FirstOrDefault().SolpPosicion.Solp.Pliego != null ? po.PeticionDeOferta.Posiciones.FirstOrDefault().SolpPosicion.Solp.Pliego.ObservacionesCotizacion  : null,
                                    ObservacionTecnica = cotizacion != null ? cotizacion.ObservacionTecnica : "",
                                    ObservacionEconomica = cotizacion != null ? cotizacion.ObservacionEconomica : "",
                                    ObservacionTecnicaOriginal = cotizacion != null ? cotizacion.ObservacionTecnica : "",
                                    ObservacionEconomicaOriginal = cotizacion != null ? cotizacion.ObservacionEconomica : "",
                                    RespetaMateriales = cotizacion != null ? cotizacion.RespetaMateriales : null,
                                    RespetaServicios = cotizacion != null ? cotizacion.RespetaServicios : null,
                                    TipoPosicionCodigo = po.PeticionDeOferta.Posiciones.FirstOrDefault().SolpPosicion.TipoPosicion.Codigo,
                                    CotizacionId = cotizacion != null ? cotizacion.Id : 0,
                                    PorcentajeDeHoras = cotizacion.PorcentajeDeHoras,
                                    PideDescripcionTecnica = po.PeticionDeOferta.Posiciones.FirstOrDefault().SolpPosicion.Solp.Pliego.TieneDescripcionTecnica == true,
                                    PideDocumentacionTecnica = po.PeticionDeOferta.Posiciones.FirstOrDefault().SolpPosicion.Solp.Pliego.TieneDocumentacionTecnica == true,
                                    EsNuevaCotizacion = cotizacion != null && cotizacion.CotizarNuevaPosicion == true ? true : false,
                                    CotizacionEstado_Id = cotizacion == null ? 0 : cotizacion.CotizacionEstado.Id,
                                    ArchivosPaso4Cotizacion = po.PeticionDeOferta.Posiciones.FirstOrDefault().SolpPosicion.Solp.Pliego.Archivos
                                            .Where(a => a.FileKey == FileKeys.AdjuntoCotizacionesSolp)
                                            .Select(a => new ArchivoDto { Ruta = a.Ruta, Id = a.Id }).ToList().OrderBy(a => a.Id),
                                    RequisitoCiberseguridad = po.PeticionDeOferta.Posiciones.FirstOrDefault().SolpPosicion.Solp.Pliego.RequisitoCiberseguridad == true ? true : false,
                                    PeticionDeOfertaPosicion = po.PeticionDeOferta.Posiciones.Where(posi => posi.SolpPosicion.EsConcluido == true && posi.SolpPosicion.Estado == true).Select(pop =>
                                    new PeticionDeOfertaSolpPosicionDto()
                                    {
                                        Id = pop.Id,
                                        PeticionDeOferta_Id = pop.PeticionDeOferta_Id,
                                        SolpPosicion_Id = pop.SolpPosicion_Id,
                                        SolpId = pop.SolpPosicion.Solp_Id,
                                        Posiciones = new SolpPosicionDto
                                        {
                                            Indice = pop.SolpPosicion.Indice,
                                            EsConcluido = pop.SolpPosicion.EsConcluido,
                                            Estado = pop.SolpPosicion.Estado,
                                            Id = pop.Id, //Pos
                                            Codigo = pop.SolpPosicion.MaterialSolp.CodigoSap, //Codigo
                                            Tarea = pop.SolpPosicion.Tarea,
                                            Modelo = pop.SolpPosicion.Modelo,
                                            TextoSuministro = pop.SolpPosicion.TextoSuministro,
                                            Cantidad = pop.SolpPosicion.Cantidad,
                                            UnidadComprasDescripcion = pop.SolpPosicion.Unidad.Descripcion,
                                            UnidadId = pop.SolpPosicion.Unidad_Id,
                                            FechaEntregaServicio = pop.SolpPosicion.FechaEntregaServicio,
                                            FechaOferta = pop.SolpPosicion.FechaEntregaServicio,
                                            NroSolp = pop.SolpPosicion.Solp.NroSolp,
                                            CotizacionPosicion = new CotizacionPosicionDto()
                                            {
                                                
                                                Cantidad = cotizacion != null && cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().Cantidad != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().Cantidad.Value : 0,
                                                UnidadMedidaDescripcion = cotizacion != null && cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().UnidadDeMedida != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().UnidadDeMedida.Descripcion : (pop.SolpPosicion.Unidad != null ? pop.SolpPosicion.Unidad.Descripcion : ""),
                                                UnidadDeMedida_Id = cotizacion != null && cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().UnidadDeMedida != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().UnidadDeMedida.Id : (pop.SolpPosicion.Unidad_Id != null ? pop.SolpPosicion.Unidad_Id.Value : 0),
                                                MonedaDescripcion = cotizacion != null && cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().Moneda != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().Moneda.Descripcion : "",
                                                MonedaCodigo = cotizacion != null && cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().Moneda != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().Moneda.Codigo : "",
                                                Moneda_Id = cotizacion != null && cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().Moneda_Id != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().Moneda_Id.Value : 0,
                                                Precio = cotizacion != null && cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().Precio != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().Precio.Value : 0,
                                                FechaOriginal = cotizacion != null && cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().FechaDeEntrega != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().FechaDeEntrega : pop.SolpPosicion.FechaEntregaServicio,
                                                FechaDeEntrega = cotizacion != null && cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().FechaDeEntrega != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().FechaDeEntrega : pop.SolpPosicion.FechaEntregaServicio,
                                                FechaDeEntregaFormateado = cotizacion != null && cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().FechaDeEntrega != null ?
                                                SqlFunctions.DateName("day", cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().FechaDeEntrega) + "/" + SqlFunctions.DatePart("month", cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().FechaDeEntrega) + "/" + SqlFunctions.DateName("year", cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().FechaDeEntrega)
                                                : SqlFunctions.DateName("day", pop.SolpPosicion.FechaEntregaServicio) + "/" + SqlFunctions.DatePart("month", pop.SolpPosicion.FechaEntregaServicio) + "/" 
                                                + SqlFunctions.DateName("year", pop.SolpPosicion.FechaEntregaServicio),
                                                PlazoDeEntrega = 0,
                                                FechaDeVigencia = cotizacion != null && cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().FechaDeVigencia != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().FechaDeVigencia : (DateTime?)null,
                                                FechaDeVigenciaFormateado = cotizacion != null && cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().FechaDeVigencia != null ? SqlFunctions.DateName("day", cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().FechaDeVigencia) + "/" + SqlFunctions.DatePart("month", cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().FechaDeVigencia) + "/" + SqlFunctions.DateName("year", cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().FechaDeVigencia) : "",
                                                PrecioTotal = cotizacion != null && cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().Cantidad != null && cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().Precio != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().Cantidad.Value * cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().Precio.Value : 0,
                                                Moneda = new TablaSapDto
                                                {
                                                    Codigo = cotizacion != null && cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().Moneda != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().Moneda.Codigo : "",
                                                    Id = cotizacion != null && cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().Moneda_Id != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().Moneda_Id.Value : 0
                                                },
                                                UnidadMedida = new TablaSapDto
                                                {
                                                    Codigo = cotizacion != null && cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().UnidadDeMedida_Id != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().UnidadDeMedida.Codigo : (pop.SolpPosicion.Unidad != null ? pop.SolpPosicion.Unidad.Codigo : ""),
                                                    Id = cotizacion != null && cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().UnidadDeMedida_Id != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().UnidadDeMedida_Id.Value : (pop.SolpPosicion.Unidad_Id != null ? pop.SolpPosicion.Unidad_Id.Value : 0),
                                                },
                                                NoDisponible = cotizacion != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().NoDisponible : null,
                                                PrimerPlazoDeOferta = cotizacion != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().PrimerPlazoDeOferta : 0,
                                                PrimeraCantidad = cotizacion != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().PrimeraCantidad : 0,
                                                SegundoPlazoDeOferta = cotizacion != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().SegundoPlazoDeOferta : 0,
                                                SegundaCantidad = cotizacion != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().SegundaCantidad : 0,
                                                TercerPlazoDeOferta = cotizacion != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().TercerPlazoDeOferta : 0,
                                                TerceraCantidad = cotizacion != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().TerceraCantidad : 0,
                                            },
                                            SubposicionesCompras = pop.SolpPosicion.Subposiciones.Select(subposicion => new SolpSubposicionDto
                                            {
                                                Id = subposicion.Id,
                                                Codigo = subposicion.ServicioSolp.CodigoSap + "",
                                                Tarea = subposicion.Tarea,
                                                Cantidad = subposicion.Cantidad,
                                                UnidadDescripcion = subposicion.Unidad.Descripcion,
                                                UnidadId = subposicion.Unidad_Id,
                                                Numero = subposicion.Numero,

                                                CotizacionSubPosicionId = (cotizacion != null && cotizacion.CotizacionPosiciones.Any(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id)) ||
                                                                      cotizacion.CotizacionPosiciones.Any(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                        && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)) ?
                                                              cotizacion.CotizacionPosiciones
                                                              .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                        && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id))
                                                              .FirstOrDefault().CotizacionSubPosiciones.Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault().Id : 0,

                                                CantidadCotizacion = (cotizacion != null && cotizacion.CotizacionPosiciones.Any(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id)) ||
                                                                      cotizacion.CotizacionPosiciones.Any(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                        && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)) ?
                                                              cotizacion.CotizacionPosiciones
                                                              .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                        && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)
                                                              ).FirstOrDefault().CotizacionSubPosiciones.Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault().Cantidad.Value : 0,
                                                                                                
                                                MonedaCotizacionId = (cotizacion != null && cotizacion.CotizacionPosiciones.Any(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id)) ||
                                                                      cotizacion.CotizacionPosiciones.Any(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                        && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)) ? cotizacion.CotizacionPosiciones
                                                               .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                        && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)
                                                               ).FirstOrDefault().CotizacionSubPosiciones.Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault().Moneda.Id : 0,

                                                MonedaCotizacionDescripcion = (cotizacion != null && cotizacion.CotizacionPosiciones.Any(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id)) ||
                                                                      cotizacion.CotizacionPosiciones.Any(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                        && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)) ? cotizacion.CotizacionPosiciones
                                                               .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                        && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)
                                                               ).FirstOrDefault().CotizacionSubPosiciones.Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault().Moneda.Descripcion : "",

                                                MonedaCotizacionCodigo = (cotizacion != null && cotizacion.CotizacionPosiciones.Any(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id)) ||
                                                                      cotizacion.CotizacionPosiciones.Any(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                        && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)) ? cotizacion.CotizacionPosiciones
                                                               .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                        && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)
                                                               ).FirstOrDefault().CotizacionSubPosiciones.Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault().Moneda.Codigo : "",

                                                MonedaCotizacion = new TablaSapDto
                                                {
                                                    Codigo = (cotizacion != null && cotizacion.CotizacionPosiciones.Any(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id)) ||
                                                                      cotizacion.CotizacionPosiciones.Any(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                        && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)) ?
                                                                       cotizacion.CotizacionPosiciones
                                                                       .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                        && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)
                                                                       ).FirstOrDefault().CotizacionSubPosiciones
                                                                       .Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault().Moneda.Codigo : "",

                                                    Id = (cotizacion != null && cotizacion.CotizacionPosiciones.Any(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id)) ||
                                                                          cotizacion.CotizacionPosiciones.Any(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                            && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)) ?
                                                                           cotizacion.CotizacionPosiciones
                                                                           .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                            && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)
                                                                           ).FirstOrDefault().CotizacionSubPosiciones
                                                                           .Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault().Moneda_Id ?? 0 : 0,

                                                },
                                                UnidadMedidaCotizacion = new TablaSapDto
                                                {
                                                    Codigo = subposicion.Unidad.Codigo,                                                    
                                                    Id = subposicion.Unidad.Id,
                                                },
                                                UnidadCotizacionDescripcion = subposicion.Unidad.Descripcion,
                                                UnidadCotizacionId = subposicion.Unidad.Id,

                                                PrecioSubPosicion = (cotizacion != null && cotizacion.CotizacionPosiciones.Any(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id)) ||
                                                                      cotizacion.CotizacionPosiciones.Any(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                        && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)) ?
                                                                       cotizacion.CotizacionPosiciones
                                                                       .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                        && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)
                                                                       ).FirstOrDefault().CotizacionSubPosiciones
                                                                       .Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault().Precio ?? 0 : 0,

                                                PrecioTotalSubPosicion = (cotizacion != null && cotizacion.CotizacionPosiciones.Any(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id)) ||
                                                                      cotizacion.CotizacionPosiciones.Any(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                        && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)) ?
                                                                       cotizacion.CotizacionPosiciones
                                                                       .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                        && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)
                                                                       ).FirstOrDefault().CotizacionSubPosiciones
                                                                       .Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault().Precio ?? 0 *
                                                                       cotizacion.CotizacionPosiciones
                                                                       .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                        && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)
                                                                       ).FirstOrDefault().CotizacionSubPosiciones
                                                                       .Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault().Cantidad ?? 0 : 0,
                                            }).ToList().OrderBy(x => x.Numero)
                                            ,
                                        }
                                    }).ToList().OrderBy(x => x.Posiciones.Indice),
                                };

                var result = resultado.First();
                return result;
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
