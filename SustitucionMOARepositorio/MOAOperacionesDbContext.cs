using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOARepositorio
{
    public class MOAOperacionesDbContext : DbContext
    {
        public MOAOperacionesDbContext() : base("CONTEXTO")
        {
            //Database.SetInitializer<MOAOperacionesDbContext>(null);
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // No usamos nombres de tablas en plural. Igualmente la pluralización fucniona solo con nombres en ingles.
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            //Se mapean todas las entidades bajo el namespace Molinos.Scato.Dominio.Entidades      
            MapearAssemblyDe<TestEntity>(modelBuilder,
                incluir: x => x.Namespace == typeof(TestEntity).Namespace,
                excluir: null);

            modelBuilder.Entity<IngresosBrutosCoeficienteUnificadoDetalle>().Property(x => x.CoeficienteGastos).HasPrecision(10, 4);
            modelBuilder.Entity<IngresosBrutosCoeficienteUnificadoDetalle>().Property(x => x.CoeficienteIngresos).HasPrecision(10, 4);
            modelBuilder.Entity<IngresosBrutosCoeficienteUnificadoDetalle>().Property(x => x.CoeficienteUnificado).HasPrecision(10, 4);

            Database.SetInitializer<MOAOperacionesDbContext>(null);
            base.OnModelCreating(modelBuilder);


            //modelBuilder.Entity<Usuario>()
            //    .HasMany<Grupo>(s => s.GrupoAsociados)
            //    .WithMany(c => c.Usuarios)
            //    .Map(cs =>
            //    {
            //        cs.MapLeftKey("Usuario_Codigo");
            //        cs.MapRightKey("Grupo_Codigo");
            //        cs.ToTable("GrupoUsuario");
            //    });
        }

        private void MapearAssemblyDe<TEntidad>(DbModelBuilder modelBuilder, Predicate<Type> incluir, Predicate<Type> excluir)
        {
            var tiposEntidades = typeof(TEntidad).Assembly.GetTypes()
                .Where(x => incluir(x) && !x.IsNestedPrivate);
            if (excluir != null)
            {
                tiposEntidades = tiposEntidades.Where(x => !excluir(x));
            }

            var metodo = modelBuilder.GetType().GetMethod("Entity");
            foreach (var tipoEntidad in tiposEntidades)
            {
                metodo.MakeGenericMethod(tipoEntidad).Invoke(modelBuilder, null);
            }
        }

    }
}
