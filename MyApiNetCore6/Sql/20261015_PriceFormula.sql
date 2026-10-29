/* Configurable purchase/sale price formulas. Run once against the application database. */
IF OBJECT_ID(N'dbo.dm_CongThucGia', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.dm_CongThucGia
    (
        LOC_ID              nvarchar(50)  NOT NULL,
        ID                  nvarchar(50)  NOT NULL,
        MA                  nvarchar(50)  NOT NULL,
        NAME                nvarchar(200) NOT NULL,
        LOAIGIA             nvarchar(20)  NOT NULL CONSTRAINT DF_dm_CongThucGia_LOAIGIA DEFAULT N'GIA01',
        CONGTHUC            nvarchar(1000) NOT NULL,
        TINHTHUE            bit NOT NULL CONSTRAINT DF_dm_CongThucGia_TINHTHUE DEFAULT (0),
        TUNGAY              datetime2(0) NOT NULL,
        DENNGAY             datetime2(0) NULL,
        ISACTIVE            bit NOT NULL CONSTRAINT DF_dm_CongThucGia_ISACTIVE DEFAULT (1),
        THOIGIANSUA         datetime2(0) NULL,
        ID_NGUOISUA         nvarchar(50) NULL,
        THOIGIANTHEM        datetime2(0) NOT NULL CONSTRAINT DF_dm_CongThucGia_THOIGIANTHEM DEFAULT (GETDATE()),
        ID_NGUOITAO         nvarchar(50) NULL,
        CONSTRAINT PK_dm_CongThucGia PRIMARY KEY (LOC_ID, ID),
        CONSTRAINT CK_dm_CongThucGia_LOAIGIA CHECK (LOAIGIA IN (N'GIAMUA', N'GIA01', N'GIA02', N'GIA03')),
        CONSTRAINT CK_dm_CongThucGia_NGAY CHECK (DENNGAY IS NULL OR DENNGAY >= TUNGAY)
    );
    CREATE UNIQUE INDEX UX_dm_CongThucGia_LOC_MA ON dbo.dm_CongThucGia(LOC_ID, MA);
    CREATE INDEX IX_dm_CongThucGia_Loai_Ngay ON dbo.dm_CongThucGia(LOC_ID, LOAIGIA, ISACTIVE, TUNGAY, DENNGAY);
END;
GO

IF OBJECT_ID(N'dbo.dm_CongThucGia_ThanhPhan', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.dm_CongThucGia_ThanhPhan
    (
        LOC_ID              nvarchar(50)  NOT NULL,
        ID                  nvarchar(50)  NOT NULL,
        ID_CONGTHUCGIA      nvarchar(50)  NOT NULL,
        MA                  nvarchar(40)  NOT NULL,
        TEN                 nvarchar(100) NOT NULL,
        KIEUGIATRI          nvarchar(20)  NOT NULL CONSTRAINT DF_dm_CongThucGia_TP_KIEU DEFAULT N'TIEN',
        GIATRI_MACDINH      decimal(18,4) NOT NULL CONSTRAINT DF_dm_CongThucGia_TP_GIATRI DEFAULT (0),
        STT                 int NOT NULL CONSTRAINT DF_dm_CongThucGia_TP_STT DEFAULT (0),
        CONSTRAINT PK_dm_CongThucGia_ThanhPhan PRIMARY KEY (LOC_ID, ID),
        CONSTRAINT FK_dm_CongThucGia_ThanhPhan_CongThucGia FOREIGN KEY (LOC_ID, ID_CONGTHUCGIA)
            REFERENCES dbo.dm_CongThucGia(LOC_ID, ID) ON DELETE CASCADE,
        CONSTRAINT CK_dm_CongThucGia_ThanhPhan_Kieu CHECK (KIEUGIATRI IN (N'TIEN', N'PHANTRAM'))
    );
    CREATE UNIQUE INDEX UX_dm_CongThucGia_ThanhPhan_MA ON dbo.dm_CongThucGia_ThanhPhan(LOC_ID, ID_CONGTHUCGIA, MA);
END;
GO

IF OBJECT_ID(N'dbo.dm_CongThucGia_NhomHangHoa', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.dm_CongThucGia_NhomHangHoa
    (
        LOC_ID          nvarchar(50) NOT NULL,
        ID_CONGTHUCGIA  nvarchar(50) NOT NULL,
        ID_NHOMHANGHOA  nvarchar(50) NOT NULL,
        CONSTRAINT PK_dm_CongThucGia_NhomHangHoa PRIMARY KEY (LOC_ID, ID_CONGTHUCGIA, ID_NHOMHANGHOA),
        CONSTRAINT FK_dm_CongThucGia_NhomHangHoa_CongThucGia FOREIGN KEY (LOC_ID, ID_CONGTHUCGIA)
            REFERENCES dbo.dm_CongThucGia(LOC_ID, ID) ON DELETE CASCADE
    );
    CREATE INDEX IX_dm_CongThucGia_NhomHangHoa_Group ON dbo.dm_CongThucGia_NhomHangHoa(LOC_ID, ID_NHOMHANGHOA, ID_CONGTHUCGIA);
END;
GO

/* Backfill one-group rules created by the first version of this feature. */
IF COL_LENGTH(N'dbo.dm_CongThucGia', N'ID_NHOMHANGHOA') IS NOT NULL
BEGIN
    EXEC sys.sp_executesql N'
        INSERT dbo.dm_CongThucGia_NhomHangHoa (LOC_ID, ID_CONGTHUCGIA, ID_NHOMHANGHOA)
        SELECT f.LOC_ID, f.ID, f.ID_NHOMHANGHOA
        FROM dbo.dm_CongThucGia f
        WHERE NOT EXISTS
        (
            SELECT 1 FROM dbo.dm_CongThucGia_NhomHangHoa g
            WHERE g.LOC_ID = f.LOC_ID AND g.ID_CONGTHUCGIA = f.ID
              AND g.ID_NHOMHANGHOA = f.ID_NHOMHANGHOA
        );';
END;
GO

/* Add the page beside product groups; then grant its CRUD permissions in the
   existing permission screen. */
IF OBJECT_ID(N'dbo.web_Menu', N'U') IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM dbo.web_Menu WHERE CONTROLLERNAME = N'PriceFormula')
BEGIN
    INSERT dbo.web_Menu (ID, NAME, ACTIONNAME, CONTROLLERNAME, ID_QUYENCHA, ICON, STT, ISACTIVE, THOIGIANSUA, ID_NGUOISUA, THOIGIANTHEM, ID_NGUOITAO)
    SELECT TOP (1) CONVERT(nvarchar(50), NEWID()), N'Công thức giá', N'Index', N'PriceFormula', template.ID_QUYENCHA,
        N'fa fa-calculator', ISNULL(template.STT, 0) + 1, 1, template.THOIGIANSUA, template.ID_NGUOISUA,
        template.THOIGIANTHEM, template.ID_NGUOITAO
    FROM dbo.web_Menu template WHERE template.CONTROLLERNAME = N'GroupProduct';
END;
GO

IF OBJECT_ID(N'dbo.dm_CongThucGia_LichSuApDung', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.dm_CongThucGia_LichSuApDung
    (
        LOC_ID              nvarchar(50) NOT NULL,
        ID                  nvarchar(50) NOT NULL,
        ID_HANGHOA          nvarchar(50) NOT NULL,
        MAHANG              nvarchar(50) NOT NULL,
        TENHANG             nvarchar(200) NOT NULL,
        LOAIGIA             nvarchar(20) NOT NULL,
        GIACU               decimal(18,4) NOT NULL,
        TYLE_QD             decimal(18,4) NULL,
        GIA_QD_CU           decimal(18,4) NULL,
        GIAMUA_GANNHAT      decimal(18,4) NOT NULL,
        NGAYNHAP_GANNHAT    datetime2(0) NOT NULL,
        GIA_MOI             decimal(18,4) NOT NULL,
        GIA_QD_MOI          decimal(18,4) NULL,
        ID_CONGTHUCGIA      nvarchar(50) NOT NULL,
        MA_CONGTHUC         nvarchar(50) NOT NULL,
        ID_NGUOITAO         nvarchar(50) NULL,
        THOIGIANTHEM        datetime2(0) NOT NULL CONSTRAINT DF_dm_CongThucGia_LichSu_Thoigian DEFAULT (GETDATE()),
        CONSTRAINT PK_dm_CongThucGia_LichSuApDung PRIMARY KEY (LOC_ID, ID)
    );
    CREATE INDEX IX_dm_CongThucGia_LichSuApDung_HangHoa ON dbo.dm_CongThucGia_LichSuApDung(LOC_ID, ID_HANGHOA, THOIGIANTHEM DESC);
END;
GO

IF OBJECT_ID(N'dbo.dm_CongThucGia_LichSuApDung', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'dbo.dm_CongThucGia_LichSuApDung', N'TYLE_QD') IS NULL
        ALTER TABLE dbo.dm_CongThucGia_LichSuApDung ADD TYLE_QD decimal(18,4) NULL;
    IF COL_LENGTH(N'dbo.dm_CongThucGia_LichSuApDung', N'GIA_QD_CU') IS NULL
        ALTER TABLE dbo.dm_CongThucGia_LichSuApDung ADD GIA_QD_CU decimal(18,4) NULL;
    IF COL_LENGTH(N'dbo.dm_CongThucGia_LichSuApDung', N'GIA_QD_MOI') IS NULL
        ALTER TABLE dbo.dm_CongThucGia_LichSuApDung ADD GIA_QD_MOI decimal(18,4) NULL;
END;
GO
