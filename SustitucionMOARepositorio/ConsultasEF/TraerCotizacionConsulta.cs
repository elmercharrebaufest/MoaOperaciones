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
using System.IO;
using System.Linq;

namespace SustitucionMOARepositorio.ConsultasEF
{
    public class TraerCotizacionConsulta : IConsultaEscalar<PeticionDeOfertaDto>
    {
        private readonly int PeticionOferta_Id;
        public TraerCotizacionConsulta(int peticionOferta_Id)
        {
            this.PeticionOferta_Id = peticionOferta_Id;
        }
        public PeticionDeOfertaDto Ejecutar(DbContext contexto)
        {
            var cotizacionesHoras = new List<CotizacionHorasDto>();
            try
            {
                ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
                var resultado = from po in contexto.Set<PeticionDeOfertaUsuario>()
                                join cotizacion in contexto.Set<Cotizacion>() on po.Id equals cotizacion.PeticionDeOfertaUsuario.Id into peticionCotizacion
                                from cotizacion in peticionCotizacion.DefaultIfEmpty()
                                where po.PeticionDeOferta_Id == PeticionOferta_Id
                                select new PeticionDeOfertaDto
                                {
                                    Id = po.Id,
                                    Solp_Id = po.PeticionDeOferta.Solp_Id,
                                    NroSolp = po.PeticionDeOferta.Solp.NroSolp,
                                    FechaCreacion = po.PeticionDeOferta.FechaCreacion,
                                    UsuarioCreador_Id = po.PeticionDeOferta.UsuarioCreador_Id,
                                    PlazoDeOferta = po.PeticionDeOferta.PlazoDeOferta,
                                    Cotizacion = new CotizacionDto(),
                                    ObservacionTecnica = cotizacion != null ? cotizacion.ObservacionTecnica : "",
                                    ObservacionEconomica = cotizacion != null ? cotizacion.ObservacionEconomica : "",
                                    RespetaMateriales = cotizacion != null ? cotizacion.RespetaMateriales : null,
                                    RespetaServicios = cotizacion != null ? cotizacion.RespetaServicios : null,
                                    TipoPosicionCodigo = po.PeticionDeOferta.Solp.Posiciones.Select(x => x.TipoPosicion.Codigo).FirstOrDefault(),
                                    CotizacionId = cotizacion != null ? cotizacion.Id : 0,
                                    PeticionDeOfertaPosicion = (from pop in contexto.Set<PeticionDeOfertaSolpPosicion>()
                                                                join cotizacionPosicion in contexto.Set<CotizacionPosicion>() on pop.Id equals cotizacionPosicion.PeticionDeOfertaSolpPosicion_Id into peticionPosicionCotizacionPosicion
                                                                from cotizacionPosicion in peticionPosicionCotizacionPosicion.DefaultIfEmpty()
                                                                where po.PeticionDeOferta_Id == pop.PeticionDeOferta_Id
                                                                select new PeticionDeOfertaSolpPosicionDto()
                                                                {
                                                                    Id = pop.Id,
                                                                    PeticionDeOferta_Id = pop.PeticionDeOferta_Id,
                                                                    SolpPosicion_Id = pop.SolpPosicion_Id,
                                                                    SolpId = pop.SolpPosicion.Solp_Id,
                                                                    Posiciones = new SolpPosicionDto
                                                                    {
                                                                        Indice = pop.SolpPosicion.Indice,
                                                                        Id = pop.Id, //Pos
                                                                        Codigo = pop.SolpPosicion.MaterialSolp.CodigoSap, //Codigo
                                                                        Tarea = pop.SolpPosicion.Tarea,
                                                                        //TextoSuministro = pop.SolpPosicion.TextoSuministro,
                                                                        Cantidad = pop.SolpPosicion.Cantidad,
                                                                        UnidadComprasDescripcion = pop.SolpPosicion.Unidad.Descripcion,
                                                                        UnidadId = pop.SolpPosicion.Unidad_Id,
                                                                        FechaEntregaServicio = cotizacion != null ? cotizacionPosicion.FechaDeEntrega : pop.SolpPosicion.FechaEntregaServicio,
                                                                        FechaOferta = pop.SolpPosicion.Solp.Pliego_Id != null ? pop.SolpPosicion.Solp.Pliego.FechaHoraEntrega : (DateTime?)null,
                                                                        CotizacionPosicion = new CotizacionPosicionDto()
                                                                        {
                                                                            Cantidad = cotizacion != null && cotizacionPosicion.Cantidad != null ? cotizacionPosicion.Cantidad.Value : 0,
                                                                            UnidadMedidaDescripcion = cotizacion != null && cotizacionPosicion.UnidadDeMedida != null ? cotizacionPosicion.UnidadDeMedida.Descripcion : "",
                                                                            UnidadDeMedida_Id = cotizacion != null && cotizacionPosicion.UnidadDeMedida != null ? cotizacionPosicion.UnidadDeMedida.Id : 0,
                                                                            MonedaDescripcion = cotizacion != null && cotizacionPosicion.Moneda != null ? cotizacionPosicion.Moneda.Descripcion : "",
                                                                            MonedaCodigo = cotizacion != null && cotizacionPosicion.Moneda != null ? cotizacionPosicion.Moneda.Codigo : "",
                                                                            Moneda_Id = cotizacion != null && cotizacionPosicion.Moneda_Id != null ? cotizacionPosicion.Moneda_Id.Value : 0,
                                                                            Precio = cotizacion != null && cotizacionPosicion.Precio != null ? cotizacionPosicion.Precio.Value : 0,
                                                                            FechaOriginal = cotizacion != null && cotizacionPosicion.FechaDeEntrega != null ? cotizacionPosicion.FechaDeEntrega : (DateTime?)null,
                                                                            FechaDeEntrega = cotizacion != null && cotizacionPosicion.FechaDeEntrega != null ? cotizacionPosicion.FechaDeEntrega : (DateTime?)null,
                                                                            FechaDeEntregaFormateado = cotizacion != null && cotizacionPosicion.FechaDeEntrega != null ? SqlFunctions.DateName("day", cotizacionPosicion.FechaDeEntrega) + "/" + SqlFunctions.DatePart("month", cotizacionPosicion.FechaDeEntrega) + "/" + SqlFunctions.DateName("year", cotizacionPosicion.FechaDeEntrega) : "",
                                                                            PlazoDeEntrega = 0,
                                                                            PrecioTotal = cotizacion != null && cotizacionPosicion.Cantidad != null && cotizacionPosicion.Precio != null ? cotizacionPosicion.Cantidad.Value * cotizacionPosicion.Precio.Value : 0,
                                                                            Moneda = new TablaSapDto
                                                                            {
                                                                                Codigo = cotizacion != null && cotizacionPosicion.Moneda != null ? cotizacionPosicion.Moneda.Codigo : "",
                                                                                Id = cotizacion != null && cotizacionPosicion.Moneda_Id != null ? cotizacionPosicion.Moneda_Id.Value : 0
                                                                            }
                                                                        },

                                                                        SubposicionesCompras = (from subposicion in contexto.Set<SolpSubposicion>()
                                                                                                join cotizacionSubposicion in contexto.Set<CotizacionSubPosicion>() on subposicion.Id equals cotizacionSubposicion.SolpSubPosicion_Id into subposicionCotizacionSubposicion
                                                                                                from cotizacionSubposicion in subposicionCotizacionSubposicion.DefaultIfEmpty()
                                                                                                where subposicion.SolpPosicion_Id == pop.SolpPosicion_Id
                                                                                                select new SolpSubposicionDto
                                                                                                {
                                                                                                    Id = subposicion.Id,
                                                                                                    Codigo = subposicion.ServicioSolp.CodigoSap + "",
                                                                                                    Tarea = subposicion.Tarea,
                                                                                                    Cantidad = subposicion.Cantidad,
                                                                                                    UnidadDescripcion = subposicion.Unidad.Descripcion,
                                                                                                    UnidadId = subposicion.Unidad_Id,
                                                                                                    Numero = subposicion.Numero,
                                                                                                    CotizacionSubPosicionId = cotizacionSubposicion != null ? cotizacionSubposicion.Id : 0,
                                                                                                    CantidadCotizacion = cotizacionSubposicion != null ? cotizacionSubposicion.Cantidad : 0,
                                                                                                    UnidadCotizacionDescripcion = cotizacionSubposicion != null && cotizacionSubposicion.UnidadDeMedida != null ? cotizacionSubposicion.UnidadDeMedida.Descripcion : "",
                                                                                                    UnidadCotizacionId = cotizacionSubposicion != null && cotizacionSubposicion.UnidadDeMedida != null ? cotizacionSubposicion.UnidadDeMedida.Id : 0,
                                                                                                    MonedaCotizacionDescripcion = cotizacionSubposicion != null && cotizacionSubposicion.Moneda != null ? cotizacionSubposicion.Moneda.Descripcion : "",
                                                                                                    MonedaCotizacionId = cotizacionSubposicion != null && cotizacionSubposicion.Moneda != null ? cotizacionSubposicion.Moneda.Id : 0,
                                                                                                    PrecioSubPosicion = cotizacionSubposicion != null ? cotizacionSubposicion.Precio.Value : 0,
                                                                                                    PrecioTotalSubPosicion = cotizacionSubposicion != null && cotizacionSubposicion.Cantidad != null && cotizacionSubposicion.Precio != null ? cotizacionSubposicion.Cantidad.Value * cotizacionSubposicion.Precio.Value : 0,
                                                                                                    MonedaCotizacionCodigo = cotizacionSubposicion != null && cotizacionSubposicion.Moneda != null ? cotizacionSubposicion.Moneda.Codigo : "",

                                                                                                }).ToList()
                                                                    },
                                                                }),


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
