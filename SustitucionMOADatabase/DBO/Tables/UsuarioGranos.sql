
CREATE TABLE [dbo].[UsuarioGranos](
	[Id] [int] NOT NULL,
	[Comercial] [nvarchar](max) NULL,
    CONSTRAINT [PK_dbo.UsuarioGranos] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[UsuarioGranos]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UsuarioGranos_dbo.Usuario_Id] FOREIGN KEY([Id])
REFERENCES [dbo].[Usuario] ([Id])
GO

ALTER TABLE [dbo].[UsuarioGranos] CHECK CONSTRAINT [FK_dbo.UsuarioGranos_dbo.Usuario_Id]
GO

