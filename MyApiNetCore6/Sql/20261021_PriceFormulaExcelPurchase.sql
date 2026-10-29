-- Run after 20261015_PriceFormula.sql and 20261020_PriceFormulaHistoryReport.sql,
-- before deploying the API that supports purchase prices from Excel.
SET XACT_ABORT ON;
IF OBJECT_ID(N'dbo.dm_CongThucGia_LichSuApDung', N'U') IS NULL
    THROW 50001, N'Run 20261015_PriceFormula.sql first.', 1;

BEGIN TRANSACTION;
IF COL_LENGTH(N'dbo.dm_CongThucGia_LichSuApDung', N'NGUON_GIA_NHAP') IS NULL
BEGIN
    ALTER TABLE dbo.dm_CongThucGia_LichSuApDung
        ADD NGUON_GIA_NHAP nvarchar(20) NOT NULL
        CONSTRAINT DF_CongThucGia_LichSu_NguonGia DEFAULT N'PHIEUNHAP' WITH VALUES;
END;
-- Excel prices have no associated receipt date. Preserve existing receipt dates.
ALTER TABLE dbo.dm_CongThucGia_LichSuApDung
    ALTER COLUMN NGAYNHAP_GANNHAT datetime2(0) NULL;
COMMIT TRANSACTION;
