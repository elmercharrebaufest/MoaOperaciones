using Quartz.Util;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.AltaEmpresa;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
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

        public List<ProveedorAltaDto> GetEmpresas(List<int> IdTiposProveedor, string fechaInicio, string fechaFin)
        {
            DateTime fechaIncioDateTime, fechaFinDateTime, currentTime = DateTime.Now;
            try
            {
                fechaIncioDateTime = DateTime.Parse(fechaInicio);
            }
            catch
            {
                try
                {
                    fechaInicio = new string(fechaInicio.Where(c => c != '\u200E').ToArray());
                    fechaIncioDateTime = DateTime.Parse(fechaInicio);
                }
                catch (Exception e)
                {
                    throw new ValidationCustomException(String.Format(ErrorMsg.ErrorFechaInvalida, "inicio"), e);
                }
            }

            try
            {
                fechaFinDateTime = DateTime.Parse(fechaFin + " " + currentTime.TimeOfDay.ToString());
            }
            catch
            {
                try
                {
                    fechaFin = new string(fechaFin.Where(c => c != '\u200E').ToArray());
                    fechaFinDateTime = DateTime.Parse(fechaFin + " " + currentTime.TimeOfDay.ToString());
                }
                catch (Exception e)
                {
                    throw new ValidationCustomException(String.Format(ErrorMsg.ErrorFechaInvalida, "fin"), e);
                }
            }

            List<ProveedorAltaDto> proveedorDtos =
                repositorio
                    .Listar<Proveedor, ProveedorAltaDto>(proveedor => new ProveedorAltaDto
                    {
                        CodigoProveedor = proveedor.CodigoProveedor ?? "",
                        CUIT = proveedor.CUIT,
                        EstadoAprobacion = proveedor.EstadoAprobacion,
                        Id = proveedor.Id,
                        IdComercialDataAgro = proveedor.IdComercialDataAgro,
                        TipoProveedorNombre = proveedor.TipoProveedor.Nombre,
                        IdDataAgro = proveedor.IdDataAgro,
                        Mail = proveedor.Mail ?? "",
                        Observaciones = proveedor.Observaciones,
                        RazonSocial = proveedor.RazonSocial ?? "",
                        RazonSocialCorredor = proveedor.TipoProveedor.NombreCorto == "NG" ? "No granos" : (proveedor.ProveedorCorredor != null ? proveedor.ProveedorCorredor.RazonSocial : ""),

                        FechaSolicitud = proveedor.HistorialAprobaciones.Where(e => e.EstadoAprobacion == EstadoAprobacion.AprobacionPendiente).OrderBy(x => x.Fecha).FirstOrDefault().Fecha,

                        Comercial = proveedor.TipoProveedor.NombreCorto == "NG" ? proveedor.SolicitanteInterno : proveedor.Comercial,
                        EstadoSIPER = proveedor.EstadoSIPER,
                        AltaInterna = proveedor.AltaInterna,
                        IngresoAPlanta = proveedor.IngresoAPlanta,

                        UltimaEdicion = proveedor.HistorialAprobaciones.OrderByDescending(x => x.Fecha).FirstOrDefault().Fecha,

                        FechaAltaAceptada = proveedor.HistorialAprobaciones.Where(x => x.EstadoAprobacion == EstadoAprobacion.Aprobado || x.Observacion == "Alta aceptada").OrderByDescending(x => x.Fecha).FirstOrDefault().Fecha,

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
                        IdIngresoBruto = proveedor.IdIngresoBruto,
                        SiperObligatorio = proveedor.SiperObligatorio,
                        ContieneDocumentacionFisica = proveedor.ContieneDocumentacionFisica,
                        SISAEstadoCuit = proveedor.EstadoSISA
                    },
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
                                || x.EstadoAprobacion == EstadoAprobacion.DocumentacionPendiente
                                || x.EstadoAprobacion == EstadoAprobacion.AnalisisInterno
                            )
                            && x.HistorialAprobaciones.Count > 0
                            && IdTiposProveedor.Contains(x.TipoProveedor.Id)
                            && x.HistorialAprobaciones.OrderByDescending(h => h.Fecha).FirstOrDefault().Fecha >= fechaIncioDateTime
                            && x.HistorialAprobaciones.OrderByDescending(h => h.Fecha).FirstOrDefault().Fecha <= fechaFinDateTime
                        , 0, null, SustitucionMOAModel.Consultas.DirOrden.Asc
                    ).ToList();

            if (proveedorDtos.Count == 0)
            {
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Empresas"));
            }

            //Esto deberia hacerse a demanda cuando se abre el proveedor en el front y no traer toda el historial al pedo
            var historial = GetProveedorHistorialAprobacion(proveedorDtos.Select(a => a.Id).ToList());
            foreach (var proveedor in proveedorDtos)
            {
                proveedor.HistorialAprobaciones = historial.Where(a => a.Proveedor_Id == proveedor.Id).ToList();
            }

            return proveedorDtos;
        }

        public string VolverProveedorCanalDeAltas(string CUIT, string mailProveedor)
        {
            var proveedores = repositorio.Listar<Proveedor>(x =>
                x.CUIT == CUIT &&
                (string.IsNullOrEmpty(mailProveedor) || x.Mail == mailProveedor));

            if (proveedores.Count == 0)
            {
                return $"No se encontró ningún proveedor con CUIT {CUIT} y mail {mailProveedor}.";
            }
            if (proveedores.Count > 1)
            {
                return $"Se encontraron múltiples proveedores con CUIT {CUIT} y mail {mailProveedor}. Por favor, contacte a Soporte.";
            }

            proveedores[0].EstadoAprobacion = EstadoAprobacion.DocumentacionPendiente;
            repositorio.GuardarCambios();
            return "El proveedor ha sido exitosamente vuelto al canal de altas.";
        }

        private List<ProveedorHistorialAprobacionDto> GetProveedorHistorialAprobacion(List<int> IdProveedor)
        {

            try
            {
                List<ProveedorHistorialAprobacionDto> proveedorDtos =
                    repositorio
                        .Listar<ProveedorHistorialAprobacion, ProveedorHistorialAprobacionDto>(a => new ProveedorHistorialAprobacionDto
                        {
                            Id = a.Id,
                            EstadoAprobacion = a.EstadoAprobacion,
                            Fecha = a.Fecha,
                            Observacion = a.Observacion,
                            Usuario = a.Usuario.Mail,
                            ObservacionParaProveedor = a.ObservacionParaProveedor,
                            Proveedor_Id = a.Proveedor_Id
                        }
                        , x => IdProveedor.Contains(x.Proveedor_Id)
                         , 0, "Fecha", SustitucionMOAModel.Consultas.DirOrden.Asc
                        ).ToList();

                if (proveedorDtos.Count == 0)
                {
                    throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Empresas"));
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
                                          bool enviarMail,
                                          string razonSocial,
                                          string codigoCliente)
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
                    if (usuario == null)
                    {
                        if (proveedor.TipoProveedor.NombreCorto == "G")
                        {
                            usuario = new UsuarioGranos { Mail = proveedor.Mail, CUITRegistro = proveedor.CUIT, Habilitado = true, TipoUsuario = proveedor.TipoProveedor, Roles = new List<Rol>(), SeccionesVisitadas = "", AceptoTyC = false };
                        }
                        else if (proveedor.TipoProveedor.NombreCorto == "NG")
                        {
                            usuario = new UsuarioNoGranos { Mail = proveedor.Mail, CUITRegistro = proveedor.CUIT, Habilitado = true, TipoUsuario = proveedor.TipoProveedor, Roles = new List<Rol>(), SeccionesVisitadas = "", AceptoTyC = false };
                        }
                        else
                        {
                            usuario = new Usuario { Mail = proveedor.Mail, CUITRegistro = proveedor.CUIT, Habilitado = true, TipoUsuario = proveedor.TipoProveedor, Roles = new List<Rol>(), SeccionesVisitadas = "", AceptoTyC = false };
                        }
                        repositorio.Agregar(usuario);
                    }
                    if (usuario.Roles == null)
                    {
                        usuario.Roles = new List<Rol>();
                    }
                    if (usuario.Proveedores == null)
                    {
                        usuario.Proveedores = new List<Proveedor>();
                    }
                    if (!usuario.TieneProveedor(proveedor.CodigoProveedor))
                    {
                        usuario.Proveedores.Add(proveedor);
                    }
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

                        case "Cliente":
                            var rolUsuarioCliente = ObtenerRolPorCodigo("CLIENT");

                            if (usuario != null)
                            {
                                //Para que no borre los roles una vez aprobado.
                                //usuario.RemoverRoles();
                                usuario.AgregarRol(rolUsuarioCliente);
                            }

                            proveedor.CodigoProveedor = codigoCliente;
                            proveedor.RazonSocial = razonSocial;

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

                SustitucionMOAWS.DataAgroServices.ResultadoValidarProveedorComercial resultadoValidarProveedorComercial = dataAgroService.ObtenerValidarCUITProveedorGranos(proveedor.CUIT);
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

            if (proveedor.AltaInterna ?? false)
            {
                if (proveedor.TipoProveedor.NombreCorto == "G")
                {
                    var usuario = repositorio.Obtener<Usuario>(U => U.Id == proveedor.IdSolicitanteInternoAltaGranos);
                    copia.Add(usuario.Mail);
                }
            }

            EmailSender.EnviarMail(new List<string> { proveedor.Mail }, asunto, cuerpo, copia, null, null, null);
        }

        private void EnviarMailRechazado(Proveedor proveedor, string observacionParaElProveedor, List<string> copia)
        {
            var cuerpoTemplate = File.ReadAllText(EMAIL_TEMPLATE);
            var cuerpo = string.Format(cuerpoTemplate, proveedor.RazonSocial, "rechazada", !string.IsNullOrWhiteSpace(observacionParaElProveedor) ? observacionParaElProveedor : "-");
            string asunto = "Molinos Agro - Solicitud Rechazada";

            if (proveedor.AltaInterna ?? false)
            {
                if (proveedor.TipoProveedor.NombreCorto == "G")
                {
                    var usuario = repositorio.Obtener<Usuario>(U => U.Id == proveedor.IdSolicitanteInternoAltaGranos);
                    copia.Add(usuario.Mail);
                }
            }

            EmailSender.EnviarMail(new List<string> { proveedor.Mail }, asunto, cuerpo, copia, null, null, null);
        }

        private void EnviarMailAprobado(Proveedor proveedor, string observacionParaElProveedor, List<string> copia)
        {
            var cuerpoTemplate = File.ReadAllText(EMAIL_TEMPLATE);
            var cuerpo = string.Format(cuerpoTemplate, proveedor.RazonSocial, "aprobada", !string.IsNullOrWhiteSpace(observacionParaElProveedor) ? observacionParaElProveedor : "-");
            string asunto;

            if (proveedor.TipoProveedor.NombreCorto == "NG")
            {
                asunto = "Molinos Agro – Alta generada con éxito";
            }
            else
            {
                asunto = "Molinos Agro – Alta generada pendiente de envío documentación original.";
            }

            if (proveedor.AltaInterna ?? false)
            {
                if (proveedor.TipoProveedor.NombreCorto == "G")
                {
                    var usuario = repositorio.Obtener<Usuario>(U => U.Id == proveedor.IdSolicitanteInternoAltaGranos);
                    copia.Add(usuario.Mail);
                }
            }

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
                    Observaciones = proveedor.Observaciones.IsNullOrWhiteSpace() ? "" : proveedor.Observaciones,
                    DocumentacionFisica = proveedor.ContieneDocumentacionFisica ?? false
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

        public string SolicitarInformacion(int proveedorId, string usuarioMail)
        {
            var proveedor = repositorio.Obtener<Proveedor>(proveedorId);
            int usuarioID = repositorio.Obtener<Usuario, int>(u => u.Mail == usuarioMail, x => x.Id);

            //obtengo el mensaje del ultimo edicion requerido
            var ultimaRequerido = proveedor.HistorialAprobaciones.LastOrDefault(x => x.EstadoAprobacion == EstadoAprobacion.EdicionRequerida);
            string mensajeNotificacion = ultimaRequerido != null ? ultimaRequerido.ObservacionParaProveedor : "Seguimos esperando la documentación solicitada ";

            NotificarProveedor(EstadoAprobacion.EdicionRequerida, mensajeNotificacion, proveedor);

            proveedor.HistorialAprobaciones.Add(
                new ProveedorHistorialAprobacion
                {
                    Fecha = DateTime.Now,
                    EstadoAprobacion = EstadoAprobacion.EdicionRequerida,
                    Observacion = ultimaRequerido.Observacion,
                    ObservacionParaProveedor = mensajeNotificacion,
                    Proveedor_Id = proveedorId,
                    Usuario_Id = usuarioID
                }
            );

            repositorio.GuardarCambios();

            return "Proveedor notificado correctamente";
        }

        public string AgregarObservacion(int proveedorId, string observacion, string usuarioMail)
        {
            Proveedor proveedor = repositorio.Obtener<Proveedor>(proveedorId);

            //Como del front estoy enviando la info en encoding URI tengo que decodificarlo.
            observacion = Uri.UnescapeDataString(observacion);

            if (proveedor == null)
            {
                throw new InfoCustomException(string.Format(InfoMsg.SinRegistros, "Empresas"));
            }
            if (proveedor.HistorialAprobaciones == null)
            {
                proveedor.HistorialAprobaciones = new List<ProveedorHistorialAprobacion>();
            }

            int usuarioId = repositorio.Obtener<Usuario, int>(u => u.Mail == usuarioMail, x => x.Id);
            //En caso de no venir observación se coloca el nuevo estado para que pueda visualizarse al menos ese paso a nuevo estado
            proveedor.HistorialAprobaciones.Add(
                new ProveedorHistorialAprobacion
                {
                    Fecha = DateTime.Now,
                    EstadoAprobacion = proveedor.EstadoAprobacion,
                    Observacion = observacion,
                    Proveedor_Id = proveedorId,
                    Usuario_Id = usuarioId,
                    ObservacionParaProveedor = "-"
                }
            );

            repositorio.GuardarCambios();

            return SuccessMsg.ObservacionAgregadaOK;
        }

        public ExistenciaEmpresaResponse VerificarExistenciaEmpresa(string cuit)
        {
            var proveedores = this.repositorio.Listar<Proveedor>(p => p.CUIT == cuit);

            if (!(proveedores?.Any() ?? false))
            {
                return new ExistenciaEmpresaResponse { Error = $"No se encontró el CUIT {cuit} en el sistema." };
            }

            var response = new ExistenciaEmpresaResponse
            {
                EmpresasExistentes = proveedores.Select(x => new EmpresaExistenciaVerificada
                {
                    Codigo = x.CodigoProveedor,
                    Id = x.Id,
                    Mail = x.Mail
                }).ToList()
            };

            if (proveedores.Count == 1)
            {
                response.Mensaje = $"El cuit {cuit} se encuentra registrado en el sistema con Razón social '{proveedores[0].RazonSocial}', Código de proveedor {proveedores[0].CodigoProveedor}.";
            }
            else
            {
                response.Mensaje = $"Se encontraron varios proveedores con el CUIT {cuit}.";
            }
            return response;
        }

        public int ModificarEstadoProveedor(int proveedorId, string nuevoEstado, string emailUsuario)
        {
            var proveedor = this.repositorio.Obtener<Proveedor>(proveedorId);

            if (proveedor == null)
            {
                throw new ValidationCustomException("No se encontró el proveedor en el sistema.");
            }

            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == emailUsuario);
            if (!usuario.TienePermiso(PermisoEnum.ModificarEstadoProveedor))
            {
                throw new ValidationCustomException("Usuario sin permisos.");
            }
            var nuevoEstadoEnum = EstadoAprobacionHelper.FromStr(nuevoEstado);
            proveedor.EstadoAprobacion = nuevoEstadoEnum;

            var historialAprobacion = new ProveedorHistorialAprobacion
            {
                EstadoAprobacion = nuevoEstadoEnum,
                Observacion = "Se modifico manualmente el estado",
                Usuario = usuario,
                Proveedor = proveedor,
                Fecha = DateTime.Now
            };

            repositorio.Agregar<ProveedorHistorialAprobacion>(historialAprobacion);
            proveedor.HistorialAprobaciones.Add(historialAprobacion);
            repositorio.GuardarCambios();
            return (int)nuevoEstadoEnum;
        }
    }
}
