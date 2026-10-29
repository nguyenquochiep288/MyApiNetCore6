# Triển khai báo cáo lịch sử áp dụng công thức giá

1. Chọn đúng database của API trong SQL Server Management Studio.
2. Nếu chưa có bảng `dbo.dm_CongThucGia_LichSuApDung`, chạy `20261015_PriceFormula.sql`.
3. Chạy `20261020_PriceFormulaHistoryReport.sql` trước khi cập nhật API. Script có thể chạy lại; bổ sung ba cột nhật ký khôi phục và chỉ mục, giữ nguyên dữ liệu lịch sử cũ.
4. Build/publish API và MVC từ mã nguồn đã cập nhật, khởi động lại API.
5. Mở `/PriceFormula/History`, chọn khoảng ngày rồi bấm **Xem báo cáo**. Có nút mở báo cáo tại trang danh sách công thức và trang tính giá.

## Nội dung

- Lọc theo thời gian áp dụng, loại giá, mã/tên hàng, mã công thức, tài khoản áp dụng và trạng thái khôi phục.
- Giá cũ/mới, tỷ lệ quy đổi, giá quy đổi cũ/mới, chênh lệch, giá nhập tham chiếu, ngày nhập, mã công thức và tài khoản đều lấy từ lịch sử đã lưu.
- Phân trang 100 dòng. Excel lấy toàn bộ bộ lọc của kết quả đang hiển thị, tối đa 10.000 dòng; vượt giới hạn cần thu hẹp bộ lọc.
- Quyền truy cập MVC: `PriceFormula/View`. Chi nhánh lấy từ phiên đăng nhập.

## Trạng thái khôi phục

- Dữ liệu cũ giữ `DA_KHOIPHUC = NULL`: **Chưa có nhật ký khôi phục**.
- Lần áp dụng mới ghi `DA_KHOIPHUC = 0`: **Chưa khôi phục**.
- Khôi phục thành công ghi `DA_KHOIPHUC = 1`, thời gian và tài khoản khôi phục trong cùng giao dịch cập nhật giá.
- “Chưa khôi phục” không xác nhận giá hiện tại còn bằng giá mới, cũng không xác nhận có thể khôi phục dòng đó.
- Báo cáo chỉ đọc dữ liệu; thao tác khôi phục vẫn ở chức năng tính/áp dụng giá hiện có.

## Trạng thái bàn giao mã nguồn

API và MVC đã build thành công. Chưa thực thi script trên SQL Server, chưa publish API mới và chưa kiểm tra dữ liệu báo cáo trên database thực tế.
