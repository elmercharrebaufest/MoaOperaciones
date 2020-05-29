using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.Home;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.HomeNGWebServiceMOA;

namespace SustitucionMOAWS.WSConsumers
{
    public class HomeNGConsumerMOA
    {
        SI_MPMF_MOAOP_HOME_NGClient service = new SI_MPMF_MOAOP_HOME_NGClient();

        public HomeWSMOAResponse request(string proveedor, List<FechaWS> fechas)
        {
            try
            {
                List<ZMPES4100> fechasSAP = new List<ZMPES4100>() { };
                foreach (FechaWS fecha in fechas)
                {
                    fechasSAP.Add(new ZMPES4100()
                    {
                        FECHA_OP = SAPFormatter.PrepararFecha(fecha.fechaInicio),
                        FECHA_OP_HASTA = SAPFormatter.PrepararFecha(fecha.fechaFin)
                    });
                }
                ZMPES4100[] fechasSAPArray = fechasSAP.ToArray();

                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                ZMPES4550[] salidas = service.SI_MPMF_MOAOP_HOME_NG(fechasSAPArray, proveedor);
                HomeWSMOAResponse result = map(salidas);
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private HomeWSMOAResponse map(ZMPES4550[] salidas)
        {
            HomeWSMOAResponse result = new HomeWSMOAResponse();

            foreach (ZMPES4550 item in salidas)
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
