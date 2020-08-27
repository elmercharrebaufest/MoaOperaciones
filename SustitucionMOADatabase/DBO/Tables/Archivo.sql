
CREATE TABLE [dbo].[Archivo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FileKey] [nvarchar](max) NULL,
	[Ruta] [nvarchar](max) NULL,
	[Usuario_Id] [int] NULL,
 CONSTRAINT [PK_dbo.Archivo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Archivo]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Archivo_dbo.Usuario_Usuario_Id] FOREIGN KEY([Usuario_Id])
REFERENCES [dbo].[Usuario] ([Id])
GO

ALTER TABLE [dbo].[Archivo] CHECK CONSTRAINT [FK_dbo.Archivo_dbo.Usuario_Usuario_Id]
GO

