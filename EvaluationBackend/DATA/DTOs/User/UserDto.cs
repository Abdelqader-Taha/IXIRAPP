namespace EvaluationBackend.DATA.DTOs.User
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
        public int? StoreCount { get; set; }

        public DateTime? LastActive { get; set; } 

    }
}