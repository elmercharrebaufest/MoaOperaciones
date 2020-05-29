using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Models.WSMapMOA.Noticia;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.NoticiasDetalleWebServiceMOA;

namespace SustitucionMOAWS.WSConsumers
{
    public class NoticiasDetalleConsumerMOA
    {
        SI_MPMF_MOAOP_DETALLES_NOTICIASClient service = new SI_MPMF_MOAOP_DETALLES_NOTICIASClient();

        public NoticiasDetallesWSMOAResponse request(string proveedor, string fecha)
        {
            try
            {
                ZMPES4530[] cabeceras = new ZMPES4530[] { };
                ZMPES4540[] contenidos = new ZMPES4540[] { };
                ZMPES4920[] salidas = new ZMPES4920[] { };
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                ZMPES4910 error = service.SI_MPMF_MOAOP_DETALLES_NOTICIAS(fecha, proveedor, ref cabeceras, ref contenidos, out salidas);
                NoticiasDetallesWSMOAResponse result = map(error, cabeceras, contenidos, salidas);
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private NoticiasDetallesWSMOAResponse map(ZMPES4910 error, ZMPES4530[] cabeceras, ZMPES4540[] contenidos, ZMPES4920[] salidas)
        {
            NoticiasDetallesWSMOAResponse result = new NoticiasDetallesWSMOAResponse();

            if (error != null)
            {
                result.error.descripcion = error.DESCRIPCION;
                result.error.codigo = error.CODIGO;
                result.error.tipo = error.TIPO;
            }

            foreach (ZMPES4530 cabecera in cabeceras)
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

            foreach (ZMPES4540 contenido in contenidos)
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

            foreach (ZMPES4920 salida in salidas)
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
