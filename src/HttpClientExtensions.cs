namespace Apollo
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public static class HttpClientExtensions
    {
        public static async Task<T> GetAsync<T>(this HttpClient client, string url)
        {
            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}