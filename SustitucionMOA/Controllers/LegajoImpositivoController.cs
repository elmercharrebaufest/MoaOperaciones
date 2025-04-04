using SustitucionMOAUtils.Logger;
using System;
using System.Diagnostics;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    [Authorize]
    public class LegajoImpositivoController : BaseController
    {
        // GET: LegajoImpositivo/Actualizar
        public JsonResult Actualizar()
        {
            try
            {
                // Ruta del archivo .cmd
                string rutaCmd = @"C:\Script\Copy_legajo_impositivo.bat";

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

                    // Mostrar los resultados
                    Console.WriteLine("Salida:");
                    Console.WriteLine(salida);
                    Console.WriteLine("Errores:");
                    Console.WriteLine(error);
                    return JsonCustom($"Salida:{salida}, Error:{error}");
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return JsonCustom(ex.Message);
            }

        }
    }
}