using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models.WSMapMOA.ContactoMail;
using SustitucionMOAWS.ContactoMailWebServiceMOA;
using SustitucionMOAWS.CredentialService;

namespace SustitucionMOAWS.WSConsumers
{
    public class ContactoMailConsumerMOA
    {
        SI_MPMF_MOAOP_MAIL_CONTACTOClient service = new SI_MPMF_MOAOP_MAIL_CONTACTOClient();

        public string request(ContactoContenido contactoContenido, byte[] file, string fileName)
        {
            try
            {
                ZMPES5350 datosAdicionales = new ZMPES5350 {
                    COMPROBANTE = contactoContenido.comprobante,
                    CONTRATO = contactoContenido.contrato,
                    CUIT = contactoContenido.cuit,
                    FECHA_PAGO = contactoContenido.fechaPago,
                    IMPORTE = contactoContenido.importeDecimal,
                    IMPUESTO = contactoContenido.impuesto,
                    INSCRIPCION = contactoContenido.inscripcion,
                    MOTIVO = contactoContenido.motivo,
                    NOMBRE_VEND = contactoContenido.nombreVendedor,
                    RAZON_SOCIAL = contactoContenido.razonSocial

                };
                ZMPES5340 datosReclamo = new ZMPES5340 {
                    CATEGORIA = contactoContenido.categoria,
                    COMENTARIO = contactoContenido.comentario,
                    EMAIL = contactoContenido.email,
                    NOMBRE = contactoContenido.nombre,
                    PROVEEDOR = contactoContenido.proveedor,
                    TELEFONO = contactoContenido.telefono
                };
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                return service.SI_MPMF_MOAOP_MAIL_CONTACTO(file, datosAdicionales, datosReclamo, fileName);
            }
            catch (Exception e)
            {
                throw e;
            }

        }
    }
}

