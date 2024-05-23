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
    public class ListarSolpCondicionEspecialConsulta : IConsultaPaginada<SolpDto>
    {
        private readonly FiltroServiceDto filtro;

        public ListarSolpCondicionEspecialConsulta(FiltroServiceDto filtro)
        {
            this.filtro = filtro;
        }
        public ListaPaginada<SolpDto> Ejecutar(DbContext contexto)
        {
            try
            {
                ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

                var resultado = from solp in contexto.Set<Solp>()
                                where 
                                (solp.EstadoSolpSap.CodigoSap == "05" || solp.EstadoSolpSap.CodigoSap == "02") &&
                                      solp.Posiciones.All(x => x.Peticiones.Any()) &&
                                      (solp.TrabajoYaHecho == true || (solp.TrabajoYaHecho == true && solp.Adicional == true)) &&
                                      (!filtro.Centros.Any() || solp.Posiciones.Any(c => filtro.Centros.Contains(c.Centro_Id))) &&
                                      (!filtro.GrupoDeCompras.Any() || solp.Posiciones.Any(gc => filtro.GrupoDeCompras.Contains((int)gc.GrupoCompras_Id))) &&
                                      (filtro.Sap && solp.TipoSolpSap == 3 || filtro.Mantenimiento && solp.TipoSolpSap == 2 || filtro.RepoAutomatica && solp.TipoSolpSap == 4 ||
                                        (filtro.Web && (solp.TipoSolpSap == null || solp.TipoSolpSap == 1)) || (!filtro.Sap && !filtro.Mantenimiento && !filtro.Web && !filtro.RepoAutomatica)) &&
                                      (filtro.FechaDesde == null || solp.FechaCreacion >= filtro.FechaDesde.Value) && 
                                      (filtro.FechaHasta == null || solp.FechaCreacion <= filtro.FechaHasta.Value) &&
                                      (string.IsNullOrEmpty(filtro.CodigoProveedor) || solp.ProveedorAsignado.Proveedores.Any(p => p.CodigoProveedor.Contains(filtro.CodigoProveedor))) &&
                                      (!filtro.ClaseDocumento.Any() || solp.EstadoSolpSap_Id != null && filtro.ClaseDocumento.Contains((int)solp.ClaseDocumento_Id)) &&
                                      (!filtro.NombrePedido.Any() || filtro.NombrePedido.All(p => solp.Pliego.NombreObra.Contains(p.ToUpper()))) &&
                                      (!filtro.TipoImputacion.Any() || solp.Posiciones.Any(c => filtro.TipoImputacion.Contains(c.TipoImputacion.Codigo))) &&
                                      (!filtro.ValorTipoImputacion.Any() || solp.Posiciones.Any(c => filtro.ValorTipoImputacion.Contains((int)c.ValorTipoImputacion_Id)) ||
                                        solp.Posiciones.Any(p => p.Subposiciones.Any(c => filtro.ValorTipoImputacion.Contains((int)c.TipoImputacion_Id)))) &&
                                      (filtro.EsServicio ? solp.Posiciones.Any(p => p.TipoPosicion.Codigo == "SERVICIO") : solp.Posiciones.Any(p => p.TipoPosicion.Codigo != "SERVICIO")) && 
                                      (filtro.Agrupada != null ? solp.Posiciones.FirstOrDefault().Peticiones.Any(x => x.PeticionDeOferta.Agrupada == filtro.Agrupada) : true) &&
                                      (!solp.Posiciones.FirstOrDefault().Peticiones.Any(x => x.SolpPosicion.AdjudicacionPosiciones.Any())) &&
                                      solp.Posiciones.All(posi => string.IsNullOrEmpty(posi.NumeroContratoSuperior))
                                select new SolpDto
                                {
                                    UsuarioActual = new UsuarioDto { Mail = solp.UsuarioCreacion != null ? solp.UsuarioCreacion.Mail : "" },
                                    Id = solp.Id,
                                    NroSolp = solp.NroSolp,
                                    NombreDeObra = solp.Pliego == null ? "" : solp.Pliego.NombreObra,
                                    FechaCreacionFormateada = SqlFunctions.DateName("day", solp.FechaCreacion) + "/" + SqlFunctions.DatePart("month", solp.FechaCreacion) + "/" + SqlFunctions.DateName("year", solp.FechaCreacion),
                                    FechaCreacion = solp.FechaCreacion,
                                    TipoSolp = new TablaGeneralDto { Descripcion = solp.TipoSolp != null ? solp.TipoSolp.Descripcion : "", Codigo = solp.TipoSolp != null ? solp.TipoSolp.Codigo : "" },
                                    TipoSolpSap = solp.TipoSolpSap,
                                    Adicional = solp.Adicional,
                                    NroOrdenDeCompraAdicional = solp.NroOrdenDeCompraAdicional,
                                    TrabajoYaHecho = solp.TrabajoYaHecho,
                                    ProveedorAsignadoCuit = solp.ProveedorAsignado.CUITRegistro,
                                    ProveedorAsignadoRazonSocial = solp.ProveedorAsignado.Proveedores.Any() ? solp.ProveedorAsignado.Proveedores.FirstOrDefault().RazonSocial ?? "No definido" : "",
                                    ItemPorPagina = filtro.Paginacion.ItemsPorPagina,
                                    Pagina = filtro.Paginacion.Pagina,
                                    EstadoSolpSap = solp.EstadoSolpSap_Id != null ? new TablaSapDto { Id = solp.EstadoSolpSap_Id ?? 0, CodigoSap = solp.EstadoSolpSap.CodigoSap, Descripcion = solp.EstadoSolpSap.Descripcion } : new TablaSapDto { Id = 0, CodigoSap = "", Descripcion = "" },
                                    NroPeticionDeOferta = solp.Posiciones.All(x => x.Peticiones.Any()) ? solp.Posiciones.FirstOrDefault().Peticiones.FirstOrDefault().PeticionDeOferta.Id : 0,
                                    Agrupada = solp.Posiciones.FirstOrDefault().Peticiones.FirstOrDefault().PeticionDeOferta.Agrupada ?? false,
                                    TipoPosicionCodigo = solp.Posiciones.Select(posiciones => posiciones.TipoPosicion.Codigo).FirstOrDefault(),
                                    PosicionCompras = (from posicion in contexto.Set<SolpPosicion>()
                                                       where posicion.Solp_Id == solp.Id 
                                                       select new SolpPosicionDto()
                                                       {
                                                           Id = posicion.Id,
                                                           Codigo = posicion.MaterialSolp.Codigo,
                                                           Indice = posicion.Indice,
                                                           Tarea = posicion.Tarea,
                                                           CentroComprasDescripcion = posicion.Centro.Descripcion,
                                                           CentroCodigoSap = posicion.Centro.CodigoSap,
                                                           AlmacenComprasDescripcion = posicion.Almacen.Descripcion,
                                                           TextoSuministro = posicion.TextoSuministro,
                                                           Modelo = posicion.Modelo,
                                                           GrupoComprasDescripcion = posicion.GrupoCompras.Descripcion,
                                                           GrupoComprasCodigoSap = posicion.GrupoCompras.CodigoSap,
                                                           MaterialComprasCodigo = posicion.MaterialSolp.CodigoSap,
                                                           MaterialDescripcion = posicion.MaterialSolp.Descripcion,
                                                           Cantidad = posicion.Cantidad,
                                                           UnidadComprasDescripcion = posicion.Unidad.Descripcion,
                                                           MonedaSolpDescripcion = posicion.Moneda.Descripcion,
                                                           FechaEntregaServicio = posicion.FechaEntregaServicio,
                                                           FechaOferta = posicion.Solp.Pliego_Id != null ? posicion.Solp.Pliego.FechaHoraEntrega : (DateTime?)null,
                                                           PlazoEntrega = posicion.PlazoEntrega,
                                                           SubposicionesCompras = (from subPosicion in contexto.Set<SolpSubposicion>()
                                                                                   where subPosicion.SolpPosicion_Id == posicion.Id
                                                                                   orderby subPosicion.Numero
                                                                                   select new SolpSubposicionDto()
                                                                                   {
                                                                                       Numero = subPosicion.Numero,
                                                                                       Tarea = subPosicion.Tarea,
                                                                                       Codigo = subPosicion.ServicioSolp.Codigo,
                                                                                       Cantidad = subPosicion.Cantidad,
                                                                                       UnidadComprasDescripcion = subPosicion.Unidad.Descripcion,
                                                                                       PrecioBruto = subPosicion.PrecioBruto,
                                                                                       ValorNeto = subPosicion.Cantidad * subPosicion.PrecioBruto
            }),
                                                       }),
                                };

                return resultado.OrdenarPaginarLista(filtro.Paginacion);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}