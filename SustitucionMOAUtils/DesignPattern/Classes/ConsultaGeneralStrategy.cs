using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.DesignPattern.Interfaces;
using SustitucionMOAUtils.Helpers;
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
    public class ConsultaGeneralStrategy : ConsultaCommon, IConsultaStrategy
    {
        public List<string> Names => new List<string>() { "Reclamo Impositivo", "Boletos",
        "Calidades", "Comisiones", "Solicitud de comprobantes", "Aplicaciones", "Pesificaciones", "Pagos",
        "Funcionamiento Web", "Operaciones MATBA", "Proveedores generales", "Fletes", "Otros", 
            "Cesión y rectificación de CPE", "Parcial Corredor", "Final Corredor", "Parcial Directo", "Final Directo" };

        private readonly IUsuarioService usuarioService;

        public ConsultaGeneralStrategy(IRepositorio repositorio, IEmailService emailService, IUsuarioService usuarioService) : base(repositorio, emailService)
        {
            this.usuarioService = usuarioService;
        }

        public Consulta AgregarConsulta(Consulta consulta, Comentario comentario, List<DestinatarioDto> destinatarios, HttpFileCollectionBase files)
        {
            try
            {
                this.RellenarCampos(consulta, comentario);
                this.repositorio.Agregar(consulta);
                this.repositorio.GuardarCambios();

                if(files.Count > 0)
                {
                    GuardarAdjuntoComentario(consulta.Id, files);
                }

                EnviarMail(consulta, comentario, files);

                return consulta;
            }
            catch(Exception e)
            {
                Log.Error("Ha ocurrido un error al intentar generar la consulta.", e);
                throw e;
            }
        }

    }
}
