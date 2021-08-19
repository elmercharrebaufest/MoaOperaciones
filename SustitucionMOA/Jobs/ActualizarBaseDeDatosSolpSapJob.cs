using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;

namespace SustitucionMOA.Jobs
{
    public interface IActualizarBaseDeDatosSolpSapJob : IHangfireJob { }

    public class ActualizarBaseDeDatosSolpSapJob : IActualizarBaseDeDatosSolpSapJob
    {
        private readonly IComprasService _comprasService;
        private readonly IRepositorio repositorio;

        public ActualizarBaseDeDatosSolpSapJob(IComprasService comprasService, IRepositorio repositorio)
        {
            _comprasService = comprasService;
            this.repositorio = repositorio;
        }

        public void Execute()
        {
            try
            {
                ActualizarCecoBase();
                ActualizarServiciosBase();
                ActualizarCuentasBase();
                ActualizarOrdenesBase();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private void ActualizarCecoBase()
        {
            var listaSap = _comprasService.ObtenerCecoSap();

            if (listaSap.Count > 0)
            {
                var listaBase = repositorio.Listar<TablaSap>(c => c.Tabla == TablasSap.CecoSolpSap).ToList();

                for (int i = 0; i < listaSap.Count; i++)
                {
                    if (!listaBase.Any(x => x.CodigoSap == listaSap[i].CodigoSap && x.Descripcion == listaSap[i].Descripcion))
                    {
                        var cecoAgregar = new TablaSap()
                        {
                            Id = -1,
                            Codigo = listaSap[i].Codigo,
                            CodigoSap = listaSap[i].CodigoSap,
                            Descripcion = listaSap[i].Descripcion,
                            Tabla = listaSap[i].Tabla,
                        };

                        repositorio.Agregar(cecoAgregar);
                    }
                }
            }

            repositorio.GuardarCambios();
        }

        private void ActualizarServiciosBase()
        {
            var listaSap = _comprasService.ObtenerServiciosSap();

            if (listaSap.Count > 0)
            {
                var listaBase = repositorio.Listar<TablaSap>(c => c.Tabla == TablasSap.CodigoServicioSap).ToList();

                for (int i = 0; i < listaSap.Count; i++)
                {
                    if (!listaBase.Any(x => x.CodigoSap == listaSap[i].CodigoSap && x.Descripcion == listaSap[i].Descripcion))
                    {
                        var servicioAgregar = new TablaSap()
                        {
                            Id = -1,
                            Codigo = listaSap[i].Codigo,
                            CodigoSap = listaSap[i].CodigoSap,
                            Descripcion = listaSap[i].Descripcion,
                            Tabla = listaSap[i].Tabla,
                        };

                        repositorio.Agregar(servicioAgregar);
                    }
                }
            }

            repositorio.GuardarCambios();
        }

        private void ActualizarCuentasBase()
        {
            var listaSap = _comprasService.ObtenerCuentasSap();

            if (listaSap.Count > 0)
            {
                var listaBase = repositorio.Listar<TablaSap>(c => c.Tabla == TablasSap.CuentasSolpSap).ToList();

                for (int i = 0; i < listaSap.Count; i++)
                {
                    if (!listaBase.Any(x => x.CodigoSap == listaSap[i].CodigoSap && x.Descripcion == listaSap[i].Descripcion))
                    {
                        var cuentaAgregar = new TablaSap()
                        {
                            Id = -1,
                            Codigo = listaSap[i].Codigo,
                            CodigoSap = listaSap[i].CodigoSap,
                            Descripcion = listaSap[i].Descripcion,
                            Tabla = listaSap[i].Tabla,
                        };

                        repositorio.Agregar(cuentaAgregar);
                    }
                }
            }

            repositorio.GuardarCambios();
        }

        private void ActualizarOrdenesBase()
        {
            var listaSap = _comprasService.ObtenerOrdenesSap();

            if (listaSap.Count > 0)
            {
                var listaBase = repositorio.Listar<TablaSap>(c => c.Tabla == TablasSap.OrdenSolpSap).ToList();

                for(int i = 0; i < listaSap.Count; i++)
                {
                    if (!listaBase.Any(x => x.CodigoSap == listaSap[i].CodigoSap && x.Descripcion == listaSap[i].Descripcion))
                    {
                        var ordenAgregar = new TablaSap()
                        {
                            Id = -1,
                            Codigo = listaSap[i].Codigo,
                            CodigoSap = listaSap[i].CodigoSap,
                            Descripcion = listaSap[i].Descripcion,
                            Tabla = listaSap[i].Tabla,
                        };

                        repositorio.Agregar(ordenAgregar);
                    }
                }
            }

            repositorio.GuardarCambios();
        }
    }
}