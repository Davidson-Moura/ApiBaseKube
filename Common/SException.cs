using Common.Messages;

namespace Common
{
    public class SException : Exception
    {
        public SException(Common.Messages.Messages msg) : base(SMessage.Message(msg))
        {
            Code = (int)msg;
        }
        public SException(Common.Messages.Messages msg, params string[] strs) : base(SMessage.Message(msg, strs))
        {
            Code = (int)msg;
        }
        public SException(Common.Messages.Messages msg, params Common.Messages.Messages[] strs) : base(SMessage.Message(msg, strs))
        {
            Code = (int)msg;
        }
        public int Code { get; set; }
    }
}
