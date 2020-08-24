using Microsoft.Identity.Client;
using Microsoft.Owin.Security;
using SustitucionMOA.Utils;
using SustitucionMOAAssets;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.DataAgro;
using SustitucionMOAModel.Models.WSMapMOA.Noticia;
using SustitucionMOARepositorio;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Entidades = SustitucionMOAModel.Entities;
using Model = SustitucionMOAModel.Models;

namespace SustitucionMOA.Controllers
{
    public class AzureB2CController : Controller
    {
        protected readonly IRepositorio repositorio;
        protected readonly IAzureB2CService azureB2CService;
        protected readonly IDataAgroService dataAgroService;


        public AzureB2CController(IRepositorio repositorio, IAzureB2CService azureB2CService, IDataAgroService dataAgroService)
        {
            this.repositorio = repositorio;
            this.azureB2CService = azureB2CService;
            this.dataAgroService = dataAgroService;
        }
    }
}
