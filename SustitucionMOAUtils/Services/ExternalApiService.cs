using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.WSMapMOA.Login;
using SustitucionMOAModel.Models.WSMapMOA.Usuario;
using SustitucionMOAModel.Models.WSMapMOA.Usuario.Perfil;
using SustitucionMOAModel.Models.WSMapMOA.Vendedor;
using SustitucionMOAModel.Models.WSMapMOA.Vendedor.Detalle;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Entidades = SustitucionMOAModel.Entities;
using Models = SustitucionMOAModel.Models;


namespace SustitucionMOAUtils.Services
{
    public class ExternalApiService : IExternalApiService
    {
        protected readonly IRepositorio repositorio;

        public ExternalApiService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetUsuarioApiKey(string apikey)
        {
            try
            {
                Entidades.Usuario usuario = repositorio.Obtener<Entidades.Usuario>(u => u.ApiKey == apikey);
                if (usuario == null)
                    return string.Empty;

                return usuario.Mail;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
