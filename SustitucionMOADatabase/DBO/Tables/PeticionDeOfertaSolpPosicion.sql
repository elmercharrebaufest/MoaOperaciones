CREATE TABLE [dbo].[PeticionDeOfertaSolpPosicion](
    [Id] INT NOT NULL IDENTITY, 
    [PeticionDeOferta_Id] [int] NOT NULL,
    [SolpPosicion_Id] [int] NOT NULL,
    [NumeroRegistroInfo] NVARCHAR(25) NULL, 
    CONSTRAINT [PK_PeticionDeOfertaSolpPosicion] PRIMARY KEY ([Id]),
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PeticionDeOfertaSolpPosicion]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PeticionDeOfertaSolpPosicion_dbo.PeticionDeOferta_PeticionDeOferta_Id] FOREIGN KEY([PeticionDeOferta_Id])
REFERENCES [dbo].[PeticionDeOferta] ([Id])
GO

ALTER TABLE [dbo].[PeticionDeOfertaSolpPosicion] CHECK CONSTRAINT [FK_dbo.PeticionDeOfertaSolpPosicion_dbo.PeticionDeOferta_PeticionDeOferta_Id]
GO

ALTER TABLE [dbo].[PeticionDeOfertaSolpPosicion]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PeticionDeOfertaSolpPosicion_dbo.SolpPosicion_SolpPosicion_Id] FOREIGN KEY([SolpPosicion_Id])
REFERENCES [dbo].[SolpPosicion] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[PeticionDeOfertaSolpPosicion] CHECK CONSTRAINT [FK_dbo.PeticionDeOfertaSolpPosicion_dbo.SolpPosicion_SolpPosicion_Id]
GO

