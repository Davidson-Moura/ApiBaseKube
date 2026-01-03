using ApiService.Definitions;
using Common;
using Common.Messages;
using System.Text.Json.Serialization;

namespace ApiService.Domain.Entities.Users
{
    public class User : PostegresEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        [JsonIgnore]
        public string Password { get => _password; set => _password = value; }
        private string _password;
        public void SetPassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password == DefaultValues.DefaultHiddenPassword) throw new SException(Messages.UninformedPassword);
            _password = Cryptography.HashPassword(password);
        }
        public bool ValidatePassword(string password)
        {
            return Cryptography.VerifyPassword(password, _password);
        }
        public override void Validate()
        {
            if (string.IsNullOrEmpty(Name)) throw new SException(Messages.UninformedName);
            if (string.IsNullOrEmpty(Email)) throw new SException(Messages.UninformedEmail);
            if (string.IsNullOrEmpty(_password) || _password == DefaultValues.DefaultHiddenPassword) throw new SException(Messages.UninformedPassword);
        }
        public void HiddenPassword()
        {
            _password = DefaultValues.DefaultHiddenPassword;
        }
    }
    public class UserFilter : Infra.Entities.Generics.PGFilterBase<User>
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? EmailEq { get; set; }
        public override IQueryable<User> ApplyFilter(IQueryable<User> query)
        {
            if (!string.IsNullOrEmpty(Search))
                query = query.Where(x => x.Name.ToLower().Contains(Search.ToLower()) || x.Email.ToLower().Contains(Search.ToLower()));

            if (!string.IsNullOrEmpty(Name))
                query = query.Where(x => x.Name.ToLower().Contains(Name.ToLower()));

            if (!string.IsNullOrEmpty(Email))
                query = query.Where(x => x.Email.ToLower().Contains(Email.ToLower()));

            if (!string.IsNullOrEmpty(EmailEq))
                query = query.Where(x => x.Email.ToLower() == EmailEq.ToLower() );
            return query;
        }
    }
}
