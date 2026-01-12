namespace ApiService.Domain.Entities.Auhtorizations.AuthorizationGroups
{
    public class AuthorizationGroup : PostgresEntity
    {
        public string Name { get; set; }
        public bool IsSystem { get; set; }
        public List<AuthorizationGroupRole> Roles { get; set; } = new List<AuthorizationGroupRole>();
    }
    public class AuthorizationGroupRole : PostgresEntity
    {
        public string Name { get; set; }
        public string Role { get; set; }

        public Guid AuthorizationGroupId { get; set; }
        public AuthorizationGroup AuthorizationGroup { get; set; }
    }
}
