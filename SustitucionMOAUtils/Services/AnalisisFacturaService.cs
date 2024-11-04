using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOAModel.Models.ViewModel.AltaEmpresa;
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
using System.Threading.Tasks;
using System.IO.Compression;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Email;
using System.Text.RegularExpressions;
using System.Globalization;
using SustitucionMOAUtils.Services.AnalisisFacturaServiceValidation;
using SustitucionMOAModel.Models;

namespace SustitucionMOAUtils.Services
{
    public class AnalisisFacturaService : IAnalisisFacturaService
    {
        protected readonly IRepositorio repositorio;

        public AnalisisFacturaService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public List<ValidationResult> AnalizarDocumento(List<string> documento, string cuitProveedor)
        {            
            var handler = new AnalisisFacturaServiceValidationHandler();
            handler.AddValidation(new CuitValidationCommand("30715118773"));
            handler.AddValidation(new CuitValidationCommand(cuitProveedor));
            handler.AddValidation(new NumeroFacturaValidationCommand());
            handler.AddValidation(new OrdenCompraValidationCommand());
            handler.AddValidation(new CodigoFacturaValidationCommand());

            List<ValidationResult> results = handler.ExecuteValidations(documento);

            return results;

        }
    }
}
