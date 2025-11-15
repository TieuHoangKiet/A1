using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace OnlineShop.Web.Services
{
    public class ApiService
    {
        private readonly HttpClient _http;
        private readonly IHttpContextAccessor _httpContext;

        public ApiService(IHttpClientFactory factory, IHttpContextAccessor accessor)
        {
            _http = factory.CreateClient("api");
            _httpContext = accessor;
        }

        // 🔥 Tự động gắn token vào request
        private void AttachToken()
        {
            _http.DefaultRequestHeaders.Authorization = null; // RESET HEADER

            var token = _httpContext.HttpContext?.Session.GetString("JwtToken");
            Console.WriteLine("==== TOKEN SENT ====");
            Console.WriteLine(token);


            if (!string.IsNullOrWhiteSpace(token))
            {
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }


        // ===============================
        //  🔥 GET
        // ===============================
        public async Task<T?> GetAsync<T>(string url)
        {
            AttachToken();
            try
            {
                var res = await _http.GetAsync(url);
                if (!res.IsSuccessStatusCode)
                {
                    Console.WriteLine($"API GET {url} returned {(int)res.StatusCode} {res.ReasonPhrase}");
                    return default;
                }

                return await res.Content.ReadFromJsonAsync<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception when calling API GET {url}: {ex.Message}");
                return default;
            }
        }

        // ===============================
        //  🔥 POST
        // ===============================
        public async Task<T?> PostAsync<T, TRequest>(string url, TRequest data)
        {
            AttachToken();
            var res = await _http.PostAsJsonAsync(url, data);
            if (!res.IsSuccessStatusCode) return default;
            return await res.Content.ReadFromJsonAsync<T>();
        }

        // ===============================
        //  🔥 PUT
        // ===============================
        public async Task<bool> PutAsync<TRequest>(string url, TRequest data)
        {
            AttachToken();
            var res = await _http.PutAsJsonAsync(url, data);
            return res.IsSuccessStatusCode;
        }

        // ===============================
        //  🔥 DELETE
        // ===============================
        public async Task<bool> DeleteAsync(string url)
        {
            AttachToken();
            var res = await _http.DeleteAsync(url);
            return res.IsSuccessStatusCode;
        }
    }
}
