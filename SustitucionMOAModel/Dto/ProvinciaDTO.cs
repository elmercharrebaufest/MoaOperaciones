using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SustitucionMOAModel.Dto
{
    public class ProvinciaDTO
    {

        public int ProvinciaId { get; set; }

        public string Nombre { get; set; }

        public int Orden { get; set; }


        public ProvinciaDTO() { }

        public ProvinciaDTO(Provincia entity)
        {
            if (entity != null)
            {
                this.ProvinciaId = entity.ProvinciaId;
                this.Nombre = entity.Nombre;
                this.Orden = entity.Orden;
            }
        }


    }

}
