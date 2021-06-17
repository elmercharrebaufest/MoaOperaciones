CREATE TABLE [dbo].[ComentarioRecordado]
(
	[Id] INT NOT NULL  IDENTITY(1,1), 
    [FechaRecordado] DATETIME NOT NULL, 
    [Comentario_Id] INT NOT NULL, 
    CONSTRAINT [PK_ComentarioRecordado] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ComentarioRecordado]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ComentarioRecordado_dbo.Comentario_Comentario_Id] FOREIGN KEY([Comentario_Id])
REFERENCES [dbo].[Comentario] ([Id])
ON DELETE CASCADE
GO
