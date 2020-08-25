using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz.Util;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOAModel.Models.WSMapMOA.ContactoMail;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.CredentialService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace SustitucionMOAUtils.Services
{
    public class AltaEmpresaService : IAltaEmpresaService
    {

        protected readonly IRepositorio repositorio;
        private readonly string DataAgroURL;

        public AltaEmpresaService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
            this.DataAgroURL = ConfigurationManager.AppSettings["DataAgroURL"];

        }

        public List<ProveedorDto> getEmpresas()
        {
            try
            {

                List<Proveedor> proveedores = repositorio.Listar<Proveedor>(
                                 x => (int)x.EstadoAprobacion == (int)EstadoAprobacion.AprobacionPendiente
                                || (int)x.EstadoAprobacion == (int)EstadoAprobacion.AnalisisDeNosis
                                || (int)x.EstadoAprobacion == (int)EstadoAprobacion.SentenciaFinal
                                || (int)x.EstadoAprobacion == (int)EstadoAprobacion.EdicionRequerida
                                );


                List<ProveedorDto> proveedorDtos = proveedores.Select(x => new ProveedorDto
                {
                    CodigoProveedor = x.CodigoProveedor ?? "",
                    CUIT = x.CUIT,
                    EstadoAprobacion = x.EstadoAprobacion,
                    EstadoAprobacionDescripcion = x.EstadoAprobacion.ToString(),
                    Id = x.Id,
                    IdComercialDataAgro = x.IdComercialDataAgro,
                    IdDataAgro = x.IdDataAgro,
                    Mail = x.Mail ?? "",
                    Observaciones = x.Observaciones,
                    RazonSocial = x.RazonSocial ?? "",
                    Comercial = x.UsuariosAsociados.Count() > 0 ? (x.UsuariosAsociados.First() as UsuarioGranos).Comercial : "",
                    HistorialAprobaciones = x.HistorialAprobaciones.Select(a => new ProveedorHistorialAprobacionDto
                    {
                        Id = a.Id,
                        EstadoAprobacionDescripcion = a.EstadoAprobacion.ToString(),
                        Fecha = a.Fecha,
                        Observacion = a.Observacion,
                        Usuario = a.Usuario.Mail
                    }).ToList()
                }).ToList();

                if (proveedorDtos.Count == 0)
                {
                    throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Empresas"));
                }
                return proveedorDtos;
            }
            catch (Exception e)
            {

                throw;
            }


        }

        public string setEstadoAprobacion(int proveedorId, EstadoAprobacion estado, string observacion, string usuarioMail, string observacionParaElProveedor)
        {
            try
            {
                Proveedor proveedor = repositorio.Obtener<Proveedor>(proveedorId);

                if (proveedor == null)
                {
                    throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Empresas"));
                }
                if (proveedor.HistorialAprobaciones == null)
                {
                    proveedor.HistorialAprobaciones = new List<ProveedorHistorialAprobacion>();
                }
                int usuarioId = repositorio.Obtener<Usuario, int>(u => u.Mail == usuarioMail, x => x.Id);
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

                    UsuarioGranos usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == proveedor.Mail);

                    usuario.Roles.Clear();

                    Rol rolUsuarioGranos = ObtenerRolPorCodigo("GRAN");

                    usuario.Roles.Add(rolUsuarioGranos);
                }

                if (estado == EstadoAprobacion.Rechazado || estado == EstadoAprobacion.EdicionRequerida)
                {
                    proveedor.Observaciones = observacionParaElProveedor;
                }


                repositorio.GuardarCambios();

                try
                {
                    if (estado == EstadoAprobacion.Aprobado)
                    {
                        enviarMailAprobado(proveedor);
                    }
                    else if (estado == EstadoAprobacion.Rechazado)
                    {
                        enviarMailRechazado(proveedor, observacionParaElProveedor);

                    }
                    else if (estado == EstadoAprobacion.EdicionRequerida)
                    {
                        enviarMailEdicionRequerida(proveedor, observacionParaElProveedor);

                    }
                    return String.Format(SuccessMsg.EmpresaCambioEstadoOK, proveedor.RazonSocial);
                }
                catch (Exception)
                {
                    throw new InfoCustomException(String.Format(SuccessMsg.EmpresaCambioEstadoOK, proveedor.RazonSocial) + ". No se pudo enviar el mail al proveedor.");
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public Rol ObtenerRolPorCodigo(string codigo)
        {
            return repositorio.Obtener<Rol>(u => u.Codigo.Equals(codigo));
        }

        private void enviarMailEdicionRequerida(Proveedor proveedor, string observacionParaElProveedor)
        {
            string cuerpo = "Estimado: " + proveedor.RazonSocial + "\n\n"
                                        + "Su alta fue Observada." + "\n\n";
            if (!string.IsNullOrWhiteSpace(observacionParaElProveedor))
            {
                cuerpo += "Observaciones: " + observacionParaElProveedor;
            }
            string asunto = "Molinos Agro - Edición Requerida";
            EmailSender.EnviarMail(new List<string> { proveedor.Mail }, asunto, cuerpo, null, null, null, null);
        }

        private void enviarMailRechazado(Proveedor proveedor, string observacionParaElProveedor)
        {
            string cuerpo = "Estimado: " + proveedor.RazonSocial + "\n\n"
                    + "Su alta fue rechazada por Administración. Comunicarse con su comercial." + "\n\n";
            if (!string.IsNullOrWhiteSpace(observacionParaElProveedor))
            {
                cuerpo += "Observaciones: " + observacionParaElProveedor;
            }
            string asunto = "Molinos Agro - Solicitud Rechazada";
            EmailSender.EnviarMail(new List<string> { proveedor.Mail }, asunto, cuerpo, null, null, null, null);
        }

        private void enviarMailAprobado(Proveedor proveedor)
        {
            string cuerpo = "Estimado: " + proveedor.RazonSocial + "\n\n"
                      + "Su alta para operar en Molinos Agro fue aprobada exitosamente." + "\n\n";
            string asunto = "Molinos Agro - Alta Exitosa";

            EmailSender.EnviarMail(new List<string> { proveedor.Mail }, asunto, cuerpo, null, null, null, null);
        }

        public EstadoAprobacionDto GetEstadoAprobacion(string mail)
        {
            try
            {
                var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == mail);

                Proveedor proveedor = usuario.ObtenerProveedorActual();

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
