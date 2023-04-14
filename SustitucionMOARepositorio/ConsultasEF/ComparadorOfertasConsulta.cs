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
                                where po.Id == PeticionOferta_Id
                                select new PeticionDeOfertaDto
                                {
                                    Id = po.Id,
                                    Solp_Id = po.Solp_Id,
                                    FechaCreacion = po.FechaCreacion,
                                    UsuarioCreador_Id = po.UsuarioCreador_Id,
                                    PlazoDeOferta = po.PlazoDeOferta,
                                    Observaciones = po.Observaciones,
                                    SolpDto = (from s in contexto.Set<Solp>()
                                               where po.Solp_Id == s.Id
                                               select new SolpDto()
                                               {
                                                   Id = s.Id,
                                                   FechaCreacion = s.FechaCreacion,
                                                   NroSolp = s.NroSolp,
                                               }),
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
                                                                        Codigo = pop.SolpPosicion.Codigo,
                                                                        Tarea = pop.SolpPosicion.Tarea,
                                                                        TextoSuministro = pop.SolpPosicion.TextoSuministro,
                                                                        Cantidad = pop.SolpPosicion.Cantidad,
                                                                        //Unidad = pop.SolpPosicion.Unidad.Descripcion,

                                                                        Subposiciones = pop.SolpPosicion.Subposiciones.Select(s => new SolpSubposicionDto
                                                                        {
                                                                            Id = s.Id,
                                                                            Codigo = s.Codigo,
                                                                            Tarea = s.Tarea,
                                                                            Cantidad = s.Cantidad,
                                                                            //Unidad = s.Unidad.Descripcion

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
                                                    //PropuestaTecnicaAprobada = u.PropuestaTecnicaAprobada,
                                                    //RealizoVisita = u.RealizoVisita,

                                                    Cotizacion = cotizacion != null ? new CotizacionDto()
                                                                 {
                                                                    Id = cotizacion.Id,
                                                                    PeticionDeOfertaUsuario_Id = cotizacion.PeticionDeOfertaUsuario_Id,
                                                                    FechaCreacion = u.Cotizaciones.FirstOrDefault().FechaCreacion,
                                                                    UsuarioCreador_Id = u.Cotizaciones.FirstOrDefault().UsuarioCreador_Id,
                                                                    CotizacionEstadoDescripcion = u.Cotizaciones.FirstOrDefault().CotizacionEstado.Descripcion,
                                                                    RespetaMateriales = u.Cotizaciones.FirstOrDefault().RespetaMateriales,
                                                                    RespetaServicios = u.Cotizaciones.FirstOrDefault().RespetaServicios,
                                                                    ObservacionEconomica = u.Cotizaciones.FirstOrDefault().ObservacionEconomica,
                                                                    ObservacionTecnica = u.Cotizaciones.FirstOrDefault().ObservacionTecnica,

                                                                    CotizacionPosiciones = cotizacion.CotizacionPosiciones.Select(p => new CotizacionPosicionDto
                                                                    {
                                                                        Id = p.Id,
                                                                        Cotizacion_Id = p.Cotizacion_Id,
                                                                        PeticionDeOfertaSolpPosicion_Id = p.PeticionDeOfertaSolpPosicion_Id,
                                                                        Cantidad = p.Cantidad,
                                                                        //UnidadMedida = p.UnidadDeMedida,
                                                                        Moneda_Id = p.Moneda_Id,
                                                                        FechaDeEntrega = p.FechaDeEntrega,
                                                                        Precio = p.Precio,
                                                                        CotizacionSubPosiciones = (from subpos in contexto.Set<CotizacionSubPosicion>()
                                                                                                   where p.Id == subpos.CotizacionPosicion_Id
                                                                                                   select new CotizacionSubPosicionDto()
                                                                                                   //p.CotizacionSubPosiciones == null ? null : p.CotizacionSubPosiciones.Select(s => new CotizacionSubPosicionDto
                                                                                                   {
                                                                                                        Id = subpos.Id,
                                                                                                        CotizacionPosicion_Id = subpos.CotizacionPosicion_Id,
                                                                                                        SolpSubPosicion_Id = subpos.SolpSubPosicion_Id,
                                                                                                        Cantidad = subpos.Cantidad,
                                                                                                        UnidadDeMedida_Id = subpos.UnidadDeMedida_Id,
                                                                                                        Moneda_Id = subpos.Moneda_Id,
                                                                                                        Precio = subpos.Precio
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
