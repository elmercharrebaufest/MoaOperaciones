CREATE TABLE [dbo].[CampoCosecha]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [CampoSustentable_Id] INT NOT NULL,
    [Cosecha_Id] INT NOT NULL, 
    [StockDisponible] FLOAT NULL, 
    CONSTRAINT [FK_CampoCosecha_ToCampoSustentable] FOREIGN KEY ([CampoSustentable_Id]) REFERENCES [CampoSustentable](Id),
    CONSTRAINT [FK_CampoCosecha_ToCosecha] FOREIGN KEY ([Cosecha_Id]) REFERENCES [Cosecha](Id)
)
