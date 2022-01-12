CREATE TABLE [dbo].[TipoImputacionSAP]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[Descripcion] VARCHAR(200) NOT NULL,
	[Codigo] VARCHAR(10) NOT NULL,
	[TablaGeneral_Id] INT NULL,
)
