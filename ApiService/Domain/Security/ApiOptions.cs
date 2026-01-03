namespace ApiService.Domain.Security
{
    public class ApiOptions
    {
        public string? IssuerSigningKey {  get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
    }
}
