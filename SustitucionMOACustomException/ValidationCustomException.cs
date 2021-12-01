using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.CustomExceptions
{
    public class ValidationCustomException : Exception
    {
        public bool LoguearExcepcion { get; set; }

        public ValidationCustomException(bool loguearExcepcion = false) : base() 
        {
            this.LoguearExcepcion = loguearExcepcion;
        }
        
        public ValidationCustomException(string msj, bool loguearExcepcion = false) : base(msj) 
        { 
            this.LoguearExcepcion = loguearExcepcion;
        }
        
        public ValidationCustomException(string msj, Exception e, bool loguearExcepcion = false) : base(msj, e) 
        { 
            this.LoguearExcepcion = loguearExcepcion; 
        }
    }
}
