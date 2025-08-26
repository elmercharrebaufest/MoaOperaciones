using SustitucionMOAModel.Models.WSMapMOA.Noticia;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.NoticiasDetalleWebServiceMOA;
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
    public class NoticiasDetalleConsumerMOA
    {

        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserSinPI"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassSinPI"];
        SI_MPMF_MOAOP_DETALLES_NOTICIASClient service = new SI_MPMF_MOAOP_DETALLES_NOTICIASClient();

        public NoticiasDetallesWSMOAResponse request(string proveedor, string fecha)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4530[] cabeceras = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4530[] { };
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4540[] contenidos = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4540[] { };
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4920[] salidas = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4920[] { };

                    var request = new Z_MPMF_MOAOP_DETALLES_NOTICIAS()
                    {
                        PE_FECHA = fecha,
                        PE_PROVEEDOR = proveedor,
                        T_CABECERA = cabeceras,
                        T_CONTENIDO = contenidos,
                    };
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_DETALLES_NOTICIAS request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MPMF_MOAOP_DETALLES_NOTICIAS(request);
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_DETALLES_NOTICIAS response");
                    Log.Info(response.ToXml());
                    return MapSinPI(response.MENSAJE_ERROR,response.T_CABECERA, response.T_CONTENIDO, response.T_SALIDA);
                }
                else
                {
                    NoticiasDetalleWebServiceMOA.ZMPES4530[] cabeceras = new NoticiasDetalleWebServiceMOA.ZMPES4530[] { };
                    NoticiasDetalleWebServiceMOA.ZMPES4540[] contenidos = new NoticiasDetalleWebServiceMOA.ZMPES4540[] { };
                    NoticiasDetalleWebServiceMOA.ZMPES4920[] salidas = new NoticiasDetalleWebServiceMOA.ZMPES4920[] { };
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    NoticiasDetalleWebServiceMOA.ZMPES4910 error = service.SI_MPMF_MOAOP_DETALLES_NOTICIAS(fecha, proveedor, ref cabeceras, ref contenidos, out salidas);
                    NoticiasDetallesWSMOAResponse result = Map(error, cabeceras, contenidos, salidas);
                    return result;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private NoticiasDetallesWSMOAResponse Map(NoticiasDetalleWebServiceMOA.ZMPES4910 error, NoticiasDetalleWebServiceMOA.ZMPES4530[] cabeceras, NoticiasDetalleWebServiceMOA.ZMPES4540[] contenidos, NoticiasDetalleWebServiceMOA.ZMPES4920[] salidas)
        {
            NoticiasDetallesWSMOAResponse result = new NoticiasDetallesWSMOAResponse();

            if (error != null)
            {
                result.error.descripcion = error.DESCRIPCION;
                result.error.codigo = error.CODIGO;
                result.error.tipo = error.TIPO;
            }

            foreach (NoticiasDetalleWebServiceMOA.ZMPES4530 cabecera in cabeceras)
            {
                
                result.noticias.Add(new Noticia() {
                    cabecera = new Cabecera()
                        {
                            fecha = cabecera.FECHA,
                            id = cabecera.ID,
                            nueva = cabecera.NUEVA,
                            titulo = cabecera.TITULO
                        }
                });
            }

            foreach (NoticiasDetalleWebServiceMOA.ZMPES4540 contenido in contenidos)
            {
                Noticia noticia = result.noticias.Find(n => n.cabecera.id == contenido.ID);
                if (noticia != null) {
                    if (noticia.contenido == null)
                    {
                        noticia.contenido = new Contenido()
                        {
                            contenido = contenido.CONTENIDO,
                            id = contenido.ID,
                            nroLinea = contenido.NRO_LINEA
                        };
                    }
                    else {
                        noticia.contenido.contenido += " " + contenido.CONTENIDO;
                    }
                }
            }

            foreach (NoticiasDetalleWebServiceMOA.ZMPES4920 salida in salidas)
            {
                result.notificaciones.Add(new Notificacion()
                {
                    id = salida.ID,
                    texto = salida.TEXTO
                });
                
            }

            return result;

        }
        private NoticiasDetallesWSMOAResponse MapSinPI(WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4910 error, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4530[] cabeceras, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4540[] contenidos, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4920[] salidas)
        {
            NoticiasDetallesWSMOAResponse result = new NoticiasDetallesWSMOAResponse();

            if (error != null)
            {
                result.error.descripcion = error.DESCRIPCION;
                result.error.codigo = error.CODIGO;
                result.error.tipo = error.TIPO;
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4530 cabecera in cabeceras)
            {

                result.noticias.Add(new Noticia()
                {
                    cabecera = new Cabecera()
                    {
                        fecha = cabecera.FECHA,
                        id = cabecera.ID.ToString(),
                        nueva = cabecera.NUEVA,
                        titulo = cabecera.TITULO
                    }
                });
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4540 contenido in contenidos)
            {
                Noticia noticia = result.noticias.Find(n => n.cabecera.id == contenido.ID.ToString());
                if (noticia != null)
                {
                    if (noticia.contenido == null)
                    {
                        noticia.contenido = new Contenido()
                        {
                            contenido = contenido.CONTENIDO,
                            id = contenido.ID.ToString(),
                            nroLinea = contenido.NRO_LINEA.ToString()
                        };
                    }
                    else
                    {
                        noticia.contenido.contenido += " " + contenido.CONTENIDO;
                    }
                }
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4920 salida in salidas)
            {
                result.notificaciones.Add(new Notificacion()
                {
                    id = salida.ID,
                    texto = salida.TEXTO
                });

            }

            return result;

        }

    }
}
