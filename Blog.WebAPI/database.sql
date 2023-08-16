CREATE DATABASE ToDoDatabase;
GO;
USE ToDoDatabase;
GO;
CREATE TABLE Tasks
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Description NVARCHAR(100) NOT NULL,
    IsCompleted BIT NOT NULL,
    CreatedAt DATETIME NOT NULL
);
GO;