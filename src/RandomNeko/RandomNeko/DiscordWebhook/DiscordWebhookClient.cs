using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RandomNeko.NekosMoe;
using RandomNeko.NekosMoe.Model;
using RestSharp;

namespace RandomNeko.DiscordWebhook
{
    class DiscordWebhookClient : IPosterService
    {
        private readonly RestClient c;
        public DiscordWebhookClient(Uri webhook) {
            c = new RestClient(webhook);
        }
        public async Task PostAsync(Image neko) {
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            var sb = new StringBuilder();
            sb.AppendLine(NekosMoeClient.GetImageUrlFromId(neko.Id));
            sb.AppendLine(neko.TagsTruncated());
            sb.Append($"nsfw: **{(neko.Nsfw ? "yes" : "no")}**");
            request.AddJsonBody(new {content = sb.ToString()});
            await c.ExecutePostAsync(request);
        }
    }
}
