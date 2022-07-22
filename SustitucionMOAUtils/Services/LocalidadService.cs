using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ScatoWebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Services
{
    public class LocalidadService : ILocalidadService
    {

        readonly IScatoConsumer scatoConsumer;
        protected readonly IRepositorio repositorio;
        public LocalidadService(IScatoConsumer scatoConsumer, IRepositorio repositorio)
        {
            this.scatoConsumer = scatoConsumer;
            this.repositorio = repositorio;
        }

        public List<LocalidadDto> SincronizarLocalidadesScato()
        {                        
           return scatoConsumer.ObtenerLocalidades();                                                
        }

        public List<ProvinciaDto> ObtenerProvinciasScato()
        {
            return scatoConsumer.ObtenerProvincias();
        }

        public List<Localidad> GetLocalidades()
        {
            return repositorio.Listar<Localidad>().ToList();     
        }
    }
}

