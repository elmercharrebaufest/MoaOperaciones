CREATE TABLE [dbo].[PeticionDeOfertaArchivo](
	[PeticionDeOferta_Id] [int] NOT NULL,
	[Archivo_Id] [int] NOT NULL,
 CONSTRAINT [PK_dbo.PeticionDeOfertaArchivo] PRIMARY KEY CLUSTERED 
(
	[PeticionDeOferta_Id] ASC,
	[Archivo_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
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

