using DocumentFormat.OpenXml.Office2010.Excel;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using SustitucionMOAModel.Enums.SustitucionMOAModel.Enums;

namespace SustitucionMOAUtils.Services
{
    public class AplicacionCartaPorteService : IAplicacionCartaPorteService
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
            var rawList = new List<AplicacionCartaPorte>()
            {
                new AplicacionCartaPorte()
                    {
                        FechaAlta= DateTime.Now,
                        FechaActualizacion = DateTime.Now,
                        Contrato =  "00123024EF",
                        CartaPorte= "090009090",
                        Kilogramos= 300000,
                        Estado= EstadoAplicacionCartaPorte.Pendiente,
                    },
                 new AplicacionCartaPorte()
                    {
                        FechaAlta= DateTime.Now,
                        FechaActualizacion = DateTime.Now,
                        Contrato =  "00123024EF",
                        CartaPorte= "090009090",
                        Kilogramos= 300000,
                        Estado= EstadoAplicacionCartaPorte.Error,
                        Error = "Todo mal aca locoooo"
                    },
                  new AplicacionCartaPorte()
                    {
                        FechaAlta= DateTime.Now,
                        FechaActualizacion = DateTime.Now,
                        Contrato =  "00123024EF",
                        CartaPorte= "090009090",
                        Kilogramos= 300000,
                        Estado= EstadoAplicacionCartaPorte.Aplicado,
                    },
                   new AplicacionCartaPorte()
                    {
                        FechaAlta= DateTime.Now,
                        FechaActualizacion = DateTime.Now,
                        Contrato =  "00123024EF",
                        CartaPorte= "090009090",
                        Kilogramos= 300000,
                        Estado= EstadoAplicacionCartaPorte.Pendiente,
                    },
            };
            var lista = rawList.Select(apl => new AplicacionCartaPorteDto(apl)).ToList();
            return lista;
        }
        public AplicacionCartaPorteDto Obtener(int aplicacionCCPPId, string mailUsuario)
        {
            return new AplicacionCartaPorteDto(new AplicacionCartaPorte());
        }
        public AplicacionCartaPorteFiltrosDto ObtenerFiltros(List<AplicacionCartaPorteDto> aplicaciones)
        {
            return new AplicacionCartaPorteFiltrosDto(aplicaciones);
        }
    }
}
