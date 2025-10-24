using System;
using System.Configuration;

namespace SustitucionMOAWS.Logger
{
    public static class SapLogHelper
    {
        public static void LogResponse(string response, string nombreMetodo)
        {
            try
            {
                int size = System.Text.Encoding.UTF8.GetByteCount(response);
                bool enableBigLogs = ConfigurationManager.AppSettings["SAPEnableBigLogs"] == "1";

                Log.Info($"SAP sin PI {nombreMetodo} response");
                if (enableBigLogs || size < 524288)
                    Log.Info(response);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error al loguear respuesta de {nombreMetodo}");
            }
        }
    }

}
