using System;
using System.Collections.Generic;
using System.IO;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models.WSMapMOA.Login;
using SustitucionMOAModel.Models.WSMapMOA.Usuario;
using SustitucionMOAModel.Models.WSMapMOA.Usuario.Perfil;
using SustitucionMOAWS.WSConsumers;

namespace SustitucionMOAUtils.Services
{
    public class UsuarioService
    {
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
                foreach (Perfil perfil in response.perfiles) {
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


        public UsuariosWSMOAResponse getUsuarios()
        {
            try
            {
                UsuariosWSMOAResponse response = new UsuariosConsumerMOA().request();

                if (response.usuarios.Count == 0)
                {
                    throw new InfoCustomException(String.Format(InfoMsg.SinRegistros,"Usuarios"));
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

        public LoginWSMOAResponse registrar(string numeroProveedor, string claveActivacion, string username, string contrasenia) {
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

                if (response.error == "10") {
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

        public string alta(UsuarioAlta usuario)
        {
            try
            {

                UsuarioCrearWSMOAResponse response = new UsuarioCrearConsumerMOA().request(usuario.email, usuario.numeroProveedor, usuario.perfil, usuario.tipo);

                if(response.error == "99")
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

        public string recuperarContrasenia(string usename)
        {
            try
            {
                if (usename == null || usename == "")
                {
                    throw new ValidationCustomException(String.Format(ErrorMsg.ErrorCompleteCampo, "Usuario"));
                }

                LoginWSMOAResponse response = new UsuarioOlvidePassConsumerMOA().request(usename);

                if (response.error != "11") {
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
                    throw new ValidationCustomException(response.texto == null ? String.Format(ErrorMsg.ErrorUsuarioDesbloquear,usename) : response.texto);
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

        public string deshabilitar(string usename)
        {
            try
            {
                if (usename == null || usename == "")
                {
                    throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "Nombre de Usuario"));
                }

                LoginWSMOAResponse response = new UsuarioInhabilitarConsumerMOA().request(usename);

                if (response.error == "10")
                {
                    throw new ValidationCustomException(response.texto == null ? String.Format(ErrorMsg.ErrorUsuarioDeshabilitar, usename) : response.texto);
                }

                return String.Format(SuccessMsg.UsuarioDeshabilitadoOK, usename);

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

        public string habilitar(string usename)
        {
            try
            {
                if (usename == null || usename == "")
                {
                    throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "Nombre de Usuario"));
                }

                LoginWSMOAResponse response = new UsuarioHabilitarConsumerMOA().request(usename);

                if (response.error == "10")
                {
                    throw new ValidationCustomException(response.texto == null ? String.Format(ErrorMsg.ErrorUsuarioDeshabilitar, usename) : response.texto );
                }

                return String.Format(SuccessMsg.UsuarioHabilitadoOK, usename);

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

        public byte[] getDocumento(string nombre) {
            string sourcePath = @"\\vicinf01\Legajo_Impositivo\Documentacion_MoaOperaciones";
            try
            {
                string[] filepaths = Directory.GetFiles(sourcePath);
                foreach (string filepath in filepaths)
                {
                    string result = Path.GetFileName("@\\" + filepath);
                    if (nombre == result) {
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
    }
}
