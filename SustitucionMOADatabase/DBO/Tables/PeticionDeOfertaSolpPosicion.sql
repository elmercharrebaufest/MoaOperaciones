CREATE TABLE [dbo].[PeticionDeOfertaSolpPosicion](
	[PeticionDeOferta_Id] [int] NOT NULL,
	[SolpPosicion_Id] [int] NOT NULL,
 CONSTRAINT [PK_dbo.PeticionDeOfertaSolpPosicion] PRIMARY KEY CLUSTERED 
(
	[PeticionDeOferta_Id] ASC,
	[SolpPosicion_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PeticionDeOfertaSolpPosicion]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PeticionDeOfertaSolpPosicion_dbo.PeticionDeOferta_PeticionDeOferta_Id] FOREIGN KEY([PeticionDeOferta_Id])
REFERENCES [dbo].[PeticionDeOferta] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[PeticionDeOfertaSolpPosicion] CHECK CONSTRAINT [FK_dbo.PeticionDeOfertaSolpPosicion_dbo.PeticionDeOferta_PeticionDeOferta_Id]
GO

ALTER TABLE [dbo].[PeticionDeOfertaSolpPosicion]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PeticionDeOfertaSolpPosicion_dbo.SolpPosicion_SolpPosicion_Id] FOREIGN KEY([SolpPosicion_Id])
REFERENCES [dbo].[SolpPosicion] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[PeticionDeOfertaSolpPosicion] CHECK CONSTRAINT [FK_dbo.PeticionDeOfertaSolpPosicion_dbo.SolpPosicion_SolpPosicion_Id]
GO

