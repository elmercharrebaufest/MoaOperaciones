using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOACustomException
{
    public class DBCustomException : Exception
    {
        public DBCustomException() : base() { }
        public DBCustomException(string msj) : base(msj) { }
        public DBCustomException(string msj, Exception e) : base(msj, e) { }
    }
}
