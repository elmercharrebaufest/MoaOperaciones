IF EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'EmailLinkToken'
          AND Object_ID = Object_ID(N'dbo.Solp'))
BEGIN
	UPDATE dbo.Solp
	   SET EmailLinkToken = NEWID()
	 WHERE 1 = 1
END