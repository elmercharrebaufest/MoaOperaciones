using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz.Util;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOAModel.Models.WSMapMOA;
using SustitucionMOARepositorio;
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
    public class FeriadoService : IFeriadoService
    {
        private readonly IDataAgroService dataAgroService;
        public FeriadoService(IDataAgroService dataAgroService)
        {
            this.dataAgroService = dataAgroService;
        }

        public List<DateTime> ObtenerFeriados()
        {
            var feriados = new List<DateTime>();
            try
            {
                var result = dataAgroService.ListarFeriados();

                for (int i = 0; i < result.Datos.Length; i++)
                {
                    DateTime fecha = result.Datos[i].Feriado;
                    feriados.Add(fecha);
                }
                return feriados;
            }
            catch (InfoCustomException)
            {
                return feriados;
            }
            catch (ValidationCustomException)
            {
                return feriados;
            }
            catch (Exception e)
            {
                return feriados;
            }
        }
    }
}
