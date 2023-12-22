using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class CentroDireccionDto
    {
        public int Id { get; set; }
        public string CodigoSap { get; set; }
        public string Direccion { get; set; }
        public string Numero { get; set; }
        public string Cp { get; set; }
        public string Pais { get; set; }
        public int RegionSap_Id { get; set; }

        public CentroDireccionDto()
        {
        }

        public CentroDireccionDto(CentroDireccion entity)
        {
            this.Id = entity.Id;
            this.CodigoSap = entity.CodigoSap;
            this.Direccion = entity.Direccion;
            this.Numero = entity.Numero;
            this.Cp = entity.Cp;
            this.Pais = entity.Pais;
            this.RegionSap_Id = entity.RegionSap_Id;
        }
    }
}
