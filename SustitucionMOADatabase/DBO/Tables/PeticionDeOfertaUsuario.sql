CREATE TABLE [dbo].[PeticionDeOfertaUsuario](
    [Id] INT NOT NULL IDENTITY, 
	[PeticionDeOferta_Id] [int] NOT NULL,
	[Usuario_Id] [int] NOT NULL, 
	[RealizoVisita] bit  NULL, 
	[RealizoVisitaFecha]          DATETIME2 (7)  NULL,
	[RealizoVisitaUsuario_Id] [int]  NULL, 
	[PropuestaTecnicaAprobada] bit  NULL, 
	[PropuestaTecnicaFecha]  DATETIME2 (7)  NULL,
	[PropuestaTecnicaUsuario_Id] [int] NULL,   
    [ObservacionNoCumple] NVARCHAR(MAX) NULL, 
    [VisibleSolicitante] BIT NULL, 
    CONSTRAINT [FK.PeticionDeOfertaUsuario_Usuario_UsuarioCreadorId] FOREIGN KEY ([Usuario_Id]) REFERENCES [Usuario]([Id]),
	CONSTRAINT [FK.PeticionDeOfertaUsuario_Usuario_RealizoVisitaUsuario_Id] FOREIGN KEY ([RealizoVisitaUsuario_Id]) REFERENCES [Usuario]([Id]),
    CONSTRAINT [FK.PeticionDeOfertaUsuario_Usuario_PropuestaTecnicaUsuario_Id] FOREIGN KEY ([PropuestaTecnicaUsuario_Id]) REFERENCES [Usuario]([Id]),
	CONSTRAINT [PK_PeticionDeOfertaUsuario] PRIMARY KEY ([Id]),
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PeticionDeOfertaUsuario]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PeticionDeOfertaUsuario_dbo.PeticionDeOferta_PeticionDeOferta_Id] FOREIGN KEY([PeticionDeOferta_Id])
REFERENCES [dbo].[PeticionDeOferta] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[PeticionDeOfertaUsuario] CHECK CONSTRAINT [FK_dbo.PeticionDeOfertaUsuario_dbo.PeticionDeOferta_PeticionDeOferta_Id]
GO

ALTER TABLE [dbo].[PeticionDeOfertaUsuario]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PeticionDeOfertaUsuario_dbo.Usuario_Usuario_Id] FOREIGN KEY([Usuario_Id])
REFERENCES [dbo].[Usuario] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[PeticionDeOfertaUsuario] CHECK CONSTRAINT [FK_dbo.PeticionDeOfertaUsuario_dbo.Usuario_Usuario_Id]
GO

