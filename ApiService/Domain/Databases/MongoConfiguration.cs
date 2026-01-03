using Common;
using Common.Messages;

namespace ApiService.Domain.Databases
{
    public class MongoConfiguration
    {
        public string? DBUser { get; set; }
        public string? DBUrl { get; set; }
        public string? DBName { get; set; }
        public int DBPort { get; set; }
        public string? DBPassword { get; set; }

        public bool IsValid()
        {
            return !(string.IsNullOrEmpty(DBUser) && string.IsNullOrEmpty(DBName) && string.IsNullOrEmpty(DBPassword) &&
                string.IsNullOrEmpty(DBUrl));
        }
        public void Validate()
        {
            if (string.IsNullOrEmpty(DBUrl)) throw new SException(Messages.InvalidServer);
            if (string.IsNullOrEmpty(DBName)) throw new SException(Messages.InvalidDatabaseName);

            if (!string.IsNullOrEmpty(DBUser) || !string.IsNullOrEmpty(DBPassword))
            {
                if (string.IsNullOrEmpty(DBUser))
                    throw new SException(Messages.InvalidUser);
                if (string.IsNullOrEmpty(DBPassword))
                    throw new SException(Messages.InvalidPassword);
            }
        }
    }
}
