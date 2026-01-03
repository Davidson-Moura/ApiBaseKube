using System.Globalization;
using System.Resources;

namespace Common.Messages
{
    public class SMessage
    {
        public static CultureInfo[] SupportedCultures = new[]
        {
            new CultureInfo("pt-BR"),
            new CultureInfo("en-US"),
            //new CultureInfo("es-ES")
        };
        public static string Message(Messages msg, params Messages[] strs) => Message(msg, strs.Select(x => Message(x.ToString()) ).ToArray() );
        public static string Message(Messages msg, params string[] strs) => Message(msg.ToString(), strs);
        public static string Message(Messages msg) => Message(msg.ToString());
        public static string Message(string msg, params string[] strs)
        {
            var value = Resource.GetString(msg);
            if (string.IsNullOrEmpty(value)) throw new Exception("Recurso de tradução não encontrado.");
            if (strs.Length > 0) return string.Format(value, strs);
            return value;
        }
        private static ResourceManager _resource;
        private static ResourceManager Resource
        {
            get
            {
                if (_resource is null) _resource = new ResourceManager("Common.Messages.Languages.Messages", typeof(SMessage).Assembly);
                return _resource;
            }
        }
    }
    public enum Messages
    {
        InvalidData,
        InvalidLogin,
        OperationSuccessfully,
        UninformedPassword,
        UninformedName,
        UninformedEmail,
        InvalidConnectionConfiguration,
        InvalidUser,
        InvalidPassword,
        InvalidServer,
        InvalidDatabaseName,
        EmailIsAlreadyInUse,
        NotFound,
        User,
        PasswordsDoNotMatch,
        InvalidCredentials
    }
}
