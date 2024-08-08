using SustitucionMOAAssets;
using System;

namespace SustitucionMOAWS.Logger
{
    public class Log
    {
        private static readonly NLog.Logger DefaultLogger = NLog.LogManager.GetLogger("defaultLogger");
        private static readonly NLog.Logger AzureLogger = NLog.LogManager.GetLogger("azureLogger");
        private static readonly NLog.Logger ExternalAPILogger = NLog.LogManager.GetLogger("externalApiLogger");
        private static readonly NLog.Logger ComprasRegistroInfoLogger = NLog.LogManager.GetLogger("comprasRegistroInfoLogger");

        public static void Error(string ip, string usuario, string controller, string method, string error)
        {
            try
            {
                DefaultLogger.Error(String.Format(ErrorMsg.ErrorLogMensaje, new string[] { controller, method, usuario, ip, error }));
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR en LogService:" + e.Message);
                Console.WriteLine("ERROR heredado:" + error);
            }
        }
        public static void Error(string ip, string usuario, string controller, string method, Exception exception)
        {
            try
            {
                DefaultLogger.Error(String.Format(ErrorMsg.ErrorLogMensaje, new string[] { controller, method, usuario, ip, exception.ToString() }));
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
                DefaultLogger.Error(exception);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR en LogService:" + e.Message);
                Console.WriteLine("ERROR heredado:" + exception.ToString());
            }
        }
        public static void Error(Exception exception, string message)
        {
            try
            {
                DefaultLogger.Error(exception, message);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR en LogService:" + e.Message);
                Console.WriteLine("ERROR heredado:" + exception.ToString());
            }
        }

        public static void Error(string message)
        {
            try
            {
                DefaultLogger.Error(message);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR en LogService:" + e.Message);
                Console.WriteLine("ERROR heredado:" + message);
            }
        }

        public static void Debug(string controller, string method, string valores)
        {
            try
            {
                DefaultLogger.Debug("Controller: " + controller + " Metodo: " + method + " Valores: " + valores );
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
                DefaultLogger.Info(mensaje);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR en LogService:" + e.Message);
            }
        }
        public static void AzureError (Exception exception)
        {
            try
            {
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
                ExternalAPILogger.Info(message);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR en API:" + e.Message);
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
                Log.Error("", "", "", "", e.Message);
                Console.WriteLine("ERROR en ComprasRegistroInfo:" + e.Message);
            }
        }
    }
}
