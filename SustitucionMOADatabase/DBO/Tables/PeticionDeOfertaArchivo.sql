CREATE TABLE [dbo].[PeticionDeOfertaArchivo](
    [Id] INT NOT NULL IDENTITY, 
	[PeticionDeOferta_Id] [int] NOT NULL,
	[Archivo_Id] [int] NOT NULL, 
    [Fecha] DATETIME2 NOT NULL, 
    CONSTRAINT [PK_PeticionDeOfertaArchivo] PRIMARY KEY ([Id]),
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PeticionDeOfertaArchivo]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PeticionDeOfertaArchivo_dbo.PeticionDeOferta_PeticionDeOferta_Id] FOREIGN KEY([PeticionDeOferta_Id])
REFERENCES [dbo].[PeticionDeOferta] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[PeticionDeOfertaArchivo] CHECK CONSTRAINT [FK_dbo.PeticionDeOfertaArchivo_dbo.PeticionDeOferta_PeticionDeOferta_Id]
GO

ALTER TABLE [dbo].[PeticionDeOfertaArchivo]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PeticionDeOfertaArchivo_dbo.Archivo_Archivo_Id] FOREIGN KEY([Archivo_Id])
REFERENCES [dbo].[Archivo] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[PeticionDeOfertaArchivo] CHECK CONSTRAINT [FK_dbo.PeticionDeOfertaArchivo_dbo.Archivo_Archivo_Id]
GO

