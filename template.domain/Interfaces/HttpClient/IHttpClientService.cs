using template.domain.Common.HttpHandle.Request;

namespace template.domain.Interfaces.HttpClient
{
    public interface IHttpClientService
    {
        Task<HttpResponseMessage> GetWithoutParametersAsync(string url);

        Task<HttpResponseMessage> GetAsync(string url, List<HttpClientParams> paramsUrl);

        Task<HttpResponseMessage> PostAsync(string url, object value);
    }
}
