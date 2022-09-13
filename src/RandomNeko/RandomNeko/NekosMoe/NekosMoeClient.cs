using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RandomNeko.NekosMoe.Model;
using RestSharp;

namespace RandomNeko.NekosMoe
{
    class NekosResponse<T>
    {
        public T? Data { get; set; }
        public int? Limit { get; set; }
        public int? Remaining { get; set; }
        public DateTime? Reset { get; set; }
        public TimeSpan? RetryAfter { get; set; }
    }
    class NekosMoeClient
    {
        private const string version = "v1";
        private static Uri BaseUri => new Uri($"https://nekos.moe/api/{version}/");
        private readonly RestClient c;
        public NekosMoeClient() {
            c = new RestClient(BaseUri);
        }
        public Task<NekosResponse<ImageList>> GetRandomNekos(bool? nsfw = null, int count = 1) {
            if(count < 1 || count > 100) throw new ArgumentException("count must be in 1-100", nameof(count));
            var request = new RestRequest("/random/image");
            if(nsfw.HasValue) request.AddParameter(nameof(nsfw), nsfw.Value ? "true" : "false");
            request.AddParameter(nameof(count), count);
            return ExecuteGetAsync<ImageList>(request);
        }

        /// <summary>
        /// Given a <paramref name="response"/> with <c>Data == null</c>, tries to handle a rate limit. Uses Task.Delay to wait for a sufficient amount of time.
        /// </summary>
        public async Task HandleRateLimitAsync<T>(NekosResponse<T> response) {
            if (response.Data != null)
                throw new ArgumentException(
                    $"{nameof(response.Data)} is not null, there is no need to handle rate limit.");
            if (response.Remaining != 0)
                throw new NullReferenceException(
                    $"{nameof(response.Data)} is null, but it's not because of a rate limit.");
            if (response.RetryAfter == null)
                throw new NullReferenceException(
                    $"{nameof(response.RetryAfter)} is null, but we are rate limited. Can't know when to try again.");
            // continue after we are not rate limited plus 30ms to be sure
            await Task.Delay(response.RetryAfter.Value + TimeSpan.FromMilliseconds(30));
        }

        public static string GetImageUrlFromId(string id) => $"https://nekos.moe/image/{id}.jpg";

        private async Task<NekosResponse<T>> ExecuteGetAsync<T>(RestRequest request)
        {
            var response = await c.ExecuteGetAsync<T>(request);
            var nekosResponse = new NekosResponse<T> {
                Data = response.Data
            };
            foreach (var header in response.Headers) {
                switch (header.Name?.ToLower()) {
                    case "x-ratelimit-limit":
                        nekosResponse.Limit = int.Parse(header.Value as string ?? "");
                        break;
                    case "x-ratelimit-remaining":
                        nekosResponse.Remaining = int.Parse(header.Value as string ?? "");
                        break;
                    case "x-ratelimit-reset": {
                        var millis = double.Parse(header.Value as string ?? "");
                        nekosResponse.Reset = new DateTime(1970, 1, 1) + TimeSpan.FromMilliseconds(millis);
                        break;
                    }
                    case "retry-after": {
                        var millis = double.Parse(header.Value as string ?? "");
                        nekosResponse.RetryAfter = TimeSpan.FromMilliseconds(millis);
                        break;
                    }
                }
            }

            return nekosResponse;
        }
    }
}
