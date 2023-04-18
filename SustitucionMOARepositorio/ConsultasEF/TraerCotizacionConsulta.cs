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
            var hoy = DateTime.Now;
            var ayer = hoy.AddDays(-1);
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
                                    Pagina = 224,
                                    ObservacionTecnica = cotizacion != null ? cotizacion.ObservacionTecnica : "",
                                    ObservacionEconomica = cotizacion != null ? cotizacion.ObservacionEconomica : "",
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
                                                                        Codigo = pop.SolpPosicion.Codigo, //Codigo
                                                                        Tarea = pop.SolpPosicion.Tarea,
                                                                        TextoSuministro = pop.SolpPosicion.TextoSuministro,
                                                                        Cantidad = pop.SolpPosicion.Cantidad,
                                                                        UnidadComprasDescripcion = pop.SolpPosicion.Unidad.Descripcion,
                                                                        UnidadId = pop.SolpPosicion.Unidad_Id,
                                                                        CotizacionPosicion = new CotizacionPosicionDto()
                                                                        {
                                                                            Cantidad = cotizacion != null && cotizacionPosicion.Cantidad != null ? cotizacionPosicion.Cantidad.Value : 0,
                                                                            UnidadMedidaDescripcion = cotizacion != null && cotizacionPosicion.UnidadDeMedida != null ? cotizacionPosicion.UnidadDeMedida.Descripcion : "",
                                                                            UnidadDeMedida_Id = cotizacion != null && cotizacionPosicion.UnidadDeMedida != null ? cotizacionPosicion.UnidadDeMedida.Id : 0,
                                                                            MonedaDescripcion = cotizacion != null && cotizacionPosicion.Moneda != null ? cotizacionPosicion.Moneda.Descripcion : "",
                                                                            MonedaCodigo = cotizacion != null && cotizacionPosicion.Moneda != null ? cotizacionPosicion.Moneda.Codigo : "",
                                                                            Moneda_Id = cotizacion != null && cotizacionPosicion.Moneda_Id != null ? cotizacionPosicion.Moneda_Id.Value : 0,
                                                                            Precio = cotizacion != null && cotizacionPosicion.Precio != null ? cotizacionPosicion.Precio.Value : 0,
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

                                                                        Subposiciones = pop.SolpPosicion.Subposiciones.Select(s => new SolpSubposicionDto
                                                                        {
                                                                            Id = s.Id,
                                                                            Codigo = s.Codigo,
                                                                            Tarea = s.Tarea,
                                                                            Cantidad = s.Cantidad,
                                                                            UnidadDescripcion = s.Unidad.Descripcion,
                                                                            Numero = s.Numero,

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
