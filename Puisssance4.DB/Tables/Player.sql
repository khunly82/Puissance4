﻿CREATE TABLE [dbo].[Player]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[Username] NVARCHAR(100) NOT NULL,
	[Password] NVARCHAR(MAX) NOT NULL
)