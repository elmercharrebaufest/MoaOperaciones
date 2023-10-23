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
        private readonly int PeticionDeOfertaUsuario_Id;
        public TraerCotizacionConsulta(int peticionDeOfertaUsuario_Id)
        {
            this.PeticionDeOfertaUsuario_Id = peticionDeOfertaUsuario_Id;
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
                                    Solp_Id = po.PeticionDeOferta.Solp_Id,
                                    PersonalHoras = po.PeticionDeOferta.Solp.Pliego != null ? po.PeticionDeOferta.Solp.Pliego.TieneGrillaPersonal ?? false : false,
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
                                    PorcentajeDeHoras = cotizacion.PorcentajeDeHoras,
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
                                            //TextoSuministro = pop.SolpPosicion.TextoSuministro,
                                            Cantidad = pop.SolpPosicion.Cantidad,
                                            UnidadComprasDescripcion = pop.SolpPosicion.Unidad.Descripcion,
                                            UnidadId = pop.SolpPosicion.Unidad_Id,
                                            FechaEntregaServicio = cotizacion != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().FechaDeEntrega : pop.SolpPosicion.FechaEntregaServicio,
                                            FechaOferta = pop.SolpPosicion.Solp.Pliego_Id != null ? pop.SolpPosicion.Solp.Pliego.FechaHoraEntrega : (DateTime?)null,
                                            CotizacionPosicion = new CotizacionPosicionDto()
                                            {
                                                Cantidad = cotizacion != null && cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().Cantidad != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().Cantidad.Value : 0,
                                                UnidadMedidaDescripcion = cotizacion != null && cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().UnidadDeMedida != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().UnidadDeMedida.Descripcion : "",
                                                UnidadDeMedida_Id = cotizacion != null && cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().UnidadDeMedida != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().UnidadDeMedida.Id : 0,
                                                MonedaDescripcion = cotizacion != null && cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().Moneda != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().Moneda.Descripcion : "",
                                                MonedaCodigo = cotizacion != null && cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().Moneda != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().Moneda.Codigo : "",
                                                Moneda_Id = cotizacion != null && cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().Moneda_Id != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().Moneda_Id.Value : 0,
                                                Precio = cotizacion != null && cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().Precio != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().Precio.Value : 0,
                                                FechaOriginal = cotizacion != null && cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().FechaDeEntrega != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().FechaDeEntrega : (DateTime?)null,
                                                FechaDeEntrega = cotizacion != null && cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().FechaDeEntrega != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().FechaDeEntrega : (DateTime?)null,
                                                FechaDeEntregaFormateado = cotizacion != null && cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().FechaDeEntrega != null ? SqlFunctions.DateName("day", cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().FechaDeEntrega) + "/" + SqlFunctions.DatePart("month", cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().FechaDeEntrega) + "/" + SqlFunctions.DateName("year", cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().FechaDeEntrega) : "",
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
                                                    Codigo = cotizacion != null && cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().UnidadDeMedida_Id != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().UnidadDeMedida.Codigo : "",
                                                    Id = cotizacion != null && cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().UnidadDeMedida_Id != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().UnidadDeMedida_Id.Value : 0
                                                },
                                                NoDisponible = cotizacion != null ? cotizacion.CotizacionPosiciones.Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id).FirstOrDefault().NoDisponible : null,
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

                                                CotizacionSubPosicionId = cotizacion == null && cotizacion.CotizacionPosiciones
                                                .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)
                                                ).FirstOrDefault().CotizacionSubPosiciones.Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault() == null ? 0 :
                                                cotizacion.CotizacionPosiciones
                                                .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)
                                                ).FirstOrDefault().CotizacionSubPosiciones.Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault().Id,

                                                CantidadCotizacion = cotizacion == null && cotizacion.CotizacionPosiciones
                                                .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)
                                                ).FirstOrDefault().CotizacionSubPosiciones.Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault().Cantidad == null ?
                                                0 : cotizacion.CotizacionPosiciones
                                                .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)
                                                ).FirstOrDefault().CotizacionSubPosiciones.Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault().Cantidad.Value,


                                                UnidadCotizacionDescripcion = cotizacion == null && cotizacion.CotizacionPosiciones
                                                 .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                 && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)
                                                 ).FirstOrDefault().CotizacionSubPosiciones.Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault().UnidadDeMedida_Id == null ?
                                                "" : cotizacion.CotizacionPosiciones
                                                 .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                 && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)
                                                 ).FirstOrDefault().CotizacionSubPosiciones.Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault().UnidadDeMedida.Descripcion,

                                                UnidadCotizacionId = cotizacion == null && cotizacion.CotizacionPosiciones
                                                 .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                 && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)
                                                 ).FirstOrDefault().CotizacionSubPosiciones.Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault().UnidadDeMedida_Id == null ?
                                                 0 : cotizacion.CotizacionPosiciones
                                                 .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                 && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)
                                                 ).FirstOrDefault().CotizacionSubPosiciones.Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault().UnidadDeMedida.Id,

                                                MonedaCotizacionId = cotizacion == null && cotizacion.CotizacionPosiciones
                                                 .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                 && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)
                                                 ).FirstOrDefault().CotizacionSubPosiciones.Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault().Moneda == null ?
                                                0 : cotizacion.CotizacionPosiciones
                                                 .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                 && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)
                                                 ).FirstOrDefault().CotizacionSubPosiciones.Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault().Moneda.Id,
                                                MonedaCotizacionDescripcion = cotizacion == null && cotizacion.CotizacionPosiciones
                                                 .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                 && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)
                                                 ).FirstOrDefault().CotizacionSubPosiciones.Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault().Moneda == null ?
                                                 "" : cotizacion.CotizacionPosiciones
                                                 .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                 && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)
                                                 ).FirstOrDefault().CotizacionSubPosiciones.Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault().Moneda.Descripcion,
                                                MonedaCotizacionCodigo = cotizacion == null && cotizacion.CotizacionPosiciones
                                                 .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                 && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)
                                                 ).FirstOrDefault().CotizacionSubPosiciones.Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault().Moneda == null ?
                                                 "" : cotizacion.CotizacionPosiciones
                                                 .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                 && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)
                                                 ).FirstOrDefault().CotizacionSubPosiciones.Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault().Moneda.Codigo,

                                                MonedaCotizacion = new TablaSapDto
                                                {
                                                    Codigo = cotizacion == null && cotizacion.CotizacionPosiciones
                                                 .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                 && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)
                                                 ).FirstOrDefault().CotizacionSubPosiciones.Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault().Moneda == null ?
                                                 "" : cotizacion.CotizacionPosiciones
                                                 .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                 && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)
                                                 ).FirstOrDefault().CotizacionSubPosiciones.Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault().Moneda.Codigo,
                                                    
                                                    Id = cotizacion == null && cotizacion.CotizacionPosiciones
                                                 .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                 && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)
                                                 ).FirstOrDefault().CotizacionSubPosiciones.Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault().Moneda == null ?
                                                0 : cotizacion.CotizacionPosiciones
                                                 .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                 && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)
                                                 ).FirstOrDefault().CotizacionSubPosiciones.Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault().Moneda_Id??0,

                                                },
                                                UnidadMedidaCotizacion = new TablaSapDto
                                                {
                                                    Codigo = cotizacion == null && cotizacion.CotizacionPosiciones
                                                 .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                 && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)
                                                 ).FirstOrDefault().CotizacionSubPosiciones.Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault().UnidadDeMedida_Id == null ?
                                                 "" : cotizacion.CotizacionPosiciones
                                                 .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                 && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)
                                                 ).FirstOrDefault().CotizacionSubPosiciones.Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault().UnidadDeMedida.Codigo,

                                                    Id = cotizacion == null && cotizacion.CotizacionPosiciones
                                                 .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                 && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)
                                                 ).FirstOrDefault().CotizacionSubPosiciones.Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault().UnidadDeMedida_Id == null ?
                                                 0 : cotizacion.CotizacionPosiciones
                                                 .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                 && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)
                                                 ).FirstOrDefault().CotizacionSubPosiciones.Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault().UnidadDeMedida_Id??0,
                                                },

                                                PrecioSubPosicion = cotizacion == null && cotizacion.CotizacionPosiciones
                                                 .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                 && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)
                                                 ).FirstOrDefault().CotizacionSubPosiciones.Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault().Precio == null ?
                                                 0 : cotizacion.CotizacionPosiciones
                                                 .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                 && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)
                                                 ).FirstOrDefault().CotizacionSubPosiciones.Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault().Precio.Value,

                                                PrecioTotalSubPosicion = cotizacion == null && cotizacion.CotizacionPosiciones
                                                 .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                 && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)
                                                 ).FirstOrDefault().CotizacionSubPosiciones.Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault().Precio == null &&
                                                 cotizacion.CotizacionPosiciones
                                                 .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                 && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)
                                                 ).FirstOrDefault().CotizacionSubPosiciones.Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault().Cantidad == null ?
                                                 0 : cotizacion.CotizacionPosiciones
                                                 .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                 && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)
                                                 ).FirstOrDefault().CotizacionSubPosiciones.Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault().Precio.Value *
                                                  cotizacion.CotizacionPosiciones
                                                 .Where(cp => cp.PeticionDeOfertaSolpPosicion_Id == pop.Id
                                                 && cp.CotizacionSubPosiciones.Any(s => s.SolpSubPosicion_Id == subposicion.Id)
                                                 ).FirstOrDefault().CotizacionSubPosiciones.Where(s => s.SolpSubPosicion_Id == subposicion.Id).FirstOrDefault().Cantidad.Value,


                                            }).ToList().OrderBy(x => x.Numero),
                                        }
                                    }).ToList().OrderBy(x => x.Posiciones.Indice)
                                };

                var cc = resultado.First();
                return cc;
            }
            catch (Exception ex)
            {
               throw;
            }
        }


    }
}
