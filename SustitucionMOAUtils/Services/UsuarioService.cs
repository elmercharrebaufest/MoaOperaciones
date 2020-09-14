using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.WSMapMOA.Login;
using SustitucionMOAModel.Models.WSMapMOA.Usuario;
using SustitucionMOAModel.Models.WSMapMOA.Usuario.Perfil;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Entidades = SustitucionMOAModel.Entities;

namespace SustitucionMOAUtils.Services
{
    public class UsuarioService : IUsuarioService
    {
        protected readonly IRepositorio repositorio;

        public UsuarioService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public GetPerfilesResponseMOA getPerfiles()
        {
            try
            {
                PerfilesWSMOAResponse response = new PerfilesConsumerMOA().request();

                if (response.perfiles.Count == 0)
                {
                    throw new ValidationCustomException(ErrorMsg.ErrorNoHayPerfiles);
                }

                if (response.tipos.Count == 0)
                {
                    throw new ValidationCustomException(ErrorMsg.ErrorNoHayTipos);
                }

                GetPerfilesResponseMOA data = new GetPerfilesResponseMOA();
                foreach (Perfil perfil in response.perfiles)
                {
                    data.perfiles.Add(new DropdownPerfilElement()
                    {
                        value = perfil.perfil,
                        label = perfil.nombre
                    });
                }
                foreach (TipoProveedor tipo in response.tipos)
                {
                    data.tipos.Add(new DropdownTipoElement()
                    {
                        value = tipo.tipo,
                        label = tipo.descripcion
                    });
                }

                return data;
            }
            catch (InfoCustomException e)
            {
                throw e;
            }
            catch (ValidationCustomException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }


        //public UsuariosWSMOAResponse getUsuarios()
        //{
        //    try
        //    {
        //        UsuariosWSMOAResponse response = new UsuariosConsumerMOA().request();

        //        if (response.usuarios.Count == 0)
        //        {
        //            throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Usuarios"));
        //        }
        //        return response;
        //    }
        //    catch (InfoCustomException e)
        //    {
        //        throw e;
        //    }
        //    catch (ValidationCustomException e)
        //    {
        //        throw e;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new WSCustomException(ErrorMsg.ErrorWS, e);
        //    }
        //}


        public List<UsuarioDto> GetUsuarios()
        {
            try
            {
                List<Entidades.Usuario> usuarios = repositorio.Listar<Entidades.Usuario>();

                List<UsuarioDto> usuariosDto = usuarios.Select(x => new UsuarioDto(x)).ToList();

                if (usuariosDto.Count == 0)
                {
                    throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Usuarios"));
                }
                return usuariosDto;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Obsolete]
        public string cambiarContrasenia(string username, string contraseniaActual, string contraseniaNueva)
        {
            try
            {
                if (contraseniaActual == null || contraseniaActual == "")
                {
                    throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "Contraseña Actual"));
                }

                if (contraseniaNueva == null || contraseniaNueva == "")
                {
                    throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "Contraseña Nueva"));
                }

                LoginWSMOAResponse response = new CambioPassConsumerMOA().request(username, contraseniaActual, contraseniaNueva);

                if (response.error != "09")
                {
                    throw new ValidationCustomException(response.texto);
                }

                return SuccessMsg.CambioPassOK;
            }
            catch (InfoCustomException e)
            {
                throw e;
            }
            catch (ValidationCustomException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        [Obsolete]
        public LoginWSMOAResponse registrar(string numeroProveedor, string claveActivacion, string username, string contrasenia)
        {
            try
            {
                if (numeroProveedor == null || numeroProveedor == "")
                {
                    throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "Número de Proveedor"));
                }

                if (claveActivacion == null || claveActivacion == "")
                {
                    throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "Clave Activación"));
                }

                if (username == null || username == "")
                {
                    throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "Nombre de Usuario"));
                }

                if (contrasenia == null || contrasenia == "")
                {
                    throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "Contraseña"));
                }

                LoginWSMOAResponse response = new UsuarioNuevoConsumerMOA().request(username, numeroProveedor, claveActivacion, contrasenia);

                if (response.error == "10")
                {
                    throw new ValidationCustomException(response.texto);
                }

                return response;
            }
            catch (InfoCustomException e)
            {
                throw e;
            }
            catch (ValidationCustomException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        [Obsolete]
        public string alta(UsuarioAlta usuario)
        {
            try
            {

                UsuarioCrearWSMOAResponse response = new UsuarioCrearConsumerMOA().request(usuario.email, usuario.numeroProveedor, usuario.perfil, usuario.tipo);

                if (response.error == "99")
                {
                    throw new ValidationCustomException(response.texto);
                }

                return SuccessMsg.AltaUsuarioOK;

            }
            catch (InfoCustomException e)
            {
                throw e;
            }
            catch (ValidationCustomException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        [Obsolete]
        public string recuperarContrasenia(string usename)
        {
            try
            {
                if (usename == null || usename == "")
                {
                    throw new ValidationCustomException(String.Format(ErrorMsg.ErrorCompleteCampo, "Usuario"));
                }

                LoginWSMOAResponse response = new UsuarioOlvidePassConsumerMOA().request(usename);

                if (response.error != "11")
                {
                    throw new ValidationCustomException(response.texto);
                }

                return SuccessMsg.OlvideContraniaOk;

            }
            catch (InfoCustomException e)
            {
                throw e;
            }
            catch (ValidationCustomException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        [Obsolete]
        public string desbloquear(string usename)
        {
            try
            {
                if (usename == null || usename == "")
                {
                    throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "Nombre de Usuario"));
                }

                LoginWSMOAResponse response = new UsuarioDesbloquearConsumerMOA().request(usename);

                if (response.error == "10")
                {
                    throw new ValidationCustomException(response.texto == null ? String.Format(ErrorMsg.ErrorUsuarioDesbloquear, usename) : response.texto);
                }

                return String.Format(SuccessMsg.UsuarioDesbloqueadoOK, usename); ;

            }
            catch (InfoCustomException e)
            {
                throw e;
            }
            catch (ValidationCustomException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public string HabilitarUsuario(string usuarioMail)
        {
            Entidades.Usuario usuario = repositorio.Obtener<Entidades.Usuario>(u => u.Mail == usuarioMail);

            var proveedor = usuario.ObtenerProveedorActual();
            
            proveedor.EstadoAprobacion = EstadoAprobacion.Aprobado;

            usuario.Habilitado = true;

            var rolUsuario = ObtenerRolPorTipo(usuario.TipoUsuario.NombreCorto);

            usuario.RemoverRoles();

            usuario.AgregarRol(rolUsuario);

            repositorio.GuardarCambios();

            return string.Format(SuccessMsg.UsuarioHabilitadoOK, usuario.Mail);
        }

        public string DeshabilitarUsuario(string usuarioMail)
        {
            Entidades.Usuario usuario = repositorio.Obtener<Entidades.Usuario>(u => u.Mail == usuarioMail);

            usuario.Habilitado = false;

            usuario.RemoverRoles();

            var proveedor = usuario.ObtenerProveedorActual();

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

        public Rol ObtenerRolPorTipo(string tipoUsuario)
        {
            var codigo = "";
            switch (tipoUsuario.ToLower())
            {
                case "g":
                    codigo = "GRAN";
                    break;

                case "ng":
                    codigo = "NOGRAN";
                    break;

                case "a":
                case "corr":
                    codigo = "GYNG";
                    break;

                case "cli":
                    codigo = "CLIENT";
                    break;
            }

            return ObtenerRolPorCodigo(codigo);
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
            return repositorio.Listar<Rol>().Select(x => new RolDropdownDto(x)).ToList();
        }

        public string GuardarRoles(List<int> idRoles, int idUsuario)
        {
            Entidades.Usuario usuario = repositorio.Obtener<Entidades.Usuario>(u => u.Id == idUsuario);

            usuario.RemoverRoles();
            
            foreach (int idRol in idRoles)
            {
                Rol rolAAgregar = repositorio.Obtener<Rol>(r => r.Id == idRol);
                usuario.AgregarRol(rolAAgregar);
            }
            
            repositorio.GuardarCambios();

            return string.Format(SuccessMsg.RolesActualizadosOk, usuario.Mail);
        }
    }
}
