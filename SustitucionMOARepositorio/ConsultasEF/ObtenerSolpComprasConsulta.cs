using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using System;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace SustitucionMOARepositorio.ConsultasEF
{
    public class ObtenerSolpCompras : IConsultaEscalar<SolpCompraDto>
    {
        private readonly int Id;
        public ObtenerSolpCompras(int id)
        {
            this.Id = id;
        }
        private static SolpCompraDto Query(DbContext contexto, int id)
        {
            try
            {
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
                var resultado = (from solp in contexto.Set<Solp>()
                                 where solp.Id == id
                                 select new SolpCompraDto
                                 {
                                     Id = solp.Id,
                                     NroSolp = solp.NroSolp,
                                     TipoPosicionCodigo = solp.Posiciones.Select(x => x.TipoPosicion.Codigo).FirstOrDefault(),
                                     PosicionCompras = (from posicion in contexto.Set<SolpPosicion>()
                                                        where posicion.Solp_Id == solp.Id && posicion.EsConcluido == true && posicion.Estado == true
                                                        orderby posicion.Indice
                                                        select new SolpPosicionDto()
                                                        {
                                                            Id = posicion.Id,
                                                            MaterialComprasCodigo = posicion.MaterialSolp.Codigo,
                                                            TieneCotizacion = posicion.Peticiones.Any(),
                                                            Indice = posicion.Indice,
                                                            Tarea = posicion.Tarea,
                                                            CentroComprasDescripcion = posicion.Centro.Descripcion,
                                                            AlmacenComprasDescripcion = posicion.Almacen.Descripcion,
                                                            TextoSuministro = posicion.TextoSuministro,
                                                            Modelo = posicion.Modelo,
                                                            GrupoComprasDescripcion = posicion.GrupoCompras.Codigo + " " + posicion.GrupoCompras.Descripcion,
                                                            Cantidad = posicion.Cantidad,
                                                            UnidadComprasDescripcion = posicion.Unidad.Descripcion,
                                                            MonedaComprasDescripcion = posicion.Moneda.Descripcion,
                                                            FechaEntregaServicio = posicion.FechaEntregaServicio,
                                                            FechaOferta = posicion.Solp.Pliego_Id != null ? posicion.Solp.Pliego.FechaHoraEntrega : (DateTime?)null,
                                                            PlazoEntrega = posicion.PlazoEntrega,
                                                            ProveedoresCompras = (from solpProveedor in contexto.Set<SolpProveedor>()
                                                                                  where solpProveedor.SolpPosicion_Id == posicion.Id
                                                                                  select new SolpProveedorDto()
                                                                                  {
                                                                                      SolpPosicionId = posicion.Id,
                                                                                      TipoFiltroProveedorSolpCodigo = solpProveedor.TipoFiltroProveedorSolp.Codigo,
                                                                                      RazonSocial = solpProveedor.RazonSocial
                                                                                  }),
                                                            SubposicionesCompras = (from subPosicion in contexto.Set<SolpSubposicion>()
                                                                                    where subPosicion.SolpPosicion_Id == posicion.Id
                                                                                    orderby subPosicion.Numero
                                                                                    select new SolpSubposicionDto()
                                                                                    {
                                                                                        Numero = subPosicion.Numero,
                                                                                        Tarea = subPosicion.Tarea,
                                                                                        Codigo = subPosicion.Codigo,
                                                                                        Cantidad = subPosicion.Cantidad,
                                                                                        UnidadComprasDescripcion = subPosicion.Unidad.Descripcion
                                                                                    }),
                                                        }),

                                 }).First();


                return resultado;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        SolpCompraDto IConsultaEscalar<SolpCompraDto>.Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, Id);
            }
        }
    }
}
