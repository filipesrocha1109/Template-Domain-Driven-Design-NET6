namespace template.domain.Common.HttpHandle.Response
{
    public class AuthResponse
    {
        public Token? Token { get; set; }
    }

    public class Token
    {
        public long Expires_in { get; set; }
        public string? Token_type { get; set; }
        public string? Access_token { get; set; }
    }
}
