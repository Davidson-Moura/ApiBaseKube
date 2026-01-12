using ApiService.Domain.Services.Caches;
using StackExchange.Redis;

namespace ApiService.Services.Caches
{
    public class UserMappingCacheService : IUserMappingCacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private IDatabase Db => _redis.GetDatabase();

        public UserMappingCacheService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }
        public async Task<bool> AddConnectionAsync(
            string connectionId, 
            string userId, 
            string tenantId,
            string machineKey)
        {
            var hashKey = ConnectionHash(connectionId);

            //var existOtherMachineByUser = await ExistConnectionByDiffMachineAsync(userId, machineKey);

            await Db.HashSetAsync(hashKey, new HashEntry[]
            {
                new(PropInfos.UserId, userId),
                new(PropInfos.TenantId, tenantId),
                new(PropInfos.MachineId, machineKey),
            });

            await Db.SetAddAsync(UserHash(userId), connectionId);

            await Db.KeyExpireAsync(hashKey, TimeSpan.FromHours(6));
            return false;
        }
        public async Task<long> RemoveConnectionAsync(string connectionId)
        {
            var hashKey = ConnectionHash(connectionId);

            var hash = await Db.HashGetAllAsync(hashKey);
            if (hash.Length == 0) return 0;

            var userId = hash.FirstOrDefault(x => x.Name == PropInfos.UserId).Value.ToString();
            var enterpriseId = hash.FirstOrDefault(x => x.Name == PropInfos.TenantId).Value.ToString();

            await Db.KeyDeleteAsync(hashKey);
            await Db.SetRemoveAsync(UserHash(userId), connectionId);

            var remaining = await Db.SetLengthAsync(UserHash(userId));
            if (remaining <= 0)
            {
                await Db.KeyDeleteAsync(UserHash(userId));
            }

            return remaining;
        }
        public async Task<IEnumerable<string>> GetConnectionsByUserAsync(string userId)
        {
            var conns = await Db.SetMembersAsync(UserHash(userId));
            return conns.Select(x => x.ToString());
        }
        public async Task<(string UserId, string TenantId, string MachineId)?> GetConnectionInfoAsync(string connectionId)
        {
            var hash = await Db.HashGetAllAsync(ConnectionHash(connectionId));
            if (hash.Length == 0) return null;

            var userId = hash.FirstOrDefault(x => x.Name == PropInfos.UserId).Value.ToString();
            var enterpriseId = hash.FirstOrDefault(x => x.Name == PropInfos.TenantId).Value.ToString();
            var machineId = hash.FirstOrDefault(x => x.Name == PropInfos.MachineId).Value.ToString();

            return (userId, enterpriseId, machineId);
        }
        public async Task<List<string>> RemoveAndGetOtherConnectionId(string userId, string machineKey)
        {
            var connectionIds = await Db.SetMembersAsync(UserHash(userId));

            var list = new List<string>();
            foreach (var conn in connectionIds)
            {
                var connId = conn.ToString();
                var hashKey = ConnectionHash(connId);
                var machineId = (await Db.HashGetAsync(hashKey, PropInfos.MachineId)).ToString();
                if (machineId != machineKey)
                {
                    list.Add(machineId);
                    _ = RemoveConnectionAsync(connId);
                }
            }
            return list;
        }
        private string ConnectionHash(string connectionId) => $"session-conn:{connectionId}";
        private string UserHash(string userId) => $"session-user:{userId}";

        private struct PropInfos
        {
            public const string UserId = "UserId";
            public const string TenantId = "TenantId";
            public const string MachineId = "MachineId";
        }
    }
}
