using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RandomNeko.DiscordWebhook;
using RandomNeko.NekosMoe;
using System.Linq;
using RandomNeko.Database;
using RandomNeko.NekosMoe.Model;
using RandomNeko.Telegram;

namespace RandomNeko
{
    class Program
    {
        static readonly List<IPosterService> posters = new List<IPosterService>();

        private static async Task Main(string[] args) {
            var config = Config.Config.Load(Environment.GetEnvironmentVariable("NEKO_USE_TEST_CONFIG") == "true"
                ? "config.test.yml"
                : "config.yml");

            using var database = new TextFileDatabase(config.DataDir);
            await database.ConnectAsync();
            if (!string.IsNullOrWhiteSpace(config.DiscordWebhook))
                posters.Add(new DiscordWebhookClient(new Uri(config.DiscordWebhook)));
            if(!string.IsNullOrWhiteSpace(config.TelegramBotToken))
                posters.Add(new TelegramClient(database, config));

            var moeClient = new NekosMoeClient();
            var random = new Random();
            var nsfw = random.NextDouble() < config.NsfwChance;
            var neko = await GetRandomNekoThatHasNotBeenPostedYet(database, moeClient, nsfw);
            if (posters.Count == 0) return;
            await Task.WhenAll(posters.Select(x => TaskWithExceptionHandling(x.PostAsync(neko))));
            await database.PostNekoAsync(neko.Id);
        }

        private static async Task TaskWithExceptionHandling(Task t) {
            try {
                await t;
            }
            catch (Exception e) {
                await Console.Error.WriteLineAsync(e.Message);
            }
        }

        private static async Task<Image> GetRandomNekoThatHasNotBeenPostedYet(INekoDatabase database, NekosMoeClient client, bool nsfw) {
            while (true) {
                // TODO handle rate limiting
                var nekos = await client.GetRandomNekos(nsfw, 50);
                foreach (var neko in nekos.Images) {
                    if (await database.HasBeenPostedAsync(neko.Id)) continue;
                    return neko;
                }
            }
        }
    }
}
