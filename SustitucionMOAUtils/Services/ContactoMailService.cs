using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.ContactoMail;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAValidator;
using SustitucionMOAWS.WSConsumers;

namespace SustitucionMOAUtils.Services
{
    public class ContactoMailService : IContactoMailService
    {
        protected readonly IRepositorio repositorio;
        public ContactoMailService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }
        private string ArmarRutaCarpeta(string rutaArchivosProveedores, Comentario comentario)
        {
            return string.Format("{0}/{1}/{2}", rutaArchivosProveedores, comentario.Usuario_Id, comentario.Consulta_Id);
        }

        private string GuardarArchivo(HttpPostedFileBase fileSubido, Comentario comentario, string fileKey)
        {
            try
            {
                string fileName = string.Format("{0}_{1}", comentario.Id, Path.GetFileName(fileSubido.FileName));

                string rutaArchivosProveedores = ConfigurationManager.AppSettings["RutaArchivosContacto"];

                string rutaCarpeta = ArmarRutaCarpeta(rutaArchivosProveedores, comentario);

                string rutaArchivo = string.Format("{0}/{1}", rutaCarpeta, fileName);

                if (File.Exists(rutaArchivo))
                {
                    return ErrorMsg.ErrorArchivoRepetido;
                }

                Directory.CreateDirectory(rutaCarpeta);

                comentario.Archivos.Add(new Archivo { FileKey = fileKey, Ruta = rutaArchivo });

                fileSubido.SaveAs(rutaArchivo);
                repositorio.GuardarCambios();

                return SuccessMsg.ArchivoSubidoOK;
            }
            catch (InfoCustomException)
            {
                throw;
            }
            catch (ValidationCustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public string sendContactoMail(ContactoContenido contactoContenido, HttpPostedFileBase file)
        {
            try
            {
                contactoContenido = validarCampos(contactoContenido);

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

                //var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
                var categoria = repositorio.Obtener<Categoria>(u => u.Code == contactoContenido.categoria);
                var proveedor = repositorio.Obtener<Proveedor>(contactoContenido.proveedor);
                var estadoInicial = repositorio.Obtener<EstadoConsulta>(contactoContenido.proveedor);
                //guardar en base
                Consulta nuevaConsulta = new Consulta()
                {
                    Id = -1,
                    Asunto = contactoContenido.asunto,
                    Categoria = categoria,
                    Categoria_Id = categoria.Id,
                    Proveedor = proveedor,
                    Proveedor_Id = proveedor.Id,
                    Comprobante = contactoContenido.comprobante,
                    Contrato = contactoContenido.contrato,
                    CUIT = contactoContenido.cuit,
                    Email = contactoContenido.email,
                    EstadoConsulta = estadoInicial,
                    EstadoConsulta_Id = estadoInicial.Id,
                    FechaCreacion = DateTime.Now,
                    FechaPago = DateTime.Parse(contactoContenido.fechaPago),
                    
                };
                



                //EmailSender.send(contactoContenido, archivoBytes, fileName);
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

        public List<CategoriaContacto> getCategorias()
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

        private ContactoContenido validarCampos(ContactoContenido contactoContenido) {

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

        private void validarRespuesta(string cod){
            if (cod != null && cod != "" && cod.ToUpper() != "OK" )
                throw new ValidationCustomException(cod); 
        }
    }
}
