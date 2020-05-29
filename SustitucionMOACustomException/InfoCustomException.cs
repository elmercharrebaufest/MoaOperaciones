using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.CustomExceptions
{
    public class InfoCustomException : Exception
    {
        public InfoCustomException() : base() { }
        public InfoCustomException(string msj) : base(msj) { }
        public InfoCustomException(string msj, Exception e) : base(msj, e) { }
    }
}
