-- Run in the application database after 20261015_PriceFormula.sql,
-- BEFORE deploying the API containing the history report.
-- Existing rows stay NULL: historical restores cannot be reconstructed reliably.
SET XACT_ABORT ON;
IF OBJECT_ID(N'dbo.dm_CongThucGia_LichSuApDung', N'U') IS NULL
BEGIN
    THROW 50001, N'Chạy 20261015_PriceFormula.sql trước khi bổ sung lịch sử khôi phục.', 1;
END;
BEGIN TRANSACTION;
IF COL_LENGTH(N'dbo.dm_CongThucGia_LichSuApDung', N'DA_KHOIPHUC') IS NULL
    ALTER TABLE dbo.dm_CongThucGia_LichSuApDung ADD DA_KHOIPHUC bit NULL;
IF COL_LENGTH(N'dbo.dm_CongThucGia_LichSuApDung', N'THOIGIAN_KHOIPHUC') IS NULL
    ALTER TABLE dbo.dm_CongThucGia_LichSuApDung ADD THOIGIAN_KHOIPHUC datetime2(0) NULL;
IF COL_LENGTH(N'dbo.dm_CongThucGia_LichSuApDung', N'ID_NGUOI_KHOIPHUC') IS NULL
    ALTER TABLE dbo.dm_CongThucGia_LichSuApDung ADD ID_NGUOI_KHOIPHUC nvarchar(50) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.dm_CongThucGia_LichSuApDung') AND name = N'IX_CongThucGia_LichSu_ThoiGian')
    CREATE INDEX IX_CongThucGia_LichSu_ThoiGian ON dbo.dm_CongThucGia_LichSuApDung(LOC_ID, THOIGIANTHEM DESC, ID);
COMMIT;
