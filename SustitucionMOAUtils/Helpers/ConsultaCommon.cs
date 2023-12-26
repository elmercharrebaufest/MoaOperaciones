using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.Helpers
{
    public class ConsultaCommon
    {
        protected readonly IRepositorio repositorio;
        protected readonly IEmailService emailService;
        private readonly string rutaArchivosConsulta = ConfigurationManager.AppSettings["RutaArchivosConsulta"];
        private static readonly string EMAIL_TEMPLATE = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "RespuestaConsulta.html");
        protected static readonly string consultaInternaCC = ConfigurationManager.AppSettings["EmailConsultaInternaCC"];

        public ConsultaCommon(IRepositorio repositorio, IEmailService emailService)
        {
            this.repositorio = repositorio;
            this.emailService = emailService;
        }
        protected string AgregarAdjuntoComentario(int consultaId, int comentarioId, HttpFileCollectionBase files)
        {
            var comentario = repositorio.Obtener<Comentario>(comentarioId);

            if (comentario == null) throw new InfoCustomException("No existe el comentario");
            if (comentario.Consulta_Id != consultaId) throw new InfoCustomException("El comentario no corresponde a la consulta especificada");

            var errores = new List<string>();

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                var fileName = string.Format("{0}_{1}", comentario.Id, Path.GetFileName(file.FileName));

                var ruta = ArmarRutaCarpeta(comentario); // $"{ConfigurationManager.AppSettings["RutaArchivosProveedores"]}/{proveedor.CUIT}/{proveedor.Id}/{FileKeys.Consultas}/{consultaId}";
                var rutaArchivo = string.Concat(ruta, "/", fileName);

                if (File.Exists(rutaArchivo))
                {
                    errores.Add($"{fileName}: {ErrorMsg.ErrorArchivoRepetido}");
                    continue;
                }

                Directory.CreateDirectory(ruta);

                if (comentario.Archivos == null)
                {
                    comentario.Archivos = new List<Archivo>();
                }

                comentario.Archivos.Add(new Archivo
                {
                    FileKey = FileKeys.Consultas,
                    Ruta = rutaArchivo,
                });

                file.SaveAs(rutaArchivo);
                repositorio.GuardarCambios();
            }

            return errores.Any() ? string.Join(".", errores) : SuccessMsg.ArchivoSubidoOK;
        }

        private string ArmarRutaCarpeta(Comentario comentario)
        {
            return string.Format("{0}/{1}/{2}", rutaArchivosConsulta, comentario.Consulta.Usuario_Id, comentario.Consulta_Id);
        }
        protected void EnviarMailInterno(Consulta consulta, Comentario comentario, HttpFileCollectionBase files, List<string> destinatariosCC, string mailDestinatario)
        {
            try
            {
                var cuerpoTemplate = File.ReadAllText(EMAIL_TEMPLATE);
                var cuerpo = string.Format(cuerpoTemplate, consulta.Asunto, !string.IsNullOrWhiteSpace(comentario.Detalle) ? comentario.Detalle : "-");
                string asunto = "Molinos Agro - Consulta N° " + consulta.Id + ": " + consulta.Asunto;
                Dictionary<string, byte[]> archivos = ConvertFiles(files);
                EmailSender.EnviarMail(new List<string> { mailDestinatario }, asunto, cuerpo,
                    null, null, null, null, null, destinatariosCC, archivos);
            }
            catch (Exception ex)
            {
                Log.Error("Hubo un problema al intentar enviar el mail al externo.", ex);
            }
        }

        private Dictionary<string, byte[]> ConvertFiles(HttpFileCollectionBase files)
        {
            Dictionary<string, byte[]> fileDataDictionary = new Dictionary<string, byte[]>();

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                if (file.ContentLength > 0)
                {
                    byte[] fileData;
                    using (BinaryReader reader = new BinaryReader(file.InputStream))
                    {
                        fileData = reader.ReadBytes(file.ContentLength);
                    }
                    fileDataDictionary.Add(file.FileName, fileData);
                }
            }

            if (fileDataDictionary.Count == 0)
                return null;

            return fileDataDictionary;
        }

        protected void RellenarCampos(Consulta consulta, Comentario comentario)
        {
            consulta.FechaCreacion = DateTime.Now;
            consulta.FechaUltimaModificacion = DateTime.Now;

            Categoria categoria = repositorio.Obtener<Categoria>(c => c.Id == consulta.Categoria_Id);

            comentario.ComentarioRecordado = new List<ComentarioRecordado>();

            if (consulta.Comentarios == null)
            {
                consulta.Comentarios = new List<Comentario>();
            }

            consulta.Comentarios.Add(comentario);

            comentario.Usuario_Id = (int)consulta.UsuarioInterno_Id;

            var estadoSolicitudInfo = this.repositorio.Obtener<EstadoConsulta>(e => e.Code == "DOC");
            consulta.EstadoConsulta_Id = estadoSolicitudInfo.Id;
        }
        protected string GuardarAdjuntoComentario(int consultaId, HttpFileCollectionBase files)
        {
            Comentario primerComentario = repositorio.Obtener<Comentario>(c => c.Consulta_Id == consultaId);
            var res = this.AgregarAdjuntoComentario(consultaId, primerComentario.Id, files);
            return res;
        }
        protected void EnviarMail(Consulta consulta, Comentario comentario, HttpFileCollectionBase files)
        {
            var destinatariosCC = this.emailService.ObtenerListaDestinatarios(new string[] { consultaInternaCC });
            var usuario = this.repositorio.Obtener<Usuario>(u => u.Id == consulta.Usuario_Id);
            this.EnviarMailInterno(consulta, comentario, files, destinatariosCC, usuario.Mail);
        }

       
    }
}
