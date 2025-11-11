using SustitucionMOAModel.Dto;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace SustitucionMOAUtils.Services
{
    public class LogService : ILogService
    {
        private readonly IRepositorio repositorio;
        public LogService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }
        public void EliminarLogsAntiguos()
        {
            string unidad = @"L:\";
            int.TryParse(ConfigurationManager.AppSettings["DiasGuardadoLogs"], out int diasGuardadoLogs);
            if (diasGuardadoLogs == 0) diasGuardadoLogs = 30;

            List<ArchivoLog> archivosEliminados = BorrarArchivosViejos(unidad, TimeSpan.FromDays(diasGuardadoLogs));

            if (archivosEliminados == null || archivosEliminados.Count == 0)
            {
                Log.Info($"No se elimino ningun archivo.");
            }
            else
            {
                Log.Info("ARCHIVOS ELIMINADOS:");
                foreach (var info in archivosEliminados)
                {
                    Log.Info($"Nombre: {info.Nombre}, Peso: {info.PesoKB} KB, Fecha creacion: {info.FechaCreacion}, Fecha ultima modificacion {info.FechaUltimaModificacion}");
                }
            }
            var date = DateTime.Today.AddDays(diasGuardadoLogs * -1).ToString("yyyyMMdd");
            repositorio.ExecuteCommand($"delete logs.logtable where date <= '{date}'");
        }

        // 🔁 Método ahora virtual para poder simularlo en tests
        protected virtual List<ArchivoLog> BorrarArchivosViejos(string path, TimeSpan antiguedad)
        {
            var archivosEliminados = new List<ArchivoLog>();
            DateTime limite = DateTime.Now.Subtract(antiguedad);

            try
            {
                foreach (string archivo in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
                {
                    try
                    {
                        FileInfo fi = new FileInfo(archivo);

                        if (fi.LastWriteTime < limite)
                        {
                            archivosEliminados.Add(new ArchivoLog
                            {
                                Nombre = fi.FullName,
                                PesoKB = fi.Length / 1024,
                                FechaCreacion = fi.CreationTime,
                                FechaUltimaModificacion = fi.LastWriteTime
                            });

                            fi.Delete();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Error al eliminar {archivo}: {ex.Message}", ex);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Error al acceder al directorio: " + e.Message, e);
            }

            return archivosEliminados;
        }
    }

}
