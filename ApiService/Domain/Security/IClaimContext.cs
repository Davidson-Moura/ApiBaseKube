namespace ApiService.Domain.Security
{
    public interface IClaimContext
    {
        bool IsAuthenticated { get; }
        string UserId { get; }
        string LoginName { get; }
        string Email { get; }
        Guid TenantId { get; }
        Guid? AuthorizationGroupId { get; }
        string GetMachineIdentifier();
        string CurrentHttpConnectionId { get; }
        string RemoteIpAddress { get; }
        void SetClaim();
    }
}
