using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
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
    public class ConsultaParcialStrategy: IConsultaStrategy
    {
        public List<string> Names => new List<string>() { "Parcial" };

        private readonly IRepositorio repositorio;
        private readonly IUsuarioService usuarioService;

        public ConsultaParcialStrategy(IRepositorio repositorio, IUsuarioService usuarioService)
        {
            this.repositorio = repositorio;
            this.usuarioService = usuarioService;
        }

        public Consulta AgregarConsulta(Consulta consulta, Comentario comentario)
        {
            try
            {
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

                comentario.Usuario_Id = (int)consulta.UsuarioInterno_Id;

                this.repositorio.Agregar(consulta);
                this.repositorio.GuardarCambios();
                return consulta;
            }
            catch (Exception e)
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
