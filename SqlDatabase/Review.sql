CREATE TABLE [dbo].[Review]
(
	[UserID] INT NOT NULL , 
    [QuestionID] INT NOT NULL, 
    [CategoryID] INT NULL, 
    [LastTook] DATETIME NULL, 
    [Priority] FLOAT NULL, 
    PRIMARY KEY ([QuestionID])
)
