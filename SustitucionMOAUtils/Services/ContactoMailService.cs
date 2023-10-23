using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models.WSMapMOA.ContactoMail;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAValidator;
using SustitucionMOAWS.WSConsumers;

namespace SustitucionMOAUtils.Services
{
    public class ContactoMailService : IContactoMailService
    {
        public ContactoMailService()
        {

        }
        public string SendContactoMail(ContactoContenido contactoContenido, HttpPostedFileBase file)
        {
            try
            {
                contactoContenido = ValidarCampos(contactoContenido);

                string fileName = "";
                var archivoBytes = new byte[0];
                try
                {
                    fileName = file.FileName;
                    var streamLength = file.InputStream.Length;
                    archivoBytes = new byte[streamLength];
                    file.InputStream.Read(archivoBytes, 0, archivoBytes.Length);
                    InputValidator.emailAttatchment(file);
                }
                catch (ValidationCustomException e) { throw e; }
                catch { }

                EmailSender.send(contactoContenido, archivoBytes, fileName);
                return SuccessMsg.EnvioMsjOk;
            }
            catch (ValidationCustomException e)
            {
                throw e;
            }
            catch (InfoCustomException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public List<CategoriaContacto> ObtenerCategorias()
        {
            try
            {
                List<CategoriaContacto> categorias = new ContactoMailCategoriasConsumerMOA().request();
                return categorias;
            }
            catch (ValidationCustomException e)
            {
                throw e;
            }
            catch (InfoCustomException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        private ContactoContenido ValidarCampos(ContactoContenido contactoContenido)
        {

            contactoContenido.proveedor = InputValidator.notEmptyOrNullString(contactoContenido.proveedor, "Proveedor");
            contactoContenido.nombre = InputValidator.notEmptyOrNullString(contactoContenido.nombre, "Nombre");
            contactoContenido.email = InputValidator.validarEmail(contactoContenido.email, "Email");
            if (contactoContenido.camposAdicionales == "A")
            {

                contactoContenido.razonSocial = InputValidator.notEmptyOrNullString(contactoContenido.razonSocial, "Razon Social");
                contactoContenido.nombreVendedor = InputValidator.notEmptyOrNullString(contactoContenido.nombreVendedor, "Nombre Vendedor");
                contactoContenido.contrato = InputValidator.notEmptyOrNullString(contactoContenido.contrato, "Contrato");
                contactoContenido.cuit = InputValidator.notEmptyOrNullString(contactoContenido.cuit, "CUIT");
                contactoContenido.comprobante = InputValidator.notEmptyOrNullString(contactoContenido.comprobante, "Comprobante");
                contactoContenido.fechaPago = InputValidator.validarCampoFecha(contactoContenido.fechaPago, "Fecha Pago");
                try
                {
                    contactoContenido.fechaPago = contactoContenido.fechaPago.Split(' ')[0];
                }
                catch { }
                contactoContenido.importeDecimal = InputValidator.validarCampoDecimal(contactoContenido.importe, "Importe");
                contactoContenido.impuesto = InputValidator.notEmptyOrNullString(contactoContenido.impuesto, "Impuesto");
                contactoContenido.inscripcion = InputValidator.notEmptyOrNullString(contactoContenido.inscripcion, "Inscripcion");
                contactoContenido.motivo = InputValidator.notEmptyOrNullString(contactoContenido.motivo, "Motivo");
            }
            contactoContenido.comentario = InputValidator.notEmptyOrNullString(contactoContenido.comentario, "Comentario");

            return contactoContenido;

        }

        private void validarRespuesta(string cod)
        {
            if (cod != null && cod != "" && cod.ToUpper() != "OK")
                throw new ValidationCustomException(cod);
        }
    }
}