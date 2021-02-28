using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomNeko.Database
{
    class TextFileDatabase : INekoDatabase
    {
        // do not dispose streamwriters and readers, they dispose the underlaying stream
        // when sw or sr get GC'd, they aren't disposed, so it's chill

        private readonly string dir;
        private FileStream nekos, telegramClients;
        public TextFileDatabase(string dir) {
            this.dir = dir;
        }
        public Task ConnectAsync() {
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            nekos = File.Open(Path.Combine(dir, "nekos"), FileMode.OpenOrCreate);
            telegramClients = File.Open(Path.Combine(dir, "telegram"), FileMode.OpenOrCreate);
            return Task.CompletedTask;
        }

        public async Task<bool> HasBeenPostedAsync(string nekoId) {
            nekos.Seek(0, SeekOrigin.Begin);
            await foreach (var line in ReadLinesAsync(nekos))
                if (line == nekoId)
                    return true;
            return false;
        }

        private static async IAsyncEnumerable<string> ReadLinesAsync(FileStream fs) {
            var sr = new StreamReader(fs);
            string line;
            while ((line = await sr.ReadLineAsync()) != null)
                yield return line.Trim();
        } 

        public async Task PostNekoAsync(string nekoId) {
            nekos.Seek(0, SeekOrigin.End);
            var sw = new StreamWriter(nekos);
            await sw.WriteLineAsync(nekoId);
            await sw.FlushAsync();
        }

        public IAsyncEnumerable<string> GetTelegramClientIdsAsync() {
            telegramClients.Seek(0, SeekOrigin.Begin);
            return ReadLinesAsync(telegramClients);
        }
        public async Task<bool> AddTelegramClientAsync(string clientId) {
            telegramClients.Seek(0, SeekOrigin.End);
            await foreach (var client in GetTelegramClientIdsAsync())
                if (client == clientId)
                    return false;
            var sw = new StreamWriter(telegramClients);
            await sw.WriteLineAsync(clientId);
            await sw.FlushAsync();
            return true;
        }

        public async Task<bool> RemoveTelegramClientAsync(string clientId) {
            bool found = false;
            var list = new List<string>();
            await foreach (var line in ReadLinesAsync(telegramClients)) {
                if (line.Trim() != clientId.Trim()) list.Add(clientId);
                else {
                    found = true;
                    break;
                }
            }

            telegramClients.SetLength(0);
            telegramClients.Seek(0, SeekOrigin.Begin);
            var sw = new StreamWriter(telegramClients);
            foreach (string client in list) await sw.WriteLineAsync(client);
            await sw.FlushAsync();
            return found;
        }

        public void Dispose() {
            nekos?.Dispose();
            telegramClients?.Dispose();
        }
    }
}
