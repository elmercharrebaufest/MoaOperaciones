CREATE TABLE [dbo].[PeticionDeOfertaRevisionTecnica](
    [Id] INT NOT NULL IDENTITY, 
	[Usuario_Id] [int] NOT NULL,
    [Fecha] DATETIME2 NOT NULL, 
    CONSTRAINT [PK_PeticionDeOfertaRevisionTecnica] PRIMARY KEY ([Id]),
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PeticionDeOfertaRevisionTecnica]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PeticionDeOfertaRevisionTecnica_dbo.Usuario_Usuario_Id] FOREIGN KEY([Usuario_Id])
REFERENCES [dbo].[Usuario] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[PeticionDeOfertaRevisionTecnica] CHECK CONSTRAINT [FK_dbo.PeticionDeOfertaRevisionTecnica_dbo.Usuario_Usuario_Id]
GO
