using SustitucionMOAWS.Util;
using System;
using System.Configuration;

namespace SustitucionMOAWS.Logger
{
    public static class SapLogHelper
    {
        public static void LogResponse(object response, string nombreMetodo)
        {
            try
            {
                string xml = response.ToXml();
                int size = System.Text.Encoding.UTF8.GetByteCount(xml);
                bool enableBigLogs = ConfigurationManager.AppSettings["SAPEnableBigLogs"] == "1";

                Log.Info($"SAP sin PI {nombreMetodo} response");
                if (enableBigLogs || size < 1048576)
                    Log.Info(xml);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error al loguear respuesta de {nombreMetodo}");
            }
        }
    }

}
