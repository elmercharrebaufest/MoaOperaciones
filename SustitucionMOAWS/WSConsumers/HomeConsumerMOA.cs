using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.Home;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.HomeWebServiceMOA;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MOAOP;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WSConsumers
{
    public class HomeConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];
        public HomeWSMOAResponse request(string proveedor, List<FechaWS> fechas)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4550[] salidas = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4550[] { };
                    List<WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100> fechasSAP = new List<WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100>() { };
                    foreach (FechaWS fecha in fechas)
                    {
                        fechasSAP.Add(new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100()
                        {
                            FECHA_OP = SAPFormatter.PrepararFecha(fecha.fechaInicio),
                            FECHA_OP_HASTA = SAPFormatter.PrepararFecha(fecha.fechaFin)
                        });
                    }
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100[] fechasSAPArray = fechasSAP.ToArray();

                    var request = new Z_MPMF_MOAOP_HOME()
                    {
                        PE_PROVEEDOR = proveedor,
                        T_FECHA_IN = fechasSAPArray,
                        T_SALIDA = salidas
                    };
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_HOME request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MPMF_MOAOP_HOME(request);
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_HOME response");
                    Log.Info(response.ToXml());
                    HomeWSMOAResponse result = MapSinPI(response.T_SALIDA);
                    return result;
                }
                else
                {

                    HomeWebServiceMOA.ZMPES4550[] salidas = new HomeWebServiceMOA.ZMPES4550[] { };
                    List<HomeWebServiceMOA.ZMPES4100> fechasSAP = new List<HomeWebServiceMOA.ZMPES4100>() { };
                    foreach (FechaWS fecha in fechas)
                    {
                        fechasSAP.Add(new HomeWebServiceMOA.ZMPES4100()
                        {
                            FECHA_OP = SAPFormatter.PrepararFecha(fecha.fechaInicio),
                            FECHA_OP_HASTA = SAPFormatter.PrepararFecha(fecha.fechaFin)
                        });
                    }
                    HomeWebServiceMOA.ZMPES4100[] fechasSAPArray = fechasSAP.ToArray();
                    SI_MPMF_MOAOP_HOMEClient service = new SI_MPMF_MOAOP_HOMEClient();
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    service.SI_MPMF_MOAOP_HOME(proveedor, ref fechasSAPArray, ref salidas);
                    HomeWSMOAResponse result = Map(salidas);
                    return result;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private HomeWSMOAResponse Map(HomeWebServiceMOA.ZMPES4550[] salidas)
        {
            HomeWSMOAResponse result = new HomeWSMOAResponse();
            
            foreach (HomeWebServiceMOA.ZMPES4550 item in salidas)
            {
                result.resumen.Add(
                    new ItemResumenHome()
                    {
                        descripcion = item.DESCRIPCION,
                        cantidad = item.CANTIDAD
                    }
                );
            }
            
            return result;
        }
        private HomeWSMOAResponse MapSinPI(WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4550[] salidas)
        {
            HomeWSMOAResponse result = new HomeWSMOAResponse();

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4550 item in salidas)
            {
                result.resumen.Add(
                    new ItemResumenHome()
                    {
                        descripcion = item.DESCRIPCION,
                        cantidad = item.CANTIDAD
                    }
                );
            }

            return result;
        }
    }
}
