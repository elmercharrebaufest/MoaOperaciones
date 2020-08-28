namespace SustitucionMOARepositorio.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Archivo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FileKey = c.String(),
                        Ruta = c.String(),
                        Usuario_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Usuario", t => t.Usuario_Id)
                .Index(t => t.Usuario_Id);
            
            CreateTable(
                "dbo.Contacto",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                        Cargo = c.String(),
                        Telefonos = c.String(),
                        Mail = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Localidad",
                c => new
                    {
                        LocalidadId = c.Int(nullable: false, identity: true),
                        CodLocalidad = c.Int(nullable: false),
                        Nombre = c.String(),
                        ProvinciaId = c.Int(nullable: false),
                        PartidoId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LocalidadId)
                .ForeignKey("dbo.Partido", t => t.PartidoId, cascadeDelete: true)
                .ForeignKey("dbo.Provincia", t => t.ProvinciaId, cascadeDelete: true)
                .Index(t => t.ProvinciaId)
                .Index(t => t.PartidoId);
            
            CreateTable(
                "dbo.Partido",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Descripcion = c.String(),
                        ProvinciaID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Provincia",
                c => new
                    {
                        ProvinciaId = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                        Orden = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProvinciaId);
            
            CreateTable(
                "dbo.ProveedorHistorialAprobacion",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Proveedor_Id = c.Int(nullable: false),
                        Usuario_Id = c.Int(nullable: false),
                        EstadoAprobacion = c.Int(nullable: false),
                        Observacion = c.String(),
                        Fecha = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Proveedor", t => t.Proveedor_Id, cascadeDelete: true)
                .ForeignKey("dbo.Usuario", t => t.Usuario_Id, cascadeDelete: true)
                .Index(t => t.Proveedor_Id)
                .Index(t => t.Usuario_Id);
            
            CreateTable(
                "dbo.Proveedor",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CUIT = c.String(),
                        RazonSocial = c.String(),
                        CodigoProveedor = c.String(),
                        Mail = c.String(),
                        EstadoAprobacion = c.Int(nullable: false),
                        Observaciones = c.String(),
                        IdDataAgro = c.Int(),
                        IdComercialDataAgro = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Usuario",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Mail = c.String(),
                        CUITRegistro = c.String(),
                        Habilitado = c.Boolean(nullable: false),
                        TipoUsuario_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TipoUsuario", t => t.TipoUsuario_Id)
                .Index(t => t.TipoUsuario_Id);
            
            CreateTable(
                "dbo.Rol",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Codigo = c.String(),
                        Nombre = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PermisoPorRol",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Permiso = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TipoUsuario",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                        NombreCorto = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TestEntity",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RolPermisoPorRol",
                c => new
                    {
                        Rol_Id = c.Int(nullable: false),
                        PermisoPorRol_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Rol_Id, t.PermisoPorRol_Id })
                .ForeignKey("dbo.Rol", t => t.Rol_Id, cascadeDelete: true)
                .ForeignKey("dbo.PermisoPorRol", t => t.PermisoPorRol_Id, cascadeDelete: true)
                .Index(t => t.Rol_Id)
                .Index(t => t.PermisoPorRol_Id);
            
            CreateTable(
                "dbo.RolUsuario",
                c => new
                    {
                        Rol_Id = c.Int(nullable: false),
                        Usuario_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Rol_Id, t.Usuario_Id })
                .ForeignKey("dbo.Rol", t => t.Rol_Id, cascadeDelete: true)
                .ForeignKey("dbo.Usuario", t => t.Usuario_Id, cascadeDelete: true)
                .Index(t => t.Rol_Id)
                .Index(t => t.Usuario_Id);
            
            CreateTable(
                "dbo.ProveedorUsuario",
                c => new
                    {
                        Proveedor_Id = c.Int(nullable: false),
                        Usuario_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Proveedor_Id, t.Usuario_Id })
                .ForeignKey("dbo.Proveedor", t => t.Proveedor_Id, cascadeDelete: true)
                .ForeignKey("dbo.Usuario", t => t.Usuario_Id, cascadeDelete: true)
                .Index(t => t.Proveedor_Id)
                .Index(t => t.Usuario_Id);
            
            CreateTable(
                "dbo.UsuarioGranos",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Comercial = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Usuario", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.UsuarioNoGranos",
                c => new
                    {
                        Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Usuario", t => t.Id)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UsuarioNoGranos", "Id", "dbo.Usuario");
            DropForeignKey("dbo.UsuarioGranos", "Id", "dbo.Usuario");
            DropForeignKey("dbo.ProveedorHistorialAprobacion", "Usuario_Id", "dbo.Usuario");
            DropForeignKey("dbo.ProveedorUsuario", "Usuario_Id", "dbo.Usuario");
            DropForeignKey("dbo.ProveedorUsuario", "Proveedor_Id", "dbo.Proveedor");
            DropForeignKey("dbo.Usuario", "TipoUsuario_Id", "dbo.TipoUsuario");
            DropForeignKey("dbo.RolUsuario", "Usuario_Id", "dbo.Usuario");
            DropForeignKey("dbo.RolUsuario", "Rol_Id", "dbo.Rol");
            DropForeignKey("dbo.RolPermisoPorRol", "PermisoPorRol_Id", "dbo.PermisoPorRol");
            DropForeignKey("dbo.RolPermisoPorRol", "Rol_Id", "dbo.Rol");
            DropForeignKey("dbo.Archivo", "Usuario_Id", "dbo.Usuario");
            DropForeignKey("dbo.ProveedorHistorialAprobacion", "Proveedor_Id", "dbo.Proveedor");
            DropForeignKey("dbo.Localidad", "ProvinciaId", "dbo.Provincia");
            DropForeignKey("dbo.Localidad", "PartidoId", "dbo.Partido");
            DropIndex("dbo.UsuarioNoGranos", new[] { "Id" });
            DropIndex("dbo.UsuarioGranos", new[] { "Id" });
            DropIndex("dbo.ProveedorUsuario", new[] { "Usuario_Id" });
            DropIndex("dbo.ProveedorUsuario", new[] { "Proveedor_Id" });
            DropIndex("dbo.RolUsuario", new[] { "Usuario_Id" });
            DropIndex("dbo.RolUsuario", new[] { "Rol_Id" });
            DropIndex("dbo.RolPermisoPorRol", new[] { "PermisoPorRol_Id" });
            DropIndex("dbo.RolPermisoPorRol", new[] { "Rol_Id" });
            DropIndex("dbo.Usuario", new[] { "TipoUsuario_Id" });
            DropIndex("dbo.ProveedorHistorialAprobacion", new[] { "Usuario_Id" });
            DropIndex("dbo.ProveedorHistorialAprobacion", new[] { "Proveedor_Id" });
            DropIndex("dbo.Localidad", new[] { "PartidoId" });
            DropIndex("dbo.Localidad", new[] { "ProvinciaId" });
            DropIndex("dbo.Archivo", new[] { "Usuario_Id" });
            DropTable("dbo.UsuarioNoGranos");
            DropTable("dbo.UsuarioGranos");
            DropTable("dbo.ProveedorUsuario");
            DropTable("dbo.RolUsuario");
            DropTable("dbo.RolPermisoPorRol");
            DropTable("dbo.TestEntity");
            DropTable("dbo.TipoUsuario");
            DropTable("dbo.PermisoPorRol");
            DropTable("dbo.Rol");
            DropTable("dbo.Usuario");
            DropTable("dbo.Proveedor");
            DropTable("dbo.ProveedorHistorialAprobacion");
            DropTable("dbo.Provincia");
            DropTable("dbo.Partido");
            DropTable("dbo.Localidad");
            DropTable("dbo.Contacto");
            DropTable("dbo.Archivo");
        }
    }
}
