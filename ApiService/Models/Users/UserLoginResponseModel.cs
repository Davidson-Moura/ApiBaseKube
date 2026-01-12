namespace ApiService.Models.Users
{
    public class UserLoginResponseModel
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string Token { get; set; }
        public bool IsAdmin { get; set; } = false;
        public DateTime ExpiresAt { get; set; }
    }
}
