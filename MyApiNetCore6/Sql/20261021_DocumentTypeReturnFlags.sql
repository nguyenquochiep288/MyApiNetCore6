-- Run on the application database before deploying DatabaseTHP.dll / API / MVC.
-- Existing types default to 0. Mark the actual return types in TypeInput/TypeOutput after deployment.
SET XACT_ABORT ON;
BEGIN TRY
    BEGIN TRANSACTION;
    IF OBJECT_ID(N'dbo.dm_LoaiPhieuXuat', N'U') IS NULL OR OBJECT_ID(N'dbo.dm_LoaiPhieuNhap', N'U') IS NULL
        THROW 50001, N'Khong tim thay bang loai phieu nhap/xuat. Hay chon dung database.', 1;

    IF COL_LENGTH(N'dbo.dm_LoaiPhieuXuat', N'ISTRAHANG') IS NULL
        ALTER TABLE dbo.dm_LoaiPhieuXuat ADD ISTRAHANG bit NOT NULL
            CONSTRAINT DF_dm_LoaiPhieuXuat_ISTRAHANG DEFAULT (0) WITH VALUES;
    ELSE
    BEGIN
        IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.dm_LoaiPhieuXuat') AND name = N'ISTRAHANG' AND system_type_id <> 104)
            THROW 50002, N'Cot ISTRAHANG da ton tai nhung khong phai kieu bit.', 1;
        EXEC(N'UPDATE dbo.dm_LoaiPhieuXuat SET ISTRAHANG = 0 WHERE ISTRAHANG IS NULL;');
        EXEC(N'ALTER TABLE dbo.dm_LoaiPhieuXuat ALTER COLUMN ISTRAHANG bit NOT NULL;');
        IF NOT EXISTS (SELECT 1 FROM sys.default_constraints d JOIN sys.columns c ON c.object_id = d.parent_object_id AND c.column_id = d.parent_column_id WHERE c.object_id = OBJECT_ID(N'dbo.dm_LoaiPhieuXuat') AND c.name = N'ISTRAHANG')
            EXEC(N'ALTER TABLE dbo.dm_LoaiPhieuXuat ADD CONSTRAINT DF_dm_LoaiPhieuXuat_ISTRAHANG DEFAULT (0) FOR ISTRAHANG;');
    END;

    IF COL_LENGTH(N'dbo.dm_LoaiPhieuNhap', N'ISNHAPTRAHANG') IS NULL
        ALTER TABLE dbo.dm_LoaiPhieuNhap ADD ISNHAPTRAHANG bit NOT NULL
            CONSTRAINT DF_dm_LoaiPhieuNhap_ISNHAPTRAHANG DEFAULT (0) WITH VALUES;
    ELSE
    BEGIN
        IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.dm_LoaiPhieuNhap') AND name = N'ISNHAPTRAHANG' AND system_type_id <> 104)
            THROW 50003, N'Cot ISNHAPTRAHANG da ton tai nhung khong phai kieu bit.', 1;
        EXEC(N'UPDATE dbo.dm_LoaiPhieuNhap SET ISNHAPTRAHANG = 0 WHERE ISNHAPTRAHANG IS NULL;');
        EXEC(N'ALTER TABLE dbo.dm_LoaiPhieuNhap ALTER COLUMN ISNHAPTRAHANG bit NOT NULL;');
        IF NOT EXISTS (SELECT 1 FROM sys.default_constraints d JOIN sys.columns c ON c.object_id = d.parent_object_id AND c.column_id = d.parent_column_id WHERE c.object_id = OBJECT_ID(N'dbo.dm_LoaiPhieuNhap') AND c.name = N'ISNHAPTRAHANG')
            EXEC(N'ALTER TABLE dbo.dm_LoaiPhieuNhap ADD CONSTRAINT DF_dm_LoaiPhieuNhap_ISNHAPTRAHANG DEFAULT (0) FOR ISNHAPTRAHANG;');
    END;
    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
