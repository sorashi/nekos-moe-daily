using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace RandomNeko.Config
{
    internal interface IConfig
    {
        string TelegramBotToken { get; set; }
        string DiscordWebhook { get; set; }
        double NsfwChance { get; set; }
        public string DataDir { get; set; }
    }

    class Config : IConfig
    {
        public string TelegramBotToken { get; set; }
        public string DiscordWebhook { get; set; }
        public double NsfwChance { get; set; } = 0.2;
        public string DataDir { get; set; } = "data";

        public static Config Load(string path) {
            var deserializer = new DeserializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();
            using var sr = new StreamReader(path);
            return deserializer.Deserialize<Config>(sr);
        }
    }
}
