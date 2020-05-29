using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.DBMap.RYD
{
    public class DbParameter
    {
        public string nombre { get; set; }
        public object value { get; set; }

        public DbParameter(string nombre, object value) {
            this.nombre = nombre;
            this.value = value;
        }
    }
}
