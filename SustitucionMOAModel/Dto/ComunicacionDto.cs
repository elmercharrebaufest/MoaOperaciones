using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAModel.Dto
{
    public class ComunicacionDto
    {
        public int Id { get; set; }
        public int ComunicacionTipo { get; set; }
        public string ProveedorId { get; set; }
        public string FechaCreacion { get; set; }
        public bool Leida { get; set; }

        //public void Add(ComunicacionDto comunicacionDto)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
