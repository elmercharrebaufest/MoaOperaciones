using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SustitucionMOAModel.Dto;
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
                if (repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "ActualizarBaseDeDatosSolpSapJob").Habilitado == false)
                    return;


                this.ActualizarTablaSap(_comprasService.ObtenerCecoSap(), TablasSap.CecoSolpSap);
                this.ActualizarTablaSap(_comprasService.ObtenerCuentasSap(), TablasSap.CuentasSolpSap);

                this._comprasService.ActualizarMaterialesSolp();
                this._comprasService.ActualizarServiciosSolp();

                this.ActualizarTablaSap(_comprasService.ObtenerOrdenesSap(), TablasSap.OrdenSolpSap);

            }
            catch (Exception e)
            {

                Log.Error(e);
            }
        }

        private void ActualizarTablaSap(List<TablaSapDto> listaSap, string tablaSap)
        {
            if (listaSap.Count > 0)
            {
                var listaBase = repositorio.Listar<TablaSap>(c => c.Tabla == tablaSap);

                for (int i = 0; i < listaSap.Count; i++)
                {
                    var item = listaBase.FirstOrDefault(x => x.CodigoSap == listaSap[i].CodigoSap);
                    if (item == null)
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
                    else
                    {
                        if (item.Descripcion != listaSap[i].Descripcion)
                            item.Descripcion = listaSap[i].Descripcion;
                    }
                }
            }

            repositorio.GuardarCambios();
        }
    }
}