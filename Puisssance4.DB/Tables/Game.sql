﻿CREATE TABLE [dbo].Game
(
	[Id] UNIQUEIDENTIFIER PRIMARY KEY,
	[RedPlayerId] INT REFERENCES Player,
	[YellowPlayerId] INT REFERENCES Player,
	[SerializedGrid] VARCHAR(MAX) NOT NULL,
	VersusAI BIT NOT NULL
)
