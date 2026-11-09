-- Estimate the unit cost only for requested returns, replaying each affected stock
-- from its opening balance in the same order as TinhGiaVonHangKho (date added).
SELECT STOCK,MAX(STAMP) AS LASTDAY INTO #EstimateScope FROM #ReturnTargets GROUP BY STOCK;
CREATE TABLE #ReplayCosts (ID nvarchar(50) PRIMARY KEY, COST decimal(28,10));

DECLARE @ecStock nvarchar(50), @ecLastDay date, @ecStart date,
    @ecBalance decimal(28,6), @ecAverage decimal(28,10), @ecHasCost bit,
    @ecId nvarchar(50), @ecKind int, @ecQty decimal(28,6), @ecNet decimal(28,6),
    @ecOriginal nvarchar(50), @ecReturnCost decimal(28,10), @ecStamp date;
DECLARE estimateStocks CURSOR LOCAL FAST_FORWARD FOR
    SELECT s.STOCK,s.LASTDAY,o.TUNGAY,o.SOLUONG,o.DONGIA
    FROM #EstimateScope s JOIN dbo.ct_GiaVonDauKy o ON o.LOC_ID=@Branch AND o.ID_HANGHOAKHO=s.STOCK
    WHERE o.TUNGAY<=s.LASTDAY AND o.SOLUONG>=0 AND o.DONGIA>=0;
OPEN estimateStocks;
FETCH NEXT FROM estimateStocks INTO @ecStock,@ecLastDay,@ecStart,@ecBalance,@ecAverage;
WHILE @@FETCH_STATUS=0
BEGIN
    SET @ecHasCost=CASE WHEN @ecBalance>0 OR @ecAverage>0 THEN 1 ELSE 0 END;
    DELETE FROM #ReplayCosts;
    DECLARE estimateMoves CURSOR LOCAL FAST_FORWARD FOR
        SELECT ID,Kind,Qty,Net,Original,Stamp FROM (
            SELECT d.ID,CASE WHEN t.ISNHAPTRAHANG=1 THEN 1 ELSE 0 END AS Kind,
                CASE WHEN d.SOLUONG>0 AND d.TYLE_QD>0 THEN TRY_CONVERT(decimal(28,6),d.SOLUONG*d.TYLE_QD) END AS Qty,
                TRY_CONVERT(decimal(28,6),d.THANHTIEN) AS Net,d.ID_PHIEUXUAT_CHITIET AS Original,
                CONVERT(date,h.THOIGIANTHEM) AS Stamp,h.SOPHIEU AS Seq
            FROM dbo.ct_PhieuNhap_ChiTiet d
            LEFT JOIN dbo.ct_PhieuNhap h ON h.LOC_ID=d.LOC_ID AND h.ID=d.ID_PHIEUNHAP
            LEFT JOIN dbo.dm_LoaiPhieuNhap t ON t.LOC_ID=h.LOC_ID AND t.ID=h.ID_LOAIPHIEUNHAP
            WHERE d.LOC_ID=@Branch AND d.ID_HANGHOAKHO=@ecStock AND (d.SOLUONG IS NULL OR d.SOLUONG<>0)
                AND (h.THOIGIANTHEM IS NULL OR (h.THOIGIANTHEM>=@ecStart AND h.THOIGIANTHEM<DATEADD(day,1,@ecLastDay)))
            UNION ALL
            SELECT d.ID,2,CASE WHEN d.SOLUONG>0 AND d.TYLE_QD>0 THEN TRY_CONVERT(decimal(28,6),d.SOLUONG*d.TYLE_QD) END,
                0,NULL,CONVERT(date,h.THOIGIANTHEM),h.SOPHIEU
            FROM dbo.ct_PhieuXuat_ChiTiet d
            LEFT JOIN dbo.ct_PhieuXuat h ON h.LOC_ID=d.LOC_ID AND h.ID=d.ID_PHIEUXUAT
            WHERE d.LOC_ID=@Branch AND d.ID_HANGHOAKHO=@ecStock AND (d.SOLUONG IS NULL OR d.SOLUONG<>0)
                AND (h.THOIGIANTHEM IS NULL OR (h.THOIGIANTHEM>=@ecStart AND h.THOIGIANTHEM<DATEADD(day,1,@ecLastDay)))
        ) m ORDER BY Stamp,Kind,Seq,ID;
    OPEN estimateMoves;
    FETCH NEXT FROM estimateMoves INTO @ecId,@ecKind,@ecQty,@ecNet,@ecOriginal,@ecStamp;
    WHILE @@FETCH_STATUS=0
    BEGIN
        -- A broken history prevents estimates from this point onward for this stock.
        IF @ecStamp IS NULL OR @ecQty IS NULL OR @ecQty<=0 BREAK;
        IF @ecKind=2
        BEGIN
            IF @ecQty>@ecBalance BREAK;
            INSERT #ReplayCosts VALUES (@ecId,@ecAverage);
            SET @ecBalance-=@ecQty;
        END
        ELSE
        BEGIN
            IF @ecKind=1
            BEGIN
                SET @ecReturnCost=COALESCE((SELECT COST FROM #ReplayCosts WHERE ID=@ecOriginal),
                    CASE WHEN @ecHasCost=1 THEN @ecAverage END);
                IF @ecReturnCost IS NULL BREAK;
                SET @ecNet=TRY_CONVERT(decimal(28,6),@ecReturnCost*@ecQty);
                IF @ecNet IS NOT NULL AND @ecHasCost=1
                    UPDATE #ReturnTargets SET UNITCOST=@ecAverage WHERE STOCK=@ecStock AND DETAILID=@ecId;
            END;
            IF @ecNet IS NULL OR @ecNet<0 BREAK;
            SET @ecAverage=TRY_CONVERT(decimal(28,10),
                TRY_CONVERT(decimal(28,8),@ecBalance*@ecAverage+@ecNet) / NULLIF(TRY_CONVERT(decimal(20,6),@ecBalance+@ecQty),0));
            IF @ecAverage IS NULL BREAK;
            SET @ecBalance+=@ecQty;
            SET @ecHasCost=1;
        END;
        FETCH NEXT FROM estimateMoves INTO @ecId,@ecKind,@ecQty,@ecNet,@ecOriginal,@ecStamp;
    END;
    CLOSE estimateMoves;
    DEALLOCATE estimateMoves;
    FETCH NEXT FROM estimateStocks INTO @ecStock,@ecLastDay,@ecStart,@ecBalance,@ecAverage;
END;
CLOSE estimateStocks;
DEALLOCATE estimateStocks;
DROP TABLE #ReplayCosts;
DROP TABLE #EstimateScope;
