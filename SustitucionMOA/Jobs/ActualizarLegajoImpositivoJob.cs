using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Configuration;
using System.Diagnostics;

namespace SustitucionMOA.Jobs
{
    public interface IActualizarLegajoImpositivoJob : IHangfireJob { }

    public class ActualizarLegajoImpositivoJob : IActualizarLegajoImpositivoJob
    {
        private readonly IRepositorio repositorio;

        public ActualizarLegajoImpositivoJob(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public void Execute()
        {
            try
            {
                if (repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "ActualizarLegajoImpositivoJob").Habilitado == false)
                    return;

                // Ruta del archivo .cmd
                string rutaCmd = ConfigurationManager.AppSettings["Script_legajo_impositivo"] ?? "";

                // Configurar el proceso
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c \"{rutaCmd}\"", // /c ejecuta y cierra el cmd
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = false // Para que no aparezca la ventana de cmd
                };

                // Iniciar el proceso
                using (Process process = new Process { StartInfo = psi })
                {
                    process.Start();

                    // Leer la salida
                    string salida = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    process.WaitForExit();
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
        }
    }
}