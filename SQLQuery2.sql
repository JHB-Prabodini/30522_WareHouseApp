DROP TABLE IF EXISTS [dbo].[Employees];

CREATE TABLE [dbo].[Employees] (
    [EmployeeId] INT             IDENTITY (1, 1) NOT NULL PRIMARY KEY,
    [Name]       VARCHAR (100)   NOT NULL,
    [Role]       VARCHAR (50)    NOT NULL,
    [Department] VARCHAR (100)   NULL,
    [Contact]    VARCHAR (20)    NULL,
    [Salary]     DECIMAL (18, 2) NOT NULL,
    [Status]     VARCHAR (50)    NULL
);