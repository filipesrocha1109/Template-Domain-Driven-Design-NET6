using template.domain.Enums;

namespace template.domain.Common
{
    public class ReturnValues
    {
        public bool Error { get; set; }

        public ReturnCodes Code { get; set; }

        public string? Message { get; set; }

        public void SetReturnValues(bool error, ReturnCodes code, string message)
        {
            this.Error = error;
            this.Code = code;
            this.Message = message;
        }
    }
}
