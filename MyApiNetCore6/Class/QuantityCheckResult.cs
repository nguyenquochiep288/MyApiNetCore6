namespace API_QuanLyTHP.Class
{
    public class QuantityCheckResult
    {
        public int ID_SANPHAM { get; set; }
        public int TotalOrdered { get; set; }
        public int TotalReceived { get; set; }
        //Status = 0 không lỗi,Status = 1 là lỗi
        public int Status { get; set; }
    }

}
