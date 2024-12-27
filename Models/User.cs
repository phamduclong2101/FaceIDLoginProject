namespace FaceIDLoginProject.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } // Tên người dùng
        public string FaceEncoding { get; set; } // Lưu mã hóa khuôn mặt (JSON hoặc chuỗi CSV)
    }
}

