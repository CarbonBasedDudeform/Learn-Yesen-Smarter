﻿CREATE TABLE [dbo].[Review]
(
	[UserID] INT NOT NULL PRIMARY KEY, 
    [QuestionID] INT NOT NULL, 
    [CategoryID] INT NOT NULL, 
    [LastTook] DATETIME NULL, 
    [Priority] INT NULL
)
