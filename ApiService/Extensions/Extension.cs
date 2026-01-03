using ApiService.Models.Responses;
using Common;

namespace ApiService.Extensions
{
    public static class Extension
    {
        public static MessageResponse ToReponse(this Exception ex)
        {
            var code = -1;
            if (ex is SException sEx)
            {
                code = sEx.Code;
            }
            return new MessageResponse(){
                Code = code,
                Description = ex.Message,
            };
        }
    }
}
