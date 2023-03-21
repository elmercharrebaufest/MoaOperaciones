CREATE TABLE [dbo].[CircularPeticionDeOfertaUsuario](
    [Id] INT NOT NULL IDENTITY, 
	[PeticionDeOfertaUsuario_Id] [int] NOT NULL,
	[Circular_Id] [int] NOT NULL, 
	[Leida] bit  NULL, 
	[FechaLeida]          DATETIME2 (7)  NULL,
    CONSTRAINT [PK_CircularPeticionDeOfertaUsuario] PRIMARY KEY ([Id]),
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CircularPeticionDeOfertaUsuario]  WITH CHECK ADD  CONSTRAINT [FK_dbo.CircularPeticionDeOfertaUsuario_dbo.PeticionDeOfertaUsuario_PeticionDeOfertaUsuario_Id] FOREIGN KEY([PeticionDeOfertaUsuario_Id])
REFERENCES [dbo].[PeticionDeOfertaUsuario] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[CircularPeticionDeOfertaUsuario] CHECK CONSTRAINT [FK_dbo.CircularPeticionDeOfertaUsuario_dbo.PeticionDeOfertaUsuario_PeticionDeOfertaUsuario_Id]
GO

ALTER TABLE [dbo].[CircularPeticionDeOfertaUsuario]  WITH CHECK ADD  CONSTRAINT [FK_dbo.CircularPeticionDeOfertaUsuario_dbo.Circular_Circular_Id] FOREIGN KEY([Circular_Id])
REFERENCES [dbo].[Circular] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[CircularPeticionDeOfertaUsuario] CHECK CONSTRAINT [FK_dbo.CircularPeticionDeOfertaUsuario_dbo.Circular_Circular_Id]
GO

