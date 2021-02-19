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
        Resultado Agregar(OrdenDeCarga ordenDeCarga, string mailUsuario);
        List<OrdenDeCargaDto> Listar(string mailUsuario);
        OrdenDeCargaDetalleDto Obtener(string mailUsuario, int ordenId);
        string AnularOrden(int ordenId);
        string NotificarTransporte(int ordenId);
        Dictionary<string, string> ObtenerContratos(int ordenId);
        Dictionary<string, string> ObtenerCorredores(int ordenId);
        string SeleccionarContrato(int ordenId, string contratoSAP);
        string SeleccionarCorredor(int ordenId, string corredor);
        string VerificarSituacionCrediticia(int ordenId);
        string VerificarTransporte(int ordenId);
        List<CorredorContratoDto> ObtenerContratosYCorredores(int ordenID);
        string SeleccionarCorredorContrato(int ordenId, CorredorContratoDto corredorContrato);
        void VerificarTransporteBulk();
    }

}
