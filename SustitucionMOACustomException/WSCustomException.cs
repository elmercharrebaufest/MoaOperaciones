using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.CustomExceptions
{
    public class WSCustomException : Exception
    {
        public WSCustomException() : base() { }
        public WSCustomException(string msj) : base(msj) { }
        public WSCustomException(string msj, Exception e) : base(msj, e) { }
    }
}
