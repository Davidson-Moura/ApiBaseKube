namespace ApiService.Models.Users
{
    public class UserLoginResponseModel
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
