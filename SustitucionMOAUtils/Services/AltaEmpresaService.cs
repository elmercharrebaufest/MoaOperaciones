using Quartz.Util;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.DataAgroServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SustitucionMOAUtils.Services
{
    public class AltaEmpresaService : IAltaEmpresaService
    {

        protected readonly IRepositorio repositorio;
        protected readonly IDataAgroService dataAgroService;

        private static readonly string EMAIL_TEMPLATE = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Template","EstadoAlta.html");

        public AltaEmpresaService(IRepositorio repositorio, IDataAgroService dataAgroService)
        {
            this.repositorio = repositorio;
            this.dataAgroService = dataAgroService;
        }

        public List<ProveedorDto> GetEmpresas()
        {
            try
            {
                List<Proveedor> proveedores = repositorio.Listar<Proveedor>(
                                 x => (x.EstadoAprobacion == EstadoAprobacion.AprobacionPendiente
                                || x.EstadoAprobacion == EstadoAprobacion.AnalisisDeNosis
                                || x.EstadoAprobacion == EstadoAprobacion.EtapaFinal
                                || x.EstadoAprobacion == EstadoAprobacion.EdicionRequerida
                                || x.EstadoAprobacion == EstadoAprobacion.Aprobado
                                || x.EstadoAprobacion == EstadoAprobacion.Rechazado
                                || x.EstadoAprobacion == EstadoAprobacion.DeshabilitadoEnDataAgro
                                || x.EstadoAprobacion == EstadoAprobacion.SinAlta)
                                && x.HistorialAprobaciones.Count > 0
                                );

                List<ProveedorDto> proveedorDtos = proveedores.Select(proveedor => new ProveedorDto
                {
                    CodigoProveedor = proveedor.CodigoProveedor ?? "",
                    CUIT = proveedor.CUIT,
                    EstadoAprobacion = proveedor.EstadoAprobacion,
                    EstadoAprobacionDescripcion = proveedor.EstadoAprobacion.ToFriendlyString(),
                    Id = proveedor.Id,
                    IdComercialDataAgro = proveedor.IdComercialDataAgro,
                    IdDataAgro = proveedor.IdDataAgro,
                    Mail = proveedor.Mail ?? "",
                    Observaciones = proveedor.Observaciones,
                    RazonSocial = proveedor.RazonSocial ?? "",
                    RazonSocialCorredor = proveedor.ProveedorCorredor != null ? proveedor.ProveedorCorredor.RazonSocial : "",
                    FechaSolicitud = proveedor.FechaSolicitud,
                    Comercial = proveedor.Comercial,
                    EstadoSIPER = proveedor.EstadoSIPER,
                    UltimaEdicion = proveedor.HistorialAprobaciones.FirstOrDefault() != null ? proveedor.HistorialAprobaciones.OrderByDescending(x => x.Fecha).FirstOrDefault().Fecha : (DateTime?)null,
                    HistorialAprobaciones = proveedor.HistorialAprobaciones?.Select(a => new ProveedorHistorialAprobacionDto
                    {
                        Id = a.Id,
                        EstadoAprobacionDescripcion = proveedor.EstadoAprobacion.ToFriendlyString(),
                        Fecha = a.Fecha,
                        Observacion = a.Observacion,
                        Usuario = a.Usuario.Mail
                    }).ToList()
                }).ToList();

                if (proveedorDtos.Count == 0)
                {
                    throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Empresas"));
                }

                //TODO: Deprecar esto y obtener la razon social a través de la FK del proveedor al proveedor que lo dio de alta
                foreach (var proveedorDto in proveedorDtos)
                {
                    //var corredorAsociado = repositorio.Listar<Proveedor>(p => p.CodigoProveedor.Contains("C") && p.Mail == proveedorDto.Mail).FirstOrDefault();
                    //if (corredorAsociado != null) proveedorDto.RazonSocialCorredor = corredorAsociado.RazonSocial;

                    if (proveedorDto.EstadoAprobacion == EstadoAprobacion.AprobacionPendiente
                        || proveedorDto.EstadoAprobacion == EstadoAprobacion.AnalisisDeNosis
                        || proveedorDto.EstadoAprobacion == EstadoAprobacion.EtapaFinal
                        || proveedorDto.EstadoAprobacion == EstadoAprobacion.EdicionRequerida
                        )
                    {
                        ResultadoValidarProveedorComercial result = dataAgroService.ObtenerValidarCUITProveedorGranos(proveedorDto.CUIT);
                        if (result != null)
                        {
                            proveedorDto.SISAEstadoCuit = result.ProveedorSISAEstadoCuit;
                        }
                    }

                }

                return proveedorDtos;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string SetEstadoAprobacion(int proveedorId,
                                          EstadoAprobacion estado,
                                          string observacion,
                                          string usuarioMail,
                                          string observacionParaElProveedor,
                                          string estadoSIPER,
                                          bool enviarMail)
        {
            try
            {
                Proveedor proveedor = repositorio.Obtener<Proveedor>(proveedorId);

                if (proveedor == null)
                {
                    throw new InfoCustomException(string.Format(InfoMsg.SinRegistros, "Empresas"));
                }
                if (proveedor.HistorialAprobaciones == null)
                {
                    proveedor.HistorialAprobaciones = new List<ProveedorHistorialAprobacion>();
                }

                int usuarioId = repositorio.Obtener<Usuario, int>(u => u.Mail == usuarioMail, x => x.Id);

                if (estado == EstadoAprobacion.AnularRechazo)
                {
                    //Lo inicializo así por las dudas, en el peor de los casos queda igual
                    EstadoAprobacion estadoAnterior = EstadoAprobacion.Rechazado;
                    var historialAnterior = proveedor.HistorialAprobaciones.Where(h => h.EstadoAprobacion != EstadoAprobacion.Rechazado).OrderByDescending(x => x.Fecha).FirstOrDefault();

                    if (historialAnterior == null)
                    {
                        estadoAnterior = EstadoAprobacion.DocumentacionPendiente;
                    }
                    else
                    {
                        estadoAnterior = historialAnterior.EstadoAprobacion;
                    }

                    estado = estadoAnterior;
                }

                proveedor.HistorialAprobaciones.Add(
                    new ProveedorHistorialAprobacion
                    {
                        Fecha = DateTime.Now,
                        EstadoAprobacion = estado,
                        Observacion = observacion,
                        Proveedor_Id = proveedorId,
                        Usuario_Id = usuarioId
                    }
                );

                proveedor.EstadoAprobacion = estado;

                if (estado.Equals(EstadoAprobacion.Aprobado))
                {

                    var usuario = repositorio.Obtener<Usuario>(u => u.Mail == proveedor.Mail);
                    var rolUsuarioGranos = ObtenerRolPorCodigo("GRAN");

                    switch (usuario.TipoUsuario.Nombre)
                    {
                        case "Granos":
                            usuario.RemoverRoles();
                            usuario.AgregarRol(rolUsuarioGranos);

                            break;

                        case "Corredor":
                            var rolCorredor = ObtenerRolPorCodigo("CORR");

                            usuario.RemoverRol("NUECORR");

                            if (!usuario.Roles.Contains(rolCorredor))
                                usuario.AgregarRol(rolCorredor);


                            if (!usuario.Roles.Contains(rolUsuarioGranos))
                                usuario.AgregarRol(rolUsuarioGranos);

                            break;
                    }

                }

                if (estado == EstadoAprobacion.Rechazado || estado == EstadoAprobacion.EdicionRequerida)
                {
                    proveedor.Observaciones = observacionParaElProveedor;
                }

                if (!string.IsNullOrWhiteSpace(estadoSIPER))
                {
                    proveedor.EstadoSIPER = estadoSIPER;
                }

                repositorio.GuardarCambios();

                if (enviarMail)
                {
                    try
                    {
                        ResultadoValidarProveedorComercial resultadoValidarProveedorComercial = dataAgroService.ObtenerValidarCUITProveedorGranos(proveedor.CUIT);
                        List<string> copia = null;
                        if (!string.IsNullOrWhiteSpace(resultadoValidarProveedorComercial.ComercialMail))
                        {
                            copia = new List<string> { resultadoValidarProveedorComercial.ComercialMail };
                        }
                        if (estado == EstadoAprobacion.Aprobado)
                        {
                            EnviarMailAprobado(proveedor, copia);
                        }
                        else if (estado == EstadoAprobacion.Rechazado)
                        {
                            EnviarMailRechazado(proveedor, observacionParaElProveedor, copia);

                        }
                        else if (estado == EstadoAprobacion.EdicionRequerida)
                        {
                            EnviarMailEdicionRequerida(proveedor, observacionParaElProveedor, copia);

                        }
                    }
                    catch (Exception ex)
                    {
                        throw new InfoCustomException(String.Format(SuccessMsg.EmpresaCambioEstadoOK, proveedor.RazonSocial) + ". No se pudo enviar el mail al proveedor.");
                    }

                }
                return string.Format(SuccessMsg.EmpresaCambioEstadoOK, proveedor.RazonSocial);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public string HabilitarUsuario(string usuarioMail, int proveedorID, string observacion)
        {
            int usuarioID = repositorio.Obtener<Usuario, int>(u => u.Mail == usuarioMail, x => x.Id);

            Proveedor proveedor = repositorio.Obtener<Proveedor>(proveedorID);

            Usuario usuario = repositorio.Obtener<Usuario>(u => u.Mail == proveedor.Mail);

            usuario.Habilitado = true;

            var rolUsuario = ObtenerRolPorCodigo(usuario.TipoUsuario.NombreCorto);

            usuario.RemoverRoles();

            usuario.AgregarRol(rolUsuario);

            proveedor.EstadoAprobacion = EstadoAprobacion.Aprobado;


            proveedor.HistorialAprobaciones.Add(
                  new ProveedorHistorialAprobacion
                  {
                      Fecha = DateTime.Now,
                      EstadoAprobacion = EstadoAprobacion.Aprobado,
                      Observacion = observacion,
                      Proveedor_Id = proveedorID,
                      Usuario_Id = usuarioID
                  }
              );

            repositorio.GuardarCambios();

            return string.Format(SuccessMsg.UsuarioHabilitadoOK, usuario.Mail);
        }

        public string DeshabilitarUsuario(string usuarioMail, int proveedorID, string observacion, string observarcionProveedor)
        {
            int usuarioID = repositorio.Obtener<Usuario, int>(u => u.Mail == usuarioMail, x => x.Id);

            Proveedor proveedor = repositorio.Obtener<Proveedor>(proveedorID);

            Usuario usuario = repositorio.Obtener<Usuario>(u => u.Mail == proveedor.Mail);

            usuario.Habilitado = false;

            usuario.RemoverRoles();

            var rolUsuario = ObtenerRolPorCodigo("DES");

            usuario.AgregarRol(rolUsuario);

            proveedor.EstadoAprobacion = EstadoAprobacion.Deshabilitado;

            proveedor.Observaciones = observarcionProveedor;

            proveedor.HistorialAprobaciones.Add(
                  new ProveedorHistorialAprobacion
                  {
                      Fecha = DateTime.Now,
                      EstadoAprobacion = EstadoAprobacion.Deshabilitado,
                      Observacion = observacion,
                      Proveedor_Id = proveedorID,
                      Usuario_Id = usuarioID
                  }
              );

            repositorio.GuardarCambios();

            return string.Format(SuccessMsg.UsuarioDeshabilitadoOK, usuario.Mail);
        }

        public Rol ObtenerRolPorCodigo(string codigo)
        {
            return repositorio.Obtener<Rol>(u => u.Codigo == codigo);
        }

        private void EnviarMailEdicionRequerida(Proveedor proveedor, string observacionParaElProveedor, List<string> copia)
        {
            var cuerpoTemplate = File.ReadAllText(EMAIL_TEMPLATE);
            var cuerpo = string.Format(cuerpoTemplate, proveedor.RazonSocial, "observada", !string.IsNullOrWhiteSpace(observacionParaElProveedor) ? observacionParaElProveedor : "-");
            string asunto = "Molinos Agro - Edición Requerida";

            EmailSender.EnviarMail(new List<string> { proveedor.Mail }, asunto, cuerpo, copia, null, null, null);
        }

        private void EnviarMailRechazado(Proveedor proveedor, string observacionParaElProveedor, List<string> copia)
        {
            var cuerpoTemplate = File.ReadAllText(EMAIL_TEMPLATE);
            var cuerpo = string.Format(cuerpoTemplate, proveedor.RazonSocial, "rechazada", !string.IsNullOrWhiteSpace(observacionParaElProveedor) ? observacionParaElProveedor : "-");
            string asunto = "Molinos Agro - Solicitud Rechazada";
            EmailSender.EnviarMail(new List<string> { proveedor.Mail }, asunto, cuerpo, copia, null, null, null);
        }

        private void EnviarMailAprobado(Proveedor proveedor, List<string> copia)
        {
            var cuerpoTemplate = File.ReadAllText(EMAIL_TEMPLATE);
            var cuerpo = string.Format(cuerpoTemplate, proveedor.RazonSocial, "aprobada", "-");
            string asunto = "Molinos Agro - Alta Exitosa";
            EmailSender.EnviarMail(new List<string> { proveedor.Mail }, asunto, cuerpo, copia, null, null, null);
        }

        public EstadoAprobacionDto GetEstadoAprobacion(string mail)
        {
            try
            {
                var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mail);

                Proveedor proveedor = usuario.ObtenerProveedor();

                EstadoAprobacionDto estadoAprobacionDto = new EstadoAprobacionDto
                {
                    Estado = proveedor.EstadoAprobacion,
                    EstadoDescripcion = proveedor.EstadoAprobacion.ToFriendlyString(),
                    Observaciones = proveedor.Observaciones.IsNullOrWhiteSpace() ? "" : proveedor.Observaciones
                };

                return estadoAprobacionDto;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
