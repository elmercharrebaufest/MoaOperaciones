using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.DataAgro
{
    public partial class CookiesDataAgro
    {
        public DateTime Fecha { get; set; }
        public List<KeyValuePair<string, string>> Cookies { get; set; } = new List<KeyValuePair<string, string>>();

        public bool esValida
        {
            get
            {
                TimeSpan result = DateTime.Now.Subtract(Fecha);
                return result.TotalHours < 7;
            }
        }
    }
}
