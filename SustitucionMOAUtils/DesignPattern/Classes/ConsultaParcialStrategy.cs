using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
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
    public class ConsultaParcialStrategy: ConsultaCommon, IConsultaStrategy
    {
        public List<string> Names => new List<string>() { "Parcial" };
        private readonly IUsuarioService usuarioService;

        public ConsultaParcialStrategy(IRepositorio repositorio, IEmailService emailService, IUsuarioService usuarioService) : base(repositorio, emailService)
        {
            this.usuarioService = usuarioService;
        }

        public Consulta AgregarConsulta(Consulta consulta, Comentario comentario, List<DestinatarioDto> destinatarios, HttpFileCollectionBase files)
        {
            try
            {
                this.RellenarCampos(consulta, comentario);
                Usuario usuario = this.repositorio.Obtener<Usuario>(u => u.Id == consulta.Usuario_Id);
                var subcategoria = this.repositorio.Obtener<SubCategoria>(s => s.Id == consulta.SubCategoria_Id);

                if (usuario.TipoUsuario.NombreCorto == "CORR")
                {
                    consulta.Categoria_Id = repositorio.Obtener<Categoria>(c => c.Code == "PARCOR").Id;
                    var subcategoriaCode = subcategoria.Code + "PC";
                    consulta.SubCategoria_Id = repositorio.Obtener<SubCategoria>(sc => sc.Code == subcategoriaCode).Id;
                }
                else
                {
                    consulta.Categoria_Id = repositorio.Obtener<Categoria>(c => c.Code == "PARDIR").Id;
                    var subcategoriaCode = subcategoria.Code + "PD";
                    consulta.SubCategoria_Id = repositorio.Obtener<SubCategoria>(sc => sc.Code == subcategoriaCode).Id;
                }

                this.repositorio.Agregar(consulta);
                this.repositorio.GuardarCambios();

                if (files.Count > 0)
                {
                    GuardarAdjuntoComentario(consulta.Id, files);
                }

                EnviarMail(consulta, comentario, files);

                return consulta;
            }
            catch (Exception e)
            {
                Log.Error("Ha ocurrido un error al intentar generar la consulta.", e);
                throw e;
            }
        }
    }
}
