
CREATE TABLE [dbo].[UsuarioNoGranos](
	[Id] [int] NOT NULL,
 CONSTRAINT [PK_dbo.UsuarioNoGranos] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UsuarioNoGranos]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UsuarioNoGranos_dbo.Usuario_Id] FOREIGN KEY([Id])
REFERENCES [dbo].[Usuario] ([Id])
GO

ALTER TABLE [dbo].[UsuarioNoGranos] CHECK CONSTRAINT [FK_dbo.UsuarioNoGranos_dbo.Usuario_Id]
GO

