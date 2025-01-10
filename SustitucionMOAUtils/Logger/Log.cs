using NLog;
using NLog.Config;
using NLog.Targets;
using SustitucionMOAAssets;
using SustitucionMOAModel.Dto;
using System;
using System.Configuration;
using System.IO;
using System.Web.Hosting;

namespace SustitucionMOAUtils.Logger
{
    public class LogConfig
    {
        public static void ConfigureNLog()
        {
            var config = LogManager.Configuration;

            if (config == null)
            {
                // Aquí configuras la conexión si es necesario
                config = new LoggingConfiguration();
            }
            // Obtener el connection string desde el web.config
            var connectionString = ConfigurationManager.ConnectionStrings["CONTEXTO"].ConnectionString;

            // Asignar el valor al parámetro de NLog
            LogManager.Configuration.Variables["dbConnectionString"] = connectionString;

            // Aplicar la nueva configuración
            LogManager.ReconfigExistingLoggers();

        }
    }
    public class Log
    {
        private static readonly NLog.Logger DefaultLogger = NLog.LogManager.GetLogger("defaultLogger");
        private static readonly NLog.Logger AzureLogger = NLog.LogManager.GetLogger("azureLogger");
        private static readonly NLog.Logger ExternalAPILogger = NLog.LogManager.GetLogger("externalApiLogger");
        private static readonly NLog.Logger FrontLogger = NLog.LogManager.GetLogger("frontLogger");
        private static readonly NLog.Logger ComprasRegistroInfoLogger = NLog.LogManager.GetLogger("comprasRegistroInfoLogger");
        private static readonly NLog.Logger RequestLogger = NLog.LogManager.GetLogger("requestLogger");

        public Log()
        {
            ConfigLog();
        }

        public static string ConfigLog()
        {
            // Crea una nueva instancia de LoggingConfiguration
            var config = new LoggingConfiguration();
            string rutaSitioWeb = HostingEnvironment.MapPath("~");
            rutaSitioWeb = Path.Combine(rutaSitioWeb, "bin");
            // Carga la configuración del archivo específico            
            var logPath = Path.Combine(rutaSitioWeb, "nlog.config");
            config = new XmlLoggingConfiguration(logPath);
            var sapUrl = System.Configuration.ConfigurationManager.AppSettings["SpaUrl"] ?? "";
            if (sapUrl.Contains("compras"))
            {
                logPath = Path.Combine(rutaSitioWeb, "nlog.compras.config");
                config = new XmlLoggingConfiguration(logPath);
            }
            if (sapUrl.Contains("huenei"))
            {
                logPath = Path.Combine(rutaSitioWeb, "nlog.huenei.config");
                config = new XmlLoggingConfiguration(logPath);
            }
            if (sapUrl.Contains("pre"))
            {
                logPath = Path.Combine(rutaSitioWeb, "nlog.pre.config");
                config = new XmlLoggingConfiguration(logPath);
            }

            // Configura LogManager con la nueva configuración
            NLog.LogManager.Configuration = config;
            LogConfig.ConfigureNLog();

            var target = GetTarget(config);

            if (target != null)
            {
                Console.WriteLine("target found:");
                Console.WriteLine("Name: " + target.Name);
                Console.WriteLine("File Name: " + Path.GetDirectoryName(target.FileName.ToString()));
                return Path.GetDirectoryName(target.FileName.ToString());
            }
            else
            {
                Console.WriteLine("Azure target not found.");
            }

            return "C:\\MOAOperacionesLogs";
        }

        static FileTarget GetTarget(LoggingConfiguration config)
        {
            foreach (var target in config.AllTargets)
            {
                if (target is FileTarget fileTarget)
                {
                    return fileTarget;
                }
            }
            return null;
        }


        public static void Error(string ip, string usuario, string controller, string method, Exception exception)
        {
            try
            {
                ConfigLog();
                DefaultLogger.Error(exception, String.Format(ErrorMsg.ErrorLogMensaje, controller, method, usuario, ip, ""));
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR en LogService:" + e.Message);
                Console.WriteLine("ERROR heredado:" + exception.ToString());
            }
        }
        public static void Error(Exception exception)
        {
            try
            {
                ConfigLog();

                DefaultLogger.Error(exception);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR en LogService:" + e.Message);
                Console.WriteLine("ERROR heredado:" + exception.ToString());
            }
        }

        public static void Error(string mensaje, Exception excepcion)
        {
            try
            {
                ConfigLog();

                DefaultLogger.Error(excepcion, mensaje);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR en LogService:" + e.Message);
                Console.WriteLine("ERROR heredado:" + excepcion.ToString());
            }
        }

        public static void Debug(string controller, string method, string valores)
        {
            try
            {
                ConfigLog();

                DefaultLogger.Debug("Controller: " + controller + " Metodo: " + method + " Valores: " + valores);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR en LogService:" + e.Message);
            }
        }

        public static void Debug(string mensaje)
        {
            try
            {
                ConfigLog();

                DefaultLogger.Debug(mensaje);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR en LogService:" + e.Message);
            }
        }

        public static void Info(string mensaje)
        {
            try
            {
                ConfigLog();

                DefaultLogger.Info(mensaje);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR en LogService:" + e.Message);
            }
        }
        public static void AzureError(Exception exception)
        {
            try
            {
                ConfigLog();

                AzureLogger.Error(exception);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR en LogService:" + e.Message);
            }
        }
        public static void ExternalAPIError(Exception exception)
        {
            try
            {
                ConfigLog();

                ExternalAPILogger.Error(exception);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR en API:" + e.Message);
            }
        }
        public static void ExternalAPIInfo(string message)
        {
            try
            {
                ConfigLog();

                ExternalAPILogger.Info(message);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR en API:" + e.Message);
            }
        }
        public static void FrontError(FrontLoggerRequestDto frontData)
        {
            FrontError(frontData.ToString());
        }
        public static void FrontError(string message)
        {
            try
            {
                FrontLogger.Error(message);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR en front:" + e.Message);
            }
        }
        public static void ComprasRegistroInfo(string message)
        {
            try
            {
                ComprasRegistroInfoLogger.Info(message);
            }
            catch (Exception e)
            {
                Log.Error(e);
                Console.WriteLine("ERROR en ComprasRegistroInfo:" + e.Message);
            }
        }
        public static void LogRequest(string mensaje)
        {
            try
            {
                RequestLogger.Info(mensaje);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR en LogService:" + e.Message);
            }
        }
    }
}
