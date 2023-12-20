using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.DesignPattern.Interfaces;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.DesignPattern.Classes
{
    public class ConsultaFinalStrategy : IConsultaStrategy
    {
        public List<string> Names => new List<string>() { "Final" };
        
        private readonly IRepositorio repositorio;
        private readonly IUsuarioService usuarioService;

        public ConsultaFinalStrategy(IRepositorio repositorio, IUsuarioService usuarioService)
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
                    consulta.Categoria_Id = repositorio.Obtener<Categoria>(c => c.Code == "FINCOR").Id;
                    var subcategoriaCode = subcategoria.Code + "FC";
                    consulta.SubCategoria_Id = repositorio.Obtener<SubCategoria>(sc => sc.Code == subcategoriaCode).Id;
                }
                else
                {
                    consulta.Categoria_Id = consulta.Categoria_Id = repositorio.Obtener<Categoria>(c => c.Code == "FINDIR").Id;
                    var subcategoriaCode = subcategoria.Code + "FD";
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
    }
}
