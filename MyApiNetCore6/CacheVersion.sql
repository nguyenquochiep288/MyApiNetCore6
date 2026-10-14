IF OBJECT_ID(N'dbo.web_CacheVersion', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.web_CacheVersion
    (
        [ID] tinyint NOT NULL CONSTRAINT PK_web_CacheVersion PRIMARY KEY,
        [Version] rowversion,
        CONSTRAINT CK_web_CacheVersion_SingleRow CHECK ([ID] = 1)
    );

END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.web_CacheVersion WHERE [ID] = 1)
    INSERT INTO dbo.web_CacheVersion ([ID]) VALUES (1);
GO

CREATE OR ALTER TRIGGER dbo.trg_web_Menu_CacheVersion
ON dbo.web_Menu
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM inserted) OR EXISTS (SELECT 1 FROM deleted)
        UPDATE dbo.web_CacheVersion SET [ID] = [ID] WHERE [ID] = 1;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_web_Quyen_CacheVersion
ON dbo.web_Quyen
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM inserted) OR EXISTS (SELECT 1 FROM deleted)
        UPDATE dbo.web_CacheVersion SET [ID] = [ID] WHERE [ID] = 1;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_web_NhomQuyen_CacheVersion
ON dbo.web_NhomQuyen
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM inserted) OR EXISTS (SELECT 1 FROM deleted)
        UPDATE dbo.web_CacheVersion SET [ID] = [ID] WHERE [ID] = 1;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_web_PhanQuyen_CacheVersion
ON dbo.web_PhanQuyen
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM inserted) OR EXISTS (SELECT 1 FROM deleted)
        UPDATE dbo.web_CacheVersion SET [ID] = [ID] WHERE [ID] = 1;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_web_NoteClass_CacheVersion
ON dbo.web_NoteClass
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM inserted) OR EXISTS (SELECT 1 FROM deleted)
        UPDATE dbo.web_CacheVersion SET [ID] = [ID] WHERE [ID] = 1;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_web_NoteTable_CacheVersion
ON dbo.web_NoteTable
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM inserted) OR EXISTS (SELECT 1 FROM deleted)
        UPDATE dbo.web_CacheVersion SET [ID] = [ID] WHERE [ID] = 1;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_web_NoteType_CacheVersion
ON dbo.web_NoteType
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM inserted) OR EXISTS (SELECT 1 FROM deleted)
        UPDATE dbo.web_CacheVersion SET [ID] = [ID] WHERE [ID] = 1;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_web_ThongBao_CacheVersion
ON dbo.web_ThongBao
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM inserted) OR EXISTS (SELECT 1 FROM deleted)
        UPDATE dbo.web_CacheVersion SET [ID] = [ID] WHERE [ID] = 1;
END;
GO
