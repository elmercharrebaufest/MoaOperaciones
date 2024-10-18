CREATE TABLE [logs].[LogTable]
(
	Id INT IDENTITY PRIMARY KEY,
    Date DATETIME,
    Level NVARCHAR(50),
    Logger NVARCHAR(300),
    Message NVARCHAR(MAX),
    Exception NVARCHAR(MAX)
)
