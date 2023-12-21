using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.DesignPattern.Interfaces;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.DesignPattern.Classes
{
    public class ConsultaGeneralStrategy : IConsultaStrategy
    {
        public List<string> Names => new List<string>() { "Reclamo Impositivo", "Boletos", "Actualización",
        "Calidades", "Comisiones", "Solicitud de comprobantes", "Aplicaciones", "Pesificaciones", "Pagos",
        "Funcionamiento Web", "Operaciones MATBA", "Proveedores generales", "Fletes", "Otros", 
            "Cesión y rectificación de CPE" };

        private readonly IRepositorio repositorio;
        private readonly IUsuarioService usuarioService;

        public ConsultaGeneralStrategy(IRepositorio repositorio, IUsuarioService usuarioService)
        {
            this.repositorio = repositorio;
            this.usuarioService = usuarioService;
        }

        public Consulta AgregarConsulta(Consulta consulta, Comentario comentario)
        {
            try
            {
                comentario.Usuario_Id = (int)consulta.UsuarioInterno_Id;

                this.repositorio.Agregar(consulta);
                this.repositorio.GuardarCambios();
                return consulta;
            }
            catch(Exception e)
            {
                Log.Info(e.Message);
                return null;
            }
        }

        public Consulta AgregarConsulta(Consulta consulta, Comentario comentario, List<DestinatarioDto> destinatarios, HttpFileCollectionBase files)
        {
            throw new NotImplementedException();
        }
    }
}
