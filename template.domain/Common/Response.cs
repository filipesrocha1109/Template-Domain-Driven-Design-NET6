namespace template.domain.Common
{
    public class Response
    {
        public bool Success { get; set; }
        public int Code { get; set; }
        public int Status { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
    }
}
