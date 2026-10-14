# Kế hoạch chuyển đổi đa nền tảng

## Mục tiêu đã thống nhất

- Backend chạy trên Windows Server.
- Frontend dùng được trên web và trình duyệt Android/iOS; ưu tiên giao diện responsive dạng PWA.
- Thay Crystal Reports bằng bộ mẫu báo cáo có thể triển khai không mất phí bản quyền runtime.
- Giữ ứng dụng hiện tại hoạt động trong thời gian chuyển đổi; thay từng phần để dễ đối chiếu.

## Hiện trạng đã kiểm tra

- API: ASP.NET Core trên .NET 6, nhưng đang tham chiếu `DatabaseTHP` nhắm .NET Framework 4.5.
- Web: ASP.NET MVC 5 trên .NET Framework 4.7.2.
- Cơ sở dữ liệu: SQL Server.
- Báo cáo: 29 mẫu Crystal Reports `.rpt` trong thư mục `MVC_QuanLyTHP/Ajax_Form_Post_MVC/Report`.
- Giao diện MVC hiện có khoảng 82 controller và 356 Razor view; cần chuyển theo module, không thể thay một bước mà giữ nguyên mọi hành vi.

## Hướng kỹ thuật

1. Tạo nhánh ứng dụng mới trên ASP.NET Core/.NET 10 LTS cho API và web; giữ SQL Server và chuyển các lớp dữ liệu/EF theo từng module.
2. Dùng giao diện responsive có thể cài dạng PWA để một frontend phục vụ trình duyệt desktop, Android và iOS.
3. Dùng FastReport Open Source làm bộ dựng báo cáo miễn phí theo giấy phép MIT. Không dùng Crystal runtime hay designer thương mại trong gói triển khai.
4. Dựng lại từng mẫu Crystal thành mẫu mới và so sánh nội dung, phân trang, tổng tiền, font và khổ giấy trước khi chuyển chức năng in sang bản mới.

## Các giai đoạn

1. Tạo bộ khung mới, cấu hình xác thực, logging và kết nối SQL Server.
2. Chuyển một luồng nghiệp vụ nhỏ từ API và web để xác nhận mô hình triển khai Windows Server/PWA.
3. Chuyển các module nghiệp vụ theo nhóm, giữ API cũ dùng được trong giai đoạn chuyển tiếp.
4. Chuyển báo cáo phiếu in từng mẫu, bắt đầu bằng mẫu phiếu nhập; chỉ bỏ mẫu Crystal tương ứng sau khi đối chiếu đạt.
5. Chốt hướng dẫn publish, cấu hình IIS và cài PWA trên Android/iOS.

## Lưu ý về phạm vi

- 29 mẫu `.rpt` cần được dựng lại; không giả định có thể chuyển đổi Crystal sang định dạng mới chính xác tự động.
- FastReport Open Source hỗ trợ xuất HTML và có PDF exporter đơn giản; kiểm tra kỹ các mẫu cần tính năng PDF nâng cao.
- PWA chạy từ browser trên điện thoại. Nếu cần app riêng phát hành trên App Store/Google Play, đó là một frontend native bổ sung.

## Bảo toàn công việc hiện có

Trước khi sửa mã nguồn, đã ghi nhận nhiều thay đổi chưa commit trong hai repository API và MVC. Kế hoạch là thêm ứng dụng mới song song và giữ nguyên các thay đổi hiện tại, không ghi đè chúng.
