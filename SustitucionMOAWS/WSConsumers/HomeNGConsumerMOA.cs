using SustitucionMOAFotmatter;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.Home;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.HomeNGWebServiceMOA;
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
    public class HomeNGConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserSinPI"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassSinPI"];
        public HomeWSMOAResponse request(string proveedor, List<FechaWS> fechas)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

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

                    var request = new Z_MPMF_MOAOP_HOME_NG()
                    {
                        PE_FECHA_IN = fechasSAPArray,
                        PE_PROVEEDOR = proveedor
                    };
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_HOME_NG request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MPMF_MOAOP_HOME_NG(request);
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_HOME_NG response");
                    Log.Info(response.ToXml());
                    HomeWSMOAResponse result = MapSinPI(response.T_SALIDA);
                    return result;
                }
                else
                {
                    List<HomeNGWebServiceMOA.ZMPES4100> fechasSAP = new List<HomeNGWebServiceMOA.ZMPES4100>() { };
                    foreach (FechaWS fecha in fechas)
                    {
                        fechasSAP.Add(new HomeNGWebServiceMOA.ZMPES4100()
                        {
                            FECHA_OP = SAPFormatter.PrepararFecha(fecha.fechaInicio),
                            FECHA_OP_HASTA = SAPFormatter.PrepararFecha(fecha.fechaFin)
                        });
                    }
                    HomeNGWebServiceMOA.ZMPES4100[] fechasSAPArray = fechasSAP.ToArray();
                    SI_MPMF_MOAOP_HOME_NGClient service = new SI_MPMF_MOAOP_HOME_NGClient();
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    HomeNGWebServiceMOA.ZMPES4550[] salidas = service.SI_MPMF_MOAOP_HOME_NG(fechasSAPArray, proveedor);
                    HomeWSMOAResponse result = Map(salidas);
                    return result;
                }


            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private HomeWSMOAResponse Map(HomeNGWebServiceMOA.ZMPES4550[] salidas)
        {
            HomeWSMOAResponse result = new HomeWSMOAResponse();

            foreach (HomeNGWebServiceMOA.ZMPES4550 item in salidas)
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
