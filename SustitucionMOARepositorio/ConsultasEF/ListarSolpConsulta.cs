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
    public class ListarSolpConsulta : IConsultaPaginada<SolpDto>
    {
        private readonly Paginacion paginacion;
        private readonly string NroSolp;
        public ListarSolpConsulta(Paginacion paginacion, string nroSolp)
        {
            this.paginacion = paginacion;
            this.NroSolp = nroSolp;


        }
        public ListaPaginada<SolpDto> Ejecutar(DbContext contexto)
        {
            var hoy = DateTime.Now;
            var ayer = hoy.AddDays(-1);
            try
            {
                ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
                var resultado = from x in contexto.Set<Solp>()
                                where (string.IsNullOrEmpty(NroSolp) || x.NroSolp.ToUpper().StartsWith(NroSolp.ToUpper())) &&
                                x.Posiciones.All(p => p.NumeroContratoSuperior == "" || p.NumeroContratoSuperior == null)
                                select new SolpDto
                                {
                                    UsuarioActual = new UsuarioDto { Mail = x.UsuarioCreacion != null ? x.UsuarioCreacion.Mail : "" },
                                    Id = x.Id,
                                    NroSolp = x.NroSolp,
                                    NombreDeObra = x.Pliego == null ? "" : x.Pliego.NombreObra,
                                    FechaCreacionFormateada = SqlFunctions.DateName("day", x.FechaCreacion) + "/" + SqlFunctions.DatePart("month", x.FechaCreacion) + "/" + SqlFunctions.DateName("year", x.FechaCreacion),
                                    FechaCreacion = x.FechaCreacion,
                                    TipoSolp = new TablaGeneralDto { Descripcion = x.TipoSolp != null ? x.TipoSolp.Descripcion : "" },
                                    TipoSolpSap = x.TipoSolpSap,
                                    PosicionCompras = (from posicion in contexto.Set<SolpPosicion>()
                                                       where posicion.Solp_Id == x.Id
                                                       select new SolpPosicionDto()
                                                       {
                                                           GrupoComprasDescripcion = posicion.GrupoCompras != null ? posicion.GrupoCompras.Descripcion : "",
                                                           CentroComprasDescripcion = posicion.Centro != null ? posicion.Centro.Descripcion : "",
                                                           GrupoComprasId = posicion.GrupoCompras != null ? posicion.GrupoCompras.Id : (int?)null,
                                                           CentroId = posicion.Centro != null ? posicion.Centro.Id : (int?)null
                                                       }),
                                    ItemPorPagina = paginacion.ItemsPorPagina,
                                    Pagina = paginacion.Pagina,
                                    EstadoSolpSap = x.EstadoSolpSap_Id != null ? new TablaSapDto { Id = x.EstadoSolpSap_Id ?? 0, CodigoSap = x.EstadoSolpSap.CodigoSap, Descripcion = x.EstadoSolpSap.Descripcion } : new TablaSapDto { Id = 0, CodigoSap = "", Descripcion = "" },
                                    FechaLiberacionSapFormateada = x.FechaLiberacionSap == null ? "" : SqlFunctions.DateName("day", x.FechaLiberacionSap) + "/" + SqlFunctions.DatePart("month", x.FechaLiberacionSap) + "/" + SqlFunctions.DateName("year", x.FechaLiberacionSap),
                                    FechaLiberacionSap = x.FechaLiberacionSap,
                                    PeticionesDeOferta = (from po in contexto.Set<PeticionDeOferta>()
                                                          where po.Solp_Id == x.Id
                                                          select new PeticionDeOfertaDto()
                                                          {
                                                              Id = po.Id,
                                                              Solp_Id = po.Solp_Id,
                                                              FechaCreacion = po.FechaCreacion,
                                                              UsuarioCreador_Id = po.UsuarioCreador_Id,
                                                              PlazoDeOferta =
                                                              po.Usuarios.GroupBy(p => p).SelectMany(p => p.Key.Circulares)
                                                               .Any(p => p.Circular.RequiereCambioDeFechas == true && p.Circular.PlazoDeOferta.HasValue) ?
                                                               po.Usuarios.GroupBy(p => p).SelectMany(p => p.Key.Circulares)
                                                               .Where(p => p.Circular.RequiereCambioDeFechas == true && p.Circular.PlazoDeOferta.HasValue)
                                                               .OrderByDescending(p => p.Circular.PlazoDeOferta).FirstOrDefault().Circular.PlazoDeOferta.Value :
                                                                po.PlazoDeOferta,
                                                              Observaciones = po.Observaciones,
                                                          }),
                                    OrdenesDeCompra = (from adjudicacion in contexto.Set<Adjudicacion>()
                                                        where adjudicacion.Solp_Id == x.Id
                                                        select new AdjudicacionDto()
                                                        {
                                                            Id = adjudicacion.Id,
                                                            NumeroOrdenDeCompra = adjudicacion.NumeroOrdenDeCompra,
                                                            FechaCreacion = adjudicacion.FechaCreacion,
                                                            Proveedor = adjudicacion.Cotizacion.PeticionDeOfertaUsuario.Usuario.Proveedores.Count > 0 ?
                                                            adjudicacion.Cotizacion.PeticionDeOfertaUsuario.Usuario.Proveedores.FirstOrDefault().RazonSocial : "",
                                                            MonedaDescripcion = adjudicacion.Moneda.Descripcion,
                                                            PrecioFinal = adjudicacion.MontoTotal,
                                                            AdjudicacionPosiciones = adjudicacion.Posiciones.Select(posicion => new AdjudicacionPosicionDto
                                                            {
                                                             SolpPosicion_Id = posicion.SolpPosicion_Id,
                                                            Id = posicion.Id,
                                                            MaterialComprasCodigo = posicion.Posicion.MaterialSolp.Codigo,
                                                            Indice = posicion.Posicion.Indice,
                                                            Tarea = posicion.Posicion.Tarea,
                                                            TextoSuministro = posicion.Posicion.TextoSuministro,
                                                            Modelo = posicion.Posicion.Modelo,
                                                            Cantidad = adjudicacion.Solp.Posiciones.Select(posicionSolp => posicionSolp.TipoPosicion.Codigo).FirstOrDefault() == "MATERIALES" ? posicion.Cantidad : 1,   
                                                            PrecioUnidad = posicion.CotizacionPosicion.Precio,
                                                            MonedaId = posicion.CotizacionPosicion.Moneda_Id,
                                                            UnidadDescripcion = posicion.CotizacionPosicion.UnidadDeMedida.Descripcion,
                                                            MonedaDescripcion = posicion.CotizacionPosicion.Moneda.CodigoSap,
                                                            PrecioTotal = posicion.Cantidad * posicion.CotizacionPosicion.Precio, 
                                                            SubposicionesCompras = posicion.CotizacionPosicion.CotizacionSubPosiciones.Select(subpos => 
                                                                                        new SolpSubposicionDto()
                                                                                        {
                                                                                            Numero = subpos.SolpSubPosicion.Numero,
                                                                                            Tarea = subpos.SolpSubPosicion.Tarea,
                                                                                            Codigo = subpos.SolpSubPosicion.Codigo,
                                                                                            Cantidad = subpos.Cantidad,
                                                                                            PrecioBruto = subpos.Precio,
                                                                                            UnidadComprasDescripcion = subpos.UnidadDeMedida.Descripcion,
                                                                                            MonedaCotizacionDescripcion = !posicion.CotizacionPosicion.CotizacionSubPosiciones.Select(moneda => moneda.Moneda_Id).GroupBy(m => m).Any(g => g.Count() > 1) ? subpos.Moneda.Descripcion : "Error",
                                                                                            PrecioTotalSubPosicion = !posicion.CotizacionPosicion.CotizacionSubPosiciones.Select(moneda => moneda.Moneda_Id).GroupBy(m => m).Any(g => g.Count() > 1) ?
                                                                                            (subpos.Cantidad.Value * subpos.Precio.Value) : 0,
                                                                                        }).ToList(),
                                                            }).ToList(),
                                                        }),
                                };

                var itemsTotales = resultado.Count();

                return resultado.OrdenarPaginarLista(paginacion);

            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}
