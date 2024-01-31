using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Vendedor
{
    public class Vendedor
    {
        public string fecha { get; set; }

        public string idVendedor { get; set; }

        public string descVendedor { get; set; }

        public string estado { get; set; }

        public string estadoMoa { get; set; }

        public string cuit { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Vendedor dto &&
                   idVendedor == dto.idVendedor &&
                   descVendedor == dto.descVendedor;
        }

        public override int GetHashCode()
        {
            int hashCode = 1873437470;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(idVendedor);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(descVendedor);
            return hashCode;
        }
    }

}
