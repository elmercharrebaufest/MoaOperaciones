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
using System.Configuration;
using System.Linq;
using System.Windows.Markup.Localizer;

namespace SustitucionMOAUtils.Services
{
    public class AltaEmpresaService : IAltaEmpresaService
    {

        protected readonly IRepositorio repositorio;
        protected readonly IDataAgroService dataAgroService;


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
                                || x.EstadoAprobacion == EstadoAprobacion.DeshabilitadoEnDataAgro)
                                && !x.CodigoProveedor.Contains("C")
                                );

                List<ProveedorDto> proveedorDtos = proveedores.Select(x => new ProveedorDto(x)).ToList();

                if (proveedorDtos.Count == 0)
                {
                    throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Empresas"));
                }

                //TODO: Deprecar esto y obtener la razon social a través de la FK del proveedor al proveedor que lo dio de alta
                foreach (var proveedorDto in proveedorDtos)
                {
                    var corredorAsociado = repositorio.Listar<Proveedor>(p => p.CodigoProveedor.Contains("C") && p.Mail == proveedorDto.Mail).FirstOrDefault();

                    if (corredorAsociado != null) proveedorDto.RazonSocialCorredor = corredorAsociado.RazonSocial;
                }

                return proveedorDtos;
            }
            catch (Exception)
            {
                throw;
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
                    catch (Exception)
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
            string cuerpo = "Estimado: " + proveedor.RazonSocial + "\n\n"
                                        + "Su alta fue Observada." + "\n\n";
            if (!string.IsNullOrWhiteSpace(observacionParaElProveedor))
            {
                cuerpo += "Observaciones: " + observacionParaElProveedor;
            }
            string asunto = "Molinos Agro - Edición Requerida";

            EmailSender.EnviarMail(new List<string> { proveedor.Mail }, asunto, cuerpo, copia, null, null, null);
        }

        private void EnviarMailRechazado(Proveedor proveedor, string observacionParaElProveedor, List<string> copia)
        {
            string cuerpo = "Estimado: " + proveedor.RazonSocial + "\n\n"
                    + "Su alta fue rechazada por Administración. Comunicarse con su comercial." + "\n\n";
            if (!string.IsNullOrWhiteSpace(observacionParaElProveedor))
            {
                cuerpo += "Observaciones: " + observacionParaElProveedor;
            }
            string asunto = "Molinos Agro - Solicitud Rechazada";
            EmailSender.EnviarMail(new List<string> { proveedor.Mail }, asunto, cuerpo, copia, null, null, null);
        }

        private void EnviarMailAprobado(Proveedor proveedor, List<string> copia)
        {
            string cuerpo = "Estimado: " + proveedor.RazonSocial + "\n\n"
                      + "Su alta para operar en Molinos Agro fue aprobada exitosamente." + "\n\n";
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
