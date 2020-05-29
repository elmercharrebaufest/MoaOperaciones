using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.CustomExceptions
{
    public class ValidationCustomException : Exception
    {
        public ValidationCustomException() : base() { }
        public ValidationCustomException(string msj) : base(msj) { }
        public ValidationCustomException(string msj, Exception e) : base(msj, e) { }
    }
}
