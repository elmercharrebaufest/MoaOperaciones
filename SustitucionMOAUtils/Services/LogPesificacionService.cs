using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto.LogPesificacion;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Interfaces.Validadores;
using SustitucionMOAWS.DataAgroServices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace SustitucionMOAUtils.Services
{
    public class LogPesificacionService : ILogPesificacionService
    {
        protected readonly IRepositorio repositorio;
        protected readonly IValidadorPesificacion validador;

        public LogPesificacionService(IRepositorio repositorio, IValidadorPesificacion validador)
        {
            this.repositorio = repositorio;
            this.validador = validador;
        }

        public LogPesificacionDto GuardarPesificacion(LogPesificacion entidad)
        {
            var resultadoValidacion = validador.IsValid(entidad);

            if (resultadoValidacion.IsValid)
            {
                var pesificacionAgregada = repositorio.Agregar(entidad);
                repositorio.GuardarCambios();

                return new LogPesificacionDto(pesificacionAgregada);
            }

            throw new ValidationCustomException(resultadoValidacion.ToString());
        }

        public LogPesificacionDto GuardarPesificacionAutomatica(LogPesificacion entidad, HttpPostedFileBase file)
        {

            if (file == null || file.ContentLength == 0 || Path.GetExtension(file.FileName).ToLower() != ".csv")
            {
                throw new ValidationCustomException("Debe seleccionar un archivo .csv valido");
            }


            string fileName = Path.GetFileNameWithoutExtension(file.FileName) + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
            string rutaCarpeta = string.Concat(ConfigurationManager.AppSettings["RutaArchivosLogPesificacion"], "/", entidad.CodigoProveedor);
            string rutaArchivo = string.Concat(rutaCarpeta, "/", fileName);

            entidad.EsCargaMasiva = true;
            entidad.RutaFisicaArchivo = rutaArchivo;
            var resultadoValidacion = validador.IsValid(entidad);

            if (resultadoValidacion.IsValid)
            {

                Directory.CreateDirectory(rutaCarpeta);

                if (File.Exists(rutaArchivo))
                {
                    File.Delete(rutaArchivo);
                }

                file.SaveAs(rutaArchivo);


                entidad.Archivo = new Archivo { FileKey = FileKeys.ArchivoLogPesificaciones, Ruta = rutaArchivo };

                return this.GuardarPesificacion(entidad);
            }

            throw new ValidationCustomException(resultadoValidacion.ToString());
        }

        public List<LogPesificacionDto> ObtenerPorFiltrosPesificacionesManuales(FiltroDeBusquedaDto filtro)
        {
            if (!EsAdministrador(filtro.IdUsuario))
                throw new InfoCustomException("El usuario no es administrador.");

            try
            {
                List<LogPesificacion> pesificaciones = repositorio.Listar<LogPesificacion>(
                    x => (
                     !x.EsCargaMasiva &&
                     (filtro.Contrato.HasValue ? x.Contrato == filtro.Contrato : true) &&
                     (filtro.Fijacion.HasValue ? x.Fijacion == filtro.Fijacion : true) &&
                     (!string.IsNullOrEmpty(filtro.Mail) ? x.Usuario.Mail.Equals(filtro.Mail) : true) &&
                     (!string.IsNullOrEmpty(filtro.Proveedor) ? x.CodigoProveedor.Equals(filtro.Proveedor) : true) &&
                     (!string.IsNullOrEmpty(filtro.Fecha) ? (x.Fecha.Year == filtro.FechaDT.Year
                       && x.Fecha.Month == filtro.FechaDT.Month
                       && x.Fecha.Day == filtro.FechaDT.Day) : true))
                    );

                List<LogPesificacionDto> usuariosDto = pesificaciones.Select(x => new LogPesificacionDto(x)).ToList();

                return usuariosDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<LogPesificacionDto> ObtenerPorFiltrosPesificacionesAutomaticas(FiltroDeBusquedaMasicoDto filtro)
        {
            if (!EsAdministrador(filtro.IdUsuario))
                throw new InfoCustomException("El usuario no es administrador.");
            var lista = repositorio.Listar<LogPesificacion>(x =>
                   x.EsCargaMasiva);
            try
            {
                List<LogPesificacion> pesificaciones = repositorio.Listar<LogPesificacion>(
                    x => (
                     x.EsCargaMasiva &&
                    (!string.IsNullOrEmpty(filtro.Mail) ? x.Usuario.Mail.Equals(filtro.Mail) : true) &&
                    (!string.IsNullOrEmpty(filtro.Proveedor) ? x.CodigoProveedor.Equals(filtro.Proveedor) : true) &&
                    (!string.IsNullOrEmpty(filtro.Fecha) ? (x.Fecha.Year == filtro.FechaDT.Year
                       && x.Fecha.Month == filtro.FechaDT.Month
                       && x.Fecha.Day == filtro.FechaDT.Day) : true))
                    );

                List<LogPesificacionDto> usuariosDto = pesificaciones.Select(x => new LogPesificacionDto(x)).ToList();

                return usuariosDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<LogPesificacionDto> ObtenerPorUsuario(int idUsuario)
        {

            if (!EsAdministrador(idUsuario))
                throw new InfoCustomException("El usuario no es administrador.");

            try
            {

                List<LogPesificacion> pesificaciones = repositorio.Listar<LogPesificacion>(x => x.IdUsuario == idUsuario);
                List<LogPesificacionDto> usuariosDto = pesificaciones.Select(x => new LogPesificacionDto(x)).ToList();

                return usuariosDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<LogPesificacionDto> ObtenerPesificacionesAutomaticas(int idUsuario)
        {
            if (!EsAdministrador(idUsuario))
                throw new InfoCustomException("El usuario no es administrador.");

            try
            {
                List<LogPesificacion> pesificaciones = repositorio.Listar<LogPesificacion>(x => x.EsCargaMasiva);
                List<LogPesificacionDto> usuariosDto = pesificaciones.Select(x => new LogPesificacionDto(x)).OrderBy(x=>x.Fecha).ToList();

                if (usuariosDto.Count == 0)
                {
                    throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Pesificaciones"));
                }
                return usuariosDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<LogPesificacionDto> ObtenerPesificacionesManuales(int idUsuario)
        {
            if (!EsAdministrador(idUsuario))
                throw new InfoCustomException("El usuario no es administrador.");

            try
            {
                List<LogPesificacion> pesificaciones = repositorio.Listar<LogPesificacion>(x => !x.EsCargaMasiva);
                List<LogPesificacionDto> usuariosDto = pesificaciones.Select(x => new LogPesificacionDto(x)).OrderBy(x => x.Fecha).ToList();

                if (usuariosDto.Count == 0)
                {
                    throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Pesificaciones"));
                }
                return usuariosDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Archivo ObtenerArchivoLogPesificacion(int idArchivo)
        {
            try
            {
                Archivo archivo = repositorio.Obtener<Archivo>(x => x.Id == idArchivo);
                return archivo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ActualizarEstadoLogPesificacion(LogPesificacion entidad)
        {
            try
            {
                if (entidad != null && entidad.Id > 0)
                {
                    var pesificacion = repositorio.Obtener<LogPesificacion>(c => c.Id == entidad.Id);
                    if (pesificacion == null) throw new InfoCustomException("No existe el estado");
                    pesificacion.EnvioExitoso = entidad.EnvioExitoso;
                    repositorio.GuardarCambios();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool EsAdministrador(int idUsuario)
        {
            Usuario usuario = repositorio.Obtener<Usuario>(x => x.Id == idUsuario);

            if (usuario == null)
            {
                throw new InfoCustomException(String.Format(InfoMsg.ElementoNoExiste, "Usuario", idUsuario));
            }

            return usuario.EsAdmin();
        }

        public List<LogPesificacionDto> GuardarPesificaciones(List<LogPesificacion> entidades)
        {
            if (entidades == null || !entidades.Any())
            {
                return new List<LogPesificacionDto>();
            }
            this.repositorio.AgregarTodos(entidades);
            this.repositorio.GuardarCambios();
            return entidades.Select(e => new LogPesificacionDto(e)).ToList();
        }

        public void ActualizarEstadoLogPesificacion(List<int> logIds)
        {
            if (logIds == null || !logIds.Any())
            {
                return;
            }

            var logsAActualizar = this.repositorio.Listar<LogPesificacion>()
                                              .Where(l => logIds.Contains(l.Id))
                                              .ToList();

            foreach (var log in logsAActualizar)
            {
                log.EnvioExitoso = true;
            }

            this.repositorio.GuardarCambios();
        }

        public List<LogPesificacionDto> ObtenerLogPesificaciones(List<int> logIds)
        {
            var logs = this.repositorio.Listar<LogPesificacion>()
                                              .Where(l => logIds.Contains(l.Id))
                                              .ToList();
            return logs.Select(e => new LogPesificacionDto(e)).ToList();
        }
    }
}
