CREATE TABLE [dbo].[CarpetasUCROPIT]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Cosecha_Id] INT NOT NULL, 
    [EPA] BIT NOT NULL, 
    [EUDER] BIT NOT NULL, 
    [BSVS2] BIT NOT NULL, 
    [UrlSubida] NVARCHAR(500) NOT NULL,
    CONSTRAINT [FK_CarpetasUCROPIT_ToCosecha] FOREIGN KEY ([Cosecha_Id]) REFERENCES [Cosecha]([Id]),
)
