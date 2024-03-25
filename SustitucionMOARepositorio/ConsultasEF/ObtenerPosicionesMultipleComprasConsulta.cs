using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace SustitucionMOARepositorio.ConsultasEF
{
    public class ObtenerPosicionesMultipleComprasConsulta : IConsultaEscalar<SolpCompraDto>
    {
        private readonly List<int> Ids;
        public ObtenerPosicionesMultipleComprasConsulta(List<int> ids)
        {
            this.Ids = ids;
        }
        private static SolpCompraDto Query(DbContext contexto, List<int> ids)
        {
            try
            {
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
                var resultado = (from posicion in contexto.Set<SolpPosicion>()
                                 where ids.Contains(posicion.Id) && posicion.EsConcluido == true && posicion.Estado == true
                                    orderby posicion.Solp_Id // posicion.Indice
                                    select new SolpPosicionDto()
                                    {
                                        Id = posicion.Id,
                                        NroSolp = posicion.Solp.NroSolp,
                                        TipoPosicionCodigo = posicion.TipoPosicion.Codigo,
                                        TieneCotizacion = posicion.Peticiones.Any(),
                                        Codigo = posicion.MaterialSolp.Codigo,
                                        Indice = posicion.Indice,
                                        Tarea = posicion.Tarea,
                                        CentroComprasDescripcion = posicion.Centro.Descripcion,
                                        Centro = new TablaSapDto
                                        {
                                            CodigoSap = posicion.Centro.CodigoSap,
                                            Descripcion = posicion.Centro.Descripcion,
                                        },
                                        AlmacenComprasDescripcion = posicion.Almacen.Descripcion,
                                        TextoSuministro = posicion.TextoSuministro,
                                        Modelo = posicion.Modelo,
                                        GrupoComprasDescripcion = posicion.GrupoCompras.Codigo + " " + posicion.GrupoCompras.Descripcion,
                                        GrupoCompras = new TablaSapDto
                                        {
                                            CodigoSap = posicion.GrupoCompras.CodigoSap,
                                            Descripcion = posicion.GrupoCompras.Descripcion,
                                        },
                                        MaterialComprasCodigo = posicion.MaterialSolp.CodigoSap,
                                        Cantidad = posicion.Cantidad,
                                        UnidadComprasDescripcion = posicion.Unidad.Descripcion,
                                        MonedaSolpDescripcion = posicion.Moneda.Descripcion,
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
                                                                })
                                        
                                    });

                SolpCompraDto solpCompra = new SolpCompraDto();
                solpCompra.PosicionCompras = resultado;
                return solpCompra;
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
                return Query(contexto, Ids);
            }
        }
    }
}
