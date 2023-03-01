CREATE TABLE [dbo].[PeticionDeOfertaUsuario](
	[PeticionDeOferta_Id] [int] NOT NULL,
	[Usuario_Id] [int] NOT NULL,
 CONSTRAINT [PK_dbo.PeticionDeOfertaUsuario] PRIMARY KEY CLUSTERED 
(
	[PeticionDeOferta_Id] ASC,
	[Usuario_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
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

