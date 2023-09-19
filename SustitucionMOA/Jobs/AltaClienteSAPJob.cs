using System;
using System.Collections.Generic;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOARepositorio;
using System.Linq;
using System.Web;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;

namespace SustitucionMOA.Jobs
{
    public interface IAltaClienteSAPJob : IHangfireJob { };

    public class AltaClienteSAPJob : IAltaClienteSAPJob
    {
        private readonly IOrdenDeCargaService ordenDeCargaService;
        private readonly IRepositorio repositorio;

        public AltaClienteSAPJob(IOrdenDeCargaService ordenDeCargaService, IRepositorio repositorio)
        {
            this.ordenDeCargaService = ordenDeCargaService;
            this.repositorio = repositorio;
        }
        public void Execute()
        {
            try
            {
                DateTime fechaFin = DateTime.Today;
                DateTime fechaInicio = fechaFin.AddDays(-2);

                if (fechaFin.Date.Equals(new DateTime(2023, 09, 19)))
                {
                    fechaInicio = fechaFin.AddMonths(-2);
                }

                var clientesSap = this.ordenDeCargaService.GetClientesVigentesSAP(fechaInicio.ToString("yyyy-MM-dd"), fechaFin.ToString("yyyy-MM-dd"));
                var clientesNuevos = this.ordenDeCargaService.FiltrarNoExistentesWeb(clientesSap);

                if (clientesNuevos == null || clientesNuevos.Count() == 0) { return; }

                var tipoProveedor = repositorio.Obtener<TipoUsuario>(t => t.NombreCorto == "CLI" && t.Id == 5);

                foreach (Proveedor c in clientesNuevos)
                {
                    //Guardamos la entidad uno por uno para luego actualizar su tipo de proveedor.
                    var cliNuevo = this.repositorio.Agregar(c);
                    cliNuevo.TipoProveedor = tipoProveedor;
                }

                this.repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}