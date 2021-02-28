using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using RandomNeko.Config;
using RandomNeko.Database;
using RandomNeko.NekosMoe;
using RandomNeko.NekosMoe.Model;
using RandomNeko.Telegram.Model;
using RestSharp;

namespace RandomNeko.Telegram
{
    class TelegramClient : IPosterService
    {
        private readonly INekoDatabase db;
        private readonly RestClient c;
        public TelegramClient(INekoDatabase database, IConfig config) {
            db = database;
            var uri = new Uri("https://api.telegram.org/");
            uri = new Uri(uri, $"/bot{config.TelegramBotToken}");
            c = new RestClient(uri);
        }
        public async Task PostAsync(Image neko) {
            var updates = await GetMessageUpdates();
            while (updates.Any()) {
                var offset = await HandleUpdates(updates);
                updates = await GetMessageUpdates(offset);
            }

            string fileId = null;
            var sb = new StringBuilder();
            sb.AppendLine(neko.TagsTruncated());
            sb.AppendLine($"nsfw: *{(neko.Nsfw ? "yes" : "no")}*");
            var caption = sb.ToString();
            await foreach (var client in db.GetTelegramClientIdsAsync()) {
                if (fileId == null)
                    fileId = await SendPhoto(client, NekosMoeClient.GetImageUrlFromId(neko.Id), caption);
                else
                    await SendPhoto(client, fileId, caption);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Max ID of update handled + 1</returns>
        public async Task<ulong> HandleUpdates(IEnumerable<Update> updates) {
            var maxId = ulong.MinValue;
            foreach (Update update in updates) {
                maxId = Math.Max(update.UpdateId, maxId);
                if (update.Message.Text == "/stop") {
                    await db.RemoveTelegramClientAsync(update.Message.Chat.Id.ToString());
                    continue;
                }

                if(update.Message.Text != "/start") continue;
                await db.AddTelegramClientAsync(update.Message.Chat.Id.ToString());
                await SendMessageAsync(update.Message.Chat.Id.ToString(),
                    $"Thanks, {update.Message.Chat.Username}! I'll send you nekos every day at 20:00 CET. Type /stop to stop me.");
            }

            return ++maxId;
        }

        public async Task SendMessageAsync(string chatId, string text) {
            var request = new RestRequest("sendMessage");
            request.AddParameter("chat_id", chatId);
            request.AddParameter("text", text);

            await c.ExecutePostAsync(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="photoId"></param>
        /// <returns>Photo ID</returns>
        public async Task<string> SendPhoto(string chatId, string photoIdOrUrl, string caption) {
            var request = new RestRequest("sendPhoto");
            request.AddParameter("chat_id", chatId);
            request.AddParameter("photo", photoIdOrUrl);
            request.AddParameter("caption", caption);
            request.AddParameter("parse_mode", "MarkdownV2");
            var resp = await c.ExecutePostAsync(request);
            var payload = c.Deserialize<Payload<Message>>(resp).Data;
            if(!payload.Ok) throw new Exception($"Request unsuccessful: {payload.Description}");
            if(payload.Result.Photo == null) throw new NullReferenceException("Photo is null");
            return payload.Result.Photo.OrderByDescending(x => x.Width * x.Height).First().FileId;
        }

        public async Task<IEnumerable<Update>> GetMessageUpdates(ulong? offset = null) {
            var request = new RestRequest("getUpdates");
            if (offset.HasValue) request.AddParameter(nameof(offset), offset);
            request.AddParameter("allowed_updates", new [] {"message"});
            var payload = await c.GetAsync<Payload<List<Update>>>(request);
            if(!payload.Ok) throw new Exception("Request unsuccessful");
            return payload.Result;
        }
    }
}
