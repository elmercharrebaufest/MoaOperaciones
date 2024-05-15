CREATE TABLE [dbo].[AreaUsuario](
	[Area_ID] [int] NOT NULL,
	[Usuario_ID] [int] NOT NULL,
	CONSTRAINT [PK_dbo.AreaUsuario] PRIMARY KEY CLUSTERED (
		[Area_ID] ASC,
		[Usuario_ID] ASC
	) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[AreaUsuario]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AreaUsuario_dbo.Area_Area_Id] FOREIGN KEY([Area_Id])
REFERENCES [dbo].[Area] ([ID_Area])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[AreaUsuario] CHECK CONSTRAINT [FK_dbo.AreaUsuario_dbo.Area_Area_Id]
GO

ALTER TABLE [dbo].[AreaUsuario]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AreaUsuario_dbo.Usuario_Usuario_Id] FOREIGN KEY([Usuario_ID])
REFERENCES [dbo].[Usuario] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[AreaUsuario] CHECK CONSTRAINT [FK_dbo.AreaUsuario_dbo.Usuario_Usuario_Id]
GO