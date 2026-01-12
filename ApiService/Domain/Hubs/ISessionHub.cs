namespace ApiService.Domain.Hubs
{
    public interface ISessionHub
    {
        Task UserSigned(string userId, string userName);
        Task UserUnsigned(string userId, string userName);
        Task ForcedDisconnect(string msg);
        //Task UserNotification(Notification notification);
    }
}
