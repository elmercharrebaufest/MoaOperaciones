CREATE TABLE [dbo].[SubCategoria]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](max) NOT NULL,
	[Nombre] [nvarchar](max) NOT NULL,
	[Categoria_Id] INT NOT NULL
CONSTRAINT [PK_dbo.SubCategoria] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[SubCategoria]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SubCategoria_dbo.Categoria_Categoria_Id] FOREIGN KEY([Categoria_Id])
REFERENCES [dbo].[Categoria] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[SubCategoria] CHECK CONSTRAINT [FK_dbo.SubCategoria_dbo.Categoria_Categoria_Id]
GO