using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RandomNeko.NekosMoe.Model;
using RestSharp;

namespace RandomNeko.NekosMoe
{
    class NekosMoeClient
    {
        private const string version = "v1";
        private static Uri BaseUri => new Uri($"https://nekos.moe/api/{version}/");
        private readonly RestClient c;
        public NekosMoeClient() {
            c = new RestClient(BaseUri);
        }
        public async Task<ImageList> GetRandomNekos(bool? nsfw = null, int count = 1) {
            if(count < 1 || count > 100) throw new ArgumentException("count must be in 1-100", nameof(count));
            var request = new RestRequest("/random/image");
            if(nsfw.HasValue) request.AddParameter(nameof(nsfw), nsfw);
            request.AddParameter(nameof(count), count);
            return await c.GetAsync<ImageList>(request);
        }

        public static string GetImageUrlFromId(string id) => $"https://nekos.moe/image/{id}.jpg";
    }
}
