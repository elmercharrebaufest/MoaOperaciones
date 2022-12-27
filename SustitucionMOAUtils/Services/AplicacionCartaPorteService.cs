using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System.Collections.Generic;


namespace SustitucionMOAUtils.Services
{
    public class AplicacionCartaPorteService: IAplicacionCartaPorteService
    {
        protected readonly IRepositorio _repositorio;
        //protected readonly IAplicacionCartaPorteConsumerMOA _consumer;
        public AplicacionCartaPorteService(
            IRepositorio repositorio
            //IAplicacionCartaPorteConsumerMOA consumer,
            )
        {
            this._repositorio = repositorio;
            //this._consumer = consumer;
        }
        public Resultado Agregar(AplicacionCartaPorte aplicacionCCPP, string mailUsuario)
        {
            return new Resultado() { Mensaje = "Testeo exitoso" };
        }
        public List<AplicacionCartaPorteDto> Listar(string mailUsuario, string fechaInicio, string fechaFin)
        {
            return new List<AplicacionCartaPorteDto>();
        }
        public AplicacionCartaPorteDto Obtener(int aplicacionCCPPId, string mailUsuario)
        {
            return new AplicacionCartaPorteDto();
        }
    }
}
