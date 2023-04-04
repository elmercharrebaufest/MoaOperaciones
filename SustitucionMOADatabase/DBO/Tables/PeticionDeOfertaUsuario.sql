CREATE TABLE [dbo].[PeticionDeOfertaUsuario](
    [Id] INT NOT NULL IDENTITY, 
	[PeticionDeOferta_Id] [int] NOT NULL,
	[Usuario_Id] [int] NOT NULL, 
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

