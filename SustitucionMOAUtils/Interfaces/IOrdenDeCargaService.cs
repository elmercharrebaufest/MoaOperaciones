using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IOrdenDeCargaService
    {
        string Agregar(OrdenDeCarga ordenDeCarga, string mailUsuario);
        List<OrdenDeCargaDto> Listar(string mailUsuario);
        string AnularOrden(int ordenId);
        string NotificarTransporte(int ordenId);
        List<string> ObtenerContratos(int ordenId);
        List<string> ObtenerCorredores(int ordenId);
        string SeleccionarContrato(int ordenId, string contratoSAP);
        string SeleccionarCorredor(int ordenId, string corredor);
        string VerificarSituacionCrediticia(int ordenId);
        string VerificarTransporte(int ordenId);
    }

}
