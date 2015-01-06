CREATE TABLE [dbo].[Questions]
(
	[QuestionID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Question] NVARCHAR(140) NULL, 
    [CategoryID] INT NULL, 
    [QuestionType] INT NULL
)
