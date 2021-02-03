using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.ViewModel.AltaEmpresa
{
    public class AltaEmpresaViewModel
    {
        public bool? VinculoConEmpleadosDeMolinos { get; set; }
        public List<AltaEmpresaEmpleadosViewModel> Empleados { get; set; } = new List<AltaEmpresaEmpleadosViewModel>();

        public bool? VinculoConFuncionariosPublicos { get; set; }

        public List<AltaEmpresaFuncionariosViewModel> Funcionarios { get; set; } = new List<AltaEmpresaFuncionariosViewModel>();
        
        public string CBU { get; set; }
        public int? IdSituacionIVA { get; set; }
        public int? IdIngresoBruto { get; set; }

        public string Comentarios { get; set; }
    }


    public class AltaEmpresaEmpleadosViewModel
    {
        public string NombreProveedora { get; set; }
        public string CargoProveedora { get; set; }
        public string NombreMolinos { get; set; }
        public string Vinculo { get; set; }
       
    }
    public class AltaEmpresaFuncionariosViewModel
    {
        public string NombreFirma { get; set; }
        public string CargoFirma { get; set; }
        public string NombreFuncionario { get; set; }
        public string CargoFuncionario { get; set; }
        public string Vinculo { get; set; }

    }
}
