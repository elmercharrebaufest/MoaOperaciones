using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.ScatoWebService;
using SustitucionMOAWS.WSConsumers;

namespace SustitucionMOA.Jobs
{

    public interface IActualizarSISAJob : IHangfireJob { };

    public class ActualizarSISAJob : IActualizarSISAJob
    {
        protected readonly IDataAgroService dataAgroService;
        protected readonly IRepositorio repositorio;
        public ActualizarSISAJob(IDataAgroService dataAgroService, IRepositorio repositorio)
        {
            this.dataAgroService = dataAgroService;
            this.repositorio = repositorio;
        }
        public void Execute()
        {
            try
            {
                if (repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "ActualizarSISAJob").Habilitado == false)
                    return;

                var proveedores = repositorio.Listar<Proveedor>(x => x.TipoProveedor.Id == (int) TipoUsuarioEnum.Granos || x.TipoProveedor.Id == (int) TipoUsuarioEnum.Corredor);

                foreach (var proveedor in proveedores)
                {                    
                        var result = dataAgroService.ObtenerValidarCUITProveedorGranos(proveedor.CUIT);
                        if (result != null)
                        {
                            proveedor.EstadoSISA = result.ProveedorSISAEstadoCuit;
                        }     
                }

                repositorio.GuardarCambios();
            }
            catch(Exception e)
            {
                Log.Error(e);
            }
        }


        
    }
}