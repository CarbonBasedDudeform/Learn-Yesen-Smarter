CREATE TABLE [dbo].[Questions]
(
	[QuestionID] INT NOT NULL PRIMARY KEY, 
    [Question] NVARCHAR(140) NULL, 
    [CategoryID] INT NOT NULL, 
    [QuestionType] INT NULL
)
