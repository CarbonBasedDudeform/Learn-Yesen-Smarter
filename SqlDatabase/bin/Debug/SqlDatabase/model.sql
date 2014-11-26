CREATE TABLE [dbo].[Category] (
    [CategoryID] INT           NOT NULL PRIMARY KEY CLUSTERED ([CategoryID] ASC),
    [Category]   NVARCHAR (50) NULL
);

GO
CREATE TABLE [dbo].[Questions] (
    [QuestionID] INT            NOT NULL PRIMARY KEY CLUSTERED ([QuestionID] ASC),
    [Questions]  NVARCHAR (140) NULL,
    [CategoryID] INT            NOT NULL
);

GO
CREATE TABLE [dbo].[Review] (
    [UserID]     INT      NOT NULL PRIMARY KEY CLUSTERED ([UserID] ASC),
    [QuestionID] INT      NOT NULL,
    [CategoryID] INT      NOT NULL,
    [LastTook]   DATETIME NULL,
    [Priority]   INT      NULL
);

GO
