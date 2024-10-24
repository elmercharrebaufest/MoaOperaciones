using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOAModel.Models.ViewModel.AltaEmpresa;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IAnalisisFacturaService
    {
        List<ValidationResult> AnalizarDocumento(List<string> documento, string cuitProveedor);
    }

    public interface IAnalisisFacturaServiceValidationCommand
    {
        List<ValidationResult> Execute(List<string> inputs);
        ValidationType ValidationCriticity { get; }

    }
}
