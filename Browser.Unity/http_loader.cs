using System.IO;
using System.Net.Http;

namespace H3ml.Browser
{
    public class http_loader
    {
        HttpClient _client = new HttpClient();

        public Stream load_file(string url) => _client.GetStreamAsync(url).Result;
    }
}