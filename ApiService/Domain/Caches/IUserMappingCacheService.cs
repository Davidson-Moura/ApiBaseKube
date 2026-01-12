namespace ApiService.Domain.Services.Caches
{
    public interface IUserMappingCacheService
    {
        Task<bool> AddConnectionAsync(
            string connectionId,
            string userId,
            string tenantId,
            string machineKey);
        Task<long> RemoveConnectionAsync(string connectionId);
        Task<IEnumerable<string>> GetConnectionsByUserAsync(string userId);
        Task<(string UserId, string TenantId, string MachineId)?> GetConnectionInfoAsync(string connectionId);
        Task<List<string>> RemoveAndGetOtherConnectionId(string userId, string machineKey);
    }
}
