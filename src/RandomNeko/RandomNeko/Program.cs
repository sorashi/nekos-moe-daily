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
            var number = random.NextDouble();
            var nsfw = number < config.NsfwChance;
            Console.WriteLine($"NSFW rolled: {number}, configured chance {config.NsfwChance}, nsfw {nsfw}");
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

        private static async Task<Image> GetRandomNekoThatHasNotBeenPostedYet(INekoDatabase database, NekosMoeClient client, bool nsfw)
        {
            if (client == null) throw new ArgumentNullException($"{nameof(client)} cannot be null");
            if (database == null) throw new ArgumentNullException($"{nameof(database)} cannot be null");
            while (true) {
                var nekos = await client.GetRandomNekos(nsfw, 100);
                if (nekos.Data == null) {
                    await client.HandleRateLimitAsync(nekos);
                    continue;
                }
                foreach (var neko in nekos.Data.Images) {
                    if (await database.HasBeenPostedAsync(neko.Id)) continue;
                    return neko;
                }
            }
        }
    }
}
