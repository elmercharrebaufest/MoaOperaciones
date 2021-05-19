using Quartz.Util;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.ViewModel.AltaEmpresa;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.DataAgroServices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace SustitucionMOAUtils.Services
{
    public class AltaEmpresaService : IAltaEmpresaService
    {

        protected readonly IRepositorio repositorio;
        protected readonly IDataAgroService dataAgroService;

        private static readonly string EMAIL_TEMPLATE = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "EstadoAlta.html");

        public AltaEmpresaService(IRepositorio repositorio, IDataAgroService dataAgroService)
        {
            this.repositorio = repositorio;
            this.dataAgroService = dataAgroService;
        }

        public List<ProveedorAltaDto> GetEmpresas(int IdTipoProveedor)
        {
            try
            {
                //{
                //    List<Proveedor> proveedores = repositorio.Listar<Proveedor>(
                //                     x =>
                //                     (x.EstadoAprobacion == EstadoAprobacion.AprobacionPendiente
                //                        || x.EstadoAprobacion == EstadoAprobacion.AnalisisDeNosis
                //                        || x.EstadoAprobacion == EstadoAprobacion.EtapaFinal
                //                        || x.EstadoAprobacion == EstadoAprobacion.EdicionRequerida
                //                        || x.EstadoAprobacion == EstadoAprobacion.Aprobado
                //                        || x.EstadoAprobacion == EstadoAprobacion.Rechazado
                //                        || x.EstadoAprobacion == EstadoAprobacion.DeshabilitadoEnDataAgro
                //                        || x.EstadoAprobacion == EstadoAprobacion.PendienteAprobacionCompras
                //                        || x.EstadoAprobacion == EstadoAprobacion.RechazadoPorCompras
                //                        || x.EstadoAprobacion == EstadoAprobacion.AltaIncompleta
                //                        || x.EstadoAprobacion == EstadoAprobacion.SinAlta
                //                        || x.EstadoAprobacion == EstadoAprobacion.DocumentacionPendiente)
                //                    && x.HistorialAprobaciones.Count > 0
                //                    && x.TipoProveedor.Id == (IdTipoProveedor > 0 ? IdTipoProveedor : x.TipoProveedor.Id)
                //                    );

                List<ProveedorAltaDto> proveedorDtos = 
                    repositorio
                        .Listar<Proveedor>(
                                 x =>
                                 (x.EstadoAprobacion == EstadoAprobacion.AprobacionPendiente
                                    || x.EstadoAprobacion == EstadoAprobacion.AnalisisDeNosis
                                    || x.EstadoAprobacion == EstadoAprobacion.EtapaFinal
                                    || x.EstadoAprobacion == EstadoAprobacion.EdicionRequerida
                                    || x.EstadoAprobacion == EstadoAprobacion.Aprobado
                                    || x.EstadoAprobacion == EstadoAprobacion.Rechazado
                                    || x.EstadoAprobacion == EstadoAprobacion.DeshabilitadoEnDataAgro
                                    || x.EstadoAprobacion == EstadoAprobacion.PendienteAprobacionCompras
                                    || x.EstadoAprobacion == EstadoAprobacion.RechazadoPorCompras
                                    || x.EstadoAprobacion == EstadoAprobacion.AltaIncompleta
                                    || x.EstadoAprobacion == EstadoAprobacion.SinAlta
                                    || x.EstadoAprobacion == EstadoAprobacion.DocumentacionPendiente)
                                && x.HistorialAprobaciones.Count > 0
                                && x.TipoProveedor.Id == (IdTipoProveedor > 0 ? IdTipoProveedor : x.TipoProveedor.Id))
                        .Select(proveedor => new ProveedorAltaDto
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
                                    RazonSocialCorredor = proveedor.TipoProveedor.NombreCorto == "NG" ? "No granos" : (proveedor.ProveedorCorredor != null ? proveedor.ProveedorCorredor.RazonSocial : ""),
                                    FechaSolicitud = proveedor.FechaSolicitud,
                                    Comercial = proveedor.TipoProveedor.NombreCorto == "NG" ? proveedor.SolicitanteInterno : proveedor.Comercial,
                                    EstadoSIPER = proveedor.EstadoSIPER,
                                    AltaInterna = proveedor.AltaInterna,
                                    IngresoAPlanta = proveedor.IngresoAPlanta,
                                    UltimaEdicion = proveedor.HistorialAprobaciones.FirstOrDefault() != null ? proveedor.HistorialAprobaciones.OrderByDescending(x => x.Fecha).FirstOrDefault().Fecha : (DateTime?)null,
                                    HistorialAprobaciones = proveedor.HistorialAprobaciones?.Select(a => new ProveedorHistorialAprobacionDto
                                    {
                                        Id = a.Id,
                                        EstadoAprobacionDescripcion = a.EstadoAprobacion.ToFriendlyString(),
                                        Fecha = a.Fecha,
                                        Observacion = a.Observacion,
                                        Usuario = a.Usuario.Mail,
                                        ObservacionParaProveedor = a.ObservacionParaProveedor
                                    }).ToList(),
                                    IdTipoUsuario = proveedor.TipoProveedor.Id,
                                    CBU = proveedor.CBU,
                                    CondicionDePago = proveedor.CondicionDePago,
                                    FacturacionAnual = proveedor.FacturacionAnual,
                                    OrganizacionDeCompra = proveedor.OrganizacionDeCompra,
                                    RazonDeEleccion = proveedor.RazonDeEleccion,
                                    RealizarAnalisisNOSIS = proveedor.RealizarAnalisisNOSIS ?? false,
                                    Rubro = proveedor.Rubro != null ? proveedor.Rubro.Nombre : "",
                                    RequiereVerificacionCompras = proveedor.RequiereVerificacionCompras ?? false,
                                    SolicitanteInterno = proveedor.SolicitanteInterno,
                                    ServicioPrestado = proveedor.ServicioPrestado,
                                    Telefono = proveedor.Telefono,
                                    IdSituacionIVA = proveedor.IdSituacionIVA,
                                    SituacionIVA = ((SituacionIVA)(proveedor.IdSituacionIVA ?? 0)).ToFriendlyString(),
                                    IdIngresoBruto = proveedor.IdIngresoBruto,
                                    IngresoBruto = ((IngresosBrutos)(proveedor.IdIngresoBruto ?? 0)).ToFriendlyString(),
                                    SiperObligatorio = proveedor.SiperObligatorio

                                })
                        .ToList();

                if (proveedorDtos.Count == 0)
                {
                    throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Empresas"));
                }

                //TODO: Deprecar esto y obtener la razon social a través de la FK del proveedor al proveedor que lo dio de alta
                foreach (var proveedorDto in proveedorDtos)
                {
                    //var corredorAsociado = repositorio.Listar<Proveedor>(p => p.CodigoProveedor.Contains("C") && p.Mail == proveedorDto.Mail).FirstOrDefault();
                    //if (corredorAsociado != null) proveedorDto.RazonSocialCorredor = corredorAsociado.RazonSocial;

                    if ((proveedorDto.EstadoAprobacion == EstadoAprobacion.AprobacionPendiente
                        || proveedorDto.EstadoAprobacion == EstadoAprobacion.AnalisisDeNosis
                        || proveedorDto.EstadoAprobacion == EstadoAprobacion.EtapaFinal
                        || proveedorDto.EstadoAprobacion == EstadoAprobacion.EdicionRequerida
                        || proveedorDto.EstadoAprobacion == EstadoAprobacion.Aprobado)
                        &&
                        (proveedorDto.IdTipoUsuario == 2 || proveedorDto.IdTipoUsuario == 4)
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

        public string GuardarSIPER(int proveedorId, string estadoSIPER)
        {
            try
            {
                Proveedor proveedor = repositorio.Obtener<Proveedor>(proveedorId);

                if (proveedor == null)
                {
                    throw new InfoCustomException(string.Format(InfoMsg.SinRegistros, "Empresas"));
                }

                if (!string.IsNullOrWhiteSpace(estadoSIPER))
                {
                    proveedor.EstadoSIPER = estadoSIPER;
                }
                else
                {
                    throw new Exception("El SIPER no puede ser nulo o un espacio en blanco.");
                }

                repositorio.GuardarCambios();

                return "Se guardo correctamente.";
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

                //Como del front estoy enviando la info en encoding URI tengo que decodificarlo.
                observacion = Uri.UnescapeDataString(observacion);
                observacionParaElProveedor = Uri.UnescapeDataString(observacionParaElProveedor);

                if (proveedor == null)
                {
                    throw new InfoCustomException(string.Format(InfoMsg.SinRegistros, "Empresas"));
                }
                if (proveedor.HistorialAprobaciones == null)
                {
                    proveedor.HistorialAprobaciones = new List<ProveedorHistorialAprobacion>();
                }

                int usuarioId = repositorio.Obtener<Usuario, int>(u => u.Mail == usuarioMail, x => x.Id);

                if (new EstadoAprobacion[] { EstadoAprobacion.AnularRechazo, EstadoAprobacion.AnularObservacion }.Contains(estado))
                {
                    //Lo inicializo así por las dudas, en el peor de los casos queda igual
                    EstadoAprobacion estadoAnterior = estado;
                    var historialAnterior = proveedor.HistorialAprobaciones.Where(h => !new EstadoAprobacion[] { EstadoAprobacion.Rechazado, EstadoAprobacion.EdicionRequerida }.Contains(h.EstadoAprobacion)).OrderByDescending(x => x.Fecha).FirstOrDefault();

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

                //En caso de no venir observación se coloca el nuevo estado para que pueda visualizarse al menos ese paso a nuevo estado
                proveedor.HistorialAprobaciones.Add(
                    new ProveedorHistorialAprobacion
                    {
                        Fecha = DateTime.Now,
                        EstadoAprobacion = estado,
                        Observacion = string.IsNullOrEmpty(observacion) ? estado.ToFriendlyString() : observacion,
                        Proveedor_Id = proveedorId,
                        Usuario_Id = usuarioId,
                        ObservacionParaProveedor = observacionParaElProveedor
                    }
                );


                if (estado.Equals(EstadoAprobacion.Aprobado))
                {

                    var usuario = repositorio.Obtener<Usuario>(u => u.Mail == proveedor.Mail);
                    var rolUsuarioGranos = ObtenerRolPorCodigo("GRAN");

                    switch (proveedor.TipoProveedor.Nombre)
                    {
                        case "Granos":

                            usuario.RemoverRol("NUEG");

                            if (!usuario.Roles.Contains(rolUsuarioGranos))
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
                        case "No Granos":
                            var rolUsuarioNoGranos = ObtenerRolPorCodigo("NOGRAN");

                            proveedor.CodigoProveedor = FormatearCodigoProveedor(proveedor.CUIT);
                            if (usuario != null)
                            {
                                usuario.RemoverRol("NUENOGRAN");
                                if (!usuario.Roles.Contains(rolUsuarioNoGranos))
                                    usuario.AgregarRol(rolUsuarioNoGranos);
                            }

                            break;
                    }

                }

                if (estado == EstadoAprobacion.AnularAprobacion)
                {
                    var usuario = repositorio.Obtener<Usuario>(u => u.Mail == proveedor.Mail);
                    var rolUsuarioNuevoGranos = ObtenerRolPorCodigo("NUEG");

                    switch (proveedor.TipoProveedor.Nombre)
                    {
                        case "Granos":
                            {
                                var actualizarRoles = true;

                                //Si es multifirma y tiene algun otro proveedor aprobado, no le tocamos los roles. Esto no debería ocurrir nunca, pero no esta mal tenerlo en cuenta
                                if (usuario.Roles.Where(r => r.Codigo == "MF").Any())
                                {
                                    if (usuario.Proveedores.Where(p => p.EstadoAprobacion == 0 && p.CUIT != proveedor.CUIT).Any())
                                    {
                                        actualizarRoles = false;
                                    }
                                }

                                if (actualizarRoles)
                                {
                                    usuario.RemoverRoles();
                                    usuario.AgregarRol(rolUsuarioNuevoGranos);
                                }
                            }

                            break;

                        case "Corredor":
                            {
                                var actualizarRoles = true;

                                if (usuario.Proveedores.Where(p => p.EstadoAprobacion == 0 && p.CUIT != proveedor.CUIT).Any())
                                {
                                    actualizarRoles = false;
                                }

                                if (actualizarRoles)
                                {
                                    var rolNuevoCorredor = ObtenerRolPorCodigo("NUECORR");
                                    usuario.AgregarRol(rolNuevoCorredor);
                                    usuario.RemoverRol("CORR");
                                }
                            }
                            break;
                        case "No Granos":
                            {
                                var nuevoNoGranos = ObtenerRolPorCodigo("NUENOGRAN");
                                usuario.RemoverRoles();
                                usuario.AgregarRol(nuevoNoGranos);
                            }
                            break;
                    }
                    estado = EstadoAprobacion.EtapaFinal;
                }

                ////Si el proveedor no tiene que pasar por analisis de nosis, lo mando al estado final directamente
                //if (estado == EstadoAprobacion.AprobacionPendiente)
                //{
                //    if (proveedor.TipoProveedor.NombreCorto == "NG" && !(proveedor.RealizarAnalisisNOSIS ?? false))
                //        estado = EstadoAprobacion.EtapaFinal;
                //}


                if (estado == EstadoAprobacion.Rechazado || estado == EstadoAprobacion.EdicionRequerida)
                {
                    proveedor.Observaciones = observacionParaElProveedor;
                }

                if (!string.IsNullOrWhiteSpace(estadoSIPER))
                {
                    proveedor.EstadoSIPER = estadoSIPER;
                }

                proveedor.EstadoAprobacion = estado;


                repositorio.GuardarCambios();

                var mailEnviado = false;
                if (enviarMail)
                {
                    mailEnviado = NotificarProveedor(estado, observacionParaElProveedor, proveedor);
                }

                string mensajeResultado = "";
                if (estado == EstadoAprobacion.AprobacionPendiente && proveedor.TipoProveedor.NombreCorto == "NG")
                {
                    mensajeResultado = string.Format(SuccessMsg.EmpresaCambioEstadoCompras, proveedor.RazonSocial);
                }
                else
                {
                    mensajeResultado = string.Format(SuccessMsg.EmpresaCambioEstadoOK, proveedor.RazonSocial);
                }

                if (enviarMail && !mailEnviado)
                {
                    mensajeResultado = string.Concat(mensajeResultado, " No se pudo enviar mail al proveedor.");
                    throw new InfoCustomException(mensajeResultado);
                }

                return mensajeResultado;

            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool NotificarProveedor(EstadoAprobacion estado, string observacionParaElProveedor, Proveedor proveedor)
        {
            try
            {
                var copia = new List<string>();

                ResultadoValidarProveedorComercial resultadoValidarProveedorComercial = dataAgroService.ObtenerValidarCUITProveedorGranos(proveedor.CUIT);
                if (resultadoValidarProveedorComercial != null)
                {
                    if (!string.IsNullOrWhiteSpace(resultadoValidarProveedorComercial.ComercialMail))
                    {
                        copia.Add(resultadoValidarProveedorComercial.ComercialMail);
                    }
                }

                if (!string.IsNullOrWhiteSpace(proveedor.SolicitanteInterno))
                {
                    copia.Add(proveedor.SolicitanteInterno);
                }

                if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["EmailToDocumentacion"]))
                {
                    copia.Add(ConfigurationManager.AppSettings["EmailToDocumentacion"]);
                }

                if (estado == EstadoAprobacion.Aprobado)
                {
                    EnviarMailAprobado(proveedor, observacionParaElProveedor, copia);
                }
                else if (estado == EstadoAprobacion.Rechazado)
                {
                    EnviarMailRechazado(proveedor, observacionParaElProveedor, copia);

                }
                else if (estado == EstadoAprobacion.EdicionRequerida)
                {
                    EnviarMailEdicionRequerida(proveedor, observacionParaElProveedor, copia);
                }

                return true;
            }
            catch
            {
                return false;
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

        private void EnviarMailAprobado(Proveedor proveedor, string observacionParaElProveedor, List<string> copia)
        {
            var cuerpoTemplate = File.ReadAllText(EMAIL_TEMPLATE);
            var cuerpo = string.Format(cuerpoTemplate, proveedor.RazonSocial, "aprobada", !string.IsNullOrWhiteSpace(observacionParaElProveedor) ? observacionParaElProveedor : "-");
            string asunto = "Molinos Agro – Alta generada pendiente de envío documentación original.";
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


        private string FormatearCodigoProveedor(string CUIT)
        {
            return string.Concat("00", CUIT.Substring(2, 8));
        }

        public string SolicitarInformacion(int proveedorId)
        {
            var proveedor = repositorio.Obtener<Proveedor>(proveedorId);

            NotificarProveedor(EstadoAprobacion.EdicionRequerida, "Seguimos esperando la documentación solicitada ", proveedor);

            return "Proveedor notificado correctamente";
        }
    }
}
