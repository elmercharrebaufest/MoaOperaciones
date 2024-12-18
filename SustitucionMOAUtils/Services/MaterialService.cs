using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Helpers;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using WSMapMOAModel = SustitucionMOAModel.Models.WSMapMOA.Compras;

namespace SustitucionMOAUtils.Services
{
    public class MaterialService : IMaterialService
    {
        private readonly IRepositorio repositorio;
        private readonly IObtenerMaterialesSolpConsumerMOA obtenerMaterialesSolpConsumerMOA;

        public MaterialService(IRepositorio repositorio,
                               IObtenerMaterialesSolpConsumerMOA obtenerMaterialesSolpConsumerMOA)
        {
            this.repositorio = repositorio;
            this.obtenerMaterialesSolpConsumerMOA = obtenerMaterialesSolpConsumerMOA;
        }

        public void ActualizarMaterialesSolp()
        {
            List<TablaSap> centros = repositorio.Listar<TablaSap>(x => x.Tabla == "Centro");
            const string materialFiltro = "";

            List<string> centrosLista = centros.ConvertAll(a => a.CodigoSap);

            WSMapMOAModel.MaterialWSMOAResponse resultSap = obtenerMaterialesSolpConsumerMOA.request(centrosLista, materialFiltro);

            List<WSMapMOAModel.Material> Materiales = resultSap.Materiales;

            if (Materiales.Any())
            {
                List<MaterialSolp> listaBase = repositorio.Listar<MaterialSolp>();

                List<string> tablasSapAConsultar = new List<string>
                {
                    TablasSap.Centro,
                    TablasSap.GrupoArticulo,
                    TablasSap.Unidad,
                    TablasSap.GrupoCompras,
                    TablasSap.CuentasSolpSap
                };

                List<TablaSap> tablaSap = repositorio.Listar<TablaSap>(x => tablasSapAConsultar.Contains(x.Tabla));
                List<int> idsActualizados = new List<int>();
                List<TablaSap> centro = tablaSap.Where(x => x.Tabla == TablasSap.Centro).ToList();
                List<TablaSap> grupoArticulo = tablaSap.Where(x => x.Tabla == TablasSap.GrupoArticulo).ToList();
                List<TablaSap> unidad = tablaSap.Where(x => x.Tabla == TablasSap.Unidad).ToList();
                List<TablaSap> grupoCompras = tablaSap.Where(x => x.Tabla == TablasSap.GrupoCompras).ToList();
                List<TablaSap> cuentas = tablaSap.Where(x => x.Tabla == TablasSap.CuentasSolpSap).ToList();
                List<UnidadMedidaSap> unidadMedidasSap = repositorio.Listar<UnidadMedidaSap>();

                int contador = 0;
                int agregados = 0;
                foreach (var material in Materiales)
                {
                    try
                    {
                        int? centroId = centro.Find(x => x.CodigoSap == material.CentroLogistico)?.Id;
                        MaterialSolp item = listaBase.Find(x => x.CodigoSap == material.NroMaterial && x.Centro_Id == centroId);

                        contador++;
                        if (item == null)
                        {
                            agregados++;
                            repositorio.Agregar(new MaterialSolp
                            {
                                Centro_Id = centroId,
                                Codigo = material.NroMaterial,
                                CodigoSap = material.NroMaterial,
                                Descripcion = material.NombreDeMaterial,
                                GrupoArticulo_Id = grupoArticulo.Find(x => x.CodigoSap == material.GrupoArticulo)?.Id,
                                TipoMaterial = material.TipoMaterial,
                                UnidadMedidaBase_Id = GetUnidadMedidaId(material.UnidadDeMedidaBase, unidad, unidadMedidasSap),
                                UnidadMedidaCompras_Id = GetUnidadMedidaId(material.UnidadDeMedidaCompras, unidad, unidadMedidasSap),
                                UnidadMedidaSalida_Id = GetUnidadMedidaId(material.UnidadDeMedidaSalida, unidad, unidadMedidasSap),
                                TipoValoracion = material.TipoValoracion,
                                PrecioMaterial = material.PrecioDelMaterial,
                                GrupoCompras_Id = grupoCompras.Find(x => x.CodigoSap == material.GrupoCompras)?.Id,
                                CuentaMayor_Id = cuentas.Find(x => x.Codigo == material.CuentaDeMayor)?.Id,
                                Estado = true,
                                TextoAmpliado = material.TextoAmpliado,
                            });
                        }
                        else
                        {
                            item.Centro_Id = centroId;
                            item.Codigo = material.NroMaterial;
                            item.CodigoSap = material.NroMaterial;
                            item.Descripcion = material.NombreDeMaterial;
                            item.GrupoArticulo_Id = grupoArticulo.Find(x => x.CodigoSap == material.GrupoArticulo)?.Id;
                            item.TipoMaterial = material.TipoMaterial;
                            item.UnidadMedidaBase_Id = GetUnidadMedidaId(material.UnidadDeMedidaBase, unidad, unidadMedidasSap);
                            item.UnidadMedidaCompras_Id = GetUnidadMedidaId(material.UnidadDeMedidaCompras, unidad, unidadMedidasSap);
                            item.UnidadMedidaSalida_Id = GetUnidadMedidaId(material.UnidadDeMedidaSalida, unidad, unidadMedidasSap);
                            item.TipoValoracion = material.TipoValoracion;
                            item.PrecioMaterial = material.PrecioDelMaterial;
                            item.GrupoCompras_Id = grupoCompras.Find(x => x.CodigoSap == material.GrupoCompras)?.Id;
                            item.CuentaMayor_Id = cuentas.Find(x => x.Codigo == material.CuentaDeMayor)?.Id;
                            item.Estado = true;
                            item.TextoAmpliado = material.TextoAmpliado;
                            idsActualizados.Add(item.Id);
                        }

                        //Al ser alrededor de 150.000 valores guardamos cada 1.000 por si hay una excepción en el medio.
                        if (contador % 1000 == 1)
                        {
                            repositorio.GuardarCambios();
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(new Exception($"Error al grabar el material {material.ToJson()}"));
                        Log.Error(e);
                    }
                }
                Log.Info($"items agregados: {agregados}");
                Log.Info($"items actualizados: {idsActualizados.Count}");
                listaBase.Where(a => !idsActualizados.Contains(a.Id)).ToList().ForEach(a => a.Estado = false);
            }
            repositorio.GuardarCambios();

            int? GetUnidadMedidaId(string unidadDeMedida, List<TablaSap> unidad, List<UnidadMedidaSap> unidadMedidasSap)
            {
                string unidadComercial = unidadMedidasSap.Find(a => a.UM == unidadDeMedida)?.Comercial;
                if (string.IsNullOrWhiteSpace(unidadComercial))
                {
                    return null;
                }
                else
                {
                    return unidad.Find(x => x.CodigoSap == unidadComercial)?.Id;
                }
            }
        }
    }
}
