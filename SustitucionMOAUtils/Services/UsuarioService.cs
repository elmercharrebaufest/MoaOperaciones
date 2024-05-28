using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using Entidades = SustitucionMOAModel.Entities;
using Proveedor = SustitucionMOAModel.Entities.Proveedor;

namespace SustitucionMOAUtils.Services
{
    public class UsuarioService : IUsuarioService
    {
        protected readonly IRepositorioUsuario repositorio;
        protected readonly IVendedorService vendedorService;
        protected readonly IAzureADConsumer azureADConsumer;

        public UsuarioService(IRepositorioUsuario repositorio, IVendedorService vendedorService, IAzureADConsumer azureADConsumer)
        {
            this.repositorio = repositorio;
            this.vendedorService = vendedorService;
            this.azureADConsumer = azureADConsumer;
        }

        public void SeccionVisitada(string mailUsuario, string seccion)
        {
            Entidades.Usuario usuario = repositorio.Obtener<Entidades.Usuario>(u => u.Mail == mailUsuario);
            if (usuario.SeccionesVisitadas.Contains(seccion)) return;

            usuario.SeccionesVisitadas += $"-{seccion}";
            repositorio.GuardarCambios();
        }

        public List<UsuarioDto> GetUsuarios()
        {
            try
            {
                //List<Entidades.Usuario> usuarios = repositorio.Listar<Entidades.Usuario>();

                //List<UsuarioDto> usuariosDto = usuarios.Select(x => new UsuarioDto(x)).ToList();
                var usuariosDto = repositorio.ObtenerUsuarios();

                if (usuariosDto.Count == 0)
                {
                    throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Usuarios"));
                }
                return usuariosDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public UsuarioDto GetUsuario(string email)
        {
            try
            {
                Entidades.Usuario usuario = repositorio.Obtener<Entidades.Usuario>(x => x.Mail == email);

                if (usuario == null)
                {
                    throw new InfoCustomException(String.Format(InfoMsg.ElementoNoExiste, "Usuario", email));
                }
                var ret = new UsuarioDto(usuario);
                ret.Permisos = usuario.ObtenerPermisos();
                ret.NuevoUsuario = usuario.EsNuevoUsuario();

                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string HabilitarUsuario(string usuarioMail)
        {
            Entidades.Usuario usuario = repositorio.Obtener<Entidades.Usuario>(u => u.Mail == usuarioMail);

            var proveedor = usuario.ObtenerProveedor();

            proveedor.EstadoAprobacion = EstadoAprobacion.Aprobado;

            usuario.Habilitado = true;

            usuario.RemoverRol("DES");

            repositorio.GuardarCambios();

            return string.Format(SuccessMsg.UsuarioHabilitadoOK, usuario.Mail);
        }

        public string DeshabilitarUsuario(string usuarioMail)
        {
            Entidades.Usuario usuario = repositorio.Obtener<Entidades.Usuario>(u => u.Mail == usuarioMail);

            usuario.Habilitado = false;

            var proveedor = usuario.ObtenerProveedor();

            proveedor.EstadoAprobacion = EstadoAprobacion.Deshabilitado;

            proveedor.Observaciones = "Su usuario ha sido deshabilitado.";

            var rolUsuario = ObtenerRolPorCodigo("DES");

            usuario.AgregarRol(rolUsuario);

            repositorio.GuardarCambios();

            return string.Format(SuccessMsg.UsuarioDeshabilitadoOK, usuario.Mail);
        }

        public Rol ObtenerRolPorCodigo(string codigo)
        {
            return repositorio.Obtener<Rol>(u => u.Codigo == codigo);
        }

        public byte[] getDocumento(string nombre)
        {
            string sourcePath = @"\\vicinf01\Legajo_Impositivo\Documentacion_MoaOperaciones";
            try
            {
                string[] filepaths = Directory.GetFiles(sourcePath);
                foreach (string filepath in filepaths)
                {
                    string result = Path.GetFileName("@\\" + filepath);
                    if (nombre == result)
                    {
                        byte[] file = System.IO.File.ReadAllBytes(filepath);
                        return file;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return null;
        }

        public List<RolDropdownDto> GetRoles()
        {
            List<string> interno = new List<string>
            {
                "ADM", "OPE", "APRO", "COMPRAS", "COMPRASADMIN", "ADMINCCSS", "TODOS", "COMERCIAL", "SOLP",
                "APIKEY", "AIGRAN","AINOGRAN", "ADMINPLATCOMPRAS","ANUL", "ECHEQ ADMIN", "FASON ADMIN","APLCCPP ADMIN", "COMPRADOR",
                "FLETE MOA","ADMIN_CURSOS","CERTIFICACION"
            };

            List<string> contacto = new List<string>
            {
                "BOL", "DATMAE", "REI", "ACT", "PAR", "FIN", "CAL", "COM",
                "COMP", "APP", "PES", "PAG", "FWEB", "MATBA",
                "PROVGC", "FLECONSULTA", "OTRO", "PARDIR", "PARCOR",
                "FINDIR", "FINCOR", "FLE", "CRDECPE", "ORD", "DISCAL"
            };

            var roles = repositorio.Listar<Rol>().Where(r => r.EsEditable)
                .Select(x => new RolDropdownDto
                {
                    Id = x.Id,
                    Nombre = x.Nombre,
                    Code = interno.Contains(x.Codigo) ? "Interno" : contacto.Contains(x.Codigo) ? "Contacto" : "Externo"
                }).ToList();

            return roles;
        }

        public string GuardarRoles(List<int> idRoles, int idUsuario, string usuarioSap)
        {
            Entidades.Usuario usuario = repositorio.Obtener<Entidades.Usuario>(u => u.Id == idUsuario);

            usuario.RemoverRolesEditables();

            usuario.UsuarioSap = usuarioSap.ToUpper();

            foreach (int idRol in idRoles)
            {
                Rol rolAAgregar = repositorio.Obtener<Rol>(r => r.Id == idRol);
                usuario.AgregarRol(rolAAgregar);
            }

            List<string> contacto = new List<string>
            {
                "BOL", "DATMAE", "REI", "ACT", "PAR", "FIN", "CAL", "COM",
                "COMP", "APP", "PES", "PAG", "FWEB", "MATBA",
                "PROVGC", "FLECONSULTA", "OTRO", "PARDIR", "PARCOR",
                "FINDIR", "FINCOR", "FLE", "CRDECPE", "ORD"
            };

            var esAdministradorMolinos = usuario
                                            .Roles
                                            .Where(
                                                r => r.Codigo == "ADM"
                                                || r.Codigo == "TODOS"
                                                || r.Codigo == "OPE"
                                                || r.Codigo == "APRO"
                                                || r.Codigo == "COMPRAS"
                                                || r.Codigo == "COMERCIAL"
                                                || contacto.Contains(r.Codigo))
                                            .Any();

            //Si es usuario de molinos, buscamos todos los proveedores que tiene, lo aprobamos y le sacamos el historial de aprobación. 
            //Esto es para que estos proveedores no aparezcan en el listado de altas pendientes

            if (esAdministradorMolinos)
            {
                //Hacemos el cambio para que no le vuele el historial a todos los proveedores
                var prov = usuario.Proveedores.FirstOrDefault(p => p.CUIT == usuario.CUITRegistro && p.Mail == usuario.Mail);

                if (prov != null)
                {
                    prov.EstadoAprobacion = EstadoAprobacion.Aprobado;

                    prov.Observaciones = "";

                    var historial = repositorio.Listar<ProveedorHistorialAprobacion>(h => h.Proveedor_Id == prov.Id);

                    repositorio.RemoverTodos(historial);

                    prov.HistorialAprobaciones = new List<ProveedorHistorialAprobacion>();
                }

            }

            repositorio.GuardarCambios();
            var proveedor = usuario.ObtenerProveedor();
            if (proveedor != null)
            {
                return string.Format(SuccessMsg.RolesActualizadosOk, usuario.Mail, " ( CUIT: " + proveedor.CUIT + ")");
            }
            else
            {
                return string.Format(SuccessMsg.RolesActualizadosOk, usuario.Mail, "");
            }

        }

        public List<RolDropdownDto> GetRolesUsuario(int idUsuario)
        {
            try
            {
                Entidades.Usuario usuario = repositorio.Obtener<Entidades.Usuario>(u => u.Id == idUsuario);

                var rolesDto = usuario.Roles.Select(x => new RolDropdownDto(x)).ToList();

                return rolesDto;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Rol> GetRolesUsuario(string email)
        {
            try
            {
                Entidades.Usuario usuario = repositorio.Obtener<Entidades.Usuario>(u => u.Mail == email);
                return usuario.Roles.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Rol> GetRolesApiKey(string apikey)
        {
            try
            {
                Entidades.Usuario usuario = repositorio.Obtener<Entidades.Usuario>(u => u.ApiKey == apikey);
                if (usuario == null)
                    return new List<Rol>();

                return usuario.Roles.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ProveedorDto> GetVendedoresUsuario(string usuarioMail)
        {
            return vendedorService.GetVendedores(usuarioMail);
        }

        public ProveedorDto GetProveedorPorCodigo(string codigo, string mailUsuario)
        {
            Proveedor proveedor = repositorio.Obtener<Proveedor>(x => x.CodigoProveedor == codigo && x.Mail == mailUsuario);

            if (proveedor == null)
            {
                proveedor = repositorio.Obtener<Proveedor>(x => x.CodigoProveedor == codigo);

                if (proveedor == null)
                {
                    throw new InfoCustomException(String.Format(InfoMsg.ElementoNoExiste, "Proveedor", codigo));
                }
            }
            var ret = new ProveedorDto(proveedor);

            return ret;
        }


        /// <summary>
        /// Es método cumple el mismo funcionamiento que GetProveedorPorCodigo. 
        /// La diferencia es que cuando se recibe un proveedor que no está en nuestra DB, lo crea. 
        /// Esto lo hacemos para los corredores, de los cuales no tenemos todos los proveedores cargados
        /// </summary>
        /// <param name="mailUsuario"></param>
        /// <param name="codigoCorredor"></param>
        /// <param name="codigoProveedor"></param>
        /// <returns></returns>
        public ProveedorDto VerificarYObtenerProveedor(string mailUsuario, string codigoCorredor, string codigoProveedor)
        {
            var usuario = repositorio.Obtener<Entidades.Usuario>(u => u.Mail == mailUsuario);
            Proveedor proveedor;
            Proveedor corredor;
            bool insertarProveedor = false;

            //Si tiene permisos para usar todos los proveedores, no filtramos por tipo de usuario
            if (usuario.TienePermiso(PermisoEnum.ElegirTodosVendedores))
            {
                proveedor = repositorio.Obtener<Proveedor>(x => x.CodigoProveedor == codigoProveedor && x.EstadoAprobacion == EstadoAprobacion.Aprobado);
                corredor = repositorio.Obtener<Proveedor>(x => x.CodigoProveedor == codigoCorredor && x.EstadoAprobacion == EstadoAprobacion.Aprobado);
            }
            else
            {
                proveedor = repositorio.Obtener<Proveedor>(x => x.CodigoProveedor == codigoProveedor && x.Mail == mailUsuario);
                corredor = repositorio.Obtener<Proveedor>(x => x.CodigoProveedor == codigoCorredor && x.Mail == mailUsuario);
                insertarProveedor = true;
            }

            if (proveedor == null)
            {
                var proveedorSAP = new VendedorDetalleConsumerMOA().request(codigoProveedor, codigoCorredor);

                if (!proveedorSAP.cabeceras.Any())
                {
                    throw new InfoCustomException(string.Format(InfoMsg.ElementoNoExiste, "Proveedor", codigoProveedor));
                }

                var tipoProveedorGranos = repositorio.Obtener<TipoUsuario>(t => t.NombreCorto == "G");

                Proveedor nuevoProveedorMOA = new Proveedor
                {
                    IdProveedorCorredor = corredor.Id,
                    Mail = usuario.Mail,
                    Observaciones = "Proveedor agregado automaticamente por MOA Operaciones",
                    CodigoProveedor = codigoProveedor,
                    CUIT = proveedorSAP.cabeceras.FirstOrDefault().cuit,
                    RazonSocial = proveedorSAP.cabeceras.FirstOrDefault().descripcion,
                    EstadoAprobacion = EstadoAprobacion.Aprobado,
                    TipoProveedor = tipoProveedorGranos
                };

                if (insertarProveedor)
                {
                    usuario.Proveedores.Add(nuevoProveedorMOA);
                    repositorio.GuardarCambios();
                }

                proveedor = nuevoProveedorMOA;
            }
            var ret = new ProveedorDto(proveedor);

            return ret;
        }

        public string ObtenerNuevoApiKey(string usuario)
        {
            Entidades.Usuario usuarioEntity = repositorio.Obtener<Entidades.Usuario>(u => u.Mail == usuario);
            var apikey = SustitucionMOACrypting.CryptoServiceProvider.GetNewApiKey();

            usuarioEntity.ApiKey = apikey;

            repositorio.GuardarCambios();
            return apikey;
        }

        public List<ProveedorDto> ListarProveedores(string filtro)
        {
            var proveedores = repositorio.Listar<Entidades.Usuario, ProveedorDto>(x => new ProveedorDto()
            {
                Id = x.Id,
                Mail = x.Mail,
                RazonSocial = x.Proveedores.Where(y => x.TipoUsuario.Id == y.TipoProveedor.Id &&
                              x.CUITRegistro == y.CUIT && x.Mail == y.Mail && y.TipoProveedor.NombreCorto == "NG").FirstOrDefault().RazonSocial,
                CUIT = x.Proveedores.Where(y => x.TipoUsuario.Id == y.TipoProveedor.Id &&
                              x.CUITRegistro == y.CUIT && x.Mail == y.Mail && y.TipoProveedor.NombreCorto == "NG").FirstOrDefault().CUIT,
                CodigoProveedor = x.Proveedores.Where(y => x.TipoUsuario.Id == y.TipoProveedor.Id &&
                              x.CUITRegistro == y.CUIT && x.Mail == y.Mail && y.TipoProveedor.NombreCorto == "NG").FirstOrDefault().CodigoProveedor,
            }, x => x.Proveedores.Any(y => x.TipoUsuario.Id == y.TipoProveedor.Id && y.CodigoProveedor != null && y.CodigoProveedor != "" &&
            x.CUITRegistro == y.CUIT && x.Mail == y.Mail && y.TipoProveedor.NombreCorto == "NG") && x.Habilitado &&
            (x.Proveedores.Where(y => x.TipoUsuario.Id == y.TipoProveedor.Id && x.CUITRegistro == y.CUIT && x.Mail == y.Mail && y.CodigoProveedor != null && y.CodigoProveedor != ""
            && y.TipoProveedor.NombreCorto == "NG").FirstOrDefault().RazonSocial.Contains(filtro) || x.Mail.Contains(filtro)
            || x.CUITRegistro.Contains(filtro))).Take(10);
            return proveedores.ToList();
        }

        public ResultadoGenerico GrabarProveedor(ProveedorDto proveedorDto, EstadoAprobacion estadoAprobacion = EstadoAprobacion.DocumentacionPendiente)
        {
            UsuarioNoGranos usuarioNoGranos = new UsuarioNoGranos { Mail = proveedorDto.Mail, CUITRegistro = proveedorDto.CUIT, SeccionesVisitadas = "" };

            TipoUsuario tipoUsuario = repositorio.Obtener<TipoUsuario>(t => t.NombreCorto == "NG");

            Entidades.Usuario usuario = new Entidades.Usuario { Mail = proveedorDto.Mail, CUITRegistro = proveedorDto.CUIT, SeccionesVisitadas = "", TipoUsuario = tipoUsuario };

            usuarioNoGranos.TipoUsuario = tipoUsuario;

            var resultado = new ResultadoGenerico();

            ValidarDatosProveedor(proveedorDto, resultado);
            if (!resultado.HayError)
            {

                var setCodigoProveedor = "00" + proveedorDto.CUIT.Remove(proveedorDto.CUIT.Length - 1).Remove(0, 2);

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
                    EstadoAprobacion = estadoAprobacion,
                    Observaciones = "Proveedor agregado por compras",
                    Mail = usuario.Mail,
                    TipoProveedor = tipoUsuario,
                    FechaSolicitud = DateTime.Now,
                    RazonSocial = proveedorDto.RazonSocial,
                    CodigoProveedor = setCodigoProveedor
                };

                proveedor.HistorialAprobaciones = new List<ProveedorHistorialAprobacion>
            {
                new ProveedorHistorialAprobacion()
                {
                    Fecha = DateTime.Now,
                    EstadoAprobacion = estadoAprobacion,
                    Observacion = "Registro de usuario",
                    Usuario_Id = usuario.Id
                }
            };

                if (repositorio.Existe<Proveedor>(x => x.CUIT == cuit && x.Mail == mailUsuario))
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
                //return repositorio.GuardarCambios() == 1;

                proveedor = repositorio.Agregar(proveedor);

                repositorio.GuardarCambios();

                resultado.Descripcion = $"{proveedor.RazonSocial} ({proveedor.CUIT}) - {proveedor.Mail}";

                var proveedorResultado = new ProveedorDto() { Mail = proveedorDto.Mail, CUIT = proveedorDto.CUIT, Id = usuario.Id, RazonSocial = proveedorDto.RazonSocial };

                resultado.ProveedorDto = proveedorResultado;
            }
            return resultado;
        }

        public ResultadoGenerico ValidarDatosProveedor(ProveedorDto proveedorDto, ResultadoGenerico resultado)
        {
            if (String.IsNullOrEmpty(proveedorDto.Mail))
            {
                resultado.Errores.Add(new ErrorMessage(1, "El mail es obligatorio"));
                return resultado;
            }

            if (String.IsNullOrEmpty(proveedorDto.CUIT))
            {
                resultado.Errores.Add(new ErrorMessage(1, "El CUIT es obligatorio"));
                return resultado;
            }

            if (String.IsNullOrEmpty(proveedorDto.RazonSocial))
            {
                resultado.Errores.Add(new ErrorMessage(1, "La razon social es obligatoria"));
                return resultado;
            }

            var existeMail = repositorio.Existe<Entidades.Usuario>(x => x.Mail == proveedorDto.Mail);

            if (existeMail)
            {
                resultado.Errores.Add(new ErrorMessage(1, "El mail ya se encuentra registrado"));
                return resultado;
            }

            return resultado;
        }

        public IEnumerable<IGrouping<int, UsuarioDto>> ListarUsuarioCreadorSolp()
        {
            return repositorio.Listar<Solp, UsuarioDto>(solp => new UsuarioDto
            {
                Mail = solp.UsuarioCreacion.Mail,
                Id = solp.UsuarioCreacion.Id
            }).GroupBy(x => x.Id);
        }

        public List<DestinatarioDto> ObtenerDestinatariosConsulta(int proveedorId)
        {
            var proveedor = this.repositorio.Obtener<Proveedor>(p => p.Id == proveedorId);
            var proveedoresMismoCodigo = repositorio.Listar<Proveedor>(p => p.CodigoProveedor == proveedor.CodigoProveedor);

            var destinatarios = proveedoresMismoCodigo.SelectMany(p=>p.UsuariosAsociados
                .Where(u=>!u.Mail.EndsWith("@molinosagro.com.ar"))
                .Select(u => new DestinatarioDto(u))).ToList();
            return destinatarios;
        }


        #region Metodos de modificacion de alta usuario
        public UsuarioDto GetUsuarioPorId(int id)
        {
            try
            {
                Entidades.Usuario usuario = repositorio.Obtener<Entidades.Usuario>(x => x.Id == id);

                if (usuario == null)
                {
                    throw new InfoCustomException(String.Format(InfoMsg.ElementoNoExiste, "Usuario", id));
                }
                var ret = new UsuarioDto(usuario);
                ret.Permisos = usuario.ObtenerPermisos();
                ret.NuevoUsuario = usuario.EsNuevoUsuario();

                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<ProveedorDto> GetProvedoresEmail(int tipoProveedorId, string email, string cuitUsuario)
        {
            var proveedores = repositorio.Listar<Proveedor>(x => x.Mail == email && x.TipoProveedor.Id == tipoProveedorId && x.CUIT == cuitUsuario);
            List<ProveedorDto> listaProvedores = new List<ProveedorDto>();
            foreach (var proveedor in proveedores)
            {
                listaProvedores.Add(new ProveedorDto(proveedor));
            }
            return listaProvedores;
        }

        public List<ProveedorDto> GetProveedoresUsuario(int usuarioId)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Id == usuarioId);
            List<ProveedorDto> listaProvedores = new List<ProveedorDto>();
            foreach (var proveedor in usuario.Proveedores)
            {
                listaProvedores.Add(new ProveedorDto(proveedor));
            }
            return listaProvedores;
        }

        public List<TipoUsuarioDto> GetTipoUsuario()
        {
            var tipoUsuarios = repositorio.Listar<TipoUsuario>();
            List<TipoUsuarioDto> listaTipoUsuarios = new List<TipoUsuarioDto>();
            foreach (var tipoUsuario in tipoUsuarios)
            {
                listaTipoUsuarios.Add(new TipoUsuarioDto(tipoUsuario));
            }
            return listaTipoUsuarios;
        }
        public List<string> ValidarMailUsuario(UsuarioModificacionDto usuarioModificacionDto)
        {
            List<string> listaValidacion = new List<string>();
            var usuarios = repositorio.Listar<Entidades.Usuario>(x => x.Mail == usuarioModificacionDto.Mail && x.Id != usuarioModificacionDto.Id);
            if (usuarios.Count > 0) listaValidacion.Add("El correo ya existe para otro usuario");
            return listaValidacion;
        }
        public List<ProveedorAuditoriaDto> GetProveedorAuditoriaPorUsuario(int usuarioId)
        {
            List<ProveedorAuditoriaDto> listaAuditoria = new List<ProveedorAuditoriaDto>();
            var listaProveedorAuditoria = repositorio.Listar<Entidades.ProveedorAuditoria>(x => x.Usuario_Id == usuarioId);
            foreach (var proveedor in listaProveedorAuditoria)
            {
                var tipoProveedor = repositorio.Obtener<Entidades.TipoUsuario>(x => x.Id == proveedor.TipoProveedor_Id);
                listaAuditoria.Add(new ProveedorAuditoriaDto()
                {
                    Id = proveedor.Id,
                    Usuario_Id = proveedor.Usuario_Id,
                    Proveedor_Id = proveedor.Proveedor_Id,
                    CodigoProveedor = proveedor.CodigoProveedor,
                    Mail = proveedor.Mail,
                    Cuit = proveedor.Cuit,
                    TipoProveedor_Id = proveedor.TipoProveedor_Id,
                    TipoProveedor = tipoProveedor.Nombre,
                    RazonSocial = proveedor.RazonSocial,
                    FechaActualizacion = proveedor.FechaActualizacion.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture),
                    UsuarioActualizacion = proveedor.UsuarioActualizacion,
                });
            }
            return listaAuditoria;
        }
        private void GuardarProveedorAuditoria(Proveedor proveedorActual, ProveedoresModificacionDto proveedorModificado, UsuarioModificacionDto usuarioModificacionDto)
        {
            if (!proveedorActual.Mail.Equals(usuarioModificacionDto.Mail) ||
                !proveedorActual.CUIT.Equals(proveedorModificado.Cuit) ||
                 proveedorActual.TipoProveedor.Id != proveedorModificado.IdTipoProveedor ||
                !proveedorActual.RazonSocial.Equals(proveedorModificado.RazonSocial) || !proveedorActual.CodigoProveedor.Equals(proveedorModificado.CodigoProveedor) ||
                 proveedorActual.CodigoProveedor != proveedorModificado.CodigoProveedor ||
                 proveedorActual.EsRevendedor != proveedorModificado.EsRevendedor || proveedorActual.OrganizacionDeCompra != usuarioModificacionDto.OrganizacionDeCompra)
            {
                ProveedorAuditoria proveedorAuditoria = new ProveedorAuditoria();
                proveedorAuditoria.Proveedor_Id = proveedorActual.Id;
                proveedorAuditoria.Usuario_Id = usuarioModificacionDto.Id;
                proveedorAuditoria.CodigoProveedor = proveedorModificado.CodigoProveedor;
                proveedorAuditoria.Mail = usuarioModificacionDto.Mail;
                proveedorAuditoria.Cuit = proveedorModificado.Cuit;
                proveedorAuditoria.TipoProveedor_Id = proveedorModificado.IdTipoProveedor;
                proveedorAuditoria.RazonSocial = proveedorModificado.RazonSocial;
                proveedorAuditoria.FechaActualizacion = DateTime.Now;
                proveedorAuditoria.UsuarioActualizacion = usuarioModificacionDto.UsuarioModificacion;
                proveedorAuditoria.EsRevendedor = proveedorModificado.EsRevendedor;
                proveedorAuditoria.OrganizacionDeCompra = usuarioModificacionDto.OrganizacionDeCompra;
                repositorio.Agregar(proveedorAuditoria);
            }
        }
        public string ModificarUsuario(UsuarioModificacionDto usuarioModificacionDto)
        {
            string resultado = string.Empty;
            try
            {
                Usuario usuario = repositorio.Obtener<Usuario>(x => x.Id == usuarioModificacionDto.Id);
                usuario.CUITRegistro = usuarioModificacionDto.Cuit;
                usuario.Mail = usuarioModificacionDto.Mail;
                usuario.TipoUsuario = repositorio.Obtener<TipoUsuario>(x => x.Id == usuarioModificacionDto.IdTipoUsuario);
                usuario.OrganizacionDeCompra = usuarioModificacionDto.OrganizacionDeCompra;
                repositorio.GuardarCambios();
                if (usuarioModificacionDto.Proveedores != null)
                {
                    foreach (var proveedor in usuario.Proveedores)
                    {
                        var proveedorModificado = usuarioModificacionDto.Proveedores.Where(x => x.Id == proveedor.Id).FirstOrDefault();
                        if (proveedorModificado != null)
                        {
                            GuardarProveedorAuditoria(proveedor, proveedorModificado, usuarioModificacionDto);
                            proveedor.Mail = usuarioModificacionDto.Mail;
                            proveedor.CUIT = proveedorModificado.Cuit;
                            proveedor.RazonSocial = proveedorModificado.RazonSocial;
                            proveedor.CodigoProveedor = proveedorModificado.CodigoProveedor;
                            proveedor.TipoProveedor = repositorio.Obtener<TipoUsuario>(x => x.Id == proveedorModificado.IdTipoProveedor);
                            proveedor.EsRevendedor = proveedorModificado.EsRevendedor;
                            repositorio.GuardarCambios();
                        }
                    }
                }
                resultado = "OK";
            }
            catch (Exception ex)
            {
                resultado = ex.Message;
            }
            return resultado;
        }

        public ProveedorDto TraerProveedorEnSAP(string codigoProveedor, string codigoCorredor)
        {
            //tengo que preguntar si es corredor o proveedor?
            var proveedorSAP = new VendedorDetalleConsumerMOA().request(codigoProveedor, codigoCorredor);
            var tipoProveedorGranos = repositorio.Obtener<TipoUsuario>(t => t.NombreCorto == "G");

            ProveedorDto proveedor = new ProveedorDto
            {
                //IdProveedorCorredor = corredor.Id,
                Mail = "",
                Observaciones = "Proveedor agregado automaticamente por MOA Operaciones",
                CodigoProveedor = codigoProveedor,
                CUIT = proveedorSAP.cabeceras.FirstOrDefault().cuit,
                RazonSocial = proveedorSAP.cabeceras.FirstOrDefault().descripcion,
                EstadoAprobacion = EstadoAprobacion.Aprobado,
                IdTipoUsuario = tipoProveedorGranos.Id
            };

            return proveedor;
        }

        #endregion

        #region EliminarCuitNoHabilitado
        public string EliminarCuitNoHabilitado(int proveedorId, string mailUsuarioSesion)
        {
            var usuarioSesion = repositorio.Obtener<Entidades.Usuario>(u => u.Mail == mailUsuarioSesion);
            if (!usuarioSesion.EsAdmin())
                throw new InfoCustomException("Ud no posee los permisos suficientes para realizar para borrar un usuario.");

            var proveedorABorrar = repositorio.Obtener<Entidades.Proveedor>(p => p.Id == proveedorId);
            if (!proveedorABorrar.EstadoAprobacion.Equals(EstadoAprobacion.AltaIncompleta))
                throw new InfoCustomException("El usuario no se encuentra en estado CUIT NO HABILITADO.");

            var usuarioABorrar = obtenerUsuarioDelVendedor(proveedorABorrar);
            if (usuarioABorrar == null)
                throw new InfoCustomException("No se encontró el usuario a eliminar.");
            try
            {
                bool puedeEliminarseProveedor = PuedeEliminarseProveedor(proveedorABorrar);
                bool puedeEliminarseUsuario = PuedeEliminarseUsuario(usuarioABorrar);
                bool eliminarUsuarioAzure = false;
                var historialProveedor = repositorio.Listar<Entidades.ProveedorHistorialAprobacion>
                    (h => h.Proveedor_Id == proveedorABorrar.Id);

                Log.Info($"Inicio UsuarioService.EliminarCuitNoHabilitado params => proveedorId: {proveedorId}");
                /*Caso en el que se deba borrar al proveedor y su usuario que no opero con ningun otro vendedor 
                Ni realizo acciones en el sistema.*/
                if (puedeEliminarseProveedor && puedeEliminarseUsuario)
                {
                    Log.Info("EliminarCuitNoHabilitado - Eliminando proveedor id: " + proveedorABorrar.Id + ", usuario id: " + usuarioABorrar.Id);
                    repositorio.RemoverTodos<Entidades.ProveedorHistorialAprobacion>(historialProveedor);
                    repositorio.Remover<Entidades.Proveedor>(proveedorABorrar);

                    var tipoDelUsuario = repositorio.Obtener<Entidades.UsuarioNoGranos>(ng => ng.Id == usuarioABorrar.Id);
                    if (tipoDelUsuario != null)
                        repositorio.Remover<Entidades.UsuarioNoGranos>(tipoDelUsuario);

                    repositorio.Remover<Entidades.Usuario>(usuarioABorrar);
                    eliminarUsuarioAzure = true;
                }
                /*Caso en el que solo eliminamos el alta ya que el usuario puede operar con otro vendedores.*/
                else if (puedeEliminarseProveedor && usuarioABorrar.Proveedores.Count() > 1)
                {
                    Log.Info("Eliminando solo alta proveedor id: " + proveedorABorrar.Id);
                    repositorio.RemoverTodos<Entidades.ProveedorHistorialAprobacion>(historialProveedor);
                    repositorio.Remover<Entidades.Proveedor>(proveedorABorrar);
                }
                else
                {
                    if (usuarioABorrar.Proveedores.Count() == 1)
                        throw new InfoCustomException($"No se puede eliminar al proveedor {usuarioABorrar.Mail} debido" +
                            $" a que su usuario realizo operaciones en el sistema y solo opera con este proveedor.");
                    else
                        throw new InfoCustomException($"No se puede eliminar al usuario {usuarioABorrar.Mail} debido a que el mismo" +
                            $" posee operaciones en el sistema.");
                }
                if (eliminarUsuarioAzure)
                {
                    this.azureADConsumer.BorrarUsuarioSegunMail(usuarioABorrar.Mail);
                }
                repositorio.GuardarCambios();
                Log.Info("Finaliza OK UsuarioService.EliminarCuitNoHabilitado.");
                return "Se ha eliminado correctamente al usuario: " + usuarioABorrar.Mail;
            }
            catch (InfoCustomException ice)
            {
                Log.Info(ice.Message);
                throw ice;
            }
            catch (Exception e)
            {
                Log.Info("Hubo un error al intentar eliminar al proveedor con id: " + proveedorId);
                Log.Error(e);
                throw new ValidationCustomException(ErrorMsg.Error, true);
            }
        }

        public bool PuedeEliminarseProveedor(Entidades.Proveedor proveedor)
        {
            var solpProveedor = repositorio.Listar<Entidades.SolpProveedor>(c => c.Proveedor_Id == proveedor.Id).FirstOrDefault();
            var campoProveedor = repositorio.Listar<Entidades.CampoProveedor>(c => c.Proveedor_Id == proveedor.Id).FirstOrDefault();
            var decCampoSustentable = repositorio.Listar<Entidades.DeclaracionCampoSustentable>(d => d.Proveedor_Id == proveedor.Id).FirstOrDefault();
            var appCartaPorte = repositorio.Listar<Entidades.AplicacionCartaPorte>(a => a.Proveedor_Id == proveedor.Id).FirstOrDefault();
            var provAuditoria = repositorio.Listar<Entidades.ProveedorAuditoria>(p => p.Proveedor_Id == proveedor.Id).FirstOrDefault();
            var relEmpleados = repositorio.Listar<Entidades.ProveedorRelacionConEmpleados>(r => r.Proveedor_Id == proveedor.Id).FirstOrDefault();
            var relFunc = repositorio.Listar<Entidades.ProveedorRelacionConFuncionarios>(r => r.Proveedor_Id == proveedor.Id).FirstOrDefault();

            return solpProveedor == null && campoProveedor == null && decCampoSustentable == null &&
                appCartaPorte == null && provAuditoria == null && relEmpleados == null && relFunc == null
                && proveedor.UsuariosAsociados.Count() <= 1;
        }
        public bool PuedeEliminarseUsuario(Entidades.Usuario usuario)
        {
            var tieneActividad = repositorio.VerificarActividadUsuario(usuario);

            return  !tieneActividad && usuario.Proveedores.Count() <= 1;
        }

        public Entidades.Usuario obtenerUsuarioDelVendedor(Entidades.Proveedor prov)
        {
            return repositorio.Obtener<Entidades.Usuario>(u => u.Mail == prov.Mail && u.TipoUsuario.Id == prov.TipoProveedor.Id
            && u.CUITRegistro == prov.CUIT);
        }

        #endregion

        #region AsignarNuevoCUIT
        public ProveedorDto GetProveedorAprobadoPorCuit(string cuit, string mailUsuarioSesion)
        {

            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuarioSesion);

            if (usuario == null || !usuario.TieneRol(RolEnum.Administracion))
            {
                throw new InfoCustomException("Usuario no autorizado a realizar esta acción.");
            }
            var proveedor = repositorio.Obtener<Proveedor>(p => p.CUIT == cuit && p.EstadoAprobacion == EstadoAprobacion.Aprobado);

            if (proveedor == null)
            {
                return null;
            }

            return new ProveedorDto
            {
                RazonSocial = proveedor.RazonSocial,
                CodigoProveedor = proveedor.CodigoProveedor,
                IdTipoProveedor = proveedor.TipoProveedor.Id
            };
        }
        public void AsignarNuevaCUIT(AsignarNuevaCuitDto datosAsignar, string mailUsuarioSesion)
        {
            var usuarioSesion = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuarioSesion);

            if (usuarioSesion == null || !usuarioSesion.TieneRol(RolEnum.Administracion))
            {
                throw new InfoCustomException("Usuario no autorizado a realizar esta acción.");
            }

            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == datosAsignar.MailUsuario && u.Id == datosAsignar.IdUsuario);

            if (usuario == null)
            {
                throw new InfoCustomException("No se ha encontrado un usuario para asignar la cuit.");
            }

            if(usuario.Proveedores.Any(p=>p.CUIT == datosAsignar.CuitAAsignar))
            {
                throw new InfoCustomException("El usuario ya tiene asignada la cuit solicitada.");
            }

            var proveedorAAsignar = new Proveedor
            {
                CUIT = datosAsignar.CuitAAsignar,
                RazonSocial = datosAsignar.RazonSocialAAsignar,
                CodigoProveedor = datosAsignar.CodigoProveedorAAsignar,
                TipoProveedor = repositorio.Obtener<TipoUsuario>(datosAsignar.TipoProveedorIdAAsignar),
                EstadoAprobacion = EstadoAprobacion.Aprobado,

                SolicitanteInterno = usuario.Mail,
            };

            var historialProveedor = new ProveedorHistorialAprobacion
            {
                EstadoAprobacion = proveedorAAsignar.EstadoAprobacion,
                Usuario = usuarioSesion,
                Observacion = "Proveedor asignado manualmente de forma directa a traves de asignaciones de CUIT",
                Fecha = DateTime.Now,
            };

            proveedorAAsignar.HistorialAprobaciones = new List<ProveedorHistorialAprobacion> { historialProveedor };

            if (!usuario.TieneRol(RolEnum.Multifirma))
            {
                var rolMultifirma = repositorio.Obtener<Rol>(r => r.Codigo == "MF");
                usuario.AgregarRol(rolMultifirma);
            }
            usuario.Proveedores.Add(proveedorAAsignar);

            repositorio.GuardarCambios();
        }

        public void DesasociarVendedor(int usuarioId, int proveedorId, string mailUsuarioSesion)
        {
            var usuarioSesion = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuarioSesion);

            if (usuarioSesion == null || !usuarioSesion.TieneRol(RolEnum.Administracion))
            {
                throw new InfoCustomException("Usuario no autorizado a realizar esta acción.");
            }

            var usuario = this.repositorio.Obtener<Usuario>(u => u.Id == usuarioId);
            var proveedor = this.repositorio.Obtener<Proveedor>(p => p.Id == proveedorId);

            if (usuario.Proveedores.Count() == 1)
            {
                throw new InfoCustomException("No se puede realizar la desasociacion. El usuario opera unicamente con este vendedor.");
            }

            /*if (!usuario.TieneRol(RolEnum.Multifirma))
            {
                throw new InfoCustomException("No se puede realizar la desasociacion. El usuario no posee rol multifirma. Contactar a sistemas.");
            }*/

            usuario.Proveedores.Remove(proveedor);

            var historialProveedor = new ProveedorHistorialAprobacion
            {
                EstadoAprobacion = proveedor.EstadoAprobacion,
                Usuario = usuarioSesion,
                Observacion = "Se elimina asociación de proveedor para el usuario: " + usuario.Mail,
                Fecha = DateTime.Now,
            };

            proveedor.HistorialAprobaciones.Add(historialProveedor);

            this.repositorio.GuardarCambios();

            return;
        }
        #endregion

        public List<string> GetMailUsuarios(string mail)
        {
            var likeString = $"%{mail}%";
            return repositorio.Listar<Usuario, string>(
                u => u.Mail
                , u => DbFunctions.Like(u.Mail, likeString));
        }
    }
}
