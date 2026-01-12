namespace ApiService.Domain.Entities
{
    public class PostgresEntity
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public DateTime UpdateDate { get; set; }
        public void GenerateId()
        {
            if (Id == Guid.Empty) Id = Guid.NewGuid();
        }
        public virtual void Validate() { }
    }
}
