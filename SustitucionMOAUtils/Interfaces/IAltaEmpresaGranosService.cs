using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IAltaEmpresaGranosService
    {
        byte[] GenerarInformeComercial(string userMail, string emplRelDep, string emplRelDepCant, string rodados, string rodadosOtros, string chacra, string chacraOtros, 
                                        string antigActividad, string actuacionProd, string clienteAnt, string comentarios, string domicilio);
        bool GuardarArchivo(HttpPostedFileBase fileSubido, string fileKey, string mailUsuario);
    }
}
