using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.WSMapMOA.Noticia;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace SustitucionMOAUtils.Services
{
    public class AzureB2CService : IAzureB2CService
    {
        protected readonly IRepositorio repositorio;
        protected readonly IDataAgroService dataAgroService;

        public AzureB2CService(IRepositorio repositorio, IDataAgroService dataAgroService)
        {
            this.repositorio = repositorio;
            this.dataAgroService = dataAgroService;
        }

        public Usuario LoguearUsuario(string mail, string CUIT, string granosFlag)
        {
            Usuario usuario = new Usuario { Mail = mail, CUITRegistro = CUIT, SeccionesVisitadas = "" };

            if (ExisteUsuario(usuario))
            {
                usuario = BuscarUsuarioPorMail(mail);
            }
            else
            {
                RegistrarUsuario(mail, CUIT, granosFlag, ref usuario);
            }

            usuario.UltimoLogin = DateTime.Now;

            repositorio.GuardarCambios();

            return usuario;
        }

        private Usuario RegistrarUsuario(string mail, string CUIT, string granosFlag, ref Usuario usuario)
        {
            // Si existe un proveedor no granos con el mismo CUIT, automaticamente le cambiamos el tipo a no granos. Con esto nos evitamos tener que editarlos cuando se registraron mal
            if (VerificarUsuarioNoGranos(CUIT))
            {
                granosFlag = "no granos";
            }

            switch (granosFlag.ToLower())
            {
                case "granos":
                    UsuarioGranos usuarioGranos = new UsuarioGranos { Mail = mail, CUITRegistro = CUIT, SeccionesVisitadas = "" };

                    RegistrarUsuarioGranos(ref usuarioGranos);
                    usuario = usuarioGranos;
                    break;

                case "no granos":
                    UsuarioNoGranos usuarioNoGranos = new UsuarioNoGranos { Mail = mail, CUITRegistro = CUIT, SeccionesVisitadas = ""  };

                    usuarioNoGranos.TipoUsuario = ObtenerTipoPorNombreCorto("NG");
                    RegistrarUsuarioNoGranos(ref usuarioNoGranos);
                    usuario = usuarioNoGranos;

                    break;

                case "corredor":
                    Usuario usuarioCorredor = new Usuario { Mail = mail, CUITRegistro = CUIT, SeccionesVisitadas = "" };

                    RegistrarUsuarioCorredor(ref usuarioCorredor);

                    usuario = usuarioCorredor;
                    break;

                case "cliente":
                    Usuario usuarioCliente = new Usuario { Mail = mail, CUITRegistro = CUIT, SeccionesVisitadas = "" };

                    usuarioCliente.TipoUsuario = ObtenerTipoPorNombreCorto("CLI");

                    RegistrarUsuarioCliente(ref usuarioCliente);

                    usuario = usuarioCliente;
                    break;
            }

            return usuario;
        }

        private bool VerificarUsuarioNoGranos(string CUIT)
        {
            return repositorio.Existe<Proveedor>(p => p.CUIT == CUIT && p.TipoProveedor.NombreCorto == "NG");
        }

        private Usuario BuscarUsuarioPorMail(string mail)
        {
            return repositorio.Obtener<Usuario>(u => u.Mail == mail);
        }

        public NoticiasDetallesWSMOAResponse getNoticias(string proveedor)
        {
            try
            {
                return new NoticiasDetalleConsumerMOA().request(proveedor, DateTime.Now.ToString("yyyy-MM-dd"));
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }


        public bool RegistrarUsuarioGranos(ref UsuarioGranos usuario)
        {
            Proveedor proveedor = new Proveedor
            {
                CUIT = usuario.CUITRegistro,
                EstadoAprobacion = EstadoAprobacion.DocumentacionPendiente,
                FechaSolicitud = DateTime.Now
            };
            var result = dataAgroService.ObtenerValidarCUITProveedorGranos(proveedor.CUIT);
            if (result != null)
            {
                proveedor.EstadoSISA = result.ProveedorSISAEstadoCuit;
            }
            return ValidarCUITProveedor(ref usuario, proveedor);
        }

        public bool RegistrarUsuarioNoGranos(ref UsuarioNoGranos usuario)
        {
            Rol nuevoNoGranos = ObtenerRolPorCodigo("NUENOGRAN");

            usuario.Roles = new List<Rol>
            {
                nuevoNoGranos
            };

            usuario.Proveedores = new List<Proveedor>();

            string cuit = usuario.CUITRegistro;
            string mailUsuario = usuario.Mail;

            Proveedor proveedor = new Proveedor
            {
                CUIT = usuario.CUITRegistro,
                EstadoAprobacion = EstadoAprobacion.AltaIncompleta,
                Observaciones = "Comunicarse con su contratante.",
                Mail = usuario.Mail,
                TipoProveedor = ObtenerTipoPorNombreCorto("NG"),
                FechaSolicitud = DateTime.Now
            };

            proveedor.HistorialAprobaciones = new List<ProveedorHistorialAprobacion>
            {
                new ProveedorHistorialAprobacion()
                {
                    Fecha = DateTime.Now,
                    EstadoAprobacion = EstadoAprobacion.AltaIncompleta,
                    Observacion = "Registro de usuario",
                    Usuario_Id = usuario.Id
                }
            };

            if (repositorio.Existe<Proveedor>(x=> x.CUIT == cuit && x.Mail == mailUsuario))
            {
                proveedor = repositorio.Obtener<Proveedor>(x => x.CUIT == cuit && x.Mail == mailUsuario); 
            }

            if (proveedor.EstadoAprobacion == EstadoAprobacion.Aprobado)
            {
                var rolUsuarioNoGranos = ObtenerRolPorCodigo("NOGRAN");

                usuario.RemoverRoles();
                usuario.AgregarRol(rolUsuarioNoGranos);
            }

            usuario.Proveedores.Add(proveedor);

            usuario.Habilitado = true;

            repositorio.Agregar(usuario);
            return repositorio.GuardarCambios() == 1;
        }

        public bool RegistrarUsuarioCorredor(ref Usuario usuario)
        {
            var infoProveedor = ObtenerInfoProveedorDA(usuario.CUITRegistro, true);

            Proveedor proveedor = new Proveedor
            {
                CUIT = usuario.CUITRegistro,
                EstadoAprobacion = EstadoAprobacion.Aprobado,
                CodigoProveedor = FormatearCodigoCorredor(usuario.CUITRegistro),
                TipoProveedor = ObtenerTipoPorNombreCorto("CORR"),
                FechaSolicitud = DateTime.Now

            };
            usuario.Roles = new List<Rol>();
            usuario.Proveedores = new List<Proveedor>();
            usuario.TipoUsuario = ObtenerTipoPorNombreCorto("CORR");

            proveedor.Mail = usuario.Mail;

            if (infoProveedor != null)
            {
                if (!infoProveedor.HayError)
                {
                    if (infoProveedor.ProveedorMails.Contains(usuario.Mail, StringComparer.OrdinalIgnoreCase) || bool.Parse(ConfigurationManager.AppSettings["EsLocal"]))
                    {
                        proveedor.IdComercialDataAgro = infoProveedor.ComercialId;
                        proveedor.EstadoSISA = infoProveedor.ProveedorSISAEstadoCuit;
                        proveedor.IdDataAgro = infoProveedor.ProveedorId;
                        proveedor.RazonSocial = infoProveedor.ProveedorRazonSocial;
                        proveedor.CodigoProveedor = FormatearCodigoCorredor(proveedor.CUIT);
                        proveedor.EstadoAprobacion = EstadoAprobacion.Aprobado;

                        proveedor.Comercial = string.Concat(infoProveedor.ComercialNombres, " ", infoProveedor.ComercialApellido);

                        if (infoProveedor.ProveedorOperando)
                        {
                            var rolGranos = ObtenerRolPorCodigo("GRAN");
                            var rolCorredor = ObtenerRolPorCodigo("CORR");

                            usuario.AgregarRol(rolGranos);
                            usuario.AgregarRol(rolCorredor);
                        }
                        else
                        {
                            var rolNuevoCorredor = ObtenerRolPorCodigo("NUECORR");
                            usuario.AgregarRol(rolNuevoCorredor);
                        }
                    }
                    else
                    {
                        Rol rolDesabilitado = ObtenerRolPorCodigo("DDAG");
                        usuario.Roles.Add(rolDesabilitado);

                        proveedor.EstadoAprobacion = EstadoAprobacion.DeshabilitadoEnDataAgro;

                        var hist = new ProveedorHistorialAprobacion
                        {
                            Fecha = DateTime.Now,
                            Proveedor_Id = proveedor.Id,
                            Usuario_Id = usuario.Id,
                            EstadoAprobacion = proveedor.EstadoAprobacion,
                            Observacion = "El mail no coincide con el registrado en Data Agro"
                        };
                        repositorio.Agregar(hist);

                        proveedor.Observaciones = "El mail del registro no se encuentra habilitado. Comunicarse con su comercial.";
                    }
                }
                else
                {
                    Rol rolDesabilitado = ObtenerRolPorCodigo("DDAG");
                    usuario.Roles.Add(rolDesabilitado);
                    proveedor.EstadoAprobacion = EstadoAprobacion.SinAlta;

                    var hist = new ProveedorHistorialAprobacion
                    {
                        Fecha = DateTime.Now,
                        Proveedor_Id = proveedor.Id,
                        Usuario_Id = usuario.Id,
                        EstadoAprobacion = proveedor.EstadoAprobacion,
                        Observacion = infoProveedor.ListaErrores.First().Message
                    };
                    repositorio.Agregar(hist);

                    proveedor.Observaciones = "El mail del registro no se encuentra habilitado. Comunicarse con su comercial.";
                }
            }
            else
            {
                Rol rolDesabilitado = ObtenerRolPorCodigo("DDAG");
                usuario.Roles.Add(rolDesabilitado);
                proveedor.EstadoAprobacion = EstadoAprobacion.DeshabilitadoEnDataAgro;

                var hist = new ProveedorHistorialAprobacion
                {
                    Fecha = DateTime.Now,
                    Proveedor_Id = proveedor.Id,
                    Usuario_Id = usuario.Id,
                    EstadoAprobacion = proveedor.EstadoAprobacion,
                    Observacion = "Ocurrió un error comunicandose con Data Agro"
                };
                repositorio.Agregar(hist);

                proveedor.Observaciones = "El mail del registro no se encuentra habilitado. Comunicarse con su comercial.";
            }
            usuario.Proveedores.Add(proveedor);
            usuario.Habilitado = true;

            repositorio.Agregar(usuario);

            return repositorio.GuardarCambios() == 1;
        }

        public bool RegistrarUsuarioGenerico(ref Usuario usuario)
        {
            Rol rolUsuarioNoImplementado = ObtenerRolPorCodigo("NOIMP");

            usuario.Roles = new List<Rol>
            {
                rolUsuarioNoImplementado
            };

            usuario.Proveedores = new List<Proveedor>();

            Proveedor proveedor = new Proveedor
            {
                CUIT = usuario.CUITRegistro,
                EstadoAprobacion = EstadoAprobacion.AunNoImplementado,
                Observaciones = "El tipo de usuario seleccionado aún no ha sido implementado. Contactese con su comercial."
            };

            usuario.Proveedores.Add(proveedor);

            usuario.Habilitado = true;

            repositorio.Agregar(usuario);
            return repositorio.GuardarCambios() == 1;
        }

        public bool RegistrarUsuarioCliente(ref Usuario usuario)
        {
            var rolNuevoCliente = ObtenerRolPorCodigo("NUECLI");

            usuario.Roles = new List<Rol>
            {
                rolNuevoCliente
            };

            usuario.Proveedores = new List<Proveedor>();

            Proveedor proveedor = new Proveedor
            {
                CUIT = usuario.CUITRegistro,
                Mail = usuario.Mail,
                EstadoAprobacion = EstadoAprobacion.EtapaFinal,
                Observaciones = "Esperando aprobación.",
                TipoProveedor = ObtenerTipoPorNombreCorto("CLI")
            };

            proveedor.HistorialAprobaciones = new List<ProveedorHistorialAprobacion>
            {
                new ProveedorHistorialAprobacion()
                {
                    Fecha = DateTime.Now,
                    Proveedor_Id = proveedor.Id,
                    EstadoAprobacion = EstadoAprobacion.EtapaFinal,
                    Observacion = "Registro de usuario cliente",
                    Usuario_Id = usuario.Id
                }
            };

            usuario.Proveedores.Add(proveedor);
            usuario.Habilitado = true;

            repositorio.Agregar(usuario);
            return repositorio.GuardarCambios() == 1;
        }

        public bool ExisteUsuario(Usuario usuario)
        {
            return (repositorio.Existe<Usuario>(u => u.Mail == usuario.Mail));
        }

        public bool ValidarCUITProveedor(ref UsuarioGranos usuario, Proveedor proveedor)
        {
            return dataAgroService.ValidarCUITProveedorGranos(ref usuario, proveedor);
        }

        public SustitucionMOAWS.DataAgroServices.ResultadoValidarProveedorComercial ObtenerInfoProveedorDA(string CUIT, bool corredor = false) => dataAgroService.ObtenerValidarCUITProveedorGranos(CUIT, corredor);

        public Usuario ObtenerUsuario(string mail, string granosFlag) => BuscarUsuarioPorMail(mail);

        private string FormatearCodigoCorredor(string CUIT) => string.Concat("C", CUIT.Substring(2, 8));

        private TipoUsuario ObtenerTipoPorNombreCorto(string nombreCorto) => repositorio.Obtener<TipoUsuario>(t => t.NombreCorto == nombreCorto);

        private Rol ObtenerRolPorCodigo(string codigo) => repositorio.Obtener<Rol>(u => u.Codigo.Equals(codigo));
    }
}
